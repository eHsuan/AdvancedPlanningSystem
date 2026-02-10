using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdvancedPlanningSystem
{
    public class ScanEventArgs : EventArgs
    {
        public string PortID { get; private set; }
        public string Barcode { get; private set; }

        public ScanEventArgs(string portID, string barcode)
        {
            PortID = portID;
            Barcode = barcode;
        }
    }

    public class PickEventArgs : EventArgs
    {
        public string PortID { get; private set; }

        public PickEventArgs(string portID)
        {
            PortID = portID;
        }
    }

    public class EnterEqpEventArgs : EventArgs
    {
        public string EqpID { get; private set; }

        public EnterEqpEventArgs(string eqpID)
        {
            EqpID = eqpID;
        }
    }

    /// <summary>
    /// 負責與硬體模擬器通訊的 TCP Server 模組。
    /// 支援單一客戶端連線，自動處理斷線重連與訊息解析。
    /// </summary>
    public class TcpServerModule : ITcpServerModule
    {
        private TcpListener _listener;
        private TcpClient _currentClient;
        private NetworkStream _clientStream;
        private CancellationTokenSource _cts;
        private SynchronizationContext _uiContext;
        private bool _isRunning = false;

        // 事件定義
        public event EventHandler<ScanEventArgs> OnScan;
        public event EventHandler<PickEventArgs> OnPick;
        public event EventHandler<EnterEqpEventArgs> OnEnterEqp; // 新增：進入機台事件
        public event EventHandler OnQueryEmptyPorts; // 新增：查詢空 Port 事件
        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;

        // 最後心跳時間
        public DateTime LastHeartbeat { get; private set; }

        public TcpServerModule()
        {
            // 捕捉當前的 SynchronizationContext (通常是 UI 執行緒)
            // 這樣觸發事件時可以直接封送到 UI 執行緒，避免跨執行緒操作例外
            _uiContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// 啟動 TCP Server 監聽
        /// </summary>
        /// <param name="port">監聽埠號 (預設 5000)</param>
        public virtual async void Start(int port = 5000)
        {
            if (_isRunning) return;

            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            _isRunning = true;
            _cts = new CancellationTokenSource();

            // 在背景執行監聽迴圈
            try
            {
                await ListenLoopAsync(_cts.Token);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Server Listen Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 停止伺服器
        /// </summary>
        public virtual void Stop()
        {
            _isRunning = false;
            _cts?.Cancel();
            _listener?.Stop();
            DisconnectCurrentClient();
        }

        /// <summary>
        /// 發送指令給模擬器
        /// </summary>
        /// <param name="cmd">指令內容 (e.g., "OPEN,P01")</param>
        public virtual async Task SendCommand(string cmd)
        {
            if (_currentClient == null || !_currentClient.Connected || _clientStream == null)
            {
                return;
            }

            try
            {
                // 確保結尾有分號
                if (!cmd.EndsWith(";")) cmd += ";";

                byte[] data = Encoding.UTF8.GetBytes(cmd);
                await _clientStream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send Error: {ex.Message}");
                DisconnectCurrentClient();
            }
        }

        private async Task ListenLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _isRunning)
            {
                try
                {
                    // 等待客戶端連線
                    TcpClient newClient = await _listener.AcceptTcpClientAsync();

                    // 處理新連線 (踢掉舊連線)
                    DisconnectCurrentClient();
                    
                    _currentClient = newClient;
                    _clientStream = _currentClient.GetStream();

                    // 觸發連線事件
                    RaiseEvent(OnConnected, EventArgs.Empty);

                    // 開始接收資料 (不等待，讓 ListenLoop 繼續運作以便未來處理重連邏輯? 
                    // 需求是單一客戶端，所以這裡我們可以用 Fire-and-forget 或是直接在這裡 await
                    // 但為了即時響應，我們開一個 Task 去讀取
                    _ = HandleClientAsync(_currentClient, token);
                }
                catch (ObjectDisposedException)
                {
                    // Listener stopped
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Accept Error: {ex.Message}");
                    // 避免無窮迴圈佔用 CPU
                    await Task.Delay(1000); 
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            byte[] buffer = new byte[1024];
            StringBuilder messageBuffer = new StringBuilder();

            try
            {
                while (client.Connected && !token.IsCancellationRequested)
                {
                    int bytesRead = await client.GetStream().ReadAsync(buffer, 0, buffer.Length, token);
                    
                    if (bytesRead == 0)
                    {
                        // 客戶端斷線
                        break;
                    }

                    string receivedChunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    messageBuffer.Append(receivedChunk);

                    // 處理完整訊息 (以 ; 分隔)
                    ProcessBuffer(messageBuffer);
                }
            }
            catch (Exception)
            {
                // Read error
            }
            finally
            {
                // 只有當此 client 還是目前的 client 時才觸發斷線 (避免新連線已經取代舊連線的情況)
                if (_currentClient == client)
                {
                    DisconnectCurrentClient();
                }
            }
        }

        private void ProcessBuffer(StringBuilder buffer)
        {
            string content = buffer.ToString();
            int delimiterIndex;

            while ((delimiterIndex = content.IndexOf(';')) != -1)
            {
                string message = content.Substring(0, delimiterIndex).Trim();
                
                // 移除已處理的部分
                content = content.Substring(delimiterIndex + 1);
                
                // 解析指令
                ParseMessage(message);
            }

            // 更新 Buffer 為剩餘部分
            buffer.Clear();
            buffer.Append(content);
        }

        private void ParseMessage(string message)
        {
            // 格式範例: COMMAND,ARG1,ARG2
            string[] parts = message.Split(',');
            if (parts.Length == 0) return;

            string cmd = parts[0].ToUpper();

            switch (cmd)
            {
                case "IN":
                case "SCAN":
                    // 支援兩種格式：
                    // 1. IN,PortID,Barcode (手動指定)
                    // 2. IN,Barcode (自動分配)
                    if (parts.Length >= 3)
                    {
                        string portId = parts[1];
                        string barcode = parts[2];
                        RaiseEvent(OnScan, new ScanEventArgs(portId, barcode));
                    }
                    else if (parts.Length == 2)
                    {
                        string barcode = parts[1];
                        RaiseEvent(OnScan, new ScanEventArgs(null, barcode)); // PortID 傳 null 表示自動分配
                    }
                    break;

                case "PICK":
                    // PICK,PortID
                    if (parts.Length >= 2)
                    {
                        string portId = parts[1];
                        RaiseEvent(OnPick, new PickEventArgs(portId));
                    }
                    break;

                case "ENTER":
                    // ENTER,EqpID
                    if (parts.Length >= 2)
                    {
                        string eqpId = parts[1];
                        RaiseEvent(OnEnterEqp, new EnterEqpEventArgs(eqpId));
                    }
                    break;

                case "HEART":
                    // 心跳包
                    LastHeartbeat = DateTime.Now;
                    break;

                case "GET_EMPTY_PORTS":
                    // 查詢空 Port
                    RaiseEvent(OnQueryEmptyPorts, EventArgs.Empty);
                    break;

                default:
                    Console.WriteLine($"Unknown Command: {message}");
                    break;
            }
        }

        private void DisconnectCurrentClient()
        {
            if (_currentClient != null)
            {
                try
                {
                    _currentClient.Close();
                }
                catch { }
                _currentClient = null;
                _clientStream = null;

                RaiseEvent(OnDisconnected, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 安全地觸發事件 (自動封送到 UI 執行緒)
        /// </summary>
        private void RaiseEvent<T>(EventHandler<T> handler, T args) where T : EventArgs
        {
            if (handler == null) return;

            if (_uiContext != null)
            {
                _uiContext.Post(s => handler(this, args), null);
            }
            else
            {
                handler(this, args);
            }
        }

        private void RaiseEvent(EventHandler handler, EventArgs args)
        {
            if (handler == null) return;

            if (_uiContext != null)
            {
                _uiContext.Post(s => handler(this, args), null);
            }
            else
            {
                handler(this, args);
            }
        }
    }
}

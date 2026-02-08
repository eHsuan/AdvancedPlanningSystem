using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APSSimulator.Client
{
    public class ApsClient
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private string _host = "127.0.0.1";
        private int _port = 5000;
        private bool _isRunning;
        private CancellationTokenSource _cts;

        public event Action<bool> OnConnectionChanged;
        public event Action<string> OnLog;
        public event Action<string> OnMessageReceived;
        public event Action<string, string> OnPortAssigned; // 新增：接收到分配儲位的事件 (PortId, CstId)

        public bool IsConnected => _client != null && _client.Connected;

        public void Start(string host, int port)
        {
            if (_isRunning) return;
            _host = host;
            _port = port;
            _isRunning = true;
            _cts = new CancellationTokenSource();
            Task.Run(() => ConnectLoop(_cts.Token));
            // Heartbeat moved to separate logic if needed, but simple loop inside Connect is safer
        }

        public void Stop()
        {
            _isRunning = false;
            _cts?.Cancel();
            Disconnect();
        }

        private void Disconnect()
        {
            try { _client?.Close(); } catch { }
            _client = null;
            OnConnectionChanged?.Invoke(false);
        }

        private async Task ConnectLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _isRunning)
            {
                if (!IsConnected)
                {
                    try
                    {
                        _client = new TcpClient();
                        await _client.ConnectAsync(_host, _port);
                        _stream = _client.GetStream();
                        OnConnectionChanged?.Invoke(true);
                        Log($"Connected to APS at {_host}:{_port}");

                        // Start Reader and Heartbeat
                        _ = ReadLoop(token);
                        _ = HeartbeatLoop(token);
                    }
                    catch
                    {
                        await Task.Delay(3000, token);
                    }
                }
                else
                {
                    await Task.Delay(1000, token);
                }
            }
        }

        private async Task ReadLoop(CancellationToken token)
        {
            byte[] buffer = new byte[1024];
            StringBuilder sb = new StringBuilder();

            try
            {
                while (IsConnected && !token.IsCancellationRequested)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (bytesRead == 0) break; // Disconnected

                    string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    sb.Append(chunk);

                    // Process messages delimited by ';'
                    string content = sb.ToString();
                    if (content.Contains(";"))
                    {
                        string[] msgs = content.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        // If last char is not ';', the last part is incomplete
                        bool incomplete = !content.EndsWith(";");
                        int count = incomplete ? msgs.Length - 1 : msgs.Length;

                        for (int i = 0; i < count; i++)
                        {
                            string msg = msgs[i].Trim();
                            if (!string.IsNullOrEmpty(msg))
                            {
                                if (msg.StartsWith("ASSIGNED_PORT,"))
                                {
                                    var parts = msg.Split(',');
                                    if (parts.Length >= 3)
                                    {
                                        OnPortAssigned?.Invoke(parts[1], parts[2]);
                                    }
                                }
                                else
                                {
                                    Log($"Received: {msg}");
                                    OnMessageReceived?.Invoke(msg);
                                }
                            }
                        }

                        sb.Clear();
                        if (incomplete) sb.Append(msgs[msgs.Length - 1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Read Error: {ex.Message}");
            }
            finally
            {
                Disconnect();
            }
        }

        private async Task HeartbeatLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _isRunning)
            {
                if (IsConnected)
                {
                    await SendCommandAsync("HEART;");
                }
                await Task.Delay(5000, token);
            }
        }

        public async Task SendCommandAsync(string command)
        {
            if (!IsConnected) return;
            try
            {
                if (!command.EndsWith(";")) command += ";";
                byte[] data = Encoding.UTF8.GetBytes(command);
                await _stream.WriteAsync(data, 0, data.Length);
                
                // Skip logging for heartbeat messages
                if (!command.Contains("HEART"))
                {
                    Log($"Sent: {command.Trim()}");
                }
            }
            catch (Exception ex)
            {
                Log($"Send Error: {ex.Message}");
                Disconnect();
            }
        }

        public async Task ScanAsync(string portId, string barcode)
        {
            await SendCommandAsync($"SCAN,{portId},{barcode};");
        }

        public async Task PickAsync(string portId)
        {
            await SendCommandAsync($"PICK,{portId};");
        }

        private void Log(string msg)
        {
            OnLog?.Invoke(msg);
        }
    }
}

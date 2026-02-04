using System;
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

        public bool IsConnected => _client != null && _client.Connected;

        public void Start(string host, int port)
        {
            if (_isRunning) return;
            _host = host;
            _port = port;
            _isRunning = true;
            _cts = new CancellationTokenSource();
            Task.Run(() => ConnectLoop(_cts.Token));
            Task.Run(() => HeartbeatLoop(_cts.Token));
        }

        public void Stop()
        {
            _isRunning = false;
            _cts?.Cancel();
            Disconnect();
        }

        private void Disconnect()
        {
            _client?.Close();
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
                    }
                    catch
                    {
                        // Log("Connection failed, retrying in 3s...");
                        await Task.Delay(3000, token);
                    }
                }
                else
                {
                    await Task.Delay(1000, token);
                }
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

using System;
using System.Threading.Tasks;

namespace Protocol.Drivers.Mitsubishi
{
    public interface IPlcDriver : IDisposable
    {
        bool IsConnected { get; }
        Task<bool> ConnectAsync(string ip, int port);
        void Disconnect();

        // 基本讀寫 (ushort 陣列)
        Task<ushort[]> ReadAsync(string address, ushort length);
        Task WriteAsync(string address, ushort[] data);

        // 輔助方法：讀寫 ASCII 字串
        Task<string> ReadAsciiAsync(string address, ushort length);
        Task WriteAsciiAsync(string address, string text, ushort length);

        // 輔助方法：讀寫單一 Bit (針對 ROLLING/BIT 交握)
        Task<bool> ReadBitAsync(string address, int bitOffset = 0);
        Task WriteBitAsync(string address, bool value, int bitOffset = 0);
        
        // 輔助方法：讀寫單一 Word
        Task<ushort> ReadWordAsync(string address);
        Task WriteWordAsync(string address, ushort value);
    }
}

using System.Threading.Tasks;

namespace AdvancedPlanningSystem
{
    public interface ITcpServerModule
    {
        Task SendCommand(string message);
        void Start(int port);
        void Stop();
    }
}

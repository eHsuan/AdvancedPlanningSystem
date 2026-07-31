using System.Collections.Generic;
using AdvancedPlanningSystem.Models;

namespace AdvancedPlanningSystem.Repositories
{
    public interface IApsLocalDbRepository
    {
        List<ConfigStepEqp> GetStepEqpMappings();
        List<StateBinding> GetSortedWaitBindings();
        List<StateBinding> GetAllBindings();
        List<StateTransit> GetAllTransits();
        List<ConfigQTime> GetQTimeConfigs();
        ConfigEqp GetEqpConfig(string eqpId);
        void InsertBinding(StateBinding binding);
        StateBinding GetBinding(string carrierId);
        List<StatePort> GetActivePorts();
        List<StatePort> GetActiveAndReservedPorts();
        void PreAssignPort(string portId, string carrierId, string lotId);
        void ConfirmPortArrival(string portId);
        StateBinding GetBindingByPort(string portId);
        void CancelPreAssignPort(string portId);
        void UpdatePortStateOnly(string portId, string status);
        void MoveToTransit(StateTransit transit);
        void RemoveTransit(string carrierId);
        void UpdateEqpMaxWip(string eqpId, int maxWip);
    }
}

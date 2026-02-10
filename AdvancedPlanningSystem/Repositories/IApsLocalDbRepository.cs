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
        void MoveToTransit(StateTransit transit);
        void RemoveTransit(string carrierId);
        void UpdateEqpMaxWip(string eqpId, int maxWip);
    }
}

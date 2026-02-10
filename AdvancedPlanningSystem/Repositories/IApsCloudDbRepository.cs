namespace AdvancedPlanningSystem.Repositories
{
    public interface IApsCloudDbRepository
    {
        void InsertGenericLog(string carrierId, string lotId, string message);
    }
}

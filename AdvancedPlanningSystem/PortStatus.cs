namespace AdvancedPlanningSystem
{
    public enum PortStatus
    {
        Empty,          // 空
        Occupied,       // 有貨 (UI: WAIT)
        Dispatching,    // 派貨中 (UI: MOVE)
        Finish,         // 完工 (UI: DONE)
        Error           // 異常
    }
}
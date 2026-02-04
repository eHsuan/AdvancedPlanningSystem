using System.Collections.Generic;

namespace AdvancedPlanningSystem
{
    public static class MockDataService
    {
        public static List<LeaderboardItem> GetMockData()
        {
            List<LeaderboardItem> data = new List<LeaderboardItem>();

            // Rank 1-4: 派貨中 (Dispatching)
            data.Add(new LeaderboardItem(1, "P05", "CASS-999", "WO-999", "EQP-01", "急件 (Urgent)", 150500, "派貨中"));
            data.Add(new LeaderboardItem(2, "P12", "CASS-123", "WO-123", "EQP-01", "急件 (Urgent)", 110000, "派貨中"));
            data.Add(new LeaderboardItem(3, "P08", "CASS-888", "WO-888", "EQP-02", "工程 (Eng)", 55200, "派貨中"));
            data.Add(new LeaderboardItem(4, "P22", "CASS-777", "WO-777", "EQP-01", "一般 (Normal)", 51000, "派貨中"));
            
            // Rank 5-10: 等待中 (Occupied)
            data.Add(new LeaderboardItem(5, "P30", "CASS-401", "WO-401", "EQP-02", "一般 (Normal)", 48000, "等待中"));
            data.Add(new LeaderboardItem(6, "P31", "CASS-402", "WO-402", "EQP-02", "一般 (Normal)", 47500, "等待中"));
            data.Add(new LeaderboardItem(7, "P45", "CASS-505", "WO-505", "EQP-01", "一般 (Normal)", 42000, "等待中"));
            data.Add(new LeaderboardItem(8, "P60", "CASS-601", "WO-601", "EQP-01", "一般 (Normal)", 39000, "等待中"));
            data.Add(new LeaderboardItem(9, "P11", "CASS-111", "WO-111", "EQP-02", "一般 (Normal)", 35000, "等待中"));
            data.Add(new LeaderboardItem(10, "P02", "CASS-002", "WO-002", "EQP-02", "一般 (Normal)", 31000, "等待中"));

            return data;
        }
    }
}

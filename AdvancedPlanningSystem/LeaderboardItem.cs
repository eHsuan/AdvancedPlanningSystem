namespace AdvancedPlanningSystem
{
    public class LeaderboardItem
    {
        public int Rank { get; set; }
        public string Port { get; set; }
        public string CassetteID { get; set; }
        public string WorkNo { get; set; }
        public string TargetEQP { get; set; }
        public string Priority { get; set; }
        public int TotalScore { get; set; }
        public string Status { get; set; }

        public LeaderboardItem(int rank, string port, string cassetteID, string workNo, string targetEQP, string priority, int totalScore, string status)
        {
            Rank = rank;
            Port = port;
            CassetteID = cassetteID;
            WorkNo = workNo;
            TargetEQP = targetEQP;
            Priority = priority;
            TotalScore = totalScore;
            Status = status;
        }
    }
}

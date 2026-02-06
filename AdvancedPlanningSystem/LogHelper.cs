using log4net;

namespace AdvancedPlanningSystem
{
    public static class LogHelper
    {
        // 預設 Logger (導向 System.log)
        private static readonly ILog _logger = LogManager.GetLogger("System");
        
        // 分類 Logger
        private static readonly ILog _mesLogger = LogManager.GetLogger("MES");
        private static readonly ILog _dispatchLogger = LogManager.GetLogger("Dispatch");
        private static readonly ILog _scoreLogger = LogManager.GetLogger("Score");

        public static ILog Logger => _logger;
        public static ILog System => _logger;
        public static ILog MES => _mesLogger;
        public static ILog Dispatch => _dispatchLogger;
        public static ILog Score => _scoreLogger;
    }
}
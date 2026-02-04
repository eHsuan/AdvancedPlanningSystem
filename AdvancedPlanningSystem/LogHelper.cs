using log4net;

namespace AdvancedPlanningSystem
{
    public static class LogHelper
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ILog Logger => _logger;
    }
}

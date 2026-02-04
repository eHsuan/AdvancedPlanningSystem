using System;
using log4net.Appender;
using log4net.Core;

namespace APSSimulator
{
    public class UiLogAppender : AppenderSkeleton
    {
        public static Action<string> LogReceived;

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (LogReceived != null)
            {
                string message = RenderLoggingEvent(loggingEvent);
                LogReceived.Invoke(message);
            }
        }
    }
}

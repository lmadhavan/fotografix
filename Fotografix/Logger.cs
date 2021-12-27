using Microsoft.Services.Store.Engagement;
using System.Diagnostics;

namespace Fotografix
{
    public static class Logger
    {
        private static readonly StoreServicesCustomEventLogger StoreLogger = StoreServicesCustomEventLogger.GetDefault();

        [Conditional("RELEASE")]
        public static void LogEvent(string eventName)
        {
            StoreLogger.Log(eventName);
        }
    }
}

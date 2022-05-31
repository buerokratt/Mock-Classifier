namespace MockClassifier.Api.Services.Dmr.Extensions
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, string, string, Exception> _dmrCallback =
            LoggerMessage.Define<string, string>(
                LogLevel.Information,
                new EventId(1, "DmrCallbackPosted"),
                "Callback to DMR with classification = '{Classification}', message = '{Message}'");

        private static readonly Action<ILogger, Exception> _dmrCallbackFailed =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(2, "DmrCallbackFailed"),
                "Callback to DMR failed");

        public static void DmrCallback(this ILogger logger, string classification, string message)
        {
            _dmrCallback(logger, classification, message, null);
        }

        public static void DmrCallbackFailed(this ILogger logger, Exception exception)
        {
            _dmrCallbackFailed(logger, exception);
        }
    }
}

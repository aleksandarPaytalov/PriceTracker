namespace PriceTracker.Infrastructure.Constants
{
	internal static class LoggerConstants
	{
		// File names
		internal const string ErrorLogFile = "errors.log";
		internal const string WarningLogFile = "warnings.log";
		internal const string InfoLogFile = "info.log";

		// Log level prefixes
		internal const string ErrorPrefix = "[ERROR]";
		internal const string WarningPrefix = "[WARNING]";
		internal const string InfoPrefix = "[INFO]";

		// Messages
		internal const string ExceptionTypeFormat = "Exception Type: {0}";
		internal const string ExceptionMessageFormat = "Exception Message: {0}";
		internal const string StackTraceFormat = "StackTrace: {0}";
		internal const string InnerExceptionTypeFormat = "Inner Exception Type: {0}";
		internal const string InnerExceptionMessageFormat = "Inner Exception Message: {0}";
		internal const string InnerStackTraceFormat = "Inner StackTrace: {0}";
		internal const string MessagePrefix = "Message: {0}";
		internal const string Separator = "----------------------------------------";

		// Error messages
		internal const string FailedToWriteLogFormat = "Failed to write to log file: {0}";
		internal const string OriginalMessageFormat = "Original message: {0}";

		// Date formats
		internal const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
		internal const string DateOnlyFormat = "yyyy-MM-dd";
	}
}

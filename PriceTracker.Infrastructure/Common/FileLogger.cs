using PriceTracker.Infrastructure.Constants;
using System.Text;

namespace PriceTracker.Infrastructure.Common
{
	public class FileLogger : ILogger
	{
		private readonly string _logPath;
		private readonly object _lock = new object();
		private readonly bool _logToConsole;

		public FileLogger(string? logPath = null, bool logToConsole = true)
		{
			_logPath = logPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
			_logToConsole = logToConsole;
			EnsureLogDirectoryExists();
		}

		public void LogError(string message, Exception? ex = null)
		{
			var logMessage = new StringBuilder();
			logMessage.AppendLine(LoggerConstants.Separator);
			logMessage.AppendLine($"{LoggerConstants.ErrorPrefix} {DateTime.UtcNow.ToString(LoggerConstants.DateTimeFormat)}");
			logMessage.AppendLine(string.Format(LoggerConstants.MessagePrefix, message));

			if (ex != null)
			{
				logMessage.AppendLine(string.Format(LoggerConstants.ExceptionTypeFormat, ex.GetType().Name));
				logMessage.AppendLine(string.Format(LoggerConstants.ExceptionMessageFormat, ex.Message));
				logMessage.AppendLine(string.Format(LoggerConstants.StackTraceFormat, ex.StackTrace));

				if (ex.InnerException != null)
				{
					logMessage.AppendLine(string.Format(LoggerConstants.InnerExceptionTypeFormat, ex.InnerException.GetType().Name));
					logMessage.AppendLine(string.Format(LoggerConstants.InnerExceptionMessageFormat, ex.InnerException.Message));
					logMessage.AppendLine(string.Format(LoggerConstants.InnerStackTraceFormat, ex.InnerException.StackTrace));
				}
			}
			logMessage.AppendLine(LoggerConstants.Separator);

			WriteToFile(LoggerConstants.ErrorLogFile, logMessage.ToString());

			if (_logToConsole)
			{
				WriteToConsole(logMessage.ToString(), ConsoleColor.Red);
			}
		}
		public void LogWarning(string message)
		{
			var logMessage = $"{LoggerConstants.WarningPrefix} {DateTime.UtcNow.ToString(LoggerConstants.DateTimeFormat)}{Environment.NewLine}{message}";
			WriteToFile(LoggerConstants.WarningLogFile, logMessage);

			if (_logToConsole)
			{
				WriteToConsole(logMessage, ConsoleColor.Yellow);
			}
		}

		public void LogInformation(string message)
		{
			var logMessage = $"{LoggerConstants.InfoPrefix} {DateTime.UtcNow.ToString(LoggerConstants.DateTimeFormat)}{Environment.NewLine}{message}";
			WriteToFile(LoggerConstants.InfoLogFile, logMessage);

			if (_logToConsole)
			{
				WriteToConsole(logMessage, ConsoleColor.Blue);
			}
		}

		private void WriteToFile(string fileName, string message)
		{
			var datePrefix = DateTime.UtcNow.ToString(LoggerConstants.DateOnlyFormat);
			var fullFileName = Path.Combine(_logPath, $"{datePrefix}_{fileName}");

			lock (_lock)
			{
				try
				{
					File.AppendAllText(fullFileName, message + Environment.NewLine + Environment.NewLine);
				}
				catch (Exception ex)
				{
					if (_logToConsole)
					{
						var errorMessage = string.Format(LoggerConstants.FailedToWriteLogFormat, ex.Message);
						var originalMessage = string.Format(LoggerConstants.OriginalMessageFormat, message);
						WriteToConsole(errorMessage + Environment.NewLine + originalMessage, ConsoleColor.Red);
					}
				}
			}
		}

		private void WriteToConsole(string message, ConsoleColor color)
		{
			var originalColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(message);
			Console.ForegroundColor = originalColor;
		}

		private void EnsureLogDirectoryExists()
		{
			if (!Directory.Exists(_logPath))
			{
				Directory.CreateDirectory(_logPath);
			}
		}

		public string GetLogsPath() => _logPath;
	}
}
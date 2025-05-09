namespace PriceTracker.Infrastructure.Common
{
	public interface ILogger
	{
		void LogError(string message, Exception? ex = null);
		void LogWarning(string message);
		void LogInformation(string message);
	}
}

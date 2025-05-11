namespace PriceTracker.Infrastructure.Common
{
	public interface IAppLogger
	{
		void LogError(string message, Exception? ex = null);
		void LogWarning(string message);
		void LogInformation(string message);
	}
}

namespace PriceTracker.Core.Models.LandingPage
{
	/// <summary>
	/// ViewModel for the About page containing app and developer information
	/// </summary>
	public class AboutViewModel
	{
		public AboutAppInformationModel AppInfo { get; set; } = new();
		public AboutDeveloperInformationModel DeveloperInfo { get; set; } = new();
	}
}

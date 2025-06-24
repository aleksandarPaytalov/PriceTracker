namespace PriceTracker.Core.Models.LandingPage
{
	public class AboutAppInformationModel
	{
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public List<string> KeyFeatures { get; set; } = new();
		public List<string> WhyUseIt { get; set; } = new();
		public string TechnicalStack { get; set; } = string.Empty;
	}
}

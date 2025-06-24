namespace PriceTracker.Core.Models.LandingPage
{
	public class AboutDeveloperInformationModel
	{
		public string Name { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public List<string> Skills { get; set; } = new();
		public string GitHubUrl { get; set; } = string.Empty;
		public string Contact { get; set; } = string.Empty;
	}
}

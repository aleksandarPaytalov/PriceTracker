namespace PriceTracker.Core.Models.Email
{
	public class ContactFormEmailViewModel
	{
		public string Name { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Subject { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public DateTime SubmittedAt { get; set; } = DateTime.Now;
		public string IpAddress { get; set; } = string.Empty;
		public string UserAgent { get; set; } = string.Empty;
	}
}

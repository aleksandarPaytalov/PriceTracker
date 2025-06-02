using Microsoft.AspNetCore.Identity.UI.Services;
using PriceTracker.Core.Services;

namespace PriceTracker.Infrastructure.Services
{
	public class CustomEmailSender : IEmailSender
	{
		private readonly IEmailService _emailService;

		public CustomEmailSender(IEmailService emailService)
		{
			_emailService = emailService;
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			await _emailService.SendEmailAsync(email, subject, htmlMessage);
		}
	}
}
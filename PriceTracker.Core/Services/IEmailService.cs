using PriceTracker.Core.Models.Email;

namespace PriceTracker.Core.Services
{
	public interface IEmailService
	{
		Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
		Task SendEmailConfirmationAsync(string email, string callbackUrl);
		Task SendPasswordResetEmailAsync(string email, string callbackUrl);
		Task SendWelcomeEmailAsync(string email, string userName);
		public Task SendContactFormEmailAsync(ContactFormEmailViewModel contactForm);
	}
}
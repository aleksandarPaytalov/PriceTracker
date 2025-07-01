using PriceTracker.Core.Models.Email;

namespace PriceTracker.Core.Services
{
	public interface IEmailTemplateService
	{
		Task<string> RenderEmailConfirmationAsync(string callbackUrl);
		Task<string> RenderPasswordResetAsync(string callbackUrl);
		Task<string> RenderWelcomeAsync(string userName);
		Task<string> RenderContactFormAsync(ContactFormEmailViewModel model);
	}
}
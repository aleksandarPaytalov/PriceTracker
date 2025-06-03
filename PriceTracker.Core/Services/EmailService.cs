using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PriceTracker.Infrastructure.Common;
using PriceTracker.Infrastructure.Configuration;

namespace PriceTracker.Core.Services
{
	public class EmailService : IEmailService
	{
		private readonly EmailSettings _emailSettings;
		private readonly IAppLogger _logger;
		private readonly IEmailTemplateService _templateService;

		public EmailService(
			IOptions<EmailSettings> emailSettings,
			IAppLogger logger,
			IEmailTemplateService templateService)
		{
			_emailSettings = emailSettings.Value;
			_logger = logger;
			_templateService = templateService;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
		{
			try
			{
				var message = new MimeMessage();
				message.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.FromEmail));
				message.To.Add(new MailboxAddress("", toEmail));
				message.Subject = subject;

				var bodyBuilder = new BodyBuilder
				{
					HtmlBody = htmlMessage
				};
				message.Body = bodyBuilder.ToMessageBody();

				using var client = new SmtpClient();

				await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
				await client.AuthenticateAsync(_emailSettings.FromEmail, _emailSettings.Password);
				await client.SendAsync(message);
				await client.DisconnectAsync(true);

				_logger.LogInformation($"Email sent successfully to {toEmail} with subject: {subject}");
			}
			catch (Exception ex)
			{
				_logger.LogError($"Failed to send email to {toEmail}", ex);
				throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
			}
		}

		public async Task SendEmailConfirmationAsync(string email, string callbackUrl)
		{
			var subject = "Confirm your email - PriceTracker";
			var htmlMessage = await _templateService.RenderEmailConfirmationAsync(callbackUrl);
			await SendEmailAsync(email, subject, htmlMessage);
		}

		public async Task SendPasswordResetEmailAsync(string email, string callbackUrl)
		{
			var subject = "Reset your password - PriceTracker";
			var htmlMessage = await _templateService.RenderPasswordResetAsync(callbackUrl);
			await SendEmailAsync(email, subject, htmlMessage);
		}

		public async Task SendWelcomeEmailAsync(string email, string userName)
		{
			var subject = "Welcome to PriceTracker!";
			var htmlMessage = await _templateService.RenderWelcomeAsync(userName);
			await SendEmailAsync(email, subject, htmlMessage);
		}
	}
}
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

		public EmailService(IOptions<EmailSettings> emailSettings, IAppLogger logger)
		{
			_emailSettings = emailSettings.Value;
			_logger = logger;
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

				// Connect to SMTP server
				await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);

				// Authenticate
				await client.AuthenticateAsync(_emailSettings.FromEmail, _emailSettings.Password);

				// Send email
				await client.SendAsync(message);

				// Disconnect
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
			var htmlMessage = CreateEmailConfirmationTemplate(callbackUrl);
			await SendEmailAsync(email, subject, htmlMessage);
		}

		public async Task SendPasswordResetEmailAsync(string email, string callbackUrl)
		{
			var subject = "Reset your password - PriceTracker";
			var htmlMessage = CreatePasswordResetTemplate(callbackUrl);
			await SendEmailAsync(email, subject, htmlMessage);
		}

		public async Task SendWelcomeEmailAsync(string email, string userName)
		{
			var subject = "Welcome to PriceTracker!";
			var htmlMessage = CreateWelcomeTemplate(userName);
			await SendEmailAsync(email, subject, htmlMessage);
		}

		private static string CreateEmailConfirmationTemplate(string callbackUrl)
		{
			return $@"
                <html>
                <head>
                    <style>
                        .container {{ max-width: 600px; margin: 0 auto; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }}
                        .header {{ background: linear-gradient(135deg, #1b6ec2, #0056b3); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ padding: 30px; background-color: #ffffff; }}
                        .button {{ 
                            background: linear-gradient(135deg, #28a745, #20c997); 
                            color: white; 
                            padding: 15px 30px; 
                            text-decoration: none; 
                            border-radius: 6px; 
                            display: inline-block; 
                            font-weight: bold;
                            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                        }}
                        .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 14px; color: #666; border-radius: 0 0 8px 8px; }}
                        .link-box {{ background-color: #f8f9fa; padding: 15px; border-radius: 6px; word-break: break-all; margin: 20px 0; }}
                        .warning {{ color: #dc3545; font-weight: bold; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🎉 Welcome to PriceTracker!</h1>
                            <p>Your journey to smart price tracking starts here</p>
                        </div>
                        <div class='content'>
                            <h2>Email Confirmation Required</h2>
                            <p>Thank you for registering with PriceTracker! We're excited to have you on board.</p>
                            <p>To complete your registration and secure your account, please confirm your email address by clicking the button below:</p>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{callbackUrl}' class='button'>✉️ Confirm Email Address</a>
                            </div>
                            
                            <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
                            <div class='link-box'>
                                {callbackUrl}
                            </div>
                            
                            <p class='warning'>⚠️ This link will expire in 24 hours for security reasons.</p>
                            
                            <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                            
                            <h3>What's next?</h3>
                            <ul>
                                <li>✅ Confirm your email (you're doing this now!)</li>
                                <li>🏪 Add your favorite stores</li>
                                <li>🛍️ Track product prices</li>
                                <li>💰 Save money with price alerts</li>
                            </ul>
                        </div>
                        <div class='footer'>
                            <p>If you didn't create an account with PriceTracker, please ignore this email.</p>
                            <p>&copy; 2025 PriceTracker. All rights reserved.</p>
                            <p>📧 Need help? Contact us at support@pricetracker.com</p>
                        </div>
                    </div>
                </body>
                </html>";
		}

		private static string CreatePasswordResetTemplate(string callbackUrl)
		{
			return $@"
                <html>
                <head>
                    <style>
                        .container {{ max-width: 600px; margin: 0 auto; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }}
                        .header {{ background: linear-gradient(135deg, #dc3545, #c82333); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ padding: 30px; background-color: #ffffff; }}
                        .button {{ 
                            background: linear-gradient(135deg, #dc3545, #c82333); 
                            color: white; 
                            padding: 15px 30px; 
                            text-decoration: none; 
                            border-radius: 6px; 
                            display: inline-block; 
                            font-weight: bold;
                            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                        }}
                        .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 14px; color: #666; border-radius: 0 0 8px 8px; }}
                        .link-box {{ background-color: #f8f9fa; padding: 15px; border-radius: 6px; word-break: break-all; margin: 20px 0; }}
                        .warning {{ color: #dc3545; font-weight: bold; }}
                        .security-tips {{ background-color: #fff3cd; padding: 15px; border-radius: 6px; border-left: 4px solid #ffc107; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🔐 Password Reset Request</h1>
                            <p>Secure your PriceTracker account</p>
                        </div>
                        <div class='content'>
                            <h2>Reset Your Password</h2>
                            <p>We received a request to reset the password for your PriceTracker account.</p>
                            <p>If you made this request, click the button below to set a new password:</p>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{callbackUrl}' class='button'>🔑 Reset Password</a>
                            </div>
                            
                            <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
                            <div class='link-box'>
                                {callbackUrl}
                            </div>
                            
                            <p class='warning'>⏰ This link will expire in 1 hour for security reasons.</p>
                            
                            <div class='security-tips'>
                                <h3>🛡️ Security Tips:</h3>
                                <ul>
                                    <li>Use a strong, unique password</li>
                                    <li>Include uppercase, lowercase, numbers, and symbols</li>
                                    <li>Don't reuse passwords from other accounts</li>
                                    <li>Consider using a password manager</li>
                                </ul>
                            </div>
                        </div>
                        <div class='footer'>
                            <p><strong>If you didn't request a password reset, please ignore this email and your password will remain unchanged.</strong></p>
                            <p>&copy; 2025 PriceTracker. All rights reserved.</p>
                            <p>📧 Suspicious activity? Contact us immediately at security@pricetracker.com</p>
                        </div>
                    </div>
                </body>
                </html>";
		}

		private static string CreateWelcomeTemplate(string userName)
		{
			return $@"
                <html>
                <head>
                    <style>
                        .container {{ max-width: 600px; margin: 0 auto; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }}
                        .header {{ background: linear-gradient(135deg, #28a745, #20c997); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ padding: 30px; background-color: #ffffff; }}
                        .button {{ 
                            background: linear-gradient(135deg, #1b6ec2, #0056b3); 
                            color: white; 
                            padding: 15px 30px; 
                            text-decoration: none; 
                            border-radius: 6px; 
                            display: inline-block; 
                            font-weight: bold;
                            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                        }}
                        .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 14px; color: #666; border-radius: 0 0 8px 8px; }}
                        .feature-box {{ background-color: #f8f9fa; padding: 20px; border-radius: 6px; margin: 15px 0; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🎉 Welcome aboard, {userName}!</h1>
                            <p>Your account is now active and ready to use</p>
                        </div>
                        <div class='content'>
                            <h2>Thanks for joining PriceTracker!</h2>
                            <p>Your email has been successfully confirmed and your account is now fully activated.</p>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='https://your-domain.com' class='button'>🚀 Start Tracking Prices</a>
                            </div>
                            
                            <h3>🌟 What you can do now:</h3>
                            
                            <div class='feature-box'>
                                <h4>🏪 Add Stores</h4>
                                <p>Add your favorite stores and start tracking products from them.</p>
                            </div>
                            
                            <div class='feature-box'>
                                <h4>📊 Track Prices</h4>
                                <p>Monitor price changes and get notified when prices drop.</p>
                            </div>
                            
                            <div class='feature-box'>
                                <h4>💰 Manage Budget</h4>
                                <p>Set monthly budgets and track your expenses.</p>
                            </div>
                            
                            <div class='feature-box'>
                                <h4>📝 Create Tasks</h4>
                                <p>Organize your shopping tasks and get notifications.</p>
                            </div>
                        </div>
                        <div class='footer'>
                            <p>Need help getting started? Check out our <a href='#'>User Guide</a> or contact support.</p>
                            <p>&copy; 2025 PriceTracker. All rights reserved.</p>
                            <p>📧 Questions? Reach us at support@pricetracker.com</p>
                        </div>
                    </div>
                </body>
                </html>";
		}
	}
}
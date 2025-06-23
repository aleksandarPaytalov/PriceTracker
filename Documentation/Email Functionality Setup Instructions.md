# Email Functionality Setup Instructions
## PriceTracker ASP.NET Core 8 MVC Application

### Prerequisites
- Your PriceTracker application cloned from GitHub
- .NET 8 SDK installed
- Email service provider (Gmail, SendGrid, Outlook, etc.)

## Step 1: Restore NuGet Packages (REQUIRED)

After cloning the repository, you must restore the NuGet packages:

```bash
# Navigate to the PriceTracker project folder
cd PriceTracker

# Restore all NuGet packages
dotnet restore

# Alternative: Build the project (which includes restore)
dotnet build
```

**Required packages (automatically restored from .csproj):**
- Microsoft.AspNetCore.Authentication.Google (8.0.15)
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.15)
- Microsoft.AspNetCore.Identity.UI (8.0.15)
- Microsoft.EntityFrameworkCore.SqlServer (8.0.15)
- Plus other dependencies listed in PriceTracker.csproj

**Note:** Your `.csproj` file already contains all necessary package references, so no additional package installation is required after restore.

## Step 2: Initial Setup After Cloning Repository

After restoring packages, complete the initial setup:

```bash
# 1. Restore NuGet packages
dotnet restore

# 2. Set up User Secrets (required for email/Google auth)
dotnet user-secrets init

# 3. Apply database migrations (if needed)
dotnet ef database update

# 4. Verify the application builds
dotnet build

# 5. Test run the application
dotnet run
```

## Step 3: Configure Email Settings in appsettings.json

Your `appsettings.json` already has the email configuration section. Update these values:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",        // ← Update this
    "Port": 587,                           // ← Update if needed
    "FromEmail": "your-email@gmail.com",   // ← Replace with your email
    "Password": "your-app-password",       // ← Will be set in User Secrets
    "DisplayName": "PriceTracker Support", // ← Update if needed
    "EnableSsl": true                      // ← Keep as true for Gmail
  }
}
```

**Security Note:** Never commit passwords to source control. Use User Secrets for development:

```bash
# Set your email password in User Secrets (REQUIRED)
dotnet user-secrets set "EmailSettings:Password" "your-gmail-app-password"
dotnet user-secrets set "EmailSettings:FromEmail" "your-email@gmail.com"
```

## Step 3: Email Service Provider Setup

### Option A: Gmail SMTP (Recommended for Development)

1. **Enable 2-Factor Authentication** on your Google account
2. **Generate App Password:**
   - Go to Google Account Settings → Security → 2-Step Verification
   - Click "App passwords" at the bottom
   - Generate password for "Mail"
   - Use this 16-character password in your User Secrets

3. **Configuration Values for Gmail:**
   ```
   SmtpServer: smtp.gmail.com
   Port: 587
   EnableSsl: true
   FromEmail: your-gmail-address@gmail.com
   Password: your-16-character-app-password (in User Secrets)
   ```

### Option B: SendGrid (Recommended for Production)

1. **Create SendGrid Account:** https://sendgrid.com
2. **Get API Key:**
   - Go to Settings → API Keys
   - Create API Key with "Full Access"
3. **Configure for SendGrid:** (Update appsettings.json)
   ```json
   "EmailSettings": {
     "SmtpServer": "smtp.sendgrid.net",
     "Port": 587,
     "FromEmail": "noreply@yourdomain.com",
     "Password": "apikey", // Literal text "apikey"
     "DisplayName": "PriceTracker",
     "EnableSsl": true
   }
   ```
4. **Set API Key in User Secrets:**
   ```bash
   dotnet user-secrets set "EmailSettings:Password" "your-sendgrid-api-key"
   ```

## Step 5: Required User Secrets Configuration

Set these secrets for your application to work:

```bash
# For Gmail
dotnet user-secrets set "EmailSettings:Password" "your-gmail-app-password"
dotnet user-secrets set "EmailSettings:FromEmail" "your-email@gmail.com"

# For SendGrid
dotnet user-secrets set "EmailSettings:Password" "your-sendgrid-api-key"
dotnet user-secrets set "EmailSettings:FromEmail" "noreply@yourdomain.com"
```

## Step 6: Email Confirmation Setup (Critical for Your Application)

Your PriceTracker application **requires email confirmation** for new user registrations. This is already implemented in your code and configured in `Program.cs`:

```csharp
// Email confirmation required (already set in your app)
options.SignIn.RequireConfirmedAccount = true;
options.SignIn.RequireConfirmedEmail = true;
```

### Email Confirmation Flow in Your Application:

1. **User registers** → Email confirmation sent automatically
2. **User clicks link in email** → Account becomes active
3. **User can then login** → Full access granted

### Required Configuration for Email Confirmation:

**Your User Secrets MUST be configured for this to work:**

```bash
# Required - Set your email provider credentials
dotnet user-secrets set "EmailSettings:Password" "your-gmail-app-password"
dotnet user-secrets set "EmailSettings:FromEmail" "your-email@gmail.com"
```

### Complete Email Confirmation Test Procedure:

**Step-by-Step Testing:**

1. **Set up email credentials:**
   ```bash
   dotnet user-secrets set "EmailSettings:Password" "your-gmail-app-password"
   dotnet user-secrets set "EmailSettings:FromEmail" "your-email@gmail.com"
   ```

2. **Start application:**
   ```bash
   dotnet run
   ```

3. **Register new user:**
   - Navigate to: `https://localhost:7262/Identity/Account/Register`
   - Enter email address and password
   - Click "Register"
   - You should see: "Please check your email to confirm your account"

4. **Check email inbox:**
   - Look for email from "PriceTracker Support"
   - Subject: "Email Confirmation - PriceTracker"
   - If not in inbox, check spam folder

5. **Click confirmation link:**
   - Open email and click "Confirm Email Address" button
   - You should be redirected to: `/Identity/Account/ConfirmEmail`
   - You should see: "Thank you for confirming your email"
   - A welcome email should be automatically sent

6. **Test login:**
   - Go to: `https://localhost:7262/Identity/Account/Login`
   - Enter your credentials
   - Login should succeed

7. **Verify database:**
   - Check that user record exists in database
   - Verify `EmailConfirmed = true` in Users table

### Email Confirmation Failure Scenarios:

**If application fails to start:**
- Verify packages restored: `dotnet restore`
- Check build succeeds: `dotnet build`
- Ensure database is updated: `dotnet ef database update`

**If no email is received:**
- Verify User Secrets: `dotnet user-secrets list`
- Check application console for error messages
- Verify Gmail App Password is correct
- Try different email provider (Outlook, Yahoo)

**If "Email service not configured" error:**
- Ensure User Secrets are set: `dotnet user-secrets list`
- Verify EmailService is registered (already done in your app)
- Check that `dotnet restore` was completed successfully

**If confirmation link doesn't work:**
- Check that link contains valid token
- Verify application is running on HTTPS
- Check for expired tokens (24-hour limit)
- Try resending confirmation email

**If welcome email isn't sent after confirmation:**
- Check `ConfirmEmail.cshtml.cs` - welcome email is sent there
- Verify EmailService is properly registered
- Check logs for any email service errors

**If database errors occur:**
- Ensure Entity Framework packages are restored
- Run: `dotnet ef database update`
- Check connection string in appsettings.json

### Email Confirmation Templates (Already Implemented):

Your app includes these professional email templates:
- **Registration Confirmation:** `Views/EmailTemplates/EmailConfirmation.cshtml`
- **Welcome Email:** `Views/EmailTemplates/Welcome.cshtml` (sent after confirmation)
- **Password Reset:** `Views/EmailTemplates/PasswordReset.cshtml`

### Email Confirmation Implementation (Already Done in Your App):

**Key Files That Handle Email Confirmation:**

1. **Registration Process:**
   - `Areas/Identity/Pages/Account/Register.cshtml.cs` - Sends confirmation email
   - `Areas/Identity/Pages/Account/Register.cshtml` - Registration form

2. **Email Confirmation:**
   - `Areas/Identity/Pages/Account/ConfirmEmail.cshtml.cs` - Processes confirmation
   - `Areas/Identity/Pages/Account/ConfirmEmail.cshtml` - Confirmation success page
   - Automatically sends welcome email after successful confirmation

3. **Resend Functionality:**
   - `Areas/Identity/Pages/Account/ResendEmailConfirmation.cshtml.cs` - Resends email
   - `Areas/Identity/Pages/Account/ResendEmailConfirmation.cshtml` - Resend form

4. **Email Templates:**
   - `Views/EmailTemplates/EmailConfirmation.cshtml` - Professional confirmation email
   - `Views/EmailTemplates/Welcome.cshtml` - Welcome email after confirmation

5. **Email Services:**
   - `PriceTracker.Infrastructure.Services.EmailService` - Core email functionality
   - `Services/RazorEmailTemplateService.cs` - Template rendering

**Configuration Files:**
- `Extensions/ApplicationServiceExtensions.cs` - Email service registration
- `appsettings.json` - Email settings (requires User Secrets for passwords)

**User Experience Flow:**
1. User registers → `RegisterConfirmation.cshtml` page shown
2. Email sent → Professional branded confirmation email
3. User clicks link → `ConfirmEmail.cshtml` success page
4. Welcome email sent → Second email with welcome message
5. User can login → Full access granted  

### Troubleshooting Email Confirmation:

**Problem: "No confirmation email received"**
- Check spam folder
- Verify User Secrets are set correctly: `dotnet user-secrets list`
- Check application logs for email errors
- Test with Gmail first (most reliable for development)

**Problem: "Invalid confirmation link"**
- Links expire after 24 hours
- Use "Resend Email Confirmation" if needed
- Check that HTTPS is working properly

**Problem: "Email service not configured"**
- Verify `EmailSettings` in User Secrets
- Check that EmailService is registered in DI container (already done in your app)

### Development vs Production Email Confirmation:

**Development (your current setup):**
- Uses Gmail SMTP
- Requires Gmail App Password
- Emails sent immediately

**Production (recommended):**
- Use SendGrid, Amazon SES, or similar service
- Configure proper DNS records (SPF, DKIM)
- Set up monitoring for delivery failures

### Development Options (Not Recommended):

**If you need to temporarily disable email confirmation for testing:**

**⚠️ Prerequisites:** Ensure initial setup is complete first (Steps 1-2)

1. **Modify Identity Configuration** in `Extensions/ApplicationServiceExtensions.cs`:
   ```csharp
   // Temporarily change these to false for development only
   options.SignIn.RequireConfirmedAccount = false;
   options.SignIn.RequireConfirmedEmail = false;
   ```

2. **Rebuild and restart application:**
   ```bash
   dotnet build
   dotnet run
   ```

3. **Remember to re-enable** for production:
   ```csharp
   // Production settings (your current configuration)
   options.SignIn.RequireConfirmedAccount = true;
   options.SignIn.RequireConfirmedEmail = true;
   ```

**⚠️ Warning:** Disabling email confirmation removes important security and reduces user engagement. It's better to properly configure email services.

### Email Confirmation Best Practices:

✅ **Always use email confirmation** in production  
✅ **Professional email templates** (already implemented)  
✅ **Clear user communication** about email verification  
✅ **Resend functionality** for users who don't receive emails  
✅ **Welcome emails** to engage confirmed users  
✅ **Proper error handling** for email delivery failures    

## Step 7: Email Templates (Already Implemented)

Your application already includes these email templates:
- `Views/EmailTemplates/EmailConfirmation.cshtml` - Registration confirmation
- `Views/EmailTemplates/PasswordReset.cshtml` - Password reset emails  
- `Views/EmailTemplates/Welcome.cshtml` - Welcome emails after confirmation

## Step 8: Production Deployment Checklist

1. **Use environment variables** for sensitive configuration
2. **Set up proper SPF/DKIM records** for your domain
3. **Use a dedicated email service** (SendGrid, Amazon SES, Mailgun)
4. **Configure proper error logging** for email delivery issues
5. **Set up monitoring** for failed email deliveries

## Step 9: Common Email Provider Settings

### Gmail
- SMTP Server: smtp.gmail.com
- Port: 587 (TLS) or 465 (SSL)
- Requires App Password (not regular password)

### Outlook/Hotmail
- SMTP Server: smtp-mail.outlook.com  
- Port: 587
- Enable StartTLS

### Yahoo Mail
- SMTP Server: smtp.mail.yahoo.com
- Port: 587 or 465
- Requires App Password

### Office 365
- SMTP Server: smtp.office365.com
- Port: 587
- Enable StartTLS

## Step 10: Troubleshooting Common Issues

**Before troubleshooting email issues, ensure initial setup is complete:**

```bash
# Verify these commands have been run successfully:
dotnet restore              # ✅ Packages restored
dotnet user-secrets init    # ✅ User secrets initialized  
dotnet ef database update   # ✅ Database migrations applied
dotnet build               # ✅ Application builds successfully
```

**Common Email Setup Issues:**

- **"Authentication failed"**: Check App Password setup
- **"SMTP timeout"**: Verify port and SSL settings  
- **Emails go to spam**: Configure SPF/DKIM records
- **Template not rendering**: Check file paths in `RazorEmailTemplateService`
- **Service registration errors**: Verify services are registered in `Program.cs` (already done)
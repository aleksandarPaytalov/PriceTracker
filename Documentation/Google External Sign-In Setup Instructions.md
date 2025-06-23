# Google External Sign-In Setup Instructions
## PriceTracker ASP.NET Core 8 MVC Application

### Prerequisites
- PriceTracker application cloned from GitHub
- .NET 8 SDK installed
- Google account for creating OAuth application
- HTTPS enabled (required for production)

## Step 1: Restore NuGet Packages (REQUIRED)

After cloning the repository, you must restore the NuGet packages:

```bash
# Navigate to the PriceTracker project folder
cd PriceTracker

# Restore all NuGet packages
dotnet restore

# Alternative: Build the project (which includes restore)
dotnet build
-------------------------------------------------------------------------------

**Required packages (automatically restored from .csproj):**
- Microsoft.AspNetCore.Authentication.Google (8.0.15)
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.15)
- Microsoft.AspNetCore.Identity.UI (8.0.15)
- Plus other dependencies

**Note:** The Google authentication package is already referenced in your `.csproj` file, so no additional package installation is required after restore.

## Step 2: Initial Setup After Cloning Repository

After restoring packages, complete the initial setup:

```bash
# 1. Restore NuGet packages
dotnet restore

# 2. Set up User Secrets (required for Google OAuth)
dotnet user-secrets init

# 3. Apply database migrations (if needed)
dotnet ef database update

# 4. Verify the application builds
dotnet build
```

## Step 3: Google Cloud Console Setup

### Create Google OAuth 2.0 Application

1. **Go to Google Cloud Console**
   - Visit: https://console.cloud.google.com/
   - Sign in with your Google account

2. **Create or Select a Project**
   - Click "Select a project" → "New Project"
   - Name: "PriceTracker-OAuth" (or your preferred name)
   - Click "Create"

3. **Enable Required APIs**
   - Go to "APIs & Services" → "Library"
   - Search for and enable these APIs:
     - **Google+ API** (required for basic profile)
     - **People API** (recommended for enhanced profile data)

4. **Configure OAuth Consent Screen**
   - Go to "APIs & Services" → "OAuth consent screen"
   - Choose **"External"** (unless you have Google Workspace)
   - Fill required fields:
     - **App name:** "PriceTracker"
     - **User support email:** your email address
     - **App logo:** (optional, upload your app logo)
     - **App domain:** your domain (e.g., `localhost:7262` for development)
     - **Authorized domains:** 
       - Development: `localhost`
       - Production: `yourdomain.com`
     - **Developer contact email:** your email address
   - Save and continue through all steps

5. **Create OAuth 2.0 Credentials**
   - Go to "APIs & Services" → "Credentials"
   - Click "Create Credentials" → "OAuth client ID"
   - **Application type:** "Web application"
   - **Name:** "PriceTracker Web Client"
   - **Authorized redirect URIs:** Add these exact URLs:
     - Development: `https://localhost:7262/signin-google`
     - Production: `https://yourdomain.com/signin-google`
   - Click "Create"
   - **📋 Copy the Client ID and Client Secret** (you'll need these next)

## Step 4: Configure Application Settings

Your `appsettings.json` already has the Google authentication section. You need to configure it:

### Update appsettings.json:
```json
{
  "Authentication": {
    "Google": {
      "ClientId": "your-actual-google-client-id.apps.googleusercontent.com",
      "ClientSecret": "your-actual-google-client-secret"
    }
  }
}
```

### Required User Secrets Configuration:

**CRITICAL:** Never commit Google credentials to source control. Use User Secrets:

```bash
# Set your Google OAuth credentials in User Secrets (REQUIRED)
dotnet user-secrets set "Authentication:Google:ClientId" "your-google-client-id.apps.googleusercontent.com"
dotnet user-secrets set "Authentication:Google:ClientSecret" "your-google-client-secret"
```

**Example User Secrets values format:**
```bash
# Replace with your actual values from Google Cloud Console
dotnet user-secrets set "Authentication:Google:ClientId" "123456789-abcdefghijklmnop.apps.googleusercontent.com"
dotnet user-secrets set "Authentication:Google:ClientSecret" "GOCSPX-abcdefghijklmnopqrstuvwxyz"
```

## Step 5: Verify Configuration (Already Implemented)

Your application already has all the necessary Google authentication configuration:

✅ **Google Authentication configured** in `ApplicationServiceExtensions.cs`  
✅ **External login pages implemented** in `Areas/Identity/Pages/Account/`  
✅ **User model supports external logins** in your User entity  
✅ **Login UI includes Google button** in the login page  

**No code changes are required** - only configuration!

## Step 6: Configure HTTPS for Development

Google OAuth requires HTTPS. Your application is already configured for this:

**Development URLs (from launchSettings.json):**
- HTTP: `http://localhost:5193`
- HTTPS: `https://localhost:7262` ← **Use this for Google redirect URI**

**Important:** Always use the HTTPS URL (`https://localhost:7262`) in your Google Cloud Console redirect URI configuration.

## Step 6: Testing Google Authentication

1. **Ensure User Secrets are set:**
   ```bash
   # Verify your secrets are configured
   dotnet user-secrets list
   ```
   You should see your Google ClientId and ClientSecret listed.

2. **Run your application:**
   ```bash
   dotnet run
   ```

3. **Navigate to login page:**
   - Go to `https://localhost:7262/Identity/Account/Login`
   - You should see the "Sign in with Google" button

4. **Test Google Sign-in:**
   - Click "Sign in with Google"
   - You should be redirected to Google's authentication page
   - After authorization, you'll return to your app
   - New users will be prompted to complete registration
   - Check your database for the new user account

## Step 7: Verify Database Integration

After successful Google sign-in, check that:

1. **User is created** in your `Users` table
2. **External login record** is created in `AspNetUserLogins` table
3. **User can sign in again** using the same Google account

## Step 8: Production Deployment Configuration

### Update Google Cloud Console for Production:

1. **Add Production URLs:**
   - Go to your OAuth client in Google Cloud Console
   - Add your production redirect URI: `https://yourdomain.com/signin-google`
   - Update authorized domains to include your production domain

2. **Environment Variables for Production:**
   ```bash
   # Set these in your production environment
   Authentication__Google__ClientId=your-client-id
   Authentication__Google__ClientSecret=your-client-secret
   ```

3. **Verify OAuth Consent Screen:**
   - For production apps serving external users, you may need Google verification
   - This process can take several days
   - Internal/testing apps can remain unverified

## Step 9: Troubleshooting Common Issues

### "redirect_uri_mismatch" Error
- **Solution:** Ensure redirect URI in Google Console exactly matches your app URL
- **Development:** `https://localhost:7262/signin-google`
- **Production:** `https://yourdomain.com/signin-google`
- **Check for:** Trailing slashes, HTTP vs HTTPS, port numbers

### "invalid_client" Error  
- **Solution:** Verify Client ID and Client Secret are correct in User Secrets
- **Check:** User secrets are properly set and spelled correctly

### Google Button Not Appearing
- **Check:** Google configuration section exists in appsettings.json
- **Verify:** User secrets contain both ClientId and ClientSecret
- **Ensure:** Application is running on HTTPS

### Users Not Being Created
- **Check:** Database connection string is correct
- **Verify:** Entity Framework migrations have been applied
- **Review:** Application logs for any error messages

### Email Conflicts with Existing Users
- **Behavior:** If email already exists, external login will be linked to existing account
- **Expected:** User can sign in with both password and Google

## Step 10: Additional Google Features (Optional)

Your current implementation already includes:
  
✅ **Email Verification** - Google emails are considered verified  

### Available Google Scopes (Already Configured):
- `profile` - Basic profile information
- `email` - Email address

## Step 11: Security Best Practices

1. **Keep Client Secret Secure:**
   - Never commit to source control
   - Use proper secret management in production
   - Rotate credentials periodically

2. **Monitor OAuth Usage:**
   - Check Google Cloud Console for usage statistics
   - Monitor for unusual authentication patterns
   - Set up alerts for quota limits

3. **HTTPS Only:**
   - Always use HTTPS in production
   - Google OAuth will not work over HTTP
   - Secure cookie settings are already configured

Your Google external sign-in should now be fully functional! 🎉
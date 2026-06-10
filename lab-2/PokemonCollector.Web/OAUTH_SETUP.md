# OAuth Authentication Setup Guide

This guide explains how to set up Google and Facebook authentication for the Pokemon Collector application.

## Prerequisites

- A Google account with access to Google Cloud Console
- A Facebook account with access to Facebook Developers portal
- ASP.NET Core application configured with Identity

## Google OAuth Setup

### Step 1: Create a Google Cloud Project

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project (e.g., "Pokemon Collector")
3. Enable the Google+ API:
   - Go to "APIs & Services" > "Library"
   - Search for "Google+ API"
   - Click "Enable"

### Step 2: Create OAuth 2.0 Credentials

1. Go to "APIs & Services" > "Credentials"
2. Click "Create Credentials" > "OAuth client ID"
3. Choose "Web application"
4. Add authorized redirect URIs:
   - `https://localhost:7001/signin-google` (for development)
   - `https://yourdomain.com/signin-google` (for production)
5. Click "Create"
6. Copy the Client ID and Client Secret

### Step 3: Configure Secrets

Use .NET User Secrets to store the credentials securely:

```bash
# Initialize user secrets (do this once)
dotnet user-secrets init -p lab-2/PokemonCollector.Web

# Set Google credentials
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID" -p lab-2/PokemonCollector.Web
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET" -p lab-2/PokemonCollector.Web
```

## Facebook OAuth Setup

### Step 1: Create a Facebook App

1. Go to [Facebook Developers](https://developers.facebook.com/)
2. Click "My Apps" > "Create App"
3. Choose "Consumer" as the app type
4. Fill in the app details
5. Add "Facebook Login" product

### Step 2: Configure Facebook Login

1. In your app dashboard, go to "Settings" > "Basic"
2. Copy the App ID and App Secret
3. Go to "Products" > "Facebook Login" > "Settings"
4. Add Valid OAuth Redirect URIs:
   - `https://localhost:7001/signin-facebook` (for development)
   - `https://yourdomain.com/signin-facebook` (for production)

### Step 3: Configure Secrets

```bash
# Set Facebook credentials
dotnet user-secrets set "Authentication:Facebook:AppId" "YOUR_APP_ID" -p lab-2/PokemonCollector.Web
dotnet user-secrets set "Authentication:Facebook:AppSecret" "YOUR_APP_SECRET" -p lab-2/PokemonCollector.Web
```

## Configuration in appsettings.json (Optional)

For production environments, use environment variables or a secure configuration service instead of user secrets.

Example `appsettings.Production.json`:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "${GOOGLE_CLIENT_ID}",
      "ClientSecret": "${GOOGLE_CLIENT_SECRET}"
    },
    "Facebook": {
      "AppId": "${FACEBOOK_APP_ID}",
      "AppSecret": "${FACEBOOK_APP_SECRET}"
    }
  }
}
```

## Testing OAuth Locally

1. Start the application:
   ```bash
   dotnet run -p lab-2/PokemonCollector.Web
   ```

2. Navigate to the login page
3. You should see "Google" and "Facebook" login buttons
4. Click on either button to test the OAuth flow

## Troubleshooting

- **Invalid redirect URI**: Make sure the redirect URI in your OAuth provider settings exactly matches your application's URL
- **Missing credentials**: Ensure user secrets are properly set with `dotnet user-secrets list`
- **HTTPS required**: OAuth providers require HTTPS. For development, the application uses HTTPS by default

## Security Notes

- Never commit credentials to version control
- Use user secrets for development only
- Use environment variables or a secret manager for production
- Rotate credentials regularly
- Enable HTTPS in production

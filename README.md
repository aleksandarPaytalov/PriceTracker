# PriceTracker

A comprehensive price tracking and budget management web application built with ASP.NET Core 8 MVC, designed to help users monitor product prices, manage budgets, and save money through intelligent price alerts.

## 🚀 Features

### 📊 Price Tracking
- **Product Monitoring**: Track prices across multiple stores and platforms
- **Price History**: View historical price data with visual charts
- **Store Management**: Add and manage multiple retail stores

### 💰 Budget Management
- **Monthly Budgets**: Set and track monthly spending limits
- **Expense Tracking**: Record and categorize your expenses
- **Budget Analytics**: Visual insights into your spending patterns
- **Smart Notifications**: Alerts when approaching budget limits

### 🔐 User Management
- **Secure Authentication**: ASP.NET Core Identity with email confirmation
- **Google OAuth**: Sign in with your Google account
- **User Profiles**: Personalized dashboards and settings
- **Two-Factor Authentication**: Enhanced security with 2FA support

### 📧 Email System
- **Professional Templates**: Beautiful, responsive email designs
- **Email Confirmation**: Secure account verification process
- **Price Alerts**: Automated notifications for price changes
- **Welcome Emails**: Engaging onboarding experience

### 📱 Modern UI/UX
- **Responsive Design**: Works seamlessly on desktop and mobile
- **Clean Interface**: Intuitive and user-friendly design

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server with Entity Framework Core 8.0
- **Authentication**: ASP.NET Core Identity + Google OAuth 2.0
- **Email**: SMTP with professional HTML templates
- **Architecture**: Clean Architecture with Repository pattern

### Frontend
- **UI Framework**: Bootstrap 5 + Custom CSS
- **JavaScript**: Vanilla JS with modern ES6+
- **Icons**: Font Awesome 6
- **Forms**: Client & Server-side validation

### Development & Deployment
- **IDE**: Visual Studio 2022 / VS Code
- **Version Control**: Git & GitHub
- **Database Migrations**: Entity Framework Core Migrations
- **Configuration**: User Secrets, appsettings.json
- **Logging**: Built-in ASP.NET Core logging

## 🚀 Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB for development)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/aleksandarPaytalov/PriceTracker.git
   cd PriceTracker
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Configure User Secrets**
   ```bash
   dotnet user-secrets init
   ```
   *Configure email and Google OAuth settings as needed. See detailed instructions in `docs/` folder.*

4. **Update database**
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Open your browser**
   - Navigate to `https://localhost:7262`
   - Register a new account and start tracking prices!

## ⚙️ Additional Configuration

The application requires additional setup for full functionality:

- **📧 Email Configuration**: Required for user registration, notifications, and password reset
- **🔑 Google OAuth Setup**: Optional - enables users to sign in with Google accounts

**📋 Detailed setup instructions for both features can be found in the `docs/` folder of this repository.**

## 🗄️ Database

The application uses SQL Server with Entity Framework Core for data persistence.

### Key Entities
- **Users**: User accounts with Identity integration
- **Products**: Items being tracked
- **Stores**: Retail stores and platforms
- **Prices**: Historical price data
- **Budgets**: User budget configurations
- **Expenses**: User expense records
- **Tasks**: User to-do items
- **Notifications**: System notifications

### Migrations
```bash
# Create new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## 🎨 Customization

### Email Templates
Professional email templates are located in `Views/EmailTemplates/`:
- `EmailConfirmation.cshtml` - Account verification
- `PasswordReset.cshtml` - Password reset
- `Welcome.cshtml` - Welcome message

### Styling
- Custom CSS: `wwwroot/css/`
- Bootstrap components with custom styling
- Responsive design for all screen sizes

### Business Logic
- Services: `Services/` directory
- Core logic: `PriceTracker.Core/` project
- Data access: `PriceTracker.Infrastructure/` project

## 🔒 Security Features

- **Email Confirmation**: Required for account activation
- **Password Requirements**: Strong password policies
- **Account Lockout**: Protection against brute force attacks
- **Two-Factor Authentication**: TOTP-based 2FA support
- **External Authentication**: Secure Google OAuth integration
- **HTTPS Enforcement**: Secure communication
- **Input Validation**: Client and server-side validation

## 🤝 Contributing

We welcome contributions from the community! Here's how you can help:

### Development Setup
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Make your changes and add tests
4. Commit your changes: `git commit -m 'Add amazing feature'`
5. Push to the branch: `git push origin feature/amazing-feature`
6. Open a Pull Request

### Coding Standards
- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Write unit tests for new features
- Ensure responsive design for UI changes

### Bug Reports
Please use GitHub Issues to report bugs. Include:
- Detailed description of the issue
- Steps to reproduce
- Expected vs actual behavior
- Screenshots (if applicable)
- Environment details (.NET version, browser, etc.)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## 👥 Team

- **Developer**: [Aleksandar Paytalov](https://github.com/aleksandarPaytalov)
- **Architecture**: Clean Architecture with SOLID principles
- **Mentoring**: Senior full-stack development guidance

## 📞 Support

### Documentation
- [Email Setup Guide](docs/email-setup-instructions.md)
- [Google OAuth Configuration](docs/google-signin-instructions.md)
- [Deployment Guide](docs/deployment.md)

### Community
- **GitHub Issues**: Bug reports and feature requests
- **Discussions**: Community support and general questions
- **Wiki**: Additional documentation and tutorials

### Contact
- **GitHub**: [@aleksandarPaytalov](https://github.com/aleksandarPaytalov)

---

**Built with ❤️ using ASP.NET Core 8.0**

*PriceTracker - Smart price tracking for smarter shopping*

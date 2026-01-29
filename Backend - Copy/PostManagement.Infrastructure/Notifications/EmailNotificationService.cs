using Microsoft.Extensions.Configuration;
using PostManagement.Application.Email;
using PostManagement.Application.Interfaces.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PostManagement.Infrastructure.Notifications
{
    public class EmailNotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public EmailNotificationService(
            IEmailService emailService,
            IConfiguration config)
        {
            _emailService = emailService;
            _config = config;
        }

        public async Task SendEmailVerificationAsync(string email, string encodedToken)
        {
            //token encoding is important because the raw(not encoded) token contains symbols like +,=,%,etc which
            //are interpreted as space by the browser while sending via frontend(URL) and hence the comparision of the received token
            // and the token in the database fails
            //var encodedToken = WebUtility.UrlEncode(token);

            var link = $"{_config["FrontendUrl"]}/verify-email?token={encodedToken}";


            var body = $@"
        <h2>Verify your email</h2>
        <p>Click the link below to confirm your email address:</p>
        <a href='{link}'>Verify Email</a>
        <p>This link expires in 30 minutes.</p>";

            await _emailService.SendEmailAsync(
                email,
                "Verify your email address",
                body
            );
        }
    

    public async Task SendPasswordResetAsync(string email, string encodedToken)
        {
            var link = $"{_config["FrontendUrl"]}/reset-password?token={encodedToken}";

            var body = $@"
        <h2>Password Reset</h2>
        <p>Click the link below to reset your password:</p>
        <a href='{link}'>Reset Password</a>
        <p>This link expires in 30 minutes.</p>";

            await _emailService.SendEmailAsync(email, "Reset your password", body);
        }


    }
}


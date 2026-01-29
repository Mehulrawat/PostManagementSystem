using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostManagement.Application.Interfaces.Notification
{
    public interface INotificationService
    {
        Task SendEmailVerificationAsync(string email, string token);
        Task SendPasswordResetAsync(string email, string token);


    }

}

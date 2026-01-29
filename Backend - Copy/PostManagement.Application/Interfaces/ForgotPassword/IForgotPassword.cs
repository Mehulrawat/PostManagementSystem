using PostManagement.Application.Contracts.Password;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostManagement.Application.Interfaces.ForgotPassword
{
    internal interface IForgotPassword
    {
        Task ForgotPassword(ForgotPasswordRequest request);

    }
}

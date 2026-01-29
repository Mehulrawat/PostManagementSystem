using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostManagement.Application.Contracts.Password
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

}

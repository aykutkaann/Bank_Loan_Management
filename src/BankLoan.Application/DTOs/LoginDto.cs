using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.DTOs
{
    public class LoginDto
    {

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

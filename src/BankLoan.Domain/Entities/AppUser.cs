using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace BankLoan.Domain.Entities
{
    public class AppUser: IdentityUser<Guid>
    {
        

        public  Customer? Customer { get; set; }

  

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

    }
}

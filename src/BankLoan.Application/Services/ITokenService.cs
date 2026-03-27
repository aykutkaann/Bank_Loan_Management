using BankLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLoan.Application.Services
{
    public interface ITokenService
    {

        string GenerateAccessToken(AppUser user, IList<string> roles);

        string GenerateRefreshToken();
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using BankLoan.Application.DTOs;

namespace BankLoan.Application.Services
{
    public interface IAuthService
    {
        Task<TokenResponseDto> LoginAsync(LoginDto loginDto);

        Task<TokenResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<TokenResponseDto> RefreshTokenAsync(string accessToken, string refreshToken);
        Task LogoutAsync(string email);
    }
}

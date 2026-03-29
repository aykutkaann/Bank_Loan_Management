using BankLoan.Api.Endpoints;
using BankLoan.Application.Services;
using BankLoan.Domain.Entities;
using BankLoan.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT key is missing");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ICreditScoreService, CreditScoreService>();


builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

//Seed Roles

using var scope = app.Services.CreateScope();

var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

string[] roles = ["Admin", "LoanOfficer", "Customer"];
foreach (var role in roles)
{
    if (!await roleManager.RoleExistsAsync(role))
        await roleManager.CreateAsync(new IdentityRole<Guid>(role));
}

var adminEmail = "admin@bank.com";
if(await userManager.FindByEmailAsync(adminEmail) == null)
{
    var adminUser = new AppUser
    {
        UserName = adminEmail,
        Email = adminEmail,
        EmailConfirmed = true,
    };

    var result = await userManager.CreateAsync(adminUser, "Admin123!");
    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

var officerEmail = "officer@bank.com";
if(await userManager.FindByEmailAsync(officerEmail) == null)
{
    var officerUser = new AppUser
    {
        UserName = officerEmail,
        Email = officerEmail,
        EmailConfirmed = true,
    };

    var result = await userManager.CreateAsync(officerUser, "Officer123!");
    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(officerUser, "LoanOfficer");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.UseAuthentication();
app.UseAuthorization();


//Endpoints
app.MapAuthEndpoint();




app.Run();

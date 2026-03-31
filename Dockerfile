# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
WORKDIR /src

# Copy solution and project files first (for layer caching)
COPY src/BankLoan.Api/BankLoan.Api.csproj src/BankLoan.Api/
COPY src/BankLoan.Application/BankLoan.Application.csproj src/BankLoan.Application/
COPY src/BankLoan.Domain/BankLoan.Domain.csproj src/BankLoan.Domain/
COPY src/BankLoan.Infrastructure/BankLoan.Infrastructure.csproj src/BankLoan.Infrastructure/
COPY src/BankLoan.Shared/BankLoan.Shared.csproj src/BankLoan.Shared/

# Restore dependencies
RUN dotnet restore src/BankLoan.Api/BankLoan.Api.csproj

# Copy everything else
COPY src/ src/

# Build and publish
RUN dotnet publish src/BankLoan.Api/BankLoan.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "BankLoan.Api.dll"]

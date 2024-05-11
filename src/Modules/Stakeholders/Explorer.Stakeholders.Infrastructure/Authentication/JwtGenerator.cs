﻿using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.UseCases;
using FluentResults;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;

namespace Explorer.Stakeholders.Infrastructure.Authentication;

public class JwtGenerator : ITokenGenerator
{
    private readonly string _key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "explorer_secret_key";
    private readonly string _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "explorer";
    private readonly string _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "explorer-front.com";

    private static readonly HttpClient _sharedClient = new HttpClient() { BaseAddress = new Uri("http://localhost:8085/") };

    public async Task<Result<AuthenticationTokensDto>> GenerateAccessToken(User user, long personId)
    {
        var authenticationResponse = new AuthenticationTokensDto();

        var requestUrl = $"jwt?id={user.Id}&username={user.Username}&personId={personId}&role={user.GetPrimaryRoleName}";

        using HttpResponseMessage response = await _sharedClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        authenticationResponse.Id = user.Id;
        authenticationResponse.AccessToken = jsonResponse.Substring(1, jsonResponse.Length - 3);

        return authenticationResponse;
    }

    public Result<ResetPasswordTokenDto> GenerateResetPasswordToken(long id, string email)
    {
        var resetPasswordResponse = new ResetPasswordTokenDto();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("email", email)
        };

        var jwt = CreateToken(claims, 15);
        resetPasswordResponse.ResetPasswordToken = jwt;

        return resetPasswordResponse;
    }

    private string CreateToken(IEnumerable<Claim> claims, double expirationTimeInMinutes)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: DateTime.Now.AddMinutes(expirationTimeInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Result<ResetPasswordTokenDto> GenerateRegistrationConfirmationToken(User user)
    {
        var confirmRegistrationToken = new ResetPasswordTokenDto();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("id", user.Id.ToString()),
            new("username", user.Username),
            new("confirm", "true")
        };

        string token = CreateToken(claims, 15);
        confirmRegistrationToken.ResetPasswordToken = token;
        return confirmRegistrationToken;
    }
}
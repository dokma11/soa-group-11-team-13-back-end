using Explorer.Payments.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using Grpc.Core;
using GrpcServiceTranscoding;
using Microsoft.Extensions.Logging;
using Grpc.Net.Client;

namespace Explorer.API.Controllers;

[Route("api/users")]
public class AuthenticationController : BaseApiController
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IWalletService _walletService;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly string _jwtServiceAddress = "http://jwt:8085"; // JWT service address


    public AuthenticationController(IAuthenticationService authenticationService, IWalletService walletService, ILogger<AuthenticationController> logger)
    {
        _authenticationService = authenticationService;
        _walletService = walletService;
        _logger = logger;
    }


    private static readonly HttpClient _sharedClient = new()
    {
        BaseAddress = new Uri("http://followers:8084/"),
    };
    
    [HttpPost]
    public async Task<ActionResult<RegistrationConfirmationTokenDto>> RegisterTourist([FromBody] AccountRegistrationDto account)
    {
        var result = _authenticationService.RegisterTourist(account);
        if(result.IsSuccess && !result.IsFailed)
        {
            _walletService.Create(new Payments.API.Dtos.WalletCreateDto(result.Value.Id));
        }
        //return CreateResponse(result);

        string json = JsonConvert.SerializeObject(account);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        try
        {
                
            HttpResponseMessage response = await _sharedClient.PostAsync("users", content);
            response.EnsureSuccessStatusCode();
            return Ok(response);
        }
        catch (HttpRequestException)
        {
            return BadRequest();
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
        
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] CredentialsDto credentials)
    {
        // Validate credentials
        var validationResult = _authenticationService.ValidateCredentials(credentials);
        if (!validationResult.IsSuccess)
        {
            _logger.LogError($"Invalid credentials for user {credentials.Username}");
            return BadRequest(validationResult.Errors);
        }

        // Create a new HttpClientHandler for each call
        var httpHandler = new HttpClientHandler
        {
            // This switch must be set before setting ServerCertificateCustomValidationCallback
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        // Create a new channel for each call
        using var channel = GrpcChannel.ForAddress(_jwtServiceAddress, new GrpcChannelOptions { HttpHandler = httpHandler });
        var jwtClient = new JwtService.JwtServiceClient(channel);

        // Use the validated user information to generate a JWT token
        var userDto = validationResult.Value;
        try
        {
            var tokenResponse = await jwtClient.GenerateTokenAsync(new GenerateTokenRequest
            {
                UserId = userDto.Id,
                Username = userDto.Username,
                PersonId = userDto.PersonId.ToString(),
                Role = userDto.Role,
            });

            _logger.LogInformation($"JWT token generated for user {userDto.Username}");

            return Ok(new AuthenticationTokensDto
            {
                AccessToken = tokenResponse.Token
            });
        }
        catch (RpcException rpcEx)
        {
            _logger.LogError($"RPC error during JWT token generation: {rpcEx.Status.Detail}");
            return StatusCode((int)rpcEx.StatusCode, rpcEx.Status.Detail);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during JWT token generation: {ex.Message}");
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }



    /*

    [HttpPost("login")]
    public ActionResult<AuthenticationTokensDto> Login([FromBody] CredentialsDto credentials)
    {
        var result = _authenticationService.Login(credentials);
        return CreateResponse(result);
    }

    */

    [HttpPost("reset-password")]
    public ActionResult<ResetPasswordTokenDto> GenerateResetPasswordLink([FromBody] ResetPasswordEmailDto resetPasswordEmail)
    {
        var result = _authenticationService.GenerateResetPasswordToken(resetPasswordEmail);
        return CreateResponse(result);
    }

    [HttpPatch("reset-password/new")]
    public ActionResult ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
    {
        string email = ExtractPayload(resetPasswordRequestDto.Token);
        var result = _authenticationService.ResetPassword(resetPasswordRequestDto, email);
        return Ok();
    }

    private string ExtractPayload(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        // Read token without validation
        var jwtToken = tokenHandler.ReadJwtToken(token);

        // Access claims directly from the token
        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        return emailClaim;
    }

    [HttpGet("confirm-registration")]
    public ActionResult ConfirmPassword([FromQuery] string confirm_registration_token)
    {

        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtToken = tokenHandler.ReadJwtToken(confirm_registration_token);

        var usermname = jwtToken.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
        var confirm = jwtToken.Claims.FirstOrDefault(c => c.Type == "confirm")?.Value;

        var result = _authenticationService.ConfirmRegistration(usermname, confirm);

        return CreateResponse(result);
    }
}

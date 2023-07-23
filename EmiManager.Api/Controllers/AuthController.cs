using System.Security.Claims;

using EmiManager.Api.Repositories.Contracts;
using EmiManager.Api.Services;
using EmiManager.Domain.Dtos;
using EmiManager.Domain.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BC = BCrypt.Net.BCrypt;

using FluentValidation;
using FluentValidation.Results;
using EmiManager.Domain.Extensions;
using EmiManager.Domain.Validators;

namespace EmiManager.Api.Controllers;

[Route("api/v1/auth")]
[ApiController]
public class AuthController : ControllerBase {
    private readonly AuthService _authService;
    private readonly IUserRepository _userRepo;

    public AuthController(AuthService authService, IUserRepository userRepo) {
        _authService = authService;
        _userRepo = userRepo;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(
        RegisterRequestDto requestDto, 
        [FromServices] IValidator<RegisterRequestDto> validator) {

        ValidationResult result = validator.Validate(requestDto);

        if (!result.IsValid) {
            return UnprocessableEntity(result.ToErrorDictionary());
        }

        User? existing = await _userRepo.GetUserByEmail(requestDto.Email);
        if (existing != null) {
            var errorDict = new Dictionary<string, string>();
            errorDict.Add("Email", ValidationErrorConstants.Duplicate);
            return UnprocessableEntity(errorDict);
        }
        
        string passwordHash = BC.HashPassword(requestDto.Password);
        await _userRepo.CreateUser(new User() { 
            Email = requestDto.Email,
            PasswordHash = passwordHash,
            Name = requestDto.Name
        });

        return NoContent();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(
        LoginRequestDto requestDto,
        [FromServices] IValidator<LoginRequestDto> validator) {

        ValidationResult result = validator.Validate(requestDto);
        if(!result.IsValid) {
            return UnprocessableEntity(result.ToErrorDictionary());
        }

        User? user = await _userRepo.GetUserByEmail(requestDto.Email);

        Dictionary<string, string> validationErrors = new();
        if(user is null) {
            validationErrors.Add("Password", ValidationErrorConstants.IncorrectAuth);
            return UnprocessableEntity(validationErrors);
        }

        if(!user.IsEmailVerified) {
            validationErrors.Add("Email", ValidationErrorConstants.UnverifiedEmail);
            return UnprocessableEntity(validationErrors);
        }

        if(!BC.Verify(requestDto.Password, user.PasswordHash)) {
            validationErrors.Add("Password", ValidationErrorConstants.IncorrectAuth);
            return UnprocessableEntity(validationErrors);
        }

        string jwtToken = _authService.SignTokenForUser(user.Email, user.Name);
        LoginResponseDto responseDto = new(jwtToken, user.Email, user.Name);
        return Ok(responseDto);
    }

    [HttpPost]
    [Route("generate-code")]
    public async Task<IActionResult> GenerateCode(
        GenerateCodeRequestDto requestDto,
        [FromServices] IValidator<GenerateCodeRequestDto> validator) {

        ValidationResult result = validator.Validate(requestDto);
        if(!result.IsValid) {
            return UnprocessableEntity(result.ToErrorDictionary());
        }

        Dictionary<string, string> validationErrors = new();
        User? user = await _userRepo.GetUserByEmail(requestDto.Email);
        if(user == null) {
            validationErrors.Add("Email", ValidationErrorConstants.NonExistent);
            return UnprocessableEntity(validationErrors);
        }

        if(user.IsEmailVerified) {
            validationErrors.Add("Email", ValidationErrorConstants.AlreadyVerified);
            return UnprocessableEntity(validationErrors);
        }

        string verificationCode = await _userRepo.GetVerificationCodeForUser(user);

        // Send code to Email

        return NoContent();
    }

    [HttpPost]
    [Route("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        VerifyEmailRequestDto requestDto,
        [FromServices] IValidator<VerifyEmailRequestDto> validator) {

        ValidationResult result = validator.Validate(requestDto);
        if(!result.IsValid) {
            return UnprocessableEntity(result.ToErrorDictionary());
        }

        User? user = await _userRepo.GetUserByEmail(requestDto.Email);
        Dictionary<string, string> validationErrors = new();
        if(user == null) {
            validationErrors.Add("Email", ValidationErrorConstants.NonExistent);
            return UnprocessableEntity(validationErrors);
        }

        if (user.IsEmailVerified) {
            validationErrors.Add("Email", ValidationErrorConstants.AlreadyVerified);
            return UnprocessableEntity(validationErrors);
        }

        bool isVerified = await _userRepo.VerifyEmail(user, requestDto.Code);

        if(!isVerified) {
            validationErrors.Add("Email", ValidationErrorConstants.Invalid);
            return UnprocessableEntity(validationErrors);
        }

        return NoContent();
    }

    [HttpGet]
    [Authorize]
    [Route("me")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IResult Me() {
        string? email = HttpContext.User.FindFirstValue(ClaimTypes.Email) ?? "No Email";
        string? name = HttpContext.User.FindFirstValue(ClaimTypes.Name) ?? "No Name";

        return Results.Ok($"{name} - {email}");
    }
}


using System.Text;

using EmiManager.Api.Data;
using EmiManager.Api.Repositories;
using EmiManager.Api.Repositories.Contracts;
using EmiManager.Api.Services;
using EmiManager.Domain.Dtos;
using EmiManager.Domain.Validators;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EmiManager.Api;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        string? issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        string? audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
        string? key = Environment.GetEnvironmentVariable("JWT_KEY");

        if ((new List<string?> { issuer, audience, key }).Any(k => k is null)) {
            throw new Exception("JWT Environment variables not set properly");
        }

        // Services
        // Validators
        builder.Services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestDtoValidator>();
        builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestDtoValidator>();
        builder.Services.AddScoped<IValidator<GenerateCodeRequestDto>, GenerateCodeRequestDtoValidator>();
        builder.Services.AddScoped<IValidator<VerifyEmailRequestDto>, VerifyEmailRequestDtoValidator>();

        builder.Services.AddSingleton<AuthService>(sp => {
            return new AuthService(issuer!, audience!, key!);
        });
        builder.Services.AddSingleton<MongoDbContext>(sp => {
            string? connStr = Environment.GetEnvironmentVariable("MONGO_CONN_STR");
            if (connStr is null) {
                throw new Exception("MongoDb connection string not set properly");
            }

            return new MongoDbContext(connStr);
        });
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                var encodedBytes = Encoding.UTF8.GetBytes(key!);
                SymmetricSecurityKey issuerSigningKey = new (encodedBytes);

                options.TokenValidationParameters = new() {
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = issuerSigningKey,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
        builder.Services.AddAuthorization();

        builder.Services.AddControllers();
        builder.Services.Configure<ApiBehaviorOptions>(options => {
            options.SuppressModelStateInvalidFilter = true;
        });
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
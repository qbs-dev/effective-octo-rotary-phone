using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Sieve.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TracklyApi.Entities;
using TracklyApi.Handlers;
using TracklyApi.Mappings;
using TracklyApi.Services;
using TracklyApi.Services.Implementations;
using TracklyApi.Sieve;
using TracklyApi.Validators;



var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Starting TracklyApi");

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddCors();

// Add database connectivity
builder.Services.AddDbContext<TracklyDbContext>(
    options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultDatabaseConnection"))
);

// Add validation and mappings
builder.Services.AddValidatorsFromAssemblyContaining<AuthRequestValidator>();

builder.Services.AddAutoMapper(options =>
{
    options.AddProfile<UserProfile>();
    options.AddProfile<UrlProfile>();
});

// Configure authorization and authentication
TokenValidationParameters tokenValidation = new()
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuers = builder.Configuration.GetSection("JWT")
                .GetRequiredSection("ValidIssuers").Get<string[]>(),
    ValidAudience = builder.Configuration["JWT:ValidAudience"],
    IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!)),
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddSingleton(tokenValidation);
builder.Services.AddAuthentication(authOpts =>
{
    authOpts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authOpts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOpts =>
{
    jwtOpts.TokenValidationParameters = tokenValidation;
    jwtOpts.Events = new JwtBearerEvents
    {
        OnTokenValidated = async (context) =>
        {
            IJWTService? jwtService = context.Request.HttpContext
                .RequestServices.GetService<IJWTService>();
            if (jwtService is null)
            {
                context.Fail("Internal service error");
                return;
            }
            if (context.SecurityToken is not JwtSecurityToken jwtToken ||
                !await jwtService.ValidateAccessAsync(jwtToken))
                context.Fail("Invalid Token details");
            return;
        }
    };
});
builder.Services.AddTransient<IJWTService, JWTService>();
builder.Services.AddSingleton<IAuthorizationHandler, UserIdHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CheckUserId", policy => policy.Requirements.Add(new UserIdRequirement()));
});

// Add sorting, filtering, pagination
builder.Services.AddScoped<ISieveProcessor, UrlSieveProcessor>();

// Add services
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IProfileService, ProfileService>();
builder.Services.AddTransient<IUrlService, UrlService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(builder => builder.AllowAnyOrigin());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();


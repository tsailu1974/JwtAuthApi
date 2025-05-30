using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using JwtAuthApi.Repositories;
using JwtAuthApi.Services;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using JwtAuthApi.Models;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);
//var conn = builder.Configuration.GetConnectionString("EnterpriseConnection");
var config = builder.Configuration;
config.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

if (builder.Environment.IsProduction())
{
    var keyVaultUrl = builder.Configuration["keyVaultUrl"] ?? "https://bmas-app-keyvault.vault.azure.net/";
    builder.Configuration.AddAzureKeyVault(
       new Uri(keyVaultUrl),
       new DefaultAzureCredential());
}
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddDbContext<EnterpriseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EnterpriseConnection"),
    
    sqlServerOptionsAction: sqloptions =>
    {
        sqloptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
            );
    }));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            var allowedOrigins = new List<string>();
            //production URL
            allowedOrigins.Add("https://zealous-flower-0a09e880f.6.azurestaticapps.net");
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                allowedOrigins.Add("http://localhost:3000");
            }

            builder.WithOrigins(allowedOrigins.ToArray())
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});
builder.Services.AddAuthentication(Options => 
{
    Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(Options => 
{
    Options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? throw new Exception("Jwt:Key is null")))
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "JwtAuthApi", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIs..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});builder.Services.AddScoped<IUserRepository, JwtAuthApi.Repositories.UserRepository>();
builder.Services.AddScoped<IAuthService, JwtAuthApi.Services.AuthService>();
builder.Services.AddControllers()
                .AddJsonOptions(options => {options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
    app.UseHttpsRedirection();

app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();



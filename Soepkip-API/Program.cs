using Microsoft.EntityFrameworkCore;
using SoepkipAPI.Data.Context;
using SoepkipAPI.Data.Interfaces;
using SoepkipAPI.Data.Repository;
using SoepkipAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SoepkipAPI.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<WeatherService>();

// JWT Configuratie
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Sensoring", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SensoringKey"]!))
        };
    }).AddJwtBearer("Monitoring", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:MonitoringKey"]!))
        };
    });

//Depencency injection
builder.Services.AddTransient<ITrashRepository, TrashRepository>();
builder.Services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

//Add database connection
var connectionString =
    builder.Configuration.GetConnectionString(Environment.GetEnvironmentVariable("CONNECTION_STRING_MYSQL") ??
                                              "DefaultConnection");
builder.Services.AddDbContext<TrashContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
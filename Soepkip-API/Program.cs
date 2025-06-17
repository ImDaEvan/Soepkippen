using Microsoft.EntityFrameworkCore;
using SoepkipAPI.Data.Context;
using SoepkipAPI.Data.Interfaces;
using SoepkipAPI.Data.Repository;
using SoepkipAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SoepkipAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;

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
builder.Services.AddTransient<IWeatherService, WeatherService>();
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

//Connection attempt logging
app.Use(async (context, next) =>
{
    var req = context.Request;
    req.EnableBuffering();
    Console.WriteLine("-----------------");
    Console.WriteLine(DateTime.Now);
    Console.WriteLine(req.HttpContext.Connection.RemoteIpAddress);
    Console.WriteLine($"Method:{req.Method}\nEndpoint:{req.Path} | {context.GetEndpoint()?.DisplayName}");
    Console.WriteLine($"Authorization: {req.Headers.Authorization}");
    Console.WriteLine($"Params: {string.Join("\n\t", req.Query.Select(q => $"{q.Key}={q.Value}"))}");
    Console.WriteLine($"Body: {await new StreamReader(req.Body).ReadToEndAsync()}");
    req.Body.Position = 0;
    Console.WriteLine("-----------------");
    await next.Invoke();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Landing page
app.MapGet("/", () => Results.Content("Api up and running", "text/html"));

app.Run();
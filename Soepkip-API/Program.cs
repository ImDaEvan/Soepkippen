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
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<WeatherService>();

// JWT Configuratie
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("sensoring", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["Jwt:Issuer"],
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes( Environment.GetEnvironmentVariable("JWT_KEY_SENSORING") ?? builder.Configuration["Jwt:SensoringKey"] ?? ""))
        };
    }).AddJwtBearer("monitoring", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["Jwt:Issuer"],
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey((Encoding.UTF8.GetBytes( Environment.GetEnvironmentVariable("JWT_KEY_MONITORING") ?? builder.Configuration["Jwt:MonitoringKey"] ?? "")))
        };
    });

//Depencency injection
builder.Services.AddTransient<ITrashRepository, TrashRepository>();
builder.Services.AddTransient<IWeatherService, WeatherService>();
builder.Services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

//Add database connection
var connectionString =
    Environment.GetEnvironmentVariable("CONNECTION_STRING_MYSQL") ?? builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TrashContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), optionsBuilder =>
    {
        optionsBuilder.CommandTimeout(10);
    });
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

//Landing page
app.MapGet("/", () => Results.Content("Api up and running", "text/html"));

//Version page
app.MapGet("/api/version", () => {
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "version.json");
        
    var json =  System.IO.File.ReadAllText(filePath);

    // Deserialize as Dictionary<string, JsonElement> to support both objects and strings (like "current": "4")
    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

    if (result == null) return "Version not found";
    
    return result["current"];
});

app.MapControllers();

app.Run();
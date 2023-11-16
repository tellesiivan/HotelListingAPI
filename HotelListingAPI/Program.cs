using System.Text;
using HotelListingAPI.Configurations;
using HotelListingAPI.Data;
using HotelListingAPI.Services;
using HotelListingAPI.Services.AuthManager;
using HotelListingAPI.Services.Country;
using HotelListingAPI.Services.Hotel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS support(has to be above .Build())
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder => 
        policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

// setup connection to the db
var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");
builder.Services.AddDbContext<HotelListingDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// serilog 
// context --> Instance of the builder
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.WriteTo.Console()
    .ReadFrom.Configuration(context.Configuration));

// Adds automapper
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

// repository scopes<InterfaceOfRepository, ImplementedRepository>
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IAuthManager, AuthManager>();

// Add identity core
builder.Services
    .AddIdentityCore<ApiUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<ApiUser>>("HotelListingAPI")
    .AddEntityFrameworkStores<HotelListingDbContext>().AddDefaultTokenProviders();
    

// JWT Setup
builder.Services.AddAuthentication((options) =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // "Bearer" (Magic string)
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // "Bearer" (Magic string)
})
    .AddJwtBearer(options =>
    {
        var signingKey = (string)builder.Configuration["JwtSettings:Key"]!;
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // simple security operations
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true, 
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"], 
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = symmetricSecurityKey
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

// serilog
app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
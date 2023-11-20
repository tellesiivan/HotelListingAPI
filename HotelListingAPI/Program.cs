using System.Text;
using HotelListingAPI.Configurations;
using HotelListingAPI.Data;
using HotelListingAPI.Middleware;
using HotelListingAPI.Services;
using HotelListingAPI.Services.AuthManager;
using HotelListingAPI.Services.Country;
using HotelListingAPI.Services.Hotel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
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

// versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    // 1.0
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
            // declares which version is declared and used at the time
            new QueryStringApiVersionReader("api-version"),
            new HeaderApiVersionReader("X-Version"),
            new MediaTypeApiVersionReader("ver")
    );
});


// version api explorer
builder.Services.AddVersionedApiExplorer(options =>
{
    // specify how we want our versioning to look(format)
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// add app caching(persist api response for specified amount of time before making a request to the db)
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024;
    options.UseCaseSensitivePaths = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// watch for exceptions been thrown 
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

// enable middleware
app.UseResponseCaching();

app.Use(async (context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
    {
        Public = true,
        MaxAge = TimeSpan.FromSeconds(10)
    };
    context.Response.Headers[HeaderNames.Vary] = new string[] { "Accept-Encoding" };
    await next();
});

// serilog
app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
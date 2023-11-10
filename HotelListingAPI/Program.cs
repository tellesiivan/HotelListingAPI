using HotelListingAPI.Configurations;
using HotelListingAPI.Data;
using Microsoft.EntityFrameworkCore;
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowAll");

// serilog
app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
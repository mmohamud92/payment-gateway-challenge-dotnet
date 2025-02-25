using PaymentGateway.Api.Middleware;
using PaymentGateway.Application;
using PaymentGateway.Application.DependencyInjections;
using PaymentGateway.Infrastructure.DependencyInjections;
using PaymentGateway.Infrastructure.Settings;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Configuration.AddTestConfiguration();

// Add services to the container.
IServiceCollection services = builder.Services;
services.AddInfrastructureServices(builder.Configuration);
services.AddApplicationServices();

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
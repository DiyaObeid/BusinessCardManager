// Program.cs serves as the entry point for the Business Card Manager application. 
// It configures and builds the application by setting up essential services such as 
// dependency injection, database context, AutoMapper for object mapping, and Swagger 
// for API documentation. The application is designed to support HTTPS, automatic 
// controller mapping, and environment-based configurations, ensuring a seamless 
// development experience and efficient service handling.

using BusinessCardManager.Core.IMapping;
using BusinessCardManager.Core.IRepositories;
using BusinessCardManager.Infrastructure;
using BusinessCardManager.Infrastructure.Repository;
using BusinessCardManager.Service.Contract.IBusinessCardContract;
using BusinessCardManager.Service.Implementation.BusinessCardImplementations;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin() // Allows requests from any origin
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});



// Register the encoding provider
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); 

// Add services to the container.
builder.Services.AddControllers(); // Registers the MVC controllers
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BusinessCardManagerDb"))); // Configures the database context to use SQL Server

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // Configures AutoMapper for mapping objects
builder.Services.AddScoped<IMappingService, MappingService>(); // Registers the mapping service
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>)); // Registers a generic repository for CRUD operations
builder.Services.AddScoped<IBusinessCardService, BusinessCardService>(); // Registers the business card service

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); // Adds support for endpoint exploration
builder.Services.AddSwaggerGen(); // Configures Swagger for API documentation

var app = builder.Build(); // Builds the application pipeline

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BusinessCard API V1");
    c.RoutePrefix = "swagger"; // Use an empty string if you want Swagger to be the root page
});

app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS
app.UseAuthorization(); // Enables authorization middleware
app.MapControllers(); // Maps attribute-routed controllers to the application
app.Run(); // Runs the application

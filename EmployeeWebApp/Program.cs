using EmployeeWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Nest;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEmployeeFactory, EmployeeRepository>();

// string currentDirectory = Directory.GetCurrentDirectory();
// string relativeLaunchSettingsPath = Path.Combine(currentDirectory,"Properties" ,"launchSettings.json");
//
// var configBuilder = new ConfigurationBuilder()
//     .AddJsonFile(relativeLaunchSettingsPath)
//     .Build();
//
// string username = configBuilder["profiles:http:environmentVariables:DB_USERNAME"];
// string password = configBuilder["profiles:http:environmentVariables:DB_PASSWORD"];
// string dbName = configBuilder["profiles:http:environmentVariables:DB_NAME"];

var appSettingsBuilder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

string connectionString = appSettingsBuilder["ConnectionStrings:DefaultConnection"]
    .Replace("${DB_NAME}", "EmployeeDB")
    .Replace("${DB_USERNAME}", "sa")
    .Replace("${DB_PASSWORD}", "Password_123");

builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(connectionString));

// Create an instance of the Elasticsearch client
var settings = new ConnectionSettings(new Uri("http://elasticsearch:9200")).DefaultIndex("employee_web_app");
builder.Services.AddSingleton<IElasticClient>(new ElasticClient(settings));


// Define some important constants and the activity source.
// These can come from a config file, constants file, etc.
var serviceName = "EmployeeOTELServiceFromWebApp";
var serviceVersion = "1.0.0";

// Configure important OpenTelemetry settings, the console exporter
builder.Services.AddOpenTelemetry()
    .WithTracing(b =>
    {
        b
            .AddSource(serviceName)
            .ConfigureResource(resource =>
                resource.AddService(
                    serviceName: serviceName,
                    serviceVersion: serviceVersion))
            .AddAspNetCoreInstrumentation()
            .AddJaegerExporter(exporterOptions =>
            {
                exporterOptions.ExportProcessorType = ExportProcessorType.Batch;
            }); // Adding jaeger exporter
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Index}/{id?}");

app.Run();
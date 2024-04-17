var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var configBuilder = new ConfigurationBuilder()
    .AddJsonFile("launchSettings.json")
    .Build();

string username = configBuilder["profiles:Development:environmentVariables:DB_USERNAME"];
string password = configBuilder["profiles:Development:environmentVariables:DB_PASSWORD"];

var appSettingsBuilder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

string connectionString = appSettingsBuilder["ConnectionStrings:DefaultConnection"]
    .Replace("<YourDatabaseName>", "your-database-name")
    .Replace("${DB_USERNAME}", username)
    .Replace("${DB_PASSWORD}", password);


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


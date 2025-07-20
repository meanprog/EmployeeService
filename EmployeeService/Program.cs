using EmployeeService.Repositories;
using EmployeeService.Repositories.DbConnection;
using Dapper;
using EmployeeService.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IEmployeeProvider, EmployeeProvider>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
builder.Logging.AddConsole();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

await WaitForDbConnection(app.Services);

Console.WriteLine("EmployeeService started. Press Ctrl+C to stop.");
app.Run();
builder.Logging.AddConsole();
async Task WaitForDbConnection(IServiceProvider services)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    var connectionFactory = services.GetRequiredService<IDbConnectionFactory>();
    var configuration = services.GetRequiredService<IConfiguration>();
    
    var maxRetries = configuration.GetValue<int>("Database:MaxConnectionRetries", 5);
    var initialDelay = configuration.GetValue<int>("Database:InitialRetryDelayMs", 5000);
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            await using var connection = connectionFactory.CreateConnection();
            await connection.OpenAsync();
            
            // Проверяем, что можем выполнить простой запрос
            await connection.ExecuteScalarAsync("SELECT 1");
            
            logger.LogInformation("Database connection established");
            return;
        }
        catch (Exception ex) when (attempt < maxRetries)
        {
            var delay = TimeSpan.FromMilliseconds(initialDelay * Math.Pow(2, attempt - 1));
            logger.LogWarning(ex, 
                "Database connection attempt {Attempt}/{MaxRetries} failed. Retrying in {DelayMs}ms",
                attempt, maxRetries, delay.TotalMilliseconds);
            
            await Task.Delay(delay);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, 
                "Fatal error: Failed to establish database connection after {MaxRetries} attempts",
                maxRetries);
            Environment.Exit(1);
        }
    }
}
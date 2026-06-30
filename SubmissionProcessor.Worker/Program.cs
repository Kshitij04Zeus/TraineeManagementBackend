using SubmissionProcessor.Worker.Workers; 
using SubmissionProcessor.Worker.Utilities;
using Serilog;
using SubmissionProcessor.Worker.Data;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using System.Net.Http.Headers;
using Microsoft.Extensions.Http.Resilience;
using SubmissionProcessor.Worker.Services;
using Polly;
using Polly.Fallback;
using SubmissionProcessor.Worker.Contracts;
using System.Net.Http.Json; 
using DotNetEnv;

DotNetEnv.Env.Load();

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddEnvironmentVariables();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() 
    .WriteTo.Console() 
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(logEvent => 
            logEvent.Properties.TryGetValue("SourceContext", out var value) && 
            (value.ToString().Contains("Microsoft") || value.ToString().Contains("System")))
        .WriteTo.File(
            path: "logs/app-.txt", 
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}"))        
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(); 

builder.Services.AddHostedService<SubmissionProcessingWorker>();
var connnectionString= builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options=>{
    options.UseMySql(connnectionString,ServerVersion.AutoDetect(connnectionString));
});

builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMq")
);
var trainingDirectorySection = builder.Configuration.GetSection("TrainingDirectory");
builder.Services.Configure<TrainingDirectorySettings>(trainingDirectorySection);

var directorySettings = trainingDirectorySection.Get<TrainingDirectorySettings>() 
    ?? throw new InvalidOperationException("TrainingDirectory settings are missing!");

builder.Services.AddHttpClient<ITrainingDirectoryClient, TrainingDirectoryClient>(client =>
{
    client.BaseAddress = new Uri(directorySettings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(directorySettings.TimeOutSeconds);
 
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
})
.AddStandardResilienceHandler(options =>
{
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(8);
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);
 
    // retry only transient failures
    options.Retry.MaxRetryAttempts = 2;
    options.Retry.Delay = TimeSpan.FromSeconds(1);
 
    // circuit breaker
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
    options.CircuitBreaker.MinimumThroughput = 2;
    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(10);
});

var host = builder.Build();
host.Run();

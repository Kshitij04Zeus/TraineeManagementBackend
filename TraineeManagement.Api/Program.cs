using Microsoft.Extensions.Options;
using TraineeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using System.Text.Json.Serialization;
using TraineeManagement.Api.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;
using System.ComponentModel;
using TraineeManagement.Api.Middleware;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Serilog; 
using TraineeManagement.Api.Services.HealthCheckServices;
using DotNetEnv;
using Microsoft.Extensions.Diagnostics.HealthChecks;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

var connnectionString= builder.Configuration.GetConnectionString("DefaultConnection");
var jwtSettings=builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
builder.Services.AddDbContext<AppDbContext>(options=>{
    options.UseMySql(connnectionString,ServerVersion.AutoDetect(connnectionString));
});
string redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379,abortConnect=false";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
{
    return ConnectionMultiplexer.Connect(redisConnectionString);
});
builder.Logging.ClearProviders();
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


builder.Host.UseSerilog(); 

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddControllers();  
builder.Services.AddScoped<ITraineeService,TraineeService>();
builder.Services.AddScoped<IMentorService,MentorService>();
builder.Services.AddScoped<ILearningTaskService,LearningTaskService>();
builder.Services.AddScoped<IJwtService,JwtService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<ISubmissionService,SubmissionService>();
builder.Services.AddScoped<IReviewService,ReviewService>();
builder.Services.AddScoped<ITaskAssignmentService,TaskAssignmentService>();
builder.Services.AddScoped<IFileStorageService,FileStorageService>();
builder.Services.AddScoped<ISubmissionFileService,SubmissionFileService>();
builder.Services.AddScoped<ICacheService,CacheService>();
builder.Services.AddScoped<IRabbitMqConnectionFactory,RabbitMQConnectionFactory>();
builder.Services.AddScoped<ISubmissionProcessingPublisher,SubmissionProcessingPublisher>();
builder.Services.AddScoped<IProcessingJobService,ProcessingJobService>();
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);
builder.Services.Configure<FileStorageSettings>(
    builder.Configuration.GetSection("FileStorage")
);
builder.Services.Configure<RedisSettings>(
    builder.Configuration.GetSection("Redis")
);
builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMq")
);
builder.Services.AddHealthChecks()
.AddCheck<MySqlHealthCheck>("mysql")
.AddCheck<RedisHealthCheck>("redis")
.AddCheck<RabbitMqHealthCheck>("reabbitmq");

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options=>{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title="Trainee Management Api",
        Version="v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name="Authorization",
        In=ParameterLocation.Header,
        Type=SecuritySchemeType.ApiKey,
        Scheme="Bearer",
        BearerFormat="JWT",
        Description="Enter your JWT status below"
    });

    options.AddSecurityRequirement(document=> new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer",document)]=[]
    }
    );
});

builder.Services.AddControllers().AddJsonOptions(
    options =>
    {
       options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); 
    }
);

var myPolicy="ReactPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myPolicy,
                      policy  =>
                      {
                          policy.WithOrigins("http://localhost:3000",
                                              "http://localhost:5173");
                        policy.AllowCredentials();
                        policy.WithHeaders("accept","content-type","origin","Authorization");
                      });

});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        };
    });


builder.Services.AddAuthorization();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();
 
    await context.Database.MigrateAsync();
 
    await DbSeeder.SeedAdminUserAsync(context);
}
app.UseExceptionHandler(); 

app.UseHttpsRedirection();
app.UseCors(myPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // options.DocumentPath="/openapi/v1.json";
        options.SwaggerEndpoint("/swagger/v1/swagger.json","Trainee Management Api V1");
    });
}

app.MapControllers();

app.Run();
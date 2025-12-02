using System.Net.Http.Headers;
using System.Reflection;
using FeedbackAnalyzer;
using FeedbackAnalyzer.Contracts.IRepository;
using FeedbackAnalyzer.Contracts.IServices;
using FeedbackAnalyzer.Data.Repositories;
using FeedbackAnalyzer.Data.Services;
using FeedbackAnalyzer.Helpers.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string CORS_NAME = "Analyzer_Origins";

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// App Services
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

builder.Services.AddHttpClient<IAnalysisService, AnalysisService>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", builder.Configuration["OpenAI:ApiKey"]);
});

builder.Services.AddMemoryCache();

// Validation + Mapping
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(typeof(GlobalMapping));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(CORS_NAME, policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Exception Handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Middleware
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(CORS_NAME);
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Welcome to the AI Analyzer API!");
app.MapControllers();

app.Run();

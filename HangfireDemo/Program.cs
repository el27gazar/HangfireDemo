using Hangfire;
using HangfireDemo;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();
builder.Services.AddTransient<DailyMessageJob>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var user = builder.Configuration["HangfireDashboard:User"]!;
var password = builder.Configuration["HangfireDashboard:Password"]!;

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new BasicAuthDashboard(user, password) }
});

TimeZoneInfo cairo;
try { cairo = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"); }  
catch (TimeZoneNotFoundException) { cairo = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo"); }

app.Services.GetRequiredService<IRecurringJobManager>()
    .AddOrUpdate<DailyMessageJob>(
        "daily-noon-message",
        job => job.Run(),
        Cron.Daily(12),
        new RecurringJobOptions { TimeZone = cairo });

app.UseAuthorization();

app.MapControllers();

app.Run();

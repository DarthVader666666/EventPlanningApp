using EventPlanning.Data;
using EventPlanning.Data.Entities;
using EventPlanning.Bll.Interfaces;
using EventPlanning.Bll.Services;
using EventPlanning.Bll.Services.SqlRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EventPlanning.Api.Configurations;
using Mapster;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logs => 
{
    logs.AddConsole();
    logs.AddAzureWebAppDiagnostics();
});

builder.Services.AddControllers();

builder.Services.AddCors(opts => opts.AddPolicy("AllowClient", policy => 
    policy.WithOrigins(builder.Configuration["ClientUrl"] ?? throw new ArgumentNullException("No ClientUrl in configuration"))
    .AllowAnyMethod()
    .AllowAnyHeader())
);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "Test",
        ValidAudience = "Test",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecurityKey"] ?? throw new ArgumentNullException("SecurityKey", "SecurityKey can't be null"))),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
    };
});

builder.Services.ConfigureAutomapper();
builder.Services.AddScoped<CryptoService>();

var dbDir = Path.Combine(builder.Environment.ContentRootPath, "DB");
Directory.CreateDirectory(dbDir);
var dbFile = builder.Configuration["EventPlanningDb"];
var dbPath = Path.Combine(dbDir, dbFile);
var connectionString = $"Data Source={dbPath}";

builder.Services.AddDbContext<EventPlanningDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddScoped<IRepository<Event>, EventRepository>();
builder.Services.AddScoped<IRepository<UserEvent>, UserEventRepository>();
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IRepository<Role>, RoleRepository>();
builder.Services.AddScoped<IRepository<Theme>, ThemeRepository>();

builder.Services.AddScoped<EmailSender>();
builder.Services.AddMapster();

var provider = builder?.Services?.BuildServiceProvider();
using var scope = provider?.CreateScope();

await MigrateDatabase(scope);

var app = builder!.Build();

app.UseCors("AllowClient");
app.UseSession();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller}/{action}/{id?}"
    );

app.MapFallbackToFile("/index.html");

app.Run();

async Task MigrateDatabase(IServiceScope? scope)
{
    var dbContext = scope?.ServiceProvider.GetRequiredService<EventPlanningDbContext>();
    dbContext?.Database.Migrate();
}
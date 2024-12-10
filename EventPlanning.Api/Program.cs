using EventPlanning.Data;
using EventPlanning.Data.Entities;
using EventPlanning.Bll.Interfaces;
using EventPlanning.Bll.Services;
using EventPlanning.Bll.Services.JsonRepositories;
using EventPlanning.Bll.Services.SqlRepositories;
using JsonFlatFileDataStore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EventPlanning.Api.Configurations;
using Microsoft.Extensions.Logging;

var jsonFileCreated = false;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

if (builder.Environment.IsDevelopment())
{
    var connectionString = builder.Configuration.GetConnectionString("EventDb") ?? throw new ArgumentNullException("EventDb", "Connection string can't be null");
    builder.Services.AddDbContext<EventPlanningDbContext>(options => options.UseSqlServer(connectionString));

    builder.Services.AddScoped<IRepository<Event>, EventRepository>();
    builder.Services.AddScoped<IRepository<UserEvent>, UserEventRepository>();
    builder.Services.AddScoped<IRepository<User>, UserRepository>();
    builder.Services.AddScoped<IRepository<Role>, RoleRepository>();
    builder.Services.AddScoped<IRepository<Theme>, ThemeRepository>();
}
else
{
    var path = $"{Directory.GetCurrentDirectory()}\\EventDbJson\\";

    if (!Directory.Exists(path))
    { 
        Directory.CreateDirectory(path);
    }

    path += "eventDb.json";

    if (!File.Exists(path))
    {
        var stream = File.Create(path);
        stream.Close();
        File.WriteAllText(path, "{}");
        jsonFileCreated = true;
    }

    builder.Services.AddScoped(serviceProvider => new DataStore(path, useLowerCamelCase: false));

    builder.Services.AddScoped<IRepository<Event>, EventJsonRepository>();
    builder.Services.AddScoped<IRepository<UserEvent>,UserEventJsonRepository>();
    builder.Services.AddScoped<IRepository<User>, UserJsonRepository>();
    builder.Services.AddScoped<IRepository<Role>, RoleJsonRepository>();
    builder.Services.AddScoped<IRepository<Theme>, ThemeJsonRepository>();
}

builder.Services.AddScoped<EmailSender>();

var provider = builder?.Services.BuildServiceProvider();
using var scope = provider?.CreateScope();
await MigrateSeedDatabase(scope, jsonFileCreated);

var app = builder?.Build();

app.UseCors("AllowClient");
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

async Task MigrateSeedDatabase(IServiceScope? scope, bool jsonFileCreated)
{
    if (builder.Environment.IsDevelopment())
    {
        var dbContext = scope?.ServiceProvider.GetRequiredService<EventPlanningDbContext>();
        dbContext?.Database.Migrate();
    }
    else if (jsonFileCreated)
    {
        var dataStore = scope?.ServiceProvider.GetRequiredService<DataStore>() ?? throw new ArgumentNullException("Could not get DataStore from DI");

        var userCollection = dataStore.GetCollection<User>();
        var roleCollection = dataStore.GetCollection<Role>();
        var userRoleCollection = dataStore.GetCollection<UserRole>();
        var themeCollection = dataStore.GetCollection<Theme>();
        var subThemeCollection = dataStore.GetCollection<SubTheme>();

        if (userCollection.Find(user => user.Email == builder.Configuration["AdminEmail"]).Count() == 0)
        {
            await userCollection.InsertOneAsync(new User { UserId = 1, Email = builder.Configuration["AdminEmail"], Password = builder.Configuration["AdminPassword"] });
            await roleCollection.InsertOneAsync(new Role { RoleId = 1, RoleName = "Admin" });
            await userRoleCollection.InsertOneAsync(new UserRole { RoleId = 1, UserId = 1 });
        }

        await themeCollection.InsertManyAsync(new List<Theme> { 
            new () { ThemeId = 1, ThemeName = "Music" },
            new () { ThemeId = 2, ThemeName = "Sport" },
            new () { ThemeId = 3, ThemeName = "Conference" },
            new () { ThemeId = 4, ThemeName = "Corporate Party" }
        });

        await subThemeCollection.InsertManyAsync(new List<SubTheme> {
            new () { SubThemeId = 1, ThemeId = 1, SubThemeName = "Rock Fest" },
            new () { SubThemeId = 2, ThemeId = 1, SubThemeName = "Classic orchestra" },
            new () { SubThemeId = 3, ThemeId = 1, SubThemeName = "Blues band" },
            new () { SubThemeId = 4, ThemeId = 2, SubThemeName = "Football match" },
            new () { SubThemeId = 5, ThemeId = 2, SubThemeName = "Fitness event" },
            new () { SubThemeId = 6, ThemeId = 2, SubThemeName = "Yoga class" },
            new () { SubThemeId = 7, ThemeId = 3, SubThemeName = "IT club" },
            new () { SubThemeId = 8, ThemeId = 3, SubThemeName = "Buisness coaching" },
            new () { SubThemeId = 9, ThemeId = 3, SubThemeName = "Literature talks" }
        });
    }
}
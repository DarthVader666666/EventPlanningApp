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

//builder.Services.AddDataProtection()
//       .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
//       {
//           EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM
//       });

builder.Services.ConfigureAutomapper();
builder.Services.AddScoped<CryptoService>();

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

#pragma warning disable ASP0000
var provider = builder?.Services?.BuildServiceProvider();
#pragma warning restore ASP0000
using var scope = provider?.CreateScope();
await MigrateSeedDatabase(scope, jsonFileCreated);

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

async Task MigrateSeedDatabase(IServiceScope? scope, bool jsonFileCreated)
{
    if (builder!.Environment.IsDevelopment())
    {
        var dbContext = scope?.ServiceProvider.GetRequiredService<EventPlanningDbContext>();
        dbContext?.Database.Migrate();
    }
    else if (jsonFileCreated)
    {
        var dataStore = scope?.ServiceProvider.GetRequiredService<DataStore>() ?? throw new ArgumentNullException("Could not get DataStore from DI");
        await SeedJsonDb(dataStore, builder.Configuration);
    }
}

async Task SeedJsonDb(DataStore dataStore, IConfiguration configuration)
{
    var userCollection = dataStore.GetCollection<User>();
    var roleCollection = dataStore.GetCollection<Role>();
    var userRoleCollection = dataStore.GetCollection<UserRole>();
    var themeCollection = dataStore.GetCollection<Theme>();
    var subThemeCollection = dataStore.GetCollection<SubTheme>();
    var eventCollection = dataStore.GetCollection<Event>();

    var adminEmail = configuration["AdminEmail"] ?? throw new ArgumentNullException("AdminEmail is null.");
    var adminPassword = configuration["AdminPassword"] ?? throw new ArgumentNullException("AdminPassword is null.");

    if (userCollection.Find(user => user.Email == adminEmail).Count() == 0)
    {
        await userCollection.InsertOneAsync(new User { UserId = 1, Email = adminEmail, Password = adminPassword });
        await roleCollection.InsertOneAsync(new Role { RoleId = 1, RoleName = "Admin" });
        await roleCollection.InsertOneAsync(new Role { RoleId = 2, RoleName = "User" });
        await userRoleCollection.InsertOneAsync(new UserRole { RoleId = 1, UserId = 1 });
    }

    await themeCollection.InsertManyAsync(new List<Theme> {
            new () { ThemeId = 1, ThemeName = "Music" },
            new () { ThemeId = 2, ThemeName = "Sport" },
            new () { ThemeId = 3, ThemeName = "Conference" },
            new () { ThemeId = 4, ThemeName = "Corporate Party" },
            new () { ThemeId = 5, ThemeName = "Theatre" },
            new () { ThemeId = 6, ThemeName = "Art Exhibition" }
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
            new () { SubThemeId = 9, ThemeId = 3, SubThemeName = "Literature talks" },
            new () { SubThemeId = 10, ThemeId = 4, SubThemeName = "Firm Anniversary" },
            new () { SubThemeId = 11, ThemeId = 4, SubThemeName = "Employee Award" },
            new () { SubThemeId = 12, ThemeId = 4, SubThemeName = "Other occasion" },
            new () { SubThemeId = 13, ThemeId = 5, SubThemeName = "Ballet" },
            new () { SubThemeId = 14, ThemeId = 5, SubThemeName = "Opera" },
            new () { SubThemeId = 15, ThemeId = 5, SubThemeName = "Conceptual performance" },
            new () { SubThemeId = 16, ThemeId = 6, SubThemeName = "Modern Art" },
            new () { SubThemeId = 17, ThemeId = 6, SubThemeName = "Paintings" },
            new () { SubThemeId = 18, ThemeId = 6, SubThemeName = "Sculptures" }
            });

    await eventCollection.InsertManyAsync(new List<Event> {
            new() { EventId = 1, ThemeId = 1, SubThemeId = 1, Title = "Nirvana Tribute", Date = DateTime.Now + TimeSpan.FromDays(10), Location = "Minsk, RE:PUBLIC", Address = "vulica Prytyckaha 62", Participants = "Mutnae Voka, Gogo Band", AmountOfVacantPlaces = 600 },
            new() { EventId = 2, ThemeId = 3, SubThemeId = 7, Title = "Syberry corp conference", Date = DateTime.Now + TimeSpan.FromDays(15), Location = "Minsk, Beijing Hotel", Address = "vulica Chyrvonaarmiejskaja 36", Participants = "A.Anovich, B.Berovich, Z.Zhydkivich", AmountOfVacantPlaces = 1000 },
            new() { EventId = 3, ThemeId = 6, SubThemeId = 13, Title = "Conseptual Art Exhibition", Date = DateTime.Now + TimeSpan.FromDays(20), Location = "Minsk, National Arts Museum", Address = "vulica Lenina 20", Participants = "A.Bondar, B.Govnar, Z.Zhopsky", AmountOfVacantPlaces = 200 },
        });
}
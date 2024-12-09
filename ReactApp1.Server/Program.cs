using EventPlanning.Bll.Interfaces;
using EventPlanning.Bll.Services.JsonRepositories;
using EventPlanning.Bll.Services;
using EventPlanning.Bll.Services.SqlRepositories;
using JsonFlatFileDataStore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EventPlanning.Data;
using EventPlanning.Data.Entities;
using Microsoft.EntityFrameworkCore;
using EventPlanning.Server.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddLogging(logs => 
{
    logs.AddConsole();
    logs.AddAzureWebAppDiagnostics();
});

builder.Services.AddControllers();
builder.Services.AddCors(opts => opts.AddPolicy("AllowClient", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "Test",
        ValidAudience = "Test",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TestTestTestTestTestTestTestTestTestTestTestTest")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
    };
});

builder.Services.ConfigureAutomapper();

if (builder.Environment.IsDevelopment())
{
    var connectionString = builder.Configuration.GetConnectionString("EventDb");
    builder.Services.AddDbContext<EventPlanningDbContext>(options => options.UseSqlServer(connectionString));

    builder.Services.AddScoped<IRepository<Event>, EventRepository>();
    builder.Services.AddScoped<IRepository<UserEvent>, UserEventRepository>();
    builder.Services.AddScoped<IRepository<User>, UserRepository>();
    builder.Services.AddScoped<IRepository<Role>, RoleRepository>();
    builder.Services.AddScoped<IRepository<Theme>, ThemeRepository>();
}
else
{
    var path = $"{Directory.GetCurrentDirectory()}\\eventDb.json";

    //if (!File.Exists(path))
    //{
    //    File.Create(path);
    //}

    builder.Services.AddScoped<EventJsonRepository>();
    builder.Services.AddScoped<UserEventJsonRepository>();
    builder.Services.AddScoped<UserJsonRepository>();
    builder.Services.AddScoped<RoleJsonRepository>();
    builder.Services.AddScoped<ThemeJsonRepository>();
    builder.Services.AddScoped(serviceProvider => new DataStore(path, useLowerCamelCase: false));
}

//builder.Services.AddScoped<EmailSender>();

using var scope = builder.Services?.BuildServiceProvider()?.CreateScope();
//await MigrateSeedDatabase(scope);

var app = builder.Build();

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

async Task MigrateSeedDatabase(IServiceScope? scope)
{
    if (builder.Environment.IsDevelopment())
    {
        var dbContext = scope?.ServiceProvider.GetRequiredService<EventPlanningDbContext>();
        dbContext?.Database.Migrate();
    }
    else
    {
        var userJsonRepository = scope?.ServiceProvider.GetRequiredService<IRepository<User>>();
        var roleJsonRepository = scope?.ServiceProvider.GetRequiredService<IRepository<Role>>();
        var userRoleCollection = scope?.ServiceProvider.GetRequiredService<DataStore>().GetCollection<UserRole>();

        if (!(await userJsonRepository.GetListAsync()).Any(user => user.Email == builder.Configuration["AdminEmail"]))
        {
            var user = await userJsonRepository.CreateAsync(new User { Email = builder.Configuration["AdminEmail"], Password = builder.Configuration["AdminPassword"] });
            var role = await roleJsonRepository.CreateAsync(new Role { RoleName = "Admin" });

            if (user != null && role != null)
            {
                await userRoleCollection.InsertOneAsync(new UserRole { RoleId = role.RoleId, UserId = user.UserId });
            }
        }
    }
}
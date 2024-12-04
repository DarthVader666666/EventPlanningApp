using EventPlanning.Bll.Interfaces;
using EventPlanning.Bll.Services;
using EventPlanning.Data;
using EventPlanning.Data.Entities;
using EventPlanning.Api.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using EventPlanning.Bll.Services.SqlRepositories;
using JsonFlatFileDataStore;
using EventPlanning.Bll.Services.JsonRepositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(opts => opts.AddPolicy("AllowClient", policy =>
policy.WithOrigins(builder.Configuration["ClientUrl"] ?? string.Empty)
    .AllowAnyHeader()
    .AllowAnyMethod()
    ));

builder.Services.AddAuthentication("JwtBearerDefaults.AuthenticationScheme").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "Issuer",
        ValidAudience = "Audience",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecurityKey")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.ConfigureAutomapper();

var connectionString = builder.Configuration.GetConnectionString("EventDb");
builder.Services.AddDbContext<EventPlanningDbContext>(options => options.UseSqlServer(connectionString));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IRepository<Event>, EventRepository>();
    builder.Services.AddScoped<IRepository<UserEvent>, UserEventRepository>();
    builder.Services.AddScoped<IRepository<User>, UserRepository>();
    builder.Services.AddScoped<IRepository<Role>, RoleRepository>();
    builder.Services.AddScoped<IRepository<Theme>, ThemeRepository>();
}
else
{
    var path = $"{Directory.GetCurrentDirectory()}\\eventDb.json";

    builder.Services.AddScoped<IRepository<Event>, EventJsonRepository>();
    builder.Services.AddScoped<IRepository<UserEvent>, UserEventJsonRepository>();
    builder.Services.AddScoped<IRepository<User>, UserJsonRepository>();
    builder.Services.AddScoped<IRepository<Role>, RoleJsonRepository>();
    builder.Services.AddScoped<IRepository<Theme>, ThemeJsonRepository>();
    builder.Services.AddScoped(serviceProvider => new DataStore(path, useLowerCamelCase: false));
}

builder.Services.AddScoped<EmailSender>();

using var scope = builder.Services?.BuildServiceProvider()?.CreateScope();
await MigrateSeedDatabase(scope);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowClient");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//app.MapControllerRoute("default", "api/{controller}/{action}");

//app.MapPost("events", () => Results.Ok())
//    .RequireAuthorization("Admin");

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
                await userRoleCollection.InsertOneAsync(new UserRole {  RoleId = role.RoleId, UserId = user.UserId });
            }
        }
    }
}
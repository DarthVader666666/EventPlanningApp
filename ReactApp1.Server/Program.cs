using EventPlanning.Bll;
using EventPlanning.Bll.Services.SqlRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(opts => opts.AddPolicy("AllowClient", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "Test",
        ValidAudience = "Test",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Test")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
    };
});

builder.Services.AddScoped<BllService>();
builder.Services.AddScoped<EventRepository>();

var app = builder.Build();

app.UseCors("AllowClient");

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

//app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller}/{action}/{id?}"
    );

app.MapFallbackToFile("/index.html");

app.Run();

using backend.Entities;
using backend.Entities.Contexts;
using backend.Hubs;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddDbContext<UserEntityDbContext>(opts => opts.UseSqlite("data source=database.db"));
builder.Services.AddDbContext<TaskEntityDbContext>(opts => opts.UseSqlite("data source=database.db"));
builder.Services.AddIdentity<UserEntity, IdentityRole>()
    .AddEntityFrameworkStores<UserEntityDbContext>()
    .AddDefaultTokenProviders();
var authSetting = builder.Configuration.GetSection("AuthSettings");
builder.Services.AddAuthentication(authOpts =>
{
    authOpts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authOpts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    authOpts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOpts =>
{
    jwtOpts.RequireHttpsMetadata = false;
    jwtOpts.SaveToken = true;
    jwtOpts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSetting.GetSection("securityKey").Value!)),
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidAudience = authSetting["validAudience"],
        ValidIssuer = authSetting["validIssuer"]
    };
});

builder.Services.AddSignalR();
builder.Services.AddCors();

builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<ITaskService, TaskService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(sg =>
{
    sg.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    sg.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options =>
{
    options.WithOrigins("http://localhost:5400", "http://localhost:5204", "https://localhost:5400", "https://localhost:5204", "http://localhost:11434")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationsHub>("/signalr");

app.MapFallbackToFile("/index.html");

app.Run();

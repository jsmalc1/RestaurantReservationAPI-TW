using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestaurantAPI.Data;
using RestaurantAPI.Repositories;
using RestaurantAPI.Services;
using System.Text;
using System.Text.Json.Serialization;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/restaurant-logs.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbContext<RestaurantDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

    // Add services to the container.
    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Restaurant API", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
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
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
    });

    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IReservationService, ReservationManagementService>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };
        });

    builder.Services.AddAuthorization();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
    });

    var app = builder.Build();

    app.UseMiddleware<RestaurantAPI.Middlewares.ExceptionMiddleware>();

    app.UseCors("AllowAll");

    app.UseDefaultFiles();
    app.UseStaticFiles();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<RestaurantAPI.Data.RestaurantDbContext>();

        if (!context.Users.Any(u => u.Email == "admin"))
        {
            var adminUser = new RestaurantAPI.Models.User
            {
                Email = "admin",
                FirstName = "Glavni",
                LastName = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123")
            };
            context.Users.Add(adminUser);
            context.SaveChanges(); 

            context.UserRoles.Add(new RestaurantAPI.Models.UserRole
            {
                UserId = adminUser.Id,
                RoleId = 1
            });
            context.SaveChanges();
        }

        if (!context.Tables.Any())
        {
            context.Tables.AddRange(
                new RestaurantAPI.Models.RestaurantTable { TableNumber = 1, Capacity = 2, LocationZone = "Terasa" },
                new RestaurantAPI.Models.RestaurantTable { TableNumber = 2, Capacity = 4, LocationZone = "Unutrašnjost" }
            );
            context.SaveChanges();
        }

        if (!context.TimeSlots.Any())
        {
            context.TimeSlots.AddRange(
                new RestaurantAPI.Models.TimeSlot { StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(20, 0, 0), IsActive = true },
                new RestaurantAPI.Models.TimeSlot { StartTime = new TimeSpan(20, 0, 0), EndTime = new TimeSpan(22, 0, 0), IsActive = true }
            );
            context.SaveChanges();
        }

        if (!context.SpecialServices.Any())
        {
            context.SpecialServices.AddRange(
                new RestaurantAPI.Models.SpecialService { Name = "Rodendanska torta", Price = 15.00m },
                new RestaurantAPI.Models.SpecialService { Name = "Sampanjac", Price = 25.00m }
            );
            context.SaveChanges();
        }
    }

    app.Run();
} catch (Exception e)
{
    Log.Fatal(e, "Aplikacija srusila");
}
finally
{
    Log.CloseAndFlush();
}


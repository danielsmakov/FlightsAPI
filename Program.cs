using FlightsAPI;
using FlightsAPI.Handlers;
using FlightsAPI.Interfaces;
using FlightsAPI.Middlewares;
using FlightsAPI.Models.Entities;
using FlightsAPI.Repositories;
using FlightsAPI.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace FlightsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Настройка логирования
            ConfigureLogging();

            var builder = WebApplication.CreateBuilder(args);

            // Настройка служб
            ConfigureServices(builder);

            var app = builder.Build();

            // Настройка HTTP-конвейера
            ConfigureMiddleware(app);

            app.Run();
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Логирование в файл
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Подключение базы данных
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Настройка MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetFlightsQueryHandler).Assembly));

            // Регистрация репозиториев и сервисов
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<ITokenService, TokenService>();

            // Настройка Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Настройка аутентификации и JWT
            ConfigureAuthentication(builder);

            // Настройка контроллеров и FluentValidation
            builder.Services.AddControllers().AddFluentValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<GetFlightsQueryValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateFlightCommandValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UpdateFlightCommandValidator>();

            // Настройка Swagger
            ConfigureSwagger(builder);

            // Эндпоинты API
            builder.Services.AddEndpointsApiExplorer();
        }

        private static void ConfigureAuthentication(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
        }

        private static void ConfigureSwagger(WebApplicationBuilder builder)
        {
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            builder.Services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(xmlPath);
            });
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            // Обработка исключений
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Миграция базы данных
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}

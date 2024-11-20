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
            // ��������� �����������
            ConfigureLogging();

            var builder = WebApplication.CreateBuilder(args);

            // ��������� �����
            ConfigureServices(builder);

            var app = builder.Build();

            // ��������� HTTP-���������
            ConfigureMiddleware(app);

            app.Run();
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // ����������� � ����
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // ����������� ���� ������
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ��������� MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetFlightsQueryHandler).Assembly));

            // ����������� ������������ � ��������
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<ITokenService, TokenService>();

            // ��������� Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // ��������� �������������� � JWT
            ConfigureAuthentication(builder);

            // ��������� ������������ � FluentValidation
            builder.Services.AddControllers().AddFluentValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<GetFlightsQueryValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateFlightCommandValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UpdateFlightCommandValidator>();

            // ��������� Swagger
            ConfigureSwagger(builder);

            // ��������� API
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
            // ��������� ����������
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // �������� ���� ������
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

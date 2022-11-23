using System;
using Scrutor;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Settings;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using GoFoodBeverage.Infrastructure.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using GoFoodBeverage.Loging.Serilog;

namespace GoFoodBeverage.Services
{
    public static class RegisterServices
    {
        public static IServiceCollection AutoInjectServices(this IServiceCollection services, IConfiguration configuration)
        {
            /// Manual config for mediator's target project
            //services.AddMediatR(AppDomain.CurrentDomain.Load("GoFoodBeverage.Application"));

            services.AddDbContext<GoFoodBeverageDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();
            });

            #region Services


            #endregion

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient();
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                var jsonSerializerSettings = options.SerializerSettings;
                jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            return services;
        }

        public static IServiceCollection WithScopedLifetime<T>(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<T>()
                .AddClasses()
                .AsImplementedInterfaces()
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsMatchingInterface()
                .WithScopedLifetime());

            return services;
        }

        public static IServiceCollection WithTransientLifetime<T>(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<T>()
                .AddClasses()
                .AsImplementedInterfaces()
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsMatchingInterface()
                .WithTransientLifetime());

            return services;
        }

        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register settings
            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
            services.Configure<JWTSettings>(configuration.GetSection(nameof(JWTSettings)));
            services.Configure<DomainFE>(configuration.GetSection(nameof(DomainFE)));
            services.Configure<SendGridMailSettings>(configuration.GetSection(nameof(SendGridMailSettings)));
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

            var jwtSettings = configuration.GetSection(nameof(JWTSettings)).Get<JWTSettings>();
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtSettings.SecretBytes)
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        /// SecurityTokenExpiredException
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    },
                };
            });

            return services;
        }

        public static IServiceCollection RegisterApplicationInsightsLogging(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<Serilog.ILogger>(Serilog.Log.Logger);
            services.EnableSqlCommandTextInstrumentation(configuration);
            services.SetupApplicationInsightsTelemetry(configuration);
            services.CreateLogger(configuration);

            return services;
        }
    }
}

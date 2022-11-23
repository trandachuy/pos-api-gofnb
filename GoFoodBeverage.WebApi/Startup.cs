using GoFoodBeverage.Application;
using GoFoodBeverage.Application.Mappings;
using GoFoodBeverage.Application.Middlewares;
using GoFoodBeverage.Common.Providers.DomainUrl;
using GoFoodBeverage.Common.AutoWire;
using GoFoodBeverage.Infrastructure.Repositories;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.Repositories;
using GoFoodBeverage.Payment.MoMo;
using GoFoodBeverage.Payment.VNPay;
using GoFoodBeverage.Services;
using GoFoodBeverage.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using GoFoodBeverage.Domain.Enums;
using System;
using System.Linq;
using GoFoodBeverage.Common.Attributes.Permission;
using GoFoodBeverage.Services.User;
using GoFoodBeverage.Email;
using GoFoodBeverage.Interfaces.POS;
using GoFoodBeverage.Services.Order;
using GoFoodBeverage.Services.Customer;
using GoFoodBeverage.Application.Providers;
using GoFoodBeverage.MemoryCaching;
using GoFoodBeverage.Delivery;
using GoogleServices;

namespace GoFoodBeverage.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .WithExposedHeaders("Token-Expired", "Content-Disposition")
                );
            });

            services.AddOptions();
            services.AddControllers();
            services.AddHealthChecks();
            services.AddHttpContextAccessor();

            services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddSwaggerDocumentation(Configuration);

            services
                .AutoInjectServices(Configuration)
                .AddIdentityInfrastructure(Configuration)
                .RegisterApplicationInsightsLogging(Configuration);

            services.AddApplicationLayer();
            services.AddSingleton(AutoMapperConfig.Configure());
            services.AutoWire();

            // Auto register services from Application layer
            services.WithScopedLifetime<IGoFoodBeverageApplication>();
            services.WithScopedLifetime<IGoFoodBeverageStorage>();
            services.WithScopedLifetime<IGoFoodBeverageMemoryCaching>();
            services.WithScopedLifetime<IGoFoodBeverageDelivery>();
            services.WithScopedLifetime<IGoogleServices>();

            // Email configurations.
            services.AddScoped<IEmailSenderProvider, EmailSenderProvider>();

            services.AddScoped<IDateTimeService, DateTimeService>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<IUserActivityService, UserActivityService>();
            services.AddScoped<ICustomerSegmentActivityService, CustomerSegmentActivityService>();
            services.AddScoped<IMoMoPaymentService, MoMoPaymentService>();
            services.AddScoped<IVNPayService, VNPayService>();

            services.AddScoped<IDomainUrlProvider, DomainUrlProvider>();
            services.AddScoped<IUserProvider, Application.Providers.HttpUserProvider>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IInternalAccountRepository, InternalAccountRepository>();
            services.AddScoped<IUserActivityRepository, UserActivityRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IStoreBannerRepository, StoreBannerRepository>();

            #region Register permission handler and add permission policies
            services.AddScoped<IUserPermissionService, UserPermissionService>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddAuthorization(options =>
            {
                var permissions = Enum.GetValues(typeof(EnumPermission))
                                .Cast<EnumPermission>()
                                .Select(e => e)
                                .ToList();

                foreach (var permission in permissions)
                {
                    options.AddPolicy(permission.ToString(), 
                        policy => policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerDocumentation();
            }

            var config = (IConfiguration)app.ApplicationServices.GetService(typeof(IConfiguration));
            var enableSwagger = bool.Parse(config.GetValue<string>("EnableSwagger") ?? "false");
            if (enableSwagger || env.IsDevelopment())
            {
                app.UseSwaggerDocumentation();
            }

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()
                .WithExposedHeaders("Token-Expired", "Content-Disposition")
            );

            app.UseMiddleware(typeof(AdminAuthenticationMiddleware));
            app.UseMiddleware(typeof(AdminErrorHandlingMiddleware));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHealthChecks("/health");
        }
    }
}

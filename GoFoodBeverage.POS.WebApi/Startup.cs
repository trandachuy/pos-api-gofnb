using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GoFoodBeverage.POS.Application;
using GoFoodBeverage.POS.Application.Mappings;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Application.Middlewares;
using GoFoodBeverage.Payment.MoMo;
using GoFoodBeverage.Payment.VNPay;
using GoFoodBeverage.Interfaces.POS;
using System.Collections.Generic;
using GoFoodBeverage.Storage;
using GoFoodBeverage.Email;
using GoFoodBeverage.Services;
using GoFoodBeverage.Services.Store;
using GoFoodBeverage.Services.User;
using GoFoodBeverage.Infrastructure.Repositories;
using GoFoodBeverage.Services.Order;
using GoFoodBeverage.MemoryCaching;
using GoogleServices;
using GoFoodBeverage.Delivery;
using GoFoodBeverage.Services.Hubs;
using GoFoodBeverage.Common.AutoWire;

namespace GoFoodBeverage.POS.WebApi
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
            services.AddSignalR();
            services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .WithExposedHeaders("Token-Expired")
                    );
            });

            services.AddSingleton<IDictionary<string, UserConnectionModel>>(opts => new Dictionary<string, UserConnectionModel>());

            services.AddOptions();
            services.AddControllers();
            services.AddHealthChecks();
            services.AddHttpContextAccessor();
            services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            
            services.AddSwaggerDocumentation(Configuration);

            services
                .AutoInjectServices(Configuration)
                .AddIdentityInfrastructure(Configuration)
                .RegisterApplicationInsightsLogging(Configuration);

            services.AddApplicationLayer();
            services.AddSingleton(AutoMapperConfig.Configure());
            services.AutoWire();

            // Auto register services from Application layer
            services.WithScopedLifetime<IGoFoodBeveragePOSApplication>();
            services.WithScopedLifetime<IGoFoodBeverageStorage>();
            services.WithScopedLifetime<IGoFoodBeverageMemoryCaching>();
            services.WithScopedLifetime<IGoogleServices>();
            services.WithScopedLifetime<IGoFoodBeverageDelivery>();

            // Email configurations.
            services.AddScoped<IEmailSenderProvider, EmailSenderProvider>();

            services.AddScoped<IDateTimeService, DateTimeService>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<IUserActivityService, UserActivityService>();

            services.AddScoped<IMoMoPaymentService, MoMoPaymentService>();
            services.AddScoped<IVNPayService, VNPayService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IKitchenService, KitchenService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IDeliveryService, DeliveryService>();

            services.AddScoped<IUserProvider, Application.Providers.HttpUserProvider>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
                .WithExposedHeaders("Token-Expired")); // allow credentials

            app.UseMiddleware(typeof(PosAuthenticationMiddleware));
            app.UseMiddleware(typeof(PosErrorHandlingMiddleware));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<KitchenSessionHub>("/kitchen-session");
                endpoints.MapControllers();
            });

            app.UseHealthChecks("/health");
        }
    }
}

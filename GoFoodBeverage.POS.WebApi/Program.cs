using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;

namespace GoFoodBeverage.POS.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            var host = Host.CreateDefaultBuilder(args);
            host.ConfigureWebHostDefaults(webBuilder => {
                   webBuilder.UseStartup<Startup>();
               });

            host.ConfigureAppConfiguration((hostContext, config) =>
            {
                config.SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                /// Default
                config.AddJsonFile("appsettings.json", optional: true, false);

                /// Appsettings on local env
                config.AddJsonFile("appsettings.Development.json", optional: true, false);

                /// Appsettings on dev env
                config.AddJsonFile("appsettings.DevRelease.json", optional: true);

                /// Appsettings on staging env
                config.AddJsonFile("appsettings.Staging.json", optional: true);

                /// Appsettings on Production env
                config.AddJsonFile("appsettings.Production.json", optional: true);


#if DEBUG
                var appsettingLocal = Path.GetFullPath($"{Environment.CurrentDirectory}\\configuration\\appsettings.{Environment.MachineName}.json");
                if (File.Exists(appsettingLocal))
                {
                    config.AddJsonFile(appsettingLocal, optional: true, false);
                }
#endif

                config.AddEnvironmentVariables();
            });

            return host;
        }
           
    }
}

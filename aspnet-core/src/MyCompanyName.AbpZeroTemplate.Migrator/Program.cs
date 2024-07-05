using System;
using Abp;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Castle.Facilities.Logging;
using Abp.Castle.Logging.Log4Net;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace MyCompanyName.AbpZeroTemplate.Migrator
{
    public class Program
    {
        private static bool _skipConnVerification;

        public static void Main(string[] args)
        {
            ParseArgs(args);

            bool.TryParse(Environment.GetEnvironmentVariable("ASPNETCORE_Docker_Enabled"), out bool isDockerEnabled);

            using (var bootstrapper = AbpBootstrapper.Create<AbpZeroTemplateMigratorModule>())
            {
                bootstrapper.IocManager.IocContainer
                    .AddFacility<LoggingFacility>(f => f.UseAbpLog4Net()
                        .WithConfig("log4net.config")
                    );

                bootstrapper.Initialize();

                using (var migrateExecuter = bootstrapper.IocManager.ResolveAsDisposable<MultiTenantMigrateExecuter>())
                {
                    migrateExecuter.Object.Run(_skipConnVerification, isDockerEnabled);
                }

                if (_skipConnVerification || isDockerEnabled) return;

                Console.WriteLine("Press ENTER to exit...");
                Console.ReadLine();
            }
        }

        private static void ParseArgs(string[] args)
        {
            if (args.IsNullOrEmpty())
            {
                return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                webBuilder.UseIISIntegration();
                webBuilder.UseStartup<Abp.Configuration.Startup>();
                });
            }

            foreach (var arg in args)
            {
                if (arg == "-s")
                {
                    _skipConnVerification = true;
                }
            }
        }
    }
}

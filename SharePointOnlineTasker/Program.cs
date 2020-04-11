using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using NLog;
using NLog.Extensions.Logging;
using SharePointOnlineTasker.Interfaces;
using SharePointOnlineTasker.Interfaces.DriveTasks;
using SharePointOnlineTasker.Services;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace SharePointOnlineTasker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            LogManager.Configuration = new NLogLoggingConfiguration(configuration.GetSection("NLog"));
            Logger logger = LogManager.GetCurrentClassLogger();

            try
            {
                IServiceProvider serviceProvider = SetupDependencyInjection(configuration);
                ITasksRunner tasksRunner = serviceProvider.GetRequiredService<ITasksRunner>();
                await tasksRunner.ExecuteAsync();
            }
            catch (Exception ex)
            {
                // NLog: catch any exception and log it.
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        private static IServiceProvider SetupDependencyInjection(IConfiguration configuration)
        {
            IServiceCollection netCoreServicesCollection = RegisterNetCoreLibraries(configuration);
            ContainerBuilder autoFacContainerBuilder = new ContainerBuilder();
            autoFacContainerBuilder.Populate(netCoreServicesCollection);
            autoFacContainerBuilder.RegisterInstance(configuration).As<IConfiguration>();
            RegisterServices(autoFacContainerBuilder);
            IContainer container = autoFacContainerBuilder.Build();
            AutofacServiceProvider serviceProvider = new AutofacServiceProvider(container);
            return serviceProvider;
        }

        private static void RegisterServices(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<MsalAuthenticationProvider>().As<IAuthenticationProvider>().SingleInstance();
            containerBuilder.RegisterType<TasksRunner>().As<ITasksRunner>();
            containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Except<DriveFileTasksComposite>()
                .AssignableTo<IDriveFileTask>()
                .Named<IDriveFileTask>("DriveFileTask");
            containerBuilder.Register(context =>
                    new DriveFileTasksComposite(
                        context.ResolveNamed<IEnumerable<IDriveFileTask>>("DriveFileTask"),
                        context.Resolve<ILogger<DriveFileTasksComposite>>()))
                .As<IDriveFileTask>();
        }

        /// <summary>
        /// The Microsoft.Extensions.DependencyInjection.ServiceCollection
        /// has extension methods provided by other .NET Core libraries to
        /// register services with DI.
        /// </summary>
        /// <returns>ServiceCollection of registered .NET Core services</returns>
        private static IServiceCollection RegisterNetCoreLibraries(IConfiguration configuration)
        {
            return new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    // configure Logging with NLog
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                    loggingBuilder.AddNLog(configuration);
                })
                .AddMemoryCache(options => { options.SizeLimit = 500; });
        }
    }
}

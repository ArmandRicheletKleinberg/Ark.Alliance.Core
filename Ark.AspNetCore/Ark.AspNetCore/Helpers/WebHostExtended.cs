using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable LocalizableElement

namespace Ark.AspNetCore
{
    /// <summary>
    /// This class extend the WebHost class to allow it to run either as a console app (IIS, HTTP.Sys, VS20xx) or as a Windows service.
    /// </summary>
    public static class WebHostExtended
    {
        /// <summary>
        /// Creates and builds the WebHostBuilder and run the hosts either in console or in a Windows service.
        /// By default the application will be launched as a Windows service unless a debugger is attached (VS20xx) or if the application is launched with the --console parameter.
        /// Setups the environment/module to run the application into by reading the appenvironment.json file if any.
        /// </summary>
        /// <param name="builderAppendAction">The action to execute on the created builder to append some services before building it.</param>
        /// <param name="args">The startup parameters of the application.</param>
        public static void RunWebHostInConsoleOrAsWindowsService(string[] args, Action<IHostBuilder> builderAppendAction)
        {
            var builder = new HostBuilder();

            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule?.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);

                builder.ConfigureServices((hostingContext, configureServices) => { hostingContext.HostingEnvironment.ContentRootPath = pathToContentRoot; });
            }

            if (!File.Exists("appenvironment.json"))
                throw new Exception("A appenvironment.json must exists on the project root folder with the configuration set with the release management token app.");

            var json = File.ReadAllText("appenvironment.json");
            var config = JsonConvert.DeserializeObject<JObject>(json);

            var environment = config.GetValue("Environment")?.Value<string>().ToEnum(EnvironmentEnum.Debug) ?? EnvironmentEnum.Debug;
            var module = config.GetValue("Module")?.Value<string>();
            EnvironmentHelper.Initialize(environment, module);

            var urls = config.GetValue("Urls")?.Value<string>();

            Console.WriteLine($"Environment : {environment}");
            Console.WriteLine($"Module : {module}");
            Console.WriteLine($"Urls : {urls}");

            builderAppendAction(builder);

            if (isService)
                builder.UseWindowsService().Build().Run();
            else
                builder.Build().Run();
        }
    }
}
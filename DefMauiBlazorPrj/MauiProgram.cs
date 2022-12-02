using DefMauiBlazorPrj.Data;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;
using System.IO;
using Serilog.Events;
using Microsoft.Maui.Controls.Shapes;
using System.Security.AccessControl;

namespace DefMauiBlazorPrj
{
    public static class MauiProgram
    {
        private static void setLoggingValues(MauiAppBuilder builder)
        {
            LogEventLevel minLevelToLog = LogEventLevel.Information;
            string logMinLevel = builder.Configuration["Serilog:MinimumLevel"];
            if (!String.IsNullOrEmpty(logMinLevel))
            {
                logMinLevel = logMinLevel.ToLower();
                switch (logMinLevel)
                {
                    case "information":
                    {
                        minLevelToLog = LogEventLevel.Information;
                        break;
                    }

                    case "verbose":
                    {
                        minLevelToLog = LogEventLevel.Verbose;
                        break;
                    }

                    case "debug":
                    {
                        minLevelToLog = LogEventLevel.Debug;
                        break;
                    }

                    case "warning":
                    {
                        minLevelToLog = LogEventLevel.Warning;
                        break;
                    }

                    case "error":
                    {
                        minLevelToLog = LogEventLevel.Error;
                        break;
                    }

                    case "fatal":
                    {
                        minLevelToLog = LogEventLevel.Fatal;
                        break;
                    }
                }
            }

            string outputTemplateStr = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{SourceContext}] [{EventId}] {Message}{NewLine}{Exception}";
            string pathStr = FileSystem.Current.AppDataDirectory;
            string appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string logFileName = System.IO.Path.Combine(pathStr, appName, "logs", "DefMauiBlazorPrj-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.File(logFileName, rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: minLevelToLog, 
                    outputTemplate: outputTemplateStr)
                .CreateLogger();
        }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
		    builder.Services.AddBlazorWebViewDeveloperTools();

#endif
            // ToDo - unembed appsettings.json
            /* As of 11/25/2022 .NET MAUI Blazor apps can't access appsettings.json.  This looks like something that 
             * was in the previews but then removed.  I am leaving this in here until I find another way to add
             * logging in the hope that I'll do an update and this will be something that works again.
             * https://stackoverflow.com/questions/70280264/maui-what-build-action-for-appsettings-json-and-how-to-access-the-file-on-andro
            builder.Logging.AddConfiguration(
                 builder.Configuration.GetSection("Logging"));
            */

            // This code sets up to use an appsettings.json file.  Maui doesn't support this by default

            // appsettings.json as an embedded resources
            string strAppConfigStreamName = "DefMauiBlazorPrj.appsettings.json";
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MauiProgram)).Assembly;
            var stream = assembly.GetManifestResourceStream(strAppConfigStreamName);
            builder.Configuration.AddJsonStream(stream);

            // Get the logging configurations from appsettings.json
            setLoggingValues(builder);
            builder.Services.AddLogging(options => 
            {
                options.AddSerilog(dispose: true);
            });

            builder.Services.AddSingleton<WeatherForecastService>();

            return builder.Build();
        }
    }
}
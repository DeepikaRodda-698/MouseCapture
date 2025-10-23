using System.Configuration;
using System.Data;
using System.Windows;
using Serilog;
using Serilog.Sinks.File;

namespace MouseCapture
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs\\MouseCapture.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Application started.");
        }
    }

}

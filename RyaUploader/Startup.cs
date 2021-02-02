using System.IO;
using RyaUploader.Extensions;
using RyaUploader.Core.Facade;
using RyaUploader.Core.Services;
using RyaUploader.Core.Services.Converters;
using RyaUploader.Core.Services.FileServices;
using RyaUploader.Core.Services.SteamServices;
using Stylet;
using StyletIoC;
using RyaUploader.ViewModels;
using Serilog;
using System.Reflection;

namespace RyaUploader
{
    public class Startup : Bootstrapper<ShellViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            Log.Information("Configuring the IoC container.");

            // Add all Transient services to the IoC
            builder.Bind<IFilePaths>().To<FilePaths>();
            builder.Bind<IFileReader>().To<FileReader>();
            builder.Bind<IFileWriter>().To<FileWriter>();

            builder.Bind<IShareCodeConverter>().To<ShareCodeConverter>();
            builder.Bind<IProtobufConverter>().To<ProtobufConverter>();

            builder.Bind<IBoilerProcess>().To<BoilerProcess>();
            builder.Bind<ISteamApi>().To<SteamApi>();

            // Add all Singleton services to the IoC
            builder.Bind<IUploader>().To<Uploader>().InSingletonScope();

            // Add all other Singleton classes to the IoC
            builder.Bind<BoilerClientViewModel>().ToSelf().InSingletonScope();
            builder.Bind<TrayIconViewModel>().ToSelf().InSingletonScope();

            // Add static httpclient
            builder.Bind<IHttpClient>().ToInstance(new HttpClientFacade());

            Log.Information("Finished configuring the IoC container.");
        }

        protected override void OnStart()
        {
            base.OnStart();

            SetupLogger();

            var executingFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var tempFolder = Path.Combine(Path.GetTempPath(), "RyaUploader");
            Directory.CreateDirectory(tempFolder);

            Copy3rdPartyFileTo(tempFolder, executingFolder, "boiler.exe");
            Copy3rdPartyFileTo(tempFolder, executingFolder, "steam_api.dll");
            Copy3rdPartyFileTo(tempFolder, executingFolder, "steam_appid.txt");
        }

        private void Copy3rdPartyFileTo(string targetPath, string executingFolder, string fileName)
        {
            Log.Information($"Saving {fileName} to the temp folder.");
            var path = Path.Combine(targetPath, fileName);
            var fileToCopy = Path.Combine(executingFolder, "3rdParty", fileName);
            File.Copy(fileToCopy, path, true);
            Log.Information($"Finished saving {fileName} to the temp folder.");
        }

        private void SetupLogger()
        {
            var appDataFolder = FilePaths.GetAppDataPath();
            var logFile = Path.Combine(appDataFolder, "RyaUploader-.log");

            // Seting up serilog to save to a log file.
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        protected override void DisplayRootView(object rootViewModel)
        {
            base.DisplayRootView(rootViewModel);

            GetActiveWindow().EnableBlur();
        }
    }
}

using System.IO;
using System.Net.Http;
using RyaUploader.Extensions;
using RyaUploader.Facade;
using RyaUploader.Models;
using RyaUploader.Services;
using RyaUploader.Services.Converters;
using RyaUploader.Services.FileServices;
using RyaUploader.Services.SteamServices;
using Stylet;
using StyletIoC;
using RyaUploader.ViewModels;
using Serilog;
using System.Reflection;

namespace RyaUploader
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
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
            builder.Bind<BoilerClient>().ToSelf().InSingletonScope();
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
            
            Copy3rdPartyFileTo(tempFolder, executingFolder, "Boiler.exe");
            Copy3rdPartyFileTo(tempFolder, executingFolder, "libEGL.dll");
            Copy3rdPartyFileTo(tempFolder, executingFolder, "libGLESv2.dll");
            Copy3rdPartyFileTo(tempFolder, executingFolder, "msvcp120.dll");
            Copy3rdPartyFileTo(tempFolder, executingFolder, "msvcr120.dll");
            Copy3rdPartyFileTo(tempFolder, executingFolder, "protobuf-net.dll");
            Copy3rdPartyFileTo(tempFolder, executingFolder, "steam_api.dll");
            Copy3rdPartyFileTo(tempFolder, executingFolder, "steam_appid.txt");
        }

        private void Copy3rdPartyFileTo(string targetPath, string executingFolder, string fileName)
        {
            Log.Information($"Saving {fileName} to the temp folder.");
            var path = Path.Combine(targetPath, "boiler.exe");
            var fileToCopy = Path.Combine(executingFolder, "3rdParty", fileName);
            var fileBytes = File.ReadAllBytes(fileToCopy);
            File.WriteAllBytes(path, fileBytes);
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

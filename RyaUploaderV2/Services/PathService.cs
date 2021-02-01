using System;
using System.IO;

namespace RyaUploaderV2.Services
{
    public interface IPathService
    {
        string BoilerPath { get; }

        string GetMatchesPath();
    }

    public class PathService : IPathService
    {
        public string BoilerPath => Path.Combine(Path.GetTempPath(), "RyaUploader", "boiler.exe");

        /// <summary>
        /// Gets the save folder located in Appdata. If it does not exist it will also be created
        /// </summary>
        /// <returns>Path to the cache</returns>
        private string GetAppDataPath()
        {
            var windowsAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDataFolder = Path.Combine(windowsAppDataFolder, "Ryada");
            appDataFolder = Path.Combine(appDataFolder, "RyaUploader");
            
            Directory.CreateDirectory(appDataFolder);

            return appDataFolder;
        }

        public string GetMatchesPath()
        {
            return Path.Combine(GetAppDataPath(), "matches.dat");
        }
    }
}

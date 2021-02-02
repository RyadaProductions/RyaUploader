using System;
using System.IO;

namespace RyaUploader.Core.Services.FileServices
{
    public class FilePaths : IFilePaths
    {
        public string BoilerPath => Path.Combine(Path.GetTempPath(), "RyaUploader", "boiler.exe");

        public string MatchListPath => Path.Combine(GetAppDataPath(), "matches.dat");

        public string JsonMatchesPath => Path.Combine(GetAppDataPath(), "matches.json");

        /// <summary>
        /// Gets the save folder located in Appdata. If it does not exist it will also be created
        /// </summary>
        /// <returns>Path to the cache</returns>
        public static string GetAppDataPath()
        {
            var windowsAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDataFolder = Path.Combine(windowsAppDataFolder, "Ryada", "RyaUploader");
            
            Directory.CreateDirectory(appDataFolder);

            return appDataFolder;
        }
    }
}

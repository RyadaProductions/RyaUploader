using System;
using System.IO;
using System.Security.Cryptography;

namespace RyaUploaderV2.Services
{
    public class PathService
    {
        /// <summary>
        /// Gets the hash in Sha1 format from a specific file. Used for validation to make sure people do not replace it with a malicious version
        /// </summary>
        /// <param name="filePath">The path to the file that you want to get the hash from</param>
        /// <returns>sha1 hash</returns>
        public string GetSha1Hash(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                var sha = new SHA1Managed();
                var hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }

        /// <summary>
        /// Gets the save folder located in Appdata.
        /// </summary>
        /// <returns>Path to the cache</returns>
        public string GetAppDataPath()
        {
            var windowsAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDataFolder = Path.Combine(windowsAppDataFolder, "Ryada");
            appDataFolder = Path.Combine(appDataFolder, "RyaUploader");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            return appDataFolder + Path.DirectorySeparatorChar + "matches.dat";
        }
    }
}

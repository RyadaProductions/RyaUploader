using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RyaUploaderV2.Models;
using Serilog;

namespace RyaUploaderV2.Services.FileServices
{
    public interface IFileWriter
    {
        /// <summary>
        /// Save a list of matchmodels to the specified file.
        /// </summary>
        /// <param name="file">file to save the list in</param>
        /// <param name="matches">the list of matches you want to save</param>
        void SaveMatchesToJson(string file, IEnumerable<MatchModel> matches);
    }

    public class FileWriter : IFileWriter
    {
        public void SaveMatchesToJson(string file, IEnumerable<MatchModel> matches)
        {
            Log.Information($"Writing matches to file: {file}.");
            using (var stream = File.CreateText(file))
            {
                var serializer = new JsonSerializer {Formatting = Formatting.Indented};
                serializer.Serialize(stream, matches);
            }
        }
    }
}

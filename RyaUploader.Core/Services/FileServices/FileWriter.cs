using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RyaUploader.Core.Models;
using Serilog;

namespace RyaUploader.Core.Services.FileServices
{
    public class FileWriter : IFileWriter
    {
        public void SaveMatchesToJson(string file, IEnumerable<Match> matches)
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

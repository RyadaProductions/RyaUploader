using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RyaUploader.Core.Models;
using Serilog;
using MatchList = RyaUploader.Core.ProtoBufs.CMsgGCCStrike15_v2_MatchList;

namespace RyaUploader.Core.Services.FileServices
{
    public class FileReader : IFileReader
    {
        public MatchList ReadProtobuf(string file)
        {
            Log.Information("Reading protobuf file.");
            using (var stream = File.OpenRead(file))
            {
                return MatchList.Parser.ParseFrom(stream);
            }
        }

        public List<Match> ReadMatchesFromJson(string file)
        {
            if (!File.Exists(file)) return new List<Match>();

            Log.Information("Reading previously saved matches");
            var content = File.ReadAllText(file);

            return string.IsNullOrEmpty(content) ? new List<Match>() : JsonConvert.DeserializeObject<List<Match>>(content);
        }
    }
}

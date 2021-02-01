using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RyaUploaderV2.Models;
using Serilog;
using MatchList = RyaUploaderV2.ProtoBufs.CMsgGCCStrike15_v2_MatchList;

namespace RyaUploaderV2.Services.FileServices
{
    public interface IFileReader
    {
        /// <summary>
        /// Read the content from an encrypted protobuf file, that contains matchdata from csgo.
        /// </summary>
        /// <param name="file">path to the file you want to read</param>
        /// <returns>MatchList of the last 8 matches</returns>
        MatchList ReadProtobuf(string file);
        
        /// <summary>
        /// Read and Deserialize the json file specified into a List of MatchModels.
        /// </summary>
        /// <param name="file">The Json file to read</param>
        /// <returns>List of MatchModels</returns>
        List<Match> ReadMatchesFromJson(string file);
    }

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

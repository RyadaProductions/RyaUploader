using System.Collections.Generic;
using RyaUploader.Core.Models;
using MatchList = RyaUploader.Core.ProtoBufs.CMsgGCCStrike15_v2_MatchList;

namespace RyaUploader.Core.Services.FileServices
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
}

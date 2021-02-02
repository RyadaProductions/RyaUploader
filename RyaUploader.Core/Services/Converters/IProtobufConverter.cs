using System.Collections.Generic;
using System.Threading.Tasks;
using RyaUploader.Core.Models;
using MatchInfo = RyaUploader.Core.ProtoBufs.CDataGCCStrike15_v2_MatchInfo;

namespace RyaUploader.Core.Services.Converters
{
    public interface IProtobufConverter
    {
        /// <summary>
        /// Convert a deserialised protobuf matchlist into mutliple MatchModels
        /// </summary>
        /// <param name="protobuf">Deserialised protobuf message received from Valve</param>
        /// <returns>list of MatchModels</returns>
        Task<List<Match>> ProtobufToMatches(List<MatchInfo> protobuf);
    }
}

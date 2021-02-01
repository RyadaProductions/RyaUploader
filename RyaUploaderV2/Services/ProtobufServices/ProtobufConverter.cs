using System.Collections.Generic;
using System.Linq;
using RyaUploaderV2.Models;
using Protobuf = RyaUploaderV2.ProtoBufs.CMsgGCCStrike15_v2_MatchList;

namespace RyaUploaderV2.Services.ProtobufServices
{
    public interface IProtobufConverter
    {
        /// <summary>
        /// Convert a deserialised protobuf matchlist into mutliple MatchModels
        /// </summary>
        /// <param name="matchList">Deserialised protobuf message received from Valve</param>
        /// <returns>list of MatchModels</returns>
        IEnumerable<MatchModel> ProtobufToMatchList(Protobuf matchList);
    }

    public class ProtobufConverter : IProtobufConverter
    {
        public IEnumerable<MatchModel> ProtobufToMatchList(Protobuf matchList)
        {
            var matchModels = new List<MatchModel>();

            foreach (var match in matchList.Matches)
            {
                var lastRound = match.Roundstatsall.Last();

                matchModels.Add(new MatchModel()
                {
                    MatchId = match.Matchid,
                    // ReservationId still needs to check the legacy version since once in a while it will encounter a match that contains the legacy format
                    ReservationId = match.RoundstatsLegacy?.Reservationid ?? lastRound.Reservationid,
                    TvPort = match.Watchablematchinfo.TvPort,

                    Players = lastRound.Reservation.AccountIds.ToList(),
                    Kills = lastRound.Kills.ToList(),
                    Deaths = lastRound.Deaths.ToList(),
                    Assists = lastRound.Assists.ToList(),

                    Victory = lastRound.MatchResult == 2,
                    Scores = lastRound.TeamScores.ToList()
                });
            }

            return matchModels;
        }
    }
}

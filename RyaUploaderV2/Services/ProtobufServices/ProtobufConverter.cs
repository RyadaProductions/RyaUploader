using System;
using System.Collections.Generic;
using System.Linq;
using RyaUploaderV2.Models;
using Serilog;
using Stylet;
using Protobuf = RyaUploaderV2.ProtoBufs.CMsgGCCStrike15_v2_MatchList;

namespace RyaUploaderV2.Services.ProtobufServices
{
    public interface IProtobufConverter
    {
        /// <summary>
        /// Convert a deserialised protobuf matchlist into mutliple MatchModels
        /// </summary>
        /// <param name="protobuf">Deserialised protobuf message received from Valve</param>
        /// <returns>list of MatchModels</returns>
        IEnumerable<MatchModel> ProtobufToMatches(Protobuf protobuf);
    }

    public class ProtobufConverter : IProtobufConverter
    {
        public IEnumerable<MatchModel> ProtobufToMatches(Protobuf protobuf)
        {
            Log.Information($"Converting {protobuf.Matches.Count} protobuf matches into MatchModels.");
            var matchModels = new List<MatchModel>();

            foreach (var match in protobuf.Matches)
            {
                var lastRound = match.Roundstatsall.Last();

                var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

                var team1 = new BindableCollection<PlayerStatsModel>();
                var team2 = new BindableCollection<PlayerStatsModel>();
                for (var i = 0; i < 5; i++)
                {
                    team1.Add(new PlayerStatsModel
                    {
                        Id = lastRound.Reservation.AccountIds[i],
                        Kills = lastRound.Kills[i],
                        Deaths = lastRound.Deaths[i],
                        Assists = lastRound.Assists[i]
                    });
                    team2.Add(new PlayerStatsModel
                    {
                        Id = lastRound.Reservation.AccountIds[i + 5],
                        Kills = lastRound.Kills[i + 5],
                        Deaths = lastRound.Deaths[i + 5],
                        Assists = lastRound.Assists[i + 5]
                    });
                }

                matchModels.Add(new MatchModel
                {
                    Date = baseDate.AddSeconds(match.Matchtime).ToLocalTime(),

                    MatchId = match.Matchid,
                    // ReservationId still needs to check the legacy version since once in a while it will encounter a match that contains the legacy format
                    ReservationId = match.RoundstatsLegacy?.Reservationid ?? lastRound.Reservationid,
                    TvPort = match.Watchablematchinfo.TvPort,

                    Team1ScoreBoard = team1,
                    Team2ScoreBoard = team2,

                    Victory = lastRound.MatchResult == 2,
                    Scores = lastRound.TeamScores.ToList()
                });
            }

            return matchModels;
        }
    }
}

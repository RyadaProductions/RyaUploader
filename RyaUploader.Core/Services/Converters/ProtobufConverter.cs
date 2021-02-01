using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RyaUploader.Core.Models;
using RyaUploader.Core.Services.SteamServices;
using Serilog;
using MatchInfo = RyaUploader.Core.ProtoBufs.CDataGCCStrike15_v2_MatchInfo;
using RoundStats = RyaUploader.Core.ProtoBufs.CMsgGCCStrike15_v2_MatchmakingServerRoundStats;

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

    public class ProtobufConverter : IProtobufConverter
    {
        private readonly ISteamApi _steamApi;

        private const long STEAM64_BASE = 76561197960265728;

        public ProtobufConverter(ISteamApi steamApi)
        {
            _steamApi = steamApi;
        }

        public async Task<List<Match>> ProtobufToMatches(List<MatchInfo> protobuf)
        {
            Log.Information($"Converting {protobuf.Count} protobuf matches into MatchModels.");
            var matchModels = new List<Match>();

            foreach (var match in protobuf)
            {
                var lastRound = match.Roundstatsall.Last();

                var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

                var playerIds = lastRound.Reservation.AccountIds.Select(x => x + STEAM64_BASE).ToArray();
                var playerProfiles = await _steamApi.GetPlayerProfilesAsync(playerIds);

                var team1 = CreateTeamLeaderboard(playerProfiles, lastRound, 0);
                var team2 = CreateTeamLeaderboard(playerProfiles, lastRound, 5);

                matchModels.Add(new Match
                {
                    Date = baseDate.AddSeconds(match.Matchtime).ToLocalTime(),

                    MatchId = match.Matchid,
                    // ReservationId still needs to check the legacy version since once in a while it will encounter a match that contains the legacy format
                    ReservationId = match.RoundstatsLegacy?.Reservationid ?? lastRound.Reservationid,
                    TvPort = match.Watchablematchinfo.TvPort,

                    Map = match.Watchablematchinfo.GameMap,

                    Team1 = team1,
                    Team2 = team2,

                    Victory = lastRound.MatchResult == 2,
                    Scores = lastRound.TeamScores.ToList()
                });
            }

            return matchModels;
        }

        /// <summary>
        /// Create a List of PlayerScores that contain the user their steamprofile and stats after the final round. This is used as the final score in the game
        /// </summary>
        /// <param name="playerProfiles">Dictionary of playerprofiles where the key is their steam64 id</param>
        /// <param name="finalRound">FinalRoundStats from the protobuf message</param>
        /// <param name="offset">Start at index 0 for team 1 and index 5 for team2</param>
        /// <returns>List of playerscores that can be used as the leaderboard of 1 team</returns>
        private List<PlayerScore> CreateTeamLeaderboard(IReadOnlyDictionary<long, PlayerProfile> playerProfiles, RoundStats finalRound, int offset)
        {
            var teamScoreBoard = new List<PlayerScore>();

            for (var i = 0 + offset; i < 5 + offset; i++)
            {
                teamScoreBoard.Add(new PlayerScore
                {
                    Player = playerProfiles[finalRound.Reservation.AccountIds[i] + STEAM64_BASE],
                    Kills = finalRound.Kills[i],
                    Deaths = finalRound.Deaths[i],
                    Assists = finalRound.Assists[i]
                });
            }
            
            return teamScoreBoard;
        }
    }
}

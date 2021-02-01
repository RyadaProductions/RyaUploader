using System;
using System.Collections.Generic;
using Stylet;

namespace RyaUploaderV2.Models
{
    public class MatchModel
    {
        /// <summary>
        /// Date and time that the match was played
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Match ID used by valve to identify the match.
        /// </summary>
        public ulong MatchId { get; set; }

        /// <summary>
        /// Reservation ID used by valve to identify the reservation with the match when the match is running.
        /// </summary>
        public ulong ReservationId { get; set; }
        
        /// <summary>
        /// Tv port, the port associated with gotv. This is used for spectating the match.
        /// </summary>
        public uint TvPort { get; set; }
        
        /// <summary>
        /// The scoreboard of team1.
        /// </summary>
        public BindableCollection<PlayerStatsModel> Team1ScoreBoard { get; set; }

        /// <summary>
        /// The scoreboard of team2
        /// </summary>
        public BindableCollection<PlayerStatsModel> Team2ScoreBoard { get; set; }

        /// <summary>
        /// Represents if we won or not, false could mean a draw or loss
        /// </summary>
        public bool Victory { get; set; }

        /// <summary>
        /// List of the scores both teams had, in case of a draw both scores will be 15
        /// </summary>
        public List<int> Scores { get; set; }
    }

    public class PlayerStatsModel
    {
        /// <summary>
        /// Id of a player that was allowed to play in the match
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Kills the player had
        /// </summary>
        public int Kills { get; set; }

        /// <summary>
        /// Deaths the player had
        /// </summary>
        public int Deaths { get; set; }

        /// <summary>
        /// Assists the player had
        /// </summary>
        public int Assists { get; set; }

    }
}

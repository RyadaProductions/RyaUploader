using System;
using System.Collections.Generic;

namespace RyaUploader.Core.Models
{
    public class Match
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
        /// The map that was played usually prefixed with the gamemode `de_` for defusal
        /// </summary>
        public string Map { get; set; }
        
        /// <summary>
        /// The scoreboard of team1.
        /// </summary>
        public List<PlayerScore> Team1 { get; set; }

        /// <summary>
        /// The scoreboard of team2
        /// </summary>
        public List<PlayerScore> Team2 { get; set; }

        /// <summary>
        /// Represents if we won or not, false could mean a draw or loss
        /// </summary>
        public bool Victory { get; set; }

        /// <summary>
        /// List of the scores both teams had, in case of a draw both scores will be 15
        /// </summary>
        public List<int> Scores { get; set; }
    }
}

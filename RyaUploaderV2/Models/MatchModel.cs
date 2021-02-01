using System.Collections.Generic;

namespace RyaUploaderV2.Models
{
    public class MatchModel
    {
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
        /// List of players that were allowed to play in the match, these are their Ids
        /// </summary>
        public List<uint> Players { get; set; }

        /// <summary>
        /// List of kills each player had
        /// </summary>
        public List<int> Kills { get; set; }

        /// <summary>
        /// List of Deaths each player had
        /// </summary>
        public List<int> Deaths { get; set; }

        /// <summary>
        /// List of assists each player had
        /// </summary>
        public List<int> Assists { get; set; }

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

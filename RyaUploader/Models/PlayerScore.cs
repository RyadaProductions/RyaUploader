using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RyaUploaderV2.Models
{
    public class PlayerScore
    {
        /// <summary>
        /// Steamprofile of a player that was allowed to play in the match
        /// </summary>
        public PlayerProfile Player { get; set; }

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

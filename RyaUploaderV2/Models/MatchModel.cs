using System.Collections.Generic;
using System.Linq;
using MatchList = RyaUploaderV2.ProtoBufs.CMsgGCCStrike15_v2_MatchList;

namespace RyaUploaderV2.Models
{
    public class MatchModel
    {
        public ulong MatchId { get; set; }
        public ulong ReservationId { get; set; }
        public uint TvPort { get; set; }

        public List<uint> Players { get; set; }
        public List<int> Kills { get; set; }
        public List<int> Deaths { get; set; }
        public List<int> Assists { get; set; }

        public bool Victory { get; set; }
        public List<int> Scores { get; set; }
    }
}

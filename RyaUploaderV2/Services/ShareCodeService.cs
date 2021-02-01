using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ProtoBuf;
using SteamKit2.GC.CSGO.Internal;

namespace RyaUploaderV2.Services
{
    public class ShareCodeService
    {
        private readonly char[] _dictionary = {
            'A','B','D','E','F','G','H','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','h','i','j','k','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            '2','3','4','5','6','7','8','9'
        };

        private readonly string _matchesFile;

        public ShareCodeService(PathService pathService)
        {
            _matchesFile = pathService.GetAppDataPath();
        }

        /// <summary>
        /// Get the sharecodes for the last 8 matches
        /// </summary>
        /// <returns>List of the last 8 shortcodes</returns>
        public List<string> GetNewestShareCodes()
        {
            var demoUrlList = new List<string>();

            using (var file = File.OpenRead(_matchesFile))
            {
                var matchList = Serializer.Deserialize<CMsgGCCStrike15_v2_MatchList>(file);

                Parallel.ForEach(matchList.matches, (matchInfo, state) =>
                {
                    var matchId = matchInfo.matchid;
                    var tvPort = matchInfo.watchablematchinfo.tv_port;

                    // Gets the legacy reservationId if it exists otherwise it will take the last reservationId of the match.
                    var reservationId = matchInfo.roundstats_legacy?.reservationid ?? matchInfo.roundstatsall.Last().reservationid;

                    if (TryEncode(matchId, reservationId, tvPort, out var shareCode))
                        demoUrlList.Add($@"steam://rungame/730/XXXXXXXXXXXXXXXXX/+csgo_download_match%{shareCode}");
                });
            }

            return demoUrlList;
        }

        /// <summary>
        /// Tries to Encode a share code with the required fields coming from a CDataGCCStrike15_v2_MatchInfo message.
        /// </summary>
        /// <param name="matchId">The match id</param>
        /// <param name="reservationId">the reservation id of the match</param>
        /// <param name="tvPort">the port that goTV was run under</param>
        /// <returns>Csgo share link</returns>
        public bool TryEncode(ulong matchId, ulong reservationId, uint tvPort, out string shareLink)
        {
            try
            {
                var matchIdBytes = BitConverter.GetBytes(matchId);
                var reservationBytes = BitConverter.GetBytes(reservationId);
                // only the low bits from the TV port are used
                var tvPort16 = (ushort)(tvPort & ((1 << 16) - 1));
                var tvBytes = BitConverter.GetBytes(tvPort16);

                var bytes = new byte[matchIdBytes.Length + reservationBytes.Length + tvBytes.Length + 1];

                Buffer.BlockCopy(matchIdBytes, 0, bytes, 1, matchIdBytes.Length);
                Buffer.BlockCopy(reservationBytes, 0, bytes, 1 + matchIdBytes.Length, reservationBytes.Length);
                Buffer.BlockCopy(tvBytes, 0, bytes, 1 + matchIdBytes.Length + reservationBytes.Length, tvBytes.Length);

                var big = new BigInteger(bytes.Reverse().ToArray());
                
                var shareCode = "";

                for (var i = 0; i < 25; i++)
                {
                    BigInteger.DivRem(big, _dictionary.Length, out var remainder);
                    shareCode += _dictionary[(int)remainder];
                    big = BigInteger.Divide(big, _dictionary.Length);
                }
                shareLink =
                    $"CSGO-{shareCode.Substring(0, 5)}-{shareCode.Substring(5, 5)}-{shareCode.Substring(10, 5)}-{shareCode.Substring(15, 5)}-{shareCode.Substring(20, 5)}";
                return true;
            }
            catch
            {
                shareLink = "";
                return false;
            }
        }
    }
}

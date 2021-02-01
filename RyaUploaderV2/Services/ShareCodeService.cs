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
        private const string _dictionary = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefhijkmnopqrstuvwxyz23456789";

        private readonly string _matchesFile;

        public ShareCodeService(PathService pathService)
        {
            _matchesFile = pathService.GetAppDataPath();
        }

        /// <summary>
        /// Get the sharecode for a match. This is needed to upload it to csgostats
        /// </summary>
        /// <returns>sharecode</returns>
        public List<string> GetShareCodes()
        {
            var demoUrlList = new List<string>();

            using (var file = File.OpenRead(_matchesFile))
            {
                try
                {
                    var matchList = Serializer.Deserialize<CMsgGCCStrike15_v2_MatchList>(file);
                    Parallel.ForEach(matchList.matches, (matchInfo, state) =>
                    {
                        var matchId = matchInfo.matchid;
                        var tvPort = matchInfo.watchablematchinfo.tv_port;
                        ulong reservationId;

                        // old definition
                        if (matchInfo.roundstats_legacy != null)
                        {
                            reservationId = matchInfo.roundstats_legacy.reservationid;
                        }
                        // new definition
                        else
                        {
                            var roundStatsList = matchInfo.roundstatsall;
                            reservationId = roundStatsList.Last().reservationid;
                        }

                        //TODO: Find beter way to solve a exception inside of Encode, with a fallback url
                        var shareCode = Encode(matchId, reservationId, tvPort);
                        if (string.IsNullOrEmpty(shareCode)) return;

                        demoUrlList.Add($@"steam://rungame/730/XXXXXXXXXXXXXXXXX/+csgo_download_match%{shareCode}");
                    });
                    return demoUrlList;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        /// <summary>
        /// Encode a share code from required fields coming from a CDataGCCStrike15_v2_MatchInfo message.
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="reservationId"></param>
        /// <param name="tvPort"></param>
        /// <returns></returns>
        public string Encode(ulong matchId, ulong reservationId, uint tvPort)
        {
            var matchIdBytes = BitConverter.GetBytes(matchId);
            var reservationBytes = BitConverter.GetBytes(reservationId);
            // only the UInt16 low bits from the TV port are used
            var tvPort16 = (ushort)(tvPort & ((1 << 16) - 1));
            var tvBytes = BitConverter.GetBytes(tvPort16);

            var bytes = new byte[matchIdBytes.Length + reservationBytes.Length + tvBytes.Length + 1];

            Buffer.BlockCopy(matchIdBytes, 0, bytes, 1, matchIdBytes.Length);
            Buffer.BlockCopy(reservationBytes, 0, bytes, 1 + matchIdBytes.Length, reservationBytes.Length);
            Buffer.BlockCopy(tvBytes, 0, bytes, 1 + matchIdBytes.Length + reservationBytes.Length, tvBytes.Length);

            var big = new BigInteger(bytes.Reverse().ToArray());

            var charArray = _dictionary.ToCharArray();
            var shareCode = "";

            for (var i = 0; i < 25; i++)
            {
                BigInteger rem;
                BigInteger.DivRem(big, charArray.Length, out rem);
                shareCode += charArray[(int)rem];
                big = BigInteger.Divide(big, charArray.Length);
            }

            return $"CSGO-{shareCode.Substring(0, 5)}-{shareCode.Substring(5, 5)}-{shareCode.Substring(10, 5)}-{shareCode.Substring(15, 5)}-{shareCode.Substring(20, 5)}";
        }
    }
}

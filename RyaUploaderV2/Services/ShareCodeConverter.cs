using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RyaUploaderV2.Models;

namespace RyaUploaderV2.Services
{
    public interface IShareCodeConverter
    {
        /// <summary>
        /// Get the last 8 sharecodes from the list of matches
        /// </summary>
        /// <param name="matchList">List of matches you want to get the sharecodes from</param>
        /// <returns>List of the last 8 sharecodes</returns>
        IEnumerable<string> ConvertMatchListToShareCodes(IEnumerable<MatchModel> matchList);
    }

    public class ShareCodeConverter : IShareCodeConverter
    {
        private const string _dictionary = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefhijkmnopqrstuvwxyz23456789";

        public IEnumerable<string> ConvertMatchListToShareCodes(IEnumerable<MatchModel> matchList)
        {
            var demoUrlList = new List<string>();

            foreach (var matchInfo in matchList)
            {
                var matchId = matchInfo.MatchId;
                var tvPort = matchInfo.TvPort;

                // Gets the legacy ReservationId if it exists otherwise it will take the last ReservationId of the match.
                var reservationId = matchInfo.ReservationId;

                if (TryParse(matchId, reservationId, tvPort, out var shareCode))
                    demoUrlList.Add(shareCode);
            }

            return demoUrlList;
        }

        /// <summary>
        /// Tries to Encode a sharecode with the required fields coming from a CDataGCCStrike15_v2_MatchInfo message.
        /// </summary>
        /// <param name="matchId">The match id</param>
        /// <param name="reservationId">the reservation id of the match</param>
        /// <param name="tvPort">the port that goTV was run under</param>
        /// <param name="shareLink">Returns the sharelink that was created by the method</param>
        /// <returns>True when succesfull, False when failed</returns>
        private bool TryParse(ulong matchId, ulong reservationId, uint tvPort, out string shareLink)
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

                var charArray = _dictionary.ToCharArray();
                var shareCode = "";

                for (var i = 0; i < 25; i++)
                {
                    BigInteger.DivRem(big, charArray.Length, out var remainder);
                    shareCode += charArray[(int)remainder];
                    big = BigInteger.Divide(big, charArray.Length);
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

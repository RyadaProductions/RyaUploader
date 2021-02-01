using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RyaUploader.Core.Models;
using Serilog;

namespace RyaUploader.Core.Services.Converters
{
    public interface IShareCodeConverter
    {
        /// <summary>
        /// Get the last 8 sharecodes from the list of matches
        /// </summary>
        /// <param name="matches">List of matches you want to get the sharecodes from</param>
        /// <returns>List of the last 8 sharecodes</returns>
        IEnumerable<string> ConvertMatchListToShareCodes(IEnumerable<Match> matches);
    }

    public class ShareCodeConverter : IShareCodeConverter
    {
        private const string DICTIONARY = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefhijkmnopqrstuvwxyz23456789";

        public IEnumerable<string> ConvertMatchListToShareCodes(IEnumerable<Match> matches)
        {
            Log.Information("Converting a list of MatchModels into a list of ShareCodes.");
            var shareCodes = new List<string>();

            foreach (var matchInfo in matches)
            {
                var matchId = matchInfo.MatchId;
                var tvPort = matchInfo.TvPort;

                // Gets the legacy ReservationId if it exists otherwise it will take the last ReservationId of the match.
                var reservationId = matchInfo.ReservationId;

                if (TryParse(matchId, reservationId, tvPort, out var shareCode))
                    shareCodes.Add(shareCode);
            }

            return shareCodes;
        }

        /// <summary>
        /// Tries to Encode a sharecode with the required fields coming from a CDataGCCStrike15_v2_MatchInfo message.
        /// </summary>
        /// <param name="matchId">The match id</param>
        /// <param name="reservationId">the reservation id of the match</param>
        /// <param name="tvPort">the port that goTV was run under</param>
        /// <param name="shareCode">Returns the sharelink that was created by the method</param>
        /// <returns>True when succesfull, False when failed</returns>
        private bool TryParse(ulong matchId, ulong reservationId, uint tvPort, out string shareCode)
        {
            Log.Information($"Converting a match into a sharecode. matchId: {matchId}, reservationId: {reservationId}, tvPort {tvPort}.");
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

                var charArray = DICTIONARY.ToCharArray();
                var decryptedCode = "";

                for (var i = 0; i < 25; i++)
                {
                    BigInteger.DivRem(big, charArray.Length, out var remainder);
                    decryptedCode += charArray[(int)remainder];
                    big = BigInteger.Divide(big, charArray.Length);
                }
                shareCode =
                    $"CSGO-{decryptedCode.Substring(0, 5)}-{decryptedCode.Substring(5, 5)}-{decryptedCode.Substring(10, 5)}-{decryptedCode.Substring(15, 5)}-{decryptedCode.Substring(20, 5)}";

                Log.Information($"Succesfully managed to convert a match into a sharecode, returning: {shareCode}.");
                return true;
            }
            catch (Exception exception)
            {
                shareCode = "";
                Log.Error(exception, "Could not convert match into sharecode.");
                return false;
            }
        }
    }
}

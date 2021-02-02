using System.Collections.Generic;
using RyaUploader.Core.Models;

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
}

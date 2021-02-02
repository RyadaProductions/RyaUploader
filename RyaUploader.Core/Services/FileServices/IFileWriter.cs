using System.Collections.Generic;
using RyaUploader.Core.Models;

namespace RyaUploader.Core.Services.FileServices
{
    public interface IFileWriter
    {
        /// <summary>
        /// Save a list of matchmodels to the specified file.
        /// </summary>
        /// <param name="file">file to save the list in</param>
        /// <param name="matches">the list of matches you want to save</param>
        void SaveMatchesToJson(string file, IEnumerable<Match> matches);
    }
}

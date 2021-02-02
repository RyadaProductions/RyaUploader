using System.Collections.Generic;
using System.Threading.Tasks;

namespace RyaUploader.Core.Services
{
    public interface IUploader
    {
        /// <summary>
        /// Uploads the last 8 matches to an online endpoint
        /// </summary>
        /// <param name="shareCodes">List of sharecodes you want to upload</param>
        /// <returns>status message</returns>
        Task<bool> UploadShareCodes(IEnumerable<string> shareCodes);
    }
}

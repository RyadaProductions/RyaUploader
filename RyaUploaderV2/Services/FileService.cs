using System;
using System.IO;
using System.Security.Cryptography;
using RyaUploaderV2.ProtoBufs;

namespace RyaUploaderV2.Services
{
    public interface IFileService
    { 
        bool IsBoilerValid { get; }

        CMsgGCCStrike15_v2_MatchList ReadMatches(string file);
    }

    public class FileService : IFileService
    {
        public bool IsBoilerValid => GetSha1Hash(_pathService.BoilerPath).Equals("80F2C8A1F51118FA450AB9E700645508172B01B8");

        private readonly IPathService _pathService;

        public FileService(IPathService pathService)
        {
            _pathService = pathService;
        }
        
        /// <summary>
        /// Gets the hash in Sha1 format from a specific file. Used for validation to make sure people do not replace it with a malicious version
        /// </summary>
        /// <param name="file">The path to the file that you want to get the hash from</param>
        /// <returns>sha1 hash</returns>
        private string GetSha1Hash(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                var sha = new SHA1Managed();
                var hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
        
        /// <summary>
        /// Read the content from a matches.dat file
        /// </summary>
        /// <param name="file">path to the file you want to read</param>
        /// <returns>MatchList of the last 8 matches</returns>
        public CMsgGCCStrike15_v2_MatchList ReadMatches(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                return CMsgGCCStrike15_v2_MatchList.Parser.ParseFrom(stream);
            }
        }
    }
}

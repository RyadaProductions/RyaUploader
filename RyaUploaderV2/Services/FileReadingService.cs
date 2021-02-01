using System.IO;
using MatchList = RyaUploaderV2.ProtoBufs.CMsgGCCStrike15_v2_MatchList;

namespace RyaUploaderV2.Services
{
    public interface IFileReadingService
    {
        MatchList ReadMatchList(string file);
    }

    public class FileReadingReadingService : IFileReadingService
    {
        /// <summary>
        /// Read the content from an encrypted.dat file, that contains matchdata from csgo.
        /// </summary>
        /// <param name="file">path to the file you want to read</param>
        /// <returns>MatchList of the last 8 matches</returns>
        public MatchList ReadMatchList(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                return MatchList.Parser.ParseFrom(stream);
            }
        }
    }
}

namespace RyaUploader.Core.Services.FileServices
{
    public interface IFilePaths
    {
        /// <summary>
        /// string that contains the path to boiler.exe
        /// </summary>
        string BoilerPath { get; }

        /// <summary>
        /// string that contains the path to matches.dat
        /// </summary>
        string MatchListPath { get; }

        /// <summary>
        /// string that contains the path to matches.json
        /// </summary>
        string JsonMatchesPath { get; }
    }
}

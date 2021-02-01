using Newtonsoft.Json;

namespace RyaUploaderV2.Models
{
    public class PlayerProfile
    {
        /// <summary>
        /// SteamId of the person that was allowed to play in the match
        /// </summary>
        public long SteamId { get; set; }

        /// <summary>
        /// Steam profile name of the person when the match was added to the application
        /// </summary>
        public string PersonaName { get; set; }

        /// <summary>
        /// The player avatar of the user when the match was added to the application
        /// </summary>
        public string AvatarFull { get; set; }

        public override string ToString() => PersonaName;
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using RyaUploader.Core.Models;

namespace RyaUploader.Core.Services.SteamServices
{
    public interface ISteamApi
    {
        Task<Dictionary<long, PlayerProfile>> GetPlayerProfilesAsync(long[] ids);
    }
}

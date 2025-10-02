using HeathenEngineering.SteamworksIntegration.API;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class GlobalManager : TMonoSingleton<GlobalManager>
    {
        public bool SteamInitialized = false;
        
        public void SteamInitSuccess()
        {
            SteamInitialized = true;
            var user = User.Client.Id;
            Log.Debug("steam init success name:" + user.Nickname);
            
        }
    }
    
    
}
using Steamworks;
using UnityEngine;

namespace RoundHero
{
    public class DataManager : Singleton<DataManager>
    {
        public Data_Game DataGame;
        public Data_User CurUser;
        
        public DataManager()
        {
            GameEntry.Setting.RemoveAllSettings();
            // var hasSetting = GameEntry.Setting.HasSetting(Constant.Game.GameDataKey);
            //
            // if (hasSetting)
            // {
            //     DataGame = GameEntry.Setting.GetObject<Data_Game>(Constant.Game.GameDataKey);
            // }
            // else
            // {
            //     DataGame = new Data_Game();
            // }
            
            DataGame = new Data_Game();
            
        }
        
        public void  Init(string userID)
        {
            CurUser = DataGame.GetUserData(userID);
            
            
            // var hasVersion = GameEntry.Setting.HasSetting(Constant.Game.VersionKey);
            // if (hasVersion)
            // {
            //     var version = GameEntry.Setting.GetString(Constant.Game.VersionKey);
            //     if (version != Application.version)
            //     {
            //         GameEntry.Setting.SetString(Constant.Game.VersionKey, Application.version);
            //         CurUser.GamePlayData = new Data_GamePlay();
            //         Save();
            //         
            //         GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            //         {
            //             Message = GameEntry.Localization.GetString(Constant.Localization.Message_ResetData),
            //         
            //         });
            //     }
            // }
            // else
            // {
            //     GameEntry.Setting.SetString(Constant.Game.VersionKey, Application.version);
            //     GameEntry.Setting.SetString(Constant.Game.VersionKey, Application.version);
            //     CurUser.GamePlayData = new Data_GamePlay();
            //     Save();
            // }

            GameEntry.Setting.SetString(Constant.Game.VersionKey, Application.version);
            
            CurUser.GamePlayData = new Data_GamePlay();
            
            CurUser.PlayerData = new Data_Player()
            {
                PlayerID = 123//SteamUser.GetSteamID().m_SteamID,
            };
            
            
            
            //BlessManager.Instance.AddBless(EBlessID.EachRoundFightCardAddLinkReceive);
            Save();

        }
        
        public void Save()
        {
            GameEntry.Setting.SetObject(Constant.Game.GameDataKey, DataGame);
            GameEntry.Setting.Save();
        }
    }
}
using Steamworks;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class DataManager : Singleton<DataManager>
    {
        public Data_Game DataGame;

        public DataManager()
        {
            var appVersions = Application.version.Split("_");
            var isForceDataReset = appVersions[appVersions.Length - 1] == "1";

            if (isForceDataReset)
            {
                DataGame = new Data_Game();
            }
            else
            {
                Log.Info("DataManager");
                var hasSetting = GameEntry.Setting.HasSetting(Constant.Game.GameDataKey);
              
                if (hasSetting)
                {
                    Log.Info("hasSetting");
                    DataGame = GameEntry.Setting.GetObject<Data_Game>(Constant.Game.GameDataKey);
                }
                else
                {
                    Log.Info("!!hasSetting");
                    DataGame = new Data_Game();
                }
            }
            

        }
        
        public void  Init(string userID)
        {
            if (DataGame.User == null || DataGame.User.UserID != userID)
            {
                DataGame.DataUsers.Add(userID, new Data_User(userID));

            }
            
            DataGame.User = DataGame.DataUsers[userID];
            

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

            //GameEntry.Setting.SetString(Constant.Game.VersionKey, Application.version);

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
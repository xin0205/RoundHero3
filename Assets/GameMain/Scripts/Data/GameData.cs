
using System.Collections.Generic;
using CatJson;

namespace RoundHero
{
    [JsonCareDefaultValue]
    public class Data_User
    {
        public string UserID;
        public int CurFileIdx = -1;
        public Dictionary<int, Dictionary<EPVEType, Data_GamePlay>> GamePlayDatas = new ();
        public Data_GamePlay CurGamePlayData = new ();
        public List<int> DefaultInitSelectCards = new List<int>();
        
        public Data_User()
        {
            
        }
        
        public Data_User(string userID)
        {
            UserID = userID;
            CurFileIdx = 0;
            SetGamePlayData(EPVEType.Battle, CurFileIdx);
            SetGamePlayData(EPVEType.Test, CurFileIdx);
        }

        public void SetGamePlayData(EPVEType pveType, int fileIdx)
        {
            if (!GamePlayDatas.ContainsKey(fileIdx))
            {
                GamePlayDatas.Add(fileIdx, new Dictionary<EPVEType, Data_GamePlay>()
                {
                    [pveType] = new Data_GamePlay(),
                });
            }
            
            // if (CurFileIdx == -1)
            // {
            //     CurFileIdx = 0;
            //     CurGamePlayData = new Data_GamePlay();
            //     GamePlayDatas.Add(fileIdx, new Dictionary<EPVEType, Data_GamePlay>()
            //     {
            //         [pveType] = CurGamePlayData,
            //     });
            //     
            // }
            // else
            // {
            //     CurGamePlayData = GamePlayDatas[CurFileIdx][pveType];
            // }
        }

        public void SetCurGamePlayData(EPVEType pveType)
        {
            SetGamePlayData(pveType, CurFileIdx);
            CurGamePlayData = GamePlayDatas[CurFileIdx][pveType];
        }

        public void Clear(EPVEType pveType)
        {
            GamePlayDatas[CurFileIdx][pveType] = new Data_GamePlay();
            CurGamePlayData = GamePlayDatas[CurFileIdx][pveType];

            // CurGamePlayData = new Data_GamePlay();
            // GamePlayDatas[CurFileIdx] = CurGamePlayData;
        }
    }
    
    
    public class Data_Game
    {
        //public Dictionary<string, Data_User> DataUsers = new Dictionary<string, Data_User>();
        public Data_User User;
        
        public Data_Game()
        {
            
        }

        // public void Clear()
        // {
        //     User.Clear();
        // }
        
        public void Clear(EPVEType pveType)
        {
            User.Clear(pveType);
        }
        
        // public Data_User GetUserData(string userID)
        // {
        //     if (!DataUsers.TryGetValue(userID, out var userData))
        //     {
        //         userData = new Data_User(userID);
        //         
        //         DataUsers.Add(userID, userData);
        //     }
        //     
        //     
        //     return  userData;
        //
        // }
    }
}

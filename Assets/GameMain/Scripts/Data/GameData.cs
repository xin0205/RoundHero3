
using System.Collections.Generic;
using CatJson;

namespace RoundHero
{
    [JsonCareDefaultValue]
    public class Data_User
    {
        public string UserID;
        public int CurFileIdx = -1;
        public Dictionary<int, Data_GamePlay> GamePlayDatas = new ();
        public Data_GamePlay CurGamePlayData = new ();
        public List<int> DefaultInitSelectCards = new List<int>();
        
        public Data_User()
        {
            
        }
        
        public Data_User(string userID)
        {
            UserID = userID;
            
            if (CurFileIdx == -1)
            {
                CurFileIdx = 0;
                CurGamePlayData = new Data_GamePlay();
                GamePlayDatas.Add(CurFileIdx, CurGamePlayData);
                
            }
            else
            {
                CurGamePlayData = GamePlayDatas[CurFileIdx];
            }
            
        }

        public void Clear()
        {
            CurGamePlayData = new Data_GamePlay();
            GamePlayDatas[CurFileIdx] = CurGamePlayData;
        }
    }
    
    
    public class Data_Game
    {
        //public Dictionary<string, Data_User> DataUsers = new Dictionary<string, Data_User>();
        public Data_User User;
        
        public Data_Game()
        {
            
        }

        public void Clear()
        {
            User.Clear();
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

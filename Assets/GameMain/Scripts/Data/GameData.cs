
using System.Collections.Generic;

namespace RoundHero
{
    public class Data_User
    {
        public string UserID;
        public Data_Player PlayerData;
        public Data_GamePlay GamePlayData = new Data_GamePlay();
        
        public Data_User(string userID)
        {
            UserID = userID;
            
        }
    }
    
    public class Data_Game
    {
        public Dictionary<string, Data_User> DataUsers = new Dictionary<string, Data_User>();

        public Data_Game()
        {
            
        }
        
        public Data_User GetUserData(string userID)
        {
            if (!DataUsers.TryGetValue(userID, out var userData))
            {
                userData = new Data_User(userID);
                
                DataUsers.Add(userID, userData);
            }
            
            
            return  userData;

        }
    }
}

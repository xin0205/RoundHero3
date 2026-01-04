
namespace RoundHero
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        public Data_Player PlayerData => DataManager.Instance.DataGame.User.CurGamePlayData.PlayerData;

        public void Init()
        {
            
        }

        public ulong GetPlayerID(EUnitCamp unitCamp)
        {
            if (GamePlayManager.Instance.GamePlayData.PlayerDataCampDict.ContainsKey(unitCamp))
                return GamePlayManager.Instance.GamePlayData.PlayerDataCampDict[unitCamp].PlayerID;
            
            return 0;

        }
    }
}
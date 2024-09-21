using Steamworks;

namespace RoundHero
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        public Data_Player PlayerData;

        public void Init()
        {
            PlayerData = DataManager.Instance.CurUser.PlayerData;
        }

        public ulong GetPlayerID(EUnitCamp unitCamp)
        {
            if (GamePlayManager.Instance.GamePlayData.PlayerDatas.ContainsKey(unitCamp))
                return GamePlayManager.Instance.GamePlayData.PlayerDatas[unitCamp].PlayerID;
            
            return 0;

        }
    }
}
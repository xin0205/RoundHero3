using System.Collections.Generic;

namespace RoundHero
{
    public class GameManager : Singleton<GameManager>
    {
        public int TmpHeroID = 0;
        public List<int> InitCards = new List<int>();
        
        public List<int> CardsForm_EquipFuneIdxs = new List<int>();
        
        public bool IsStartBattle = false;
        
        
        public Data_Game GameData => DataManager.Instance.DataGame;
    }
}
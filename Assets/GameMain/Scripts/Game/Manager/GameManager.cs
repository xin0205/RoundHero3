using System.Collections.Generic;

namespace RoundHero
{
    public class GameManager : Singleton<GameManager>
    {
        public int TmpHeroID = 0;
        public List<int> TmpInitCards = new List<int>();
        
        public List<int> CardsForm_EquipFuneIdxs = new List<int>();
        
        
        
    }
}
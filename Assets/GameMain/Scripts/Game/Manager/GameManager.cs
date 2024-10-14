using System.Collections.Generic;

namespace RoundHero
{
    public class GameManager : Singleton<GameManager>
    {
        public int StartSelect_HeroID = -1;
        public List<int> Cards = new List<int>();
        
        
        
    }
}
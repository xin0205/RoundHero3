using System.Collections.Generic;

namespace RoundHero
{
    public class GameManager : Singleton<GameManager>
    {
        public EHeroID StartSelect_HeroID = EHeroID.Empty;
        public List<int> Cards = new List<int>();
        
        
        
    }
}
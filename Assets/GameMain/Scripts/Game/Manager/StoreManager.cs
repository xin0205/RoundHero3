using System;

namespace RoundHero
{
    public class StoreManager : Singleton<FuneManager>
    {
        public Random Random;
        private int randomSeed;
        
        public void Init(int randomSeed)
        {
            
            this.randomSeed = randomSeed;
            Random = new Random(randomSeed);

        }
    }
}
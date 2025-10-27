using System.Collections.Generic;
using UnityEngine;

namespace RoundHero
{
    public enum BattleModeStage
    {
        Battle,
        Reward,
    }
    
    public class BattleModeProduce
    {
        public int Session;
        public BattleModeStage BattleModeStage;
        public int RewardRandomSeed;

        public List<SelectAcquireItemData> selectAcquireItemDatas = new List<SelectAcquireItemData>();

        public BattleModeProduce Copy()
        {
            var battleModeProduce = new BattleModeProduce();
            battleModeProduce.Session = Session;
            battleModeProduce.BattleModeStage = BattleModeStage;
            return battleModeProduce;
        }
    }
    
    public class BattleModeManager: Singleton<BattleModeManager>
    {
        //public System.Random Random;
        private int randomSeed;
        
        public int RandomIdx
        {
            get
            {
                return BattleManager.Instance.BattleData.BattleModeRandomIdx;
            }

            set
            {
                BattleManager.Instance.BattleData.BattleModeRandomIdx = value;
            }
        }

        public List<int> RandomCaches = new List<int>();

        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            var random = new System.Random(this.randomSeed);
            
            RandomCaches.Clear();
            for (int i = 0; i < 100; i++)
            {
                RandomCaches.Add(random.Next());
            }
        }
        
        public void Destory()
        {
            
        }
        
        public int GetRandomSeed()
        {
            return RandomCaches[RandomIdx++ % RandomCaches.Count];
        }
    }
}
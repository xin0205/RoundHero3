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
        public System.Random Random;
        private int randomSeed;

        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
        }
        
        public void Destory()
        {
            
        }
    }
}
using UnityEngine;

namespace RoundHero
{
    public class BattleHeroEntityData : EntityData
    {
        public Data_BattleHero BattleHeroData;

        public void Init(int entityId, Vector3 pos, Data_BattleHero heroData)
        {
            base.Init(entityId, pos);
            BattleHeroData = heroData;

        }
    }
}
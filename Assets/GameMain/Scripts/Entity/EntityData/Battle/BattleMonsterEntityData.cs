
using UnityEngine;

namespace RoundHero
{
    public class BattleMonsterEntityData : EntityData
    {
        public Data_BattleMonster BattleMonsterData;
        
        public void Init(int entityId, Vector3 pos, Data_BattleMonster battleMonsterData)
        {
            base.Init(entityId, pos);
            this.BattleMonsterData = battleMonsterData;
    
        }
    }
}
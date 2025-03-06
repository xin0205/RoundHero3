using UnityEngine;

namespace RoundHero
{
    public class BattleCoreEntityData : EntityData
    {
        public Data_BattleCore BattleCoreData;

        public void Init(int entityId, Vector3 pos, Data_BattleCore battleCoreData)
        {
            base.Init(entityId, pos);
            BattleCoreData = battleCoreData;


        }
    }
}
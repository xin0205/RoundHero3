using UnityEngine;

namespace RoundHero
{
    public class BattleSoliderEntityData : EntityData
    {
        public Data_BattleSolider BattleSoliderData;
        public void Init(int entityId, Vector3 pos, Data_BattleSolider battleSoliderData)
        {
            base.Init(entityId, pos);
            BattleSoliderData = battleSoliderData;

        }
    }
}
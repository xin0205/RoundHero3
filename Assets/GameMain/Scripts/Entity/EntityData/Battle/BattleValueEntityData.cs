using UnityEngine;

namespace RoundHero
{
    public class BattleValueEntityData : EntityData
    {
        public int Value;
        public int EntityIdx;
        public Vector3 TargetPosition;
        
        public void Init(int entityId, Vector3 targetPos, int value, int entityIdx)
        {
            base.Init(entityId, targetPos);
            this.Value = value;
            this.TargetPosition = targetPos;
            EntityIdx = entityIdx;
        }
    }
}
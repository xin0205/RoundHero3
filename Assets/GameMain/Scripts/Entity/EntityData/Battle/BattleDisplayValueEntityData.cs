using UnityEngine;

namespace RoundHero
{
    public class BattleDisplayValueEntityData : EntityData
    {
        public int Value;
        public int EntityIdx;
        public Vector3 TargetPosition;
        
        public void Init(int entityId, Vector3 pos, Vector3 targetPos, int value, int entityIdx)
        {
            base.Init(entityId, pos);
            this.Value = value;
            this.TargetPosition = targetPos;
            EntityIdx = entityIdx;
        }
    }
}
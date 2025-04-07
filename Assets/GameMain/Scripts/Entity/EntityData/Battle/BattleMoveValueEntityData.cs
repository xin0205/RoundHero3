using UnityEngine;

namespace RoundHero
{
    public class BattleMoveValueEntityData : EntityData
    {
        public int Value;
        public Vector3 TargetPos;
        public int EntityIdx;
        public bool IsLoop;
        public bool IsAdd;
        
        public void Init(int entityId, Vector3 pos, Vector3 targetPos, int value, int entityIdx = -1, bool isLoop = false, bool isAdd = false)
        {
            base.Init(entityId, pos);
            this.TargetPos = targetPos;
            this.Value = value;
            this.EntityIdx = entityIdx;
            IsLoop = isLoop;
            IsAdd = isAdd;
        }
    }
}
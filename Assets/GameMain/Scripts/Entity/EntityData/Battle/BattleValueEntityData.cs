using UnityEngine;

namespace RoundHero
{
    public class BattleValueEntityData : EntityData
    {
        public int Value;
        public Vector3 TargetPos;
        
        public void Init(int entityId, Vector3 pos, Vector3 targetPos, int value)
        {
            base.Init(entityId, pos);
            this.TargetPos = targetPos;
            this.Value = value;
    
        }
    }
}
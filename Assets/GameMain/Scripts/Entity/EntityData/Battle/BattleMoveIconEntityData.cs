using UnityEngine;

namespace RoundHero
{
    public class BattleMoveIconEntityData : EntityData
    {
        public EUnitState UnitState;
        public int EntityIdx;
        public bool IsLoop;
        
        public MoveParams FollowParams;
        public MoveParams TargetFollowParams;
        
        //public bool IsAdd;
        public void Init(int entityId, EUnitState unitState, int entityIdx = -1,
            bool isLoop = false, MoveParams followParams = null,MoveParams targetFollowParams = null)
        {
            base.Init(entityId, Vector3.zero);
            
            this.UnitState = unitState;
            this.EntityIdx = entityIdx;
            IsLoop = isLoop;
            
            FollowParams = followParams;
            TargetFollowParams = targetFollowParams;
        }
    }
}
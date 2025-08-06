using UnityEngine;

namespace RoundHero
{
    public class MoveParams
    {
        public GameObject FollowGO;
        public Vector2 DeltaPos;
        public bool IsUIGO;
    }
    
    public class BattleMoveValueEntityData : EntityData
    {
        public int StartValue;
        public int EndValue;
        //public Vector3 TargetPos;
        public int EntityIdx;
        public bool IsLoop;
        public bool IsAdd;
        
        public MoveParams FollowParams;
        public MoveParams TargetFollowParams;
        
        
        //Vector3 pos, Vector3 targetPos, 

        public void Init(int entityId, int startValue, int endValue, int entityIdx = -1, bool isLoop = false,
            bool isAdd = false, MoveParams followParams = null,MoveParams targetFollowParams = null)

        {
            base.Init(entityId, Vector3.zero);
            //this.TargetPos = targetPos;
            this.StartValue = startValue;
            this.EndValue = endValue;
            this.EntityIdx = entityIdx;
            IsLoop = isLoop;
            IsAdd = isAdd;
            
            FollowParams = followParams;
            TargetFollowParams = targetFollowParams;
            
        }
    }
}
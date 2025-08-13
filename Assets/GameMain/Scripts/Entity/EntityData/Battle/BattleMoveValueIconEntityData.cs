using UnityEngine;

namespace RoundHero
{

    public class UnitStateIconValueEntityData : BattleMoveValueEntityData
    {
        public EUnitState UnitState;

        public void Init(int entityId, int startValue, int endValue, EUnitState unitState, int entityIdx = -1, bool isLoop = false,
            bool isAdd = false, MoveParams followParams = null,MoveParams targetFollowParams = null)
        {
            base.Init(entityId, startValue, endValue, entityIdx, isLoop,
                isAdd, followParams, targetFollowParams);
            this.UnitState = unitState;
        }
        
    }
    
    public class BlessIconValueEntityData : BattleMoveValueEntityData
    {
        public EBlessID BlessID;

        public void Init(int entityId, int startValue, int endValue, EBlessID blessID, int entityIdx = -1, bool isLoop = false,
            bool isAdd = false, MoveParams followParams = null,MoveParams targetFollowParams = null)
        {
            base.Init(entityId, startValue, endValue, entityIdx, isLoop,
                isAdd, followParams, targetFollowParams);
            this.BlessID = blessID;
        }
        
    }
    
    public class FuneIconValueEntityData : BattleMoveValueEntityData
    {
        public int FuneIdx;

        public void Init(int entityId, int startValue, int endValue, int funeIdx, int entityIdx = -1, bool isLoop = false,
            bool isAdd = false, MoveParams followParams = null,MoveParams targetFollowParams = null)
        {
            base.Init(entityId, startValue, endValue, entityIdx, isLoop,
                isAdd, followParams, targetFollowParams);
            this.FuneIdx = funeIdx;
        }
        
    }
}

using UnityEngine;

namespace RoundHero
{
    public class BattleFlyDirectEntityData : EntityData
    {
        public ERelativePos Direct;
        public int GridPosIdx;
        public int TargetGridfPosIdx;
        public int EntityIdx;
        
        public void Init(int entityId, int gridPosIdx, ERelativePos direct, int entityIdx)
        {
            var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
            base.Init(entityId, pos);
            GridPosIdx = gridPosIdx;
            Direct = direct;
            EntityIdx = entityIdx;

        }
        
        public void Init(int entityId, int gridPosIdx, int targetGridfPosIdx, int entityIdx)
        {
            var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
            base.Init(entityId, pos);
            GridPosIdx = gridPosIdx;
            TargetGridfPosIdx = targetGridfPosIdx;
            EntityIdx = entityIdx;

        }
    }
}
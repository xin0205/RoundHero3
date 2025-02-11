
using UnityEngine;

namespace RoundHero
{
    public class BattleFlyDirectEntityData : EntityData
    {
        public ERelativePos Direct;
        public int GridPosIdx;
        public int EntityIdx;
        
        public void Init(int entityId, int gridPosIdx, ERelativePos direct, int entityIdx)
        {
            var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
            base.Init(entityId, pos);
            Direct = direct;
            EntityIdx = entityIdx;

        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace RoundHero
{
    public class BattleRouteEntityData : EntityData
    {
        public List<int> GridPosIdxs;
        public int EntityIdx;

        public void Init(int entityId, Vector3 pos, List<int> gridPosIdxs, int entityIdx)
        {
            base.Init(entityId, pos);
            GridPosIdxs = gridPosIdxs;
            EntityIdx = entityIdx;

        }
    }
}
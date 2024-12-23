using System.Collections.Generic;
using UnityEngine;

namespace RoundHero
{
    public class BattleRouteEntityData : EntityData
    {
        public List<int> GridPosIdxs;
        public int RouteIdx;

        public void Init(int entityId, Vector3 pos, List<int> gridPosIdxs, int routeIdx)
        {
            base.Init(entityId, pos);
            GridPosIdxs = gridPosIdxs;
            RouteIdx = routeIdx;

        }
    }
}
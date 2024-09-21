using UnityEngine;

namespace RoundHero
{
    public class BattleGridEntityData : EntityData
    {
        public int GridPosIdx;
        public EGridType GridType;
        public void Init(int entityId, Vector3 pos, int posIdx, EGridType gridType)
        {
            base.Init(entityId, pos);
            GridPosIdx = posIdx;
            GridType = gridType;
            

        }
    }
}
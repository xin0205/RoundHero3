using System.Collections.Generic;
using UnityEngine;

namespace RoundHero
{
    public class LineMultiEffectEntityData : CommonEffectEntityData
    {
        public List<int> GridPosIdxs = new List<int>();
        
        public void Init(int entityId, Vector3 pos, EColor color, List<int> gridPosIdxs)
        {
            base.Init(entityId, pos, color);
            GridPosIdxs = gridPosIdxs;
            
        }
    }
}
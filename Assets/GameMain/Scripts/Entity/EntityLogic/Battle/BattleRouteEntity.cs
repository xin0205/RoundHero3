using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleRouteEntity : Entity
    {
        public BattleRouteEntityData BattleRouteEntityData { get; protected set; }

        [SerializeField]
        private LineRenderer line;
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            BattleRouteEntityData = userData as BattleRouteEntityData;
            if (BattleRouteEntityData == null)
            {
                Log.Error("Error BattleRouteEntityData");
                return;
            }

            line.positionCount = BattleRouteEntityData.GridPosIdxs.Count;

            var idx = 0;
            foreach (var gridPosIdx in BattleRouteEntityData.GridPosIdxs)
            {
                var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
                pos.y = 0.5f;
                line.SetPosition(idx++, pos);
            }
            

            line.material.SetInt("_Number", 3 * (BattleRouteEntityData.GridPosIdxs.Count - 1));

        }

        public void SetCurrent(bool isCurrent)
        {
            line.material.SetColor("_Color", isCurrent ? Color.yellow : Color.white); 
        }

    }
}
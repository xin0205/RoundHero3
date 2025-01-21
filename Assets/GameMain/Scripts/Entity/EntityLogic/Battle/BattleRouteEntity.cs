using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleRouteEntity : Entity
    {
        public BattleRouteEntityData BattleRouteEntityData { get; protected set; }

        [SerializeField]
        private LineRenderer line;

        private Color yellow = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.5f);
        
        private Color white = new Color(Color.white.r, Color.white.g, Color.white.b, 0.5f);

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

            line.startWidth = 0.02f;
            line.endWidth = 0.02f;

            line.material.SetInt("_Number", 3 * (BattleRouteEntityData.GridPosIdxs.Count - 1));
            SetCurrent(false);
        }

        public void SetCurrent(bool isCurrent)
        {
            line.material.SetColor("_Color", isCurrent ? yellow : white); 
        }

    }
}
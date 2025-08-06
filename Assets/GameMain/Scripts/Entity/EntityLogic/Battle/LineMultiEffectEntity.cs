using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class LineMultiEffectEntity : CommonEffectEntity
    {
        public LineMultiEffectEntityData LineMultiEffectEntityData { get; protected set; }
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            LineMultiEffectEntityData = userData as LineMultiEffectEntityData;
            if (LineMultiEffectEntityData == null)
            {
                Log.Error("Error LineMultiEffectEntityData");
                return;
            }

            ShowEffect();
        }
        
        protected override void ShowEffect()
        {
            base.ShowEffect();

            var fastDis = 0f;
            var fastGridPosIdx = 0;
            foreach (var gridPosIdx in LineMultiEffectEntityData.GridPosIdxs)
            {
                var gridPos = GameUtility.GridPosIdxToPos(gridPosIdx);
                var dis = Vector3.Distance(gridPos, transform.position);
                if (dis > fastDis)
                {
                    fastGridPosIdx = gridPosIdx;
                    fastDis = dis;
                }
            }
            var fastGridPos = GameUtility.GridPosIdxToPos(fastGridPosIdx);
            transform.LookAt(fastGridPos);

            
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1.2f * fastDis / (3f * Constant.Area.GridRange.x));


        }
    }
}
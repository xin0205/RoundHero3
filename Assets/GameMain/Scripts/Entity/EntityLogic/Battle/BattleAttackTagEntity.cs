using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleAttackTagEntity : Entity
    {
        public BattleAttackTagEntityData BattleAttackTagEntityData { get; protected set; }

        [SerializeField]
        private LineRenderer line;
        
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        
        private Color red = new Color(Color.red.r, Color.red.g, Color.red.b, 0.5f);

        protected Quaternion cameraQuaternion = Quaternion.identity;
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            BattleAttackTagEntityData = userData as BattleAttackTagEntityData;
            if (BattleAttackTagEntityData == null)
            {
                Log.Error("Error BattleAttackTagEntityData");
                return;
            }

            var endGridPosIdx = GameUtility.GridPosToPosIdx(BattleAttackTagEntityData.TargetPos);
            var startGridPosIdx = GameUtility.GridPosToPosIdx(BattleAttackTagEntityData.Position);
            var gridPosIdxs = GameUtility.GetMoveIdxs(startGridPosIdx, endGridPosIdx);

            var centerPoint = GameUtility.GetBetweenPoint(BattleAttackTagEntityData.Position,
                BattleAttackTagEntityData.TargetPos);
            centerPoint.y += 1f;

            spriteRenderer.transform.position = centerPoint;

            line.positionCount = gridPosIdxs.Count;

            var idx = 0;
            foreach (var gridPosIdx in gridPosIdxs)
            {
                var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
                pos.y += 1f;
                line.SetPosition(idx++, pos);
            }
            

            line.material.SetInt("_Number", 3 * (gridPosIdxs.Count - 1));
            line.material.SetColor("_Color", red); 
            line.material.SetInt("_Speed", 20);
        }

        private void Update()
        {
            cameraQuaternion.SetLookRotation(Camera.main.transform.forward, Camera.main.transform.up);
            spriteRenderer.transform.rotation = cameraQuaternion;

            var dis = Mathf.Abs(AreaController.Instance.GetDistanceToPoint(transform.position));
            
            spriteRenderer.transform.localScale = Vector3.one *  dis / 90f;
        }

    }
}
﻿
using UnityEngine;

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

            
            var startGridPosIdx = GameUtility.GridPosToPosIdx(BattleAttackTagEntityData.StartPos);
            var endGridPosIdx = GameUtility.GridPosToPosIdx(BattleAttackTagEntityData.TargetPos);
            var gridPosIdxs = GameUtility.GetMoveIdxs(startGridPosIdx, endGridPosIdx);

            // var centerPoint = GameUtility.GetBetweenPoint(BattleAttackTagEntityData.StartPos,
            //     BattleAttackTagEntityData.TargetPos);
            // centerPoint.y += 1f;
            
            // spriteRenderer.transform.position = centerPoint;
            //
            spriteRenderer.gameObject.SetActive(BattleAttackTagEntityData.ShowAttackPos);
            line.gameObject.SetActive(BattleAttackTagEntityData.ShowAttackLine);

            
            if (BattleAttackTagEntityData.ShowAttackPos)
            {
                spriteRenderer.transform.position = new Vector3(BattleAttackTagEntityData.TargetPos.x,
                    BattleAttackTagEntityData.TargetPos.y + 0.1f, BattleAttackTagEntityData.TargetPos.z);
            }
            

            //line.gameObject.SetActive(false);
            if (BattleAttackTagEntityData.ShowAttackLine)
            {
                line.positionCount = gridPosIdxs.Count;

                var idx = 0;
                foreach (var gridPosIdx in gridPosIdxs)
                {
                    var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
                    pos.y += 1f;
                    line.SetPosition(idx++, pos);
                }
            
                line.startWidth = 0.02f;
                line.endWidth = 0.02f;
                line.material.SetInt("_Number", 3 * (gridPosIdxs.Count - 1));
                line.material.SetColor("_Color", red); 
                line.material.SetInt("_Speed", 50);
            }
            
        }

        private void Update()
        {
            // cameraQuaternion.SetLookRotation(Camera.main.transform.forward, Camera.main.transform.up);
            // spriteRenderer.transform.rotation = cameraQuaternion;
            //
            // var dis = Mathf.Abs(AreaController.Instance.GetDistanceToPoint(transform.position));
            //
            // spriteRenderer.transform.localScale = Vector3.one *  dis / 90f;
        }

    }
}
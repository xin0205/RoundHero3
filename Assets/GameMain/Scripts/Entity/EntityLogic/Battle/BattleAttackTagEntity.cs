
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
        
        private Color red = new Color(Color.red.r, Color.red.g, Color.red.b, 0.3f);
        private Color yellow = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.3f);

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
            var targetGridPosIdx = GameUtility.GridPosToPosIdx(BattleAttackTagEntityData.TargetPos);
            var gridPosIdxs = GameUtility.GetMoveIdxs(startGridPosIdx, targetGridPosIdx);

            // var centerPoint = GameUtility.GetBetweenPoint(BattleAttackTagEntityData.StartPos,
            //     BattleAttackTagEntityData.TargetPos);
            // centerPoint.y += 1f;
            
            // spriteRenderer.transform.position = centerPoint;
            //
            //spriteRenderer.color = 
            spriteRenderer.gameObject.SetActive(BattleAttackTagEntityData.ShowAttackPos);
            line.gameObject.SetActive(BattleAttackTagEntityData.ShowAttackLine);

            spriteRenderer.gameObject.SetActive(BattleAttackTagEntityData.ShowAttackPos);
            if (BattleAttackTagEntityData.ShowAttackPos)
            {
                spriteRenderer.transform.position = new Vector3(BattleAttackTagEntityData.TargetPos.x,
                    BattleAttackTagEntityData.TargetPos.y + 0.1f, BattleAttackTagEntityData.TargetPos.z);
                
                // var gridType = GameUtility.GetGridType(targetGridPosIdx, false);
                // if (gridType == EGridType.Unit || gridType == EGridType.TemporaryUnit)
                // {
                //     spriteRenderer.gameObject.SetActive(true);
                //     spriteRenderer.color = red;
                // }
                // else if (gridType == EGridType.Empty)
                // {
                //     spriteRenderer.gameObject.SetActive(true);
                //     spriteRenderer.color = yellow;
                // }
                // else
                // {
                //     spriteRenderer.gameObject.SetActive(false);
                // }
                var gridType = GameUtility.GetGridType(targetGridPosIdx, false);
                
                spriteRenderer.gameObject.SetActive(gridType != EGridType.Obstacle);
                
                spriteRenderer.color =  BattleAttackTagEntityData.ShowAttackLine ? red : yellow;
            }

            
            

            //line.gameObject.SetActive(false);
            if (BattleAttackTagEntityData.ShowAttackLine)
            {
                if (BattleAttackTagEntityData.BuffValue.BuffData.TriggerRange.ToString().Contains("Extend"))
                {
                    line.positionCount = gridPosIdxs.Count;
                    line.material.SetInt("_Number", 3 * (gridPosIdxs.Count - 1));
                    var idx = 0;
                    foreach (var gridPosIdx in gridPosIdxs)
                    {
                        var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
                        pos.y += 1f;
                        line.SetPosition(idx++, pos);
                    }
                }
                // if (BattleAttackTagEntityData.BuffValue.BuffData.TriggerRange.ToString().Contains("Parabola"))
                else
                {
                    var startPos = GameUtility.GridPosIdxToPos(startGridPosIdx) + new Vector3(0, 1f, 0);
                    var endPos = GameUtility.GridPosIdxToPos(targetGridPosIdx) + new Vector3(0, 1f, 0);
                    
                    var deg = new Vector2(endPos.x - startPos.x, endPos.z -  startPos.z);
                    var dis = Vector3.Distance(startPos, endPos);
                    var radian = Vector2.SignedAngle(new Vector2(1, 0), deg) * Mathf.Deg2Rad;

                    var time = dis * Constant.Battle.ParabolaBulletShootTime / Constant.Area.GridRange.x;
                    var horizontalVelocity = dis / time;

                    var verticalVelocity = Constant.Battle.G * 0.5f * time;

                    var gridCount = (int)Mathf.Ceil(dis / Constant.Area.GridRange.x);
                    var lineCount = gridCount * 10;
                    var intervalTime = time / lineCount;

                    line.positionCount = (int)lineCount;
                    line.material.SetInt("_Number", 3 * gridCount);

                    var addTime = 0f;
                    for (int i = 0; i < lineCount; i++)
                    {
                        var posY = verticalVelocity * addTime + 0.5f * -Constant.Battle.G * addTime * addTime;
                        line.SetPosition(i,
                            startPos + new Vector3(horizontalVelocity * addTime * Mathf.Cos(radian), posY,
                                horizontalVelocity * addTime * Mathf.Sin(radian)));

                        addTime += intervalTime;

                    }
                }
                
            
                line.startWidth = 0.02f;
                line.endWidth = 0.02f;
                
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
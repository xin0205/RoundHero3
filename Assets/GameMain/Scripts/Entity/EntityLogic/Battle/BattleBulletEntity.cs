using DG.Tweening;

using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleBulletEntity : Entity
    {
        public BattleBulletEntityData BattleBulletEntityData { get; protected set; }
        
        [SerializeField]
        private GameObject explodeParticleTemp;
        [SerializeField]
        private GameObject bulletParticleTemp;
        [SerializeField]
        private GameObject shootParticleTemp;
        [SerializeField]
        private GameObject[] trailParticles;

        private GameObject explodeParticle;
        private GameObject bulletParticle;
        private GameObject shootParticle;
        
        private GameObject[] trails;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            BattleBulletEntityData = userData as BattleBulletEntityData;
            if (BattleBulletEntityData == null)
            {
                Log.Error("Error BattleBulletEntityData");
                return;
            }

            ShowShootParticle();
            ShowBulletParticle();
            //Move();
            InitParabola();
        }

        private void ShowShootParticle()
        {
            shootParticle = Instantiate(shootParticleTemp, transform.position, transform.rotation) as GameObject;
            shootParticle.transform.parent = transform;
            
            GameUtility.DelayExcute(2f, () =>
            {
                Destroy(shootParticle);
            });
        }
        
        private void ShowBulletParticle()
        {
            bulletParticle = Instantiate(bulletParticleTemp, transform.position, transform.rotation) as GameObject;
            bulletParticle.transform.parent = transform;
        }
        
        private void ShowExplodeParticle()
        {
            explodeParticle = Instantiate(explodeParticleTemp, transform.position, transform.rotation) as GameObject;
            explodeParticle.transform.parent = transform;
            explodeParticle.transform.position = bulletParticle.transform.position;
         
            Destroy(bulletParticle);
            bulletParticle = null;
            
            GameUtility.DelayExcute(2f, () =>
            {
                Destroy(explodeParticle);
            });
            
            GameUtility.DelayExcute(3f, () =>
            {

                GameEntry.Entity.HideEntity(this);
            });
        }

        private float moveTime = 0f;
        private float radian = 0f;
        private Vector3 startPos;
        private Vector3 endPos;
        private float dis;
        private float horizontalVelocity = 0f;
        private float verticalVelocity = 0f;
        private float g = 9.8f;
        public void InitParabola()
        {
            moveTime = 0;
            var moveGridPosIdx = BattleBulletEntityData.BulletData.MoveGridPosIdxs;
            var startIdx = BattleBulletEntityData.BulletData.MoveGridPosIdxs[0];
            var endIdx = BattleBulletEntityData.BulletData.MoveGridPosIdxs[moveGridPosIdx.Count - 1];

            startPos = GameUtility.GridPosIdxToPos(startIdx);
            endPos = GameUtility.GridPosIdxToPos(endIdx);

            var a = new Vector2(endPos.x - startPos.x, endPos.z -  startPos.z);
            dis = Vector3.Distance(startPos, endPos);
            radian = Vector2.Angle(a, new Vector2(1, 0)) * Mathf.Deg2Rad;

            var time = dis * Constant.Battle.BulletShootTime * 4f / Constant.Area.GridRange.x;
            horizontalVelocity = dis / time;

            verticalVelocity = g * 0.5f * time;
            
            
            //verticalVelocity = Mathf.Sqrt(2 * g * 2.5f);
            GameUtility.DelayExcute(time, () =>
            {
                
                ShowExplodeParticle();

                if (BattleBulletEntityData.BulletData.TriggerActionDataDict.Contains(endIdx))
                {
                    foreach (var triggerActionData in BattleBulletEntityData.BulletData.TriggerActionDataDict[endIdx])
                    {
                        if (triggerActionData.TriggerData != null)
                        {
                            BattleFightManager.Instance.TriggerAction(triggerActionData.TriggerData);

                        }

                        if (triggerActionData.MoveUnitData != null)
                        {
                            BattleBulletManager.Instance.UseMoveActionData(triggerActionData.MoveUnitData);
                        }

                        BattleManager.Instance.RefreshView();
                    }
                }

            });
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (bulletParticle != null)
            {
                moveTime += Time.deltaTime;

                var posY = verticalVelocity * moveTime + 0.5f * -g * moveTime * moveTime;
                bulletParticle.transform.position = startPos + new Vector3(horizontalVelocity * moveTime * Mathf.Cos(radian), posY, horizontalVelocity * moveTime * Mathf.Sin(radian));

            }
            
            
        }

        private void Move()
        {
            for (int i = 0; i < BattleBulletEntityData.BulletData.MoveGridPosIdxs.Count; i++)
            {
                var pos = GameUtility.GetMovePos(EUnitActionState.Fly, BattleBulletEntityData.BulletData.MoveGridPosIdxs, i, false);
                
                var tIdx = i;

                var time = Constant.Battle.BulletShootTime * (i - 1);
                time = time < 0 ? 0 : time;

                GameUtility.DelayExcute(time, () =>
                {
                    var moveTIdx = tIdx;
                    
                    var movePos = pos;
                    var moveGridPosIdx = BattleBulletEntityData.BulletData.MoveGridPosIdxs[moveTIdx];
                    movePos.y = transform.position.y;
                    bulletParticle.transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                    

                    bulletParticle.transform.DOMove(movePos, moveTIdx == 0 ? 0 : Constant.Battle.BulletShootTime).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        if (BattleBulletEntityData.BulletData.TriggerActionDataDict.Contains(moveGridPosIdx))
                        {
                            ShowExplodeParticle();
      
                            foreach (var triggerActionData in BattleBulletEntityData.BulletData.TriggerActionDataDict[moveGridPosIdx])
                            {
                                if (triggerActionData.TriggerData != null)
                                {
                                    BattleFightManager.Instance.TriggerAction(triggerActionData.TriggerData);

                                }
                                
                                if (triggerActionData.MoveUnitData != null)
                                {
                                    BattleBulletManager.Instance.UseMoveActionData(triggerActionData.MoveUnitData);
                                }
                                
                                BattleManager.Instance.RefreshView();
                            }
                            
                        }
                        
                    });
                    
                    

                });
                
                
                
            }
            
            

            var moveCount = BattleBulletEntityData.BulletData.MoveGridPosIdxs.Count > 1 ? BattleBulletEntityData.BulletData.MoveGridPosIdxs.Count - 1 : 1;
            
            GameUtility.DelayExcute(moveCount * Constant.Battle.BulletShootTime  + 0.1f, () =>
            {
                BattleBulletManager.Instance.ClearData(BattleBulletEntityData.BulletData.ActionUnitID);
                
            });
            
            
        }

    }
}
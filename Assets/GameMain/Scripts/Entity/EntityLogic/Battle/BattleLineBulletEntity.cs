using DG.Tweening;

using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleLineBulletEntity : Entity
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
            Move();

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

        private void DestoryBulletParticle()
        {
            if(bulletParticle == null)
                return;
            
            Destroy(bulletParticle);
            bulletParticle = null;
        }
        
        private void ShowExplodeParticle()
        {
            explodeParticle = Instantiate(explodeParticleTemp, transform.position, transform.rotation) as GameObject;
            explodeParticle.transform.parent = transform;
            explodeParticle.transform.position = bulletParticle.transform.position;
         
            DestoryBulletParticle();
            
            GameUtility.DelayExcute(2f, () =>
            {
                Destroy(explodeParticle);
            });
            
            GameUtility.DelayExcute(10f, () =>
            {

                GameEntry.Entity.HideEntity(this);
            });
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
                                if (triggerActionData is TriggerActionTriggerData triggerActionTriggerData)
                                {
                                    if (triggerActionTriggerData.TriggerData != null)
                                    {
                                        BattleBulletManager.Instance.UseTriggerData(triggerActionTriggerData.TriggerData);

                                    }
                                }

                                if (triggerActionData is TriggerActionMoveData triggerActionMoveData)
                                {
                                    if (triggerActionMoveData.MoveUnitData != null)
                                    {
                                        BattleBulletManager.Instance.UseMoveActionData(triggerActionMoveData.MoveUnitData);
                                    }
                                }

                                BattleManager.Instance.RefreshView();
                            }
                            
                        }
                        
                    });

                });

            }
            
            

            var moveCount = BattleBulletEntityData.BulletData.MoveGridPosIdxs.Count > 1 ? BattleBulletEntityData.BulletData.MoveGridPosIdxs.Count - 1 : 1;
            
            GameUtility.DelayExcute(moveCount * Constant.Battle.BulletShootTime + 0.05f, () =>
            {
                HeroManager.Instance.UpdateCacheHPDelta();
            });
            GameUtility.DelayExcute(moveCount * Constant.Battle.BulletShootTime  + 1f, () =>
            {
                DestoryBulletParticle();
                
            });
            
        }

    }
}
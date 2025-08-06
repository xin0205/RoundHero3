using DG.Tweening;

using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleLineBulletEntity : Entity
    {
        public BattleBulletEntityData BattleBulletEntityData { get; protected set; }
        
        [SerializeField]
        private EColorGODictionary explodeParticleTemps;
        [SerializeField]
        private EColorGODictionary bulletParticleTemps;
        [SerializeField]
        private EColorGODictionary shootParticleTemps;
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
            var color = BattleBulletEntityData.BulletData.EffectColor;
            shootParticle = Instantiate(shootParticleTemps[color], transform.position, transform.rotation) as GameObject;
            shootParticle.transform.parent = transform;
            
            GameUtility.DelayExcute(2f, () =>
            {
                Destroy(shootParticle);
            });
        }
        
        private void ShowBulletParticle()
        {
            var color = BattleBulletEntityData.BulletData.EffectColor;
            bulletParticle = Instantiate(bulletParticleTemps[color], transform.position, transform.rotation) as GameObject;
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
            var color = BattleBulletEntityData.BulletData.EffectColor;
            explodeParticle = Instantiate(explodeParticleTemps[0], transform.position, transform.rotation) as GameObject;
            explodeParticle.transform.parent = transform;
            explodeParticle.transform.position = bulletParticle.transform.position;
         
            DestoryBulletParticle();
            
            GameUtility.DelayExcute(2f, () =>
            {
                Destroy(explodeParticle);
                GameEntry.Entity.HideEntity(this);
            });
            
            // GameUtility.DelayExcute(10f, () =>
            // {
            //
            //     GameEntry.Entity.HideEntity(this);
            // });
        }

        private void Move()
        {
            for (int i = 0; i < BattleBulletEntityData.BulletData.MoveGridPosIdxs.Count; i++)
            {
                var pos = GameUtility.GetMovePos(EUnitActionState.Fly, BattleBulletEntityData.BulletData.MoveGridPosIdxs, i, false);
                
                var tIdx = i;

                var time = Constant.Battle.LineBulletShootTime * (i - 1);
                time = time < 0 ? 0 : time;

                GameUtility.DelayExcute(time, () =>
                {
                    var moveTIdx = tIdx;
                    
                    var movePos = pos;
                    var moveGridPosIdx = BattleBulletEntityData.BulletData.MoveGridPosIdxs[moveTIdx];
                    movePos.y = transform.position.y;
                    bulletParticle.transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                    

                    bulletParticle.transform.DOMove(movePos, moveTIdx == 0 ? 0 : Constant.Battle.LineBulletShootTime).SetEase(Ease.Linear).OnComplete(() =>
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
            
            GameUtility.DelayExcute(moveCount * Constant.Battle.LineBulletShootTime + 0.05f, () =>
            {
                HeroManager.Instance.UpdateCacheHPDelta();
            });
            GameUtility.DelayExcute(moveCount * Constant.Battle.LineBulletShootTime  + 1f, () =>
            {
                DestoryBulletParticle();
                
            });
            
        }

    }
}
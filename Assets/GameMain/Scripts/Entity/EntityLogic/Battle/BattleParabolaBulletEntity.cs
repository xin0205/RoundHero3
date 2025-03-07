using DG.Tweening;

using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleParabolaBulletEntity : Entity
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

            startPos = GameUtility.GridPosIdxToPos(startIdx) + new Vector3(0, 1f, 0);
            endPos = GameUtility.GridPosIdxToPos(endIdx) + new Vector3(0, 1f, 0);;

            var a = new Vector2(endPos.x - startPos.x, endPos.z -  startPos.z);
            dis = Vector3.Distance(startPos, endPos);
            radian = Vector2.SignedAngle(new Vector2(1, 0), a) * Mathf.Deg2Rad;

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

                        HeroManager.Instance.UpdateCacheHPDelta();
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

    }
}
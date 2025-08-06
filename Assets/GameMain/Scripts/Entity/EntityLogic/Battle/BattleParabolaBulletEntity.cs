using DG.Tweening;

using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleParabolaBulletEntity : Entity
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
        
        //private GameObject[] trails;
        
        // public float initialSpeed = 10f;   // 初始速度
        // public float launchAngle = 45f;
        // public float gravity = -9.81f;     // 重力加速度
        //
        // private Vector3 startPosition;     // 起始位置
        // private Vector3 initialVelocity;   // 初始速度向量
        // private float elapsedTime;

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
            var color = BattleBulletEntityData.BulletData.EffectColor;
            shootParticle = Instantiate(shootParticleTemps[color], transform.position, transform.rotation) as GameObject;
            shootParticle.transform.parent = transform;
            shootParticle.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            
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
            bulletParticle.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            // startPosition = transform.position;
            // elapsedTime = 0f;
            //
            // float angleRad = launchAngle * Mathf.Deg2Rad;
            // initialVelocity = new Vector3(
            //     initialSpeed * Mathf.Cos(angleRad),
            //     initialSpeed * Mathf.Sin(angleRad),
            //     0
            // );
        }
        
        private void ShowExplodeParticle()
        {
            var color = BattleBulletEntityData.BulletData.EffectColor;
            explodeParticle = Instantiate(explodeParticleTemps[color], transform.position, transform.rotation) as GameObject;
            explodeParticle.transform.parent = transform;
            explodeParticle.transform.position = bulletParticle.transform.position;
            explodeParticle.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
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
        
        public void InitParabola()
        {
            moveTime = 0;
            var moveGridPosIdx = BattleBulletEntityData.BulletData.MoveGridPosIdxs;
            var startIdx = BattleBulletEntityData.BulletData.MoveGridPosIdxs[0];
            var endIdx = BattleBulletEntityData.BulletData.MoveGridPosIdxs[moveGridPosIdx.Count - 1];

            startPos = GameUtility.GridPosIdxToPos(startIdx) + new Vector3(0, 1f, 0);
            endPos = GameUtility.GridPosIdxToPos(endIdx) + new Vector3(0, 1f, 0);

            var deg = new Vector2(endPos.x - startPos.x, endPos.z -  startPos.z);
            dis = Vector3.Distance(startPos, endPos);
            radian = Vector2.SignedAngle(new Vector2(1, 0), deg) * Mathf.Deg2Rad;

            var time = dis * Constant.Battle.ParabolaBulletShootTime / Constant.Area.GridRange.x;
            horizontalVelocity = dis / time;

            verticalVelocity = Constant.Battle.G * 0.5f *  time;
            
            
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

                var posY = verticalVelocity * moveTime + 0.5f * -Constant.Battle.G * moveTime * moveTime;
                bulletParticle.transform.position = startPos + new Vector3(horizontalVelocity * moveTime * Mathf.Cos(radian), posY, horizontalVelocity * moveTime * Mathf.Sin(radian));

                var nextTime = moveTime + Time.deltaTime;
                 var posY2 = verticalVelocity * nextTime + 0.5f * -Constant.Battle.G * nextTime * nextTime;
                 var pos = startPos + new Vector3(horizontalVelocity * nextTime * Mathf.Cos(radian), posY2,
                     horizontalVelocity * nextTime * Mathf.Sin(radian));
                bulletParticle.transform.LookAt(pos);

            }
            
            
        }

    }
}
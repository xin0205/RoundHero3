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
            Move();
        }

        private void ShowShootParticle()
        {
            shootParticle = Instantiate(shootParticleTemp, transform.position, transform.rotation) as GameObject;
            shootParticle.transform.parent = transform;
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
        }

        private void Move()
        {
            for (int i = 0; i < BattleBulletEntityData.BulletData.MoveGridPosIdxs.Count; i++)
            {
                var moveGridPosIdx = BattleBulletEntityData.BulletData.MoveGridPosIdxs[i];

                var pos = GameUtility.GetMovePos(EUnitActionState.Fly, BattleBulletEntityData.BulletData.MoveGridPosIdxs, i);
                
                var tIdx = i;

                var time = Constant.Battle.BulletShootTime * (tIdx - 1);
                time = time < 0 ? 0 : time;

                GameUtility.DelayExcute(time, () =>
                {
                    var moveTIdx = tIdx;
                    
                    var movePos = pos;

                    transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                    

                    transform.DOMove(movePos, moveTIdx == 0 ? 0 : Constant.Battle.BulletShootTime).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        if (BattleBulletEntityData.BulletData.TriggerDataDict.ContainsKey(moveTIdx))
                        {
                            ShowExplodeParticle();
                            BattleFightManager.Instance.TriggerAction(
                                BattleBulletEntityData.BulletData.TriggerDataDict[moveTIdx]);
                        }
                        
                    });

                });
                
            }

            // var moveCount = BattleBulletEntityData.BulletData.MoveGridPosIdxs.Count > 1 ? BattleBulletEntityData.BulletData.MoveGridPosIdxs.Count - 1 : 1;
            //
            // GameUtility.DelayExcute(moveCount * Constant.Battle.BulletShootTime  + 0.1f, () =>
            // {
            //     
            //     ShowExplodeParticle();
            // });
        }

    }
}
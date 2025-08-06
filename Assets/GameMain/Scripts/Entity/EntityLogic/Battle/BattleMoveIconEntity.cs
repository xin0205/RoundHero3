using System.Net.Mime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleMoveIconEntity : Entity
    {
        public BattleMoveIconEntityData BattleMoveIconEntityData { get; protected set; }
        
        protected Quaternion cameraQuaternion = Quaternion.identity;
        
        private Tween moveTween;

        [SerializeField] private Image Icon;

        protected override async void OnShow(object userData)
        {
            transform.SetParent(AreaController.Instance.BattleFormRoot.transform);
            
            base.OnShow(userData);

            BattleMoveIconEntityData = userData as BattleMoveIconEntityData;
            if (BattleMoveIconEntityData == null)
            {
                Log.Error("Error BattleMoveIconEntityData");
                return;
            }
            this.time = 0;
           
            // KillTween();
            //
            Icon.sprite = await AssetUtility.GetUnitStateIcon(BattleMoveIconEntityData.UnitState);
            //
            //
            // var dis = Vector3.Distance(BattleMoveIconEntityData.TargetPos, CachedTransform.localPosition);
            // //Constant.Battle.BattleValueVelocity
            // var time = dis / (Constant.Battle.BattleValueVelocity * 100);
            //
            // if (time < 1.5f)
            // {
            //     time += 1.5f;
            // }
            this.time = 0f;
            
            //
            // var targetPos = BattleMoveIconEntityData.TargetPos;
            // moveTween = DOTween.To(()=>
            // {
            //     if(transform == null)
            //         return Vector4.zero;
            //     
            //     return transform.localPosition;
            // }, x =>
            // {
            //     if(this == null || transform == null)
            //         return;
            //     
            //     transform.localPosition = x;
            // }, targetPos, time).SetEase(Ease.InOutQuart);
            //
            //
            //
            // if (BattleMoveIconEntityData.IsLoop)
            // {
            //
            //     moveTween.SetLoops(-1);
            // }
            // else
            // {
            //     
            //     
            //     GameUtility.DelayExcute(time, () =>
            //     {
            //         if (GameEntry.Entity.HasEntity(this.Id))
            //         {
            //             GameEntry.Entity.HideEntity(this);
            //         }
            //         
            //     });
            //     
            // }
            
            GameUtility.DelayExcute(2, () =>
            {
                if (GameEntry.Entity.HasEntity(this.Id))
                {
                    GameEntry.Entity.HideEntity(this);
                }
                    
            });
            
            if (BattleMoveIconEntityData.FollowParams.IsUIGO)
            {
                startPos = BattleMoveIconEntityData.FollowParams.FollowGO.transform.localPosition;
                startPos += BattleMoveIconEntityData.FollowParams.DeltaPos;
            }
            
            if (BattleMoveIconEntityData.TargetFollowParams.IsUIGO)
            {
                endPos = BattleMoveIconEntityData.TargetFollowParams.FollowGO.transform.localPosition;
                endPos += BattleMoveIconEntityData.TargetFollowParams.DeltaPos;
            }
        }
        
        private float time = 0;        

        private Vector2 startPos = Vector2.zero;
        private Vector2 endPos = Vector2.zero;
        private void Update()
        {
            time += Time.deltaTime;
            
            if (!BattleMoveIconEntityData.FollowParams.IsUIGO)
            {
                startPos = PositionConvert.WorldPointToUILocalPoint(
                    AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), BattleMoveIconEntityData.FollowParams.FollowGO.transform.localPosition);
                startPos += BattleMoveIconEntityData.FollowParams.DeltaPos;
            }
            
            if (!BattleMoveIconEntityData.TargetFollowParams.IsUIGO)
            {
                endPos = PositionConvert.WorldPointToUILocalPoint(
                    AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), BattleMoveIconEntityData.TargetFollowParams.FollowGO.transform.localPosition);
                endPos += BattleMoveIconEntityData.TargetFollowParams.DeltaPos;
            }
            
            
            this.transform.localPosition = Vector2.Lerp(startPos, endPos, time);
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            //Log.Debug("OnHide" + BattleMoveValueEntityData.Id);

            KillTween();

        }

        private void KillTween()
        {
            if (moveTween == null)
            {
                Log.Debug("moveTween");
            }

            moveTween?.Kill();

        }
    }
}
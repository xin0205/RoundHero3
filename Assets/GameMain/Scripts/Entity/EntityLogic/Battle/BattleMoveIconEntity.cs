using System.Net.Mime;
using DG.Tweening;
using Unity.VisualScripting;
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

        [SerializeField] private GameObject positivesign;
        [SerializeField] private GameObject negativeSign;


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

            positivesign.SetActive(BattleMoveIconEntityData.Value > 0);
            negativeSign.SetActive(BattleMoveIconEntityData.Value < 0);

            
            Icon.sprite = await AssetUtility.GetUnitStateIcon(BattleMoveIconEntityData.UnitState);

 
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
            
            if(BattleMoveIconEntityData.FollowParams.FollowGO.IsDestroyed())
                return;
            
            if(BattleMoveIconEntityData.TargetFollowParams.FollowGO.IsDestroyed())
                return;
            
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
            
            if((Vector2)this.transform.localPosition == endPos)
            {
                if (BattleMoveIconEntityData.IsLoop )
                {
                    time = 0;
                }
                else
                {
                    if (GameEntry.Entity.HasEntity(this.Id))
                    {
                        GameEntry.Entity.HideEntity(this);
                    }
                }
                
            }
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
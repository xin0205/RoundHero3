using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleMoveValueEntity : Entity
    {
        public BattleMoveValueEntityData BattleMoveValueEntityData { get; protected set; }

        
        [SerializeField] private Text text;
        [SerializeField] private Color hurtColor;
        [SerializeField] private Color recoverColor;

        protected Quaternion cameraQuaternion = Quaternion.identity;
        private Tween textColTween;
        //private Tween moveTween;
        private Tween textStrTween;
        
        private string positiveStartValue;
        private string positiveEndValue;

        private string negativeStartValue;
        private string negativeEndValue;
        
        protected override void OnShow(object userData)
        {
            transform.SetParent(AreaController.Instance.BattleFormRoot.transform);
            
            base.OnShow(userData);

            BattleMoveValueEntityData = userData as BattleMoveValueEntityData;
            if (BattleMoveValueEntityData == null)
            {
                Log.Error("Error BattleMoveValueEntity");
                return;
            }

            //this.transform.position = BattleMoveValueEntityData.Position;
            //KillTween();

            
            //text.text = Mathf.Abs(BattleValueEntityData.Value).ToString();
            //text.color = BattleMoveValueEntityData.StartValue < 0 ? hurtColor : recoverColor;
            //text.color = hurtColor;

            this.time = 0;
            this.timeEnd = 0;

            //var dis = Vector3.Distance(BattleMoveValueEntityData.TargetPos, CachedTransform.localPosition);
            //Constant.Battle.BattleValueVelocity
            // var time = 0f;//dis / (Constant.Battle.BattleValueVelocity * 100);
            //
            // if (time < 1.5f)
            // {
            //     time += 1.5f;
            // }
            //textStrTween = 

            // if (BattleMoveValueEntityData.IsAdd)
            // {
            //     textColTween = DOTween.To(() =>
            //     {
            //         if (text == null)
            //             return Color.white;
            //
            //         return text.color;
            //     }, x =>
            //     {
            //         if (text == null)
            //             return;
            //
            //         text.color = x;
            //     }, recoverColor, time).SetEase(Ease.InOutQuart);
            // }

            // var targetPos = BattleMoveValueEntityData.TargetPos;
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
            
            //transform.DOMove(BattleMoveValueEntityData.TargetPos, time).SetEase(Ease.InOutQuart);
            var absStartValue = Mathf.Abs(BattleMoveValueEntityData.StartValue);
            var absEndValue = Mathf.Abs(BattleMoveValueEntityData.EndValue);

            positiveStartValue = "+" + absStartValue;
            positiveEndValue = "+" + absEndValue;
            
            negativeStartValue = BattleMoveValueEntityData.StartValue < 0 ? "-" + absStartValue.ToString() : absStartValue.ToString();
            negativeEndValue = BattleMoveValueEntityData.EndValue < 0 ? "-" + absEndValue.ToString() : absEndValue.ToString();
            
            text.text = BattleMoveValueEntityData.StartValue < 0
                ? negativeStartValue
                : BattleMoveValueEntityData.StartValue > 0 ? positiveStartValue: negativeStartValue;

            text.color = BattleMoveValueEntityData.StartValue < 0 ? hurtColor : recoverColor;
            
            // if (BattleMoveValueEntityData.IsAdd)
            // {
            //     textStrTween = DOTween.To(() =>
            //     {
            //         if(text == null)
            //             return "";
            //         return text.text;
            //     }, x =>
            //     {
            //         if(text == null)
            //             return;
            //         text.text = x;
            //     }, "+" + absEndValue, time).From("-" + absStartValue).SetEase(Ease.InOutExpo);
            // }

            // textColTween.Play();
            // textStrTween.Play();
            // moveTween.Play();

            
            // if (BattleMoveValueEntityData.IsLoop)
            // {
            //     textColTween.SetLoops(-1);
            //     textStrTween.SetLoops(-1);
            //     //moveTween.SetLoops(-1);
            // }
            // else
            // {
            //     
            //     
            //     // GameUtility.DelayExcute(time, () =>
            //     // {
            //     //     if (GameEntry.Entity.HasEntity(this.Id))
            //     //     {
            //     //         GameEntry.Entity.HideEntity(this);
            //     //     }
            //     //     
            //     // });
            //     // moveTween.OnComplete(() =>
            //     // {
            //     //     GameEntry.Entity.HideEntity(this);
            //     // });
            // }
            
            // GameUtility.DelayExcute(time / 2f, () =>
            // {
            //     text.text = "+" + Mathf.Abs(BattleMoveValueEntityData.Value);
            // });
            
            if (BattleMoveValueEntityData.FollowParams.IsUIGO)
            {
                startPos = BattleMoveValueEntityData.FollowParams.FollowGO.transform.localPosition;
                startPos += BattleMoveValueEntityData.FollowParams.DeltaPos;
            }
            
            if (BattleMoveValueEntityData.TargetFollowParams.IsUIGO)
            {
                endPos = BattleMoveValueEntityData.TargetFollowParams.FollowGO.transform.localPosition;
                endPos += BattleMoveValueEntityData.TargetFollowParams.DeltaPos;
            }
        }

        private float time = 0f;
        private float timeEnd = 0f;
        private Vector2 startPos = Vector2.zero;
        private Vector2 endPos = Vector2.zero;
        private void Update()
        {
            time += Time.deltaTime;

            
            if(BattleMoveValueEntityData.FollowParams.FollowGO.IsDestroyed())
                return;
            
            if(BattleMoveValueEntityData.TargetFollowParams.FollowGO.IsDestroyed())
                return;
            
            if (!BattleMoveValueEntityData.FollowParams.IsUIGO)
            {
                startPos = PositionConvert.WorldPointToUILocalPoint(
                    AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), BattleMoveValueEntityData.FollowParams.FollowGO.transform.localPosition);
                startPos += BattleMoveValueEntityData.FollowParams.DeltaPos;
            }
            
            if (!BattleMoveValueEntityData.TargetFollowParams.IsUIGO)
            {
                endPos = PositionConvert.WorldPointToUILocalPoint(
                    AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), BattleMoveValueEntityData.TargetFollowParams.FollowGO.transform.localPosition);
                endPos += BattleMoveValueEntityData.TargetFollowParams.DeltaPos;
            }

            
            
            this.transform.localPosition = Vector2.Lerp(startPos, endPos, time * 0.8f);
            
            if (BattleMoveValueEntityData.IsAdd)
            {
                if (time >= 0.5f * 0.8f)
                {
                    text.text = positiveEndValue;
                    text.color = recoverColor;
                }
                else
                {
                    text.text = negativeEndValue;
                    text.color = hurtColor;
                }
  
            }
            

            if((Vector2)this.transform.localPosition == endPos)
            {
                timeEnd += Time.deltaTime;
                if (timeEnd > 0.5f)
                {
                    timeEnd = 0f;
                    if (BattleMoveValueEntityData.IsLoop )
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
            
        }

        

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            //Log.Debug("OnHide" + BattleMoveValueEntityData.Id);

            //KillTween();

        }

        // private void KillTween()
        // {
        //     // if (moveTween == null)
        //     // {
        //     //     Log.Debug("moveTween");
        //     // }
        //     
        //     if (textColTween == null)
        //     {
        //         Log.Debug("textColTween");
        //     }
        //
        //     if (textStrTween == null)
        //     {
        //         Log.Debug("textStrTween");
        //     }
        //        
        //     
        //     //moveTween?.Kill();
        //     textColTween?.Kill();
        //     textStrTween?.Kill();
        // }
    }
}
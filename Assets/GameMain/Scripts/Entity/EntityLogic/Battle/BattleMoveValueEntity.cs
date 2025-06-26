using DG.Tweening;
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
        private Tween moveTween;
        private Tween textStrTween;

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
            KillTween();

            text.text = BattleMoveValueEntityData.Value < 0
                ? BattleMoveValueEntityData.Value.ToString()
                : BattleMoveValueEntityData.Value > 0 ? "+" + BattleMoveValueEntityData.Value : BattleMoveValueEntityData.Value.ToString();

            //text.text = Mathf.Abs(BattleValueEntityData.Value).ToString();
            //text.color = BattleValueEntityData.Value < 0 ? hurtColor : recoverColor;
            text.color = hurtColor;

            var dis = Vector3.Distance(BattleMoveValueEntityData.TargetPos, CachedTransform.localPosition);
            //Constant.Battle.BattleValueVelocity
            var time = dis / (Constant.Battle.BattleValueVelocity * 100);

            if (time < 2f)
            {
                time = 2f;
            }
            //textStrTween = 

            if (BattleMoveValueEntityData.IsAdd)
            {
                textColTween = DOTween.To(() =>
                {
                    if (text == null)
                        return Color.white;

                    return text.color;
                }, x =>
                {
                    if (text == null)
                        return;

                    text.color = x;
                }, recoverColor, time).SetEase(Ease.InOutQuart);
            }

            var targetPos = BattleMoveValueEntityData.TargetPos;
            moveTween = DOTween.To(()=>
            {
                if(transform == null)
                    return Vector4.zero;
                
                return transform.localPosition;
            }, x =>
            {
                if(this == null || transform == null)
                    return;
                
                transform.localPosition = x;
            }, targetPos, time).SetEase(Ease.InOutQuart);
            //transform.DOMove(BattleMoveValueEntityData.TargetPos, time).SetEase(Ease.InOutQuart);
            var absValue = Mathf.Abs(BattleMoveValueEntityData.Value);

            if (BattleMoveValueEntityData.IsAdd)
            {
                textStrTween = DOTween.To(() =>
                {
                    if(text == null)
                        return "";
                    return text.text;
                }, x =>
                {
                    if(text == null)
                        return;
                    text.text = x;
                }, "+" + absValue, time).From("-" + absValue).SetEase(Ease.InOutExpo);
            }

            // textColTween.Play();
            // textStrTween.Play();
            // moveTween.Play();

            
            if (BattleMoveValueEntityData.IsLoop)
            {
                textColTween.SetLoops(-1);
                textStrTween.SetLoops(-1);
                moveTween.SetLoops(-1);
            }
            else
            {
                
                
                GameUtility.DelayExcute(time, () =>
                {
                    if (GameEntry.Entity.HasEntity(this.Id))
                    {
                        GameEntry.Entity.HideEntity(this);
                    }
                    
                });
                // moveTween.OnComplete(() =>
                // {
                //     GameEntry.Entity.HideEntity(this);
                // });
            }
            
            // GameUtility.DelayExcute(time / 2f, () =>
            // {
            //     text.text = "+" + Mathf.Abs(BattleMoveValueEntityData.Value);
            // });
        }

        

        private void Update()
        {
            // if (BattleManager.Instance.BattleState == EBattleState.EndBattle)
            // {
            //     GameEntry.Entity.HideEntity(this);
            // }
            //
            // cameraQuaternion.SetLookRotation(Camera.main.transform.forward, Camera.main.transform.up);
            // transform.rotation = cameraQuaternion;
            // var dis = Mathf.Abs(AreaController.Instance.GetDistanceToPoint(transform.position));
            //
            // transform.localScale = Vector3.one *  dis / 8f;
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            //Log.Debug("OnHide" + BattleMoveValueEntityData.Id);

            KillTween();

        }

        private void KillTween()
        {
            if(moveTween == null)
                //Log.Debug("moveTween");
                // if(text == null)
                //     Log.Debug("text");
            if(textColTween == null)
               Log.Debug("textColTween");
            if(textStrTween == null)
                Log.Debug("textStrTween");
            
            moveTween?.Kill();
            textColTween?.Kill();
            textStrTween?.Kill();
        }
    }
}
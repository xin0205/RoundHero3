using DG.Tweening;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleMoveValueEntity : Entity
    {
        public BattleMoveValueEntityData BattleMoveValueEntityData { get; protected set; }

        
        [SerializeField] private TextMesh text;
        [SerializeField] private Color hurtColor;
        [SerializeField] private Color recoverColor;

        protected Quaternion cameraQuaternion = Quaternion.identity;
        private Tween textColTween;
        private Tween textStrTween;
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            BattleMoveValueEntityData = userData as BattleMoveValueEntityData;
            if (BattleMoveValueEntityData == null)
            { 
                Log.Error("Error BattleMoveValueEntity");
                return;
            }

            text.text = BattleMoveValueEntityData.Value < 0 ? BattleMoveValueEntityData.Value.ToString() :  "+" + BattleMoveValueEntityData.Value;
            
            //text.text = Mathf.Abs(BattleValueEntityData.Value).ToString();
            //text.color = BattleValueEntityData.Value < 0 ? hurtColor : recoverColor;
            text.color = hurtColor;

            var dis = Vector3.Distance(BattleMoveValueEntityData.TargetPos, transform.position);
            //Constant.Battle.BattleValueVelocity
            var time = dis / Constant.Battle.BattleValueVelocity;

            textStrTween = 
            
            textColTween = DOTween.To(()=> text.color, x => text.color = x, recoverColor, time).SetEase(Ease.InOutQuart);
            var moveTween = transform.DOLocalMove(BattleMoveValueEntityData.TargetPos, time).SetEase(Ease.InOutQuart);
            var absValue = Mathf.Abs(BattleMoveValueEntityData.Value);
            
            textStrTween = DOTween.To(() => text.text, x => text.text = x, "+" + absValue, time)
                    .From("-" + absValue).SetEase(Ease.InOutExpo);
            
            
            if (BattleMoveValueEntityData.IsLoop)
            {
                textColTween.SetLoops(-1);
                textStrTween.SetLoops(-1);
                moveTween.SetLoops(-1);
            }
            else
            {
                moveTween.OnComplete(() =>
                {
                    GameEntry.Entity.HideEntity(this);
                });
            }
            
            // GameUtility.DelayExcute(time / 2f, () =>
            // {
            //     text.text = "+" + Mathf.Abs(BattleMoveValueEntityData.Value);
            // });
        }

        

        private void Update()
        {
            cameraQuaternion.SetLookRotation(Camera.main.transform.forward, Camera.main.transform.up);
            transform.rotation = cameraQuaternion;
            var dis = Mathf.Abs(AreaController.Instance.GetDistanceToPoint(transform.position));
            
            transform.localScale = Vector3.one *  dis / 8f;
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            
            transform.DOKill();
            textColTween.Kill();
            textStrTween.Kill();
            base.OnHide(isShutdown, userData);
        }
    }
}
using DG.Tweening;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleValueEntity : Entity
    {
        public BattleValueEntityData BattleValueEntityData { get; protected set; }

        
        [SerializeField] private TextMesh text;
        [SerializeField] private Color hurtColor;
        [SerializeField] private Color recoverColor;

        protected Quaternion cameraQuaternion = Quaternion.identity;
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            BattleValueEntityData = userData as BattleValueEntityData;
            if (BattleValueEntityData == null)
            { 
                Log.Error("Error BattleValueEntityData");
                return;
            }

            text.text = BattleValueEntityData.Value < 0 ? BattleValueEntityData.Value.ToString() :  "+" + BattleValueEntityData.Value;
            
            //text.text = Mathf.Abs(BattleValueEntityData.Value).ToString();
            //text.color = BattleValueEntityData.Value < 0 ? hurtColor : recoverColor;
            text.color = hurtColor;

            var dis = Vector3.Distance(BattleValueEntityData.TargetPos, transform.position);
            //Constant.Battle.BattleValueVelocity
            var time = 2;
            
            DOTween.To(()=> text.color, x => text.color = x, recoverColor, time).SetEase(Ease.InOutQuart);
            transform.DOMove(BattleValueEntityData.TargetPos, time).SetEase(Ease.InOutQuart).OnComplete(() =>
            {
                GameEntry.Entity.HideEntity(this);
            });
            GameUtility.DelayExcute(time / 2f, () =>
            {
                text.text = "+" + Mathf.Abs(BattleValueEntityData.Value);
            });
        }

        

        private void Update()
        {
            cameraQuaternion.SetLookRotation(Camera.main.transform.forward, Camera.main.transform.up);
            transform.rotation = cameraQuaternion;
            var dis = Mathf.Abs(AreaController.Instance.GetDistanceToPoint(transform.position));
            
            transform.localScale = Vector3.one *  dis / 8f;
        }
    }
}
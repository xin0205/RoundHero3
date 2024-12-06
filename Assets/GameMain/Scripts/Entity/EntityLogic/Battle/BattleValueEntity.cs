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
            text.color = BattleValueEntityData.Value < 0 ? hurtColor : recoverColor;

            transform.DOMove(BattleValueEntityData.TargetPos, 1f);
        }

        

        private void Update()
        {
            cameraQuaternion.SetLookRotation(Camera.main.transform.forward, Camera.main.transform.up);
            
            var dis = Mathf.Abs(AreaController.Instance.GetDistanceToPoint(transform.position));
            
            transform.localScale = Vector3.one *  dis / 8f;
        }
    }
}
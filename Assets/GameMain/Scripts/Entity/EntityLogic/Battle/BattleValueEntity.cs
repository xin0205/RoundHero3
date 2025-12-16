using UnityEngine.UI;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleValueEntity : Entity
    {
        public BattleValueEntityData BattleValueEntityData { get; protected set; }

        
        [SerializeField] private Text text;
        

        protected Quaternion cameraQuaternion = Quaternion.identity;
        protected override void OnShow(object userData)
        {
            transform.SetParent(AreaController.Instance.BattleFormRoot.transform);
            
            base.OnShow(userData);
            
            BattleValueEntityData = userData as BattleValueEntityData;
            if (BattleValueEntityData == null)
            { 
                Log.Error("Error BattleDisplayValueEntityData");
                return;
            }

            var time = 2;
            text.text = BattleValueEntityData.Value.ToString();

            
            //transform.position = BattleValueEntityData.TargetPosition;

        }
        
        public void SetData(Vector2 pos, int value)
        {
            transform.SetParent(AreaController.Instance.BattleFormRoot.transform);
            transform.localScale = Vector3.one; 
            
            text.text = value.ToString();

            //text.color = hurtColor;
            transform.localPosition = pos;
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);

        }

        private void Update()
        {
            // cameraQuaternion.SetLookRotation(Camera.main.transform.forward, Camera.main.transform.up);
            // transform.rotation = cameraQuaternion;
            // var dis = Mathf.Abs(AreaController.Instance.GetDistanceToPoint(transform.position));
            //
            // transform.localScale = Vector3.one *  dis / 8f;
        }
    }
}
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleValueEntity : Entity
    {
        public BattleValueEntityData BattleValueEntityData { get; protected set; }

        
        [SerializeField] private TextMeshPro text;
        

        protected Quaternion cameraQuaternion = Quaternion.identity;
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            BattleValueEntityData = userData as BattleValueEntityData;
            if (BattleValueEntityData == null)
            { 
                Log.Error("Error BattleDisplayValueEntityData");
                return;
            }

            var time = 2;
            text.text = BattleValueEntityData.Value.ToString();

            
            transform.position = BattleValueEntityData.TargetPosition;

        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);

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
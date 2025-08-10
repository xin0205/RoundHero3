

using UnityEngine;
using UnityEngine.UIElements;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleIconEntity : Entity
    {
        public BattleIconEntityData BattleIconEntityData { get; protected set; }
        protected Quaternion cameraQuaternion = Quaternion.identity;

        [SerializeField] private Image icon;
        protected async override void OnShow(object userData)
        {
            transform.SetParent(AreaController.Instance.BattleFormRoot.transform);

            base.OnShow(userData);
            
            BattleIconEntityData = userData as BattleIconEntityData;
            if (BattleIconEntityData == null)
            {
                Log.Error("Error BattleIconEntityData");
                return;
            }
            
            //icon.sprite = await AssetUtility.GetUnitStateIcon(BattleIconEntityData.BattleIconType);

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
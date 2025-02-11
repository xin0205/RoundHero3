

using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleIconEntity : Entity
    {
        public BattleIconEntityData BattleIconEntityData { get; protected set; }
        protected Quaternion cameraQuaternion = Quaternion.identity;
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            BattleIconEntityData = userData as BattleIconEntityData;
            if (BattleIconEntityData == null)
            {
                Log.Error("Error BattleIconEntityData");
                return;
            }

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
using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleHurtEntity : Entity
    {
        public BattleHurtEntityData BattleHurtEntityData { get; protected set; }

        [SerializeField] private Animation animation;
        [SerializeField] private TextMesh text;
        [SerializeField] private Color hurtColor;
        [SerializeField] private Color recoverColor;

        protected Quaternion cameraQuaternion = Quaternion.identity;
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            BattleHurtEntityData = userData as BattleHurtEntityData;
            if (BattleHurtEntityData == null)
            {
                Log.Error("Error BattleHurtEntityData");
                return;
            }

            text.text = BattleHurtEntityData.Hurt < 0 ? BattleHurtEntityData.Hurt.ToString() :  "+" + BattleHurtEntityData.Hurt;
            text.color = BattleHurtEntityData.Hurt < 0 ? hurtColor : recoverColor;
            GameUtility.DelayExcute(animation.clip.length, () =>
            {
                GameEntry.Entity.HideEntity(this);
            });
        }

        

        private void Update()
        {
            cameraQuaternion.SetLookRotation(Camera.main.transform.forward, Camera.main.transform.up);
            animation.transform.rotation = cameraQuaternion;

            var dis = Mathf.Abs(AreaController.Instance.GetDistanceToPoint(animation.transform.position));
            
            animation.transform.localScale = Vector3.one *  dis / 8f;
        }
    }
}
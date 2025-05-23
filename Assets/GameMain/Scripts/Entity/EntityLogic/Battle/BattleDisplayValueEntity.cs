﻿using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleDisplayValueEntity : Entity
    {
        public BattleDisplayValueEntityData BattleDisplayValueEntityData { get; protected set; }

        
        [SerializeField] private Text text;
        [SerializeField] private Color hurtColor;
        [SerializeField] private Color recoverColor;

        protected Quaternion cameraQuaternion = Quaternion.identity;
        protected override void OnShow(object userData)
        {
            transform.SetParent(AreaController.Instance.BattleFormRoot.transform);
            
            base.OnShow(userData);
            
            BattleDisplayValueEntityData = userData as BattleDisplayValueEntityData;
            if (BattleDisplayValueEntityData == null)
            { 
                Log.Error("Error BattleDisplayValueEntityData");
                return;
            }

            

            if (BattleDisplayValueEntityData.Value == 0)
            {
                GameEntry.Entity.HideEntity(this);
                return;
            }
            
           
            //transform.GetComponent<RectTransform>().anchoredPosition = BattleDisplayValueEntityData.Position;

            var time = 2;
            text.text = BattleDisplayValueEntityData.Value < 0 ? BattleDisplayValueEntityData.Value.ToString() :  "+" + BattleDisplayValueEntityData.Value;

            text.color = BattleDisplayValueEntityData.Value < 0 ? hurtColor : recoverColor;
            //transform.position = BattleDisplayValueEntityData.TargetPosition;
            //transform.DOLocalMove(BattleDisplayValueEntityData.TargetPosition, time).SetEase(Ease.InOutQuart).SetLoops(-1);
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            transform.DOKill();
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
using System;
using GameFramework;
using RoundHero;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class ConfirmFormParams
    {
        public string Message { get; set; }
        public bool IsShowCancel { get; set; } = true;
        

        public Action OnConfirm
        {
            get;
            set;
        }  
         
        public Action OnClose
        {
            get;
            set;
        }
    }
    
    public class ConfirmForm : UGuiForm
    {
        [SerializeField] private Text message;
        private ConfirmFormParams confirmFormParams;
        [SerializeField] private GameObject cancelButtonGO;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            confirmFormParams = userData as ConfirmFormParams;

            message.text = confirmFormParams.Message;

            cancelButtonGO.SetActive(confirmFormParams.IsShowCancel);

        }
        public void OnConfirmButtonClick()
        {
            Close();

            if (confirmFormParams.OnConfirm != null)
            {
                confirmFormParams.OnConfirm();
            }
        }
         
        public void OnCloseButtonClick()
        {
            Close();

            if (confirmFormParams.OnClose != null)
            {
                confirmFormParams.OnClose();
            }
        }

    }
}
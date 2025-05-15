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

        public string ConfirmStr { get; set; }
        public string CancelStr { get; set; }


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
        
        [SerializeField] private Text confirmStr;
        [SerializeField] private Text cancelStr;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            confirmFormParams = userData as ConfirmFormParams;

            message.text = confirmFormParams.Message;

            if (!string.IsNullOrEmpty(confirmFormParams.ConfirmStr))
            {
                confirmStr.text = confirmFormParams.ConfirmStr;
            }
            
            if (!string.IsNullOrEmpty(confirmFormParams.CancelStr))
            {
                cancelStr.text = confirmFormParams.CancelStr;
            }
            
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
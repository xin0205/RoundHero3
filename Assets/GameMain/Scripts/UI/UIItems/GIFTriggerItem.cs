using System;
using UGFExtensions.Await;
using UnityEngine;

namespace RoundHero
{
    public class GIFTriggerItem : MonoBehaviour
    {
        [SerializeField]
        private GIFFormData gifFormData;

        private GIFForm gifForm;
        
        private bool isOpen = false;
        
        public async void OnPointerEnter()
        {
            if(isOpen)
                return;

            isOpen = true;
            var formAsync = await GameEntry.UI.OpenUIFormAsync(UIFormId.GIFForm, gifFormData);
            gifForm = formAsync.Logic as GIFForm;
        }

        public void OnPointerExit()
        {
            if(!isOpen)
                return;

            isOpen = false;
            if (gifForm == null)
                return;
                
            
            GameEntry.UI.CloseUIForm(gifForm);
            gifForm = null;
        }

        private void Update()
        {
            if (!isOpen && gifForm != null)
            {
                GameEntry.UI.CloseUIForm(gifForm);
                gifForm = null;
            }
        }
    }
}
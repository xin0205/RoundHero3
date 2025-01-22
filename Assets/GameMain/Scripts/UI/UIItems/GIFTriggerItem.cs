using System;
using UGFExtensions.Await;
using UnityEngine;

namespace RoundHero
{
    public enum EShowPosition
    {
        BattleLeft,
        MousePosition,
    }
    
    public class GIFTriggerItem : MonoBehaviour
    {
        [SerializeField]
        public GIFFormData GifFormData;

        private GIFForm gifForm;
        
        private bool isOpen = false;
        
        public async void OnPointerEnter()
        {
            if(isOpen)
                return;

            isOpen = true;
            var formAsync = await GameEntry.UI.OpenUIFormAsync(UIFormId.GIFForm, GifFormData);
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
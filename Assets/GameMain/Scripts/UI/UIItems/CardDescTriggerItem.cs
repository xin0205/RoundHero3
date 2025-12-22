using UGFExtensions.Await;
using UnityEngine;

namespace RoundHero
{
    public class CardDescTriggerItem : MonoBehaviour
    {
        [SerializeField]
        public CardDescFormData CardDescFormData;

        private CardDescForm cardDescForm;
        
        private bool isOpen = false;
        private bool isClose = true;

        private void OnDisable()
        {
            CloseForm();
        }

        public async void OnPointerEnter()
        {
            if (cardDescForm != null)
            {
                CloseForm();
            }
            
            if (!isClose)
                return;
                

            isOpen = true;
            isClose = false;
            var formAsync = await GameEntry.UI.OpenUIFormAsync(UIFormId.CardDescForm, CardDescFormData);
            if (formAsync != null)
            {
                cardDescForm = formAsync.Logic as CardDescForm;
                
            }

        }

        public void OnPointerExit()
        {
            // if(!isOpen)
            //     return;

            // isOpen = false;
            // if (cardDescForm == null)
            //     return;

            isOpen = false;
            CloseForm();
        }

        public void Update()
        {
            
            if (cardDescForm != null && !isOpen)
            {
                isOpen = false;
                CloseForm();
                //battleUnitEntity = null;
                //
            }
        }

        public void CloseForm()
        {
            // isOpen = false;
            // if (cardDescForm == null)
            //     return;
            //
            // var form = GameEntry.UI.GetUIForm(cardDescForm.UIForm.SerialId);
            // cardDescForm = null;
            // if(form == null)
            //     return;
            //
            // GameEntry.UI.CloseUIForm(form);
            
            if (cardDescForm != null)
            {
                isClose = true;
                GameEntry.UI.CloseUIForm(cardDescForm);
                cardDescForm = null;
            }
            
        }
    }
}
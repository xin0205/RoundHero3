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
                

            isOpen = true;
            
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

            isOpen = false;
            if (cardDescForm == null)
                return;


            CloseForm();
        }

        public void Update()
        {
            
            if (cardDescForm != null && !isOpen)
            {
                CloseForm();
                //battleUnitEntity = null;
                //isOpen = false;
            }
        }

        public void CloseForm()
        {
            isOpen = false;
            if (cardDescForm == null)
                return;
            
            var form = GameEntry.UI.GetUIForm(cardDescForm.UIForm.SerialId);
            cardDescForm = null;
            if(form == null)
                return;
            
            GameEntry.UI.CloseUIForm(form);
            
        }
    }
}
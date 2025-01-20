using UGFExtensions.Await;
using UnityEngine;

namespace RoundHero
{
    public class UnitDescTriggerItem : MonoBehaviour
    {
        [SerializeField]
        public UnitDescFormData UnitDescFormData;

        private UnitDescForm unitDescForm;
        
        private bool isOpen = false;
        
        public async void OnPointerEnter()
        {
            if(isOpen)
                return;

            isOpen = true;
            var formAsync = await GameEntry.UI.OpenUIFormAsync(UIFormId.UnitDescForm, UnitDescFormData);
            unitDescForm = formAsync.Logic as UnitDescForm;
        }

        public void OnPointerExit()
        {
            if(!isOpen)
                return;

            isOpen = false;
            if (unitDescForm == null)
                return;
                
            
            GameEntry.UI.CloseUIForm(unitDescForm);
            unitDescForm = null;
        }

        private void Update()
        {
            if (!isOpen && unitDescForm != null)
            {
                GameEntry.UI.CloseUIForm(unitDescForm);
                unitDescForm = null;
            }
        }
    }
}
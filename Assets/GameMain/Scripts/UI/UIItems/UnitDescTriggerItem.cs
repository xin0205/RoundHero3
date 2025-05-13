using UGFExtensions.Await;
using UnityEngine;
using UnityGameFramework.Runtime;

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
            if (formAsync != null)
            {
                unitDescForm = formAsync.Logic as UnitDescForm;
            }

        }

        public void OnPointerExit()
        {
            if(!isOpen)
                return;

            isOpen = false;
            if (unitDescForm == null)
                return;


            CloseForm();
        }

        private void Update()
        {
            
            if (unitDescForm != null && !isOpen)
            {
                CloseForm();
                //battleUnitEntity = null;
                //isOpen = false;
            }
        }

        public void CloseForm()
        {
            if (unitDescForm == null)
                return;

            if(GameEntry.UI.GetUIForm(unitDescForm.UIForm.SerialId) == null)
                return;
            
            GameEntry.UI.CloseUIForm(unitDescForm);
            unitDescForm = null;
        }
    }
}
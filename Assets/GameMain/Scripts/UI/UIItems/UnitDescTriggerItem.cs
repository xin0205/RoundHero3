using System;
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

        private void OnDisable()
        {
            CloseForm();
        }

        public async void OnPointerEnter()
        {
            if (unitDescForm != null)
            {
                CloseForm();
            }
                

            isOpen = true;
            
            var formAsync = await GameEntry.UI.OpenUIFormAsync(UIFormId.UnitDescForm, UnitDescFormData);
            if (formAsync != null)
            {
                unitDescForm = formAsync.Logic as UnitDescForm;
            }

            if (BattleAreaManager.Instance.CurPointGridPosIdx == -1)
            {
                CloseForm();
            }

            // if (!BattleManager.Instance.IsOpenUnitDescForm)
            // {
            //     BattleManager.Instance.CloseForm();
            // }

        }

        public void OnPointerExit()
        {
            if(!isOpen)
                return;

            
            if (unitDescForm == null)
                return;


            CloseForm();
        }

        public void Update()
        {
            
            if (unitDescForm != null && BattleAreaManager.Instance.CurPointGridPosIdx == -1)
            {
                CloseForm();
                //battleUnitEntity = null;
                //isOpen = false;
            }
        }

        public void CloseForm()
        {
            isOpen = false;
            if (unitDescForm == null)
                return;


            var form = GameEntry.UI.GetUIForm(unitDescForm.UIForm.SerialId);
            unitDescForm = null;
            if(form == null)
                return;
            
            
            GameEntry.UI.CloseUIForm(form);
            
        }


    }
}
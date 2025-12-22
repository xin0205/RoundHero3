using UGFExtensions.Await;
using UnityEngine;

namespace RoundHero
{
    public class ExplainTriggerItem : MonoBehaviour
    {
        [SerializeField]
        public ExplainData ExplainData;

        private ExplainForm explainForm;
        
        private bool isOpen = false;
        
        public async void OnPointerEnter()
        {
            if(isOpen)
                return;

            isOpen = true;
            if (ExplainData != null)
            {
                var formAsync = await GameEntry.UI.OpenUIFormAsync(UIFormId.ExplainForm, ExplainData);
                explainForm = formAsync?.Logic as ExplainForm;
            }
            
        }

        public void OnPointerExit()
        {

            if(!isOpen)
                return;

            isOpen = false;
            if (explainForm == null)
                return;
                
            
            GameEntry.UI.CloseUIForm(explainForm);
            explainForm = null;
        }

        private void Update()
        {
            if (!isOpen && explainForm != null)
            {
                GameEntry.UI.CloseUIForm(explainForm);
                explainForm = null;
            }
        }
    }
}
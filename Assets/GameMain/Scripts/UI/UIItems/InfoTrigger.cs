using UnityEngine;
using UnityEngine.Events;

namespace RoundHero
{
    public class InfoTrigger : MonoBehaviour
    {
        [SerializeField] private string name;
        [SerializeField] private string desc;
        
        private bool isShowInfo = false;
        
        private InfoForm infoForm;

        
        public UnityEvent<InfoFormParams> infoParams; 
        
        private void Update()
        {
            if (infoForm != null && !isShowInfo)
            {
                GameEntry.UI.CloseUIForm(infoForm);
                infoForm = null;
            }
        }

        public async void ShowInfo()
        {
            isShowInfo = true;

            var infoFormParams = new InfoFormParams()
            {
                Name = string.IsNullOrEmpty(name) ? "" : GameEntry.Localization.GetString(name),
                Desc = string.IsNullOrEmpty(desc) ? "" : GameEntry.Localization.GetString(desc),
                Position = this.transform.position + new Vector3(0.5f, -0.5f, 0),
            };
            
            if (infoParams != null)
            {
                infoParams.Invoke(infoFormParams);
            }

            var uiForm = await GameEntry.UI.OpenInfoFormAsync(infoFormParams);
            
            infoForm = uiForm.Logic as InfoForm;
        }

        
        
        
        public void HideInfo()
        {
            isShowInfo = false;
            if (infoForm != null && !isShowInfo)
            {
                GameEntry.UI.CloseUIForm(infoForm);
                infoForm = null;
            }
        }


    }
}
using UnityEngine;
using UnityEngine.Events;
using UnityGameFramework.Runtime;


namespace RoundHero
{
    public class InfoTrigger : MonoBehaviour
    {
        [SerializeField] private string name;
        [SerializeField] private string desc;
        
        [SerializeField] private Vector2 infoDelta = new Vector2(1f, 1f);
        
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

            // var mousePosition = Vector2.zero;
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(AreaController.Instance.Canvas.transform as RectTransform,
            //         Input.mousePosition, AreaController.Instance.UICamera, out mousePosition);
            //
            var infoFormParams = new InfoFormParams()
            {
                Name = string.IsNullOrEmpty(name) ? "" : GameEntry.Localization.GetString(name),
                Desc = string.IsNullOrEmpty(desc) ? "" : GameEntry.Localization.GetString(desc),
                //Position = mousePosition + infoDelta,
            };
            
            
            
            if (infoParams != null)
            {
                infoParams.Invoke(infoFormParams);
            }

            var uiForm = await GameEntry.UI.OpenInfoFormAsync(infoFormParams);
            
            infoForm = uiForm.Logic as InfoForm;
        }

        // public async void ShowInfoInWorld()
        // {
        //     isShowInfo = true;
        //
        //     var infoFormParams = new InfoFormParams()
        //     {
        //         Name = string.IsNullOrEmpty(name) ? "" : GameEntry.Localization.GetString(name),
        //         Desc = string.IsNullOrEmpty(desc) ? "" : GameEntry.Localization.GetString(desc),
        //         Position = AreaController.Instance.UICamera.WorldToScreenPoint(this.transform.position) + (Vector3)infoDelta,
        //     };
        //     
        //     if (infoParams != null)
        //     {
        //         infoParams.Invoke(infoFormParams);
        //     }
        //
        //     var uiForm = await GameEntry.UI.OpenInfoFormAsync(infoFormParams);
        //     
        //     infoForm = uiForm.Logic as InfoForm;
        // }
        
        
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
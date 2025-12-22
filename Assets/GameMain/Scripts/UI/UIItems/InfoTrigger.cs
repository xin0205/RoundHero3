 using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityGameFramework.Runtime;


namespace RoundHero
{
    public class InfoTrigger : MonoBehaviour
    {
        [SerializeField] private string name;
        [SerializeField] private string desc;
        
        [SerializeField] private Vector2 infoDelta = new Vector2(0.5f, 0.5f);
        
        private bool isShow = false;
        private bool isClose = true;
        
        private InfoForm infoForm;

        private List<string> descParams = new List<string>();
        
        public UnityEvent<InfoFormParams> infoParams; 
        
        private void Update()
        {
            // || BattleCardManager.Instance.SelectCardIdx == -1
            if (infoForm != null && !isShow)
            {
                HideInfo();
            }
        }

        public void SetDescParams(List<string> paramList)
        {
            descParams = paramList;

        }
        
        public void SetNameDesc(string name, string desc)
        {
            this.name = name;
            this.desc = desc;

        }


        public async void ShowInfo()
        {
            
            if(string.IsNullOrEmpty(name) && string.IsNullOrEmpty(desc))
                return;
            
            if (infoForm != null)
            {
                CloseForm();
            }
            
            if(!isClose)
                return;
            
            isShow = true;
            isClose = false;
            // var mousePosition = Vector2.zero;
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(AreaController.Instance.Canvas.transform as RectTransform,
            //         Input.mousePosition, AreaController.Instance.UICamera, out mousePosition);
            //
            
            var infoFormParams = new InfoFormParams()
            {
                Name = string.IsNullOrEmpty(name) ? "" : GameEntry.Localization.GetString(name),
                Desc = string.IsNullOrEmpty(desc) ? "" : GameEntry.Localization.GetLocalizedStrings(desc, descParams),
                //Position = mousePosition + infoDelta,
            };
            
            
            
            if (infoParams != null)
            {
                infoParams.Invoke(infoFormParams);
            }
            
            

            var uiForm = await GameEntry.UI.OpenInfoFormAsync(infoFormParams);
            if (uiForm != null)
            {
                infoForm = uiForm.Logic as InfoForm;
                
            }


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
            isShow = false;
            // && isShowInfo
            CloseForm();
        }
        
        public void CloseForm()
        {

            if (infoForm != null)
            {
                
                isClose = true;
                GameEntry.UI.CloseUIForm(infoForm);
                infoForm = null;
            }
        }

        private void OnDisable()
        {
            
            HideInfo();
        }
    }
}
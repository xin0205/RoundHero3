using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class BlessIconItem : MonoBehaviour
    {
        [SerializeField] 
        private Image BlessIcon;
        
        [SerializeField] 
        private Text Value;

        private int blessID = -1;

        private bool isShowInfo = false;

        private InfoForm infoForm;
        
        public void Init()
        {
            
        }
        
        public async void SetItemData(Data_Bless blessData, int itemIndex,int row,int column)
        {
            if (blessID != blessData.BlessID)
            {
                blessID = blessData.BlessID;

                BlessIcon.sprite = await AssetUtility.GetBlessIcon(blessID);
                
            }

            var drBless = GameEntry.DataTable.GetBless(blessID);
            var deltaValue = BattleBuffManager.Instance.GetBuffValue(drBless.Values1[0]) - blessData.Value;
            Value.gameObject.SetActive(deltaValue != 0);
            if (deltaValue != 0)
            {
                Value.text = deltaValue.ToString();
            }

            
            
        }

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
            var blessName = "";
            var blessDesc = "";
            GameUtility.GetBlessText(blessID, ref blessName, ref blessDesc);
            
            var uiForm = await GameEntry.UI.OpenInfoFormAsync(new InfoFormParams()
            {
                Name = blessName,
                Desc = blessDesc,
                Position = this.transform.position + new Vector3(0.5f, -0.5f, 0),
            });
            
            infoForm = uiForm.Logic as InfoForm;
        }

        public void HideInfo()
        {
            isShowInfo = false;

        }
    }
}
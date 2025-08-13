using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class HPDeltaDataItem : MonoBehaviour
    {
        private HPDeltaData hpDeltaData;

        [SerializeField] private Image icon;
        [SerializeField] private Text text;
        [SerializeField] private Text value;


        
        public void Init()
        {
            
        }
    
        public async void SetUI()
        {
            text.text = "【" + GameEntry.Localization.GetString(Constant.Localization.HPDeltaOwners[hpDeltaData.HPDeltaOwnerType]) + "】";
            value.text = hpDeltaData.HPDelta > 0 ? "+" + hpDeltaData.HPDelta : hpDeltaData.HPDelta.ToString();

            switch (hpDeltaData)
            {
                case BlessHPDeltaData blessHpDeltaData:
                    icon.sprite = await AssetUtility.GetBlessIcon(blessHpDeltaData.BlessIdx);
                    break;
                case CardHPDeltaData cardHpDeltaData:
                    break;
                case EnemyHPDeltaData enemyHpDeltaData:
                    break;
                case FuneHPDeltaData funeHpDeltaData:
                    icon.sprite = await AssetUtility.GetFuneIcon(funeHpDeltaData.FuneIdx);
                    break;
                case SoliderHPDeltaData soliderHpDeltaData:
                    break;
                default:
                    break;
            }
            
        }
        
        public void SetItemData(HPDeltaData hpDeltaData, int itemIndex,int row,int column)
        {
            this.hpDeltaData = hpDeltaData;

            SetUI();

        }
    }
    
    
}
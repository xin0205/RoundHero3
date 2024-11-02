using System;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class AcquireFormData
    {
        public EItemType ItemType;
        public int ItemID;
        public string Value;
        public Action ClickCloseAction;
    }
        
    public class AcquireForm : UGuiForm
    {
        private AcquireFormData acquireFormData;

        [SerializeField] private CardItem cardItem;
        
        [SerializeField] private CommonIconItem commonIconItem;
        [SerializeField] private Text text;
        [SerializeField] private GameObject confirmGO;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            acquireFormData = (AcquireFormData)userData;
            if (acquireFormData == null)
            {
                Log.Warning("AcquireFormData is null.");
                return;
            }

            ShowValueText();
            cardItem.gameObject.SetActive(acquireFormData.ItemType == EItemType.Card);
            commonIconItem.gameObject.SetActive(acquireFormData.ItemType != EItemType.Card);
            confirmGO.SetActive(acquireFormData.ClickCloseAction != null);
            text.text = acquireFormData.Value;
            
            switch (acquireFormData.ItemType)
            {
                case EItemType.Card:
                    cardItem.SetCard(acquireFormData.ItemID);
                    break;
                case EItemType.Bless:
                case EItemType.Fune:
                case EItemType.HP:
                case EItemType.Heart:
                case EItemType.Coin:
                    commonIconItem.SetIcon(acquireFormData.ItemType, acquireFormData.ItemID);
                    break;
                default:
                    break;
            }

            ShowItem();
        }

        public void ShowItem()
        {
            if (acquireFormData.ItemType == EItemType.Card)
            {
                cardItem.GetComponent<Animation>().Play();
                
            }
            else
            {
                commonIconItem.GetComponent<Animation>().Play();
                
            }
            
            

            if (acquireFormData.ClickCloseAction == null)
            {
                GameUtility.DelayExcute(4f, () =>
                {
                    Close();
                });
            }
            
        }

        public void ShowValueText()
        {
            text.gameObject.SetActive(Constant.Hero.AttributeItemTypes.Contains(acquireFormData.ItemType));

            
        }

        public void OnClickClose()
        {
            Close();
            acquireFormData.ClickCloseAction?.Invoke();
        }
        
        
    }
}
﻿using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class CommonIconItem : MonoBehaviour
    {
        [SerializeField] private Image icon;

        private EItemType itemType;
        private int itemID;
        
        public async void SetIcon(EItemType itemType, int itemID)
        {
            this.itemType = itemType;
            this.itemID = itemID;
            
            switch (itemType)
            {
                case EItemType.Bless:
                    icon.sprite = await AssetUtility.GetBlessIcon(itemID);
                    break;
                case EItemType.Fune:
                    icon.sprite = await AssetUtility.GetFuneIcon(itemID);
                    break;
                case EItemType.Coin:
                case EItemType.HP:
                case EItemType.Heart:
                    icon.sprite = await AssetUtility.GetCommonIcon(itemType);
                    break;
                default:
                    break;
            }
        }
    }
}
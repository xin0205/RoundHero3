using UnityEngine;
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
                case EItemType.UnitState:
                    icon.sprite = await AssetUtility.GetUnitStateIcon((EUnitState)itemID);
                    break;
                case EItemType.AddMaxHP:
                case EItemType.AddCardFuneSlot:
                case EItemType.RemoveCard:
                    icon.sprite = await AssetUtility.GetCommonIcon(itemType);
                    break;
                default:
                    break;
            }
        }
    }
}
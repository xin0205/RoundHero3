using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public enum EItemType
    {
        Card,
        Bless,
        Fune,
    }
    
    public class ItemData
    {
        public EItemType ItemType { get; set; }
        
        public int FuneID { get; set; }
        public int CardID { get; set; }
        public EBlessID BlessID { get; set; }
    }

    public class StoreItemData
    {
        public ItemData ItemData;
        public int Price { get; set; }
        public int StoreIdx { get;set; }
        public bool IsSale { get; set; }
        
    }
    
    public class CommonStoreItem : MonoBehaviour
    {
        [SerializeField] private Text title;
        [SerializeField] private Text desc;
        [SerializeField] private CoinItem coinItem;
        [SerializeField] private Image icon;
        private StoreItemData storeItemData;

        public void Init()
        {
            
        }
        
        public void SetItemData(StoreItemData storeItemData)
        {
            this.storeItemData = storeItemData;
            Refresh();
        }


        public async void Refresh()
        {
            var name = "";
            var desc = "";
            switch (storeItemData.ItemData.ItemType)
            {
                // case EItemType.Card:
                //     GameUtility.GetCardText(storeItemData.ItemData.CardID, ref name, ref desc);
                //     break;
                case EItemType.Bless:
                    GameUtility.GetItemText(storeItemData.ItemData.BlessID, ref name, ref desc);
                    icon.sprite = await AssetUtility.GetBlessIcon(storeItemData.ItemData.BlessID);
                    break;
                case EItemType.Fune:
                    GameUtility.GetItemText(storeItemData.ItemData.FuneID, ref name, ref desc);
                    icon.sprite = await AssetUtility.GetFuneIcon(storeItemData.ItemData.FuneID);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            coinItem.SetPrice(storeItemData.Price);
            title.text = name; 
            this.desc.text = desc;
            
        }

    }
}
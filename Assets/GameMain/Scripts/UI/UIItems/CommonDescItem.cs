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
        Coin,
        HP,
        Heart,
    }
    
    public class CommonItemData
    {
        public EItemType ItemType { get; set; }
        
        public int ItemID { get; set; }
        // public int CardID { get; set; }
        // public int BlessID { get; set; }
    }
    
    public class CommonDescItem : MonoBehaviour
    {
        [SerializeField] private Text title;
        [SerializeField] private Text desc;
        [SerializeField] private CommonIconItem commonIconItem;
        
        private CommonItemData commonItemData;
        
        public void Init()
        {
            
        }
        
        public void SetItemData(CommonItemData commonItemData)
        {
            this.commonItemData = commonItemData;
            
            Refresh();
        }


        public async void Refresh()
        {
            var name = "";
            var desc = "";
            GameUtility.GetItemText(commonItemData.ItemType, commonItemData.ItemID, ref name, ref desc);
   
            title.text = name; 
            this.desc.text = desc;

            commonIconItem.SetIcon(commonItemData.ItemType, commonItemData.ItemID);
            
        }

    }
}
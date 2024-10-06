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
        
        public EBuffID BuffID { get; set; }
        public int CardID { get; set; }
        public EBlessID BlessID { get; set; }
    }

    public class StoreItemData
    {
        public ItemData ItemData;
        public int Price { get; set; }
        
    }
    
    public class CommonStoreItem : MonoBehaviour
    {
        [SerializeField] private Text title;
        [SerializeField] private Text desc;
        [SerializeField] private CoinItem coinItem;
        [SerializeField] private Image icon;
        private StoreItemData storeItemData;  

        public void SetItemData(StoreItemData storeItemData)
        {
            this.storeItemData = storeItemData;
            
            var name = "";
            var desc = "";
            switch (storeItemData.ItemData.ItemType)
            {
                
            }
            GameUtility.GetCardText(card.CardID, ref name, ref desc);

            title.text = name;
            desc.text = desc;

            Refresh();
        }


        public void Refresh()
        {
            
            
        }

    }
}
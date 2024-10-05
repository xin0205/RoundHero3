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
    
    
    public class StoreItemData
    {
        public int Price { get; set; }
        public EItemType ItemType { get; set; }
        public int ItemID { get; set; }
    }
    
    public class StoreCardItem : MonoBehaviour
    {

        private StoreItemData StoreItemData;
        
        [SerializeField] private BaseCard BaseCard;

        [SerializeField] private Text CardPrice;

        public void Init()
        {
            
        }
        
        public void SetItemData(StoreItemData storeItemData)
        {
            this.StoreItemData = storeItemData;
            

            Refresh();
        }


        public void Refresh()
        {
            BaseCard.SetCardUI(this.StoreItemData.ItemID);
            CardPrice.text = StoreItemData.Price.ToString();
            
        }
        
    }
}
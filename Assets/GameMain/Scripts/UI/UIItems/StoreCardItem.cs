using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class StoreCardItem : MonoBehaviour
    {

        private StoreItemData storeItemData;
        
        [SerializeField] private BaseCard BaseCard;

        [SerializeField] private Text CardPrice;

        public void Init()
        {
            
        }
        
        public void SetItemData(StoreItemData storeItemData)
        {
            this.storeItemData = storeItemData;
            

            Refresh();
        }


        public void Refresh()
        {
            BaseCard.SetCardUI(this.storeItemData.ItemData.CardID);
            CardPrice.text = storeItemData.Price.ToString();
            
        }
        
    }
}
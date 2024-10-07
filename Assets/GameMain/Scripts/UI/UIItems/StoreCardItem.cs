using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class StoreCardItem : MonoBehaviour
    {

        private StoreItemData storeItemData;
        
        [SerializeField] private BaseCard BaseCard;

        [SerializeField] private Text CardPrice;
        
        private Action<int, int> purchaseAction;

        public void Init()
        {
            
        }
        
        public void SetItemData(StoreItemData storeItemData, Action<int, int> purchaseAction)
        {
            this.storeItemData = storeItemData;
            this.purchaseAction = purchaseAction;

            Refresh();
        }


        public void Refresh()
        {
            BaseCard.SetCardUI(this.storeItemData.ItemData.CardID);
            CardPrice.text = storeItemData.Price.ToString();
            
        }

        public void Purchase()
        {
            this.purchaseAction.Invoke(storeItemData.StoreIdx, storeItemData.Price);
        }
        
    }
}
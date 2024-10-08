using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RoundHero
{
    public class StoreCardItem : MonoBehaviour
    {
        private StoreItemData storeItemData;
        
        [SerializeField] private BaseCard baseCard;

        [SerializeField] private Text cardPrice;
        
        [SerializeField] private GameObject mask;
        
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
            baseCard.SetCardUI(this.storeItemData.CommonItemData.CardID);
            cardPrice.text = storeItemData.Price.ToString();
            mask.SetActive(storeItemData.IsSaleOut);
        }

        public void Purchase()
        {
            if(storeItemData.IsSaleOut)
                return;
            
            this.purchaseAction.Invoke(storeItemData.StoreIdx, storeItemData.Price);
        }
        
    }
}
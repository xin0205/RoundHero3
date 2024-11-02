using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RoundHero
{
    

    public class StoreItemData
    {
        public CommonItemData CommonItemData;
        public int Price { get; set; }
        public int StoreIdx { get;set; }
        public bool IsSaleOut { get; set; }
        
    }
    
    public class CommonStoreItem : MonoBehaviour
    {
        [FormerlySerializedAs("commonItem")] [SerializeField] private CommonDescItem commonDescItem;
        [SerializeField] private CoinItem coinItem;
        [SerializeField] private GameObject mask;
        
        private StoreItemData storeItemData;
        
        private Action<int, int> purchaseAction;

        public void Init()
        {
            
        }
        
        public void SetItemData(StoreItemData storeItemData, Action<int, int> purchaseAction)
        {
            this.storeItemData = storeItemData;
            this.purchaseAction = purchaseAction;
            commonDescItem.SetItemData(storeItemData.CommonItemData);
            
            Refresh();
        }


        public async void Refresh()
        {
            commonDescItem.Refresh();
            coinItem.SetPrice(storeItemData.Price);
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
using System;
using UnityEngine;

namespace RoundHero
{
    public class SelectAcquireItemData
    {
        public EItemType ItemType;
        public int ItemID;
    }
    
    public class SelectAcquireItem : MonoBehaviour
    {
        [SerializeField] private CardItem cardItem;
        [SerializeField] private CommonDescItem commonDescItem;
        
        private SelectAcquireItemData selectAcquireItemData;

        public Action<int> onClickAction;

        private int itemIdx;
        
        public void Init()
        {
            
        }
        
        public void SetItemData(SelectAcquireItemData selectAcquireItemData, Action<int> onClick, int itemIndex)
        {
            this.selectAcquireItemData = selectAcquireItemData;
            this.onClickAction = onClick;
            this.itemIdx = itemIdx;
            
            cardItem.gameObject.SetActive(selectAcquireItemData.ItemType == EItemType.Card);
            commonDescItem.gameObject.SetActive(selectAcquireItemData.ItemType != EItemType.Card);
            
            if (selectAcquireItemData.ItemType == EItemType.Card)
            {
                cardItem.SetCard(selectAcquireItemData.ItemID);
            }
            else if (Constant.Hero.CommonItemTypes.Contains(selectAcquireItemData.ItemType))
            {
                commonDescItem.SetItemData(new CommonItemData()
                {
                    ItemType = this.selectAcquireItemData.ItemType,
                    ItemID = this.selectAcquireItemData.ItemID,
                });
                
            }
            
            Refresh();
        }
        
        public void Refresh()
        {
            
            
            if (selectAcquireItemData.ItemType == EItemType.Card)
            {
                cardItem.Refresh();
            }
            else if (Constant.Hero.CommonItemTypes.Contains(selectAcquireItemData.ItemType))
            {
                commonDescItem.Refresh();
            }
            
        }

        public void OnClick()
        {
            var idx = 0;
            switch (selectAcquireItemData.ItemType)
            {
                case EItemType.Card:
                    idx = CardManager.Instance.GetIdx();
                    CardManager.Instance.CardDatas.Add(idx, new Data_Card(idx, selectAcquireItemData.ItemID));
                    break;
                case EItemType.Bless:
                    idx = BlessManager.Instance.GetIdx();
                    var drBless = GameEntry.DataTable.GetBless(selectAcquireItemData.ItemID);
                    
                    BlessManager.Instance.BlessDatas.Add(idx, new Data_Bless(idx, drBless.BlessID));
                    break;
                case EItemType.Fune:
                    idx = FuneManager.Instance.GetIdx();
                    FuneManager.Instance.FuneDatas.Add(idx, new Data_Fune(idx, selectAcquireItemData.ItemID));
                    BattlePlayerManager.Instance.PlayerData.UnusedFuneIdxs.Add(idx);
                    break;
                
                default:
                    break;
            }
            
            onClickAction?.Invoke(itemIdx);

        }
    }
}
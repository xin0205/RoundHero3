using System;

using UnityEngine;
using UnityEngine.EventSystems;

namespace RoundHero
{
    public class SelectAcquireItemData
    {
        public EItemType ItemType;
        public int ItemID;
        public bool IsSelected;
    }
    
    public class SelectAcquireItem : MonoBehaviour
    {
        [SerializeField] private CardItem cardItem;
        [SerializeField] private CommonDescItem commonDescItem;
        //[SerializeField] private GameObject selectIcon;
        [SerializeField] private ScaleGameObject scaleGameObject;
        [SerializeField] private GameObject selectGameObject;
        private SelectAcquireItemData selectAcquireItemData;

        public Action<int> onClickAction;

        private int itemIdx;
        
        private bool isSelect = false;

        private void Awake()
        {
            
        }

        private void OnEnable()
        {
            isSelect = false;
            selectGameObject.SetActive(false);
        }
        

        public void Init()
        {
            
        }

        public void SetSelect()
        {
            isSelect = !isSelect;
            selectGameObject.SetActive(isSelect);
            scaleGameObject.Scale(Vector3.one, Vector3.one, 0.1f);
            //selectIcon.SetActive(isSelect);
        }
        
        public void SetItemData(SelectAcquireItemData selectAcquireItemData, Action<int> onClick, int itemIndex)
        {
            this.selectAcquireItemData = selectAcquireItemData;
            this.onClickAction = onClick;
            this.itemIdx = itemIndex;
            
            cardItem.gameObject.SetActive(selectAcquireItemData.ItemType == EItemType.TacticCard || selectAcquireItemData.ItemType == EItemType.UnitCard);
            commonDescItem.gameObject.SetActive(selectAcquireItemData.ItemType != EItemType.TacticCard && selectAcquireItemData.ItemType != EItemType.UnitCard);
            
            if (selectAcquireItemData.ItemType == EItemType.TacticCard || selectAcquireItemData.ItemType == EItemType.UnitCard)
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
            
            
            if (selectAcquireItemData.ItemType == EItemType.TacticCard || selectAcquireItemData.ItemType == EItemType.UnitCard)
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
            if(isSelect)
                return;
            
            
            this.onClickAction.Invoke(this.itemIdx);


        }

        public void PointerOnEnter(BaseEventData baseEventData)
        {
            if(isSelect)
                return;
            
            scaleGameObject.Scale(Vector3.one, Vector3.one * 1.2f, 0.1f);
        }
        
        public void PointerOnExit(BaseEventData baseEventData)
        {
            if(isSelect)
                return;
            
            scaleGameObject.Scale(Vector3.one * 1.2f, Vector3.one, 0.1f);
        }
    }
}
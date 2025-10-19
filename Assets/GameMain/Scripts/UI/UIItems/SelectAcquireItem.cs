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

        public SelectAcquireItemData Copy()
        {
            var selectAcquireItemData = new SelectAcquireItemData();
            selectAcquireItemData.ItemType = ItemType;
            selectAcquireItemData.ItemID = ItemID;
            selectAcquireItemData.IsSelected = IsSelected;

            return selectAcquireItemData;
        }
    }
    
    public class SelectAcquireItem : MonoBehaviour
    {
        [SerializeField] private CardItem cardItem;
        [SerializeField] private CommonDescItem commonDescItem;
        //[SerializeField] private GameObject selectIcon;
        [SerializeField] private ScaleGameObject scaleGameObject;
        [SerializeField] private GameObject selectGameObject;
        private SelectAcquireItemData selectAcquireItemData;
        
        [SerializeField] private ExplainTriggerItem explainTriggerItem;

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
        
        public void SetSelect(bool isSelect)
        {
            this.isSelect = isSelect;
            selectGameObject.SetActive(this.isSelect);

        }
        
        public void SetItemData(SelectAcquireItemData selectAcquireItemData, Action<int> onClick, int itemIndex)
        {
            this.selectAcquireItemData = selectAcquireItemData;
            this.onClickAction = onClick;
            this.itemIdx = itemIndex;
            SetSelect(selectAcquireItemData.IsSelected);
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
            
            explainTriggerItem.ExplainData = new ExplainData()
            {
                ItemType = selectAcquireItemData.ItemType,
                ItemID = selectAcquireItemData.ItemID,
                ShowPosition = EShowPosition.MousePosition,
            };
            
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

        private int siblingIndex;
        public void PointerOnEnter(BaseEventData baseEventData)
        {
            if(isSelect)
                return;

            siblingIndex = scaleGameObject.transform.GetSiblingIndex();
            scaleGameObject.transform.SetSiblingIndex(99);
            scaleGameObject.Scale(Vector3.one, Vector3.one * 1.2f, 0.1f);
        }
        
        public void PointerOnExit(BaseEventData baseEventData)
        {
            if(isSelect)
                return;
            
            scaleGameObject.transform.SetSiblingIndex(siblingIndex);
            scaleGameObject.Scale(Vector3.one * 1.2f, Vector3.one, 0.1f);
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


namespace RoundHero
{
    public class SelectCardItem : MonoBehaviour
    {
        [SerializeField] private CardItem CardItem;

        //[SerializeField] private GameObject SelectFrame;

        [SerializeField] private GameObject useTag;
        [SerializeField] private GameObject unUseTag;
        [SerializeField] private GameObject tag;

        private bool isUse;
        
        private int cardID = -1;
        
        public Action<int> ClickAction;
        
        public void Init()
        {
            
        }
        
        public void SetItemData(int cardID, int itemIndex,int row,int column)
        {

            if (this.cardID != cardID)
            {
                this.cardID = cardID;
                CardItem.SetCard(cardID);
                
            }
            isUse = GameManager.Instance.TmpInitCards.Contains(cardID);
            if (!isUse)
            {
                this.useTag.SetActive(false);
                tag.SetActive(false);
            }
        }
        
        
        public void OnClick()
        {
            ClickAction.Invoke(cardID);
            SetUseTag(GameManager.Instance.TmpInitCards.Contains(cardID));
        }

        public void SetUseTag(bool isUse)
        {
            this.isUse = isUse;
            RefreshUseTag();
            
        }

        public void OnPointerEnter()
        {
            RefreshUseTag();
            if (!isUse)
            {
                this.useTag.SetActive(true);
                tag.SetActive(true);
            }
        }

        private void RefreshUseTag()
        {
            this.useTag.SetActive(!isUse);
            this.unUseTag.SetActive(isUse);
            tag.SetActive(true);
        }
        
        public void OnPointerExit()
        {
            tag.SetActive(isUse);
        }
    }
}
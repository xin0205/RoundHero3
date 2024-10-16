using System;
using UnityEngine;


namespace RoundHero
{
    public class SelectCardItem : MonoBehaviour
    {
        [SerializeField] private CardItem CardItem;

        //[SerializeField] private GameObject SelectFrame;

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
                CardItem.SetCardUI(cardID);
            }
            
        }
        
        
        public void OnClick()
        {
            ClickAction.Invoke(cardID);
        }
    }
}
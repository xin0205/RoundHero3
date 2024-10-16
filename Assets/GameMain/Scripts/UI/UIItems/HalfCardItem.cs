using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class HalfCardItem : MonoBehaviour
    {
        [SerializeField]
        private Text cardName;


        [SerializeField] private Image Icon;
        
        private int CardID = -1;

        public Action<int> ClickAction;

        
        public void SetCardUI(int cardID)
        {
            CardID = cardID;
            RefreshCardUI();
        }

        public async void RefreshCardUI()
        {
            var drCard = GameEntry.DataTable.GetCard(CardID);
            if (drCard.CardType == ECardType.Unit)
            {
                Icon.sprite = await AssetUtility.GetFollowerIcon(CardID);
            }
            else if (drCard.CardType == ECardType.Tactic)
            {
                Icon.sprite = await AssetUtility.GetTacticIcon(CardID);
            }
        }
        
        public void Init()
        {
            
        }
        
        public void SetItemData(int cardID, int itemIndex,int row,int column)
        {
            if (CardID != cardID)
            {
                CardID = cardID;
                SetCardUI(cardID);
            }
            
        }

        public void OnClick()
        {
            ClickAction.Invoke(CardID);
        }
    }
}
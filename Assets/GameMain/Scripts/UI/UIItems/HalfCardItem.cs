using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class HalfCardItem : MonoBehaviour
    {
        [SerializeField]
        private Text cardName;


        [SerializeField] private Image UnitIcon;
        [SerializeField] private Image TacticIcon;
        
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
            
            UnitIcon.gameObject.SetActive(drCard.CardType == ECardType.Unit);
            TacticIcon.gameObject.SetActive(drCard.CardType == ECardType.Tactic);

            if (drCard.CardType == ECardType.Unit)
            {
                UnitIcon.sprite = await AssetUtility.GetFollowerIcon(CardID);
            }
            else if (drCard.CardType == ECardType.Tactic)
            {
                TacticIcon.sprite = await AssetUtility.GetTacticIcon(CardID);
            }
            
            var cardName = "";
            var cardDesc = "";
            GameUtility.GetCardText(CardID, ref cardName, ref cardDesc);

            this.cardName.text = cardName;

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
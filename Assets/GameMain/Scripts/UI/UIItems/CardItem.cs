using UnityEngine;

namespace RoundHero
{
    public class CardItem : MonoBehaviour
    {
        
        private BaseCard baseCard;
        
        [SerializeField]
        private BaseCard unitCard;
        
        [SerializeField]
        private BaseCard tacticCard;
        
        [SerializeField]
        private BaseCard stateCard;
        
        private int CardID = -1;
        private int CardIdx = -1;
        
        public void SetCard(int cardID, int cardIdx = -1)
        {

            CardID = cardID;
            CardIdx = cardIdx;
            var drCard = GameEntry.DataTable.GetCard(CardID);
            if (drCard.CardType == ECardType.Unit)
            {
                baseCard = unitCard;
            }
            else if (drCard.CardType == ECardType.Tactic)
            {
                baseCard = tacticCard;
            }
            else if (drCard.CardType == ECardType.Prop)
            {
                baseCard = tacticCard;
            }
            else if (drCard.CardType == ECardType.State)
            {
                baseCard = stateCard;
            }
            
            unitCard.gameObject.SetActive(drCard.CardType == ECardType.Unit);
            tacticCard.gameObject.SetActive(drCard.CardType == ECardType.Tactic || drCard.CardType == ECardType.Prop);
            stateCard.gameObject.SetActive(drCard.CardType == ECardType.State);

            baseCard.SetCardUI(CardID, CardIdx);
        }

        public void Refresh()
        {
            baseCard.RefreshCardUI();
        }
        
        public void RefreshCardUI(string cardName, string cardDesc, int cardID)
        {
            baseCard.RefreshCardUI(cardName, cardDesc, cardID);
        }
        
        public void RefreshEnergy(int energy)
        {
            baseCard.RefreshEnergy(energy);
        }

        public void SetIconVisible(bool isVisible)
        {
            baseCard.SetIconVisible(isVisible);
            
        }
    }
}
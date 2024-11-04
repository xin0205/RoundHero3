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

        
        public void SetCard(int cardID)
        {

            CardID = cardID;
            var drCard = GameEntry.DataTable.GetCard(CardID);
            if (drCard.CardType == ECardType.Unit)
            {
                baseCard = unitCard;
            }
            else if (drCard.CardType == ECardType.Tactic)
            {
                baseCard = tacticCard;
            }
            else if (drCard.CardType == ECardType.State)
            {
                baseCard = stateCard;
            }
            
            unitCard.gameObject.SetActive(drCard.CardType == ECardType.Unit);
            tacticCard.gameObject.SetActive(drCard.CardType == ECardType.Tactic);
            stateCard.gameObject.SetActive(drCard.CardType == ECardType.State);
            
            baseCard.SetCardUI(CardID);
        }

        public void Refresh()
        {
            baseCard.RefreshCardUI();
        }
    }
}
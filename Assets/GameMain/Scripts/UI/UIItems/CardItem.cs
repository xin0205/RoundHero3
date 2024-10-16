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
        
        private int CardID = -1;

        
        public void SetCardUI(int cardID)
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
            
            unitCard.gameObject.SetActive(drCard.CardType == ECardType.Unit);
            tacticCard.gameObject.SetActive(drCard.CardType == ECardType.Tactic);

            baseCard.SetCardUI(CardID);
        }

        public void RefreshCardUI()
        {
            baseCard.RefreshCardUI();
        }
    }
}
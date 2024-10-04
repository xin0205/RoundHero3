
using System.Collections.Generic;

using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    
    public class CardsForm : UGuiForm
    {

        [SerializeField]
        private CardsView cardsViews;
        
        private List<int> cardIdxs = new List<int>();

        private ECardType cardType = ECardType.Unit;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

        }

        public void SelectUnit(bool isSelect)
        {
            if (isSelect)
            {
                SelectCardType(ECardType.Unit);
            }
            
        }
        
        public void SelectTactic(bool isSelect)
        {
            if (isSelect)
            {
                SelectCardType(ECardType.Tactic);
            }
            
        }

        public void SelectCardType(ECardType cardType)
        {
            
            foreach (var kv in CardManager.Instance.CardDatas)
            {
                var drCard = CardManager.Instance.GetCardTable(kv.Key);
                if (drCard.CardType == cardType)
                {
                    cardIdxs.Add(drCard.Id);
                }
                
            }
            
            cardsViews.Init(cardIdxs);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            
        }

    }
}

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    
    public class CardsForm : UGuiForm
    {

        [SerializeField]
        private CardsView cardsViews;

        [SerializeField] private Toggle unitToggle;
        
        private List<int> cardIdxs = new List<int>();

        private ECardType cardType = ECardType.Unit;
        
        [SerializeField]
        private GameObject cardGO;
        
        [SerializeField]
        private GameObject funeGO;
        
        [SerializeField]
        private GameObject funeTag;

        [SerializeField]
        private Toggle switchViewToggle;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            unitToggle.isOn = false;
            unitToggle.isOn = true;

            switchViewToggle.isOn = false;
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
            cardIdxs.Clear();
            foreach (var kv in CardManager.Instance.CardDatas)
            {
                var drCard = CardManager.Instance.GetCardTable(kv.Key);
                if (drCard.CardType == cardType)
                {
                    cardIdxs.Add(kv.Key);
                }
                
            }
            
            cardsViews.Init(cardIdxs);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            
        }

        public void SwitchView(bool isOn)
        {
            cardGO.SetActive(!isOn);
            funeGO.SetActive(isOn);
            funeTag.SetActive(isOn);
        }
        
    }
}
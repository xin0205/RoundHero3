using System;
using TMPro;
using UnityEngine;

namespace RoundHero
{
    
    
    public class CardItem : MonoBehaviour
    {
        
        private CardShowData _cardShowData;
        
        [SerializeField] private BaseCard BaseCard;

        public void Init()
        {
            
        }
        
        public void SetItemData(CardShowData cardShowData)
        {
            this._cardShowData = cardShowData;
            

            Refresh();
        }


        public void Refresh()
        {
            BaseCard.SetCardUI(this._cardShowData.CardID);

            
        }
        
    }
}
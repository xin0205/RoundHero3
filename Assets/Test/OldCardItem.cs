using System;
using TMPro;
using UnityEngine;

namespace RoundHero
{
    
    
    public class OldCardItem : MonoBehaviour
    {
        [SerializeField] private GameObject select;

        [SerializeField]
        private TextMeshProUGUI useTips;
        
        [SerializeField]
        private GameObject mask;
        
        private OldCardShowData _cardShowData;

        private Action<int> selectAction;

        [SerializeField] private BaseCard BaseCard;

        public void Init()
        {
            
        }
        
        public void SetItemData(OldCardShowData cardShowData, Action<int> selectAction)
        {
            this._cardShowData = cardShowData;
            
            this.selectAction = selectAction;
            
            
            
            Refresh();
        }

        public void Select()
        {
            selectAction?.Invoke(_cardShowData.CardID);

        }

        public void Refresh()
        {
            BaseCard.SetCardUI(this._cardShowData.CardID);
            select.SetActive(this._cardShowData.Select);
            
            
        }
        
    }
}
using System;
using System.Collections.Generic;
using GameFramework.Event;

using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{

    public class CardsFormParams
    {
        public string Tips;
        public List<ECardType> ShowCardTypes = new List<ECardType>();
        public Action<int> OnClickAction;
        public Action OnCloseAction;
        public bool IsShowAllFune;

    }
    
    public class CardsForm : UGuiForm
    {
        [SerializeField]
        public CardsView CardsViews;
        
        [SerializeField]
        private CardTypeToggleDictionary toggles;
        [SerializeField]
        private Text tips;
        
        private CardsFormParams cardsFormParams;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            cardsFormParams = (CardsFormParams)userData;
            if (cardsFormParams == null)
            {
                Log.Warning("CardsFormParams is null.");
                return;
            }
            
            foreach (var kv in toggles)
            {
                kv.Value.gameObject.SetActive(false);
            }

            foreach (var cardType in cardsFormParams.ShowCardTypes)
            {
                toggles[cardType].gameObject.SetActive(true);
            }
            
            
            
            tips.text = cardsFormParams.Tips;
            
            CardsViews.Init(OnClick, cardsFormParams.IsShowAllFune);
            if (cardsFormParams.ShowCardTypes.Count > 0)
            {
                var cardType = cardsFormParams.ShowCardTypes[0];
                toggles[cardType].isOn = false;
                toggles[cardType].isOn = true;
            }

            GameEntry.Event.Subscribe(RefreshCardsFormEventArgs.EventId, OnRefreshCardsForm);
        }

        public void OnClick(int cardIdx)
        {
            cardsFormParams.OnClickAction?.Invoke(cardIdx);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            GameEntry.Event.Unsubscribe(RefreshCardsFormEventArgs.EventId, OnRefreshCardsForm);
            
        }

        public void ConfirmClose()
        {
            if (cardsFormParams.OnCloseAction != null)
            {
                cardsFormParams.OnCloseAction.Invoke();
            }
            else
            {
                Close();
            }
        }
        
        public void OnRefreshCardsForm(object sender, GameEventArgs e)
        {
            RefreshView();
        }

        public void RefreshView()
        {

            CardsViews.Refresh();
        }
    }
}
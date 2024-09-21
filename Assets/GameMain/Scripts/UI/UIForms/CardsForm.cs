using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{

    public class CardsFormData
    {
        public List<int> Cards;
        public Action<List<int>> SelectAction;
        public Action CloseAction;
        public int SelectCount;
        public string Tips;
    }

    public class CardsForm : UGuiForm
    {
        private CardsFormData cardsFormData;

        [SerializeField]
        private CardsView cardsViews;
        
        [SerializeField]
        private TextMeshProUGUI tips;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            cardsFormData = (CardsFormData)userData;
            if (cardsFormData == null)
            {
                Log.Warning("CardsFormData is null.");
                return;
            }

            tips.text = cardsFormData.Tips;
            cardsViews.Init(cardsFormData);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            cardsFormData.CloseAction?.Invoke();
        }

    }
}
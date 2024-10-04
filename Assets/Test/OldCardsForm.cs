using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;

namespace RoundHero
{

    public class OldCardsFormData
    {
        public List<int> Cards;
        public Action<List<int>> SelectAction;
        public Action CloseAction;
        public int SelectCount;
        public string Tips;
    }

    public class OldCardsForm : UGuiForm
    {
        private OldCardsFormData cardsFormData;

        [FormerlySerializedAs("cardsViews")] [SerializeField]
        private OldCardsView oldCardsViews;
        
        [SerializeField]
        private TextMeshProUGUI tips;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            cardsFormData = (OldCardsFormData)userData;
            if (cardsFormData == null)
            {
                Log.Warning("CardsFormData is null.");
                return;
            }

            tips.text = cardsFormData.Tips;
            oldCardsViews.Init(cardsFormData);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            cardsFormData.CloseAction?.Invoke();
        }

    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;

namespace RoundHero
{
    public class OldCardShowData
    {
        public int CardID;
        public bool Select;
    }

    public class OldCardsView : MonoBehaviour
    {
        [SerializeField]
        private LoopGridView cardView;

        private Dictionary<int, int>  cardDict = new Dictionary<int, int>();
        private List<OldCardShowData> cards = new List<OldCardShowData>();

        private List<int> selectCards = new List<int>();
        private OldCardsFormData cardsFormData;
        [SerializeField]
        private GameObject confirmGO;

        private void Awake()
        {
            cardView.InitGridView(0, OnGetCardItemByRowColumn);
        }

        public void Init(OldCardsFormData cardsFormData)
        {
            this.cardsFormData = cardsFormData;
            this.cards.Clear();
            this.cardDict.Clear();
            var idx = 0;
            foreach (var card in this.cardsFormData.Cards)
            {
                this.cards.Add(new OldCardShowData()
                {
                    CardID = card,
                    Select = false,
                });
                cardDict.Add(card, idx++);
            }

            selectCards.Clear();
            
            cardView.SetListItemCount(this.cards.Count);
            cardView.RefreshAllShownItem();
            confirmGO.SetActive(this.cardsFormData.SelectAction != null);
        }
        
        LoopGridViewItem OnGetCardItemByRowColumn(LoopGridView view, int itemIndex,int row,int column)
        {
            if (itemIndex < 0)
            {
                return null;
            }
            
            var item = view.NewListViewItem("CardItem");
        
            var itemScript = item.GetComponent<OldCardItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
        
            itemScript.SetItemData(cards[itemIndex], SelectCard);
            
            return item;
        }
        
        public void SelectCard(int cardID)
        {
            if (selectCards.Contains(cardID))
            {
                cards[cardDict[cardID]].Select = false;
                selectCards.Remove(cardID);
            }
            else
            {
                if (selectCards.Count >= this.cardsFormData.SelectCount && selectCards.Count > 0)
                {
                    cards[cardDict[selectCards[0]]].Select = false;
                    selectCards.RemoveAt(0);
                    
                }

                if (this.cardsFormData.SelectCount > 0)
                {
                    selectCards.Add(cardID);
                    cards[cardDict[cardID]].Select = true;
                }
               
                
            }

            cardView.RefreshAllShownItem();

        }

        public void Confirm()
        {
            this.cardsFormData.SelectAction?.Invoke(selectCards);
        }
    }
}
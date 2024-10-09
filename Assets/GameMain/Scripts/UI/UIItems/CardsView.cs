                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;

namespace RoundHero
{
    public class PlayerCardData
    {
        public int CardIdx;
        public int CardID;
    }

    public class CardsView : MonoBehaviour
    {
        [SerializeField]
        private LoopGridView cardView;

        private Dictionary<int, int>  cardDict = new Dictionary<int, int>();
        private List<PlayerCardData> cards = new List<PlayerCardData>();

        //private List<int> cardIdxs = new List<int>();


        private void Awake()
        {
            cardView.InitGridView(0, OnGetCardItemByRowColumn);
        }

        public void Init(List<int> cardIdxs)
        {
            //this.cardIdxs = cardIdxs;
            this.cards.Clear();
            this.cardDict.Clear();
            var idx = 0;
            foreach (var cardIdx in cardIdxs)
            {
                var drCard = CardManager.Instance.GetCardTable(cardIdx);
                this.cards.Add(new PlayerCardData()
                {
                    CardIdx = cardIdx,
                    CardID = drCard.Id,
                });
                cardDict.Add(cardIdx, idx++);
            }

            cardView.SetListItemCount(this.cards.Count);
            cardView.RefreshAllShownItem();

        }
        
        LoopGridViewItem OnGetCardItemByRowColumn(LoopGridView view, int itemIndex,int row,int column)
        {
            if (itemIndex < 0)
            {
                return null;
            }
            
            var item = view.NewListViewItem("PlayerCardItem");
        
            var itemScript = item.GetComponent<PlayerCardItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            
            
        
            itemScript.SetItemData(cards[itemIndex]);
            
            return item;
        }


    }
}
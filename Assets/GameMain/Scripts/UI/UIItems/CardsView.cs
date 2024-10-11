                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;
using UnityGameFramework.Runtime;

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
        private GameObject parentForm;
        
        public int CurSelectCardIdx;

        private void Awake()
        {
            cardView.InitGridView(0, OnGetCardItemByRowColumn);
        }

        public void Init(List<int> cardIdxs, GameObject parentForm)
        {
            this.parentForm = parentForm;
            
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
        
        public void Refresh()
        {
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

            itemScript.SetItemData(cards[itemIndex], OnPointEnter, OnPointExit);
            
            return item;
        }

        public void OnPointEnter(int cardIdx)
        {
            CurSelectCardIdx = cardIdx;
        }
        
        public void OnPointExit()
        {
            CurSelectCardIdx = -1;
        }
        
        public void OnDrop(int cardIdx)
        {
 
            
        }
    }
}
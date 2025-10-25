                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
using System;
using System.Collections.Generic;
using SuperScrollView;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
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
        
        private List<int> cardIdxs = new List<int>();

        private ECardType cardType = ECardType.Unit;

        //private List<int> cardIdxs = new List<int>();
        //private GameObject parentForm;
        
        public int CurSelectCardIdx = -1;
        
        [SerializeField] private Toggle allToggle;

        private Action<int> onClickAction;
        private bool isShowAllFune;

        private void Awake()
        {
            cardView.InitGridView(0,  OnGetCardItemByRowColumn);
            
        }

        private void OnEnable()
        {
            allToggle.isOn = false;
            allToggle.isOn = true;
        }

        public void Init(Action<int> onClickAction, bool isShowAllFune)
        {
            this.isShowAllFune = isShowAllFune;
            this.onClickAction = onClickAction;
        }

        public void Init(List<int> cardIdxs)
        {
            //this.parentForm = parentForm;
            
            //this.cardIdxs = cardIdxs;
            this.cards.Clear();
            this.cardDict.Clear();
            var idx = 0;
            foreach (var cardIdx in cardIdxs)
            {
                if(!CardManager.Instance.CardDatas.ContainsKey(cardIdx))
                    continue;
                
                var cardData = CardManager.Instance.CardDatas[cardIdx];
                var drCard = GameEntry.DataTable.GetCard(cardData.CardID);
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
                itemScript.Init(OnPointEnter, OnPointExit, OnClick);
            }

            itemScript.SetItemData(cards[itemIndex], isShowAllFune);
            
            return item;
        }
        
        public void OnClick(int cardIdx)
        {
            onClickAction?.Invoke(cardIdx);
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
        
        public void SelectAll(bool isSelect)
        {
            if (isSelect)
            {
                SelectCardType(new List<ECardType>(){ECardType.Unit, ECardType.Tactic});
            }
            
        }
        
        public void SelectUnit(bool isSelect)
        {
            if (isSelect)
            {
                SelectCardType(new List<ECardType>(){ECardType.Unit});
            }
            
        }
        
        public void SelectTactic(bool isSelect)
        {
            if (isSelect)
            {
                SelectCardType(new List<ECardType>(){ECardType.Tactic});
            }
            
        }
        
        public void SelectState(bool isSelect)
        {
            if (isSelect)
            {
                SelectCardType(new List<ECardType>(){ECardType.State});
            }
            
        }

        public void SelectCardType(List<ECardType> cardTypes)
        {
            cardIdxs.Clear();
            foreach (var kv in CardManager.Instance.CardDatas)
            {
                var drCard = GameEntry.DataTable.GetCard(kv.Value.CardID);
                if (cardTypes.Contains(drCard.CardType))
                {
                    cardIdxs.Add(kv.Key);
                }
                
            }
            
            Init(cardIdxs);
        }
    }
}
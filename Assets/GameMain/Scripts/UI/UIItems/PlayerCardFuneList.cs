using System;
using System.Collections.Generic;
using GameFramework.Event;
using UnityEngine;

namespace RoundHero
{
    public class PlayerCardFuneList : MonoBehaviour
    {
        [SerializeField] private List<PlayerFuneItem> PlayerFuneItems = new List<PlayerFuneItem>();
        
        [SerializeField] private GameObject funeListGO;

        private int CardIdx;
        [SerializeField] public bool IsShowFuneDownTag = false;


        private void OnEnable()
        {
            GameEntry.Event.Subscribe(UnEquipFuneEventArgs.EventId, OnUnEquipFune);
            
            var items = funeListGO.GetComponentsInChildren<PlayerFuneItem>(true);
            PlayerFuneItems.Clear();
            PlayerFuneItems.AddRange(items);
        }

        private void OnDisable()
        {
            GameEntry.Event.Unsubscribe(UnEquipFuneEventArgs.EventId, OnUnEquipFune);
        }
        
        public void OnUnEquipFune(object sender, GameEventArgs e)
        {
            Refresh();
        }

        public void Init(int cardIdx, bool isShowAll)
        {
            CardIdx = cardIdx;
            ShowAllFune(isShowAll);
            Refresh();
        }
        
        public void ShowAllFune(bool isShowAll)
        {
            var cardData = CardManager.Instance.GetCard(CardIdx);
            var drCard = CardManager.Instance.GetCardTable(CardIdx);
            var idx = 0;
            foreach (var playerFuneItem in PlayerFuneItems)
            {
                playerFuneItem.gameObject.SetActive(false);
                
                if(idx >= cardData.MaxFuneCount)
                    continue;
                idx++;

                playerFuneItem.gameObject.SetActive(isShowAll && drCard.CardType == ECardType.Unit);
            }
            
        }
        
        public async void Refresh()
        {
            
            var cardData = CardManager.Instance.GetCard(CardIdx);

            // foreach (var playerFuneItem in PlayerFuneItems)
            // {
            //     playerFuneItem.gameObject.SetActive(false);
            // }
            for (int i = 0; i < cardData.MaxFuneCount; i++)
            {
                PlayerFuneItems[i].gameObject.SetActive(true);
                PlayerFuneItems[i].SetFune(CardIdx, -1);
            }
            
            var idx = 0;
            foreach (var funeIdx in cardData.FuneIdxs)
            {
                var isTmpEquipFune = GameManager.Instance.CardsForm_EquipFuneIdxs.Contains(funeIdx);
 
                //var funeData = FuneManager.Instance.GetFuneData(funeIdx);
                PlayerFuneItems[idx].gameObject.SetActive(true);
                PlayerFuneItems[idx].SetFune(CardIdx, funeIdx);
                PlayerFuneItems[idx].ShowUnEquip(isTmpEquipFune && IsShowFuneDownTag);
                idx++;
            }


        }
        
        
    }
}
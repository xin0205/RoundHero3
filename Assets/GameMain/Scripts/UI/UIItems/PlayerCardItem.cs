using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RoundHero
{
    
    public class PlayerCardItem : MonoBehaviour
    {
        
        private PlayerCardData playerCardData;
        
        [SerializeField] private CardItem CardItem;

        [SerializeField] private List<Image> FuneIcons = new List<Image>();
        
        [SerializeField] private List<GameObject> FuneGOs = new List<GameObject>();
        
        [SerializeField] private List<GameObject> FuneDownGOs = new List<GameObject>();

        public Action<int> OnPointEnterAction;
        public Action OnPointExitAction;
        public Action<int> OnClickAction;
        
        public Action<int> OnDropAction;

        public bool isShowFuneDownTag = false;
        
        public void Init(Action<int> onPointEnterAction, Action onPointExitAction, Action<int> onClickAction)
        {
            OnPointEnterAction = onPointEnterAction;
            OnPointExitAction = onPointExitAction;
            OnClickAction = onClickAction;
        }
        
        public void SetItemData(PlayerCardData playerCardData, bool idShowAllFune)
        {
            
            this.playerCardData = playerCardData;
            
            //OnDropAction = onPointUpAction;
            
            CardItem.SetCard(this.playerCardData.CardID);
            ShowAllFune(idShowAllFune);
            Refresh();
            
        }

        public void ShowAllFune(bool isShowAll)
        {
            var cardData = CardManager.Instance.GetCard(this.playerCardData.CardIdx);
            var drCard = CardManager.Instance.GetCardTable(playerCardData.CardIdx);
            var idx = 0;
            foreach (var funeGO in FuneGOs)
            {
                funeGO.SetActive(false);
                
                if(idx >= cardData.MaxFuneCount)
                    continue;
                idx++;

                funeGO.SetActive(isShowAll && drCard.CardType == ECardType.Unit);
            }
            
        }


        public async void Refresh()
        {
            
            var cardData = CardManager.Instance.GetCard(this.playerCardData.CardIdx);

            foreach (var funeIcon in FuneIcons)
            {
                funeIcon.gameObject.SetActive(false);
            }
            
            var idx = 0;
            foreach (var funeIdx in cardData.FuneIdxs)
            {
                var isTmpEquipFune = GameManager.Instance.CardsForm_EquipFuneIdxs.Contains(funeIdx);
 
                var funeData = FuneManager.Instance.GetFuneData(funeIdx);
                FuneIcons[idx].gameObject.SetActive(true);
                FuneIcons[idx].overrideSprite = await AssetUtility.GetFuneIcon(funeData.FuneID);
                FuneGOs[idx].SetActive(true);
                FuneDownGOs[idx].SetActive(isTmpEquipFune && isShowFuneDownTag);
                idx++;
            }
            

        }
        
        public void OnPointEnter()
        {
            var drCard = CardManager.Instance.GetCardTable(this.playerCardData.CardIdx);
            if(drCard.CardType != ECardType.Unit)
                return;
            
            OnPointEnterAction?.Invoke(playerCardData.CardIdx);
            

        }

        public void OnClick()
        {
            OnClickAction.Invoke(playerCardData.CardIdx);
        }
        
        public void OnPointExit()
        {
            var drCard = CardManager.Instance.GetCardTable(this.playerCardData.CardIdx);
            if(drCard.CardType != ECardType.Unit)
                return;
            
            OnPointExitAction?.Invoke();
            

        }
        
        public void OnPointUp()
        {
            

            
        }
        
        public void OnDrop()
        {
            OnDropAction?.Invoke(playerCardData.CardIdx);
        }

        public void DownFune(int funeOrder)
        {
            var cardData = CardManager.Instance.GetCard(this.playerCardData.CardIdx);
            var funeIdx = cardData.FuneIdxs[funeOrder];
            if(!GameManager.Instance.CardsForm_EquipFuneIdxs.Contains(funeIdx))
                return;
            
            
            BattlePlayerManager.Instance.PlayerData.UnusedFuneIdxs.Add(funeIdx);
            GameManager.Instance.CardsForm_EquipFuneIdxs.Remove(funeIdx);
            cardData.FuneIdxs.RemoveAt(funeOrder);
            FuneDownGOs[funeOrder].SetActive(false);
            GameEntry.Event.Fire(null, RefreshCardsFormEventArgs.Create());
        }
        
    }
}
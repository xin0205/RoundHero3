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
        
        public Action<int> OnDropAction;

        public bool isShowFuneDownTag = false;
        
        public void Init()
        {
            
        }
        
        public void SetItemData(PlayerCardData playerCardData, Action<int> onPointEnterAction, Action onPointExitAction)
        {
            
            this.playerCardData = playerCardData;
            OnPointEnterAction = onPointEnterAction;
            OnPointExitAction = onPointExitAction;
            //OnDropAction = onPointUpAction;
            
            CardItem.SetCardUI(this.playerCardData.CardID);
            Refresh();
            
        }


        public async void Refresh()
        {
            
            var cardData = CardManager.Instance.GetCard(this.playerCardData.CardIdx);

            foreach (var funeGO in FuneGOs)
            {
                funeGO.SetActive(false);
            }
            
            var idx = 0;
            foreach (var funeIdx in cardData.FuneIdxs)
            {
                var isTmpEquipFune = GameManager.Instance.CardsForm_EquipFuneIdxs.Contains(funeIdx);
 
                var funeData = FuneManager.Instance.GetFuneData(funeIdx);
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
            GameEntry.Event.Fire(null, RefreshCardsFormEventArgs.Create());
        }
        
    }
}
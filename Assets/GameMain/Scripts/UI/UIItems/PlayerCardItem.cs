using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    
    public class PlayerCardItem : MonoBehaviour
    {
        
        private PlayerCardData playerCardData;
        
        [SerializeField] private CardItem CardItem;

        [SerializeField] private List<Image> FuneIcons = new List<Image>();
        
        [SerializeField] private List<GameObject> FuneGOs = new List<GameObject>();

        public Action<int> OnPointEnterAction;
        public Action OnPointExitAction;
        
        public Action<int> OnDropAction;
        
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
                var funeData = FuneManager.Instance.GetFuneData(funeIdx);
                FuneIcons[idx].overrideSprite = await AssetUtility.GetFuneIcon(funeData.FuneID);
                FuneGOs[idx].SetActive(true);
                idx++;
            }
            

        }
        
        public void OnPointEnter()
        {
            OnPointEnterAction?.Invoke(playerCardData.CardIdx);
            

        }
        
        public void OnPointExit()
        {
            OnPointExitAction?.Invoke();
            

        }
        
        public void OnPointUp()
        {
            

            
        }
        
        public void OnDrop()
        {
            OnDropAction?.Invoke(playerCardData.CardIdx);
        }
    }
}
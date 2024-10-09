using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    
    public class PlayerCardItem : MonoBehaviour
    {
        
        private PlayerCardData playerCardData;
        
        [SerializeField] private BaseCard BaseCard;

        [SerializeField] private List<Image> FuneIcons = new List<Image>();
        
        [SerializeField] private List<GameObject> FuneGOs = new List<GameObject>();

        public void Init()
        {
            
        }
        
        public void SetItemData(PlayerCardData playerCardData)
        {
            this.playerCardData = playerCardData;
            

            Refresh();
        }


        public async void Refresh()
        {
            BaseCard.SetCardUI(this.playerCardData.CardID);
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
        
    }
}
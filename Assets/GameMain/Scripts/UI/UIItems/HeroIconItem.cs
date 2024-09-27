using GameFramework;
using GameFramework.Resource;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class HeroIconItem : MonoBehaviour
    {
        [SerializeField] private Image HeroIcon;
        private EHeroID heroID = EHeroID.Empty;
        [SerializeField] 
        private GameObject SelectGO;
        
        public void Init()
        {
            
        }
        
        public async void SetItemData(DRHero drHero, int itemIndex,int row,int column)
        {
            if (heroID != drHero.HeroID)
            {
                heroID = drHero.HeroID;

                HeroIcon.sprite = await AssetUtility.GetHeroIcon(drHero.Id);
                
            }
            
            
            SelectGO.SetActive(GameManager.Instance.StartSelect_HeroID == heroID);
        }


        public void OnClick()
        {
            GameEntry.Event.Fire(null, StartSelect_SelectHeroEventArgs.Create(heroID));
        }

    }
}
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
        private int heroID = -1;
        [SerializeField] 
        private GameObject SelectGO;
        
        public void Init()
        {
            
        }
        
        public async void SetItemData(DRHero drHero, int itemIndex,int row,int column)
        {
            if (heroID != drHero.Id)
            {
                heroID = drHero.Id;

                HeroIcon.sprite = await AssetUtility.GetHeroIcon(drHero.Id);
                
            }
            
            
            SelectGO.SetActive(GameManager.Instance.StartSelect_HeroID == drHero.Id);
        }


        public void OnClick()
        {
            GameEntry.Event.Fire(null, StartSelect_SelectHeroEventArgs.Create(heroID));
        }

    }
}
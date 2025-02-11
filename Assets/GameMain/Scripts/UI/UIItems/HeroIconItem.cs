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
            
            
            SelectGO.SetActive(GameManager.Instance.TmpHeroID == drHero.Id);
        }


        public void OnSelectHero()
        {
            GameManager.Instance.TmpHeroID = heroID;
            GameEntry.Event.Fire(null, StartSelect_SelectHeroEventArgs.Create(heroID));
        }
        
        public void OneEnterHeroIcon()
        {
            GameEntry.Event.Fire(null, StartSelect_SelectHeroEventArgs.Create(heroID));
        }
        
        public void OnExitHeroIcon()
        {
            if (heroID == GameManager.Instance.TmpHeroID)
                return;
            
            GameEntry.Event.Fire(null, StartSelect_SelectHeroEventArgs.Create(GameManager.Instance.TmpHeroID));
        }

    }
}
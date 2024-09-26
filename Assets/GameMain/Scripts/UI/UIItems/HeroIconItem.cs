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
        private EHeroID heroID;
        [SerializeField] 
        private GameObject SelectGO;
        
        public void Init()
        {
            
        }
        
        public async void SetItemData(DRHero drHero, int itemIndex,int row,int column)
        {
            heroID = drHero.HeroID;
            

            
            var sprite = await AssetUtility.GetHeroIcon(drHero.Id);
            if (sprite != null)
            {
                HeroIcon.sprite = sprite;
            }
            
            SelectGO.SetActive(GameManager.Instance.StartSelect_HeroID == heroID);
        }


        public void OnClick()
        {
            GameManager.Instance.StartSelect_HeroID = heroID;
            SelectGO.SetActive(true);
            GameEntry.Event.Fire(null, StartSelect_SelectHeroEventArgs.Create(heroID));
        }

    }
}
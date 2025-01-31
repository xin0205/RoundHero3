using UnityEngine;

namespace RoundHero
{
    public class HeroSceneEntity : Entity
    {
        public DisplayHeroEntity DisplayHeroEntity;

        [SerializeField] private Transform heroPos;
        
        [SerializeField] private GameObject lightGO;
        

        public async void ShowDisplayHeroEntity(int heroID)
        {
            if (DisplayHeroEntity != null)
            {
                GameEntry.Entity.HideEntity(DisplayHeroEntity);
                DisplayHeroEntity = null;
            }
            
            DisplayHeroEntity = await GameEntry.Entity.ShowDisplayHeroEntityAsync(heroID);
            DisplayHeroEntity.transform.position = heroPos.position;
        }
        
        public void ShowLight(bool isOn)
        {
            lightGO.SetActive(isOn);
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            if (DisplayHeroEntity != null)
            {
                GameEntry.Entity.HideEntity(DisplayHeroEntity);
                DisplayHeroEntity = null;
            }
        }
    }
}
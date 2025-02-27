using UnityEngine;

namespace RoundHero
{
    public class HeroSceneEntity : Entity
    {
        public DisplayHeroEntity DisplayHeroEntity;

        [SerializeField] private Transform heroPos;
        
        [SerializeField] private GameObject lightGO;

        private int heroSceneEntitySerialId;

        public async void ShowDisplayHeroEntity(int heroID)
        {
            if (DisplayHeroEntity != null)
            {
                GameEntry.Entity.HideEntity(DisplayHeroEntity);
                DisplayHeroEntity = null;
            }

            heroSceneEntitySerialId = GameEntry.Entity.GetSerialId() - 1;
            var tmpEntity = await GameEntry.Entity.ShowDisplayHeroEntityAsync(heroID);

            if (tmpEntity.Id != heroSceneEntitySerialId)
            {
                GameEntry.Entity.HideEntity(tmpEntity);
            }
            else
            {
                DisplayHeroEntity = tmpEntity;
                DisplayHeroEntity.transform.position = heroPos.position;
            }
            
            
        }
        
        public void ShowLight(bool isOn)
        {
            lightGO.SetActive(isOn);
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            if (DisplayHeroEntity != null && GameEntry.Entity.GetEntity(DisplayHeroEntity.Id) != null)
            {
                GameEntry.Entity.HideEntity(DisplayHeroEntity);
                DisplayHeroEntity = null;
            }
        }
    }
}
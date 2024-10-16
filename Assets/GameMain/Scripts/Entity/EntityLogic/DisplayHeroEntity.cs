

using UnityEngine;

namespace RoundHero
{
    public class DisplayHeroEntity : Entity
    {
        [SerializeField] public GameObject EffectGO;
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            
        }

        public void ShowEffect(bool isOn)
        {
            EffectGO.SetActive(isOn);
        }
        
    }
}
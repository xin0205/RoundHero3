
using UnityEngine;
using UnityEngine.UI;

namespace RoundHero
{
    public class RestForm : UGuiForm
    {
        [SerializeField] private Button addHeart;
        [SerializeField] private Button addMaxHP;
        
        private SceneEntity restSceneEntity;
        
        protected override async void OnOpen(object userData)
        {
            base.OnOpen(userData);

            var heroAttribute = BattleHeroManager.Instance.BattleHeroData.Attribute;
            var curHeart = heroAttribute.GetAttribute(EHeroAttribute.CurHeart);
            var maxHeart = heroAttribute.GetAttribute(EHeroAttribute.MaxHeart);
            addHeart.interactable = curHeart < maxHeart;

            //restSceneEntity = await GameEntry.Entity.ShowSceneEntityAsync("Rest");

        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            BattleMapManager.Instance.NextStep();
            
            //GameEntry.Entity.HideEntity(restSceneEntity);
        }
        
        public void AddHeart()
        {
            var heroAttribute = BattleHeroManager.Instance.BattleHeroData.Attribute;
            var curHeart = heroAttribute.GetAttribute(EHeroAttribute.CurHeart);
            var maxHeart = heroAttribute.GetAttribute(EHeroAttribute.MaxHeart);
            if (curHeart < maxHeart)
            {
                heroAttribute.SetAttribute(EHeroAttribute.CurHeart, curHeart + 1);
            }
            
        }
        
        public void AddMaxHP()
        {
            BattleHeroManager.Instance.BattleHeroData.BaseMaxHP += 1;
            BattleHeroManager.Instance.BattleHeroData.CurHP += 1;

        }

        
        
    }
}
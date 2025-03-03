
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleHeroEntity : BattleUnitEntity
    {

        public BattleHeroEntityData BattleHeroEntityData { get; protected set; }
        [SerializeField] protected TextMesh cacheHPText;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            BattleHeroEntityData = userData as BattleHeroEntityData;
            if (BattleHeroEntityData == null)
            {
                Log.Error("Error BattleHeroEntityData");
                return;
            }
            
            BattleUnitData = BattleHeroEntityData.BattleHeroData;
            

            ShowInit();

        }
        
        public override void RefreshData()
        {
            base.RefreshData();
            //cacheHPText.text = BattleHeroEntityData.BattleHeroData.CacheHPDelta.ToString();
        }
        
        public override void LookAtHero()
        {

        }
        
        public override void Dead()
        {
            base.Dead();
            
            BattleManager.Instance.ShowGameOver();
            
        }

        public void UpdateCacheHPDelta()
        {
            ChangeCurHP(BattleHeroEntityData.BattleHeroData.CacheHPDelta, false, false, true, false);
            
            if (CurHP == 0)
            {
                CurHP = -1;
                GameUtility.DelayExcute(1.5f, () =>
                {
                    Dead();
                });
               
            }
            // if (BattleHeroEntityData.BattleHeroData.CacheHPDelta < 0)
            // {
            //     Hurt();
            // }
            BattleHeroEntityData.BattleHeroData.CacheHPDelta = 0;
            //cacheHPText.text = BattleHeroEntityData.BattleHeroData.CacheHPDelta.ToString();
        }

        // public override async void ChangeCurHP(int changeHP, bool useDefense = true, bool addHeroHP = false, bool changeHPInstantly = false)
        // {
        //     HeroManager.Instance.ChangeHP(changeHP, EHPChangeType.Action, useDefense, addHeroHP, changeHPInstantly);
        //     await GameEntry.Entity.ShowBattleHurtEntityAsync(BattleHeroEntityData.BattleHeroData.GridPosIdx, changeHP);
        // }
        
        
    }
}
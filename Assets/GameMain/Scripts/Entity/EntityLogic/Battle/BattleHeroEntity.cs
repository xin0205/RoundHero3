
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleHeroEntity : BattleUnitEntity
    {

        public BattleHeroEntityData BattleHeroEntityData { get; protected set; }
        
        
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

        public override void ChangeCurHP(int changeHP, bool useDefense = true, bool addHeroHP = false, bool changeHPInstantly = false)
        {
            HeroManager.Instance.ChangeHP(changeHP, EHPChangeType.Action, useDefense, addHeroHP, changeHPInstantly);
        }
        
        
    }
}
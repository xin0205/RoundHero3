
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleSoliderEntity : BattleUnitEntity
    {
        public BattleSoliderEntityData BattleSoliderEntityData { get; protected set; }
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            BattleSoliderEntityData = userData as BattleSoliderEntityData;
            if (BattleSoliderEntityData == null)
            {
                Log.Error("Error BattleSoliderEntityData");
                return;
            }
            
            BattleUnitData = BattleSoliderEntityData.BattleSoliderData;

            ShowInit();
        }

        public override void Quit()
        {
            base.Quit();
            BattleSoliderManager.Instance.RemoveSolider(BattleSoliderEntityData.BattleSoliderData.ID);
            
        }
        
        public override void Dead()
        {
            base.Dead();
            BattleSoliderManager.Instance.RemoveSolider(BattleSoliderEntityData.BattleSoliderData.ID);
 
        }

        public override void ChangeCurHP(int changeHP, bool useDefense = true, bool addHeroHP = true, bool changeHPInstantly = true)
        {

            BattleManager.Instance.ChangeHP(BattleUnitData, changeHP,  GamePlayManager.Instance.GamePlayData, EHPChangeType.Action, useDefense, addHeroHP, changeHPInstantly);
        }
    }
}
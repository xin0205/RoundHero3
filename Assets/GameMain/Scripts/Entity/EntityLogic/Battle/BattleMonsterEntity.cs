
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleMonsterEntity : BattleUnitEntity
    {

        public BattleMonsterEntityData BattleMonsterEntityData { get; protected set; }
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            BattleMonsterEntityData = userData as BattleMonsterEntityData;
            if (BattleMonsterEntityData == null)
            {
                Log.Error("Error BattleMonsterEntityData");
                return;
            }
            
            BattleUnitData = BattleMonsterEntityData.BattleMonsterData;

            ShowInit();
            var drEnemy = GameEntry.DataTable.GetEnemy(BattleMonsterEntityData.BattleMonsterData.MonsterID);
            InitWeaponType(drEnemy.WeaponHoldingType, drEnemy.WeaponType, drEnemy.WeaponID);
            AttachWeapon(drEnemy.WeaponHoldingType, drEnemy.WeaponType, drEnemy.WeaponID);
            
            UnitAttackCastType = drEnemy.AttackCastType;
        }

        public override void Dead()
        {
            base.Dead();
            BattleEnemyManager.Instance.RemoveEnemy(BattleMonsterEntityData.BattleMonsterData.ID);
            
        }
        
        public override void Quit()
        {
            base.Quit();
            BattleEnemyManager.Instance.RemoveEnemy(BattleMonsterEntityData.BattleMonsterData.ID);
            
        }

        // public override void ChangeCurHP(int changeHP, bool useDefense = false, bool addHeroHP = false, bool changeHPInstantly = true)
        // {
        //     BattleManager.Instance.ChangeHP(BattleUnitData, changeHP,  GamePlayManager.Instance.GamePlayData, EHPChangeType.Action, useDefense, addHeroHP, changeHPInstantly);
        // }


    }
}
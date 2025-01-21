
using UnityEngine;
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

            
            var unitDescFormData = GetComponent<UnitDescTriggerItem>().UnitDescFormData;
            unitDescFormData.UnitCamp = BattleMonsterEntityData.BattleMonsterData.UnitCamp;
            unitDescFormData.UnitRole = EUnitRole.Staff;

            unitDescFormData.Idx = BattleMonsterEntityData.BattleMonsterData.Idx;

            
            
            
            UnitAttackCastType = drEnemy.AttackCastType;
        }

        

        public override void Dead()
        {
            base.Dead();
            BattleEnemyManager.Instance.RemoveEnemy(BattleMonsterEntityData.BattleMonsterData.Idx);
            GameUtility.DelayExcute(3f, () =>
            {
                GameEntry.Entity.HideEntity(this);
            });
        }
        
        public override void Quit()
        {
            base.Quit();
            BattleEnemyManager.Instance.RemoveEnemy(BattleMonsterEntityData.BattleMonsterData.Idx);
            
        }

        // public override void ChangeCurHP(int changeHP, bool useDefense = false, bool addHeroHP = false, bool changeHPInstantly = true)
        // {
        //     BattleManager.Instance.ChangeHP(BattleUnitData, changeHP,  GamePlayManager.Instance.GamePlayData, EHPChangeType.Action, useDefense, addHeroHP, changeHPInstantly);
        // }



        public void OnPointerEnter()
        {
            var movePaths = BattleFightManager.Instance.GetMovePaths(Idx);
            Root.position = GameUtility.GridPosIdxToPos(movePaths[movePaths.Count - 1]);
            BattleValueManager.Instance.ShowDisplayValue(Idx);
            BattleAttackTagManager.Instance.ShowAttackTag(Idx,
                GameUtility.GridPosIdxToPos(movePaths[movePaths.Count - 1]));
        }
        
        public void OnPointerExit()
        {
            var movePaths = BattleFightManager.Instance.GetMovePaths(Idx);
            Root.position = GameUtility.GridPosIdxToPos(movePaths[0]);
            BattleValueManager.Instance.UnShowDisplayValues();
            BattleAttackTagManager.Instance.UnShowAttackTags();
        }
    }
}
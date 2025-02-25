
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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



        public override void OnPointerEnter(BaseEventData baseEventData)
        {
            base.OnPointerEnter(baseEventData);
            
            if(CurHP <= 0)
                return;
        
            if(IsMove)
                return;
            
            if (BattleManager.Instance.BattleState == EBattleState.UseCard)
            {
                var movePaths = BattleFightManager.Instance.GetMovePaths(UnitIdx);
                if (movePaths != null && movePaths.Count > 0)
                {
                    Root.position = GameUtility.GridPosIdxToPos(movePaths[movePaths.Count - 1]);
                    BattleAttackTagManager.Instance.ShowAttackTag(UnitIdx, false);
                    BattleFlyDirectManager.Instance.ShowFlyDirect(UnitIdx);
                    BattleIconManager.Instance.ShowBattleIcon(UnitIdx, EBattleIconType.Collison);
                    //BattleFlyDirectManager.Instance.ShowEffectUnitFly(UnitIdx);
                }
            
                BattleValueManager.Instance.ShowDisplayValue(UnitIdx);
            }
            else if (BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit)
            {
                var actionUnitIdx = BattleManager.Instance.TempTriggerData.UnitData.Idx;
                BattleAttackTagManager.Instance.ShowAttackTag(actionUnitIdx, false);
                BattleFlyDirectManager.Instance.ShowFlyDirect(actionUnitIdx);
                BattleIconManager.Instance.ShowBattleIcon(actionUnitIdx, EBattleIconType.Collison);
                BattleValueManager.Instance.ShowDisplayValue(actionUnitIdx);
            }
            
        }

        

        public override void OnPointerExit(BaseEventData baseEventData)
        {
            base.OnPointerExit(baseEventData);
            
            if(BattleManager.Instance.BattleState != EBattleState.UseCard && BattleManager.Instance.BattleState != EBattleState.SelectHurtUnit)
                return;
            
            if(CurHP <= 0)
                return;
            
            if(IsMove)
                return;
            
            
            
            BattleValueManager.Instance.UnShowDisplayValues();
            
            if (BattleManager.Instance.BattleState == EBattleState.UseCard)
            {
                var movePaths = BattleFightManager.Instance.GetMovePaths(UnitIdx);
                if (movePaths != null && movePaths.Count > 0)
                {
                    Root.position = GameUtility.GridPosIdxToPos(movePaths[0]);
                    BattleAttackTagManager.Instance.UnShowAttackTags();
                    BattleFlyDirectManager.Instance.UnShowFlyDirects();
                    BattleIconManager.Instance.UnShowBattleIcons();
                    //BattleFlyDirectManager.Instance.UnShowEffectUnitFly(UnitIdx);
                }
            
                BattleValueManager.Instance.UnShowDisplayValues();
            }
            else if (BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit)
            {
                //var actionUnitIdx = BattleManager.Instance.TempTriggerData.UnitData.Idx;
                UnShowTags();
            }
            
        }
        
        protected void UnShowTags()
        {
            BattleAttackTagManager.Instance.UnShowAttackTags();
            BattleFlyDirectManager.Instance.UnShowFlyDirects();
            BattleIconManager.Instance.UnShowBattleIcons();
            BattleValueManager.Instance.UnShowDisplayValues();
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            UnShowTags();
        }
    }
}
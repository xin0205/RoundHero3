
using System.Collections.Generic;
using System.Threading.Tasks;
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
            BattleManager.Instance.ShowGameOver();
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
        public override void LookAtHero()
        {
            var attackDatas = BattleFightManager.Instance.GetDirectAttackDatas(BattleMonsterEntityData.BattleMonsterData.Idx);
            foreach (var kv in attackDatas)
            {
                 var unitEntity = BattleUnitManager.Instance.GetUnitByIdx(kv.Key);
                 if (unitEntity is BattleCoreEntity)
                 {
                     var pos = unitEntity.Position;
                     roleRoot.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                 }
            }
            
            
            
        }



        public override void OnPointerEnter(BaseEventData baseEventData)
        {
            base.OnPointerEnter(baseEventData);
            
            // if(CurHP <= 0)
            //     return;
            //
            // if(IsMove)
            //     return;
            //
            // //var actionUnitIdx = BattleManager.Instance.TempTriggerData.UnitData.Idx;
            //
            // if (BattleManager.Instance.BattleState != EBattleState.SelectHurtUnit)
            // {
            //     ShowTags(UnitIdx);
            //     
            // }
            
            
            // if (BattleManager.Instance.BattleState == EBattleState.UseCard)
            // {
            //     var movePaths = BattleFightManager.Instance.GetMovePaths(UnitIdx);
            //     if (movePaths != null && movePaths.Count > 0)
            //     {
            //         Root.position = GameUtility.GridPosIdxToPos(movePaths[movePaths.Count - 1]);
            //         BattleAttackTagManager.Instance.ShowAttackTag(UnitIdx, false);
            //         BattleFlyDirectManager.Instance.ShowFlyDirect(UnitIdx);
            //         BattleIconManager.Instance.ShowBattleIcon(UnitIdx, EBattleIconType.Collison);
            //         //BattleFlyDirectManager.Instance.ShowEffectUnitFly(UnitIdx);
            //     }
            //
            //     ShowDisplayValue(UnitIdx);
            // }
            // else if (BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit)
            // {
            //     var actionUnitIdx = BattleManager.Instance.TempTriggerData.UnitData.Idx;
            //     BattleAttackTagManager.Instance.ShowAttackTag(actionUnitIdx, false);
            //     BattleFlyDirectManager.Instance.ShowFlyDirect(actionUnitIdx);
            //     BattleIconManager.Instance.ShowBattleIcon(actionUnitIdx, EBattleIconType.Collison);
            //     ShowDisplayValue(actionUnitIdx);
            // }
            
        }

        

        public override void OnPointerExit(BaseEventData baseEventData)
        {
            base.OnPointerExit(baseEventData);
            
            // if(BattleManager.Instance.BattleState != EBattleState.UseCard && BattleManager.Instance.BattleState != EBattleState.SelectHurtUnit)
            //     return;
            
            // if(CurHP <= 0)
            //     return;
            //
            // if(IsMove)
            //     return;
            //
            // if (BattleManager.Instance.BattleState != EBattleState.SelectHurtUnit)
            // {
            //     UnShowTags();
            //
            // }

            // UnShowDisplayValues();
            //
            // if (BattleManager.Instance.BattleState == EBattleState.UseCard)
            // {
            //     var movePaths = BattleFightManager.Instance.GetMovePaths(UnitIdx);
            //     if (movePaths != null && movePaths.Count > 0)
            //     {
            //         Root.position = GameUtility.GridPosIdxToPos(movePaths[movePaths.Count - 1]);
            //         BattleAttackTagManager.Instance.UnShowAttackTags();
            //         BattleFlyDirectManager.Instance.UnShowFlyDirects();
            //         BattleIconManager.Instance.UnShowBattleIcons();
            //         //BattleFlyDirectManager.Instance.UnShowEffectUnitFly(UnitIdx);
            //     }
            //
            //     UnShowDisplayValues();
            // }
            // else if (BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit)
            // {
            //     //var actionUnitIdx = BattleManager.Instance.TempTriggerData.UnitData.Idx;
            //     UnShowTags();
            // }
            
        }
        
        

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            UnShowTags();
        }
        
        protected async override Task ShowBattleHurts(int hurt)
        {
            var effectUnitPos = Root.position;
            // var pos = ValuePos.position;
            //
            // var pos2 = Camera.main.WorldToScreenPoint(pos);
            //
            // pos2.y += 50f;
            // pos2.z = Camera.main.transform.position.z;
            // Vector3 pos3 = Camera.main.ScreenToWorldPoint(pos2);
            //
            var uiLocalPoint = PositionConvert.WorldPointToUILocalPoint(
                AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), effectUnitPos);
            var movePos  = new Vector2(uiLocalPoint.x, uiLocalPoint.y + 100);

            await GameEntry.Entity.ShowBattleMoveValueEntityAsync(uiLocalPoint,
                movePos,
                hurt, -1, false, false);
            

        }
        
        public void ShowMoveRange(bool isShow)
        {
            var drEnemy =
                GameEntry.DataTable.GetEnemy(BattleMonsterEntityData.BattleMonsterData.MonsterID);

            var range = GameUtility.GetRange(GridPosIdx, drEnemy.MoveType, EUnitCamp.Empty, null);

            foreach (var gridPosIdx in range)
            {
                var gridType = GameUtility.GetGridType(gridPosIdx, false);
                if(gridType != EGridType.Empty)
                    continue;
                
                var gridEntity = BattleAreaManager.Instance.GetGridEntityByGridPosIdx(gridPosIdx);
                gridEntity.ShowBackupGrid(isShow);
                    
            }
            
            
            
        }
    }
}
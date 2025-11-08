
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameFramework;
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
            //AttachWeapon(drEnemy.WeaponHoldingType, drEnemy.WeaponType, drEnemy.WeaponID);
            
            
            var unitDescFormData = UnitDescTriggerItem.UnitDescFormData;
            unitDescFormData.UnitCamp = BattleMonsterEntityData.BattleMonsterData.UnitCamp;
            unitDescFormData.UnitRole = EUnitRole.Staff;

            unitDescFormData.Idx = BattleMonsterEntityData.BattleMonsterData.Idx;
            unitDescFormData.GridType = EGridType.Unit;
            
            
            
            UnitAttackCastType = drEnemy.AttackCastType;
        }

        public override Data_BattleUnit BattleUnitData 
        {
            get =>  BattleMonsterEntityData.BattleMonsterData; 
            set =>  BattleMonsterEntityData.BattleMonsterData = value as Data_BattleMonster;
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
            var isLook = false;
            foreach (var kv in attackDatas)
            {
                 var unitEntity = BattleUnitManager.Instance.GetUnitByIdx(kv.Key);
                 if (unitEntity is BattleCoreEntity)
                 {
                     isLook = true;
                     var pos = unitEntity.Position;
                     roleRoot.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                     break;
                 }
            }
            foreach (var kv in attackDatas)
            {
                var unitEntity = BattleUnitManager.Instance.GetUnitByIdx(kv.Key);
                if (unitEntity is BattleSoliderEntity)
                {
                    isLook = true;
                    var pos = unitEntity.Position;
                    roleRoot.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                    break;
                }
            }
            
            foreach (var kv in attackDatas)
            {
                var unitEntity = BattleUnitManager.Instance.GetUnitByIdx(kv.Key);
                if (unitEntity.UnitIdx != UnitIdx)
                {
                    isLook = true;
                    var pos = unitEntity.Position;
                    roleRoot.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                    break;
                }
            }

            // if (!isLook && attackDatas.Count > 0)
            // {
            //     var list = attackDatas.Keys.ToList();
            //     
            //     var unitEntity = BattleUnitManager.Instance.GetUnitByIdx(list[0]);
            //     if (unitEntity != null)
            //     {
            //         var pos = unitEntity.Position;
            //         roleRoot.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
            //     }
            //     
            // }
            
        }
        
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (infoForm != null && !IsPointer)
            {
                GameEntry.UI.CloseUIForm(infoForm);
                infoForm = null;
            }
        }


        private InfoForm infoForm;
        public override async Task OnPointerEnter()
        {
            base.OnPointerEnter();

            if (BattleMonsterEntityData.BattleMonsterData.IsRoundStart == false)
            {
                var infoFormParams = new InfoFormParams()
                {
                    
                    Desc = GameEntry.Localization.GetString(Constant.Localization.Tips_RoundGenerateEenmy),
                    //Position = mousePosition + infoDelta,
                };
                
                var uiForm = await GameEntry.UI.OpenInfoFormAsync(infoFormParams);
            
                infoForm = uiForm.Logic as InfoForm;
            }
            
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

        public override void OnPointerExit()
        {
            base.OnPointerExit();
            if (infoForm != null)
            {
                GameEntry.UI.CloseUIForm(infoForm);
                infoForm = null;
            }
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
            //UnShowTags();
            if (infoForm != null)
            {
                GameEntry.UI.CloseUIForm(infoForm);
                infoForm = null;
            }
        }
        
        // protected async override Task ShowBattleHurts(int hurt)
        // {
        //     var effectUnitPos = Root.position;
        //     // var pos = ValuePos.position;
        //     //
        //     // var pos2 = Camera.main.WorldToScreenPoint(pos);
        //     //
        //     // pos2.y += 50f;
        //     // pos2.z = Camera.main.transform.position.z;
        //     // Vector3 pos3 = Camera.main.ScreenToWorldPoint(pos2);
        //     //
        //     
        //     // var uiLocalPoint = PositionConvert.WorldPointToUILocalPoint(
        //     //     AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), effectUnitPos);
        //     // var movePos  = new Vector2(uiLocalPoint.x, uiLocalPoint.y + 100);
        //     
        //     var moveParams = new MoveParams()
        //     {
        //         FollowGO = this.gameObject,
        //         DeltaPos = new Vector2(0, 25f),
        //         IsUIGO = false,
        //     };
        //     
        //     var targetMoveParams = new MoveParams()
        //     {
        //         FollowGO = this.gameObject,
        //         DeltaPos = new Vector2(0, 125f),
        //         IsUIGO = false,
        //     };
        //
        //     AddMoveValue(hurt, hurt, -1, false,
        //         this is BattleSoliderEntity && hurt < 0, moveParams, targetMoveParams);
        //         
        //     
        //     // await GameEntry.Entity.ShowBattleMoveValueEntityAsync(hurt, hurt, 0, -1, false,
        //     //     this is BattleSoliderEntity && hurt < 0, moveParams, targetMoveParams);
        //
        //
        // }
        
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
        
        public void ShowAttackRange(bool isShow)
        {
            var drEnemy =
                GameEntry.DataTable.GetEnemy(BattleMonsterEntityData.BattleMonsterData.MonsterID);
            
            foreach (var buffID in drEnemy.OwnBuffs)
            {
                var buffData = BattleBuffManager.Instance.GetBuffData(buffID);
                var range = GameUtility.GetRange(GridPosIdx,
                    buffData.TriggerRange == EActionType.HeroDirect ? EActionType.Direct82Long : buffData.TriggerRange,
                    EUnitCamp.Empty, null);

                foreach (var gridPosIdx in range)
                {
                    var gridType = GameUtility.GetGridType(gridPosIdx, false);
       
                    var gridEntity = BattleAreaManager.Instance.GetGridEntityByGridPosIdx(gridPosIdx);
                    gridEntity.ShowBackupGrid(isShow);
                    
                }
            }
            
            
            
            
            
        }
    }
}
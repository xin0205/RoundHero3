
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
            
            if(BattleManager.Instance.BattleState != EBattleState.UseCard)
                return;
                
            var movePaths = BattleFightManager.Instance.GetMovePaths(UnitIdx);
            if (movePaths != null && movePaths.Count > 0)
            {
                Root.position = GameUtility.GridPosIdxToPos(movePaths[movePaths.Count - 1]);
                BattleAttackTagManager.Instance.ShowAttackTag(UnitIdx);
                ShowEffectUnitFly(UnitIdx);
            }
            
            BattleValueManager.Instance.ShowDisplayValue(UnitIdx);
            
        }

        public void ShowEffectUnitFly(int unitIdx)
        {

            Dictionary<int, MoveUnitData> moveDataDict = new Dictionary<int, MoveUnitData>();
            
            if (BattleFightManager.Instance.RoundFightData.EnemyAttackDatas.ContainsKey(unitIdx))
            {
                moveDataDict = BattleFightManager.Instance.RoundFightData.EnemyAttackDatas[unitIdx].MoveData
                    .MoveUnitDatas;
            }
            
            foreach (var kv in moveDataDict)
            {
                var moveGridPosIdx = kv.Value.MoveActionData.MoveGridPosIdxs;
                
                var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(kv.Value.MoveActionData.ActionUnitIdx);
                actionUnit.Root.position = GameUtility.GridPosIdxToPos(moveGridPosIdx[moveGridPosIdx.Count - 1]);

            }
            
            

        }

        
        public void UnShowEffectUnitFly(int unitIdx)
        {
            Dictionary<int, MoveUnitData> moveDataDict = new Dictionary<int, MoveUnitData>();
            
            
            if (BattleFightManager.Instance.RoundFightData.EnemyAttackDatas.ContainsKey(unitIdx))
            {
                moveDataDict = BattleFightManager.Instance.RoundFightData.EnemyAttackDatas[unitIdx].MoveData
                    .MoveUnitDatas;
            }
            
            foreach (var kv in moveDataDict)
            {
                var moveGridPosIdx = kv.Value.MoveActionData.MoveGridPosIdxs;
                    
                var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(kv.Value.MoveActionData.ActionUnitIdx);
                actionUnit.Root.position = GameUtility.GridPosIdxToPos(moveGridPosIdx[0]);

            }

        }

        public override void OnPointerExit(BaseEventData baseEventData)
        {
            base.OnPointerExit(baseEventData);
            
            if(BattleManager.Instance.BattleState != EBattleState.UseCard)
                return;
            
            var movePaths = BattleFightManager.Instance.GetMovePaths(UnitIdx);
            if (movePaths != null && movePaths.Count > 0)
            {
                Root.position = GameUtility.GridPosIdxToPos(movePaths[0]);
                BattleAttackTagManager.Instance.UnShowAttackTags();
                UnShowEffectUnitFly(UnitIdx);
            }
            
            BattleValueManager.Instance.UnShowDisplayValues();
            
        }
    }
}
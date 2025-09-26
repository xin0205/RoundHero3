using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public partial class BattleUnitEntity
    {
        public Dictionary<int, BattleAttackTagEntity> BattleAttackTagEntities = new();
        
        private int curAttackTagEntityIdx = 0;
        private int showAttackTagEntityIdx = 0;
        
        // public async void ShowHurtAttackTag(int effectUnitIdx, int actionUnitIdx)
        // {
        //     // || BattleManager.Instance.BattleState == EBattleState.End
        //     if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
        //     {
        //         return;
        //     }
        //
        //     //BattleAttackTagEntities.Clear();
        //     
        //     var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
        //
        //     if (effectUnit == null)
        //         return;
        //
        //     var triggerDataDict =
        //         GameUtility.MergeDict(BattleFightManager.Instance.GetHurtDirectAttackDatas(effectUnitIdx, actionUnitIdx),
        //                               BattleFightManager.Instance.GetHurtInDirectAttackDatas(effectUnitIdx, actionUnitIdx));
        //
        //     if (triggerDataDict.Values.Count <= 0)
        //     {
        //         return;
        //     }
        //
        //     var values = triggerDataDict.Values.ToList();
        //     if (values[0].Count <= 0)
        //     {
        //         return;
        //     }
        //     
        //     var entityIdx = curAttackTagEntityIdx;
        //     
        //     
        //     foreach (var triggerDatas in values)
        //     {
        //         foreach (var triggerData in triggerDatas)
        //         {
        //             if(actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
        //                 continue;
        //             
        //             if(effectUnitIdx != -1 && triggerData.EffectUnitIdx != -1 && triggerData.EffectUnitIdx != effectUnitIdx)
        //                 continue;
        //             
        //             // var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
        //             // if(actionUnit == null)
        //             //     continue;
        //
        //             curAttackTagEntityIdx++;
        //         }
        //
        //     }
        //     
        //     var effectGridPosIdxs = new List<int>();
        //     foreach (var triggerDatas in values)
        //     {
        //         foreach (var triggerData in triggerDatas)
        //         {
        //             if(actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
        //                 continue;
        //             
        //             if(effectUnitIdx != -1 && triggerData.EffectUnitIdx != -1 && triggerData.EffectUnitIdx != effectUnitIdx)
        //                 continue;
        //             
        //             // var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
        //             // if(actionUnit == null)
        //             //     continue;
        //             
        //             if(triggerData.ActionUnitGridPosIdx == -1)
        //                 continue;
        //             
        //             var _entityIdx = entityIdx;
        //             var battleAttackTagEntity = await BattleStaticAttackTagManager.Instance.ShowTag(
        //                 triggerData.ActionUnitGridPosIdx, effectUnit.GridPosIdx, triggerData, _entityIdx,
        //                 triggerData.ActionUnitIdx != Constant.Battle.UnUnitTriggerIdx,
        //                 !effectGridPosIdxs.Contains(triggerData.EffectUnitGridPosIdx),
        //                 triggerData.TriggerDataSubType == ETriggerDataSubType.Collision, false);
        //             effectGridPosIdxs.Add(triggerData.EffectUnitGridPosIdx);
        //             entityIdx++;
        //             
        //             if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showAttackTagEntityIdx)
        //             {
        //                 //Log.Debug("Tag hide");
        //                 GameEntry.Entity.HideEntity(battleAttackTagEntity);
        //             }
        //             else
        //             {
        //         
        //                 BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
        //                 //Log.Debug("Tag add:" + BattleUnitData.Idx + "-" + BattleAttackTagEntities.Count); 
        //             }
        //             
        //             //InternalShowTag(actionUnit.BattleUnitData, triggerData.BuffValue);
        //         }
        //
        //     }
        //
        // }
        
        public async void ShowAttackTag(int actionUnitIdx, bool showAttackPos)
        {

            if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
            {
                return;
            }

            var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(actionUnitIdx);

            if (actionUnit == null)
                return;

            var triggerDataDict =
                GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(actionUnitIdx),
                                      BattleFightManager.Instance.GetInDirectAttackDatas(actionUnitIdx));

            if (triggerDataDict.Values.Count <= 0)
            {
                return;
            }

            var values = triggerDataDict.Values.ToList();
            if (values[0].Count <= 0)
            {
                return;
            }
            
            var entityIdx = curAttackTagEntityIdx;
            //var effectGridPosIdxs = new List<int>();
            
            foreach (var triggerDatas in values)
            {
                foreach (var triggerData in triggerDatas)
                {
                    if (triggerData.TriggerDataType != ETriggerDataType.Atrb)
                    {
                        continue;
                    }
                    
                    if(actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                        continue;
                    
                    if(triggerData.EffectUnitIdx == PlayerManager.Instance.PlayerData.BattleHero.Idx)
                        continue;
                    
                    var ownUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.OwnUnitIdx);
                    if(ownUnit == null)
                        continue;
                    
                    // var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                    // if(actionUnit == null)
                    //     continue;
                    //effectGridPosIdxs.Add(triggerData.EffectUnitGridPosIdx);

                    curAttackTagEntityIdx++;
                }

            }
            
            
            foreach (var triggerDatas in values)
            {
                foreach (var triggerData in triggerDatas)
                {
                    if (triggerData.TriggerDataType != ETriggerDataType.Atrb)
                    {
                        continue;
                    }
                    
                    if(actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                        continue;

                    if(triggerData.EffectUnitIdx == PlayerManager.Instance.PlayerData.BattleHero.Idx)
                        continue;

                    var ownUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.OwnUnitIdx);
                    if(ownUnit == null)
                        continue;

                    //var battleAttackTagEntity = await 
                    //!effectGridPosIdxs.Contains(triggerData.EffectUnitGridPosIdx)
                    var battleAttackTagEntity = await BattleStaticAttackTagManager.Instance.ShowTag(ownUnit.GridPosIdx, triggerData.EffectUnitGridPosIdx,
                        triggerData, entityIdx, triggerData.ActionUnitIdx != Constant.Battle.UnUnitTriggerIdx,
                        false,
                        triggerData.TriggerDataSubType == ETriggerDataSubType.Collision, false);
                    entityIdx++;
                    
                    if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showAttackTagEntityIdx)
                    {
                        //Log.Debug("Tag hide");
                        GameEntry.Entity.HideEntity(battleAttackTagEntity);
                    }
                    else
                    {
                    
                        BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
                        //Log.Debug("Tag add:" + BattleUnitData.Idx + "-" + BattleAttackTagEntities.Count); 
                    }

                }

            }
            
        }
        
        // public async void ShowAttackTag2(int actionUnitIdx, bool showAttackPos)
        // {
        //     // ||BattleManager.Instance.BattleState == EBattleState.End
        //     if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
        //     {
        //         return;
        //     }
        //     
        //     //BattleAttackTagEntities.Clear();
        //     
        //     var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(actionUnitIdx);
        //     if(actionUnit == null)
        //         return;
        //     
        //     
        //     BattleUnitManager.Instance.GetBuffValue(null, actionUnit.BattleUnitData, out List<BuffValue> triggerBuffValues);
        //     foreach (var buffValue in triggerBuffValues)
        //     {
        //         InternalShowTag(actionUnit.BattleUnitData, buffValue, showAttackPos);
        //     }
        //     
        // }
        //
        // private async void InternalShowTag(Data_BattleUnit actionUnit, BuffValue buffValue, bool showAttackPos)
        // {
        //     var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(actionUnit.Idx),
        //         BattleFightManager.Instance.GetInDirectAttackDatas(actionUnit.Idx));
        //
        //     var effectGridPosIdxs = new List<int>();
        //     foreach (var kv in triggerDataDict)
        //     {
        //         foreach (var triggerData in kv.Value)
        //         {
        //             effectGridPosIdxs.Add(triggerData.EffectUnitGridPosIdx);
        //         }
        //     }
        //
        //     var lists = new List<List<int>>();
        //     
        //
        //     if (buffValue.BuffData.TriggerRange == EActionType.HeroDirect)
        //     {
        //         var list = GameUtility.GetRange(actionUnit.GridPosIdx, buffValue.BuffData.TriggerRange, actionUnit.UnitCamp,
        //             buffValue.BuffData.TriggerUnitCamps);
        //         lists.Add(list);
        //     }
        //     else
        //     {
        //         lists = GameUtility.GetRangeNest(actionUnit.GridPosIdx, buffValue.BuffData.TriggerRange,
        //             false);
        //     }
        //
        //     var isExtend = buffValue.BuffData.TriggerRange.ToString().Contains("Extend");
        //     
        //     var entityIdx = curAttackTagEntityIdx;
        //
        //     if (isExtend)
        //     {
        //         curAttackTagEntityIdx += lists.Count;
        //     }
        //     else
        //     {
        //         foreach (var list in lists)
        //         {
        //             curAttackTagEntityIdx += list.Count;
        //         }
        //     }
        //
        //     //curAttackTagEntityIdx += effectGridPosIdxs.Count;
        //
        //     foreach (var list in lists)
        //     {
        //         for (int i = 0; i < list.Count; i++)
        //         {
        //             var gridPosIdx = list[i];
        //
        //
        //             if (BattleManager.Instance.TempTriggerData.TargetGridPosIdx != -1 &&
        //                 gridPosIdx != BattleManager.Instance.TempTriggerData.TargetGridPosIdx)
        //             {
        //                 continue;
        //             }
        //
        //             //var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(gridPosIdx);
        //             var gridType = GameUtility.GetGridType(gridPosIdx, false);
        //             //effectGridPosIdxs.Contains(gridPosIdx) || ((gridType == EGridType.Empty && i == list.Count - 1) || gridType != EGridType.Empty) && isExtend
        //             //!isExtend || (((gridType == EGridType.Empty && i == list.Count - 1) || gridType != EGridType.Empty) && isExtend )
        //             if (effectGridPosIdxs.Contains(gridPosIdx))
        //             {
        //                 
        //                 await InternalShowTag(actionUnit.GridPosIdx, gridPosIdx, buffValue, entityIdx, effectGridPosIdxs.Contains(gridPosIdx), showAttackPos, false);
        //                 entityIdx++;
        //                 
        //                 
        //                 // var effectUnitPos = GameUtility.GridPosIdxToPos(gridPosIdx);
        //                 // var actionUnitPos = GameUtility.GridPosIdxToPos(actionUnit.GridPosIdx);
        //                 //
        //                 // var attackTagType = GameUtility.IsSubCurHPBuffValue(buffValue) ? EAttackTagType.Attack :
        //                 //     GameUtility.IsSubCurHPBuffValue(buffValue) ? EAttackTagType.Recover : EAttackTagType.UnitState;
        //                 //
        //                 // var unitState = attackTagType == EAttackTagType.UnitState ? buffValue.BuffData.UnitState : EUnitState.Empty;
        //                 //
        //                 // var battleAttackTagEntity = await GameEntry.Entity.ShowBattleAttackTagEntityAsync(actionUnitPos, actionUnitPos,
        //                 //     effectUnitPos, attackTagType, unitState, entityIdx, effectGridPosIdxs.Contains(gridPosIdx), true);
        //                 //
        //                 //entityIdx++;
        //
        //                 // if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showAttackTagEntityIdx)
        //                 // {
        //                 //     GameEntry.Entity.HideEntity(battleAttackTagEntity);
        //                 // }
        //                 // else
        //                 // {
        //                 //     BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
        //                 // }
        //
        //                 if (gridType != EGridType.Empty != null && isExtend)
        //                 {
        //                     break;
        //                 }
        //             }
        //
        //         }
        //
        //     }
        // }


        // private async Task<BattleAttackTagEntity> InternalShowTag(int actionGridPosIdx, int effectUnitGridPosIdx, TriggerData triggerData,
        //     int entityIdx, bool showAttackLine, bool showAttackPos, bool isCollision)
        // {
        //
        //     var effectUnitPos = GameUtility.GridPosIdxToPos(effectUnitGridPosIdx);
        //     var actionUnitPos = GameUtility.GridPosIdxToPos(actionGridPosIdx);
        //
        //     
        //
        //     EAttackTagType attackTagType;
        //     EUnitState unitState;
        //     EAttackCastType attackCastType = EAttackCastType.ExtendMulti;
        //     if (isCollision)
        //     {
        //         attackTagType = EAttackTagType.Attack;
        //         unitState = EUnitState.Empty;
        //         showAttackLine = false;
        //         attackCastType = EAttackCastType.CloseSingle;
        //     }
        //     else
        //     {
        //         
        //         var isSubCurHPTrigger = GameUtility.IsSubCurHPTrigger(triggerData);
        //         
        //         attackTagType = isSubCurHPTrigger ? EAttackTagType.Attack :
        //             isSubCurHPTrigger ? EAttackTagType.Recover : EAttackTagType.UnitState;
        //         unitState = attackTagType == EAttackTagType.UnitState ? triggerData.UnitStateDetail.UnitState : EUnitState.Empty;
        //         attackCastType = BattleUnitManager.Instance.GetAttackCastType(triggerData.ActionUnitIdx);
        //         
        //     }
        //
        //     //Log.Debug("InternalShowTag:" + entityIdx);
        //     var battleAttackTagEntity = await GameEntry.Entity.ShowBattleAttackTagEntityAsync(actionUnitPos,
        //         actionUnitPos,
        //         effectUnitPos, attackTagType, unitState, triggerData.BuffValue, attackCastType, entityIdx, showAttackLine,
        //         showAttackPos);
        //
        //     //Log.Debug("Tag Show:" + battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx + "-" + showAttackTagEntityIdx);
        //     if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showAttackTagEntityIdx)
        //     {
        //         //Log.Debug("Tag hide");
        //         GameEntry.Entity.HideEntity(battleAttackTagEntity);
        //     }
        //     else
        //     {
        //         
        //         BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
        //         //Log.Debug("Tag add:" + BattleUnitData.Idx + "-" + BattleAttackTagEntities.Count); 
        //     }
        //
        //     return battleAttackTagEntity;
        // }
        
        
        public void UnShowAttackTags()
        {
            showAttackTagEntityIdx = curAttackTagEntityIdx;
            //Log.Debug("UnShowAttackTags:" + showAttackTagEntityIdx + "-" + BattleAttackTagEntities.Count + "-" +  + BattleUnitData.Idx);

            foreach (var kv in BattleAttackTagEntities)
            {
                //Log.Debug("HideEntity:" + kv.Value.BattleAttackTagEntityData.EntityIdx);
                GameEntry.Entity.HideEntity(kv.Value.Entity);
            }

            BattleAttackTagEntities.Clear();

        }
    }
}
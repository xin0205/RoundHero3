using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleStaticAttackTagManager : Singleton<BattleStaticAttackTagManager>
    {
        public Dictionary<int, BattleAttackTagEntity> BattleAttackTagEntities = new();
        
        private int curAttackTagEntityIdx = 0;
        private int showAttackTagEntityIdx = 0;
        
        public async Task ShowStaticAttackTags()
        {
            // if (BattleManager.Instance.BattleState != EBattleState.UseCard &&
            //     BattleManager.Instance.BattleState != EBattleState.SelectHurtUnit)
            //     return;
            
            //Log.Debug("ShowStaticAttackTags");
            UnshowStaticAttackTags();
            //var effectGridPosIdxs = new List<int>();
            var entityIdx = curAttackTagEntityIdx;
            foreach (var kv in BattleUnitManager.Instance.BattleUnitEntities)
            {
                var triggerDataDict =
                    GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(kv.Value.UnitIdx),
                        BattleFightManager.Instance.GetInDirectAttackDatas(kv.Value.UnitIdx));
                if (triggerDataDict.Values.Count <= 0)
                {
                    continue;
                }
                
                var values = triggerDataDict.Values.ToList();
                if (values[0].Count <= 0)
                {
                    return;
                }
                
                foreach (var triggerDatas in values)
                {
                    foreach (var triggerData in triggerDatas)
                    {
                        if (triggerData.TriggerDataType != ETriggerDataType.Atrb)
                        {
                            continue;
                        }
                        
                        if(kv.Value.UnitIdx != -1 && triggerData.ActionUnitIdx != -1 && triggerData.ActionUnitIdx != kv.Value.UnitIdx)
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
                        //Log.Debug("cur:" + curAttackTagEntityIdx);
                    }

                }
                
               
                
                
            }

            var keys = BattleUnitManager.Instance.BattleUnitEntities.Keys.ToList();
            for (int i = BattleUnitManager.Instance.BattleUnitEntities.Count - 1; i >= 0; i--)
            {
                if (!BattleUnitManager.Instance.BattleUnitEntities.ContainsKey(keys[i]) ||
                    BattleUnitManager.Instance.BattleUnitEntities[keys[i]] == null)
                    continue;
                
                var value = BattleUnitManager.Instance.BattleUnitEntities[keys[i]];
                var triggerDataDict =
                    GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(value.UnitIdx),
                        BattleFightManager.Instance.GetInDirectAttackDatas(value.UnitIdx));
                if (triggerDataDict.Values.Count <= 0)
                {
                    continue;
                }
                
                var values = triggerDataDict.Values.ToList();
                if (values[0].Count <= 0)
                {
                    return;
                }
                
                //var entityIdx = curAttackTagEntityIdx;
                

                foreach (var triggerDatas in values)
                {
                    foreach (var triggerData in triggerDatas)
                    {
                        if (triggerData.TriggerDataType != ETriggerDataType.Atrb)
                        {
                            continue;
                        }
                        
                        if(value.UnitIdx != -1 && triggerData.ActionUnitIdx != -1 && triggerData.ActionUnitIdx != value.UnitIdx)
                            continue;

                        if(triggerData.EffectUnitIdx == PlayerManager.Instance.PlayerData.BattleHero.Idx)
                            continue;

                        var ownUnit = GameUtility.GetUnitByIdx(BattleFightManager.Instance.RoundFightData.GamePlayData,
                            triggerData.OwnUnitIdx);
                        //BattleUnitManager.Instance.GetUnitByIdx(triggerData.OwnUnitIdx);
                        
                        if(ownUnit == null)
                            continue;

                        //Log.Debug("show before:" + entityIdx + "-" + triggerData.EffectUnitGridPosIdx);
                        //var _entityIdx = entityIdx;
                        //var battleAttackTagEntity = await 
                        //!effectGridPosIdxs.Contains(triggerData.EffectUnitGridPosIdx)
                        var battleAttackTagEntity = await ShowTag(ownUnit.GridPosIdx, triggerData.EffectUnitGridPosIdx,
                            triggerData, entityIdx, triggerData.ActionUnitIdx != Constant.Battle.UnUnitTriggerIdx,
                            false,
                            triggerData.TriggerDataSubType == ETriggerDataSubType.Collision, true);
                        
                        entityIdx++;
                        //Log.Debug("show after:" + entityIdx);
                        //
                        if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showAttackTagEntityIdx)
                        {
                            //Log.Debug("Tag hide:" + battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx + "-" + showAttackTagEntityIdx);
                            GameEntry.Entity.HideEntity(battleAttackTagEntity);
                        }
                        else
                        {
                        
                            BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
                            //Log.Debug("Tag add:" + battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx + "-" + showAttackTagEntityIdx); 
                        }

                    }

                }
            }
            
            // foreach (var kv in BattleUnitManager.Instance.BattleUnitEntities)
            // {
            //     var triggerDataDict =
            //         GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(kv.Value.UnitIdx),
            //             BattleFightManager.Instance.GetInDirectAttackDatas(kv.Value.UnitIdx));
            //     if (triggerDataDict.Values.Count <= 0)
            //     {
            //         continue;
            //     }
            //     
            //     var values = triggerDataDict.Values.ToList();
            //     if (values[0].Count <= 0)
            //     {
            //         return;
            //     }
            //     
            //     //var entityIdx = curAttackTagEntityIdx;
            //     
            //
            //     foreach (var triggerDatas in values)
            //     {
            //         foreach (var triggerData in triggerDatas)
            //         {
            //             if (triggerData.TriggerDataType != ETriggerDataType.RoleAttribute)
            //             {
            //                 continue;
            //             }
            //             
            //             if(kv.Value.UnitIdx != -1 && triggerData.ActionUnitIdx != -1 && triggerData.ActionUnitIdx != kv.Value.UnitIdx)
            //                 continue;
            //
            //             if(triggerData.EffectUnitIdx == PlayerManager.Instance.PlayerData.BattleHero.Idx)
            //                 continue;
            //
            //             var ownUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.OwnUnitIdx);
            //             if(ownUnit == null)
            //                 continue;
            //
            //             //Log.Debug("show before:" + entityIdx + "-" + triggerData.EffectUnitGridPosIdx);
            //             //var _entityIdx = entityIdx;
            //             //var battleAttackTagEntity = await 
            //             //!effectGridPosIdxs.Contains(triggerData.EffectUnitGridPosIdx)
            //             var battleAttackTagEntity = await ShowTag(ownUnit.GridPosIdx, triggerData.EffectUnitGridPosIdx,
            //                 triggerData, entityIdx, triggerData.ActionUnitIdx != Constant.Battle.UnUnitTriggerIdx,
            //                 false,
            //                 triggerData.TriggerDataSubType == ETriggerDataSubType.Collision, true);
            //             
            //             entityIdx++;
            //             //Log.Debug("show after:" + entityIdx);
            //             //
            //             if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showAttackTagEntityIdx)
            //             {
            //                 //Log.Debug("Tag hide:" + battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx + "-" + showAttackTagEntityIdx);
            //                 GameEntry.Entity.HideEntity(battleAttackTagEntity);
            //             }
            //             else
            //             {
            //             
            //                 BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
            //                 //Log.Debug("Tag add:" + battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx + "-" + showAttackTagEntityIdx); 
            //             }
            //
            //         }
            //
            //     }
            //     
            // }
        }
        
        public async Task<BattleAttackTagEntity> ShowTag(int actionGridPosIdx, int effectUnitGridPosIdx, TriggerData triggerData,
            int entityIdx, bool showAttackLine, bool showAttackPos, bool isCollision, bool isStatic)
        {

            var effectUnitPos = GameUtility.GridPosIdxToPos(effectUnitGridPosIdx);
            var actionUnitPos = GameUtility.GridPosIdxToPos(actionGridPosIdx);

            var effectUnit = BattleUnitManager.Instance.GetUnitByGridPosIdx(effectUnitGridPosIdx);
            if (effectUnit != null)
            {
                effectUnitPos = effectUnit.Position;
            }
            
            var actionsUnit = BattleUnitManager.Instance.GetUnitByGridPosIdx(actionGridPosIdx);
            if (actionsUnit != null)
            {
                actionUnitPos = actionsUnit.Position;
            }
            

            EAttackTagType attackTagType;
            EUnitState unitState;
            EAttackCastType attackCastType = EAttackCastType.ExtendMulti;
            if (isCollision)
            {
                attackTagType = EAttackTagType.Attack;
                unitState = EUnitState.Empty;
                showAttackLine = false;
                attackCastType = EAttackCastType.CloseSingle;
            }
            else
            {
                
                var isSubCurHPTrigger = GameUtility.IsSubCurHPTrigger(triggerData);
                
                attackTagType = isSubCurHPTrigger ? EAttackTagType.Attack :
                    isSubCurHPTrigger ? EAttackTagType.Recover : EAttackTagType.UnitState;
                unitState = attackTagType == EAttackTagType.UnitState ? triggerData.UnitStateDetail.UnitState : EUnitState.Empty;
                attackCastType = BattleUnitManager.Instance.GetAttackCastType(triggerData.ActionUnitIdx);
                
            }

            //Log.Debug("InternalShowTag:" + entityIdx);
            var battleAttackTagEntity = await GameEntry.Entity.ShowBattleAttackTagEntityAsync(actionUnitPos,
                actionUnitPos,
                effectUnitPos, attackTagType, unitState, triggerData.BuffValue, attackCastType, entityIdx, showAttackLine,
                showAttackPos, isStatic);

            //Log.Debug("Tag Show:" + battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx + "-" + showAttackTagEntityIdx);
            
            // if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < _showAttackTagEntityIdx)
            // {
            //     Log.Debug("Tag hide:" + battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx + "-" + showAttackTagEntityIdx);
            //     GameEntry.Entity.HideEntity(battleAttackTagEntity);
            // }
            // else
            // {
            //     
            //     battleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
            //     Log.Debug("Tag add:" + battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx + "-" + _showAttackTagEntityIdx); 
            // }
            
            //action.Invoke(battleAttackTagEntities, battleAttackTagEntity);
            

            return battleAttackTagEntity;
        }

        public void UnshowStaticAttackTags()
        {
            // if (BattleManager.Instance.BattleState != EBattleState.UseCard &&
            //     BattleManager.Instance.BattleState != EBattleState.SelectHurtUnit)
            //     return;
            
            showAttackTagEntityIdx = curAttackTagEntityIdx;
            //Log.Debug("UnShowAttackTags:" + showAttackTagEntityIdx + "-" + BattleAttackTagEntities.Count);

            foreach (var kv in BattleAttackTagEntities)
            {
                //Log.Debug("HideEntity:" + kv.Value.BattleAttackTagEntityData.EntityIdx);
                GameEntry.Entity.HideEntity(kv.Value.Entity);
            }

            BattleAttackTagEntities.Clear();
        }

        public void Destory()
        {
            UnshowStaticAttackTags();
        }
    }
}
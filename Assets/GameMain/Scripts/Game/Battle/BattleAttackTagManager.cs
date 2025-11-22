using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace RoundHero
{
    public class BattleAttackTagManager : Singleton<BattleAttackTagManager>
    {
        public Dictionary<int, BattleAttackTagEntity> BattleAttackTagEntities = new();
        
        private int curAttackTagEntityIdx = 0;
        private int showAttackTagEntityIdx = 0;
        
        public async Task ShowAttackTag(int actionUnitIdx, bool showAttackPos)
        {

            if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
            {
                return;
            }

            var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(actionUnitIdx);

            if (actionUnit == null && actionUnitIdx != Constant.Battle.UnUnitTriggerIdx)
                return;

            var triggerDataDict =BattleFightManager.Instance.GetAttackDatas(actionUnitIdx);

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
                    
                    var _actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                    if(_actionUnit == null && triggerData.ActionUnitIdx != Constant.Battle.UnUnitTriggerIdx)
                        continue;
                    
                    var _effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
                    if(_effectUnit == null)
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

                    var _actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                    if(_actionUnit == null && triggerData.ActionUnitIdx != Constant.Battle.UnUnitTriggerIdx)
                        continue;
                    
                    var _effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
                    if(_effectUnit == null)
                        continue;

                    //var battleAttackTagEntity = await 
                    //!effectGridPosIdxs.Contains(triggerData.EffectUnitGridPosIdx)
                    //triggerData.ActionUnitIdx != Constant.Battle.UnUnitTriggerIdx
                    var actionPos = GameUtility.GridPosIdxToPos(triggerData.ActionUnitGridPosIdx);
                    var battleAttackTagEntity = await BattleStaticAttackTagManager.Instance.ShowTag(actionPos, _effectUnit.Position,
                        triggerData, entityIdx, true,
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
        
        public async Task ShowHurtAttackTag(int effectUnitIdx, [CanBeNull] List<int>  actionUnitIdxs, List<int> exceptUnitIdxs = null)
        {
            // || BattleManager.Instance.BattleState == EBattleState.End
            if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
            {
                return;
            }
        
            //BattleAttackTagEntities.Clear();

            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx); //BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
        
            if (effectUnit == null)
                return;
        
            var triggerDataDict =
                GameUtility.MergeDict(BattleFightManager.Instance.GetHurtDirectAttackDatas(effectUnitIdx, actionUnitIdxs),
                                      BattleFightManager.Instance.GetHurtInDirectAttackDatas(effectUnitIdx, actionUnitIdxs));
        
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
            
            
            foreach (var triggerDatas in values)
            {
                foreach (var triggerData in triggerDatas)
                {
                    if (actionUnitIdxs != null && !actionUnitIdxs.Contains(triggerData.ActionUnitIdx))
                        continue;
                    
                    if(effectUnitIdx != -1 && triggerData.EffectUnitIdx != -1 && triggerData.EffectUnitIdx != effectUnitIdx)
                        continue;
                    
                    if(exceptUnitIdxs != null && exceptUnitIdxs.Contains(triggerData.ActionUnitIdx))
                        continue;
                    
                    // var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                    // if(actionUnit == null)
                    //     continue;
        
                    curAttackTagEntityIdx++;
                }
        
            }
            
            var effectGridPosIdxs = new List<int>();
            foreach (var triggerDatas in values)
            {
                foreach (var triggerData in triggerDatas)
                {
                    if (actionUnitIdxs != null && !actionUnitIdxs.Contains(triggerData.ActionUnitIdx))
                        continue;
                    
                    if(effectUnitIdx != -1 && triggerData.EffectUnitIdx != -1 && triggerData.EffectUnitIdx != effectUnitIdx)
                        continue;
                    
                    if(exceptUnitIdxs != null && exceptUnitIdxs.Contains(triggerData.ActionUnitIdx))
                        continue;
                    
                    var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                    if(actionUnit == null)
                        continue;
                    
                    //actionUnit == null ? Vector3.one * -999 :
                    if(actionUnit == null)
                        continue;
                    
                    if(triggerData.ActionUnitGridPosIdx == -1)
                        continue;

                    var actionPos = actionUnit != null
                        ? actionUnit.Position
                        : GameUtility.GridPosIdxToPos(triggerData.ActionUnitGridPosIdx);

                    
                    var _entityIdx = entityIdx;
                    var battleAttackTagEntity = await BattleStaticAttackTagManager.Instance.ShowTag(
                        actionPos, effectUnit.Position, triggerData, _entityIdx,
                        triggerData.ActionUnitIdx != Constant.Battle.UnUnitTriggerIdx,
                        !effectGridPosIdxs.Contains(triggerData.EffectUnitGridPosIdx),
                        triggerData.TriggerDataSubType == ETriggerDataSubType.Collision, false);
                    effectGridPosIdxs.Add(triggerData.EffectUnitGridPosIdx);
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
                    
                    //InternalShowTag(actionUnit.BattleUnitData, triggerData.BuffValue);
                }
        
            }
        
        }
        
        
        public async Task ShowTacticHurtAttackTag(int effectUnitIdx, int actionUnitIdx, List<int> exceptUnitIdxs = null)
        {

            if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
            {
                return;
            }

            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
        
            if (effectUnit == null)
                return;
        
            var triggerDataDict =
                BattleFightManager.Instance.GetTacticHurtAttackDatas(effectUnitIdx);
        
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
            
            
            foreach (var triggerDatas in values)
            {
                foreach (var triggerData in triggerDatas)
                {
                    if(actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                        continue;
                    
                    if(effectUnitIdx != -1 && triggerData.EffectUnitIdx != -1 && triggerData.EffectUnitIdx != effectUnitIdx)
                        continue;
                    
                    if(exceptUnitIdxs != null && exceptUnitIdxs.Contains(triggerData.ActionUnitIdx))
                        continue;
                    
                    // var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                    // if(actionUnit == null)
                    //     continue;
        
                    curAttackTagEntityIdx++;
                }
        
            }
            
            var effectGridPosIdxs = new List<int>();
            foreach (var triggerDatas in values)
            {
                foreach (var triggerData in triggerDatas)
                {
                    if(actionUnitIdx != -1 && triggerData.ActionUnitIdx != -1 && triggerData.ActionUnitIdx != actionUnitIdx)
                        continue;
                    
                    if(effectUnitIdx != -1 && triggerData.EffectUnitIdx != -1 && triggerData.EffectUnitIdx != effectUnitIdx)
                        continue;
                    
                    if(exceptUnitIdxs != null && exceptUnitIdxs.Contains(triggerData.ActionUnitIdx))
                        continue;
                    
                    // var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                    // if(actionUnit == null)
                    //     continue;
                    
                    if(triggerData.ActionUnitGridPosIdx == -1)
                        continue;

                    //actionUnit != null ? actionUnit.Position : 
                    var actionPos = GameUtility.GridPosIdxToPos(triggerData.ActionUnitGridPosIdx);
                    var _entityIdx = entityIdx;
                    var battleAttackTagEntity = await BattleStaticAttackTagManager.Instance.ShowTag(
                        actionPos, effectUnit.Position, triggerData, _entityIdx,
                        triggerData.ActionUnitIdx == Constant.Battle.UnUnitTriggerIdx,
                        !effectGridPosIdxs.Contains(triggerData.EffectUnitGridPosIdx),
                        triggerData.TriggerDataSubType == ETriggerDataSubType.Collision, false);
                    effectGridPosIdxs.Add(triggerData.EffectUnitGridPosIdx);
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
                    
                    //InternalShowTag(actionUnit.BattleUnitData, triggerData.BuffValue);
                }
        
            }
        
        }
        
        
    }
}
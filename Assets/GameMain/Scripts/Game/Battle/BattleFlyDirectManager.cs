using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace RoundHero
{
    public class BattleFlyDirectManager : Singleton<BattleFlyDirectManager>
    {
        public Dictionary<int, BattleFlyDirectEntity> BattleFlyDirectEntities = new();

        private int curFlyDirectEntityIdx = 0;
        private int showFlyDirectEntityIdx = 0;
        
        public async Task ShowFlyDirect(int unitIdx)
        {
            UnShowFlyDirects();
            await ShowFlyDirects(unitIdx);
        }
        public async Task ShowFlyDirects(int unitIdx)
        {
            BattleFlyDirectEntities.Clear();
            
            var triggerDataDict = BattleFightManager.Instance.GetAttackDatas(unitIdx);

            var entityIdx = curFlyDirectEntityIdx;
            
            foreach (var kv in triggerDataDict)
            {
                foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
                {
                    curFlyDirectEntityIdx++;
                }
                // var triggerData = triggerDatas[0];
                //
                // var effectUnitIdx = triggerData.EffectUnitIdx;
                // var actionUnitIdx = triggerData.ActionUnitIdx;
                //
                // var flyPathDict =
                //     BattleFightManager.Instance.GetAttackHurtFlyPaths(actionUnitIdx, effectUnitIdx);
                //
                // foreach (var kv in flyPathDict)
                // {
                //     if (kv.Value == null || kv.Value.Count <= 1)
                //     {
                //         continue;
                //     }
                //
                //     curFlyDirectEntityIdx++;
                // }

            }

            foreach (var kv in triggerDataDict)
            {
                foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
                {
                    if (kv2.Value.MoveActionData.MoveGridPosIdxs.Count < 2)
                        continue;
                    
                    var battleFlyDirectEntity =
                        await GameEntry.Entity.ShowBattleFlyDirectEntityAsync(
                            kv2.Value.MoveActionData.MoveGridPosIdxs[0], kv2.Value.MoveActionData.MoveGridPosIdxs[1],
                            entityIdx++);
                        
                    //entityIdx++;

                    if (battleFlyDirectEntity.BattleFlyDirectEntityData.EntityIdx < showFlyDirectEntityIdx)
                    {
                    
                        GameEntry.Entity.HideEntity(battleFlyDirectEntity);
                        //break;
                    }
                    else
                    {
                        BattleFlyDirectEntities.Add(battleFlyDirectEntity.Entity.Id, battleFlyDirectEntity);
                    }
                }

            }


            // foreach (var triggerDatas in triggerDataDict.Values)
            // {
            //     var triggerData = triggerDatas[0];
            //     
            //     // if(triggerData.BuffValue.BuffData.FlyType == EFlyType.Exchange)
            //     //     continue;
            //
            //     var effectUnitIdx = triggerData.EffectUnitIdx;
            //     var actionUnitIdx = triggerData.ActionUnitIdx;
            //     
            //     var flyPathDict =
            //         BattleFightManager.Instance.GetAttackHurtFlyPaths(actionUnitIdx, effectUnitIdx);
            //     
            //     foreach (var kv in flyPathDict)
            //     {
            //         if (kv.Value == null || kv.Value.Count <= 1)
            //         {
            //             continue;
            //         }
            //
            //         // var moveUnit = BattleUnitManager.Instance.GetUnitByIdx(kv.Key);
            //         // if (moveUnit != null)
            //         // {
            //         //     var pos = GameUtility.GridPosIdxToPos(kv.Value[kv.Value.Count - 1]);
            //         //     
            //         //     moveUnit.Position = pos;
            //         //     
            //         // }
            //         
            //         var battleFlyDirectEntity =
            //             await GameEntry.Entity.ShowBattleFlyDirectEntityAsync(kv.Value[0], kv.Value[1],
            //                 entityIdx++);
            //             
            //         //entityIdx++;
            //
            //         if (battleFlyDirectEntity.BattleFlyDirectEntityData.EntityIdx < showFlyDirectEntityIdx)
            //         {
            //         
            //             GameEntry.Entity.HideEntity(battleFlyDirectEntity);
            //             //break;
            //         }
            //         else
            //         {
            //             BattleFlyDirectEntities.Add(battleFlyDirectEntity.Entity.Id, battleFlyDirectEntity);
            //         }
            //     }
            //     
            //     
            //
            // }

        }

        public void UnShowFlyDirects()
        {

            showFlyDirectEntityIdx = curFlyDirectEntityIdx;

            foreach (var kv in BattleFlyDirectEntities)
            {
                GameEntry.Entity.HideEntity(kv.Value.Entity);
            }

            BattleFlyDirectEntities.Clear();

        }
        
        public void ShowHurtFlyDirect(int effectUnitIdx, [CanBeNull] List<int> actionUnitIdxs)
        {
            UnShowFlyDirects();
            ShowHurtFlyDirects(effectUnitIdx, actionUnitIdxs);
        }
        
        public async void ShowHurtFlyDirects(int effectUnitIdx, [CanBeNull] List<int> actionUnitIdxs)
        {

            BattleFlyDirectEntities.Clear();
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
            if(effectUnit == null)
                return;
            //var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(unitIdx);
            //BattleManager.Instance.TempTriggerData.UnitData.Idx, BattleManager.Instance.TempTriggerData.TargetGridPosIdx
            var triggerDataDict = BattleFightManager.Instance.GetHurtMoveDatas(actionUnitIdxs, effectUnit.GridPosIdx);
            
            var entityIdx = curFlyDirectEntityIdx;
            // curFlyDirectEntityIdx += triggerDataDict.Count;
            
            foreach (var moveUnitData in triggerDataDict.Values)
            {
                var flyPathDict =
                    BattleFightManager.Instance.GetAttackHurtFlyPaths(moveUnitData.ActionUnitIdx, moveUnitData.MoveActionData.MoveUnitIdx);
                
                foreach (var kv in flyPathDict)
                {
                    if (kv.Value == null || kv.Value.Count <= 1)
                    {
                        continue;
                    }

                    curFlyDirectEntityIdx++;
                }
                
                
            
            }
            
            foreach (var moveUnitData in triggerDataDict.Values)
            {
            
                // var effectUnitEntity = BattleUnitManager.Instance.GetUnitByGridPosIdx(moveUnitData.MoveActionData.MoveUnitIdx);
                // var actionUnitIdx = moveUnitData.ActionUnitIdx;
                // if(effectUnitEntity == null)
                //     continue;
                
                
                var flyPathDict =
                    BattleFightManager.Instance.GetAttackHurtFlyPaths(moveUnitData.ActionUnitIdx, moveUnitData.MoveActionData.MoveUnitIdx);
                
                foreach (var kv in flyPathDict)
                {
                    if (kv.Value == null || kv.Value.Count <= 1)
                    {
                        continue;
                    }
                    
                    // var direct = GameUtility.GetRelativePos(kv.Value[0], kv.Value[1]);
                    //
                    // if (direct != null)
                    // {
                    //     //(ERelativePos)direct
                    //     
                    // }
                    var battleFlyDirectEntity =
                        await GameEntry.Entity.ShowBattleFlyDirectEntityAsync(kv.Value[0], kv.Value[1],
                            entityIdx++);
                        
                    //entityIdx++;
            
                    if (battleFlyDirectEntity.BattleFlyDirectEntityData.EntityIdx < showFlyDirectEntityIdx)
                    {
                    
                        GameEntry.Entity.HideEntity(battleFlyDirectEntity);
                        //break;
                    }
                    else
                    {
                        BattleFlyDirectEntities.Add(battleFlyDirectEntity.Entity.Id, battleFlyDirectEntity);
                    }
                }
                
                
            
            }

        }
        
        
        public void ShowTacticHurtFlyDirect(int effectUnitIdx, int actionUnitIdx)
        {
            UnShowFlyDirects();
            ShowTacticHurtFlyDirects(effectUnitIdx, actionUnitIdx);
        }
        public async void ShowTacticHurtFlyDirects(int effectUnitIdx, int actionUnitIdx)
        {

            BattleFlyDirectEntities.Clear();
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
            if(effectUnit == null)
                return;
            var triggerDataDict = BattleFightManager.Instance.GetHurtMoveDatas(new List<int>() { actionUnitIdx}, effectUnit.GridPosIdx);
            
            var entityIdx = curFlyDirectEntityIdx;
            // curFlyDirectEntityIdx += triggerDataDict.Count;
            foreach (var moveUnitData in triggerDataDict.Values)
            {
            
                var flyPathDict =
                    BattleFightManager.Instance.GetAttackHurtFlyPaths(moveUnitData.ActionUnitIdx, moveUnitData.MoveActionData.MoveUnitIdx);
                
                foreach (var kv in flyPathDict)
                {
                    if (kv.Value == null || kv.Value.Count <= 1)
                    {
                        continue;
                    }

                    curFlyDirectEntityIdx++;
                }
                
                
            
            }
            
            foreach (var moveUnitData in triggerDataDict.Values)
            {
            
                // var effectUnitEntity = BattleUnitManager.Instance.GetUnitByGridPosIdx(moveUnitData.MoveActionData.MoveUnitIdx);
                // var actionUnitIdx = moveUnitData.ActionUnitIdx;
                // if(effectUnitEntity == null)
                //     continue;
                
                
                var flyPathDict =
                    BattleFightManager.Instance.GetAttackHurtFlyPaths(moveUnitData.ActionUnitIdx, moveUnitData.MoveActionData.MoveUnitIdx);
                
                foreach (var kv in flyPathDict)
                {
                    if (kv.Value == null || kv.Value.Count <= 1)
                    {
                        continue;
                    }
                    
                    // var direct = GameUtility.GetRelativePos(kv.Value[0], kv.Value[1]);
                    //
                    // if (direct != null)
                    // {
                    //     //(ERelativePos)direct
                    //     
                    // }
                    var battleFlyDirectEntity =
                        await GameEntry.Entity.ShowBattleFlyDirectEntityAsync(kv.Value[0], kv.Value[1],
                            entityIdx++);
                        
                    //entityIdx++;
            
                    if (battleFlyDirectEntity.BattleFlyDirectEntityData.EntityIdx < showFlyDirectEntityIdx)
                    {
                    
                        GameEntry.Entity.HideEntity(battleFlyDirectEntity);
                        //break;
                    }
                    else
                    {
                        BattleFlyDirectEntities.Add(battleFlyDirectEntity.Entity.Id, battleFlyDirectEntity);
                    }
                }
                
                
            
            }

        }
    }
}
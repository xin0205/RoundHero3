using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace RoundHero
{
    public partial class BattleUnitEntity
    {
        public Dictionary<int, BattleAttackTagEntity> BattleAttackTagEntities = new();

        //private bool isShowEntity = false;
        private int curAttackTagEntityIdx = 0;
        private int showAttackTagEntityIdx = 0;

        // public Random Random;
        // private int randomSeed;
        //
        // public void Init(int randomSeed)
        // {
        //     this.randomSeed = randomSeed;
        //     Random = new System.Random(this.randomSeed);
        //     curAttackTagEntityIdx = 0;
        //     showAttackTagEntityIdx = 0;
        //
        // }

        public void ShowAttackTag(int unitIdx)
        {
            UnShowAttackTags();
            ShowAttackTags(unitIdx);
        }
        
        public void ShowHurtAttackTag(int unitIdx)
        {
            UnShowAttackTags();
            ShowHurtAttackTags(unitIdx);
        }


        public async void ShowAttackTags(int unitIdx)
        {
            if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting ||
                BattleManager.Instance.BattleState == EBattleState.End)
            {
                return;
            }

            // if (isShowRoute)
            // {
            //     UnShowEnemyRoutes();
            // }

            //isShowEntity = true;
            BattleAttackTagEntities.Clear();
             
            //var attackMovePaths = BattleFightManager.Instance.GetMovePaths(unitIdx);
            //var attackStartPos = GameUtility.GridPosIdxToPos(attackMovePaths[attackMovePaths.Count - 1]);

            var triggerDataDict = BattleFightManager.Instance.GetDirectAttackDatas(unitIdx);

            var entityIdx = curAttackTagEntityIdx;
            
            
            
            // foreach (var triggerData in triggerDatas)
            // {
            //     var unit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
            //
            //     if (unit != null)
            //     {
            //         curEntityIdx++;
            //     }
            //         
            // }
            if (triggerDataDict.Values.Count <= 0)
            {
                return;
            }
            
            var values = triggerDataDict.Values.ToList();
            if (values[0].Count <= 0)
            {
                return;
            }
            
            var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(unitIdx);
            
            var triggerData = values[0][0];
                
            // var range = GameUtility.GetRange(triggerData.ActionUnitGridPosIdx, triggerData.BuffValue.BuffData.TriggerRange,
            //     actionUnit.UnitCamp, triggerData.BuffValue.BuffData.TriggerUnitCamps, true, true);

            var lists = GameUtility.GetRangeNest(triggerData.ActionUnitGridPosIdx, triggerData.BuffValue.BuffData.TriggerRange,
                false);

            var isExtend = triggerData.BuffValue.BuffData.TriggerRange.ToString().Contains("Extend");

            if (isExtend)
            {
                curAttackTagEntityIdx += lists.Count;
            }
            else
            {
                foreach (var list in lists)
                {
                    curAttackTagEntityIdx += list.Count;
                }
            }
            
            
            foreach (var list in lists)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var gridPosIdx = list[i];


                    if (BattleManager.Instance.TempTriggerData.TargetGridPosIdx != -1 &&
                        gridPosIdx != BattleManager.Instance.TempTriggerData.TargetGridPosIdx)
                    {
                        continue;
                    }

                    var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(gridPosIdx);
                    if (!isExtend || (((unit == null && i == list.Count - 1) || unit != null) && isExtend ))
                    {
                        var effectUnitPos = GameUtility.GridPosIdxToPos(gridPosIdx);
                        var actionUnitPos = GameUtility.GridPosIdxToPos(triggerData.ActionUnitGridPosIdx);

                        var attackTagType = GameUtility.IsSubCurHPTrigger(triggerData) ? EAttackTagType.Attack :
                            GameUtility.IsAddCurHPTrigger(triggerData) ? EAttackTagType.Recover : EAttackTagType.UnitState;

                        var unitState = attackTagType == EAttackTagType.UnitState ? triggerData.UnitState : EUnitState.Empty;

                        var battleAttackTagEntity = await GameEntry.Entity.ShowBattleAttackTagEntityAsync(actionUnitPos, actionUnitPos,
                            effectUnitPos, attackTagType, unitState, entityIdx);
                
                        entityIdx++;
                        //battleRouteEntity.SetCurrent(kv.Value.First() == BattleAreaManager.Instance.CurPointGridPosIdx);
                
                        // !isShowRoute || 
                        if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showAttackTagEntityIdx)
                        {
                    
                            GameEntry.Entity.HideEntity(battleAttackTagEntity);
                            //break;
                        }
                        else
                        {
                            BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
                        }

                        if (unit != null && isExtend)
                        {
                            break;
                        }
                    }

                }

            }
            
            

            // foreach (var gridPosIdx in range)
            // {
            //     var effectUnitPos = GameUtility.GridPosIdxToPos(gridPosIdx);
            //     var actionUnitPos = GameUtility.GridPosIdxToPos(triggerData.ActionUnitGridPosIdx);
            //
            //     var attackTagType = GameUtility.IsSubCurHPTrigger(triggerData) ? EAttackTagType.Attack :
            //         GameUtility.IsAddCurHPTrigger(triggerData) ? EAttackTagType.Recover : EAttackTagType.UnitState;
            //
            //     var unitState = attackTagType == EAttackTagType.UnitState ? triggerData.UnitState : EUnitState.Empty;
            //
            //     var battleAttackTagEntity = await GameEntry.Entity.ShowBattleAttackTagEntityAsync(actionUnitPos, actionUnitPos,
            //         effectUnitPos, attackTagType, unitState, entityIdx);
            //     
            //     entityIdx++;
            //     //battleRouteEntity.SetCurrent(kv.Value.First() == BattleAreaManager.Instance.CurPointGridPosIdx);
            //     
            //     // !isShowRoute || 
            //     if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showEntityIdx)
            //     {
            //         
            //         GameEntry.Entity.HideEntity(battleAttackTagEntity);
            //         //break;
            //     }
            //     else
            //     {
            //         BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
            //     }
            // }


            // foreach (var triggerDatas in triggerDataDict.Values)
            // {
            //     var triggerData = triggerDatas[0];
            //     //GameUtility.GetRange(triggerData.ActionUnitGridPosIdx, triggerData.)
            //     
            //     var actionUnitIdx = triggerData.ActionUnitIdx;
            //     //var effectUnitIdx = triggerData.EffectUnitIdx;
            //     //var actionUnit = GameUtility.GetUnitDataByIdx(actionUnitIdx);
            //     //var effectUnit = GameUtility.GetUnitDataByIdx(effectUnitIdx);
            //
            //     // var effectUnitGridPosIdx = effectUnit.GridPosIdx;
            //     // var movePaths = BattleFightManager.Instance.GetMovePaths(effectUnitIdx);
            //     // // && effectUnit.GridPosIdx < actionUnit.GridPosIdx
            //     // if (movePaths != null && movePaths.Count > 0 && isEffectUnitMove)
            //     // {
            //     //     effectUnitGridPosIdx = movePaths[movePaths.Count - 1];
            //     // }
            //
            //     
            //     
            //
            //     // var effectUnitPos = GameUtility.GridPosIdxToPos(triggerData.EffectUnitGridPosIdx);
            //     // var actionUnitPos = GameUtility.GridPosIdxToPos(triggerData.ActionUnitGridPosIdx);
            //     //
            //     // var attackTagType = GameUtility.IsSubCurHPTrigger(triggerData) ? EAttackTagType.Attack :
            //     //     GameUtility.IsAddCurHPTrigger(triggerData) ? EAttackTagType.Recover : EAttackTagType.UnitState;
            //     //
            //     // var unitState = attackTagType == EAttackTagType.UnitState ? triggerData.UnitState : EUnitState.Empty;
            //     //
            //     // var battleAttackTagEntity = await GameEntry.Entity.ShowBattleAttackTagEntityAsync(actionUnitPos, actionUnitPos,
            //     //     effectUnitPos, attackTagType, unitState, entityIdx);
            //     //
            //     // entityIdx++;
            //     // //battleRouteEntity.SetCurrent(kv.Value.First() == BattleAreaManager.Instance.CurPointGridPosIdx);
            //     //
            //     // // !isShowRoute || 
            //     // if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showEntityIdx)
            //     // {
            //     //     
            //     //     GameEntry.Entity.HideEntity(battleAttackTagEntity);
            //     //     //break;
            //     // }
            //     // else
            //     // {
            //     //     BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
            //     // }
            //
            // }

        }
        
        public async void ShowHurtAttackTags(int unitIdx)
        {


            BattleAttackTagEntities.Clear();

            var triggerDataDict = BattleFightManager.Instance.GetHurtDirectAttackDatas(unitIdx);

            var entityIdx = curAttackTagEntityIdx;
            
  
            if (triggerDataDict.Values.Count <= 0)
            {
                return;
            }
            
            var values = triggerDataDict.Values.ToList();
            if (values[0].Count <= 0)
            {
                return;
            }
            
            var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(unitIdx);
            
            var triggerData = values[0][0];
                
            // var range = GameUtility.GetRange(triggerData.ActionUnitGridPosIdx, triggerData.BuffValue.BuffData.TriggerRange,
            //     actionUnit.UnitCamp, triggerData.BuffValue.BuffData.TriggerUnitCamps, true, true);

            var lists = GameUtility.GetRangeNest(triggerData.ActionUnitGridPosIdx, triggerData.BuffValue.BuffData.TriggerRange,
                false);

            var isExtend = triggerData.BuffValue.BuffData.TriggerRange.ToString().Contains("Extend");

            if (isExtend)
            {
                curAttackTagEntityIdx += lists.Count;
            }
            else
            {
                foreach (var list in lists)
                {
                    curAttackTagEntityIdx += list.Count;
                }
            }
            
            
            foreach (var list in lists)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var gridPosIdx = list[i];
                    
                    if (BattleManager.Instance.TempTriggerData.TargetGridPosIdx != -1 &&
                        gridPosIdx != BattleManager.Instance.TempTriggerData.TargetGridPosIdx)
                    {
                        continue;
                    }

                    var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(gridPosIdx);
                    if (!isExtend || (((unit == null && i == list.Count - 1) || unit != null) && isExtend ))
                    {
                        var effectUnitPos = GameUtility.GridPosIdxToPos(gridPosIdx);
                        var actionUnitPos = GameUtility.GridPosIdxToPos(triggerData.ActionUnitGridPosIdx);

                        var attackTagType = GameUtility.IsSubCurHPTrigger(triggerData) ? EAttackTagType.Attack :
                            GameUtility.IsAddCurHPTrigger(triggerData) ? EAttackTagType.Recover : EAttackTagType.UnitState;

                        var unitState = attackTagType == EAttackTagType.UnitState ? triggerData.UnitState : EUnitState.Empty;

                        var battleAttackTagEntity = await GameEntry.Entity.ShowBattleAttackTagEntityAsync(actionUnitPos, actionUnitPos,
                            effectUnitPos, attackTagType, unitState, entityIdx);
                
                        entityIdx++;
                        //battleRouteEntity.SetCurrent(kv.Value.First() == BattleAreaManager.Instance.CurPointGridPosIdx);
                
                        // !isShowRoute || 
                        if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showAttackTagEntityIdx)
                        {
                    
                            GameEntry.Entity.HideEntity(battleAttackTagEntity);
                            //break;
                        }
                        else
                        {
                            BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
                        }

                        if (unit != null && isExtend)
                        {
                            break;
                        }
                    }

                }

            }
            

        }

        public void UnShowAttackTags()
        {

            //isShowEntity = false;
            showAttackTagEntityIdx = curAttackTagEntityIdx;

            foreach (var kv in BattleAttackTagEntities)
            {
                GameEntry.Entity.HideEntity(kv.Value.Entity);
            }

            BattleAttackTagEntities.Clear();

        }
    }
    
    
    // public class BattleAttackTagManager : Singleton<BattleAttackTagManager>
    // {
    //     public Dictionary<int, BattleAttackTagEntity> BattleAttackTagEntities = new();
    //
    //     //private bool isShowEntity = false;
    //     private int curEntityIdx = 0;
    //     private int showEntityIdx = 0;
    //
    //     public Random Random;
    //     private int randomSeed;
    //
    //
    //
    //     public void Init(int randomSeed)
    //     {
    //         this.randomSeed = randomSeed;
    //         Random = new System.Random(this.randomSeed);
    //         curEntityIdx = 0;
    //         showEntityIdx = 0;
    //
    //     }
    //
    //     public void ShowAttackTag(int unitIdx, bool isEffectUnitMove)
    //     {
    //         UnShowAttackTags();
    //         ShowAttackTags(unitIdx, isEffectUnitMove);
    //     }
    //
    //
    //     public async void ShowAttackTags(int unitIdx, bool isEffectUnitMove)
    //     {
    //         if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting ||
    //             BattleManager.Instance.BattleState == EBattleState.End)
    //         {
    //             return;
    //         }
    //
    //         // if (isShowRoute)
    //         // {
    //         //     UnShowEnemyRoutes();
    //         // }
    //
    //         //isShowEntity = true;
    //         BattleAttackTagEntities.Clear();
    //          
    //         //var attackMovePaths = BattleFightManager.Instance.GetMovePaths(unitIdx);
    //         //var attackStartPos = GameUtility.GridPosIdxToPos(attackMovePaths[attackMovePaths.Count - 1]);
    //
    //         var triggerDataDict = BattleFightManager.Instance.GetDirectAttackDatas(unitIdx);
    //
    //         var entityIdx = curEntityIdx;
    //         
    //         
    //         
    //         // foreach (var triggerData in triggerDatas)
    //         // {
    //         //     var unit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
    //         //
    //         //     if (unit != null)
    //         //     {
    //         //         curEntityIdx++;
    //         //     }
    //         //         
    //         // }
    //         if (triggerDataDict.Values.Count <= 0)
    //         {
    //             return;
    //         }
    //         
    //         var values = triggerDataDict.Values.ToList();
    //         if (values[0].Count <= 0)
    //         {
    //             return;
    //         }
    //         
    //         var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(unitIdx);
    //         
    //         var triggerData = values[0][0];
    //             
    //         // var range = GameUtility.GetRange(triggerData.ActionUnitGridPosIdx, triggerData.BuffValue.BuffData.TriggerRange,
    //         //     actionUnit.UnitCamp, triggerData.BuffValue.BuffData.TriggerUnitCamps, true, true);
    //
    //         var lists = GameUtility.GetRangeNest(triggerData.ActionUnitGridPosIdx, triggerData.BuffValue.BuffData.TriggerRange,
    //             false);
    //
    //         var isExtend = triggerData.BuffValue.BuffData.TriggerRange.ToString().Contains("Extend");
    //
    //         if (isExtend)
    //         {
    //             curEntityIdx += lists.Count;
    //         }
    //         else
    //         {
    //             foreach (var list in lists)
    //             {
    //                 curEntityIdx += list.Count;
    //             }
    //         }
    //         
    //         
    //         foreach (var list in lists)
    //         {
    //             for (int i = 0; i < list.Count; i++)
    //             {
    //                 var gridPosIdx = list[i];
    //
    //                 var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(gridPosIdx);
    //                 if (!isExtend || (((unit == null && i == list.Count - 1) || unit != null) && isExtend ))
    //                 {
    //                     var effectUnitPos = GameUtility.GridPosIdxToPos(gridPosIdx);
    //                     var actionUnitPos = GameUtility.GridPosIdxToPos(triggerData.ActionUnitGridPosIdx);
    //
    //                     var attackTagType = GameUtility.IsSubCurHPTrigger(triggerData) ? EAttackTagType.Attack :
    //                         GameUtility.IsAddCurHPTrigger(triggerData) ? EAttackTagType.Recover : EAttackTagType.UnitState;
    //
    //                     var unitState = attackTagType == EAttackTagType.UnitState ? triggerData.UnitState : EUnitState.Empty;
    //
    //                     var battleAttackTagEntity = await GameEntry.Entity.ShowBattleAttackTagEntityAsync(actionUnitPos, actionUnitPos,
    //                         effectUnitPos, attackTagType, unitState, entityIdx);
    //             
    //                     entityIdx++;
    //                     //battleRouteEntity.SetCurrent(kv.Value.First() == BattleAreaManager.Instance.CurPointGridPosIdx);
    //             
    //                     // !isShowRoute || 
    //                     if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showEntityIdx)
    //                     {
    //                 
    //                         GameEntry.Entity.HideEntity(battleAttackTagEntity);
    //                         //break;
    //                     }
    //                     else
    //                     {
    //                         BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
    //                     }
    //
    //                     if (unit != null && isExtend)
    //                     {
    //                         break;
    //                     }
    //                 }
    //
    //             }
    //
    //         }
    //         
    //         
    //
    //         // foreach (var gridPosIdx in range)
    //         // {
    //         //     var effectUnitPos = GameUtility.GridPosIdxToPos(gridPosIdx);
    //         //     var actionUnitPos = GameUtility.GridPosIdxToPos(triggerData.ActionUnitGridPosIdx);
    //         //
    //         //     var attackTagType = GameUtility.IsSubCurHPTrigger(triggerData) ? EAttackTagType.Attack :
    //         //         GameUtility.IsAddCurHPTrigger(triggerData) ? EAttackTagType.Recover : EAttackTagType.UnitState;
    //         //
    //         //     var unitState = attackTagType == EAttackTagType.UnitState ? triggerData.UnitState : EUnitState.Empty;
    //         //
    //         //     var battleAttackTagEntity = await GameEntry.Entity.ShowBattleAttackTagEntityAsync(actionUnitPos, actionUnitPos,
    //         //         effectUnitPos, attackTagType, unitState, entityIdx);
    //         //     
    //         //     entityIdx++;
    //         //     //battleRouteEntity.SetCurrent(kv.Value.First() == BattleAreaManager.Instance.CurPointGridPosIdx);
    //         //     
    //         //     // !isShowRoute || 
    //         //     if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showEntityIdx)
    //         //     {
    //         //         
    //         //         GameEntry.Entity.HideEntity(battleAttackTagEntity);
    //         //         //break;
    //         //     }
    //         //     else
    //         //     {
    //         //         BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
    //         //     }
    //         // }
    //
    //
    //         // foreach (var triggerDatas in triggerDataDict.Values)
    //         // {
    //         //     var triggerData = triggerDatas[0];
    //         //     //GameUtility.GetRange(triggerData.ActionUnitGridPosIdx, triggerData.)
    //         //     
    //         //     var actionUnitIdx = triggerData.ActionUnitIdx;
    //         //     //var effectUnitIdx = triggerData.EffectUnitIdx;
    //         //     //var actionUnit = GameUtility.GetUnitDataByIdx(actionUnitIdx);
    //         //     //var effectUnit = GameUtility.GetUnitDataByIdx(effectUnitIdx);
    //         //
    //         //     // var effectUnitGridPosIdx = effectUnit.GridPosIdx;
    //         //     // var movePaths = BattleFightManager.Instance.GetMovePaths(effectUnitIdx);
    //         //     // // && effectUnit.GridPosIdx < actionUnit.GridPosIdx
    //         //     // if (movePaths != null && movePaths.Count > 0 && isEffectUnitMove)
    //         //     // {
    //         //     //     effectUnitGridPosIdx = movePaths[movePaths.Count - 1];
    //         //     // }
    //         //
    //         //     
    //         //     
    //         //
    //         //     // var effectUnitPos = GameUtility.GridPosIdxToPos(triggerData.EffectUnitGridPosIdx);
    //         //     // var actionUnitPos = GameUtility.GridPosIdxToPos(triggerData.ActionUnitGridPosIdx);
    //         //     //
    //         //     // var attackTagType = GameUtility.IsSubCurHPTrigger(triggerData) ? EAttackTagType.Attack :
    //         //     //     GameUtility.IsAddCurHPTrigger(triggerData) ? EAttackTagType.Recover : EAttackTagType.UnitState;
    //         //     //
    //         //     // var unitState = attackTagType == EAttackTagType.UnitState ? triggerData.UnitState : EUnitState.Empty;
    //         //     //
    //         //     // var battleAttackTagEntity = await GameEntry.Entity.ShowBattleAttackTagEntityAsync(actionUnitPos, actionUnitPos,
    //         //     //     effectUnitPos, attackTagType, unitState, entityIdx);
    //         //     //
    //         //     // entityIdx++;
    //         //     // //battleRouteEntity.SetCurrent(kv.Value.First() == BattleAreaManager.Instance.CurPointGridPosIdx);
    //         //     //
    //         //     // // !isShowRoute || 
    //         //     // if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showEntityIdx)
    //         //     // {
    //         //     //     
    //         //     //     GameEntry.Entity.HideEntity(battleAttackTagEntity);
    //         //     //     //break;
    //         //     // }
    //         //     // else
    //         //     // {
    //         //     //     BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
    //         //     // }
    //         //
    //         // }
    //
    //     }
    //
    //     public void UnShowAttackTags()
    //     {
    //
    //         //isShowEntity = false;
    //         showEntityIdx = curEntityIdx;
    //
    //         foreach (var kv in BattleAttackTagEntities)
    //         {
    //             GameEntry.Entity.HideEntity(kv.Value.Entity);
    //         }
    //
    //         BattleAttackTagEntities.Clear();
    //
    //     }
    // }
}
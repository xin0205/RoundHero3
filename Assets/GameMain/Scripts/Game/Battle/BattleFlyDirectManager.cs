using System.Collections.Generic;
using Random = System.Random;

namespace RoundHero
{
    public partial class BattleUnitEntity
    {
        public Dictionary<int, BattleFlyDirectEntity> BattleFlyDirectEntities = new();

        private int curFlyDirectEntityIdx = 0;
        private int showFlyDirectEntityIdx = 0;

        // public Random Random;
        // private int randomSeed;
        //
        // public void Init(int randomSeed)
        // {
        //     this.randomSeed = randomSeed;
        //     Random = new System.Random(this.randomSeed);
        //     curEntityIdx = 0;
        //     showEntityIdx = 0;
        //
        // }

        public void ShowFlyDirect(int unitIdx)
        {
            UnShowFlyDirects();
            ShowFlyDirects(unitIdx);
        }
        
        public void ShowHurtFlyDirect(int unitIdx)
        {
            UnShowFlyDirects();
            ShowHurtFlyDirects(unitIdx);
        }
        
        public async void ShowHurtFlyDirects(int unitIdx)
        {

            BattleFlyDirectEntities.Clear();
            
            
            var triggerDataDict = GameUtility.MergeDict(BattleFightManager.Instance.GetHurtDirectAttackDatas(unitIdx),
                BattleFightManager.Instance.GetHurtInDirectAttackDatas(unitIdx));

            var entityIdx = curFlyDirectEntityIdx;
            curFlyDirectEntityIdx += triggerDataDict.Count;
            
            foreach (var triggerDatas in triggerDataDict.Values)
            {
                var triggerData = triggerDatas[0];

                var effectUnitIdx = triggerData.EffectUnitIdx;
                var actionUnitIdx = triggerData.ActionUnitIdx;
                
                var flyPathDict =
                    BattleFightManager.Instance.GetAttackHurtFlyPaths(actionUnitIdx, effectUnitIdx);
                
                foreach (var kv in flyPathDict)
                {
                    if (kv.Value == null || kv.Value.Count <= 1)
                    {
                        continue;
                    }
                    
                    var direct = GameUtility.GetRelativePos(kv.Value[0], kv.Value[1]);

                    if (direct != null)
                    {
                        var battleFlyDirectEntity =
                            await GameEntry.Entity.ShowBattleFlyDirectEntityAsync(kv.Value[0], (ERelativePos)direct,
                                entityIdx);
                        
                        entityIdx++;

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
        
        public async void ShowFlyDirects(int unitIdx)
        {

            BattleFlyDirectEntities.Clear();
            
            
            var triggerDataDict = BattleFightManager.Instance.GetDirectAttackDatas(unitIdx);

            var entityIdx = curFlyDirectEntityIdx;
            curFlyDirectEntityIdx += triggerDataDict.Count;
            
            foreach (var triggerDatas in triggerDataDict.Values)
            {
                var triggerData = triggerDatas[0];

                var effectUnitIdx = triggerData.EffectUnitIdx;
                var actionUnitIdx = triggerData.ActionUnitIdx;
                
                var flyPathDict =
                    BattleFightManager.Instance.GetAttackHurtFlyPaths(actionUnitIdx, effectUnitIdx);
                
                foreach (var kv in flyPathDict)
                {
                    if (kv.Value == null || kv.Value.Count <= 1)
                    {
                        continue;
                    }
                    
                    var direct = GameUtility.GetRelativePos(kv.Value[0], kv.Value[1]);

                    if (direct != null)
                    {
                        var battleFlyDirectEntity =
                            await GameEntry.Entity.ShowBattleFlyDirectEntityAsync(kv.Value[0], (ERelativePos)direct,
                                entityIdx);
                        
                        entityIdx++;

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

        public void UnShowFlyDirects()
        {

            showFlyDirectEntityIdx = curFlyDirectEntityIdx;

            foreach (var kv in BattleFlyDirectEntities)
            {
                GameEntry.Entity.HideEntity(kv.Value.Entity);
            }

            BattleFlyDirectEntities.Clear();

        }
        
        // public void ShowEffectUnitFly(int unitIdx)
        // {
        //
        //     Dictionary<int, MoveUnitData> moveDataDict = new Dictionary<int, MoveUnitData>();
        //     
        //     if (BattleFightManager.Instance.RoundFightData.EnemyAttackDatas.ContainsKey(unitIdx))
        //     {
        //         moveDataDict = BattleFightManager.Instance.RoundFightData.EnemyAttackDatas[unitIdx].MoveData
        //             .MoveUnitDatas;
        //     }
        //     
        //     foreach (var kv in moveDataDict)
        //     {
        //         var moveGridPosIdx = kv.Value.MoveActionData.MoveGridPosIdxs;
        //         
        //         var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(kv.Value.MoveActionData.MoveUnitIdx);
        //         actionUnit.Root.position = GameUtility.GridPosIdxToPos(moveGridPosIdx[moveGridPosIdx.Count - 1]);
        //
        //     }
        //     
        //     
        //
        // }
        //
        //
        // public void UnShowEffectUnitFly(int unitIdx)
        // {
        //     Dictionary<int, MoveUnitData> moveDataDict = new Dictionary<int, MoveUnitData>();
        //     
        //     
        //     if (BattleFightManager.Instance.RoundFightData.EnemyAttackDatas.ContainsKey(unitIdx))
        //     {
        //         moveDataDict = BattleFightManager.Instance.RoundFightData.EnemyAttackDatas[unitIdx].MoveData
        //             .MoveUnitDatas;
        //     }
        //     
        //     foreach (var kv in moveDataDict)
        //     {
        //         var moveGridPosIdx = kv.Value.MoveActionData.MoveGridPosIdxs;
        //             
        //         var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(kv.Value.MoveActionData.MoveUnitIdx);
        //         actionUnit.Root.position = GameUtility.GridPosIdxToPos(moveGridPosIdx[0]);
        //
        //     }
        //
        // }
    }
    
    // public class BattleFlyDirectManager : Singleton<BattleFlyDirectManager>
    // {
    //     public Dictionary<int, BattleFlyDirectEntity> BattleFlyDirectEntities = new();
    //
    //     private int curEntityIdx = 0;
    //     private int showEntityIdx = 0;
    //
    //     public Random Random;
    //     private int randomSeed;
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
    //     public void ShowFlyDirect(int unitIdx)
    //     {
    //         UnShowFlyDirects();
    //         ShowFlyDirects(unitIdx);
    //     }
    //     
    //     public async void ShowFlyDirects(int unitIdx)
    //     {
    //         if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting ||
    //             BattleManager.Instance.BattleState == EBattleState.End)
    //         {
    //             return;
    //         }
    //         
    //         BattleFlyDirectEntities.Clear();
    //         
    //         
    //         var triggerDataDict = BattleFightManager.Instance.GetDirectAttackDatas(unitIdx);
    //
    //         var entityIdx = curEntityIdx;
    //         curEntityIdx += triggerDataDict.Count;
    //         
    //         foreach (var triggerDatas in triggerDataDict.Values)
    //         {
    //             var triggerData = triggerDatas[0];
    //
    //             var effectUnitIdx = triggerData.EffectUnitIdx;
    //             var actionUnitIdx = triggerData.ActionUnitIdx;
    //             
    //             var flyPathDict =
    //                 BattleFightManager.Instance.GetAttackHurtFlyPaths(actionUnitIdx, effectUnitIdx);
    //             
    //             foreach (var kv in flyPathDict)
    //             {
    //                 if (kv.Value == null || kv.Value.Count <= 1)
    //                 {
    //                     continue;
    //                 }
    //                 
    //                 var direct = GameUtility.GetRelativePos(kv.Value[0], kv.Value[1]);
    //
    //                 if (direct != null)
    //                 {
    //                     var battleFlyDirectEntity =
    //                         await GameEntry.Entity.ShowBattleFlyDirectEntityAsync(kv.Value[0], (ERelativePos)direct,
    //                             entityIdx);
    //                     
    //                     entityIdx++;
    //
    //                     if (battleFlyDirectEntity.BattleFlyDirectEntityData.EntityIdx < showEntityIdx)
    //                     {
    //                 
    //                         GameEntry.Entity.HideEntity(battleFlyDirectEntity);
    //                         //break;
    //                     }
    //                     else
    //                     {
    //                         BattleFlyDirectEntities.Add(battleFlyDirectEntity.Entity.Id, battleFlyDirectEntity);
    //                     }
    //                 }
    //             }
    //             
    //             
    //
    //         }
    //
    //     }
    //
    //     public void UnShowFlyDirects()
    //     {
    //
    //         showEntityIdx = curEntityIdx;
    //
    //         foreach (var kv in BattleFlyDirectEntities)
    //         {
    //             GameEntry.Entity.HideEntity(kv.Value.Entity);
    //         }
    //
    //         BattleFlyDirectEntities.Clear();
    //
    //     }
    //     
    //     public void ShowEffectUnitFly(int unitIdx)
    //     {
    //
    //         Dictionary<int, MoveUnitData> moveDataDict = new Dictionary<int, MoveUnitData>();
    //         
    //         if (BattleFightManager.Instance.RoundFightData.EnemyAttackDatas.ContainsKey(unitIdx))
    //         {
    //             moveDataDict = BattleFightManager.Instance.RoundFightData.EnemyAttackDatas[unitIdx].MoveData
    //                 .MoveUnitDatas;
    //         }
    //         
    //         foreach (var kv in moveDataDict)
    //         {
    //             var moveGridPosIdx = kv.Value.MoveActionData.MoveGridPosIdxs;
    //             
    //             var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(kv.Value.MoveActionData.MoveUnitIdx);
    //             actionUnit.Root.position = GameUtility.GridPosIdxToPos(moveGridPosIdx[moveGridPosIdx.Count - 1]);
    //
    //         }
    //         
    //         
    //
    //     }
    //
    //     
    //     public void UnShowEffectUnitFly(int unitIdx)
    //     {
    //         Dictionary<int, MoveUnitData> moveDataDict = new Dictionary<int, MoveUnitData>();
    //         
    //         
    //         if (BattleFightManager.Instance.RoundFightData.EnemyAttackDatas.ContainsKey(unitIdx))
    //         {
    //             moveDataDict = BattleFightManager.Instance.RoundFightData.EnemyAttackDatas[unitIdx].MoveData
    //                 .MoveUnitDatas;
    //         }
    //         
    //         foreach (var kv in moveDataDict)
    //         {
    //             var moveGridPosIdx = kv.Value.MoveActionData.MoveGridPosIdxs;
    //                 
    //             var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(kv.Value.MoveActionData.MoveUnitIdx);
    //             actionUnit.Root.position = GameUtility.GridPosIdxToPos(moveGridPosIdx[0]);
    //
    //         }
    //
    //     }
    // }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    public partial class BattleUnitEntity
    {
        public Dictionary<int, BattleIconEntity> BattleIconEntities = new();

        private int curEntityIdx = 0;
        private int showEntityIdx = 0;



        public void ShowBattleIcon(int actionUnitIdx, EBattleIconType battleIconType)
        {
            UnShowBattleIcons();
            ShowBattleIcons(actionUnitIdx, battleIconType);
        }
        
        public void ShowHurtBattleIcon(int effectUnitIdx, int actionUnitIdx, EBattleIconType battleIconType)
        {
            UnShowBattleIcons();
            ShowHurtBattleIcons(effectUnitIdx, actionUnitIdx, battleIconType);
        }
        
        public async void ShowBattleIcons(int unitIdx, EBattleIconType battleIconType)
        {
            // ||BattleManager.Instance.BattleState == EBattleState.End
            if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
            {
                return;
            }
            
            BattleIconEntities.Clear();
            
            //GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(unitIdx),
            //BattleFightManager.Instance.GetInDirectAttackDatas(unitIdx));
            
            
            
             var flyPathDict =
                    BattleFightManager.Instance.GetAttackHurtFlyPaths(unitIdx);
             
             var entityIdx = curEntityIdx;
             curEntityIdx += flyPathDict.Count;

                foreach (var kv in flyPathDict)
                {
                    if (kv.Value == null || kv.Value.Count <= 2)
                    {
                        continue;
                    }
                    
                    if (kv.Value[kv.Value.Count - 1] == kv.Value[kv.Value.Count - 3])
                    {
                        var pos1 = GameUtility.GridPosIdxToPos(kv.Value[kv.Value.Count - 1]);
                        var pos2 = GameUtility.GridPosIdxToPos(kv.Value[kv.Value.Count - 2]);

                        var unit1 = GameUtility.GetUnitByGridPosIdx(kv.Value[kv.Value.Count - 1]);
                        var unit2 = GameUtility.GetUnitByGridPosIdx(kv.Value[kv.Value.Count - 2]);

                        if (unit1 != null )
                        {
                            var unit1Dict = BattleFightManager.Instance.GetHurtInDirectAttackDatas(unit1.Idx,
                                unit2.Idx);
                            foreach (var kv2 in unit1Dict)
                            {
                                var datas = new List<TriggerData>();
                                foreach (var data in kv2.Value)
                                {
                                    if(data.ActionUnitIdx == unitIdx)
                                        continue;
                                    datas.Add(data);
                                }
                                if (datas.Count > 0)
                                {
                                    ShowValues(datas);
                                }
                            
                            }
                        }
                        
                        if (unit2 != null)
                        {
                            var unit2Dict = BattleFightManager.Instance.GetHurtInDirectAttackDatas(unit2.Idx,
                                unit1.Idx);
                            foreach (var kv2 in unit2Dict)
                            {
                                var datas = new List<TriggerData>();
                                foreach (var data in kv2.Value)
                                {
                                    if(data.ActionUnitIdx == unitIdx)
                                        continue;
                                    datas.Add(data);
                                }

                                if (datas.Count > 0)
                                {
                                    ShowValues(datas);
                                }
                                
                        
                            }
                        }

                        var centerPos = (pos1 + pos2) / 2.0f;
                        centerPos.y += 1f;
                        
                        var battleIconEntity =
                            await GameEntry.Entity.ShowBattleIconEntityAsync(centerPos, EBattleIconType.Collision, entityIdx);
                        
                        entityIdx++;

                        if (battleIconEntity.BattleIconEntityData.EntityIdx < showEntityIdx)
                        {
                    
                            GameEntry.Entity.HideEntity(battleIconEntity);
                            //break;
                        }
                        else
                        {
                            BattleIconEntities.Add(battleIconEntity.Entity.Id, battleIconEntity);
                        }
                        
                    }


                }
            
            //var triggerDataDict = BattleFightManager.Instance.GetDirectAttackDatas(unitIdx);

            // var entityIdx = curEntityIdx;
            // curEntityIdx += triggerDataDict.Count;
            // foreach (var triggerDatas in triggerDataDict.Values)
            // {
            //     var triggerData = triggerDatas[0];
            //
            //     var effectUnitIdx = triggerData.EffectUnitIdx;
            //     var actionUnitIdx = triggerData.ActionUnitIdx;
            //     
            //     
            //     
            //     var flyPathDict =
            //         BattleFightManager.Instance.GetAttackHurtFlyPaths(actionUnitIdx, effectUnitIdx);
            //
            //     foreach (var kv in flyPathDict)
            //     {
            //         if (kv.Value == null || kv.Value.Count <= 2)
            //         {
            //             continue;
            //         }
            //         
            //         if (kv.Value[kv.Value.Count - 1] == kv.Value[kv.Value.Count - 3])
            //         {
            //             var pos1 = GameUtility.GridPosIdxToPos(kv.Value[kv.Value.Count - 1]);
            //             var pos2 = GameUtility.GridPosIdxToPos(kv.Value[kv.Value.Count - 2]);
            //
            //             var unit1 = GameUtility.GetUnitByGridPosIdx(kv.Value[kv.Value.Count - 1]);
            //             var unit2 = GameUtility.GetUnitByGridPosIdx(kv.Value[kv.Value.Count - 2]);
            //
            //             if (unit1 != null )
            //             {
            //                 var unit1Dict = BattleFightManager.Instance.GetHurtInDirectAttackDatas(unit1.Idx,
            //                     unit2.Idx);
            //                 foreach (var kv2 in unit1Dict)
            //                 {
            //                     var datas = new List<TriggerData>();
            //                     foreach (var data in kv2.Value)
            //                     {
            //                         if(data.ActionUnitIdx == actionUnitIdx)
            //                             continue;
            //                         datas.Add(data);
            //                     }
            //                     if (datas.Count > 0)
            //                     {
            //                         ShowValues(datas);
            //                     }
            //                 
            //                 }
            //             }
            //             
            //             if (unit2 != null)
            //             {
            //                 var unit2Dict = BattleFightManager.Instance.GetHurtInDirectAttackDatas(unit2.Idx,
            //                     unit1.Idx);
            //                 foreach (var kv2 in unit2Dict)
            //                 {
            //                     var datas = new List<TriggerData>();
            //                     foreach (var data in kv2.Value)
            //                     {
            //                         if(data.ActionUnitIdx == actionUnitIdx)
            //                             continue;
            //                         datas.Add(data);
            //                     }
            //
            //                     if (datas.Count > 0)
            //                     {
            //                         ShowValues(datas);
            //                     }
            //                     
            //             
            //                 }
            //             }
            //
            //             var centerPos = (pos1 + pos2) / 2.0f;
            //             centerPos.y += 1f;
            //             
            //             var battleIconEntity =
            //                 await GameEntry.Entity.ShowBattleIconEntityAsync(centerPos, EBattleIconType.Collision, entityIdx);
            //             
            //             entityIdx++;
            //
            //             if (battleIconEntity.BattleIconEntityData.EntityIdx < showEntityIdx)
            //             {
            //         
            //                 GameEntry.Entity.HideEntity(battleIconEntity);
            //                 //break;
            //             }
            //             else
            //             {
            //                 BattleIconEntities.Add(battleIconEntity.Entity.Id, battleIconEntity);
            //             }
            //             
            //         }
            //
            //
            //     }
            //
            // }

        }
        
        public async void ShowHurtBattleIcons(int effectUnitIdx, int actionUnitIdx, EBattleIconType battleIconType)
        {
            // ||BattleManager.Instance.BattleState == EBattleState.End
            if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
            {
                return;
            }
            
            BattleIconEntities.Clear();


            var triggerDataDict =
                GameUtility.MergeDict(BattleFightManager.Instance.GetHurtDirectAttackDatas(effectUnitIdx, actionUnitIdx)
                    ,BattleFightManager.Instance.GetHurtInDirectAttackDatas(effectUnitIdx, actionUnitIdx));
            var entityIdx = curEntityIdx;
            curEntityIdx += triggerDataDict.Count;
            
            foreach (var triggerDatas in triggerDataDict.Values)
            {
                var triggerData = triggerDatas[0];

                // var effectUnitIdx = triggerData.EffectUnitIdx;
                // var actionUnitIdx = triggerData.ActionUnitIdx;
                
                
                
                var flyPathDict =
                    BattleFightManager.Instance.GetAttackHurtFlyPaths(triggerData.ActionUnitIdx, triggerData.EffectUnitIdx);

                foreach (var kv in flyPathDict)
                {
                    if (kv.Value == null || kv.Value.Count <= 2)
                    {
                        continue;
                    }
                    
                    if (kv.Value[kv.Value.Count - 1] == kv.Value[kv.Value.Count - 3])
                    {
                        var pos1 = GameUtility.GridPosIdxToPos(kv.Value[kv.Value.Count - 1]);
                        var pos2 = GameUtility.GridPosIdxToPos(kv.Value[kv.Value.Count - 2]);

                        //var centerPos = (pos1 + pos2) / 2.0f;
                        var centerPos = pos2;
                        centerPos.y += 1f;
                        
                        var battleIconEntity =
                            await GameEntry.Entity.ShowBattleIconEntityAsync(centerPos, EBattleIconType.Collision, entityIdx);
                        
                        entityIdx++;

                        if (battleIconEntity.BattleIconEntityData.EntityIdx < showEntityIdx)
                        {
                    
                            GameEntry.Entity.HideEntity(battleIconEntity);
                            //break;
                        }
                        else
                        {
                            BattleIconEntities.Add(battleIconEntity.Entity.Id, battleIconEntity);
                        }
                        
                    }


                }

            }

        }

        public void UnShowBattleIcons()
        {

            showEntityIdx = curEntityIdx;

            foreach (var kv in BattleIconEntities)
            {
                GameEntry.Entity.HideEntity(kv.Value);
            }

            BattleIconEntities.Clear();

        }
    }
}
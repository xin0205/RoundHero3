using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace RoundHero
{
    public class BattleTagManager : Singleton<BattleTagManager>
    {
        public async Task ShowTags(int actionUnitIdx, bool isShowAttackPos = true)
        {
            if(BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
                return;

            RefreshFlyDirects(actionUnitIdx);
            BattleFlyDirectManager.Instance.ShowFlyDirect(actionUnitIdx);
            BattleStaticAttackTagManager.Instance.ShowStaticAttackTags();
            BattleAttackTagManager.Instance.ShowAttackTag(actionUnitIdx, isShowAttackPos);
            BattleIconManager.Instance.ShowBattleIcon(actionUnitIdx, EBattleIconType.Collision);
            BattleValueManager.Instance.ShowDisplayValue(actionUnitIdx);
            BattleIconValueManager.Instance.ShowDisplayIcon(actionUnitIdx);
        }
        
        public async Task ShowTagsWithFlyUnitIdx(int actionUnitIdx, bool isShowAttackPos = true)
        {
            RefreshFlyDirects(actionUnitIdx);
            await ShowTags(actionUnitIdx, isShowAttackPos);
            await ShowFlyUnitIdx(actionUnitIdx);
        }
        
        public void RefreshFlyDirects(int unitIdx)
        {
            var triggerDataDict = BattleFightManager.Instance.GetAttackDatas(unitIdx);
            
            foreach (var kv in triggerDataDict)
            {
                foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
                {
                    var moveUnit = BattleUnitManager.Instance.GetUnitByIdx(kv2.Value.UnitIdx);
                    if (moveUnit != null)
                    {
                        var pos = GameUtility.GridPosIdxToPos(
                            kv2.Value.MoveActionData.MoveGridPosIdxs
                                [kv2.Value.MoveActionData.MoveGridPosIdxs.Count - 1]);
                        
                        moveUnit.Position = pos;
                        
                    }
                }
                // var triggerData = triggerDatas[0];
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
                //     var moveUnit = BattleUnitManager.Instance.GetUnitByIdx(kv.Key);
                //     if (moveUnit != null)
                //     {
                //         var pos = GameUtility.GridPosIdxToPos(kv.Value[kv.Value.Count - 1]);
                //         
                //         moveUnit.Position = pos;
                //         
                //     }
                //
                // }

            }

        }
        
        public async Task ShowFlyUnitIdx(int unitIdx)
        {

            // var flyPathDict =
            //     BattleFightManager.Instance.GetAttackHurtFlyPaths(unitIdx);
            //     
            // foreach (var kv in flyPathDict)
            // {
            //     if (kv.Value == null || kv.Value.Count <= 1)
            //     {
            //         continue;
            //     }
            //
            //     var flyEffectUnit = BattleUnitManager.Instance.GetUnitByIdx(kv.Key);
            //     
            //     if (flyEffectUnit != null && flyEffectUnit.UnitIdx != BattleManager.Instance.TempTriggerData.UnitData.Idx)
            //     {
            //         await flyEffectUnit.ShowHurtAttackTag(kv.Key, new List<int>(){unitIdx}, new List<int>(){unitIdx});
            //         //await flyEffectUnit.ShowAttackTag(kv.Key, false);
            //         
            //         //, new List<int>(){unitIdx}
            //         // await flyEffectUnit.ShowHurtTags(kv.Key, -1);
            //         // await flyEffectUnit.ShowTags(kv.Key, false);
            //         //
            //     }
            //
            //         
            //         
            // }
            
            
        }
        
   
        
        public void UnShowTags()
        {
            if(BattleFightManager.Instance.IsAction)
                return;
            
            BattleAttackTagManager.Instance.UnShowAttackTags();
            BattleFlyDirectManager.Instance.UnShowFlyDirects();
            BattleIconManager.Instance.UnShowBattleIcons();
            BattleValueManager.Instance.UnShowDisplayValues();
            BattleIconValueManager.Instance.UnShowDisplayIcons();

        }
        
        public async Task ShowHurtTags(int effectUnitIdx, [CanBeNull] List<int> actionUnitIdxs)
        {
            if(BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
                return;
            
            UnShowTags();
            RefreshHurtFlyDirects(effectUnitIdx);
            BattleAttackTagManager.Instance.ShowHurtAttackTag(effectUnitIdx, actionUnitIdxs);
            BattleFlyDirectManager.Instance.ShowHurtFlyDirect(effectUnitIdx, actionUnitIdxs);
            BattleIconManager.Instance.ShowHurtBattleIcon(effectUnitIdx, actionUnitIdxs, EBattleIconType.Collision);
            BattleValueManager.Instance.ShowHurtDisplayValue(effectUnitIdx, actionUnitIdxs);
            BattleIconValueManager.Instance.ShowHurtDisplayIcon(effectUnitIdx, actionUnitIdxs);
            
            BattleStaticAttackTagManager.Instance.ShowStaticAttackTags();
        }
        
        public async Task ShowTacticHurtTags(int effectUnitIdx)
        {
            if(BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
                return;

            UnShowTags();
            
            RefreshHurtFlyDirects(effectUnitIdx);
            BattleAttackTagManager.Instance.ShowTacticHurtAttackTag(effectUnitIdx, Constant.Battle.UnUnitTriggerIdx);
            BattleFlyDirectManager.Instance.ShowTacticHurtFlyDirect(effectUnitIdx, Constant.Battle.UnUnitTriggerIdx);
            BattleIconManager.Instance.ShowHurtBattleIcon(effectUnitIdx, new List<int>(){Constant.Battle.UnUnitTriggerIdx}, EBattleIconType.Collision);
            BattleValueManager.Instance.ShowTacticHurtDisplayValues(effectUnitIdx);
            BattleIconValueManager.Instance.ShowTacticHurtDisplayIcons(effectUnitIdx);
            
            BattleStaticAttackTagManager.Instance.ShowStaticAttackTags();
        }
        
        public void RefreshHurtFlyDirects(int unitIdx)
        {
            var triggerDataDict = BattleFightManager.Instance.GetHurtDirectAttackDatas(unitIdx, null);
            
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

                    var moveUnit = BattleUnitManager.Instance.GetUnitByIdx(kv.Key);
                    if (moveUnit != null)
                    {
                        var pos = GameUtility.GridPosIdxToPos(kv.Value[kv.Value.Count - 1]);
                        
                        moveUnit.Position = pos;
                        
                    }

                }

            }

        }

    }
}
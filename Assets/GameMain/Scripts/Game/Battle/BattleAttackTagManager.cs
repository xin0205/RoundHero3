using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace RoundHero
{
    public partial class BattleUnitEntity
    {
        public Dictionary<int, BattleAttackTagEntity> BattleAttackTagEntities = new();
        
        private int curAttackTagEntityIdx = 0;
        private int showAttackTagEntityIdx = 0;



        public async void ShowHurtAttackTags(int effectUnitIdx, int actionUnitIdx)
        {
            if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting ||
                BattleManager.Instance.BattleState == EBattleState.End)
            {
                return;
            }

            BattleAttackTagEntities.Clear();
            
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);

            if (effectUnit == null)
                return;

            var triggerDataDict = BattleFightManager.Instance.GetHurtDirectAttackDatas(effectUnitIdx);

            if (triggerDataDict.Values.Count <= 0)
            {
                return;
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
                    if(triggerData.ActionUnitIdx != actionUnitIdx)
                        continue;
                    
                    if(triggerData.EffectUnitIdx != effectUnitIdx)
                        continue;
                    
                    var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                    if(actionUnit == null)
                        continue;
                    
                    InternalShowTag(actionUnit.BattleUnitData, triggerData.BuffValue);
                }

            }

        }
        
        public async void ShowAttackTags(int unitIdx)
        {
            if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting ||
                BattleManager.Instance.BattleState == EBattleState.End)
            {
                return;
            }
            
            BattleAttackTagEntities.Clear();
            
            var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(unitIdx);
            if(actionUnit == null)
                return;
            
            
            
            BattleUnitManager.Instance.GetBuffValue(null, actionUnit.BattleUnitData, out List<BuffValue> triggerBuffValues);
            foreach (var buffValue in triggerBuffValues)
            {
                InternalShowTag(actionUnit.BattleUnitData, buffValue);
            }
            
        }
        
        private async void InternalShowTag(Data_BattleUnit actionUnit, BuffValue buffValue)
        {
            var lists = GameUtility.GetRangeNest(actionUnit.GridPosIdx, buffValue.BuffData.TriggerRange,
                false);

            var isExtend = buffValue.BuffData.TriggerRange.ToString().Contains("Extend");
            
            var entityIdx = curAttackTagEntityIdx;

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
                        var actionUnitPos = GameUtility.GridPosIdxToPos(actionUnit.GridPosIdx);

                        var attackTagType = GameUtility.IsSubCurHPBuffValue(buffValue) ? EAttackTagType.Attack :
                            GameUtility.IsSubCurHPBuffValue(buffValue) ? EAttackTagType.Recover : EAttackTagType.UnitState;

                        var unitState = attackTagType == EAttackTagType.UnitState ? buffValue.BuffData.UnitState : EUnitState.Empty;

                        var battleAttackTagEntity = await GameEntry.Entity.ShowBattleAttackTagEntityAsync(actionUnitPos, actionUnitPos,
                            effectUnitPos, attackTagType, unitState, entityIdx);
                
                        entityIdx++;
        
                        if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showAttackTagEntityIdx)
                        {
                            GameEntry.Entity.HideEntity(battleAttackTagEntity);
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
            showAttackTagEntityIdx = curAttackTagEntityIdx;

            foreach (var kv in BattleAttackTagEntities)
            {
                GameEntry.Entity.HideEntity(kv.Value.Entity);
            }

            BattleAttackTagEntities.Clear();

        }
    }
}
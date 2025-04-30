using System.Collections.Generic;


namespace RoundHero
{
    public partial class BattleUnitEntity
    {
        public Dictionary<int, BattleFlyDirectEntity> BattleFlyDirectEntities = new();

        private int curFlyDirectEntityIdx = 0;
        private int showFlyDirectEntityIdx = 0;

        public void ShowFlyDirect(int unitIdx)
        {
            UnShowFlyDirects();
            ShowFlyDirects(unitIdx);
        }
        
        public void ShowHurtFlyDirect(int effectUnitIdx, int actionUnitIdx)
        {
            UnShowFlyDirects();
            ShowHurtFlyDirects(effectUnitIdx, actionUnitIdx);
        }
        
        public async void ShowHurtFlyDirects(int effectUnitIdx, int actionUnitIdx)
        {

            BattleFlyDirectEntities.Clear();
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);
            if(effectUnit == null)
                return;
            //var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(unitIdx);
            //BattleManager.Instance.TempTriggerData.UnitData.Idx, BattleManager.Instance.TempTriggerData.TargetGridPosIdx
            var triggerDataDict = BattleFightManager.Instance.GetHurtMoveDatas(actionUnitIdx, effectUnit.GridPosIdx);
            
            var entityIdx = curFlyDirectEntityIdx;
            curFlyDirectEntityIdx += triggerDataDict.Count;
            
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

    }

}
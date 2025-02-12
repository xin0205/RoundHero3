using System.Collections.Generic;

using Random = System.Random;

namespace RoundHero
{
    public class BattleAttackTagManager : Singleton<BattleAttackTagManager>
    {
        public Dictionary<int, BattleAttackTagEntity> BattleAttackTagEntities = new();

        //private bool isShowEntity = false;
        private int curEntityIdx = 0;
        private int showEntityIdx = 0;

        public Random Random;
        private int randomSeed;



        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
            curEntityIdx = 0;
            showEntityIdx = 0;

        }

        public void ShowAttackTag(int unitIdx, bool isEffectUnitMove)
        {
            UnShowAttackTags();
            ShowAttackTags(unitIdx, isEffectUnitMove);
        }


        public async void ShowAttackTags(int unitIdx, bool isEffectUnitMove)
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
            
            var attackMovePaths = BattleFightManager.Instance.GetMovePaths(unitIdx);
            var attackStartPos = GameUtility.GridPosIdxToPos(attackMovePaths[attackMovePaths.Count - 1]);

            var triggerDataDict = BattleFightManager.Instance.GetDirectAttackDatas(unitIdx);

            var entityIdx = curEntityIdx;
            curEntityIdx += triggerDataDict.Count;
            
            
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


            foreach (var triggerDatas in triggerDataDict.Values)
            {
                var triggerData = triggerDatas[0];
                var actionUnitIdx = triggerData.ActionUnitIdx;
                //var effectUnitIdx = triggerData.EffectUnitIdx;
                //var actionUnit = GameUtility.GetUnitDataByIdx(actionUnitIdx);
                //var effectUnit = GameUtility.GetUnitDataByIdx(effectUnitIdx);

                // var effectUnitGridPosIdx = effectUnit.GridPosIdx;
                // var movePaths = BattleFightManager.Instance.GetMovePaths(effectUnitIdx);
                // // && effectUnit.GridPosIdx < actionUnit.GridPosIdx
                // if (movePaths != null && movePaths.Count > 0 && isEffectUnitMove)
                // {
                //     effectUnitGridPosIdx = movePaths[movePaths.Count - 1];
                // }

                var effectUnitPos = GameUtility.GridPosIdxToPos(triggerData.EffectUnitGridPosIdx);
                var actionUnitPos = GameUtility.GridPosIdxToPos(triggerData.ActionUnitGridPosIdx);

                var attackTagType = GameUtility.IsSubCurHPTrigger(triggerData) ? EAttackTagType.Attack :
                    GameUtility.IsAddCurHPTrigger(triggerData) ? EAttackTagType.Recover : EAttackTagType.UnitState;

                var unitState = attackTagType == EAttackTagType.UnitState ? triggerData.UnitState : EUnitState.Empty;

                var battleAttackTagEntity = await GameEntry.Entity.ShowBattleAttackTagEntityAsync(actionUnitPos, attackStartPos,
                    effectUnitPos, attackTagType, unitState, entityIdx);
                
                entityIdx++;
                //battleRouteEntity.SetCurrent(kv.Value.First() == BattleAreaManager.Instance.CurPointGridPosIdx);
                
                // !isShowRoute || 
                if (battleAttackTagEntity.BattleAttackTagEntityData.EntityIdx < showEntityIdx)
                {
                    
                    GameEntry.Entity.HideEntity(battleAttackTagEntity);
                    //break;
                }
                else
                {
                    BattleAttackTagEntities.Add(battleAttackTagEntity.Entity.Id, battleAttackTagEntity);
                }

            }

        }

        public void UnShowAttackTags()
        {

            //isShowEntity = false;
            showEntityIdx = curEntityIdx;

            foreach (var kv in BattleAttackTagEntities)
            {
                GameEntry.Entity.HideEntity(kv.Value.Entity);
            }

            BattleAttackTagEntities.Clear();

        }
    }
}
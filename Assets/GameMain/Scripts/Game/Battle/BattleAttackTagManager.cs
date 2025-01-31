using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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

        public void ShowAttackTag(int unitIdx)
        {
            UnShowAttackTags();
            ShowAttackTags(unitIdx);
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
            
            var attckMovePaths = BattleFightManager.Instance.GetMovePaths(unitIdx);
            var attackStartPos = GameUtility.GridPosIdxToPos(attckMovePaths[attckMovePaths.Count - 1]);

            var triggerDatas = BattleFightManager.Instance.GetAttackDatas(unitIdx);

            var entityIdx = curEntityIdx;
            foreach (var triggerData in triggerDatas)
            {
                var unit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);

                if (unit != null)
                {
                    curEntityIdx++;
                }
                    
            }


            foreach (var triggerData in triggerDatas)
            {
                var actionUnitIdx = triggerData.ActionUnitIdx;
                var effectUnitIdx = triggerData.EffectUnitIdx;
                var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(actionUnitIdx);
                var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitIdx);

                var effectUnitGridPosIdx = effectUnit.GridPosIdx;
                var movePaths = BattleFightManager.Instance.GetMovePaths(effectUnitIdx);
                if (movePaths != null && movePaths.Count > 0 && effectUnit.GridPosIdx < actionUnit.GridPosIdx)
                {
                    effectUnitGridPosIdx = movePaths[movePaths.Count - 1];
                }
                
                

                var effectUnitPos = GameUtility.GridPosIdxToPos(effectUnitGridPosIdx);

                var attackTagType = GameUtility.IsSubCurHPTrigger(triggerData) ? EAttackTagType.Attack :
                    GameUtility.IsAddCurHPTrigger(triggerData) ? EAttackTagType.Recover : EAttackTagType.UnitState;

                var unitState = attackTagType == EAttackTagType.UnitState ? triggerData.UnitState : EUnitState.Empty;

                var battleAttackTagEntity = await GameEntry.Entity.ShowBattleAttackTagEntityAsync(actionUnit.Position, attackStartPos,
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
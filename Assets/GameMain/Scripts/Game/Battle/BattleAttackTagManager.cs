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

        public void ShowAttackTag(int unitID, Vector3 attackStartPos)
        {
            UnShowAttackTags();
            ShowAttackTags(unitID, attackStartPos);
        }


        public async void ShowAttackTags(int unitID, Vector3 attackStartPos)
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

            var triggerDatas = BattleFightManager.Instance.GetAttackData(unitID);

            var entityIdx = curEntityIdx;
            foreach (var triggerData in triggerDatas)
            {
                var unit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitID);

                if (unit != null)
                {
                    curEntityIdx++;
                }
                    
            }


            foreach (var triggerData in triggerDatas)
            {
                var actionUnitID = triggerData.ActionUnitID;
                var effectUnitID = triggerData.EffectUnitID;
                var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(actionUnitID);
                var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(effectUnitID);

                var attackTagType = GameUtility.IsSubCurHPTrigger(triggerData) ? EAttackTagType.Attack :
                    GameUtility.IsAddCurHPTrigger(triggerData) ? EAttackTagType.Recover : EAttackTagType.UnitState;

                var unitState = attackTagType == EAttackTagType.UnitState ? triggerData.UnitState : EUnitState.Empty;

                var battleAttackTagEntity = await GameEntry.Entity.ShowBattleAttackTagEntityAsync(actionUnit.Position, attackStartPos,
                    effectUnit.Position, attackTagType, unitState, entityIdx);
                
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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace RoundHero
{
    public class BattleGridPropManager : Singleton<BattleGridPropManager>
    {
        public Random Random;
        private int randomSeed;

        public Dictionary<int, Data_GridProp> GridPropDatas =>
            DataManager.Instance.DataGame.User.CurGamePlayData.BattleData.GridPropDatas;

        public Dictionary<int, GridPropEntity> GridPropEntities = new();

        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
        }

        public async Task GenerateGridProp(Data_GridProp gridPropData)
        {
            BattleAreaManager.Instance.RefreshObstacles();
            var places = BattleAreaManager.Instance.GetPlaces();

            var placeIdx = MathUtility.GetRandomNum(
                1, 0,
                places.Count, Random);

            for (int i = 0; i < 1; i++)
            {
                var battleGridPropEntity =
                    await GameEntry.Entity.ShowBattleGridPropEntityAsync(gridPropData);

                BattleGridPropManager.Instance.GridPropEntities.Add(battleGridPropEntity.GridPropEntityData.Id,
                    battleGridPropEntity);
                RefreshEntities();

                if (battleGridPropEntity is IMoveGrid moveGrid)
                {
                    BattleAreaManager.Instance.MoveGrids.Add(battleGridPropEntity.GridPropEntityData.Id, moveGrid);
                }
            }
        }

        public async Task GenerateMoveDirect()
        {
            BattleAreaManager.Instance.RefreshObstacles();
            var places = BattleAreaManager.Instance.GetPlaces();

            var enemyIdxs = MathUtility.GetRandomNum(
                1, 0,
                places.Count, Random);

            for (int i = 0; i < 1; i++)
            {
                var randomDirect = Random.Next(0, Enum.GetValues(typeof(ERelativePos)).Length);
                //Wrong
                // var gridPropMoveDirectEntity =
                //     await GameEntry.Entity.ShowGridPropMoveDirectEntityAsync(EGridPropID.MoveDirect,
                //         (ERelativePos) randomDirect, places[enemyIdxs[i]]);
                //
                // BattleGridPropManager.Instance.GridPropEntities.Add(
                //     gridPropMoveDirectEntity.GridPropMoveDirectEntityData.Id, gridPropMoveDirectEntity);
                // RefreshEntities();
                //
                // if (gridPropMoveDirectEntity is IMoveGrid moveGrid)
                // {
                //     BattleAreaManager.Instance.MoveGrids.Add(gridPropMoveDirectEntity.GridPropMoveDirectEntityData.Id,
                //         moveGrid);
                // }
            }
        }

        public void RefreshEntities()
        {
            // GridPropEntities.Clear();
            // foreach (var kv in BattleUnitManager.Instance.BattleUnitEntities)
            // {
            //     GridPropEntities.Add(kv.Key, kv.Value);
            // }

        }

        public void Destory()
        {
            foreach (var kv in GridPropEntities)
            {
                GameEntry.Entity.HideEntity(kv.Value);
            }
            GridPropEntities.Clear();
        }

        public Data_GridProp GetGridProp(int gridPosIdx)
        {
            // var gridProps = new List<Data_GridProp>();
            // foreach (var kv in BattleGridPropManager.Instance.GridPropDatas)
            // {
            //     if (kv.Value.GridPosIdx == gridPosIdx)
            //     {
            //         gridProps.Add(kv.Value);
            //     }
            // }
            //
            // return gridProps;


            foreach (var kv in BattleGridPropManager.Instance.GridPropDatas)
            {
                if (kv.Value.GridPosIdx == gridPosIdx)
                {
                    return kv.Value;
                }
            }

            return null;


        }

        public GridPropEntity GetGridPropEntity(int gridPosIdx)
        {
            // var gridProps = new List<Data_GridProp>();
            // foreach (var kv in BattleGridPropManager.Instance.GridPropDatas)
            // {
            //     if (kv.Value.GridPosIdx == gridPosIdx)
            //     {
            //         gridProps.Add(kv.Value);
            //     }
            // }
            //
            // return gridProps;


            foreach (var kv in BattleGridPropManager.Instance.GridPropEntities)
            {
                if (kv.Value.GridPosIdx == gridPosIdx)
                {
                    return kv.Value;
                }
            }

            return null;


        }

        public Data_GridProp Contain(int gridPropID, int gridPosIdx)
        {
            foreach (var kv in GridPropDatas)
            {
                if (kv.Value.GridPropID == gridPropID && kv.Value.GridPosIdx == gridPosIdx)
                    return kv.Value;
            }

            return null;

        }

        public void RefreshPropMoveDirectUseInRound()
        {
            foreach (var kv in GridPropDatas)
            {
                var drGridProp = GameEntry.DataTable.GetGridProp(kv.Value.GridPropID);
                if (Contain(drGridProp.Id, EGridPropID.MoveDirect))
                {
                    (kv.Value as Data_GridPropMoveDirect).UseInRound = false;
                }

            }
        }

        public void AttackTrigger(int gridPosIdx, TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            foreach (var kv in GridPropDatas)
            {
                var drGridProp = GameEntry.DataTable.GetGridProp(kv.Value.GridPropID);
                var idx = 0;
                foreach (var buffIDStr in drGridProp.GridPropIDs)
                {
                    var buffData = BattleBuffManager.Instance.GetBuffData(buffIDStr);

                    if(!GameUtility.InRange(kv.Value.GridPosIdx, buffData.TriggerRange, gridPosIdx))
                        continue;
                    
                    idx++;
                    BattleBuffManager.Instance.BuffTrigger(EBuffTriggerType.Attack,
                        buffData, GetValues(kv.Value.GridPropID, idx), triggerData.ActionUnitIdx, triggerData.ActionUnitIdx, triggerData.EffectUnitIdx,
                        triggerDatas);
                }

            }
        }

        public List<string> GetValues(int gridPropID, int idx)
        {

            var drGridProp = GameEntry.DataTable.GetGridProp(gridPropID);
            if (drGridProp == null)
                return new List<string>();
            
            return drGridProp.GetValues(idx);
            
        }
        
        public bool Contain(int gridPropID, EGridPropID eGridPropID)
        {
            return Contain(gridPropID, eGridPropID.ToString());
        }
        
        public bool Contain(int gridPropID, string buffIDStr)
        {
            var drGridProp = GameEntry.DataTable.GetGridProp(gridPropID);
            return drGridProp.GridPropIDs.Contains(buffIDStr);
        }

        public bool IsStayProp(int propID)
        {
            var isStayProp = false;
            var drProp = GameEntry.DataTable.GetGridProp(propID);
            var buffData = BattleBuffManager.Instance.GetBuffData(drProp.GridPropIDs[0]);

            return buffData.BuffTriggerType == EBuffTriggerType.Stay;
        }

        public void TriggerStayPropState(int gridPosIdx, Data_BattleUnit unit, EUnitState state)
        {
            var prop = BattleGridPropManager.Instance.GetGridProp(gridPosIdx);
            if (prop != null)
            {
                var drProp = GameEntry.DataTable.GetGridProp(prop.GridPropID);
                var buffData = BattleBuffManager.Instance.GetBuffData(drProp.GridPropIDs[0]);
                var buffValue = BattleBuffManager.Instance.GetBuffValue(drProp.GetValues(0)[0]);
                if (buffData.BuffTriggerType == EBuffTriggerType.Stay && buffData.UnitState == state)
                {
                    unit.ChangeState(buffData.UnitState, (int)buffValue);
                }
            }
        }
        
    }
}
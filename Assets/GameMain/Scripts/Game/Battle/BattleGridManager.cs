using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RoundHero
{
    public class BattleGridManager : Singleton<BattleGridManager>
    {
        public List<BattleGridEntity> GridEntities = new();
        public Dictionary<int, BattleGridEntity> GridEntitiesMap = new();
        
        public void Destory()
        {
            foreach (var kv in GridEntities)
            {
                GameEntry.Entity.HideEntity(kv);
                            
            }
            GridEntities.Clear();
            GridEntitiesMap.Clear();
            BattleAreaManager.Instance.CurPointGridPosIdx = -1;
        }
        
        public void ShowGreenGrids(List<int> gridPosIdxs)
        {
            foreach (var kv in GridEntities)
            {
                if (gridPosIdxs == null)
                {
                    kv.ShowGreenGrid(false);
                }
                else if (gridPosIdxs.Contains(kv.GridPosIdx))
                {
                    kv.ShowGreenGrid(true);
                }
                else
                {
                    kv.ShowGreenGrid(false);
                }

            }
        }
        
        public void ShowRedGrids(List<int> gridPosIdxs)
        {
            foreach (var kv in GridEntities)
            {
                if (gridPosIdxs == null)
                {
                    kv.ShowRedGrid(false);
                }
                else if (gridPosIdxs.Contains(kv.GridPosIdx))
                {
                    kv.ShowRedGrid(true);
                }
                else
                {
                    kv.ShowRedGrid(false);
                }

            }
        }
        
        public void ShowYellowGrids(List<int> gridPosIdxs)
        {
            foreach (var kv in GridEntities)
            {
                if (gridPosIdxs == null)
                {
                    kv.ShowYellowGrid(false);
                }
                else if (gridPosIdxs.Contains(kv.GridPosIdx))
                {
                    kv.ShowYellowGrid(true);
                }
                else
                {
                    kv.ShowYellowGrid(false);
                }

            }
        }
        
        public void UnshowGrids()
        {
            foreach (var kv in GridEntities)
            {
                kv.UnshowGrid();

            }
        }
        
        public void RefreshGirdEntities()
        {
            GridEntitiesMap.Clear();
            foreach (var kv in GridEntities)
            {
                kv.Refresh();
                GridEntitiesMap.Add(kv.GridPosIdx, kv);
            }

        }
        
        public void ShowAllGrid(bool show)
        {
            foreach (var kv in GridEntities)
            {
                kv.Show(show);
            }
        }
        
        public BattleGridEntity GetGridEntityByGridPosIdx(int gridPosIdx)
        {
            foreach (var kv in GridEntities)
            {
                if (kv.BattleGridEntityData.GridPosIdx == gridPosIdx)
                {
                    return kv;
                }
            }

            return null;
        }

        public async Task GenerateGridEntity(int gridPosIdx, EGridType gridType)
        {
            var gridEntity = await GameEntry.Entity.ShowGridEntityAsync(gridPosIdx,
                gridType);
                
            GridEntities.Add(gridEntity);
            GridEntitiesMap.Add(gridPosIdx, gridEntity);

            if (gridEntity is IMoveGrid moveGrid)
            {
                BattleAreaManager.Instance.MoveGrids.Add(gridEntity.BattleGridEntityData.Id, moveGrid);
            }

            if (gridType == EGridType.Obstacle)
            {
                var obstacleEntity =
                    await GameEntry.Entity.ShowGridPropObstacleEntityAsync(Constant.Battle.ObstacleGridID, gridPosIdx);
                if (obstacleEntity is IMoveGrid moveGrid2)
                {
                    BattleAreaManager.Instance.MoveGrids.Add(obstacleEntity.GridPropEntityData.Id, moveGrid2);
                }
                    
                BattleGridPropManager.Instance.GridPropDatas.Add(obstacleEntity.GridPropData.Idx, obstacleEntity.GridPropData);
                BattleGridPropManager.Instance.GridPropEntities.Add(obstacleEntity.GridPropEntityData.Id,
                    obstacleEntity);
            }
        }

    }
}
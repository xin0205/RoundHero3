using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public partial class BattleAreaManager : Singleton<BattleAreaManager>
    {
        public void HideTmpUnitEntity()
        {
            if (TmpUnitEntity != null)
            {
                Log.Debug("TmpUnitEntity != null:" + TmpUnitEntity.Id + "-" + TmpUnitEntity.BattleUnitData.Idx + "-" + BattleUnitManager.Instance.BattleUnitDatas.Count);
                TmpUnitEntity.UnShowTags();
                            
                BattleUnitManager.Instance.BattleUnitDatas.Remove(TmpUnitEntity.BattleUnitData.Idx);
                Log.Debug("22:" + BattleUnitManager.Instance.BattleUnitDatas.Count);
                BattleUnitManager.Instance.BattleUnitEntities.Remove(TmpUnitEntity.BattleUnitData.Idx);
                if(GameEntry.Entity.HasEntity(TmpUnitEntity.Id))
                {
                    Log.Debug("HasEntity(TmpUnitEntity.Id)");
                    GameEntry.Entity.HideEntity(TmpUnitEntity);
                }
                
                TmpUnitEntity = null;
            }
        }
        
        
        public void HideTmpPropEntity()
        {
            if (TmpPropEntity != null)
            {
                Log.Debug("TmpUnitEntity != null:" + TmpPropEntity.GridPropData.Idx + "-" + BattleGridPropManager.Instance.GridPropDatas.Count);
                //TmpPropEntity.UnShowTags();
                            
                BattleGridPropManager.Instance.GridPropDatas.Remove(TmpPropEntity.GridPropData.Idx);
                Log.Debug("22:" + BattleGridPropManager.Instance.GridPropDatas.Count);
                BattleGridPropManager.Instance.GridPropEntities.Remove(TmpPropEntity.GridPropData.Idx);
                if(GameEntry.Entity.HasEntity(TmpPropEntity.Id))
                {
                    Log.Debug("HasEntity(TmpPropEntity.Id)");
                    GameEntry.Entity.HideEntity(TmpPropEntity);
                }
                
                TmpPropEntity = null;
            }
        }
        
        public async Task ShowMoveUnitTags(bool isShow)
        {
            // foreach (var kv in moveGridPosIdxs)
            // {
            //     var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(kv.Value);
            //     if(unit == null)
            //         continue;
            //     if (isShow)
            //     {
            //         unit.ShowTags(unit.UnitIdx, false);
            //         unit.ShowHurtTags(unit.UnitIdx);
            //     }
            //     else
            //     {
            //         unit.UnShowTags();
            //     }
            // }
            
            Log.Debug("ShowMove:" + isShow);
            foreach (var kv in BattleUnitManager.Instance.BattleUnitEntities)
            {

                if (isShow)
                {
                    kv.Value.UnShowTags();
                    if(!MoveGridPosIdxs.ContainsValue(kv.Value.GridPosIdx))
                        continue;
                    
                    await kv.Value.ShowTagsWithFlyUnitIdx(kv.Value.UnitIdx, false);
                    //kv.Value.ShowHurtTags(kv.Value.UnitIdx);
                }
                else
                {
                    kv.Value.UnShowTags();
                }
                
            }
        }

       
        public void RefreshObstacles()
        {
            for (int i = 0; i < BattleManager.Instance.BattleData.GridTypes.Count; i++)
            {
                if (BattleManager.Instance.BattleData.GridTypes[i] != EGridType.TemporaryUnit)
                {
                    BattleManager.Instance.BattleData.GridTypes[i] = EGridType.Empty;
                }

            }

            foreach (var kv in MoveGrids)
            {
                if (kv.Value is BattleGridEntity gridEntity)
                {
                    if (gridEntity.BattleGridEntityData.GridType == EGridType.Obstacle)
                    {
                        BattleManager.Instance.BattleData.GridTypes[gridEntity.BattleGridEntityData.GridPosIdx] =
                            EGridType.Obstacle;
                    }
                }
                else if (kv.Value is BattleSoliderEntity || kv.Value is BattleMonsterEntity ||
                         kv.Value is BattleHeroEntity || kv.Value is BattleCoreEntity)
                {
                    if (!(BattleManager.Instance.TempTriggerData.UnitData != null &&
                          BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx == kv.Value.GridPosIdx))
                    {
                        BattleManager.Instance.BattleData.GridTypes[kv.Value.GridPosIdx] = EGridType.Unit;
                    }
                    
                }
            }
        }

        public List<int> GetPlaces()
        {
            var places = new List<int>();
            foreach (var kv in BattleManager.Instance.BattleData.GridTypes)
            {
                if (kv.Value == EGridType.Empty)
                {
                    places.Add(kv.Key);
                }
            }

            return places;


        }
        
        public void ResetMoveGrid()
        {
            if (IsMoveGrid)
            {
                //UpdateGrid(pointDownCoord, MoveDirection, -allMoveDelta);
                foreach (var kv in MoveGridPosIdxs)
                {
                    var moveGrid = MoveGrids[kv.Key];
                    moveGrid.GridPosIdx = kv.Value;
                    moveGrid.Position = GameUtility.GridPosIdxToPos(moveGrid.GridPosIdx);
                }
                

                ClearMoveGrid();
            
                RefreshGirdEntities();
                RefreshObstacles();
                BattleManager.Instance.RefreshEnemyAttackData();
            }
            

            
        }

        public void ClearMoveGrid()
        {
            mousePositionDelta = Vector2.zero;
            moveDelta = Vector2.zero;
            //allMoveDelta = Vector2.zero;
            curMoveDelta = Vector3.zero;
            IsMoveGrid = false;
        }

        public void ClearExchangeGrid()
        {
            TempExchangeGridData.GridPosIdx1 = -1;
            TempExchangeGridData.GridPosIdx2 = -1;
            ShowBackupGrids(null);
            //BattleEnemyManager.Instance.UnShowEnemyRoutes();
        }
        
        public List<IMoveGrid> GetUnits(int gridPosIdx)
        {
            var moveGirds = new List<IMoveGrid>();
            foreach (var kv in MoveGrids)
            {
                var moveGridType = kv.Value.GetType();
                if (kv.Value.GridPosIdx == gridPosIdx && (moveGridType == typeof(BattleMonsterEntity) ||
                                                          moveGridType == typeof(BattleSoliderEntity) ||
                                                          moveGridType == typeof(BattleCoreEntity)))
                {
                    moveGirds.Add(kv.Value);
                }
            }

            return moveGirds;
        }
        
        public async Task<BattleSoliderEntity> GenerateSolider(Data_BattleSolider battleSoliderData)
        {
            var battleSoliderEntity =
                await GameEntry.Entity.ShowBattleSoliderEntityAsync(battleSoliderData);
            
            BattleUnitManager.Instance.BattleUnitDatas.Add(battleSoliderData.Idx, battleSoliderData);
            BattleUnitManager.Instance.BattleUnitEntities.Add(
                battleSoliderEntity.BattleSoliderEntityData.BattleSoliderData.Idx, battleSoliderEntity);

            if (battleSoliderEntity is IMoveGrid moveGrid)
            {
                BattleAreaManager.Instance.MoveGrids.Add(battleSoliderEntity.BattleSoliderEntityData.Id, moveGrid);
            }

            BattleSoliderManager.Instance.RefreshSoliderEntities();

            //BattleManager.Instance.RecordLastActionBattleData();

            return battleSoliderEntity;
        }

        public async Task<GridPropEntity> GenerateProp(Data_GridProp gridPropData)
        {
            var propEntity =
                await GameEntry.Entity.ShowBattleGridPropEntityAsync(gridPropData);
            
            BattleGridPropManager.Instance.GridPropDatas.Add(gridPropData.Idx, gridPropData);
            BattleGridPropManager.Instance.GridPropEntities.Add(
                propEntity.GridPropEntityData.GridPropData.Idx, propEntity);

            if (propEntity is IMoveGrid moveGrid)
            {
                BattleAreaManager.Instance.MoveGrids.Add(propEntity.GridPropEntityData.Id, moveGrid);
            }

            BattleGridPropManager.Instance.RefreshEntities();

            //BattleManager.Instance.RecordLastActionBattleData();

            return propEntity;
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

        public async Task GenerateArea()
        {
            BattleAreaManager.Instance.RefreshObstacles();

            var obstacles = new List<int>();
            var obstacleIdxs = new List<int>();

            if (GamePlayManager.Instance.GamePlayData.IsTutorialBattle)
            {
                obstacles = Constant.Tutorial.Obstacles;
                
                for (int i = 0; i < Constant.Area.ObstacleCount; i++)
                {
                    obstacleIdxs.Add(obstacles[i]);
                }
            }
            else
            {
                var places = BattleAreaManager.Instance.GetPlaces();
                var insidePlaces = new List<int>();
                foreach (var gridPosIdx in places)
                {
                    var coord = GameUtility.GridPosIdxToCoord(gridPosIdx);
                    if (coord.x == 0 || coord.y == 0 || coord.x == Constant.Area.GridSize.x - 1 ||
                        coord.y == Constant.Area.GridSize.y - 1)
                    {
                        continue;
                    }
                    insidePlaces.Add(gridPosIdx);
                }
                
                obstacles = MathUtility.GetRandomNum(Constant.Area.ObstacleCount, 0,
                    insidePlaces.Count, Random);
                
                for (int i = 0; i < Constant.Area.ObstacleCount; i++)
                {
                    obstacleIdxs.Add(insidePlaces[obstacles[i]]);
                }
            }

            for (int i = 0; i < Constant.Area.GridSize.x * Constant.Area.GridSize.y; i++)
            {
                var isObstacle = obstacleIdxs.Contains(i);
                
                await GenerateGridEntity(i, isObstacle ? EGridType.Obstacle : EGridType.Empty);

            }

        }


        public async Task GenerateGridEntity(int gridPosIdx, EGridType gridType)
        {
            var gridEntity = await GameEntry.Entity.ShowGridEntityAsync(gridPosIdx,
                gridType);
                
            GridEntities.Add(gridEntity);
            GridEntitiesMap.Add(gridPosIdx, gridEntity);

            if (gridEntity is IMoveGrid moveGrid)
            {
                MoveGrids.Add(gridEntity.BattleGridEntityData.Id, moveGrid);
            }

            if (gridType == EGridType.Obstacle)
            {
                var obstacleEntity =
                    await GameEntry.Entity.ShowGridPropObstacleEntityAsync(Constant.Battle.ObstacleGridID, gridPosIdx);
                if (obstacleEntity is IMoveGrid moveGrid2)
                {
                    MoveGrids.Add(obstacleEntity.GridPropEntityData.Id, moveGrid2);
                }
                    
                BattleGridPropManager.Instance.GridPropDatas.Add(obstacleEntity.GridPropData.Idx, obstacleEntity.GridPropData);
                BattleGridPropManager.Instance.GridPropEntities.Add(obstacleEntity.GridPropEntityData.Id,
                    obstacleEntity);
            }
        }

        public void UpdateMoveGrid()
        {
            if (!IsMoveGrid)
            {
                if (Input.GetMouseButtonDown(0) && !pointerDownInRange)
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (!Physics.Raycast(ray, out hit))
                    {
                        return;
                    }

                    pointDownCoord = GameUtility.GridPosToCoord(hit.point);

                    if (pointDownCoord.x >= 0 && pointDownCoord.y >= 0 &&
                        pointDownCoord.x < Constant.Area.GridSize.x &&
                        pointDownCoord.y < Constant.Area.GridSize.y)
                    {
                        mousePositionDelta = Vector2.zero;
                        moveDelta = Vector2.zero;
                        //allMoveDelta = Vector2.zero;
                        curMoveDelta = Vector3.zero;
                        moveCountDelta = 0;
                        pointerDownInRange = true;
                        lastMousePosition = hit.point;
                        MoveDirection = null;
                        //BattleEnemyManager.Instance.ShowEnemyRoutes();
                        ShowMoveUnitTags(false);
                    }

                    lastMoveCount = 0;
                }
                else if (Input.GetMouseButton(0) && pointerDownInRange)
                {

                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (!Physics.Raycast(ray, out hit))
                    {
                        return;
                    }

    
                    if (Math.Abs(mousePositionDelta.x) >= 0.5f || Math.Abs(mousePositionDelta.z) >= 0.5f)
                    {

                        moveDelta = hit.point - lastMousePosition;
                        //allMoveDelta += moveDelta;
                        curMoveDelta += moveDelta;

                        if (MoveDirection == null)
                        {
                            var dragAngle = Mathf.Atan2(mousePositionDelta.z, mousePositionDelta.x) * 180 / Math.PI;
                            //allMoveDelta += mousePositionDelta;
                            if (dragAngle >= -20 && dragAngle <= 20 || dragAngle >= 160 && dragAngle <= 180 ||
                                dragAngle >= -180 && dragAngle <= -160)
                            {
                                MoveDirection = EDirection.Horizonal;
                            }
                            else if (dragAngle >= 70 && dragAngle <= 110 || dragAngle >= -110 && dragAngle <= -70)
                            {
                                MoveDirection = EDirection.Vertial;

                            }
                            else if (dragAngle >= 20 && dragAngle <= 70 || dragAngle >= -160 && dragAngle <= -110)
                            {
                                MoveDirection = EDirection.XRight;

                            }
                            else if (dragAngle >= 110 && dragAngle <= 160 || dragAngle >= -70 && dragAngle <= -20)
                            {
                                MoveDirection = EDirection.XLeft;

                            }
                            
                            var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
                            var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
                            var isAllMove = buffData.BuffStr == EBuffID.Spec_MoveAllGrid.ToString();
                            
                            MoveGridPosIdxs.Clear();
                            
                            foreach (var kv in MoveGrids)
                            {
                                var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
                                var pos = kv.Value.Position;

                                if (MoveDirection == EDirection.Horizonal && (isAllMove || coord.y == pointDownCoord.y))
                                {
                                    MoveGridPosIdxs.Add(kv.Key, kv.Value.GridPosIdx);
                                }
                                else if (MoveDirection == EDirection.Vertial && (isAllMove || coord.x == pointDownCoord.x))
                                {
                                    MoveGridPosIdxs.Add(kv.Key, kv.Value.GridPosIdx);
                                }
                                else if (MoveDirection == EDirection.XRight && !isAllMove &&
                                         coord.x - pointDownCoord.x == coord.y - pointDownCoord.y)
                                {
                                    MoveGridPosIdxs.Add(kv.Key, kv.Value.GridPosIdx);

                                }
                                else if (MoveDirection == EDirection.XLeft && !isAllMove &&
                                         coord.x - pointDownCoord.x == pointDownCoord.y - coord.y)
                                {
                                    MoveGridPosIdxs.Add(kv.Key, kv.Value.GridPosIdx);
                                }
                            }
                        }
                        

                        MoveGrid(pointDownCoord, MoveDirection, moveDelta);
                        CheckUpdateGrid(pointDownCoord, MoveDirection, curMoveDelta);
                        
                    }
                    else
                    {
                        
                        moveDelta = hit.point - lastMousePosition;

                        mousePositionDelta += moveDelta;
                    }

                    lastMousePosition = hit.point;
                }
                else if (Input.GetMouseButtonUp(0) && pointerDownInRange)
                {
                    pointerDownInRange = false;
                    UpdateGrid();
                    ShowMoveUnitTags(false);
                }
            }

        }

        private void MoveGrid(Vector2Int pointDownCoord, EDirection? direction, Vector3 deltaPos)
        {
            var gridRootPos = AreaController.Instance.GridRoot.transform.position;
            var gridRange = Constant.Area.GridRange;
            var gridSize = Constant.Area.GridSize;

            var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
            var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
            var isAllMove = buffData.BuffStr == EBuffID.Spec_MoveAllGrid.ToString();

            foreach (var kv in MoveGridPosIdxs)
            {
                var moveGrid = MoveGrids[kv.Key];
                
                var coord = GameUtility.GridPosIdxToCoord(moveGrid.GridPosIdx);
                var pos = moveGrid.Position;

                // && (isAllMove || coord.y == pointDownCoord.y)
                if (direction == EDirection.Horizonal)
                {
                    moveGrid.Position = new Vector3(pos.x + deltaPos.x, pos.y, pos.z);
                    
                    if (moveGrid.Position.x >= gridRootPos.x +
                        gridSize.x * gridRange.x - gridRange.x / 2)
                    {
                        var deltaPosX = moveGrid.Position.x - (gridRootPos.x +
                            gridSize.x * gridRange.x - gridRange.x / 2);
                        moveGrid.Position = new Vector3(gridRootPos.x - gridRange.x / 2 + deltaPosX, pos.y, pos.z);
                    }
                    else if (moveGrid.Position.x <=
                             gridRootPos.x - gridRange.x / 2)
                    {
                        var deltaPosX = gridRootPos.x - gridRange.x / 2 - moveGrid.Position.x;
                        moveGrid.Position =
                            new Vector3(
                                gridRootPos.x +
                                gridSize.x * gridRange.x -
                                gridRange.x / 2 - deltaPosX, pos.y, pos.z);
                    }
                    else
                    {
                        moveGrid.Position = new Vector3(pos.x + deltaPos.x, pos.y, pos.z);
                    }

                }
                // && (isAllMove || coord.x == pointDownCoord.x)
                else if (direction == EDirection.Vertial)
                {
                    moveGrid.Position = new Vector3(pos.x, pos.y, pos.z + deltaPos.z);
                    
                    if (moveGrid.Position.z >= gridRootPos.z +
                        gridSize.y * gridRange.y - gridRange.y / 2)
                    {
                        var deltaPosZ = moveGrid.Position.z - (gridRootPos.z +
                            gridSize.y * gridRange.y - gridRange.y / 2);
                        
                        moveGrid.Position = new Vector3(pos.x, pos.y, gridRootPos.z - gridRange.y / 2  + deltaPosZ);
                    }
                    else if (moveGrid.Position.z <=
                             gridRootPos.z - gridRange.y / 2)
                    {
                        var deltaPosZ = gridRootPos.z - gridRange.y / 2 - moveGrid.Position.z;
                        moveGrid.Position =
                            new Vector3(pos.x,
                                pos.y, gridRootPos.z +
                                gridSize.y * gridRange.y -
                                gridRange.y / 2 - deltaPosZ);
                    }
   
                }
                // && !isAllMove &&
                //coord.x - pointDownCoord.x == coord.y - pointDownCoord.y
                else if (direction == EDirection.XRight)
                {
                    //Log.Debug("XRight");
                    var isMinX = pointDownCoord.x < pointDownCoord.y;
                    var min = isMinX ? pointDownCoord.x : pointDownCoord.y;
                    var isMaxX = !isMinX;
                    var max = isMaxX ? gridSize.x - pointDownCoord.x - 1 : gridSize.y - pointDownCoord.y - 1;
                    
                    var leftCoord = new Vector2Int(pointDownCoord.x - min, pointDownCoord.y - min);
                    var rightCoord = new Vector2Int(pointDownCoord.x + max, pointDownCoord.y + max);
                    
                    var deltaY = deltaPos.x * Constant.Area.GridRange.y / Constant.Area.GridRange.x;
                    moveGrid.Position = new Vector3(pos.x + deltaPos.x, pos.y, pos.z + deltaY);

                    if (moveGrid.Position.z >= gridRootPos.z +
                        rightCoord.y * gridRange.y + gridRange.y / 2)
                    {
                        var deltaPosX = moveGrid.Position.x - (gridRootPos.x +
                            rightCoord.x * gridRange.x + gridRange.x / 2);

                        var deltaPosZ = moveGrid.Position.z -
                                        (gridRootPos.z + rightCoord.y * gridRange.y + gridRange.y / 2);
                        
                        moveGrid.Position = new Vector3(
                            gridRootPos.x + leftCoord.x * gridRange.x - gridRange.x / 2 + deltaPosX,
                            pos.y, gridRootPos.z + leftCoord.y * gridRange.y - gridRange.y / 2 + deltaPosZ);
                    }
                    else if (moveGrid.Position.z <= gridRootPos.z +
                        leftCoord.y * gridRange.y - gridRange.y / 2)
                    {
                        var deltaPosX = gridRootPos.x + leftCoord.x * gridRange.x - gridRange.x / 2 - moveGrid.Position.x;
                        var deltaPosZ = gridRootPos.z + leftCoord.y * gridRange.y - gridRange.y / 2 - moveGrid.Position.z;
                        
                        moveGrid.Position = new Vector3(
                            gridRootPos.x + rightCoord.x * gridRange.x + gridRange.x / 2 - deltaPosX,
                            pos.y, gridRootPos.z + rightCoord.y * gridRange.y + gridRange.y / 2 - deltaPosZ);
                    }
 

                }
                // && !isAllMove &&
                //coord.x - pointDownCoord.x == pointDownCoord.y - coord.y
                else if (direction == EDirection.XLeft)
                {
                    //Log.Debug("XLeft");
                    var isMinX = pointDownCoord.x < gridSize.y - pointDownCoord.y;
                    var min = isMinX ? pointDownCoord.x : gridSize.y - pointDownCoord.y - 1;
                    var isMaxX = !isMinX;
                    var max = isMaxX ? gridSize.x - pointDownCoord.x - 1 : pointDownCoord.y;
                    
                    // var deltaY = -deltaPos.x * Constant.Area.GridRange.y / Constant.Area.GridRange.x;
                    // moveGrid.Position = new Vector3(pos.x + deltaPos.x, pos.y, pos.z + deltaY);
                    
                    var deltaX = -deltaPos.z * Constant.Area.GridRange.y / Constant.Area.GridRange.x;
                    moveGrid.Position = new Vector3(pos.x + deltaX, pos.y, pos.z + deltaPos.z);
                    
                    var leftCoord = new Vector2Int(pointDownCoord.x - min, pointDownCoord.y + min);
                    var rightCoord = new Vector2Int(pointDownCoord.x + max, pointDownCoord.y - max);
                    
                    if (moveGrid.Position.z >= gridRootPos.z +
                        leftCoord.y * gridRange.y + gridRange.y / 2)
                    {
                        Log.Debug("aa");
                        var deltaPosX = (gridRootPos.x + leftCoord.x * gridRange.x - gridRange.x / 2) - moveGrid.Position.x;
                        var deltaPosZ = moveGrid.Position.z  - (gridRootPos.z + leftCoord.y * gridRange.y + gridRange.y / 2);
                        
                        moveGrid.Position = new Vector3(
                            gridRootPos.x + rightCoord.x * gridRange.x + gridRange.x / 2 - deltaPosX,
                            pos.y, gridRootPos.z + rightCoord.y * gridRange.y - gridRange.y / 2 + deltaPosZ);
                    }
                    else if (moveGrid.Position.z <= gridRootPos.z +
                        rightCoord.y * gridRange.y - gridRange.y / 2)
                    {
                        Log.Debug("bb");
                        var deltaPosX = moveGrid.Position.x - (gridRootPos.x + rightCoord.x * gridRange.x + gridRange.x / 2);
                        var deltaPosZ = (gridRootPos.z + rightCoord.y * gridRange.y - gridRange.y / 2) - moveGrid.Position.z;
                        
                        moveGrid.Position = new Vector3(
                            gridRootPos.x + leftCoord.x * gridRange.x - gridRange.x / 2 + deltaPosX,
                            pos.y, gridRootPos.z + leftCoord.y * gridRange.y + gridRange.y / 2 - deltaPosZ);
                    }
                    else
                    {
                        
                    }

                }
            }


        }
        
        public void ResetTmpUnitEntity()
        {
            if (TmpUnitEntity != null)
            {
                TmpUnitEntity.SetPosition(BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx);
                TmpUnitEntity.UnShowTags();
                BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx =
                    BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx;
                BattleManager.Instance.TempTriggerData.TempUnitMovePaths.Clear();
                BattleManager.Instance.RefreshEnemyAttackData();
            }
        }
        
    }
}
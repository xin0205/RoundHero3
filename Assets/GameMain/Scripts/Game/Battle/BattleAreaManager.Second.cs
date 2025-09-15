using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public partial class BattleAreaManager : Singleton<BattleAreaManager>
    {
        private int lastMoveCount = 0;
        
        private void CheckUpdateGrid(Vector2Int pointDownCoord, EDirection? direction, Vector3 deltaPos)
        {
            var isUpdateGrid = false;

            var gridRange = Constant.Area.GridRange;
            var gridSize = Constant.Area.GridSize;
            var curSelectCard = CardManager.Instance.GetCard(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx);
            var isAllMove = CardManager.Instance.Contain(curSelectCard.CardIdx, EBuffID.Spec_MoveAllGrid);

            var moveCount = 0;
            
            if (direction == EDirection.Horizonal)
            {
                var delta = 0;//deltaPos.x <= 0 ? -gridRange.x / 2 : gridRange.x / 2;
                var moveDelta = deltaPos.x <= 0 ? deltaPos.x % gridRange.x  : deltaPos.x % gridRange.x;
                moveCount = (int) ((deltaPos.x + delta) / gridRange.x) % gridSize.x;
                moveCount = moveCount >= 0 ? moveCount : gridSize.x + moveCount;

                if (lastMoveCount != moveCount)
                {

                    var curMoveCount = moveCount;
                    curMoveDelta = new Vector3(moveDelta, 0, 0);
                    Log.Debug("1:" + lastMoveCount + "-" + moveCount + "-" + moveCountDelta);
                    foreach (var kv in MoveGrids)
                    {
                        var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
                        // 
                        if (direction == EDirection.Horizonal && (isAllMove || coord.y == pointDownCoord.y))
                        {
                            var newCoordX = coord.x + curMoveCount;
                            newCoordX = newCoordX % gridSize.x;
                            var newCoord = new Vector2Int(newCoordX, coord.y);
                            kv.Value.GridPosIdx = GameUtility.GridCoordToPosIdx(newCoord);

                        }
                    }
                }
            }
            else if (direction == EDirection.Vertial)
            {
                var delta = 0;//deltaPos.z <= 0 ? -gridRange.y / 2 : gridRange.y / 2;
                var moveDelta = deltaPos.z <= 0 ? deltaPos.z % gridRange.y  : deltaPos.z % gridRange.y;
                moveCount = (int) ((deltaPos.z + delta) / gridRange.y) % gridSize.y;
                moveCount = moveCount >= 0 ? moveCount : gridSize.y + moveCount;

                if (lastMoveCount != moveCount)
                {

                    var curMoveCount = moveCount;
                    curMoveDelta = new Vector3(0, 0, moveDelta);
                    //Log.Debug("2:" + lastMoveCount + "-" + moveCount + "-" + moveCountDelta + "-" + moveDelta + "-" + deltaPos.z);
                    foreach (var kv in MoveGrids)
                    {
                        var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
                        if (direction == EDirection.Vertial && (isAllMove || coord.x == pointDownCoord.x))
                        {
                            var newCoordY = coord.y + curMoveCount;
                            newCoordY = newCoordY % gridSize.y;
                            var newCoord = new Vector2Int(coord.x, newCoordY);
                            kv.Value.GridPosIdx = GameUtility.GridCoordToPosIdx(newCoord);
                            
                        }
                    }
                }

            }
            else if (direction == EDirection.XRight && !isAllMove)
            {
                var isMinX = pointDownCoord.x < pointDownCoord.y;
                var min = isMinX ? pointDownCoord.x : pointDownCoord.y;
                var isMaxX = !isMinX;
                var max = isMaxX ? gridSize.x - pointDownCoord.x - 1 : gridSize.y - pointDownCoord.y - 1;
                var leftCoord = new Vector2Int(pointDownCoord.x - min, pointDownCoord.y - min);
                var rightCoord = new Vector2Int(pointDownCoord.x + max, pointDownCoord.y + max);
                var moveMaxCount = rightCoord.x - leftCoord.x + 1;
                

                var delta = 0;//deltaPos.x <= 0 ? -gridRange.x / 2 : gridRange.x / 2;
                var moveDelta = deltaPos.x <= 0 ? deltaPos.x % gridRange.x   : deltaPos.x % gridRange.x;
                moveCount = (int) ((deltaPos.x + delta) / gridRange.x) % moveMaxCount;
                moveCount = moveCount >= 0 ? moveCount : moveMaxCount + moveCount;

                if (lastMoveCount != moveCount)
                {
                    var curMoveCount = moveCount;
                    curMoveDelta = new Vector3(moveDelta, 0, 0);
                    Log.Debug("3:" + lastMoveCount + "-" + moveCount + "-" + moveCountDelta + "-" + moveMaxCount);
                    foreach (var kv in MoveGrids)
                    {
                        var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
                        if (direction == EDirection.XRight && coord.x - pointDownCoord.x == coord.y - pointDownCoord.y)
                        {
                            var newCoord = coord + new Vector2Int(curMoveCount, curMoveCount);

                            if (newCoord.y > rightCoord.y || newCoord.x > rightCoord.x)
                            {
                                newCoord = new Vector2Int(leftCoord.x + newCoord.x - rightCoord.x - 1,
                                    leftCoord.y + newCoord.y - rightCoord.y - 1);

                            }

                            kv.Value.GridPosIdx = GameUtility.GridCoordToPosIdx(newCoord);
                        }
                    }
                }
            }
            else if (direction == EDirection.XLeft && !isAllMove)
            {
                var isMinX = pointDownCoord.x < gridSize.y - pointDownCoord.y;
                var min = isMinX ? pointDownCoord.x : gridSize.y - pointDownCoord.y - 1;
                var isMaxX = !isMinX;
                var max = isMaxX ? gridSize.x - pointDownCoord.x - 1 : pointDownCoord.y;
                var leftCoord = new Vector2Int(pointDownCoord.x - min, pointDownCoord.y + min);
                var rightCoord = new Vector2Int(pointDownCoord.x + max, pointDownCoord.y - max);
                var moveMaxCount = rightCoord.x - leftCoord.x + 1;
                
                
                var delta = 0;//deltaPos.x <= 0 ? -gridRange.x / 2 : gridRange.x / 2;
                var moveDelta = deltaPos.z <= 0 ? deltaPos.z % gridRange.y  : deltaPos.z % gridRange.y;
                moveCount = (int) ((-deltaPos.z + delta) / gridRange.y) % moveMaxCount;
                moveCount = moveCount >= 0 ? moveCount : moveMaxCount + moveCount;

                Log.Debug("11:" + deltaPos.z +"-" + moveDelta);
                if (lastMoveCount != moveCount)
                {
                    curMoveDelta = new Vector3(0, 0, moveDelta);
                    var curMoveCount = moveCount;
                    Log.Debug("4:" + lastMoveCount + "-" + moveCount + "-" + moveDelta);
                    foreach (var kv in MoveGrids)
                    {
                        var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
                        if (direction == EDirection.XLeft && coord.x - pointDownCoord.x == pointDownCoord.y - coord.y)
                        {
                            var newCoord = coord + new Vector2Int(curMoveCount, -curMoveCount);
                            if (newCoord.y < rightCoord.y || newCoord.x > rightCoord.x)
                            {
                                
                                newCoord = new Vector2Int(leftCoord.x + newCoord.x - rightCoord.x - 1,
                                    leftCoord.y - (rightCoord.y - newCoord.y - 1));

                            }

                            kv.Value.GridPosIdx = GameUtility.GridCoordToPosIdx(newCoord);
                            
                        }
                    }
                }
            }

            if (lastMoveCount != moveCount)
            {
                
                lastMoveCount = moveCount;
                RefreshGirdEntities();
                RefreshObstacles();
                BattleManager.Instance.RefreshEnemyAttackData();
                ShowMoveUnitTags(true);
                //BattleEnemyManager.Instance.ShowEnemyRoutes();
            }

        }
        
        private void UpdateGrid()
        {
            IsMoveGrid = true;
            foreach (var kv in MoveGridPosIdxs)
            {
                var moveGrid = MoveGrids[kv.Key];
                if (moveGrid.GridPosIdx == kv.Value)
                {
                    IsMoveGrid = false;
                    break;
                }
            }

            if (IsMoveGrid)
            {
                BattleCardManager.Instance.RefreshCardConfirm();
            }
            

            foreach (var kv in MoveGrids)
            {
                kv.Value.Position = GameUtility.GridPosIdxToPos(kv.Value.GridPosIdx);
            }
            
            //BattleEnemyManager.Instance.UnShowEnemyRoutes();

        }
    }
}
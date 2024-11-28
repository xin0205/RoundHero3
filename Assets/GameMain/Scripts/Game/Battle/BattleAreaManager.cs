using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameFramework.Event;
using JetBrains.Annotations;
using UnityEngine;
using UnityGameFramework.Runtime;
using Random = System.Random;

namespace RoundHero
{
    public class TempExchangeGridData
    {
        public int GridPosIdx1 = -1;
        public int GridPosIdx2 = -1;

        
    }

    public class BattleAreaManager : Singleton<BattleAreaManager>
    {
        public Random Random;
        private int randomSeed;

        public Dictionary<int, BattleGridEntity> GridEntities = new();

        private bool pointerDownInRange;
        private Vector3 lastMousePosition;
        private Vector3 mousePositionDelta;
        //private Vector3 allMoveDelta;
        private Vector3 curMoveDelta;
        private Vector3 moveDelta;
        private int moveCountDelta;
        private Vector2Int pointDownCoord;
        private EDirection? MoveDirection = null;
        public bool IsMoveGrid = false;
        public Dictionary<int, IMoveGrid> MoveGrids = new();

        //public Dictionary<int, EGridType> GridTypes = new ();
        //public Dictionary<int, EGridType> CurObstacleMask = new ();

        public int CurPointGridPosIdx = -1;
        public TempExchangeGridData TempExchangeGridData = new();

        public void Init(int randomSeed)
        {
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowGridDetailEventArgs.EventId, OnShowGridDetail);
            GameEntry.Event.Subscribe(ClickGridEventArgs.EventId, OnClickGrid);

            BattleManager.Instance.BattleData.GridTypes.Clear();
            for (int i = 0; i < Constant.Area.GridSize.x * Constant.Area.GridSize.y; i++)
            {
                BattleManager.Instance.BattleData.GridTypes.Add(i, EGridType.Empty);
                //CurObstacleMask.Add(i, EGridType.Empty);
            }
            
            this.randomSeed = randomSeed;
            Random = new Random(randomSeed);

        }
        
        
        
        public async Task InitArea()
        {
            var initIdx = MathUtility.GetRandomNum(Constant.Area.ObstacleCount, 0,
                Constant.Area.GridSize.x * Constant.Area.GridSize.y, Random);


            var obstacleIdxs = new List<int>();
            for (int i = 0; i < Constant.Area.ObstacleCount; i++)
            {
                obstacleIdxs.Add(initIdx[i]);
            }

            for (int i = 0; i < Constant.Area.GridSize.x * Constant.Area.GridSize.y; i++)
            {
                var gridEntity = await GameEntry.Entity.ShowGridEntityAsync(i,
                    obstacleIdxs.Contains(i) ? EGridType.Obstacle : EGridType.Empty);

                GridEntities.Add(gridEntity.BattleGridEntityData.Id, gridEntity);

                if (gridEntity is IMoveGrid moveGrid)
                {
                    MoveGrids.Add(gridEntity.BattleGridEntityData.Id, moveGrid);
                }
            }

        }

        public void Destory()
        {
            GameEntry.Event.Unsubscribe(ShowGridDetailEventArgs.EventId, OnShowGridDetail);
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(ClickGridEventArgs.EventId, OnClickGrid);
            MoveGrids.Clear();
            foreach (var kv in GridEntities)
            {
                GameEntry.Entity.HideEntity(kv.Value);
                            
            }
            GridEntities.Clear();
        }



        public void Update()
        {
            if (BattleManager.Instance.BattleState == EBattleState.MoveGrid)
            {
                UpdateMoveGrid();
            }

        }

        private List<int> runPaths = new List<int>(32);
        public void OnShowGridDetail(object sender, GameEventArgs e)
        {
            var ne = e as ShowGridDetailEventArgs;
            if (ne.ShowState == EShowState.Show)
            {
                BattleAreaManager.Instance.CurPointGridPosIdx = ne.GridPosIdx;
            }
            else if (ne.ShowState == EShowState.Unshow)
            {
                BattleAreaManager.Instance.CurPointGridPosIdx = -1;
            }

            var soliderEntityID = BattleUnitManager.Instance.GetUnitID(ne.GridPosIdx,
                BattleManager.Instance.CurUnitCamp, ERelativeCamp.Us, EUnitRole.Staff);

            if (ne.ShowState == EShowState.Show)
            {
                if (soliderEntityID != -1)
                {
                    GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                }
            }
            
            var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(ne.GridPosIdx);

            // if (unit != null &&
            //     BattleManager.Instance.BattleState == EBattleState.ActionSelectUnit)
            // {
            //     if (ne.ShowState == EShowState.Show)
            //     {
            //         var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
            //         var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
            //         
            //         if (buffData.BuffTriggerType != EBuffTriggerType.AutoAttack)
            //         {
            //             return;
            //         }
            //
            //         BattleManager.Instance.TempTriggerData.UnitData =
            //             BattleUnitManager.Instance.GetBattleUnitData(unit);
            //
            //         BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.AutoAtk;
            //         
            //
            //     }
            //     else if (ne.ShowState == EShowState.Unshow)
            //     {
            //         //BattleManager.Instance.TempTriggerData.Reset();
            //     }
            //
            //     BattleManager.Instance.Refresh();
            //
            // }

            if (BattleManager.Instance.BattleState == EBattleState.UnitSelectGrid)
            {
                var unPlacePosIdxs = BattleBuffManager.Instance.GetUnPlacePosIdxs(GamePlayManager.Instance.GamePlayData);

                if (ne.ShowState == EShowState.Show &&
                    BattleManager.Instance.BattleData.GridTypes[ne.GridPosIdx] == EGridType.Empty &&
                    !unPlacePosIdxs.Contains(ne.GridPosIdx))
                {
                    BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.NewUnit;

                    var triggerBuffData = BattleManager.Instance.TempTriggerData.TriggerBuffData;
                    
                    var cardID = triggerBuffData.CardIdx;
                    var cardData = BattleManager.Instance.GetCard(cardID);
                    var cardEnergy = BattleCardManager.Instance.GetCardEnergy(cardID);
                    
                    var aroundHeroRange = GameUtility.GetRange(HeroManager.Instance.BattleHeroData.GridPosIdx, EActionType.Around, EUnitCamp.Player1);

                    if (HeroManager.Instance.BattleHeroData.HeroID == EHeroID.SubUnitCardEnergy)
                    {
                        if (aroundHeroRange.Contains(ne.GridPosIdx))
                        {
                            var values = HeroManager.Instance.GetHeroBuffValues();
                            cardEnergy += (int)values[0];
                            
                        }
                        
                    }
                    
                    BattleManager.Instance.TempTriggerData.UnitData = new Data_BattleSolider(
                        BattleUnitManager.Instance.GetTempID(), cardID,
                        ne.GridPosIdx, cardEnergy, BattleManager.Instance.CurUnitCamp,  cardData.FuneIdxs);
                    
                    //AddUnitState
                    //BattleUnitManager.Instance.TempUnitData.UnitData.AddState(EUnitState.AttackPassUs, 1);
                    
                    BattleManager.Instance.TempTriggerData.UnitData.CurHP =
                        BattleUnitManager.Instance.GetUnitHP(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx);

                    if (HeroManager.Instance.BattleHeroData.HeroID == EHeroID.AddUnitMaxHP)
                    {

                        if (aroundHeroRange.Contains(ne.GridPosIdx))
                        {
                            var values = HeroManager.Instance.GetHeroBuffValues();
                            BattleManager.Instance.TempTriggerData.UnitData.BaseMaxHP += (int)values[0];
                            BattleManager.Instance.TempTriggerData.UnitData.CurHP += (int)values[0];
                        }
                        
                    }

                    BlessManager.Instance.EachRoundFightCardAddLink(GamePlayManager.Instance.GamePlayData,
                        BattleManager.Instance.TempTriggerData.UnitData, EBlessID.EachRoundFightCardAddLinkReceive,
                        ELinkID.Link_Receive_Around_Us);
                    BlessManager.Instance.EachRoundFightCardAddLink(GamePlayManager.Instance.GamePlayData,
                        BattleManager.Instance.TempTriggerData.UnitData, EBlessID.EachRoundFightCardAddLinkSend,
                        ELinkID.Link_Send_Around_Us);


                    BattleManager.Instance.Refresh();

                    BattleEnemyManager.Instance.ShowEnemyRoutes();
                    GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
                }
                else if (ne.ShowState == EShowState.Unshow)
                {
                    if (BattleManager.Instance.TempTriggerData.UnitData != null &&
                        BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx == ne.GridPosIdx)
                    {
                        BattleManager.Instance.TempTriggerData.UnitData = null;
                        BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.Null;

                        BattleManager.Instance.Refresh();
                        GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
                    }

                    // if (BattleManager.Instance.BattleData.GridTypes[ne.GridPosIdx] == EGridType.TemporaryUnit)
                    // {
                    //     BattleManager.Instance.BattleData.GridTypes[ne.GridPosIdx] = EGridType.Empty;
                    //     
                    // }

                    BattleEnemyManager.Instance.UnShowEnemyRoutes();

                }
            }

            if (BattleManager.Instance.BattleState == EBattleState.MoveUnit ||
                BattleManager.Instance.BattleState == EBattleState.FuneMoveUnit)
            {

                var moveRanges =
                    BattleUnitManager.Instance.GetMoveRanges(BattleManager.Instance.TempTriggerData.UnitData.ID,
                        BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx);
                if (!moveRanges.Contains(ne.GridPosIdx))
                {
                    BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx =
                        BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx;
                    BattleManager.Instance.TempTriggerData.TempUnitMovePaths.Clear();
                    BattleManager.Instance.Refresh();
                    if (ne.ShowState == EShowState.Show)
                    {
                        BattleEnemyManager.Instance.ShowEnemyRoutes();
                    }
                    else if (ne.ShowState == EShowState.Unshow)
                    {
                        BattleEnemyManager.Instance.UnShowEnemyRoutes();
                    }

                    return;
                }

                if (ne.ShowState == EShowState.Show)
                {
                    BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.MoveUnit;

                    var tempUnitMovePaths = BattleManager.Instance.TempTriggerData.TempUnitMovePaths =
                        BattleFightManager.Instance.GetRunPaths(BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx,
                            ne.GridPosIdx, runPaths);
                    //var realTargetGridPosIdx = BattleManager.Instance.TempTriggerData.TargetGridPosIdx =
                        
                    BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx = tempUnitMovePaths[tempUnitMovePaths.Count - 1];
                    BattleManager.Instance.Refresh();
                    BattleEnemyManager.Instance.ShowEnemyRoutes();

                }
                else if (ne.ShowState == EShowState.Unshow)
                {
                    BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx =
                        BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx;
                    BattleManager.Instance.TempTriggerData.TempUnitMovePaths.Clear();
                    
                    BattleManager.Instance.Refresh();
                    BattleEnemyManager.Instance.UnShowEnemyRoutes();
                }

                

            }

            if (BattleManager.Instance.BattleState == EBattleState.ExchangeSelectGrid)
            {
                var card = BattleManager.Instance.GetCard(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx);
                if (CardManager.Instance.Contain(card.CardIdx, EBuffID.Spec_ExchangeGrid))
                {
                    var tempExchangeGridData = BattleAreaManager.Instance.TempExchangeGridData;
                    if (tempExchangeGridData.GridPosIdx1 != -1 && tempExchangeGridData.GridPosIdx2 == -1 &&
                        ne.GridPosIdx != tempExchangeGridData.GridPosIdx1)
                    {
                        if (ne.ShowState == EShowState.Show)
                        {
                            // var grid2 = GetGridEntityByGridPosIdx(tempExchangeGridData.GridPosIdx2);
                            // grid2.ShowBackupGrid(true);
                            ExchangeGrid(tempExchangeGridData.GridPosIdx1, ne.GridPosIdx);
                            BattleEnemyManager.Instance.ShowEnemyRoutes();
                        }
                        else if (ne.ShowState == EShowState.Unshow)
                        {
                            ExchangeGrid(tempExchangeGridData.GridPosIdx1, ne.GridPosIdx);
                            BattleEnemyManager.Instance.UnShowEnemyRoutes();
                        }
                    }
                }



            }

            if (BattleManager.Instance.BattleState == EBattleState.TacticSelectUnit)
            {
                if (ne.ShowState == EShowState.Show)
                {
                    var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
                    
                    var drBuff = BattleBuffManager.Instance.GetBuffData(buffStr);
                    List<ERelativeCamp> relativeCamps = drBuff.TriggerUnitCamps;
                    
                    unit = BattleUnitManager.Instance.GetUnitByGridPosIdxMoreCamps(ne.GridPosIdx,
                        BattleManager.Instance.CurUnitCamp,
                        relativeCamps);
                    if (unit == null)
                        return;
                    
                    BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.UseBuff;
                    BattleManager.Instance.TempTriggerData.CardEffectUnitID = unit.BattleUnit.ID;
                    
                    BattleManager.Instance.TempTriggerData.UnitData =
                        BattleUnitManager.Instance.GetBattleUnitData(unit);
                    
                    
                    var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
                    if (buffData.BuffStr == EBuffID.Spec_AttackUs.ToString())
                    {
                        var unitBuffDatas = BattleUnitManager.Instance.GetBuffDatas(unit.BattleUnit);
                        foreach (var unitBuffData in unitBuffDatas)
                        {
                            if (!(unitBuffData.BuffTriggerType == EBuffTriggerType.AutoAttack ||
                                  unitBuffData.BuffTriggerType == EBuffTriggerType.SelectUnit ||
                                  unitBuffData.BuffTriggerType == EBuffTriggerType.SelectGrid))
                            {
                                continue;
                            }

                            BattleManager.Instance.TempTriggerData.UnitData =
                                BattleUnitManager.Instance.GetBattleUnitData(unit);

                            if (unitBuffData.BuffTriggerType == EBuffTriggerType.AutoAttack)
                            {
                                BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.AutoAtk;
                            }   
                            else if (unitBuffData.BuffTriggerType == EBuffTriggerType.SelectUnit)
                            {
                                var attackRanges = BattleUnitManager.Instance.GetAttackRanges(unit.ID, ne.GridPosIdx);
                                ShowBackupGrids(attackRanges);
                            }
                            else if (unitBuffData.BuffTriggerType == EBuffTriggerType.SelectGrid)
                            {
                        
                            }
                    
                            BattleManager.Instance.Refresh();
                            
                        }
                    }
                    
                    
                    else
                    {
                        BattleManager.Instance.Refresh();
                        ShowBackupGrids(null);
                        //BattleBuffManager.Instance.UseBuff(ne.GridPosIdx, unit.ID);
                    }

                    
                }
                else if (ne.ShowState == EShowState.Unshow)
                {
                    ShowBackupGrids(null);
                    BattleManager.Instance.TempTriggerData.UnitData = null;
                    BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.Null;
                    BattleManager.Instance.TempTriggerData.CardEffectUnitID = -1;
                    //BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.CardID = -1;
                    BattleManager.Instance.Refresh();
                    
                }
                
            }
            
            
            
            if (BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit)
            {
                var attackRanges =
                    BattleUnitManager.Instance.GetAttackRanges(BattleManager.Instance.TempTriggerData.UnitData.ID,
                        BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx);
                if (attackRanges.Contains(ne.GridPosIdx))
                {
                    if (ne.ShowState == EShowState.Show)
                    {
                        BattleManager.Instance.TempTriggerData.TargetGridPosIdx = ne.GridPosIdx;
                        BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.ActiveAtk;
                        //BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.SelectHurtUnit;
                    }
                    else if (ne.ShowState == EShowState.Unshow)
                    {
                        BattleManager.Instance.TempTriggerData.TargetGridPosIdx = -1;
                    }
                    BattleManager.Instance.Refresh();
                }

                if (ne.ShowState == EShowState.Show)
                {
                    BattleEnemyManager.Instance.ShowEnemyRoutes();
                }
                else if (ne.ShowState == EShowState.Unshow)
                {
                    BattleEnemyManager.Instance.UnShowEnemyRoutes();
                }
            }
        }

        private Dictionary<int, int> moveGridPosIdx = new Dictionary<int, int>();

        public void UpdateMoveGrid()
        {
            if (!IsMoveGrid)
            {
                if (Input.GetMouseButtonDown(0))
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
                        BattleEnemyManager.Instance.ShowEnemyRoutes();

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
                            
                            moveGridPosIdx.Clear();
                            
                            foreach (var kv in MoveGrids)
                            {
                                var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
                                var pos = kv.Value.Position;

                                if (MoveDirection == EDirection.Horizonal && (isAllMove || coord.y == pointDownCoord.y))
                                {
                                    moveGridPosIdx.Add(kv.Key, kv.Value.GridPosIdx);
                                }
                                else if (MoveDirection == EDirection.Vertial && (isAllMove || coord.x == pointDownCoord.x))
                                {
                                    moveGridPosIdx.Add(kv.Key, kv.Value.GridPosIdx);
                                }
                                else if (MoveDirection == EDirection.XRight && !isAllMove &&
                                         coord.x - pointDownCoord.x == coord.y - pointDownCoord.y)
                                {
                                    moveGridPosIdx.Add(kv.Key, kv.Value.GridPosIdx);

                                }
                                else if (MoveDirection == EDirection.XLeft && !isAllMove &&
                                         coord.x - pointDownCoord.x == pointDownCoord.y - coord.y)
                                {
                                    moveGridPosIdx.Add(kv.Key, kv.Value.GridPosIdx);
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

            foreach (var kv in moveGridPosIdx)
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
        
        // private void MoveGrid(Vector2Int pointDownCoord, EDirection? direction, Vector3 deltaPos)
        // {
        //     var gridRootPos = AreaController.Instance.GridRoot.transform.position;
        //     var gridRange = Constant.Area.GridRange;
        //     var gridSize = Constant.Area.GridSize;
        //
        //     var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
        //     var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
        //     var isAllMove = buffData.BuffStr == EBuffID.Spec_MoveAllGrid.ToString();
        //
        //     foreach (var kv in MoveGrids)
        //     {
        //         var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
        //         var pos = kv.Value.Position;
        //
        //         if (direction == EDirection.Horizonal && (isAllMove || coord.y == pointDownCoord.y))
        //         {
        //
        //             if (kv.Value.Position.x >= gridRootPos.x +
        //                 gridSize.x * gridRange.x - gridRange.x / 2)
        //             {
        //                 var deltaPosX = kv.Value.Position.x - (gridRootPos.x +
        //                     gridSize.x * gridRange.x - gridRange.x / 2);
        //                 kv.Value.Position = new Vector3(gridRootPos.x - gridRange.x / 2 + deltaPosX, pos.y, pos.z);
        //             }
        //             else if (kv.Value.Position.x <=
        //                      gridRootPos.x - gridRange.x / 2)
        //             {
        //                 var deltaPosX = gridRootPos.x - gridRange.x / 2 - kv.Value.Position.x;
        //                 kv.Value.Position =
        //                     new Vector3(
        //                         gridRootPos.x +
        //                         gridSize.x * gridRange.x -
        //                         gridRange.x / 2 - deltaPosX, pos.y, pos.z);
        //             }
        //             else
        //             {
        //                 kv.Value.Position = new Vector3(pos.x + deltaPos.x, pos.y, pos.z);
        //             }
        //
        //         }
        //         else if (direction == EDirection.Vertial && (isAllMove || coord.x == pointDownCoord.x))
        //         {
        //             // if (kv.Value.Position.z >= gridRootPos.z +
        //             //     gridSize.y * gridRange.y - gridRange.y / 2)
        //             // {
        //             //     kv.Value.Position = new Vector3(pos.x, pos.y, gridRootPos.z - gridRange.y / 2 + deltaPos.z);
        //             // }
        //             // else if (kv.Value.Position.z <=
        //             //          gridRootPos.z - gridRange.y / 2)
        //             // {
        //             //     kv.Value.Position =
        //             //         new Vector3(pos.x,
        //             //             pos.y, gridRootPos.z +
        //             //             gridSize.y * gridRange.y -
        //             //             gridRange.y / 2 + deltaPos.z);
        //             // }
        //             // else
        //             // {
        //             //     kv.Value.Position = new Vector3(pos.x, pos.y, pos.z + deltaPos.z);
        //             // }
        //             kv.Value.Position = new Vector3(pos.x, pos.y, pos.z + deltaPos.z);
        //         }
        //         else if (direction == EDirection.XRight && !isAllMove &&
        //                  coord.x - pointDownCoord.x == coord.y - pointDownCoord.y)
        //         {
        //             var isMinX = pointDownCoord.x < pointDownCoord.y;
        //             var min = isMinX ? pointDownCoord.x : pointDownCoord.y;
        //             var isMaxX = !isMinX;
        //             var max = isMaxX ? gridSize.x - pointDownCoord.x - 1 : gridSize.y - pointDownCoord.y - 1;
        //
        //             var leftCoord = new Vector2Int(pointDownCoord.x - min, pointDownCoord.y - min);
        //             var rightCoord = new Vector2Int(pointDownCoord.x + max, pointDownCoord.y + max);
        //
        //             if (kv.Value.Position.z >= gridRootPos.z +
        //                 rightCoord.y * gridRange.y + gridRange.y / 2)
        //             {
        //                 kv.Value.Position = new Vector3(
        //                     gridRootPos.x + leftCoord.x * gridRange.x - gridRange.x / 2 + deltaPos.x,
        //                     pos.y, gridRootPos.z + leftCoord.y * gridRange.y - gridRange.y / 2 + deltaPos.z);
        //             }
        //             else if (kv.Value.Position.z <= gridRootPos.z +
        //                 leftCoord.y * gridRange.y - gridRange.y / 2)
        //             {
        //                 kv.Value.Position = new Vector3(
        //                     gridRootPos.x + rightCoord.x * gridRange.x + gridRange.x / 2 + deltaPos.x,
        //                     pos.y, gridRootPos.z + rightCoord.y * gridRange.y + gridRange.y / 2 + deltaPos.z);
        //             }
        //             else
        //             {
        //                 var deltaY = deltaPos.x * Constant.Area.GridRange.y / Constant.Area.GridRange.x;
        //                 kv.Value.Position = new Vector3(pos.x + deltaPos.x, pos.y, pos.z + deltaY);
        //             }
        //
        //         }
        //         else if (direction == EDirection.XLeft && !isAllMove &&
        //                  coord.x - pointDownCoord.x == pointDownCoord.y - coord.y)
        //         {
        //             var isMinX = pointDownCoord.x < gridSize.y - pointDownCoord.y;
        //             var min = isMinX ? pointDownCoord.x : gridSize.y - pointDownCoord.y - 1;
        //             var isMaxX = !isMinX;
        //             var max = isMaxX ? gridSize.x - pointDownCoord.x - 1 : pointDownCoord.y;
        //
        //             var leftCoord = new Vector2Int(pointDownCoord.x - min, pointDownCoord.y + min);
        //             var rightCoord = new Vector2Int(pointDownCoord.x + max, pointDownCoord.y - max);
        //             if (kv.Value.Position.z >= gridRootPos.z +
        //                 leftCoord.y * gridRange.y + gridRange.y / 2)
        //             {
        //                 kv.Value.Position = new Vector3(
        //                     gridRootPos.x + rightCoord.x * gridRange.x + gridRange.x / 2 + deltaPos.x,
        //                     pos.y, gridRootPos.z + rightCoord.y * gridRange.y - gridRange.y / 2 + deltaPos.z);
        //             }
        //             else if (kv.Value.Position.z <= gridRootPos.z +
        //                 rightCoord.y * gridRange.y - gridRange.y / 2)
        //             {
        //                 kv.Value.Position = new Vector3(
        //                     gridRootPos.x + leftCoord.x * gridRange.x - gridRange.x / 2 + deltaPos.x,
        //                     pos.y, gridRootPos.z + leftCoord.y * gridRange.y + gridRange.y / 2 + deltaPos.z);
        //             }
        //             else
        //             {
        //                 var deltaY = -deltaPos.x * Constant.Area.GridRange.y / Constant.Area.GridRange.x;
        //                 kv.Value.Position = new Vector3(pos.x + deltaPos.x, pos.y, pos.z + deltaY);
        //             }
        //
        //         }
        //     }
        //
        //
        // }

        private int lastMoveCount = 0;
        // private void CheckUpdateGrid(Vector2Int pointDownCoord, EDirection direction, Vector3 allDeltaPos)
        // {
        //     var isUpdateGrid = false;
        //
        //     var gridRange = Constant.Area.GridRange;
        //     var gridSize = Constant.Area.GridSize;
        //     var curSelectCard = CardManager.Instance.GetCard(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardID);
        //     var isAllMove = CardManager.Instance.Contain(curSelectCard.ID, EBuffID.Spec_MoveAllGrid);
        //
        //     var moveCount = 0;
        //     
        //     if (direction == EDirection.Horizonal)
        //     {
        //         var delta = allDeltaPos.x <= 0 ? -gridRange.x / 2 : gridRange.x / 2;
        //         moveCount = (int) ((allDeltaPos.x + delta) / gridRange.x) % gridSize.x;
        //         moveCount = moveCount >= 0 ? moveCount : gridSize.x + moveCount;
        //
        //         if (lastMoveCount != moveCount)
        //         {
        //             var curMoveCount = moveCount - lastMoveCount;
        //             Log.Debug("1lastMoveCount:" + lastMoveCount + "-" + moveCount);
        //             foreach (var kv in MoveGrids)
        //             {
        //                 var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
        //                 // 
        //                 if (direction == EDirection.Horizonal && (isAllMove || coord.y == pointDownCoord.y))
        //                 {
        //                     var newCoordX = coord.x + curMoveCount;
        //                     newCoordX = newCoordX % gridSize.x;
        //                     var newCoord = new Vector2Int(newCoordX, coord.y);
        //                     kv.Value.GridPosIdx = GameUtility.GridCoordToPosIdx(newCoord);
        //
        //                 }
        //             }
        //         }
        //     }
        //     else if (direction == EDirection.Vertial)
        //     {
        //         var delta = allDeltaPos.z <= 0 ? -gridRange.y / 2 : gridRange.y / 2;
        //         moveCount = (int) ((allDeltaPos.z + delta) / gridRange.y) % gridSize.y;
        //         moveCount = moveCount >= 0 ? moveCount : gridSize.y + moveCount;
        //
        //         if (lastMoveCount != moveCount)
        //         {
        //             curMoveDelta = Vector3.zero;
        //             var curMoveCount = moveCount;
        //             Log.Debug("2lastMoveCount:" + lastMoveCount + "-" + moveCount);
        //             foreach (var kv in MoveGrids)
        //             {
        //                 var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
        //                 if (direction == EDirection.Vertial && (isAllMove || coord.x == pointDownCoord.x))
        //                 {
        //                     var newCoordY = coord.y + curMoveCount;
        //                     newCoordY = newCoordY % gridSize.y;
        //                     var newCoord = new Vector2Int(coord.x, newCoordY);
        //                     kv.Value.GridPosIdx = GameUtility.GridCoordToPosIdx(newCoord);
        //                 }
        //             }
        //         }
        //
        //     }
        //     else if (direction == EDirection.XRight && !isAllMove)
        //     {
        //         Log.Debug("3lastMoveCount:" + lastMoveCount + "-" + moveCount);
        //         var isMinX = pointDownCoord.x < pointDownCoord.y;
        //         var min = isMinX ? pointDownCoord.x : pointDownCoord.y;
        //         var isMaxX = !isMinX;
        //         var max = isMaxX ? gridSize.x - pointDownCoord.x - 1 : gridSize.y - pointDownCoord.y - 1;
        //         var minCoord = new Vector2Int(pointDownCoord.x - min, pointDownCoord.y - min);
        //         var maxCoord = new Vector2Int(pointDownCoord.x + max, pointDownCoord.y + max);
        //
        //         var moveMaxCount = maxCoord.x - minCoord.x + 1;
        //         var delta = allDeltaPos.x <= 0 ? -gridRange.x / 2 : gridRange.x / 2;
        //         moveCount = (int) ((allDeltaPos.x + delta) / gridRange.x) % moveMaxCount;
        //         moveCount = moveCount >= 0 ? moveCount : moveMaxCount + moveCount;
        //
        //         if (lastMoveCount != moveCount)
        //         {
        //             //lastMoveDelta = allMoveDelta;
        //             var curMoveCount = moveCount - lastMoveCount;
        //             foreach (var kv in MoveGrids)
        //             {
        //                 var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
        //                 if (direction == EDirection.XRight && coord.x - pointDownCoord.x == coord.y - pointDownCoord.y)
        //                 {
        //
        //                     var newCoord = coord + new Vector2Int(curMoveCount, curMoveCount);
        //
        //                     if (newCoord.y > maxCoord.y || newCoord.x > maxCoord.x)
        //                     {
        //                         newCoord = new Vector2Int(minCoord.x + newCoord.x - maxCoord.x - 1,
        //                             minCoord.y + newCoord.y - maxCoord.y - 1);
        //
        //                     }
        //
        //                     kv.Value.GridPosIdx = GameUtility.GridCoordToPosIdx(newCoord);
        //                 }
        //             }
        //         }
        //     }
        //     else if (direction == EDirection.XLeft && !isAllMove)
        //     {
        //         Log.Debug("4lastMoveCount:" + lastMoveCount + "-" + moveCount);
        //         var isMinX = pointDownCoord.x < gridSize.y - pointDownCoord.y;
        //         var min = isMinX ? pointDownCoord.x : gridSize.y - pointDownCoord.y - 1;
        //         var isMaxX = !isMinX;
        //         var max = isMaxX ? gridSize.x - pointDownCoord.x - 1 : pointDownCoord.y;
        //
        //         var leftCoord = new Vector2Int(pointDownCoord.x - min, pointDownCoord.y + min);
        //         var rightCoord = new Vector2Int(pointDownCoord.x + max, pointDownCoord.y - max);
        //
        //         var moveMaxCount = rightCoord.x - leftCoord.x + 1;
        //         var delta = allDeltaPos.x <= 0 ? -gridRange.x / 2 : gridRange.x / 2;
        //         moveCount = (int) ((allDeltaPos.x + delta) / gridRange.x) % moveMaxCount;
        //         moveCount = moveCount >= 0 ? moveCount : moveMaxCount + moveCount;
        //
        //         
        //         if (lastMoveCount != moveCount)
        //         {
        //             //lastMoveDelta = allMoveDelta;
        //             var curMoveCount = moveCount - lastMoveCount;
        //             foreach (var kv in MoveGrids)
        //             {
        //                 var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
        //                 if (direction == EDirection.XLeft && coord.x - pointDownCoord.x == pointDownCoord.y - coord.y)
        //                 {
        //                     var newCoord = coord + new Vector2Int(curMoveCount, -curMoveCount);
        //                     if (newCoord.y < rightCoord.y || newCoord.x > rightCoord.x)
        //                     {
        //                         newCoord = new Vector2Int(leftCoord.x + newCoord.x - rightCoord.x - 1,
        //                             leftCoord.y - (rightCoord.y - newCoord.y - 1));
        //
        //                     }
        //
        //                     kv.Value.GridPosIdx = GameUtility.GridCoordToPosIdx(newCoord);
        //                 }
        //             }
        //         }
        //     }
        //
        //     if (lastMoveCount != moveCount)
        //     {
        //         lastMoveCount = moveCount;
        //         RefreshGirdEntities();
        //         RefreshObstacles();
        //         BattleManager.Instance.Refresh();
        //         BattleEnemyManager.Instance.ShowEnemyRoutes();
        //     }
        //
        // }
        
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
                BattleManager.Instance.Refresh();
                BattleEnemyManager.Instance.ShowEnemyRoutes();
            }

        }
        
        private void UpdateGrid()
        {
            IsMoveGrid = true;
            foreach (var kv in moveGridPosIdx)
            {
                var moveGrid = MoveGrids[kv.Key];
                if (moveGrid.GridPosIdx == kv.Value)
                {
                    IsMoveGrid = false;
                    break;
                }
            }
            

            foreach (var kv in MoveGrids)
            {
                kv.Value.Position = GameUtility.GridPosIdxToPos(kv.Value.GridPosIdx);
            }
            
            BattleEnemyManager.Instance.UnShowEnemyRoutes();

        }

        // private void UpdateGrid(Vector2Int pointDownCoord, EDirection? direction, Vector3 allDeltaPos)
        // {
        //     var gridRange = Constant.Area.GridRange;
        //     var gridSize = Constant.Area.GridSize;
        //     var curSelectCard = CardManager.Instance.GetCard(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardID);
        //     var isAllMove = CardManager.Instance.Contain(curSelectCard.ID, EBuffID.Spec_MoveAllGrid);
        //
        //     var delta = 0f;
        //     if (direction == EDirection.Horizonal)
        //     {
        //         //delta = allDeltaPos.x <= 0 ? -gridRange.x / 2 : gridRange.x / 2;
        //         var moveCount = (int) ((allDeltaPos.x + delta) / gridRange.x) % gridSize.x;
        //         moveCount = moveCount >= 0 ? moveCount : gridSize.x + moveCount;
        //
        //         foreach (var kv in MoveGrids)
        //         {
        //             var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
        //             // 
        //             if (direction == EDirection.Horizonal && (isAllMove || coord.y == pointDownCoord.y))
        //             {
        //                 var newCoordX = coord.x + moveCount;
        //                 newCoordX = newCoordX % gridSize.x;
        //                 var newCoord = new Vector2Int(newCoordX, coord.y);
        //                 var targetPos = GameUtility.GridCoordToPos(newCoord);
        //                 kv.Value.GridPosIdx = GameUtility.GridCoordToPosIdx(newCoord);
        //                 kv.Value.Position = targetPos;
        //
        //             }
        //         }
        //     }
        //     else if (direction == EDirection.Vertial)
        //     {
        //         //delta = allDeltaPos.z <= 0 ? -gridRange.y / 2 : gridRange.y / 2;
        //         var moveCount = (int) ((allDeltaPos.z + delta) / gridRange.y) % gridSize.y;
        //         moveCount = moveCount >= 0 ? moveCount : gridSize.y + moveCount;
        //
        //         foreach (var kv in MoveGrids)
        //         {
        //             var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
        //             if (direction == EDirection.Vertial && (isAllMove || coord.x == pointDownCoord.x))
        //             {
        //
        //                 var newCoordY = coord.y + moveCount;
        //                 newCoordY = newCoordY % gridSize.y;
        //                 var newCoord = new Vector2Int(coord.x, newCoordY);
        //                 var targetPos = GameUtility.GridCoordToPos(newCoord);
        //                 kv.Value.GridPosIdx = GameUtility.GridCoordToPosIdx(newCoord);
        //                 kv.Value.Position = targetPos;
        //             }
        //         }
        //
        //     }
        //     else if (direction == EDirection.XRight && !isAllMove)
        //     {
        //         var isMinX = pointDownCoord.x < pointDownCoord.y;
        //         var min = isMinX ? pointDownCoord.x : pointDownCoord.y;
        //         var isMaxX = !isMinX;
        //         var max = isMaxX ? gridSize.x - pointDownCoord.x - 1 : gridSize.y - pointDownCoord.y - 1;
        //         var minCoord = new Vector2Int(pointDownCoord.x - min, pointDownCoord.y - min);
        //         var maxCoord = new Vector2Int(pointDownCoord.x + max, pointDownCoord.y + max);
        //
        //         var moveMaxCount = maxCoord.x - minCoord.x + 1;
        //         //delta = allDeltaPos.x <= 0 ? -gridRange.x / 2 : gridRange.x / 2;
        //         var moveCount = (int) ((allDeltaPos.x + delta) / gridRange.x) % moveMaxCount;
        //         moveCount = moveCount >= 0 ? moveCount : moveMaxCount + moveCount;
        //
        //         if (moveCount > 0 && !IsMoveGrid)
        //         {
        //             IsMoveGrid = true;
        //         }
        //
        //         foreach (var kv in MoveGrids)
        //         {
        //             var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
        //             if (direction == EDirection.XRight && coord.x - pointDownCoord.x == coord.y - pointDownCoord.y)
        //             {
        //
        //                 var newCoord = coord + new Vector2Int(moveCount, moveCount);
        //
        //                 if (newCoord.y > maxCoord.y || newCoord.x > maxCoord.x)
        //                 {
        //                     newCoord = new Vector2Int(minCoord.x + newCoord.x - maxCoord.x - 1,
        //                         minCoord.y + newCoord.y - maxCoord.y - 1);
        //
        //                 }
        //
        //                 var targetPos = GameUtility.GridCoordToPos(newCoord);
        //                 kv.Value.GridPosIdx = GameUtility.GridCoordToPosIdx(newCoord);
        //                 kv.Value.Position = targetPos;
        //             }
        //         }
        //     }
        //     else if (direction == EDirection.XLeft && !isAllMove)
        //     {
        //         var isMinX = pointDownCoord.x < gridSize.y - pointDownCoord.y;
        //         var min = isMinX ? pointDownCoord.x : gridSize.y - pointDownCoord.y - 1;
        //         var isMaxX = !isMinX;
        //         var max = isMaxX ? gridSize.x - pointDownCoord.x - 1 : pointDownCoord.y;
        //
        //         var leftCoord = new Vector2Int(pointDownCoord.x - min, pointDownCoord.y + min);
        //         var rightCoord = new Vector2Int(pointDownCoord.x + max, pointDownCoord.y - max);
        //
        //         var moveMaxCount = rightCoord.x - leftCoord.x + 1;
        //         //delta = allDeltaPos.x <= 0 ? -gridRange.x / 2 : gridRange.x / 2;
        //         var moveCount = (int) ((allDeltaPos.x + delta) / gridRange.x) % moveMaxCount;
        //         moveCount = moveCount >= 0 ? moveCount : moveMaxCount + moveCount;
        //
        //
        //         foreach (var kv in MoveGrids)
        //         {
        //             var coord = GameUtility.GridPosIdxToCoord(kv.Value.GridPosIdx);
        //             if (direction == EDirection.XLeft && coord.x - pointDownCoord.x == pointDownCoord.y - coord.y)
        //             {
        //                 var newCoord = coord + new Vector2Int(moveCount, -moveCount);
        //                 if (newCoord.y < rightCoord.y || newCoord.x > rightCoord.x)
        //                 {
        //                     newCoord = new Vector2Int(leftCoord.x + newCoord.x - rightCoord.x - 1,
        //                         leftCoord.y - (rightCoord.y - newCoord.y - 1));
        //
        //                 }
        //
        //                 var targetPos = GameUtility.GridCoordToPos(newCoord);
        //                 kv.Value.GridPosIdx = GameUtility.GridCoordToPosIdx(newCoord);
        //                 kv.Value.Position = targetPos;
        //             }
        //         }
        //     }
        //
        //     //IsMoveGrid = false;
        //     RefreshGirdEntities();
        //     RefreshObstacles();
        //     BattleManager.Instance.Refresh();
        //     BattleEnemyManager.Instance.UnShowEnemyRoutes();
        // }

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
                         kv.Value is BattleHeroEntity)
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


        private void OnShowEntitySuccess(object sender, GameEventArgs e)
        {

        }



        public int GetGridEntityID(int gridPosIdx)
        {
            foreach (var kv in GridEntities)
            {
                if (kv.Value.BattleGridEntityData.GridPosIdx == gridPosIdx)
                {
                    return kv.Key;
                }
            }

            return -1;
        }

        public BattleGridEntity GetGridEntityByGridPosIdx(int gridPosIdx)
        {
            foreach (var kv in GridEntities)
            {
                if (kv.Value.BattleGridEntityData.GridPosIdx == gridPosIdx)
                {
                    return kv.Value;
                }
            }

            return null;
        }

        public void ResetMoveGrid()
        {
            if (IsMoveGrid)
            {
                //UpdateGrid(pointDownCoord, MoveDirection, -allMoveDelta);
                foreach (var kv in moveGridPosIdx)
                {
                    var moveGrid = MoveGrids[kv.Key];
                    moveGrid.GridPosIdx = kv.Value;
                    moveGrid.Position = GameUtility.GridPosIdxToPos(moveGrid.GridPosIdx);
                }
                
                
                IsMoveGrid = false;
            }

            ClearMoveGrid();
            
            RefreshGirdEntities();
            RefreshObstacles();
            BattleManager.Instance.Refresh();
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
            BattleEnemyManager.Instance.UnShowEnemyRoutes();
        }

        public async void OnClickGrid(object sender, GameEventArgs e)
        {
            if (BattleManager.Instance.CurUnitCamp != PlayerManager.Instance.PlayerData.UnitCamp)
                return;

            var ne = e as ClickGridEventArgs;

            var heroID = BattleUnitManager.Instance.GetUnitID(ne.GridPosIdx, BattleManager.Instance.CurUnitCamp,
                ERelativeCamp.Us, EUnitRole.Hero);
            var enemyEntityID = BattleUnitManager.Instance.GetUnitID(ne.GridPosIdx, BattleManager.Instance.CurUnitCamp,
                ERelativeCamp.Enemy);
            //var cardIndexs = BattleCardManager.Instance.GetCardIndexs(ne.GridPosIdx);
            var soliderEntityID = BattleUnitManager.Instance.GetUnitID(ne.GridPosIdx,
                BattleManager.Instance.CurUnitCamp, ERelativeCamp.Us, EUnitRole.Staff);

            if (soliderEntityID != -1)
            {
                GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            }

            if (BattleManager.Instance.BattleState == EBattleState.UseCard)
            {
                var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(ne.GridPosIdx);
                if (unit == null)
                    return;
                    
                // if (unit.BattleUnit.FuneCount(EFuneID.MoveInRound, true) > 0)
                // {
                //     var moveInRoundFune = unit.BattleUnit.GetFune(EFuneID.MoveInRound, true);
                //     if (moveInRoundFune != null && moveInRoundFune.Value > 0)
                //     {
                //         BattleManager.Instance.TempTriggerData.UnitData =
                //             BattleUnitManager.Instance.GetBattleUnitData(unit);
                //
                //         BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.MoveUnit;
                //         BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx =
                //             BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx;
                //
                //         var moveRanges = BattleUnitManager.Instance.GetMoveRanges(unit.ID, ne.GridPosIdx);
                //         ShowBackupGrids(moveRanges);
                //         BattleManager.Instance.BattleState = EBattleState.FuneMoveUnit;
                //     }
                // }
                // else 
            //     if (unit.BattleUnit.GetStateCount(EUnitState.ActiveAtk) > 0)
            //     {
            //         var battleUnitData = BattleUnitManager.Instance.GetBattleUnitData(unit);
            //         BattleUnitManager.Instance.GetBuffValue(GamePlayManager.Instance.GamePlayData, battleUnitData, out List<BuffValue> triggerBuffDatas);
            //         var triggerBuffData = triggerBuffDatas.Find(data => data.BuffData.BuffTriggerType == EBuffTriggerType.ActiveAttack);
            //         
            //         if (triggerBuffData != null)
            //         {
            //             BattleManager.Instance.TempTriggerData.UnitData = battleUnitData;
            //             BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.SelectHurtUnit;
            //             
            //             var buffData = triggerBuffData.BuffData;
            //             var attackRanges = GameUtility.GetRange(ne.GridPosIdx, buffData.TriggerRange,
            //                 unit.UnitCamp, buffData.TriggerUnitCamps, false);
            //             ShowBackupGrids(attackRanges);
            //             BattleManager.Instance.BattleState = EBattleState.SelectHurtUnit;
            //         }
            //
            //     }
            //     else if (unit.BattleUnit.GetStateCount(EUnitState.AutoAtk) > 0)
            //     {
            //         // BattleManager.Instance.TempTriggerData.UnitData =
            //         //     BattleUnitManager.Instance.GetBattleUnitData(unit);
            //         
            //         ShowBackupGrids(null);
            //             
            //         FightManager.Instance.SoliderAutoAttack();
            //             
            //         //BattleManager.Instance.Refresh();
            //         BattleEnemyManager.Instance.UnShowEnemyRoutes();
            //
            //         BattleManager.Instance.TempTriggerData.Reset();
            //         // BattleUnitManager.Instance.TempUnitData.TriggerType = ETempUnitType.Null;
            //         //BattleUnitManager.Instance.TempUnitData.UnitData = null;
            //         BattleManager.Instance.BattleState = EBattleState.UseCard;
            //         unit.BattleUnit.RemoveState(EUnitState.AutoAtk);
            //
            //     }
            //
            }
            else if (BattleManager.Instance.BattleState == EBattleState.UnitSelectGrid)
            {
                if (enemyEntityID != -1)
                {
                    GameEntry.UI.OpenMessage("AAA");
                    return;
                }
                
                if (soliderEntityID != -1)
                {
                    GameEntry.UI.OpenMessage("BBB");
                    return;
                }
                
                if (heroID != -1)
                {
                    GameEntry.UI.OpenMessage("CCC");
                    return;
                }
                
                BattleManager.Instance.PlaceUnitCard(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx, ne.GridPosIdx, BattleManager.Instance.CurUnitCamp);
                

            }
            else if (BattleManager.Instance.BattleState == EBattleState.TacticSelectUnit)
            {
                var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
                var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
                List<ERelativeCamp> relativeCamps = buffData.TriggerUnitCamps;
                
                var unit = BattleUnitManager.Instance.GetUnitByGridPosIdxMoreCamps(ne.GridPosIdx,
                    BattleManager.Instance.CurUnitCamp,
                    relativeCamps);
                
                if (unit == null)
                {
                    return;
                }

                
                if (buffData.BuffStr == EBuffID.Spec_MoveUs.ToString() || buffData.BuffStr == EBuffID.Spec_ActionEnemy.ToString())
                {
                    //|| unit.BattleUnit.GetStateCount(EUnitState.UnAction) > 0
                    // if ((unit.BattleUnit.GetStateCount(EUnitState.UnMove) > 0 ) &&
                    //     !GameUtility.ContainRoundState(GamePlayManager.Instance.GamePlayData, ECardID.RoundDeBuffUnEffect))
                    // {
                    //     return;
                    // }

                    BattleManager.Instance.TempTriggerData.UnitData =
                        BattleUnitManager.Instance.GetBattleUnitData(unit);
                    BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.MoveUnit;
                    BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx =
                        BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx;

                    var moveRanges = BattleUnitManager.Instance.GetMoveRanges(BattleManager.Instance.TempTriggerData.UnitData.ID, ne.GridPosIdx);
                    ShowBackupGrids(moveRanges);
                    BattleManager.Instance.BattleState = EBattleState.MoveUnit;
                }
                else if (buffData.BuffStr == EBuffID.Spec_AttackUs.ToString())
                {
                    var unitBuffDatas = BattleUnitManager.Instance.GetBuffDatas(unit.BattleUnit);

                    foreach (var unitBuffData in unitBuffDatas)
                    {
                        if (!(unitBuffData.BuffTriggerType == EBuffTriggerType.AutoAttack ||
                              unitBuffData.BuffTriggerType == EBuffTriggerType.SelectUnit ||
                              unitBuffData.BuffTriggerType == EBuffTriggerType.SelectGrid))
                        {
                            continue;
                        }

                        BattleManager.Instance.TempTriggerData.UnitData =
                            BattleUnitManager.Instance.GetBattleUnitData(unit);

                        if (unitBuffData.BuffTriggerType == EBuffTriggerType.AutoAttack)
                        {
                            BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.AutoAtk;
                            
                            BattleManager.Instance.Refresh();
                            BattleFightManager.Instance.SoliderAutoAttack();
                            BattleBuffManager.Instance.UseBuff(ne.GridPosIdx, unit.ID);
                            
                            ShowBackupGrids(null);
                            BattleEnemyManager.Instance.UnShowEnemyRoutes();
                            BattleManager.Instance.TempTriggerData.Reset();

                            BattleManager.Instance.BattleState = EBattleState.UseCard;
                        }   
                        else if (unitBuffData.BuffTriggerType == EBuffTriggerType.SelectUnit)
                        {
                            BattleManager.Instance.BattleState = EBattleState.SelectHurtUnit;
                            var attackRanges = BattleUnitManager.Instance.GetAttackRanges(unit.ID, ne.GridPosIdx);
                            ShowBackupGrids(attackRanges);
                        }
                        else if (unitBuffData.BuffTriggerType == EBuffTriggerType.SelectGrid)
                        {
                        
                        }
                    
                        
                    }
                    
                        
                    
                    
                }
                else
                {
                    //BattleBuffManager.Instance.TriggerBuff();
                    BattleBuffManager.Instance.UseBuff(ne.GridPosIdx);
                    
                    BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.Null;
                    BattleManager.Instance.TempTriggerData.CardEffectUnitID = -1;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.Clear();
                }
                // if (buffID == EBuffID.HurtUsDamage || buffID == EBuffID.MoveCountDamage ||
                //          buffID == EBuffID.UnitCountDamage)
                // {
                //     var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(ne.GridPosIdx,
                //         BattleManager.Instance.CurUnitCamp,
                //         ERelativeCamp.Enemy);
                //     if (unit == null)
                //     {
                //         return;
                //     }
                //
                //     BattleBuffManager.Instance.TriggerBuff();
                //     UseBuff(ne.GridPosIdx);
                //     
                //     BattleUnitManager.Instance.TempUnitData.TriggerType = ETempUnitType.Null;
                //     BattleUnitManager.Instance.TempUnitData.CardEffectUnitID = -1;
                //     BattleUnitManager.Instance.TempUnitData.TriggerBuffData.Clear();
                //
                //     
                //     
                //     
                //
                // }
                // else if (buffID == EBuffID.UnitAddCurHP || buffID == EBuffID.Link_Send_CrossLong_Us ||
                //          buffID == EBuffID.Link_Receive_XLong_Us || buffID == EBuffID.RemoveCardAddCurHP)
                // {
                //     var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(ne.GridPosIdx,
                //         BattleManager.Instance.CurUnitCamp,
                //         ERelativeCamp.Us);
                //     if (unit == null)
                //     {
                //         return;
                //     }
                //
                //     BattleBuffManager.Instance.TriggerBuff();
                //     UseBuff(ne.GridPosIdx);
                //     
                //     BattleUnitManager.Instance.TempUnitData.TriggerType = ETempUnitType.Null;
                //     BattleUnitManager.Instance.TempUnitData.CardEffectUnitID = -1;
                //     BattleUnitManager.Instance.TempUnitData.TriggerBuffData.Clear();
                //     
                // }
                // else if (buffID == EBuffID.RemoveDebuff)
                // {
                //     var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(ne.GridPosIdx,
                //         BattleManager.Instance.CurUnitCamp,
                //         ERelativeCamp.Us);
                //     if (unit == null)
                //     {
                //         return;
                //     }
                //
                //     BattleBuffManager.Instance.TriggerBuff();
                //     UseBuff(ne.GridPosIdx);
                //     
                //     BattleUnitManager.Instance.TempUnitData.TriggerType = ETempUnitType.Null;
                //     BattleUnitManager.Instance.TempUnitData.CardEffectUnitID = -1;
                //     BattleUnitManager.Instance.TempUnitData.TriggerBuffData.Clear();
                //     
                //
                // }

            }
            else if (BattleManager.Instance.BattleState == EBattleState.MoveUnit)
            {
                var moveRanges =
                    BattleUnitManager.Instance.GetMoveRanges(BattleManager.Instance.TempTriggerData.UnitData.ID,
                        BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx);
                if (moveRanges.Contains(ne.GridPosIdx))
                {
                    ShowBackupGrids(null);

                    var unit = BattleUnitManager.Instance.GetUnitByID(BattleManager.Instance.TempTriggerData.UnitData
                        .ID);

                    var moveActionData = BattleFightManager.Instance.RoundFightData.SoliderMoveDatas[unit.ID];

                    var time = unit.GetMoveTime(EUnitActionState.Run, moveActionData);
                    unit.Run(moveActionData);
                    GameUtility.DelayExcute(time, () =>
                    {
                        BattleManager.Instance.TempTriggerData.Reset();
                        BattleManager.Instance.Refresh();

                    });
                    
                    RefreshObstacles();
                    BattleEnemyManager.Instance.UnShowEnemyRoutes();
                    BattleBuffManager.Instance.UseBuff(ne.GridPosIdx, unit.BattleUnit.ID);
                    unit.BattleUnit.RoundMoveTimes += 1;
                }
            }
            else if (BattleManager.Instance.BattleState == EBattleState.FuneMoveUnit)
            {
                var moveRanges =
                    BattleUnitManager.Instance.GetMoveRanges(BattleManager.Instance.TempTriggerData.UnitData.ID,
                        BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx);
                if (moveRanges.Contains(ne.GridPosIdx))
                {
                    ShowBackupGrids(null);

                    var unit = BattleUnitManager.Instance.GetUnitByID(BattleManager.Instance.TempTriggerData.UnitData
                        .ID);
                    
                    var moveActionData = BattleFightManager.Instance.RoundFightData.SoliderMoveDatas[unit.ID];
                    

                    var time = unit.GetMoveTime(EUnitActionState.Run, moveActionData);
                    unit.Run(moveActionData);
                    GameUtility.DelayExcute(time, () => { BattleManager.Instance.Refresh(); });

                    RefreshObstacles();
                    BattleManager.Instance.Refresh();
                    BattleEnemyManager.Instance.UnShowEnemyRoutes();
                    
                    BattleManager.Instance.TempTriggerData.Reset();
                    BattleManager.Instance.BattleState = EBattleState.UseCard;
                    // var moveInRoundFune = unit.BattleUnit.GetFune(EFuneID.MoveInRound, true);
                    // if (moveInRoundFune != null)
                    // {
                    //     moveInRoundFune.Value -= 1;
                    // }
                }
            }
            else if (BattleManager.Instance.BattleState == EBattleState.ExchangeSelectGrid)
            {
                var card = BattleManager.Instance.GetCard(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx);
                if (CardManager.Instance.Contain(card.CardIdx, EBuffID.Spec_ExchangeGrid))
                {
                    var gridEntity = GetGridEntityByGridPosIdx(ne.GridPosIdx);
                    var tempExchangeGridData = BattleAreaManager.Instance.TempExchangeGridData;

                    if (ne.GridPosIdx == tempExchangeGridData.GridPosIdx1)
                    {
                        if (tempExchangeGridData.GridPosIdx2 != -1)
                        {
                            ExchangeGrid(tempExchangeGridData.GridPosIdx1, tempExchangeGridData.GridPosIdx2);
                            var grid2Entity = GetGridEntityByGridPosIdx(tempExchangeGridData.GridPosIdx2);
                            grid2Entity.ShowBackupGrid(false);
                            tempExchangeGridData.GridPosIdx2 = -1;
                        }

                        tempExchangeGridData.GridPosIdx1 = -1;
                        gridEntity.ShowBackupGrid(false);
                    }
                    else if (ne.GridPosIdx == tempExchangeGridData.GridPosIdx2)
                    {
                        if (tempExchangeGridData.GridPosIdx1 != -1)
                        {
                            ExchangeGrid(tempExchangeGridData.GridPosIdx1, tempExchangeGridData.GridPosIdx2);
                            var grid1Entity = GetGridEntityByGridPosIdx(tempExchangeGridData.GridPosIdx1);
                            grid1Entity.ShowBackupGrid(false);
                            tempExchangeGridData.GridPosIdx1 = -1;
                            BattleEnemyManager.Instance.UnShowEnemyRoutes();
                        }

                        tempExchangeGridData.GridPosIdx2 = -1;
                        gridEntity.ShowBackupGrid(false);
                    }
                    else if (tempExchangeGridData.GridPosIdx1 == -1)
                    {
                        tempExchangeGridData.GridPosIdx1 = ne.GridPosIdx;
                        gridEntity.ShowBackupGrid(true);
                    }
                    else if (tempExchangeGridData.GridPosIdx2 == -1)
                    {
                        tempExchangeGridData.GridPosIdx2 = ne.GridPosIdx;
                        gridEntity.ShowBackupGrid(true);
                        BattleEnemyManager.Instance.UnShowEnemyRoutes();
                    }

                }
            }
            else if (BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit)
            {
                var attackRanges = BattleUnitManager.Instance.GetAttackRanges(
                    BattleManager.Instance.TempTriggerData.UnitData.ID,
                    BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx);
                
                if (attackRanges.Contains(ne.GridPosIdx))
                {
                    ShowBackupGrids(null);
                    
                    var unitData = GameUtility.GetUnitDataByID(BattleManager.Instance.TempTriggerData.UnitData.ID, false);
                    if (unitData != null)
                    {
                        //unit.UnitState.RemoveState(EUnitState.ActiveAttack);
                    }
                    
                    BattleFightManager.Instance.SoliderActiveAttack();
                    BattleBuffManager.Instance.UseBuff(ne.GridPosIdx, unitData.ID);
                    unitData.RoundAttackTimes += 1;
                    //BattleManager.Instance.Refresh();
                    BattleEnemyManager.Instance.UnShowEnemyRoutes();

                    BattleManager.Instance.TempTriggerData.Reset();
                    // BattleUnitManager.Instance.TempUnitData.TriggerType = ETempUnitType.Null;
                    //BattleUnitManager.Instance.TempUnitData.UnitData = null;
                    BattleManager.Instance.BattleState = EBattleState.UseCard;
                    
                    //BattleManager.Instance.TempTriggerData.UnitData.RemoveState(EUnitState.ActiveAtk);
                   
                }
                
       
            }
        }

        public void ExchangeGrid(int gridPosIdx1, int gridPosIdx2)
        {
            if (gridPosIdx1 == -1 || gridPosIdx2 == -1)
                return;

            var pos1 = GameUtility.GridPosIdxToPos(gridPosIdx1);
            var pos2 = GameUtility.GridPosIdxToPos(gridPosIdx2);

            var unit1 = BattleUnitManager.Instance.GetUnitByGridPosIdx(gridPosIdx1);
            var unit2 = BattleUnitManager.Instance.GetUnitByGridPosIdx(gridPosIdx2);
            if (unit1 != null)
            {
                unit1.BattleUnit.GridPosIdx = gridPosIdx2;
                unit1.UpdatePos(pos2);
            }

            if (unit2 != null)
            {
                unit2.BattleUnit.GridPosIdx = gridPosIdx1;
                unit2.UpdatePos(pos1);
            }

            var gridProp1 = BattleGridPropManager.Instance.GetGridPropEntity(gridPosIdx1);
            var gridProp2 = BattleGridPropManager.Instance.GetGridPropEntity(gridPosIdx2);
            if (gridProp1 != null)
            {
                gridProp1.GridPosIdx = gridPosIdx2;
                gridProp1.Position = pos2;
            }

            if (gridProp2 != null)
            {
                gridProp2.GridPosIdx = gridPosIdx1;
                gridProp2.Position = pos1;
            }

            var grid1 = GetGridEntityByGridPosIdx(gridPosIdx1);
            var grid2 = GetGridEntityByGridPosIdx(gridPosIdx2);
            if (grid1 != null)
            {
                grid1.Show(true);
                //grid1.Position = pos2;
            }

            if (grid2 != null)
            {
                grid2.Show(true);
                //grid2.Position = pos1;
            }

            var gridType1 = GamePlayManager.Instance.GamePlayData.BattleData.GridTypes[gridPosIdx1];
            var gridType2 = GamePlayManager.Instance.GamePlayData.BattleData.GridTypes[gridPosIdx2];
            GamePlayManager.Instance.GamePlayData.BattleData.GridTypes[gridPosIdx1] = gridType2;
            GamePlayManager.Instance.GamePlayData.BattleData.GridTypes[gridPosIdx2] = gridType1;

            BattleManager.Instance.Refresh();
        }

        public List<IMoveGrid> GetUnits(int gridPosIdx)
        {
            var moveGirds = new List<IMoveGrid>();
            foreach (var kv in MoveGrids)
            {
                var moveGridType = kv.Value.GetType();
                if (kv.Value.GridPosIdx == gridPosIdx && (moveGridType == typeof(BattleMonsterEntity) ||
                                                          moveGridType == typeof(BattleSoliderEntity) ||
                                                          moveGridType == typeof(BattleHeroEntity)))
                {
                    moveGirds.Add(kv.Value);
                }
            }

            return moveGirds;
        }

        public void RefreshGirdEntities()
        {
            foreach (var kv in GridEntities)
            {
                kv.Value.Refresh();
            }
        }

        public void ShowBackupGrids(List<int> gridPosIdxs)
        {
            foreach (var kv in GridEntities)
            {
                if (gridPosIdxs == null)
                {
                    kv.Value.ShowBackupGrid(false);
                }
                else if (gridPosIdxs.Contains(kv.Value.GridPosIdx))
                {
                    kv.Value.ShowBackupGrid(true);
                }
                else
                {
                    kv.Value.ShowBackupGrid(false);
                }

            }
        }
        
        public async void PlaceUnitCard(int cardID, int gridPosIdx, EUnitCamp playerUnitCamp)
        {

            var unPlacePosIdxs = BattleBuffManager.Instance.GetUnPlacePosIdxs(GamePlayManager.Instance.GamePlayData);
            if (unPlacePosIdxs.Contains(gridPosIdx))
                return;
            
            // var battleSoliderEntity =
            //     await GameEntry.Entity.ShowBattleSoliderEntityAsync(new Data_BattleSolider(
            //         BattleUnitManager.Instance.GetTempID(), cardID,
            //         gridPosIdx, cardEnergy, playerUnitCamp, cardData.FuneIDs));
            
            if (BattleManager.Instance.CurUnitCamp == PlayerManager.Instance.PlayerData.UnitCamp)
            {
                BattleBuffManager.Instance.UseBuff(gridPosIdx);
                
            }
            
            var battleSoliderEntity =
                await GameEntry.Entity.ShowBattleSoliderEntityAsync(BattleManager.Instance.TempTriggerData.UnitData as Data_BattleSolider);
            
            BattleUnitManager.Instance.BattleUnitEntities.Add(
                battleSoliderEntity.BattleSoliderEntityData.BattleSoliderData.ID, battleSoliderEntity);

            if (battleSoliderEntity is IMoveGrid moveGrid)
            {
                BattleAreaManager.Instance.MoveGrids.Add(battleSoliderEntity.BattleSoliderEntityData.Id, moveGrid);
            }

            BattleSoliderManager.Instance.RefreshSoliderEntities();
            
            BattleManager.Instance.TempTriggerData.Reset();

            FuneManager.Instance.TriggerUnitUse();

            BattleAreaManager.Instance.RefreshObstacles();
            BattleManager.Instance.Refresh();

            BattleEnemyManager.Instance.UnShowEnemyRoutes();
            
            GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
        }

        
    }
}
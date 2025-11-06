using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFramework.Event;
using UGFExtensions.Await;
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

    public partial class BattleAreaManager : Singleton<BattleAreaManager>
    {
        public Random Random;
        private int randomSeed;

        public List<BattleGridEntity> GridEntities = new();
        public Dictionary<int, BattleGridEntity> GridEntitiesMap = new();

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
            pointerDownInRange = false;
            Subscribe();

            
            
            this.randomSeed = randomSeed;
            Random = new Random(randomSeed);

        }

        public async Task Start()
        {
            BattleManager.Instance.BattleData.GridTypes.Clear();
            for (int i = 0; i < Constant.Area.GridSize.x * Constant.Area.GridSize.y; i++)
            {
                BattleManager.Instance.BattleData.GridTypes.Add(i, EGridType.Empty);
                //CurObstacleMask.Add(i, EGridType.Empty);
            }
            //await GenerateArea();
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

        public void Destory()
        {
            Unsubscribe();
            MoveGrids.Clear();
            MoveGridPosIdxs.Clear();
            if (TmpUnitEntity != null && GameEntry.Entity.HasEntity(TmpUnitEntity.Id))
            {
                GameEntry.Entity.HideEntity(TmpUnitEntity);
                TmpUnitEntity = null;
            }
            
            foreach (var kv in GridEntities)
            {
                GameEntry.Entity.HideEntity(kv);
                            
            }
            GridEntities.Clear();
            GridEntitiesMap.Clear();
            BattleAreaManager.Instance.CurPointGridPosIdx = -1;
        }
        
        public void Subscribe()
        {
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowGridDetailEventArgs.EventId, OnShowGridDetail);
            GameEntry.Event.Subscribe(ClickGridEventArgs.EventId, OnClickGrid);
            GameEntry.Event.Subscribe(SelectGridEventArgs.EventId, OnSelectGrid);
        }

        public void Unsubscribe()
        {
            GameEntry.Event.Unsubscribe(ShowGridDetailEventArgs.EventId, OnShowGridDetail);
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(ClickGridEventArgs.EventId, OnClickGrid);
            GameEntry.Event.Unsubscribe(SelectGridEventArgs.EventId, OnSelectGrid);
        }


        public void Update()
        {
            if (BattleManager.Instance.BattleState == EBattleState.MoveGrid)
            {
                UpdateMoveGrid();
            }
            else
            {
                pointerDownInRange = false;
            }

        }

        private List<int> runPaths = new List<int>(32);
        public BattleUnitEntity TmpUnitEntity;
        public GridPropEntity TmpPropEntity;
        //private int tmpEntityIdx;
        public async void OnShowGridDetail(object sender, GameEventArgs e)
        {
            var ne = e as ShowGridDetailEventArgs;


            if (ne.ShowState == EShowState.Show)
            {
                await GameEntry.UI.OpenUIFormAsync(UIFormId.GridDescForm, new GridDescData()
                {
                    GridPosIdx = ne.GridPosIdx,
                });
                ShowAllGrid(true);
                BattleAreaManager.Instance.CurPointGridPosIdx = ne.GridPosIdx;
                BattleManager.Instance.TempTriggerData.TargetGridPosIdx = ne.GridPosIdx;
            }
            else if (ne.ShowState == EShowState.Unshow)
            {
                BattleUnitManager.Instance.UnShowTags();
                ShowAllGrid(false);
                BattleAreaManager.Instance.CurPointGridPosIdx = -1;
                BattleManager.Instance.TempTriggerData.TargetGridPosIdx = -1;
            }

            var soliderEntityID = BattleUnitManager.Instance.GetUnitIdx(ne.GridPosIdx,
                BattleManager.Instance.CurUnitCamp, ERelativeCamp.Us, EUnitRole.Staff);

            if (ne.ShowState == EShowState.Show)
            {
                if (soliderEntityID != -1)
                {
                    GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                }
            }

            if (ne.GridPosIdx == 23)
            {
                var a = 6;
            }
            

            // if (BattleManager.Instance.BattleState == EBattleState.UseCard)
            // {
            //     if (unit != null)
            //     {
            //         var movePaths = BattleFightManager.Instance.GetMovePaths(unit.Idx);
            //         if (movePaths != null)
            //         {
            //             if (ne.ShowState == EShowState.Show)
            //             {
            //                 unit.Root.position = GameUtility.GridPosIdxToPos(movePaths[movePaths.Count - 1]);
            //                 BattleValueManager.Instance.ShowDisplayValue(unit.Idx);
            //                 BattleAttackTagManager.Instance.ShowAttackTag(unit.Idx,
            //                     GameUtility.GridPosIdxToPos(movePaths[movePaths.Count - 1]));
            //             }
            //             else if (ne.ShowState == EShowState.Unshow)
            //             {
            //                 unit.Root.position = GameUtility.GridPosIdxToPos(movePaths[0]);
            //                 BattleValueManager.Instance.UnShowDisplayValues();
            //                 BattleAttackTagManager.Instance.UnShowAttackTags();
            //             }
            //         }
            //     }
            // }
            
            
            
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
                    BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.NewUnit;
                    var triggerBuffData = BattleManager.Instance.TempTriggerData.TriggerBuffData;
                    var cardIdx = triggerBuffData.CardIdx;
                    
                    BattleManager.Instance.TempTriggerData.UnitData = new Data_BattleSolider(
                        BattleUnitManager.Instance.GetIdx(), cardIdx,
                        ne.GridPosIdx, BattleManager.Instance.CurUnitCamp, BattleManager.Instance.BattleData.Round);
                    
                    var cardData = BattleManager.Instance.GetCard(cardIdx);
                    if (cardData != null)
                    {
                        var cardEnergy = BattleCardManager.Instance.GetCardEnergy(cardIdx,
                            BattleManager.Instance.TempTriggerData.UnitData.Idx);
                    
                        var aroundHeroRange = GameUtility.GetRange(HeroManager.Instance.BattleHeroData.GridPosIdx, EActionType.Direct82Short, EUnitCamp.Player1, null);

                        if (HeroManager.Instance.BattleHeroData.HeroID == EHeroID.SubUnitCardEnergy)
                        {
                            if (aroundHeroRange.Contains(ne.GridPosIdx))
                            {
                                var values = HeroManager.Instance.GetHeroBuffValues();
                                cardEnergy += (int)values[0];
                                
                            }
                            
                        }
                        //cardEnergy, ,  cardData.FuneIdxs
                        
                        //AddUnitState
                        //BattleUnitManager.Instance.TempUnitData.UnitData.AddState(EUnitState.AttackPassUs, 1);
                        
                        BattleManager.Instance.TempTriggerData.UnitData.CurHP =
                            BattleUnitManager.Instance.GetUnitHP(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx);
                        // BattleManager.Instance.TempTriggerData.UnitData.CurHP =
                        //     BattleManager.Instance.TempTriggerData.UnitData.MaxHP;
                        
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
                        
                        var battleSoliderData = (BattleManager.Instance.TempTriggerData.UnitData as Data_BattleSolider).Copy();
                        
                        Log.Debug("ShowBattleSolider:" + BattleManager.Instance.TempTriggerData.UnitData.Idx + "-" + ne.GridPosIdx);
                        //battleSoliderData.Idx = BattleUnitManager.Instance.GetIdx();
                        
                        HideTmpUnitEntity();
                            var tmpEntity =
                                await GameEntry.Entity.ShowBattleSoliderEntityAsync(battleSoliderData);
                            Log.Debug("ShowBattleSolider2:" + tmpEntity.UnitIdx + "-" + ne.GridPosIdx);
                            
                            if(BattleManager.Instance.TempTriggerData.UnitData == null || tmpEntity.UnitIdx < BattleManager.Instance.TempTriggerData.UnitData.Idx)
                            {
                                Log.Debug("HideEntity:");
                                BattleUnitManager.Instance.BattleUnitDatas.Remove(tmpEntity.BattleSoliderEntityData
                                    .BattleSoliderData.Idx);
                                // BattleUnitManager.Instance.BattleUnitEntities.Remove(tmpEntity.BattleSoliderEntityData
                                //     .BattleSoliderData.Idx);
                                GameEntry.Entity.HideEntity(tmpEntity);
                                BattleManager.Instance.RefreshEnemyAttackData();
                            }
                            else
                            {
                                Log.Debug("AddEntity：" + BattleManager.Instance.TempTriggerData.UnitData.Idx);
                                TmpUnitEntity = tmpEntity;
                                //TmpUnitEntity.ShowCollider(false);
                            
                                BattleUnitManager.Instance.BattleUnitDatas.Add(battleSoliderData.Idx, battleSoliderData);
                                BattleUnitManager.Instance.BattleUnitEntities.Add(
                                    TmpUnitEntity.BattleUnitData.Idx, TmpUnitEntity);
                            
                                BattleManager.Instance.RefreshEnemyAttackData();
                            
                                //showhurt
                                TmpUnitEntity.ShowHurtTags(TmpUnitEntity.UnitIdx);
                                //TmpUnitEntity.ShowTags(TmpUnitEntity.UnitIdx);
                            }
                        
                        

                        //BattleEnemyManager.Instance.ShowEnemyRoutes();
                        GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
                    }
                    
                    
                    
                }
                else if (ne.ShowState == EShowState.Unshow)
                {
                    Log.Debug("Unshow" + "-" + ne.GridPosIdx);
                    // && BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx == ne.GridPosIdx
                    if (BattleManager.Instance.TempTriggerData.UnitData != null)
                    {
                        Log.Debug("HideTmpUnitEntity");
                        BattleManager.Instance.TempTriggerData.UnitData = null;
                        BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.Empty;
                        HideTmpUnitEntity();
                        
                        Log.Debug("HideTmpUnitEntity2");
                        BattleManager.Instance.RefreshEnemyAttackData();
                        GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
                        Log.Debug("HideTmpUnitEntity3");
                    }

                    // if (BattleManager.Instance.BattleData.GridTypes[ne.GridPosIdx] == EGridType.TemporaryUnit)
                    // {
                    //     BattleManager.Instance.BattleData.GridTypes[ne.GridPosIdx] = EGridType.Empty;
                    //     
                    // }

                    //BattleEnemyManager.Instance.UnShowEnemyRoutes();

                }
            }
            
            if (BattleManager.Instance.BattleState == EBattleState.PropSelectGrid)
            {
                var unPlacePosIdxs = BattleBuffManager.Instance.GetUnPlacePosIdxs(GamePlayManager.Instance.GamePlayData);
                var triggerBuffData = BattleManager.Instance.TempTriggerData.TriggerBuffData;
                var cardIdx = triggerBuffData.CardIdx;
                var drCard = CardManager.Instance.GetCardTable(cardIdx);
                var isStayProp = false;
                var propID = -1;
                if (drCard != null)
                {
                    var buffStrList = drCard.BuffIDs[0].Split("_");
                    propID = int.Parse(buffStrList[1]);
                    isStayProp = BattleGridPropManager.Instance.IsStayProp(propID);
                }

                if (ne.ShowState == EShowState.Show &&
                    ((BattleManager.Instance.BattleData.GridTypes[ne.GridPosIdx] == EGridType.Empty &&
                    !unPlacePosIdxs.Contains(ne.GridPosIdx)) || isStayProp))
                {
                    BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.NewProp;

                    if (propID != -1)
                    {
                        BattleManager.Instance.TempTriggerData.PropData = new Data_GridProp(propID,
                            BattleUnitManager.Instance.GetIdx(),
                            ne.GridPosIdx, BattleManager.Instance.CurUnitCamp);
                        
                        var gridPropData = BattleManager.Instance.TempTriggerData.PropData.Copy();
                        
                        gridPropData.Idx = BattleUnitManager.Instance.GetIdx();
                        var tmpEntity =
                            await GameEntry.Entity.ShowBattleGridPropEntityAsync(gridPropData);

                        if (tmpEntity == null)
                        {
                            Log.Debug("tmpEntity == null");
                        }

                        if(BattleManager.Instance.TempTriggerData.PropData == null || tmpEntity.GridPropData.Idx < BattleManager.Instance.TempTriggerData.PropData.Idx)
                        {
                            BattleGridPropManager.Instance.GridPropDatas.Remove(tmpEntity.GridPropData.Idx);

                            GameEntry.Entity.HideEntity(tmpEntity);
                            BattleManager.Instance.RefreshEnemyAttackData();
                        }
                        else
                        {
                            TmpPropEntity = tmpEntity;
                            //TmpUnitEntity.ShowCollider(false);
                            
                            BattleGridPropManager.Instance.GridPropDatas.Add(gridPropData.Idx, gridPropData);
                            BattleGridPropManager.Instance.GridPropEntities.Add(
                                TmpPropEntity.GridPropData.Idx, TmpPropEntity);
                            
                            BattleManager.Instance.RefreshEnemyAttackData();

                        }
                        
                        GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
                    }

                }
                else if (ne.ShowState == EShowState.Unshow)
                {
                    if (BattleManager.Instance.TempTriggerData.PropData != null &&
                        BattleManager.Instance.TempTriggerData.PropData.GridPosIdx == ne.GridPosIdx)
                    {

                        BattleManager.Instance.TempTriggerData.PropData = null;
                        BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.Empty;
                        HideTmpPropEntity();
                        

                        BattleManager.Instance.RefreshEnemyAttackData();
                        GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
                        
                    }

                    // if (BattleManager.Instance.BattleData.GridTypes[ne.GridPosIdx] == EGridType.TemporaryUnit)
                    // {
                    //     BattleManager.Instance.BattleData.GridTypes[ne.GridPosIdx] = EGridType.Empty;
                    //     
                    // }

                    //BattleEnemyManager.Instance.UnShowEnemyRoutes();

                }
            }

            if (BattleManager.Instance.BattleState == EBattleState.MoveUnit ||
                BattleManager.Instance.BattleState == EBattleState.FuneMoveUnit)
            {

                
                var moveRanges =
                    BattleUnitManager.Instance.GetMoveRanges(BattleManager.Instance.TempTriggerData.UnitData.Idx,
                        BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx);
                if (!moveRanges.Contains(ne.GridPosIdx))
                {
                    Log.Debug("moveC" + ne.GridPosIdx + ne.ShowState);
                    BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx =
                        BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx;
                    BattleManager.Instance.TempTriggerData.TempUnitMovePaths.Clear();
                    BattleManager.Instance.RefreshEnemyAttackData();
                    if (ne.ShowState == EShowState.Show)
                    {
                        //BattleEnemyManager.Instance.ShowEnemyRoutes();
                    }
                    else if (ne.ShowState == EShowState.Unshow)
                    {
                        //BattleEnemyManager.Instance.UnShowEnemyRoutes();
                    }

                }
                else
                {
                    if (ne.ShowState == EShowState.Show)
                    {
                        TmpUnitEntity.SetPosition(ne.GridPosIdx);
                        Log.Debug("moveA" + ne.GridPosIdx);
                        BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.MoveUnit;
                        //BattleFightManager.Instance.RoundFightData.GamePlayData.LastBattleData.GridTypes
                        var tempUnitMovePaths = BattleManager.Instance.TempTriggerData.TempUnitMovePaths =
                            BattleFightManager.Instance.GetRunPaths(GamePlayManager.Instance.GamePlayData.BattleData.GridTypes, BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx,
                                ne.GridPosIdx, runPaths);
                        //var realTargetGridPosIdx = BattleManager.Instance.TempTriggerData.TargetGridPosIdx =
                        
                        BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx = tempUnitMovePaths[tempUnitMovePaths.Count - 1];
                        BattleManager.Instance.RefreshEnemyAttackData();
                        
                        TmpUnitEntity.ShowHurtTags(TmpUnitEntity.UnitIdx);
                        
                        // var triggerDataDict =
                        //     GameUtility.MergeDict(BattleFightManager.Instance.GetHurtDirectAttackDatas(TmpUnitEntity.UnitIdx),
                        //         BattleFightManager.Instance.GetHurtInDirectAttackDatas(TmpUnitEntity.UnitIdx));

                        // var idx = 0;
                        // var actionUnitList = new List<int>();
                        // foreach (var kv in triggerDataDict)
                        // {
                        //     foreach (var triggerData in kv.Value)
                        //     {
                        //         var actionUnitIdx = triggerData.ActionUnitIdx;
                        //         if(actionUnitList.Contains(actionUnitIdx))
                        //             continue;
                        //         actionUnitList.Add(actionUnitIdx);
                        //                 
                        //         var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                        //         if (actionUnit != null)
                        //         {
                        //             GameUtility.DelayExcute(0.25f * idx, () =>
                        //             {
                        //                 actionUnit.ShowTags(actionUnit.UnitIdx, true);
                        //             });
                        //             idx++;
                        //         }
                        //             //actionUnit.ShowTags(actionUnit.UnitIdx, true);
                        //         
                        //     }
                        // }
                        
                        
                    
                        //BattleEnemyManager.Instance.ShowEnemyRoutes();

                    }
                    else if (ne.ShowState == EShowState.Unshow)
                    {
                        TmpUnitEntity.SetPosition(BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx);
                        Log.Debug("moveB" + ne.GridPosIdx);
                        //TmpUnitEntity.UnShowTags();
                        ResetTmpUnitEntity();
                        
                    
                        //BattleEnemyManager.Instance.UnShowEnemyRoutes();
                    }
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
                            //BattleEnemyManager.Instance.ShowEnemyRoutes();
                        }
                        else if (ne.ShowState == EShowState.Unshow)
                        {
                            ExchangeGrid(tempExchangeGridData.GridPosIdx1, ne.GridPosIdx);
                            //BattleEnemyManager.Instance.UnShowEnemyRoutes();
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
                    
                    var relativeUnit = BattleUnitManager.Instance.GetUnitByGridPosIdxMoreCamps(ne.GridPosIdx,
                        BattleManager.Instance.CurUnitCamp,
                        relativeCamps);
                    if (relativeUnit != null)
                    {
                        BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.UseBuff;
                        //BattleManager.Instance.TempTriggerData.TargetGridPosIdx = relativeUnit.BattleUnitData.GridPosIdx;
                        
                        BattleManager.Instance.TempTriggerData.UnitData =
                            BattleUnitManager.Instance.GetBattleUnitData(relativeUnit);
                        
                        
                        var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
                        if (buffData.BuffStr == EBuffID.Spec_AttackUs.ToString())
                        {
                            if (BattleManager.Instance.TempTriggerData.UnitData.CurHP <= 0)
                            {
                                return;
                            }
                            
                            var actionTimes = relativeUnit.BattleUnitData.RoundAttackTimes + relativeUnit.BattleUnitData.RoundMoveTimes;
                            BattleCardManager.Instance.RefreshCurCardEnergy(actionTimes);
                            var unitBuffDatas = BattleUnitManager.Instance.GetBuffDatas(relativeUnit.BattleUnitData);
                            foreach (var unitBuffData in unitBuffDatas)
                            {
                                if (!(unitBuffData.BuffTriggerType == EBuffTriggerType.AutoAttack ||
                                      unitBuffData.BuffTriggerType == EBuffTriggerType.SelectUnit ||
                                      unitBuffData.BuffTriggerType == EBuffTriggerType.SelectGrid))
                                {
                                    continue;
                                }

                                BattleManager.Instance.TempTriggerData.UnitData =
                                    BattleUnitManager.Instance.GetBattleUnitData(relativeUnit);

                                if (unitBuffData.BuffTriggerType == EBuffTriggerType.AutoAttack)
                                {
                                    BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.AutoAtk;
                                }   
                                else if (unitBuffData.BuffTriggerType == EBuffTriggerType.SelectUnit)
                                {
                                    var attackRanges = BattleUnitManager.Instance.GetAttackRanges(relativeUnit.UnitIdx, ne.GridPosIdx);
                                    
                                    ShowBackupGrids(attackRanges);
                                }
                                else if (unitBuffData.BuffTriggerType == EBuffTriggerType.SelectGrid)
                                {
                                    var attackRanges = BattleUnitManager.Instance.GetAttackRanges(relativeUnit.UnitIdx, ne.GridPosIdx);
                                    ShowBackupGrids(attackRanges);
                                }
                        
                                BattleManager.Instance.RefreshEnemyAttackData();
                                
                            }
                        } 
                        else if (buffData.BuffStr == EBuffID.Spec_MoveUs.ToString())
                        {
                            if (BattleManager.Instance.TempTriggerData.UnitData.CurHP <= 0)
                            {
                                return;
                            }
                            
                            var actionTimes = relativeUnit.BattleUnitData.RoundAttackTimes + relativeUnit.BattleUnitData.RoundMoveTimes;
                            BattleCardManager.Instance.RefreshCurCardEnergy(actionTimes);
                        }
                        
                        else
                        {
                            BattleManager.Instance.RefreshEnemyAttackData();
                            ShowBackupGrids(null);
                            //BattleBuffManager.Instance.UseBuff(ne.GridPosIdx, unit.ID);
                        }

                    }

                }
                else if (ne.ShowState == EShowState.Unshow)
                {
                    ShowBackupGrids(null);
                    BattleManager.Instance.TempTriggerData.UnitData = null;
                    BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.Empty;
                    //BattleManager.Instance.TempTriggerData.TargetGridPosIdx = -1;
                    //BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.CardID = -1;
                    BattleManager.Instance.RefreshEnemyAttackData();
                    
                }
                
            }

            if (BattleManager.Instance.BattleState == EBattleState.TacticSelectGrid)
            {
                if (ne.ShowState == EShowState.Show)
                {
                    //BattleManager.Instance.TempTriggerData.TargetGridPosIdx = ne.GridPosIdx;
                }
                else
                {
                    //BattleManager.Instance.TempTriggerData.TargetGridPosIdx = -1;
                }
                BattleManager.Instance.RefreshEnemyAttackData();
                
                var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
                var drBuff = BattleBuffManager.Instance.GetBuffData(buffStr);
                     
                var range = GameUtility.GetRange(ne.GridPosIdx, drBuff.TriggerRange, BattleManager.Instance.CurUnitCamp, drBuff.TriggerUnitCamps);
                if (BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr2 != string.Empty)
                {
                    var drBuff2 = BattleBuffManager.Instance.GetBuffData(BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr2);
                    range.AddRange(GameUtility.GetRange(ne.GridPosIdx, drBuff2.TriggerRange, BattleManager.Instance.CurUnitCamp, drBuff2.TriggerUnitCamps));

                }
                
                
                foreach (var gridPosIdx in range)
                {
                    var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(gridPosIdx);
                    
                    if (ne.ShowState == EShowState.Show)
                    {
                        unit.ShowTacticHurtDisplayValues(unit.UnitIdx);
                        unit.ShowTacticHurtDisplayIcons(unit.UnitIdx);
                        unit.ShowTacticHurtAttackTag(unit.UnitIdx, Constant.Battle.UnUnitTriggerIdx);
                    }
                    else
                    {
                        unit.UnShowTags();
                    }
                }
                
                
            }
            
            if (BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit)
            {
                var attackRanges =
                    BattleUnitManager.Instance.GetAttackRanges(BattleManager.Instance.TempTriggerData.UnitData.Idx,
                        BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx);
                
                if (attackRanges.Contains(ne.GridPosIdx))
                {
                    var attackUnitEntity = BattleUnitManager.Instance.GetUnitByIdx(BattleManager.Instance.TempTriggerData.UnitData.Idx);
                    var effectUnitEntity = BattleUnitManager.Instance.GetUnitByGridPosIdx(ne.GridPosIdx);
                    if (ne.ShowState == EShowState.Show)
                    {
                        //BattleManager.Instance.TempTriggerData.TargetGridPosIdx = ne.GridPosIdx;
                        BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.ActiveAtk;
                        
                        BattleManager.Instance.RefreshEnemyAttackData();
                        
                        if (attackUnitEntity != null)
                        {
                            //attackUnitEntity.transform.LookAt(effectUnitEntity.transform.position);
                            attackUnitEntity.ShowHurtTags(attackUnitEntity.UnitIdx, effectUnitEntity.UnitIdx);
                        }
                        
                        if (effectUnitEntity != null)
                        {
                            //, BattleManager.Instance.TempTriggerData.UnitData.Idx
                            //effectUnitEntity.ShowHurtTags(effectUnitEntity.UnitIdx);
                        }
                        
                        if (attackUnitEntity != null && effectUnitEntity != null)
                        {
                            attackUnitEntity.LookAt(effectUnitEntity.transform.position);
                            
                        }
                        
                        //BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.SelectHurtUnit;
                    }
                    else if (ne.ShowState == EShowState.Unshow)
                    {
                        //BattleManager.Instance.TempTriggerData.TargetGridPosIdx = -1;
                        
                        BattleManager.Instance.RefreshEnemyAttackData();
                        if (attackUnitEntity != null)
                        {
                            attackUnitEntity.UnShowTags();
                        }
                        
                        if (effectUnitEntity != null)
                        {
                            effectUnitEntity.UnShowTags();
                        }
                    }
                    
                    
                }

                if (ne.ShowState == EShowState.Show)
                {
                    //BattleEnemyManager.Instance.ShowEnemyRoutes();
                }
                else if (ne.ShowState == EShowState.Unshow)
                {
                    //BattleEnemyManager.Instance.UnShowEnemyRoutes();
                }
            }

            if (ne.ShowState == EShowState.Unshow)
            {
                BattleUnitManager.Instance.UnShowTags();
            }
            
            if (!pointerDownInRange)
            {
                var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(ne.GridPosIdx);

                var gridEntity = GetGridEntityByGridPosIdx(ne.GridPosIdx);
                if (ne.ShowState == EShowState.Show)
                {
                    //Log.Debug("4 Enter");
                    gridEntity.OnPointerEnter();
                }
                else if (ne.ShowState == EShowState.Unshow)
                {
                    //Log.Debug("4 Exit");
                    gridEntity.OnPointerExit();
                }

                if (unit != null)
                {
                    if (ne.ShowState == EShowState.Show)
                    {
                        //Log.Debug("1 Enter");
                        unit.OnPointerEnter();
                        if (unit.BattleUnitData.Exist() && !unit.IsMove)
                        {
                            if (BattleManager.Instance.BattleState == EBattleState.TacticSelectUnit)
                            {
                                var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
                                var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
                                if (buffData.BuffStr == EBuffID.Spec_AttackUs.ToString())
                                {
                                    unit.ShowTagsWithFlyUnitIdx(unit.UnitIdx, true);
                                }
                                else
                                {
                                    unit.ShowTacticHurtDisplayValues(unit.UnitIdx);
                                    unit.ShowTacticHurtDisplayIcons(unit.UnitIdx);
                                }
                                
                            }
                            else if(BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit)
                            {
                                var attackUnit = BattleUnitManager.Instance.GetUnitByIdx(BattleManager.Instance.TempTriggerData.UnitData.Idx);
                                if (attackUnit != null)
                                {
                                    attackUnit.ShowTagsWithFlyUnitIdx(attackUnit.UnitIdx, true);
                                }
                                
                                //unit.ShowHurtTags(unit.UnitIdx, BattleManager.Instance.TempTriggerData.UnitData.Idx);
                            }
                            // else if (BattleManager.Instance.BattleState == EBattleState.TacticSelectUnit)
                            // {
                            //     unit.ShowHurtTags(unit.UnitIdx, Constant.Battle.CardTriggerIdx);
                            //
                            // }
                            else if(BattleManager.Instance.BattleState == EBattleState.MoveUnit)
                            {
                                var attackUnit = BattleUnitManager.Instance.GetUnitByIdx(BattleManager.Instance.TempTriggerData.UnitData.Idx);
                                if (attackUnit != null)
                                {
                                    attackUnit.ShowTagsWithFlyUnitIdx(attackUnit.UnitIdx, true);
                                }
                                
                                //unit.ShowHurtTags(unit.UnitIdx, BattleManager.Instance.TempTriggerData.UnitData.Idx);
                            }
                            else if (BattleManager.Instance.BattleState == EBattleState.UnitSelectGrid)
                            {
                                // var hurtTriggerDataDict =
                                //     GameUtility.MergeDict(BattleFightManager.Instance.GetHurtDirectAttackDatas(unit.UnitIdx),
                                //         BattleFightManager.Instance.GetHurtInDirectAttackDatas(unit.UnitIdx));
                                //
                                // var idx = 0;
                                // var actionUnitList = new List<int>();
                                // foreach (var kv in hurtTriggerDataDict)
                                // {
                                //     foreach (var triggerData in kv.Value)
                                //     {
                                //         var actionUnitIdx = triggerData.ActionUnitIdx;
                                //         if(actionUnitList.Contains(actionUnitIdx))
                                //             continue;
                                //         actionUnitList.Add(actionUnitIdx);
                                //         
                                //         var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                                //         if (actionUnit != null)
                                //         {
                                //             // GameUtility.DelayExcute(0.25f * idx, () =>
                                //             // {
                                //             //     
                                //             // });
                                //             actionUnit.ShowTags(actionUnit.UnitIdx, true);
                                //             idx++;
                                //         }
                                //     }
                                // }
                                
                                var triggerDataDict =
                                    GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(unit.UnitIdx),
                                        BattleFightManager.Instance.GetInDirectAttackDatas(unit.UnitIdx));
                                var idx = 0;
                                var actionUnitList = new List<int>();
                                foreach (var kv in triggerDataDict)
                                {
                                    foreach (var triggerData in kv.Value)
                                    {
                                        var actionUnitIdx = triggerData.ActionUnitIdx;
                                        if(actionUnitList.Contains(actionUnitIdx))
                                            continue;
                                        actionUnitList.Add(actionUnitIdx);
                                        
                                        var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                                        if (actionUnit != null)
                                        {
                                            // GameUtility.DelayExcute(0.25f * idx, () =>
                                            // {
                                            //     
                                            // });
                                            //actionUnit.ShowTagsWithFlyUnitIdx(actionUnit.UnitIdx, true);
                                            actionUnit.ShowFlyUnitIdx(actionUnit.UnitIdx);
                                            idx++;
                                        }
                                    }
                                }
                                
                            }
                            else
                            {
                                //showhurt
                                //unit.ShowHurtTags(unit.UnitIdx);
                                unit.ShowTagsWithFlyUnitIdx(unit.UnitIdx);
                                //unit.ShowHurtTags(unit.UnitIdx);
                            }
                        }
                        
                
                    }
                    else if (ne.ShowState == EShowState.Unshow)
                    {
                        //Log.Debug("1 Exit");
                        unit.OnPointerExit();
                        //unit.UnShowTags();
                    }
                }

            }
            
            

        }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
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
        
        

        public Dictionary<int, int> MoveGridPosIdxs = new Dictionary<int, int>();

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
        
       
        
        
        
        
        public void ShowMoveUnitTags(bool isShow)
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
                    
                    kv.Value.ShowTagsWithFlyUnitIdx(kv.Value.UnitIdx, false);
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


        private void OnShowEntitySuccess(object sender, GameEventArgs e)
        {

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

        public void OnSelectGrid(object sender, GameEventArgs e)
        {
            var ne = e as SelectGridEventArgs;
            if(GridEntitiesMap.ContainsKey(ne.GridPosIdx))
            {
                GridEntitiesMap[ne.GridPosIdx].ShowSelectGrid(ne.IsSelect);
            }
        }

        public async void OnClickGrid(object sender, GameEventArgs e)
        {
            if (BattleManager.Instance.CurUnitCamp != PlayerManager.Instance.PlayerData.UnitCamp)
                return;
            
            BattleUnitManager.Instance.UnShowTags();

            var ne = e as ClickGridEventArgs;

            var heroID = BattleUnitManager.Instance.GetUnitIdx(ne.GridPosIdx, BattleManager.Instance.CurUnitCamp,
                ERelativeCamp.Us, EUnitRole.Core);
            var enemyEntityID = BattleUnitManager.Instance.GetUnitIdx(ne.GridPosIdx, BattleManager.Instance.CurUnitCamp,
                ERelativeCamp.Enemy);
            //var cardIndexs = BattleCardManager.Instance.GetCardIndexs(ne.GridPosIdx);
            var soliderEntityID = BattleUnitManager.Instance.GetUnitIdx(ne.GridPosIdx,
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
                    
                
            }
            else if (BattleManager.Instance.BattleState == EBattleState.UnitSelectGrid)
            {
                if(TutorialManager.Instance.Switch_UseUnitCard(ne.GridPosIdx) == ETutorialState.UnMatch)
                    return;
                
                // if (enemyEntityID != -1)
                // {
                //     GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_UnPlaceUnit);
                //     return;
                // }
                //
                // if (soliderEntityID != -1)
                // {
                //     GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_UnPlaceUnit);
                //     return;
                // }
                //
                // if (heroID != -1)
                // {
                //     GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_UnPlaceUnit);
                //     return;
                // }
                
                HideTmpUnitEntity();
                
                BattleManager.Instance.PlaceUnitCard(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx, ne.GridPosIdx, BattleManager.Instance.CurUnitCamp);
                

            }
            else if (BattleManager.Instance.BattleState == EBattleState.PropSelectGrid)
            {
                var isStayProp = false;
                var prop = BattleGridPropManager.Instance.GetGridProp(ne.GridPosIdx);
                if (prop != null)
                {
                   
                    isStayProp = BattleGridPropManager.Instance.IsStayProp(prop.GridPropID);
                }
                
                if (enemyEntityID != -1 && !isStayProp)
                {
                    GameEntry.UI.OpenMessage("AAA");
                    return;
                }
                
                if (soliderEntityID != -1 && !isStayProp)
                {
                    GameEntry.UI.OpenMessage("BBB");
                    return;
                }
                
                if (heroID != -1 && !isStayProp)
                {
                    GameEntry.UI.OpenMessage("CCC");
                    return;
                }
                
                HideTmpPropEntity();
                
                BattleManager.Instance.PlaceProp(prop.GridPropID, ne.GridPosIdx, BattleManager.Instance.CurUnitCamp);
                

            }
            else if (BattleManager.Instance.BattleState == EBattleState.TacticSelectUnit)
            {
                if(TutorialManager.Instance.Switch_SelectMoveUnit(ne.GridPosIdx) == ETutorialState.UnMatch &&
                   TutorialManager.Instance.Switch_SelectAttackUnit(ne.GridPosIdx) == ETutorialState.UnMatch)
                    return;
                
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

                
                if (buffData.BuffStr == EBuffID.Spec_MoveUs.ToString() || buffData.BuffStr == EBuffID.Spec_MoveEnemy.ToString())
                {
                    //|| unit.BattleUnit.GetStateCount(EUnitState.UnAction) > 0
                    // if ((unit.BattleUnit.GetStateCount(EUnitState.UnMove) > 0 ) &&
                    //     !GameUtility.ContainRoundState(GamePlayManager.Instance.GamePlayData, ECardID.RoundDeBuffUnEffect))
                    // {
                    //     return;
                    // }

                    if (unit.BattleUnitData.CurHP <= 0)
                    {
                        GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_NoHPToMove);
                        return;
                    }

                    var relativeCamp =
                        GameUtility.GetRelativeCamp(PlayerManager.Instance.PlayerData.UnitCamp, unit.UnitCamp);
                    if (!buffData.TriggerUnitCamps.Contains(relativeCamp))
                    {
                        return;
                    }

                    TmpUnitEntity = unit;
                    BattleManager.Instance.TempTriggerData.UnitData =
                        BattleUnitManager.Instance.GetBattleUnitData(unit);
                    BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.MoveUnit;
                    BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx =
                        BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx;

                    var moveRanges = BattleUnitManager.Instance.GetMoveRanges(BattleManager.Instance.TempTriggerData.UnitData.Idx, ne.GridPosIdx);
                    ShowBackupGrids(moveRanges);
                    BattleManager.Instance.SetBattleState(EBattleState.MoveUnit);
                }
                else if (buffData.BuffStr == EBuffID.Spec_AttackUs.ToString())
                {
                    if (unit.BattleUnitData.CurHP <= 0)
                    {
                        GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_NoHPToAttack);
                        return;
                    }
                    
                    var unitBuffDatas = BattleUnitManager.Instance.GetBuffDatas(unit.BattleUnitData);

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
                            if (!BattleFightManager.Instance.IsSoliderAutoAttackData(unit.UnitIdx))
                            {
                                GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_NotTarget);
                                return;
                            }

                            BattleManager.Instance.RecordLastActionBattleData();
                            BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.AutoAtk;
                            
                            BattleManager.Instance.RefreshEnemyAttackData();
                            BattleFightManager.Instance.SoliderAutoAttack();
                            BattleBuffManager.Instance.UseBuff(ne.GridPosIdx, unit.UnitIdx);
                            
                            ShowBackupGrids(null);
                            //BattleEnemyManager.Instance.UnShowEnemyRoutes();
                            BattleManager.Instance.TempTriggerData.Reset();
                            BattleManager.Instance.SetBattleState(EBattleState.UseCard);
                            unit.BattleUnitData.RoundAttackTimes += 1;
                            
                        }   
                        else if (unitBuffData.BuffTriggerType == EBuffTriggerType.SelectUnit)
                        {
                            BattleManager.Instance.SetBattleState(EBattleState.SelectHurtUnit);
                            var attackRanges = BattleUnitManager.Instance.GetAttackRanges(unit.UnitIdx, ne.GridPosIdx);
                            ShowBackupGrids(attackRanges);
                        }
                        else if (unitBuffData.BuffTriggerType == EBuffTriggerType.SelectGrid)
                        {
                            GameEntry.Event.Fire(null, RefreshBattleStateEventArgs.Create(EBattleState.SelectHurtUnit));
                            BattleManager.Instance.SetBattleState(EBattleState.SelectHurtUnit);
                            var attackRanges = BattleUnitManager.Instance.GetAttackRanges(unit.UnitIdx, ne.GridPosIdx);
                            ShowBackupGrids(attackRanges);
                        }
                    
                        BattleUnitManager.Instance.UnShowTags();                          
                    }
                    
                        
                    
                    
                }
                else
                {
                    BattleManager.Instance.RecordLastActionBattleData();
                    //BattleBuffManager.Instance.TriggerBuff();
                    BattleBuffManager.Instance.UseBuff(ne.GridPosIdx);
                    
                    BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.Empty;
                    BattleManager.Instance.TempTriggerData.TargetGridPosIdx = -1;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.Clear();
                    
                }
               
            }
            else if (BattleManager.Instance.BattleState == EBattleState.TacticSelectGrid)
            {
                BattleManager.Instance.RecordLastActionBattleData();
                BattleBuffManager.Instance.UseBuff(ne.GridPosIdx);
                
            }
            else if (BattleManager.Instance.BattleState == EBattleState.MoveUnit)
            {
                if(TutorialManager.Instance.Switch_SelectMovePos(ne.GridPosIdx) == ETutorialState.UnMatch)
                    return;
                
                var moveRanges =
                    BattleUnitManager.Instance.GetMoveRanges(BattleManager.Instance.TempTriggerData.UnitData.Idx,
                        BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx);
                if (moveRanges.Contains(ne.GridPosIdx))
                {
                    
                    ShowBackupGrids(null);

                    var unit = BattleUnitManager.Instance.GetUnitByIdx(BattleManager.Instance.TempTriggerData.UnitData
                        .Idx);

                    MoveActionData moveActionData = null;
                    if (BattleFightManager.Instance.RoundFightData.SoliderMoveDatas.ContainsKey(unit.UnitIdx))
                    {
                        moveActionData = BattleFightManager.Instance.RoundFightData.SoliderMoveDatas[unit.UnitIdx];
                    }
                    else if(BattleFightManager.Instance.RoundFightData.EnemyMoveDatas.ContainsKey(unit.UnitIdx))
                    {
                        moveActionData = BattleFightManager.Instance.RoundFightData.EnemyMoveDatas[unit.UnitIdx];
                    }
                    


                    if (moveActionData != null)
                    {
                        var moveGridPosIdxs = moveActionData.MoveGridPosIdxs;
            
                        
                        unit.Position = moveGridPosIdxs.Count > 0 ? GameUtility.GridPosIdxToPos(moveGridPosIdxs[0]) : unit.Position;
                        
                        var time = unit.GetMoveTime(EUnitActionState.Run, moveActionData);
                        unit.Run(moveActionData);
                        GameUtility.DelayExcute(time, () =>
                        {
                            BattleManager.Instance.SetBattleState(EBattleState.UseCard);
                            BattleManager.Instance.RefreshEnemyAttackData();

                        });
                    }
                    
                    BattleManager.Instance.RecordLastActionBattleData();
                    RefreshObstacles();
                    //BattleEnemyManager.Instance.UnShowEnemyRoutes();
                    BattleBuffManager.Instance.UseBuff(ne.GridPosIdx, unit.BattleUnitData.Idx);
                    unit.BattleUnitData.RoundMoveTimes += 1;
                    BattleManager.Instance.TempTriggerData.Reset();
                    TmpUnitEntity = null;
                    BattleManager.Instance.SetBattleState(EBattleState.Animation);
                    
                }
            }
            else if (BattleManager.Instance.BattleState == EBattleState.FuneMoveUnit)
            {
                var moveRanges =
                    BattleUnitManager.Instance.GetMoveRanges(BattleManager.Instance.TempTriggerData.UnitData.Idx,
                        BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx);
                if (moveRanges.Contains(ne.GridPosIdx))
                {
                    ShowBackupGrids(null);

                    var unit = BattleUnitManager.Instance.GetUnitByIdx(BattleManager.Instance.TempTriggerData.UnitData
                        .Idx);
                    
                    var moveActionData = BattleFightManager.Instance.RoundFightData.SoliderMoveDatas[unit.UnitIdx];
                    

                    var time = unit.GetMoveTime(EUnitActionState.Run, moveActionData);
                    unit.Run(moveActionData);
                    GameUtility.DelayExcute(time, () => { BattleManager.Instance.RefreshEnemyAttackData(); });

                    RefreshObstacles();
                    BattleManager.Instance.RefreshEnemyAttackData();
                    //BattleEnemyManager.Instance.UnShowEnemyRoutes();
                    
                    BattleManager.Instance.TempTriggerData.Reset();
                    GameEntry.Event.Fire(null, RefreshBattleStateEventArgs.Create(EBattleState.UseCard));
                    BattleManager.Instance.SetBattleState(EBattleState.UseCard);
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
                            //BattleEnemyManager.Instance.UnShowEnemyRoutes();
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
                        //BattleEnemyManager.Instance.UnShowEnemyRoutes();
                    }

                }
                
                BattleCardManager.Instance.RefreshCardConfirm();
            }
            else if (BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit)
            {
                var attackRanges = BattleUnitManager.Instance.GetAttackRanges(
                    BattleManager.Instance.TempTriggerData.UnitData.Idx,
                    BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx);
                
                if (attackRanges.Contains(ne.GridPosIdx))
                {
                    BattleManager.Instance.RecordLastActionBattleData();
                    ShowBackupGrids(null);
                    
                    var unitData = GameUtility.GetUnitDataByIdx(BattleManager.Instance.TempTriggerData.UnitData.Idx, false);
                    var unit = BattleUnitManager.Instance.GetUnitByIdx(unitData.Idx);
                    if (unit != null)
                    {
                        unit.TargetPosIdx = ne.GridPosIdx;
                    }
                    if (unitData != null)
                    {
                        //unit.UnitState.RemoveState(EUnitState.ActiveAttack);
                    }
                    
                    var attackUnitEntity = BattleUnitManager.Instance.GetUnitByIdx(BattleManager.Instance.TempTriggerData.UnitData.Idx);
                    var effectUnitEntity = BattleUnitManager.Instance.GetUnitByGridPosIdx(ne.GridPosIdx);
                    if (attackUnitEntity != null)
                    {
                        attackUnitEntity.UnShowTags();
                    }
                    
                    if (effectUnitEntity != null)
                    {
                        effectUnitEntity.UnShowTags();
                    }
                    
                    BattleFightManager.Instance.SoliderActiveAttack();
                    BattleBuffManager.Instance.UseBuff(ne.GridPosIdx, unitData.Idx);
                    unitData.RoundAttackTimes += 1;
                    //BattleManager.Instance.Refresh();
                    //BattleEnemyManager.Instance.UnShowEnemyRoutes();

                    BattleManager.Instance.TempTriggerData.Reset();
                    // BattleUnitManager.Instance.TempUnitData.TriggerType = ETempUnitType.Null;
                    //BattleUnitManager.Instance.TempUnitData.UnitData = null;
                    BattleManager.Instance.SetBattleState(EBattleState.UseCard);
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
                unit1.BattleUnitData.GridPosIdx = gridPosIdx2;
                unit1.UpdatePos(pos2);
            }

            if (unit2 != null)
            {
                unit2.BattleUnitData.GridPosIdx = gridPosIdx1;
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

            BattleManager.Instance.RefreshEnemyAttackData();
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

        public void RefreshGirdEntities()
        {
            GridEntitiesMap.Clear();
            foreach (var kv in GridEntities)
            {
                kv.Refresh();
                GridEntitiesMap.Add(kv.GridPosIdx, kv);
            }

        }

        public void ShowBackupGrids(List<int> gridPosIdxs)
        {
            foreach (var kv in GridEntities)
            {
                if (gridPosIdxs == null)
                {
                    kv.ShowBackupGrid(false);
                }
                else if (gridPosIdxs.Contains(kv.GridPosIdx))
                {
                    kv.ShowBackupGrid(true);
                }
                else
                {
                    kv.ShowBackupGrid(false);
                }

            }
        }

        public async void PlaceProp(int propID, int gridPosIdx, EUnitCamp playerUnitCamp)
        {
            var isStayProp = BattleGridPropManager.Instance.IsStayProp(propID);
            
            
            var unPlacePosIdxs = BattleBuffManager.Instance.GetUnPlacePosIdxs(GamePlayManager.Instance.GamePlayData);
            if (unPlacePosIdxs.Contains(gridPosIdx) && !isStayProp)
                return;
            
            if (BattleManager.Instance.CurUnitCamp == PlayerManager.Instance.PlayerData.UnitCamp)
            {
                BattleManager.Instance.RecordLastActionBattleData();
                BattleBuffManager.Instance.UseBuff(gridPosIdx);
                
            }
            var gridPropData = BattleManager.Instance.TempTriggerData.PropData.Copy();
            //battleSoliderData.UnitRole = EUnitRole.Staff;
            gridPropData.Idx = BattleUnitManager.Instance.GetIdx();
            await GenerateProp(gridPropData);
            
            BattleManager.Instance.TempTriggerData.Reset();

            //FuneManager.Instance.TriggerUnitUse();

            BattleAreaManager.Instance.RefreshObstacles();
            BattleManager.Instance.RefreshEnemyAttackData();
            
            GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());

        }

        public async void PlaceUnitCard(int cardIdx, int gridPosIdx, EUnitCamp playerUnitCamp)
        {

            var unPlacePosIdxs = BattleBuffManager.Instance.GetUnPlacePosIdxs(GamePlayManager.Instance.GamePlayData);
            if (unPlacePosIdxs.Contains(gridPosIdx))
            {
                GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_UnPlaceUnit);
                return;
            }

            // var battleSoliderEntity =
            //     await GameEntry.Entity.ShowBattleSoliderEntityAsync(new Data_BattleSolider(
            //         BattleUnitManager.Instance.GetTempID(), cardID,
            //         gridPosIdx, cardEnergy, playerUnitCamp, cardData.FuneIDs));
            
            
            BattleManager.Instance.RecordLastActionBattleData();
            var soliderData = BattleManager.Instance.TempTriggerData.UnitData as Data_BattleSolider;
            //soliderData.RefreshCardData();
            
            
            var battleSoliderData = soliderData.Copy();
            //battleSoliderData.UnitRole = EUnitRole.Staff;
            //battleSoliderData.Idx = BattleUnitManager.Instance.GetIdx();
            await GenerateSolider(battleSoliderData);
            
            if (BattleManager.Instance.CurUnitCamp == PlayerManager.Instance.PlayerData.UnitCamp)
            {
                BattleBuffManager.Instance.UseBuff(gridPosIdx);
                
            }
            
            BattleManager.Instance.TempTriggerData.Reset();

            //FuneManager.Instance.TriggerUnitUse();

            BattleAreaManager.Instance.RefreshObstacles();
            BattleManager.Instance.RefreshEnemyAttackData();

            //BattleEnemyManager.Instance.UnShowEnemyRoutes();
            
            GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
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

        public void ShowAllGrid(bool show)
        {
            foreach (var kv in GridEntities)
            {
                kv.Show(show);
            }
        }
        
        
    }
}
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

        
        

        public void Destory()
        {
            Unsubscribe();
            MoveGrids.Clear();
            MoveGridPosIdxs.Clear();
            HideTmpUnitEntity();
            // if (TmpUnitEntity != null && GameEntry.Entity.HasEntity(TmpUnitEntity.Id))
            // {
            //     GameEntry.Entity.HideEntity(TmpUnitEntity);
            //     TmpUnitEntity = null;
            // }

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
            
            var soliderEntityID = BattleUnitManager.Instance.GetUnitIdx(ne.GridPosIdx,
                BattleManager.Instance.CurUnitCamp, ERelativeCamp.Us, EUnitRole.Staff);
            var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(ne.GridPosIdx);
            
            if (ne.ShowState == EShowState.Show)
            {
                await GameEntry.UI.OpenUIFormAsync(UIFormId.GridDescForm, new GridDescData()
                {
                    GridPosIdx = ne.GridPosIdx,
                });
                BattleGridManager.Instance.ShowAllGrid(true);
                BattleAreaManager.Instance.CurPointGridPosIdx = ne.GridPosIdx;
                BattleManager.Instance.TempTriggerData.TargetGridPosIdx = ne.GridPosIdx;
                if (soliderEntityID != -1)
                {
                    GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                }
                
                //var curUnit = BattleUnitManager.Instance.GetUnitByIdx(BattleManager.Instance.TempTriggerData.UnitData.Idx);
                var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
                var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);

                if (unit != null)
                {
                    if (unit.UnitCamp == EUnitCamp.Player1 && buffData.BuffStr == EBuffID.Spec_AttackUs.ToString())
                    {
                        unit.ShowAttackRange(true);
                    
                    }
                    else if (unit.UnitCamp == EUnitCamp.Player1 && buffData.BuffStr == EBuffID.Spec_MoveUs.ToString())
                    {
                        unit.ShowMoveRange(true);
                    
                    }
                    else if (BattleManager.Instance.BattleState != EBattleState.SelectHurtUnit)
                    {
                        unit.ShowAttackRange(true);
                    }
                }
                
            }
            else if (ne.ShowState == EShowState.Unshow)
            {
                BattleUnitManager.Instance.UnShowTags();
                BattleGridManager.Instance.ShowAllGrid(false);
                BattleAreaManager.Instance.CurPointGridPosIdx = -1;
                BattleManager.Instance.TempTriggerData.TargetGridPosIdx = -1;
                
                if (BattleManager.Instance.BattleState != EBattleState.SelectHurtUnit)
                {
                    BattleGridManager.Instance.UnshowGrids();
                    //unit.ShowAttackRange(false);
                }
            }

            if (BattleManager.Instance.BattleState == EBattleState.UnitSelectGrid)
            {
                
                if (!GameUtility.CheckUnitSelectGrid(ne.GridPosIdx, false))
                {
                    if (ne.ShowState == EShowState.Show)
                    {
                        BattleController.Instance.ShowUnAttackTag(ne.GridPosIdx);
                    }
                    else if (ne.ShowState == EShowState.Unshow)
                    {
                        BattleController.Instance.UnShowUnAttackTag();
                    }
                }
                
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
                        //Log.Debug("ShowBattleSolider2:" + tmpEntity.UnitIdx + "-" + ne.GridPosIdx);
                        
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
                            tmpEntity.OnPointerEnter();
                            //showhurt
                            BattleTagManager.Instance.ShowHurtTags(TmpUnitEntity.UnitIdx, null);
                            //await TmpUnitEntity.ShowHurtTags(TmpUnitEntity.UnitIdx, null);
                            //TmpUnitEntity.ShowTags(TmpUnitEntity.UnitIdx);
                            tmpEntity.ShowAttackRange(true);
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
                if (!GameUtility.CheckPropSelectGrid(ne.GridPosIdx, false))
                {
                    if (ne.ShowState == EShowState.Show)
                    {
                        BattleController.Instance.ShowUnAttackTag(ne.GridPosIdx);
                    }
                    else if (ne.ShowState == EShowState.Unshow)
                    {
                        BattleController.Instance.UnShowUnAttackTag();
                    }
                }
                
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
                        
                        var idx = BattleUnitManager.Instance.GetIdx();
                        Log.Debug("A:" + ne.GridPosIdx + "-" + idx);
                        BattleManager.Instance.TempTriggerData.PropData = new Data_GridProp(propID,
                            idx,
                            ne.GridPosIdx, BattleManager.Instance.CurUnitCamp);
                        
                        var gridPropData = BattleManager.Instance.TempTriggerData.PropData.Copy();
                        
                        HideTmpPropEntity();
                        //gridPropData.Idx = idx;
                        var tmpEntity =
                            await GameEntry.Entity.ShowBattleGridPropEntityAsync(gridPropData);

                        if (tmpEntity == null)
                        {
                            Log.Debug("tmpEntity == null");
                        }
                        Log.Debug("B:" + ne.GridPosIdx + "-" + tmpEntity.GridPropData.Idx );
                        if(BattleManager.Instance.TempTriggerData.PropData == null || tmpEntity.GridPropData.Idx < BattleManager.Instance.TempTriggerData.PropData.Idx)
                        {
                            Log.Debug("C:");
                            BattleGridPropManager.Instance.GridPropDatas.Remove(tmpEntity.GridPropData.Idx);

                            GameEntry.Entity.HideEntity(tmpEntity);
                            BattleManager.Instance.RefreshEnemyAttackData();
                        }
                        else
                        {
                            Log.Debug("D");
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
                    Log.Debug("E:" + ne.GridPosIdx);
                    // &&
                    //BattleManager.Instance.TempTriggerData.PropData.GridPosIdx == ne.GridPosIdx
                    if (BattleManager.Instance.TempTriggerData.PropData != null)
                    {
                        Log.Debug("F:"+ BattleManager.Instance.TempTriggerData.PropData.Idx);
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
                if (ne.ShowState == EShowState.Show)
                {
                    TmpUnitEntity.ShowMoveRange(true);
                }
                else if (ne.ShowState == EShowState.Unshow)
                {
                    TmpUnitEntity.ShowMoveRange(false);
                }

                var moveRanges = TmpUnitEntity.GetMoveRange(BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx);
                    // BattleUnitManager.Instance.GetMoveRanges(BattleManager.Instance.TempTriggerData.UnitData.Idx,
                    //     BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx);
                if (!moveRanges.Contains(ne.GridPosIdx))
                {
                    if (ne.ShowState == EShowState.Show)
                    {
                        BattleController.Instance.ShowUnAttackTag(ne.GridPosIdx);

                    }
                    else if (ne.ShowState == EShowState.Unshow)
                    {
                        BattleController.Instance.UnShowUnAttackTag();

                    }
                    
                    Log.Debug("moveC" + ne.GridPosIdx + ne.ShowState);
                    BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx =
                        BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx;
                    BattleManager.Instance.TempTriggerData.TempUnitMovePaths.Clear();
                    BattleManager.Instance.RefreshEnemyAttackData();
                    
                }
                else
                {
                    BattleController.Instance.UnShowUnAttackTag();
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
                        
                        BattleTagManager.Instance.ShowHurtTags(TmpUnitEntity.UnitIdx, null);
                        //await TmpUnitEntity.ShowHurtTags(TmpUnitEntity.UnitIdx, null);
                        
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
                    if (relativeUnit != null && relativeUnit.UnitRole == EUnitRole.Staff)
                    {
                        BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.UseBuff;
                        //BattleManager.Instance.TempTriggerData.TargetGridPosIdx = relativeUnit.BattleUnitData.GridPosIdx;
                        
                        BattleManager.Instance.TempTriggerData.UnitData =
                            BattleUnitManager.Instance.GetBattleUnitData(relativeUnit);
                        
                        
                        var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
                        if (buffData.BuffStr == EBuffID.Spec_AttackUs.ToString())
                        {
                            // if (BattleManager.Instance.TempTriggerData.UnitData.CurHP <= 0)
                            // {
                            //     return;
                            // }
                            
                            var actionTimes = relativeUnit.BattleUnitData.RoundAttackTimes;
                            //BattleCardManager.Instance.RefreshCurCardEnergy();
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
                                    
                                    //BattleGridManager.Instance.ShowGreenGrids(attackRanges);
                                }
                                else if (unitBuffData.BuffTriggerType == EBuffTriggerType.SelectGrid)
                                {
                                    var attackRanges = BattleUnitManager.Instance.GetAttackRanges(relativeUnit.UnitIdx, ne.GridPosIdx);
                                    //BattleGridManager.Instance.ShowGreenGrids(attackRanges);
                                }
                        
                                BattleManager.Instance.RefreshEnemyAttackData();
                                
                            }
                            BattleCardManager.Instance.RefreshCurCardEnergy(BattleManager.Instance.TempTriggerData
                                .UnitData.RoundAttackTimes);
                        } 
                        else if (buffData.BuffStr == EBuffID.Spec_MoveUs.ToString())
                        {
                            // if (BattleManager.Instance.TempTriggerData.UnitData.CurHP <= 0)
                            // {
                            //     return;
                            // }
                            //
                            // var actionTimes = relativeUnit.BattleUnitData.RoundMoveTimes;
                            // //BattleCardManager.Instance.RefreshCurCardEnergy();
                            BattleCardManager.Instance.RefreshCurCardEnergy(BattleManager.Instance.TempTriggerData
                                .UnitData.RoundMoveTimes);
                        }
                        
                        else
                        {
                            BattleManager.Instance.RefreshEnemyAttackData();
                            //BattleGridManager.Instance.ShowGreenGrids(null);
                            //BattleCardManager.Instance.RefreshCurCardEnergy();
                            //BattleBuffManager.Instance.UseBuff(ne.GridPosIdx, unit.ID);
                            BattleCardManager.Instance.RefreshCurCardEnergy();
                        }

                    }
                    else
                    {
                        BattleCardManager.Instance.RefreshCurCardEnergy(-1);
                    }

                }
                else if (ne.ShowState == EShowState.Unshow)
                {

                    BattleManager.Instance.TempTriggerData.UnitData = null;
                    BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.Empty;
                    //BattleManager.Instance.TempTriggerData.TargetGridPosIdx = -1;
                    //BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.CardID = -1;
                    BattleManager.Instance.RefreshEnemyAttackData();
                    //BattleCardManager.Instance.RefreshCurCardEnergy();
                }
                
                if (!GameUtility.CheckTacticSelectUnit(ne.GridPosIdx, false))
                {
                    if (ne.ShowState == EShowState.Show)
                    {
                        BattleController.Instance.ShowUnAttackTag(ne.GridPosIdx);
                    }
                    else if (ne.ShowState == EShowState.Unshow)
                    {
                        BattleController.Instance.UnShowUnAttackTag();
                    }
                }
                
            }

            if (BattleManager.Instance.BattleState == EBattleState.TacticSelectGrid)
            {
                BattleManager.Instance.RefreshEnemyAttackData();
                if (ne.ShowState == EShowState.Show)
                {
                    GameUtility.ShowAttackRange(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx,
                        ne.GridPosIdx, true);
                }
                else if (ne.ShowState == EShowState.Unshow)
                {
                    GameUtility.ShowAttackRange(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx,
                        ne.GridPosIdx, false);
                }
                
                var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
                var drBuff = BattleBuffManager.Instance.GetBuffData(buffStr);
                     
                var range = GameUtility.GetRange(ne.GridPosIdx, drBuff.TriggerRange, BattleManager.Instance.CurUnitCamp, drBuff.TriggerUnitCamps, false);
                if (BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr2 != string.Empty)
                {
                    var drBuff2 = BattleBuffManager.Instance.GetBuffData(BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr2);
                    range.AddRange(GameUtility.GetRange(ne.GridPosIdx, drBuff2.TriggerRange, BattleManager.Instance.CurUnitCamp, drBuff2.TriggerUnitCamps));

                }

                
                
                if (ne.ShowState == EShowState.Show)
                {
                    BattleTagManager.Instance.ShowTags(Constant.Battle.UnUnitTriggerIdx);
                    //BattleGridManager.Instance.ShowRedGrids(range);
                    
                    if (!BattleFightManager.Instance.ExistUseCardData())
                    {
                        BattleController.Instance.ShowUnAttackTag(ne.GridPosIdx);
                    }
                    else
                    {
                        BattleController.Instance.UnShowUnAttackTag();
                    }
                }
                else
                {
                    BattleTagManager.Instance.UnShowTags();
                    //BattleGridManager.Instance.ShowRedGrids(null);
                    
                    BattleController.Instance.UnShowUnAttackTag();
                }
                
                // foreach (var gridPosIdx in range)
                // {
                //     var _unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(gridPosIdx);
                //     
                //     if (ne.ShowState == EShowState.Show)
                //     {
                //         
                //         _unit.ShowTacticHurtTags(_unit.UnitIdx);
                //     }
                //     else
                //     {
                //         _unit.UnShowTags();
                //     }
                // }
                
                
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
                        
                        
                        // if (effectUnitEntity != null)
                        // {
                        //     effectUnitEntity.RefreshFlyDirects(effectUnitEntity.UnitIdx);
                        // }
                        
                        if (attackUnitEntity != null)
                        {
                            
                            //attackUnitEntity.transform.LookAt(effectUnitEntity.transform.position);
                            //, effectUnitEntity.UnitIdx
                            //await attackUnitEntity.ShowHurtTags(attackUnitEntity.UnitIdx);
                            
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
                        BattleTagManager.Instance.UnShowTags();
                        if (attackUnitEntity != null)
                        {
                            attackUnitEntity.ResetPosition();
                        }
                        
                        if (effectUnitEntity != null)
                        {
                            effectUnitEntity.ResetPosition();
                        }
                    }
                    BattleController.Instance.UnShowUnAttackTag();
                    
                }
                else
                {
                    if (ne.ShowState == EShowState.Show)
                    {
                        BattleController.Instance.ShowUnAttackTag(ne.GridPosIdx);
                    }
                    
                }


                if (ne.ShowState == EShowState.Show)
                {
                    if (!BattleFightManager.Instance.ExistSoliderAutoAttackData(BattleManager.Instance.TempTriggerData.UnitData.Idx))
                    {
                        BattleController.Instance.ShowUnAttackTag(ne.GridPosIdx);
                    }
                    //BattleEnemyManager.Instance.ShowEnemyRoutes();
                }
                else if (ne.ShowState == EShowState.Unshow)
                {
                    BattleController.Instance.UnShowUnAttackTag();
                    //BattleEnemyManager.Instance.UnShowEnemyRoutes();
                }
            }

            if (ne.ShowState == EShowState.Unshow)
            {
                BattleUnitManager.Instance.UnShowTags();
            }
            
            if (unit != null)
            {
                if (ne.ShowState == EShowState.Show)
                {
                    //Log.Debug("1 Enter");
                    unit.OnPointerEnter();
                }
                else if (ne.ShowState == EShowState.Unshow)
                {
                    //Log.Debug("1 Exit");
                    unit.OnPointerExit();
                    //unit.UnShowTags();
                }
            }


            if (!pointerDownInRange && !BattleFightManager.Instance.IsAction)
            {
                //var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(ne.GridPosIdx);

                // var gridEntity = GetGridEntityByGridPosIdx(ne.GridPosIdx);
                // if (ne.ShowState == EShowState.Show)
                // {
                //     //Log.Debug("4 Enter");
                //     gridEntity.OnPointerEnter();
                // }
                // else if (ne.ShowState == EShowState.Unshow)
                // {
                //     //Log.Debug("4 Exit");
                //     gridEntity.OnPointerExit();
                // }
                
                
                
                
                if (unit != null)
                {
                    if (ne.ShowState == EShowState.Show)
                    {
                        //Log.Debug("1 Enter");
                        //unit.OnPointerEnter();
                        if (unit.BattleUnitData.Exist() && !unit.IsMove)
                        {
                            if (BattleManager.Instance.BattleState == EBattleState.TacticSelectUnit)
                            {
                                var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
                                var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
                                if (buffData.BuffStr == EBuffID.Spec_AttackUs.ToString())
                                {
                                    BattleTagManager.Instance.ShowTags(unit.UnitIdx);
                                    //unit.ShowHurtTagByEffectUnit(unit.UnitIdx);
                                    //await unit.ShowTagsWithFlyUnitIdx(unit.UnitIdx, true);
                                }
                                else
                                {
                                    BattleTagManager.Instance.ShowTags(Constant.Battle.UnUnitTriggerIdx);
                                    // unit.ShowTacticHurtDisplayValues(unit.UnitIdx);
                                    // unit.ShowTacticHurtDisplayIcons(unit.UnitIdx);
                                }
                                
                            }
                            else if(BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit)
                            {
                                var attackUnit = BattleUnitManager.Instance.GetUnitByIdx(BattleManager.Instance.TempTriggerData.UnitData.Idx);
                                if (attackUnit != null)
                                {
                                    BattleTagManager.Instance.ShowTags(attackUnit.UnitIdx);
                                    //await attackUnit.ShowTagsWithFlyUnitIdx(attackUnit.UnitIdx, true);
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
                                    BattleTagManager.Instance.ShowTags(unit.UnitIdx);
                                    //await attackUnit.ShowTagsWithFlyUnitIdx(attackUnit.UnitIdx, true);
                                }
                                
                                //unit.ShowHurtTags(unit.UnitIdx, BattleManager.Instance.TempTriggerData.UnitData.Idx);
                            }
                            else if (BattleManager.Instance.BattleState == EBattleState.UnitSelectGrid)
                            {
                                BattleTagManager.Instance.ShowTags(Constant.Battle.UnUnitTriggerIdx);
                                //BattleTagManager.Instance.ShowHurtTags(unit.UnitIdx, null);
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

                                // var triggerDataDict =
                                //     GameUtility.MergeDict(BattleFightManager.Instance.GetDirectAttackDatas(unit.UnitIdx),
                                //         BattleFightManager.Instance.GetInDirectAttackDatas(unit.UnitIdx));
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
                                //             // GameUtility.DelayExcute(0.25f * idx, () =>
                                //             // {
                                //             //     
                                //             // });
                                //             //actionUnit.ShowTagsWithFlyUnitIdx(actionUnit.UnitIdx, true);
                                //             actionUnit.ShowFlyUnitIdx(actionUnit.UnitIdx);
                                //             idx++;
                                //         }
                                //     }
                                // }

                            }
                            else if (BattleManager.Instance.BattleState == EBattleState.TacticSelectGrid)
                            {
                            }
                            else
                            {
                                //showhurt
                                //unit.ShowHurtTags(unit.UnitIdx);
                                //unit.ShowHurtTagByEffectUnit(unit.UnitIdx);
                                BattleTagManager.Instance.ShowTags(unit.UnitIdx);
                                //await unit.ShowTagsWithFlyUnitIdx(unit.UnitIdx);
                                //unit.ShowHurtTags(unit.UnitIdx);
                            }
                        }
                        
                
                    }
                    // else if (ne.ShowState == EShowState.Unshow)
                    // {
                    //     //Log.Debug("1 Exit");
                    //     //unit.OnPointerExit();
                    //     //unit.UnShowTags();
                    // }
                }

            }
            
            
            //BattleCardManager.Instance.RefreshCurCardEnergy();
        }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    

        public Dictionary<int, int> MoveGridPosIdxs = new Dictionary<int, int>();

        private void OnShowEntitySuccess(object sender, GameEventArgs e)
        {

        }

       

        public void OnSelectGrid(object sender, GameEventArgs e)
        {
            var ne = e as SelectGridEventArgs;
            if(BattleGridManager.Instance.GridEntitiesMap.ContainsKey(ne.GridPosIdx))
            {
                BattleGridManager.Instance.GridEntitiesMap[ne.GridPosIdx].ShowSelectGrid(ne.IsSelect);
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
            var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(ne.GridPosIdx);

            if (soliderEntityID != -1)
            {
                GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            }

            

            if (BattleManager.Instance.BattleState == EBattleState.UseCard)
            {
                //var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(ne.GridPosIdx);
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
                if (!GameUtility.CheckUnitSelectGrid(ne.GridPosIdx, true))
                {
                    return;
                }
                BattleManager.Instance.PlaceUnitCard(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx, ne.GridPosIdx, BattleManager.Instance.CurUnitCamp);
                

            }
            else if (BattleManager.Instance.BattleState == EBattleState.PropSelectGrid)
            {
                
                
                var prop = BattleGridPropManager.Instance.GetGridProp(ne.GridPosIdx);
                
                HideTmpPropEntity();
                if (!GameUtility.CheckPropSelectGrid(ne.GridPosIdx, true))
                {
                    return;
                }
                BattleManager.Instance.PlaceProp(prop.GridPropID, ne.GridPosIdx, BattleManager.Instance.CurUnitCamp);
                

            }
            else if (BattleManager.Instance.BattleState == EBattleState.TacticSelectUnit)
            {
                if(TutorialManager.Instance.Switch_SelectMoveUnit(ne.GridPosIdx) == ETutorialState.UnMatch &&
                   TutorialManager.Instance.Switch_SelectAttackUnit(ne.GridPosIdx) == ETutorialState.UnMatch)
                    return;
                
                var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
                var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
                
                if(!GameUtility.CheckTacticSelectUnit(ne.GridPosIdx, true))
                    return;
                
                if (buffData.BuffStr == EBuffID.Spec_MoveUs.ToString() || buffData.BuffStr == EBuffID.Spec_MoveEnemy.ToString())
                {
                    
                    TmpUnitEntity = unit;
                    BattleManager.Instance.TempTriggerData.UnitData =
                        BattleUnitManager.Instance.GetBattleUnitData(unit);
                    BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.MoveUnit;
                    BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx =
                        BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx;
                    TmpUnitEntity.ShowMoveRange(true);
                    // var moveRanges = TmpUnitEntity.GetMoveRange();//BattleUnitManager.Instance.GetMoveRanges(BattleManager.Instance.TempTriggerData.UnitData.Idx, ne.GridPosIdx);
                    // BattleGridManager.Instance.ShowGreenGrids(moveRanges);
                    BattleManager.Instance.SetBattleState(EBattleState.MoveUnit);
                    
                    BattleUnitManager.Instance.ShowMoveTag(false);
                    BattleUnitManager.Instance.ShowAttackTag(false);
                    BattleUnitManager.Instance.ShowTargetTag(false);
                }
                else if (buffData.BuffStr == EBuffID.Spec_AttackUs.ToString())
                {
                    var cardEnergy = BattleFightManager.Instance.RoundFightData.BuffData_Use.CardEnergy;
                
                    if (cardEnergy >=
                        HeroManager.Instance.GetAllCurHP())
                    {
                        GameEntry.UI.OpenMessage(GameEntry.Localization.GetString(Constant.Localization.Message_HPNotUseAll));
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
                            BattleManager.Instance.RecordLastActionBattleData();
                            BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.AutoAtk;
                            
                            BattleManager.Instance.RefreshEnemyAttackData();
                            BattleFightManager.Instance.SoliderAutoAttack();
                            BattleBuffManager.Instance.UseBuff(ne.GridPosIdx, unit.UnitIdx);
                            
                            //BattleGridManager.Instance.ShowGreenGrids(null);
                            //BattleEnemyManager.Instance.UnShowEnemyRoutes();
                            BattleManager.Instance.TempTriggerData.Reset();
                            BattleManager.Instance.SetBattleState(EBattleState.UseCard);
                            unit.BattleUnitData.RoundAttackTimes += 1;
                            
                        }   
                        else if (unitBuffData.BuffTriggerType == EBuffTriggerType.SelectUnit)
                        {
                            BattleManager.Instance.SetBattleState(EBattleState.SelectHurtUnit);
                            var attackRanges = BattleUnitManager.Instance.GetAttackRanges(unit.UnitIdx, ne.GridPosIdx);
                            //BattleGridManager.Instance.ShowGreenGrids(attackRanges);
                        }
                        else if (unitBuffData.BuffTriggerType == EBuffTriggerType.SelectGrid)
                        {
                            //GameEntry.Event.Fire(null, RefreshBattleStateEventArgs.Create(EBattleState.SelectHurtUnit));
                            BattleManager.Instance.SetBattleState(EBattleState.SelectHurtUnit);
                            var attackRanges = BattleUnitManager.Instance.GetAttackRanges(unit.UnitIdx, ne.GridPosIdx);
                            //BattleGridManager.Instance.ShowGreenGrids(attackRanges);
                        }
                    
                        BattleUnitManager.Instance.UnShowTags();                          
                    }
                    
                        
                    BattleUnitManager.Instance.ShowMoveTag(false);
                    BattleUnitManager.Instance.ShowAttackTag(false);
                    BattleUnitManager.Instance.ShowTargetTag(false);
                }
                else
                {
                    
                    BattleManager.Instance.RecordLastActionBattleData();
                    //BattleBuffManager.Instance.TriggerBuff();
                    BattleBuffManager.Instance.UseBuff(ne.GridPosIdx);
                    
                    BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.Empty;
                    BattleManager.Instance.TempTriggerData.TargetGridPosIdx = -1;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.Clear();
                    BattleUnitManager.Instance.ShowTargetTag(false);
                }
               
            }
            else if (BattleManager.Instance.BattleState == EBattleState.TacticSelectGrid)
            {
                if (!BattleFightManager.Instance.ExistUseCardData())
                {
                    GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_NotTarget);
                    return;
                }
                
                BattleManager.Instance.RecordLastActionBattleData();
                BattleBuffManager.Instance.UseBuff(ne.GridPosIdx);
                
            }
            else if (BattleManager.Instance.BattleState == EBattleState.MoveUnit)
            {
                if(TutorialManager.Instance.Switch_SelectMovePos(ne.GridPosIdx) == ETutorialState.UnMatch)
                    return;
                
                 
                
                var moveRanges = TmpUnitEntity.GetMoveRange(BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx);
                    // BattleUnitManager.Instance.GetMoveRanges(BattleManager.Instance.TempTriggerData.UnitData.Idx,
                    //     BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx);
                if (moveRanges.Contains(ne.GridPosIdx))
                {
                    var cardEnergy = BattleFightManager.Instance.RoundFightData.BuffData_Use.CardEnergy;
                
                    if (cardEnergy >=
                        HeroManager.Instance.GetAllCurHP())
                    {
                        GameEntry.UI.OpenMessage(GameEntry.Localization.GetString(Constant.Localization.Message_HPNotUseAll));
                        return;
                    }
                    
                    BattleGridManager.Instance.ShowGreenGrids(null);

                    var unit2 = BattleUnitManager.Instance.GetUnitByIdx(BattleManager.Instance.TempTriggerData.UnitData
                        .Idx);

                    MoveActionData moveActionData = null;
                    if (BattleFightManager.Instance.RoundFightData.SoliderMoveDatas.ContainsKey(unit2.UnitIdx))
                    {
                        moveActionData = BattleFightManager.Instance.RoundFightData.SoliderMoveDatas[unit2.UnitIdx].Copy();
                    }
                    else if(BattleFightManager.Instance.RoundFightData.EnemyMoveDatas.ContainsKey(unit2.UnitIdx))
                    {
                        moveActionData = BattleFightManager.Instance.RoundFightData.EnemyMoveDatas[unit2.UnitIdx].Copy();;
                    }
                    


                    if (moveActionData != null)
                    {
                        var moveGridPosIdxs = moveActionData.MoveGridPosIdxs;
            
                        
                        unit2.Position = moveGridPosIdxs.Count > 0 ? GameUtility.GridPosIdxToPos(moveGridPosIdxs[0]) : unit.Position;
                        
                        var time = unit2.GetMoveTime(EUnitActionState.Run, moveActionData);
                        unit2.Run(moveActionData);
                        GameUtility.DelayExcute(time, () =>
                        {
                            BattleManager.Instance.SetBattleState(EBattleState.UseCard);
                            BattleManager.Instance.RefreshEnemyAttackData();

                        });
                    }
                    
                    BattleManager.Instance.RecordLastActionBattleData();
                    RefreshObstacles();
                    //BattleEnemyManager.Instance.UnShowEnemyRoutes();
                    BattleManager.Instance.SetBattleState(EBattleState.Animation);
                    BattleBuffManager.Instance.UseBuff(ne.GridPosIdx, unit2.BattleUnitData.Idx);
                    unit2.BattleUnitData.RoundMoveTimes += 1;
                    BattleManager.Instance.TempTriggerData.Reset();
                    TmpUnitEntity = null;
                    
                    
                }
                else
                {
                    GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_NotMoveRange);
                }
            }
            else if (BattleManager.Instance.BattleState == EBattleState.FuneMoveUnit)
            {
                var moveRanges = TmpUnitEntity.GetMoveRange(BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx);
                    // BattleUnitManager.Instance.GetMoveRanges(BattleManager.Instance.TempTriggerData.UnitData.Idx,
                    //     BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx);
                if (moveRanges.Contains(ne.GridPosIdx))
                {
                    BattleGridManager.Instance.ShowGreenGrids(null);

                    var unit2 = BattleUnitManager.Instance.GetUnitByIdx(BattleManager.Instance.TempTriggerData.UnitData
                        .Idx);
                    
                    var moveActionData = BattleFightManager.Instance.RoundFightData.SoliderMoveDatas[unit2.UnitIdx].Copy();
                    

                    var time = unit2.GetMoveTime(EUnitActionState.Run, moveActionData);
                    unit2.Run(moveActionData);
                    GameUtility.DelayExcute(time, () => { BattleManager.Instance.RefreshEnemyAttackData(); });

                    RefreshObstacles();
                    BattleManager.Instance.RefreshEnemyAttackData();
                    //BattleEnemyManager.Instance.UnShowEnemyRoutes();
                    
                    BattleManager.Instance.TempTriggerData.Reset();
                    //GameEntry.Event.Fire(null, RefreshBattleStateEventArgs.Create(EBattleState.UseCard));
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
                    var gridEntity = BattleGridManager.Instance.GetGridEntityByGridPosIdx(ne.GridPosIdx);
                    var tempExchangeGridData = BattleAreaManager.Instance.TempExchangeGridData;

                    if (ne.GridPosIdx == tempExchangeGridData.GridPosIdx1)
                    {
                        if (tempExchangeGridData.GridPosIdx2 != -1)
                        {
                            ExchangeGrid(tempExchangeGridData.GridPosIdx1, tempExchangeGridData.GridPosIdx2);
                            var grid2Entity = BattleGridManager.Instance.GetGridEntityByGridPosIdx(tempExchangeGridData.GridPosIdx2);
                            //grid2Entity.ShowGreenGrid(false);
                            tempExchangeGridData.GridPosIdx2 = -1;
                        }

                        tempExchangeGridData.GridPosIdx1 = -1;
                        //gridEntity.ShowGreenGrid(false);
                    }
                    else if (ne.GridPosIdx == tempExchangeGridData.GridPosIdx2)
                    {
                        if (tempExchangeGridData.GridPosIdx1 != -1)
                        {
                            ExchangeGrid(tempExchangeGridData.GridPosIdx1, tempExchangeGridData.GridPosIdx2);
                            var grid1Entity = BattleGridManager.Instance.GetGridEntityByGridPosIdx(tempExchangeGridData.GridPosIdx1);
                            //grid1Entity.ShowGreenGrid(false);
                            tempExchangeGridData.GridPosIdx1 = -1;
                            //BattleEnemyManager.Instance.UnShowEnemyRoutes();
                        }

                        tempExchangeGridData.GridPosIdx2 = -1;
                        //gridEntity.ShowGreenGrid(false);
                    }
                    else if (tempExchangeGridData.GridPosIdx1 == -1)
                    {
                        tempExchangeGridData.GridPosIdx1 = ne.GridPosIdx;
                        //gridEntity.ShowGreenGrid(true);
                    }
                    else if (tempExchangeGridData.GridPosIdx2 == -1)
                    {
                        tempExchangeGridData.GridPosIdx2 = ne.GridPosIdx;
                        //gridEntity.ShowGreenGrid(true);
                        //BattleEnemyManager.Instance.UnShowEnemyRoutes();
                    }

                }
                
                BattleCardManager.Instance.RefreshCardConfirm();
            }
            else if (BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit)
            {
                // var attackRanges = BattleUnitManager.Instance.GetAttackRanges(
                //     BattleManager.Instance.TempTriggerData.UnitData.Idx,
                //     BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx);
                var cardEnergy = BattleFightManager.Instance.RoundFightData.BuffData_Use.CardEnergy;
                
                if (cardEnergy >=
                    HeroManager.Instance.GetAllCurHP())
                {
                    GameEntry.UI.OpenMessage(GameEntry.Localization.GetString(Constant.Localization.Message_HPNotUseAll));
                    return;
                }

                
                var curUnit = BattleUnitManager.Instance.GetUnitByIdx(BattleManager.Instance.TempTriggerData.UnitData.Idx);
                
                 List<int> attackRanges = new List<int>();
                 if (curUnit != null)
                 {
                     attackRanges = curUnit.GetAttackRange();
                 }

                if (!attackRanges.Contains(ne.GridPosIdx))
                {
                    GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_NotAttackRange);

                }
                else if (!BattleFightManager.Instance.ExistSoliderAutoAttackData(BattleManager.Instance.TempTriggerData.UnitData.Idx))
                {
                    GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_NotTarget);
                }
                // if (attackRanges.Contains(ne.GridPosIdx))
                else
                {
                    BattleManager.Instance.RecordLastActionBattleData();
                    //BattleGridManager.Instance.ShowGreenGrids(null);
                    
                    var unitData = GameUtility.GetUnitDataByIdx(BattleManager.Instance.TempTriggerData.UnitData.Idx, false);
                    var unit2 = BattleUnitManager.Instance.GetUnitByIdx(unitData.Idx);
                    if (unit2 != null)
                    {
                        unit2.TargetPosIdx = ne.GridPosIdx;
                    }
                    if (unitData != null)
                    {
                        //unit.UnitState.RemoveState(EUnitState.ActiveAttack);
                    }
                    
                    var attackUnitEntity = BattleUnitManager.Instance.GetUnitByIdx(BattleManager.Instance.TempTriggerData.UnitData.Idx);
                    var effectUnitEntity = BattleUnitManager.Instance.GetUnitByGridPosIdx(ne.GridPosIdx);
                    BattleTagManager.Instance.UnShowTags();
                    if (attackUnitEntity != null)
                    {
                        attackUnitEntity.ResetPosition();
                    }
                    
                    if (effectUnitEntity != null)
                    {
                        effectUnitEntity.ResetPosition();
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
                // if (allAttackRange.Contains(ne.GridPosIdx))
                // {
                //     GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_NotTarget);
                // }
                
                
       
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

            var grid1 = BattleGridManager.Instance.GetGridEntityByGridPosIdx(gridPosIdx1);
            var grid2 = BattleGridManager.Instance.GetGridEntityByGridPosIdx(gridPosIdx2);
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
        public async void PlaceProp(int propID, int gridPosIdx, EUnitCamp playerUnitCamp)
        {
            var isStayProp = BattleGridPropManager.Instance.IsStayProp(propID);
            
            
            // var unPlacePosIdxs = BattleBuffManager.Instance.GetUnPlacePosIdxs(GamePlayManager.Instance.GamePlayData);
            // if (unPlacePosIdxs.Contains(gridPosIdx) && !isStayProp)
            //     return;
            
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

            // var unPlacePosIdxs = BattleBuffManager.Instance.GetUnPlacePosIdxs(GamePlayManager.Instance.GamePlayData);
            // if (unPlacePosIdxs.Contains(gridPosIdx))
            // {
            //     GameEntry.UI.OpenLocalizationMessage(Constant.Localization.Message_UnPlaceUnit);
            //     return;
            // }

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

        
        
        
        
    }
}
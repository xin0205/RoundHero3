﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameFramework;
using UnityEngine;

using UnityGameFramework.Runtime;
using Random = System.Random;


namespace RoundHero
{
    
    
    public enum ETriggerResult
    {
        Dodge,
        UnHurt,
        Continue,
    }

    // public class FlyData
    // {
    //     public EFlyType FlyType;
    //     public Dictionary<int, FlyUnitData> FlyUnitDatas = new Dictionary<int, FlyUnitData>();
    //
    // }
    //
    public class MoveUnitData
    {
        public int UnitID;
        public EUnitActionState UnitActionState = EUnitActionState.Empty;
        public int TargetGridPosIdx;
        public MoveActionData MoveActionData;
    
    }
    
    public class MoveData
    {
        public EFlyType FlyType = EFlyType.Empty;
        public Dictionary<int, MoveUnitData> MoveUnitDatas = new Dictionary<int, MoveUnitData>();

    }



    public class ActionData
    {
        public int ActionUnitID;
        public EActionDataType ActionDataType = EActionDataType.Unit;
        public Dictionary<int, List<TriggerData>> TriggerDatas = new ();
        public MoveData MoveData = new MoveData();
        public void AddEmptyTriggerDataList(int id)
        {
            if (!TriggerDatas.ContainsKey(id))
            {
                TriggerDatas.Add(id, new List<TriggerData>());
                
            }
        }
        
        public void AddTriggerData(int id, TriggerData triggerData, Data_BattleUnit effectUnit)
        {
            if(triggerData == null)
                return;
            
            if (!TriggerDatas.ContainsKey(id))
            {
                TriggerDatas.Add(id, new List<TriggerData>());
                
            }
            
            TriggerDatas[id].Add(triggerData);

            if (triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
                triggerData.BattleUnitAttribute == EUnitAttribute.HP &&
                triggerData.Value + triggerData.DeltaValue < 0)
            {
                if (effectUnit.GetAllStateCount(EUnitState.UnHurt) > 0)
                {
                    triggerData.TriggerResult = ETriggerResult.UnHurt;
                }
                // else if (effectUnit.GetAllStateCount(EUnitState.Dodge) > 0)
                // {
                //     effectUnit.RemoveState(EUnitState.Dodge);
                //     triggerData.TriggerResult = ETriggerResult.Dodge;
                // }

            }
                
        }

        public void Clear()
        {
            TriggerDatas.Clear();
        }
    }

    public class MoveActionData
    {
        public int ActionUnitID;
        public List<int> MoveGridPosIdxs = new ();
        public Dictionary<int, List<TriggerData>> TriggerDatas = new ();
    }

    public class TriggerData
    {
        public int OwnUnitID = -1;
        public int ActionUnitID = -1;
        public int EffectUnitID = -1;
        public EUnitAttribute BattleUnitAttribute = EUnitAttribute.Empty;
        public EHeroAttribute HeroAttribute = EHeroAttribute.Empty;
        public EUnitState UnitState = EUnitState.Empty;
        public ELinkID LinkID = ELinkID.Empty;
        public EBlessID BlessID = EBlessID.Empty;
        //public ECardID CardID = ECardID.Empty;
        public EBuffID BuffID = EBuffID.Empty;
        public ECardTriggerType CardTriggerType = ECardTriggerType.Empty;
        public ETriggerDataSubType TriggerDataSubType = ETriggerDataSubType.Empty;
        public ETriggerDataType TriggerDataType = ETriggerDataType.Empty;
        public float Value;
        public float DeltaValue;
        public ETriggerResult TriggerResult = ETriggerResult.Continue;

        public bool ChangeHPInstantly = false;
        //public bool AddHeroHP = true;
    }
    
    public class RoundFightData
    {
        public Dictionary<int, ActionData> UseCardTriggerDatas = new ();
        public Dictionary<int, ActionData> RoundStartBuffDatas = new();
        public Dictionary<int, ActionData> RoundStartUnitDatas = new();
        public Dictionary<int, ActionData> BlessTriggerDatas = new ();
        public Dictionary<int, ActionData> CurseTriggerDatas = new ();
        public Dictionary<int, ActionData> RoundEndDatas = new();
        public Dictionary<int, ActionData> SoliderAttackDatas = new ();
        public Dictionary<int, ActionData> SoliderActiveAttackDatas = new ();
        public Dictionary<int, ActionData> ThirdUnitAttackDatas = new ();
        public Dictionary<int, MoveActionData> EnemyMoveDatas = new ();
        public Dictionary<int, MoveActionData> ThirdUnitMoveDatas = new ();
        public Dictionary<int, MoveActionData> SoliderMoveDatas = new ();
        public Dictionary<int, ActionData> EnemyAttackDatas = new ();
        
        public List<TriggerData> UseCardDatas = new ();
        public ActionData BuffData_Use = new();
        
        public Data_GamePlay GamePlayData;
        public Dictionary<int, List<int>> EnemyMovePaths = new ();
        public Dictionary<int, List<int>> ThirdUnitMovePaths = new ();
        
        public TempTriggerData TempTriggerData;

        public void Clear()
        {
            RoundStartBuffDatas.Clear();
            RoundStartUnitDatas.Clear();
            RoundEndDatas.Clear();
            SoliderAttackDatas.Clear();
            SoliderActiveAttackDatas.Clear();
            EnemyMoveDatas.Clear();
            ThirdUnitMoveDatas.Clear();
            SoliderMoveDatas.Clear();
            EnemyAttackDatas.Clear();
            EnemyMovePaths.Clear();
            ThirdUnitMovePaths.Clear();
            ThirdUnitAttackDatas.Clear();
            BuffData_Use.Clear();
            BlessTriggerDatas.Clear();
            UseCardTriggerDatas.Clear();
        }
    }
    
    public enum EReachState
    {
        Unknow,
        Can,
        Cant,
    }
        
    public class PathState
    {
        public Vector2Int Coord;
        public int DisWithHero;
        public int MoveDis;
        public EReachState ReachState = EReachState.Unknow;
        public Vector2Int ObstacleCoord;

        public PathState(Vector2Int coord, int disWithHero, int moveDis)
        {
            Coord = coord;
            DisWithHero = disWithHero;
            MoveDis = moveDis;
        }
    }
        
    public class UnitTargetCoord
    {
        public Vector2Int Coord;
        public int GridPosIdx;
        //public int DisWithHero;
        public int MoveDis;
        public int AttackCount;
        
        public UnitTargetCoord(Vector2Int coord, int moveDis)
        {
            Coord = coord;
            //DisWithHero = disWithHero;
            MoveDis = moveDis;
        }
        
        public UnitTargetCoord()
        {
        }
    }



    public class BattleFightManager : Singleton<BattleFightManager>
    {
        public RoundFightData RoundFightData = new();

        private Dictionary<int, List<UnitActionRange>> UnitActionRange = new();

        // private Dictionary<int, Data_BattleSolider> Soliders;
        // private Dictionary<int, Data_BattleMonster> Enemies;

        public Dictionary<int, Data_BattleUnit> BattleUnitDatas = new Dictionary<int, Data_BattleUnit>();
        public Dictionary<int, Data_BattleUnit> BattleUnitDatasByGridPosIdx = new Dictionary<int, Data_BattleUnit>();

        private Dictionary<int, Data_BattleUnit> Roles;
        public EActionProgress ActionProgress;
        public int AcitonUnitIdx;

        private Dictionary<int, Data_GridPropMoveDirect> MoveDirectPropUseDict =
            new Dictionary<int, Data_GridPropMoveDirect>();

        private Dictionary<EBuffTriggerType, List<int>> ActionUnitIDs = new();

        public Dictionary<EGridPropID, string> CacheGridPorpIDStr = new ();

        public BattleFightManager()
        {
            foreach (EBuffTriggerType triggerType in Enum.GetValues(typeof(EBuffTriggerType)))
            {
                ActionUnitIDs.Add(triggerType, new List<int>(10));
            }
        }

        public Random Random;
        private int randomSeed;

        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);

        }

        public void Destory()
        {
            RoundFightData.Clear();
        }

        public Data_Player PlayerData;

        public void CacheRoundFightData()
        {
            //Log.Debug("CacheRoundFightData");

            //BattleAreaManager.Instance.RefreshObstacles();


            RoundFightData.Clear();

            RoundFightData.GamePlayData = GamePlayManager.Instance.GamePlayData.Copy();
            RoundFightData.TempTriggerData = BattleManager.Instance.TempTriggerData.Copy();

            PlayerData = RoundFightData.GamePlayData.GetPlayerData(BattleManager.Instance.CurUnitCamp);

            foreach (var kv in ActionUnitIDs)
            {
                kv.Value.Clear();
            }

            BattleUnitDatas = RoundFightData.GamePlayData.BattleData.BattleUnitDatas;
            
            RefreshUnitGridPosIdx();
            
            foreach (var kv in RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
                if (kv.Value is Data_BattleHero hero)
                {
                    PlayerData.BattleHero = hero;
                }
            }


            MoveDirectPropUseDict.Clear();
            foreach (var kv in RoundFightData.GamePlayData.BattleData.GridPropDatas)
            {
                if (kv.Value is Data_GridPropMoveDirect moveDirect)
                {
                    MoveDirectPropUseDict.Add(moveDirect.ID, moveDirect.Copy());

                }

            }

            if (BattleManager.Instance.TempTriggerData.TriggerType == ETempUnitType.NewUnit)
            {
                RoundFightData.GamePlayData.BattleData.GridTypes[RoundFightData.TempTriggerData.UnitData.GridPosIdx] =
                    EGridType.TemporaryUnit;

                var newUnitID = RoundFightData.TempTriggerData.UnitData.ID;

                if (RoundFightData.GamePlayData.BattleData.BattleUnitDatas.ContainsKey(newUnitID))
                {
                    RoundFightData.GamePlayData.BattleData.BattleUnitDatas.Remove(newUnitID);
                }

                RoundFightData.GamePlayData.BattleData.BattleUnitDatas.Add(newUnitID,
                    RoundFightData.TempTriggerData.UnitData);

                var soliderData = RoundFightData.TempTriggerData.UnitData as Data_BattleSolider;
                if (soliderData != null)
                {
                    FuneManager.Instance.CacheUnitUseData(newUnitID, newUnitID, soliderData.CardID,
                        BattleManager.Instance.CurUnitCamp,
                        RoundFightData.TempTriggerData.UnitData.GridPosIdx);
                }
            }
            else if (BattleManager.Instance.TempTriggerData.TriggerType == ETempUnitType.MoveUnit)
            {
                RoundFightData.GamePlayData.BattleData.GridTypes[
                    BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx] = EGridType.Empty;
                RoundFightData.GamePlayData.BattleData.GridTypes[
                    BattleManager.Instance.TempTriggerData.UnitData.GridPosIdx] = EGridType.TemporaryUnit;
            }
            if (BattleManager.Instance.TempTriggerData.TriggerType == ETempUnitType.NewUnit)
            {
                var newUnitID = RoundFightData.TempTriggerData.UnitData.ID;

            }
            else if (BattleManager.Instance.TempTriggerData.TriggerType == ETempUnitType.SelectHurtUnit)
            {
                RoundFightData.GamePlayData.BattleData.GridTypes[
                    BattleManager.Instance.TempTriggerData.UnitOriGridPosIdx] = EGridType.Empty;
                RoundFightData.GamePlayData.BattleData.GridTypes[
                    BattleManager.Instance.TempTriggerData.TargetGridPosIdx] = EGridType.TemporaryUnit;
            }
            else if (BattleManager.Instance.TempTriggerData.TriggerType == ETempUnitType.UseBuff)
            {
                var effectUnit =
                    RoundFightData.GamePlayData.BattleData.BattleUnitDatas.ContainsKey(BattleManager.Instance.TempTriggerData.CardEffectUnitID)
                        ? RoundFightData.GamePlayData.BattleData.BattleUnitDatas[
                            BattleManager.Instance.TempTriggerData.CardEffectUnitID]
                        : null;

                if (BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType == TriggerBuffType.Card)
                {
                    BattleCardManager.Instance.CacheTacticCardData(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardID,
                        BattleManager.Instance.CurUnitCamp, effectUnit);
                }
                else if (BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType == TriggerBuffType.EnergyBuff)
                {
                    BattleCardManager.Instance.CacheTacticCardData(BattleManager.Instance.TempTriggerData.TriggerBuffData.CardID,
                        BattleManager.Instance.CurUnitCamp, effectUnit);
                }

            }

            CacheLinks();
            
            CacheUseCardTriggerDatas();
            CacheSoliderActiveAttackDatas();
            CacheSoliderAutoAttackDatas();
            
            CacheRoundStartDatas();
            
            CacheSoliderMoveDatas();
            CacheSoliderAttackDatas();
            
            CalculateEnemyPaths();
            CacheEnemyMoveDatas();
            //CacheEnemyAttackDatas();
            
            CalculateThirdUnitPaths();
            CacheThirdUnitMoveDatas();
            CacheThirdUnitAttackDatas();
            
            CacheRoundEndDatas();
            
            
        }

        public void CacheUseCardTriggerDatas()
        {
            if (BattleCardManager.Instance.PointerCardIdx == -1)
                return;

            var unitID = BattleManager.Instance.TempTriggerData.UnitData != null
                ? BattleManager.Instance.TempTriggerData.UnitData.ID
                : -1;

            BattleFightManager.Instance.CacheConsumeCardEnergy(BattleCardManager.Instance.PointerCardIdx, unitID);
            //CacheUseCardTriggerAttack();
            CacheUseCardTrigger();
        }

        public void CacheConsumeCardEnergy(int cardID, int unitID = -1)
        {
            var cardEnergy = BattleCardManager.Instance.GetCardEnergy(cardID, unitID);
            // var solider = GameUtility.GetUnitByID(RoundFightData.GamePlayData, unitID);
            // if (solider != null)
            // {
            //     var subEnergyCount = solider.FuneCount(EBuffID.Spec_SubEnergy);
            //     if (subEnergyCount > 0 && cardEnergy > 0)
            //     {
            //         cardEnergy -= subEnergyCount;
            //         //cardEnergy = Math.Abs(cardEnergy);
            //         cardEnergy = cardEnergy < 0 ? 0 : cardEnergy;
            //     }
            // }

            var bless = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.EachUseCardUnUseEnergy,
                BattleManager.Instance.CurUnitCamp);
            if (bless == null || (bless != null && bless.Value > 0))
            {
                BattleFightManager.Instance.ChangeHP(BattleFightManager.Instance.PlayerData.BattleHero, -cardEnergy,
                    EHPChangeType.CardConsume, true, true);
            }
        }

        private void CacheUseCardTriggerAttack()
        {
            // foreach (var kv in BattleUnitDatas)
            // {
            //     if (kv.Value.CurHP <= 0)
            //         continue;
            //
            //     //kv.Value.GetAllStateCount(EUnitState.UnAction) > 0 ||
            //     if (kv.Value.GetAllStateCount(EUnitState.UnAttack) > 0 &&
            //         !GameUtility.ContainRoundState(RoundFightData.GamePlayData, ECardID.RoundDeBuffUnEffect))
            //         continue;
            //
            //     if (kv.Value.BuffCount(EBuffID.UseCardTriggerAttack) <= 0)
            //         continue;
            //
            //
            //     BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, kv.Value,
            //         out List<BuffValue> triggerBuffDatas);
            //
            //     if (triggerBuffDatas != null)
            //     {
            //         var actionData = new ActionData();
            //         actionData.ActionUnitID = kv.Value.ID;
            //         RoundFightData.UseCardTriggerDatas.Add(kv.Value.ID, actionData);
            //
            //         CacheAttackData(kv.Value.UnitCamp, triggerBuffDatas, kv.Value.GridPosIdx, actionData,
            //             kv.Value.ID, EBuffTriggerType.UseCard);
            //     }
            //
            //
            // }
        }

        private void CacheUseCardTrigger()
        {
            foreach (var kv in BattleUnitDatas)
            {
                if (kv.Value.CurHP <= 0)
                    continue;

                //(kv.Value.GetAllStateCount(EUnitState.UnAction) > 0 ||
                if (kv.Value.GetAllStateCount(EUnitState.UnAtk) > 0 &&
                    !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    continue;

                BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, kv.Value,
                    out List<BuffValue> triggerBuffDatas);

                if (triggerBuffDatas != null)
                {
                    var actionData = new ActionData();
                    actionData.ActionUnitID = kv.Value.ID;

                    var emptyTriggerData = new TriggerData();
                    var triggerDatas = new List<TriggerData>();
                    BattleBuffManager.Instance.BuffsTrigger(RoundFightData.GamePlayData,
                        kv.Value, emptyTriggerData, triggerDatas, EBuffTriggerType.UseCard);

                    foreach (var triggerData in triggerDatas)
                    {
                        actionData.AddTriggerData(kv.Value.ID, triggerData, kv.Value);
                    }

                    if (triggerDatas.Count > 0)
                    {
                        RoundFightData.UseCardTriggerDatas.Add(kv.Value.ID, actionData);
                    }

                }


            }
        }

        private void CacheRoundStartBuffDatas()
        {
            foreach (var kv in BattleUnitDatas)
            {
                var actionData = new ActionData();
                actionData.ActionUnitID = kv.Key;
                actionData.ActionDataType = EActionDataType.UnitState;
                var triggerDatas = new List<TriggerData>();

                var hurtRoundStartCount = kv.Value.GetAllStateCount(EUnitState.HurtRoundStart);
                if (hurtRoundStartCount > 0)
                {
                    if (!GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    {
                        var triggerData = BattleFightManager.Instance.BattleRoleAttribute(kv.Key, kv.Key, kv.Key,
                            EUnitAttribute.HP, -1, ETriggerDataSubType.Unit);

                        triggerData.UnitState = EUnitState.HurtRoundStart;

                        triggerDatas.Add(triggerData);

                        SimulateTriggerData(triggerData, triggerDatas);
                    }

                    kv.Value.RemoveState(EUnitState.HurtRoundStart);


                }
                
                // foreach (var funeID in kv.Value.FuneIDs)
                // {
                //     var triggerData = FuneManager.Instance.RoundTrigger(funeID, kv.Value.ID, kv.Value.ID, triggerDatas);
                //
                // }
                
                if (triggerDatas.Count > 0)
                {
                    RoundFightData.RoundStartBuffDatas.Add(kv.Key, actionData);
                    actionData.TriggerDatas.Add(kv.Key, triggerDatas);
                }

                CalculateHeroHPDelta(actionData);

            }
        }

        private void CacheRoundStartUnitDatas()
        {
            foreach (var kv in BattleUnitDatas)
            {
                var actionData = new ActionData();
                actionData.ActionUnitID = kv.Key;
                var triggerDatas = new List<TriggerData>();

                
                BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, kv.Value,
                    out List<BuffValue> triggerBuffDatas);

                if (triggerBuffDatas == null)
                    continue;
                
                //var attackWithoutHero = kv.Value.BuffCount(EBuffID.AttackWithoutHero) > 0;
                var idx = -1;
                foreach (var triggerBuffData in triggerBuffDatas)
                {
                    idx++;
                    if (triggerBuffData.BuffData.BuffTriggerType != EBuffTriggerType.RoundStart)
                        continue;

                    // || attackWithoutHero
                    var buffData = triggerBuffData.BuffData;
                    var range = GameUtility.GetRange(kv.Value.GridPosIdx, buffData.TriggerRange, kv.Value.UnitCamp,
                        buffData.TriggerUnitCamps,true);

                    foreach (var rangeGridPosIdx in range)
                    {
                        var unit = GetUnitByGridPosIdx(rangeGridPosIdx);
                        if (unit == null)
                            continue;

                        List<float> values = new List<float>();
                        // if (triggerBuffData.DrBuff.BuffID == EBuffID.Round_HurtSubDamageSameAttack)
                        // {
                        //     if ((RoundFightData.GamePlayData.BattleData.Round + 1) % triggerBuffData.ValueList[0] ==
                        //         triggerBuffData.ValueList[1])
                        //     {
                        //         values = new List<float>()
                        //             {Math.Abs(GetDamage(unit.ID))};
                        //     }
                        //     else
                        //     {
                        //         continue;
                        //     }
                        // }
                        // else if (triggerBuffData.DrBuff.BuffID == EBuffID.Round_AddDamageSameLastHurt ||
                        //          triggerBuffData.DrBuff.BuffID == EBuffID.Round_HurtSubDamageSameLastHurt)
                        // {
                        //     
                        //     if (unit.LastCurHPDelta < 0)
                        //     {
                        //         values = new List<float>()
                        //             {-unit.LastCurHPDelta};
                        //
                        //     }
                        //     else
                        //     {
                        //         continue;
                        //     }
                        // }
                        // else if (triggerBuffData.DrBuff.BuffID == EBuffID.Round_AddDamageSameEnemyCount ||
                        //          triggerBuffData.DrBuff.BuffID == EBuffID.Round_HurtSubDamageSameEnemyCount)
                        // {
                        //
                        //     var unitCount = GameUtility.GetUnitCount(RoundFightData.GamePlayData, unit.UnitCamp,
                        //         ERelativeCamp.Enemy);
                        //     if (unitCount >= 0)
                        //     {
                        //         values = new List<float>()
                        //             {unitCount};
                        //
                        //     }
                        //     else
                        //     {
                        //         continue;
                        //     }
                        // }
                        // else
                        // {
                        //     values = triggerBuffData.ValueList;
                        //
                        // }
                        values = triggerBuffData.ValueList;

                        BattleBuffManager.Instance.RoundStartTrigger(triggerBuffData.BuffData, values,
                            kv.Value.ID,
                            kv.Value.ID,
                            unit.ID, triggerDatas);


                    }

                }

                if (triggerDatas.Count > 0)
                {
                    RoundFightData.RoundStartUnitDatas.Add(kv.Key, actionData);
                    actionData.TriggerDatas.Add(kv.Key, triggerDatas);
                }

                CalculateHeroHPDelta(actionData);
            }
        }
        
        private void CacheUnitAutoAttackDatas(EUnitCamp unitCamp, List<BuffValue> triggerBuffDatas, int gridPosIdx,
            ActionData actionData, int attackUnitID)
        {
            foreach (var triggerBuffData in triggerBuffDatas)
            {
                if (triggerBuffData.BuffData.BuffTriggerType != EBuffTriggerType.AutoAttack)
                    continue;


                var range = GameUtility.GetRange(gridPosIdx, triggerBuffData.BuffData.TriggerRange, unitCamp,
                    triggerBuffData.BuffData.TriggerUnitCamps, true);

                foreach (var rangeGridPosIdx in range)
                {
                    var unit = GetUnitByGridPosIdx(rangeGridPosIdx);
                    if (unit == null)
                        continue;

                    List<float> values = triggerBuffData.ValueList;
                    
                    actionData.AddEmptyTriggerDataList(unit.ID);
                    var triggerDatas = actionData.TriggerDatas[unit.ID];

                    BattleBuffManager.Instance.AutoAttackTrigger(triggerBuffData.BuffData, values,
                        attackUnitID,
                        attackUnitID,
                        unit.ID, triggerDatas);


                }

            }

        }
        
        private void CacheRoundStartDatas()
        {
            BattleCurseManager.Instance.CacheRoundStartDatas();
            BlessManager.Instance.CacheRoundStartDatas();
        
            CacheRoundStartBuffDatas();
            CacheRoundStartUnitDatas();

        }


        private void CacheRoundEndDatas()
        {
            var actionData = BattleCurseManager.Instance.CacheAttackMostUnit_Attack();
            CalculateHeroHPDelta(actionData);
        }

        private void CacheAttackData(EUnitCamp unitCamp, List<BuffValue> triggerBuffDatas, int gridPosIdx,
            ActionData actionData, int attackUnitID, EBuffTriggerType buffTriggerType)
        {
            var attackUnit = GetUnitByID(attackUnitID);
            //var attackWithoutHero = attackUnit.BuffCount(EBuffID.AttackWithoutHero) > 0;

            foreach (var triggerBuffData in triggerBuffDatas)
            {
                // || attackWithoutHero
                var range = GameUtility.GetRange(gridPosIdx, triggerBuffData.BuffData.TriggerRange, unitCamp, triggerBuffData.BuffData.TriggerUnitCamps, true);

                var isSubCurHP = false;
   
                foreach (var rangeGridPosIdx in range)
                {
                    var unit = GetUnitByGridPosIdx(rangeGridPosIdx);
                    if (unit == null)
                        continue;

                    if (unit.CurHP <= 0)
                        continue;

                    var triggerDatas = new List<TriggerData>();
                    if (!actionData.TriggerDatas.ContainsKey(unit.ID))
                    {
                        actionData.TriggerDatas.Add(unit.ID, triggerDatas);
                    }

                    var triggerData = BattleBuffManager.Instance.BuffTrigger(buffTriggerType, triggerBuffData.BuffData,
                        triggerBuffData.ValueList, attackUnitID,
                        attackUnitID,
                        unit.ID, null, triggerDatas);

                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        isSubCurHP = true;
                    }
                }

                if (isSubCurHP)
                {
                    var triggerDatas = actionData.TriggerDatas.Values.ToList()[actionData.TriggerDatas.Count - 1];
                    BattleBuffManager.Instance.AttackTrigger(triggerDatas[0], triggerDatas);
                    BattleUnitStateManager.Instance.CheckUnitState(attackUnitID, triggerDatas);
                }
            }
        }
        
        private void CacheUnitActiveAttackData(EUnitCamp unitCamp, List<BuffValue> triggerBuffDatas, int gridPosIdx,
            ActionData actionData, int attackUnitID, int effectGridPosIdx)
        {
            foreach (var triggerBuffData in triggerBuffDatas)
            {
                var isSubCurHP = false;
                
                var effectUnit = GetUnitByGridPosIdx(effectGridPosIdx);
                
                if (effectUnit != null)
                {
                    if (effectUnit.CurHP <= 0)
                        continue;
                    
                    actionData.AddEmptyTriggerDataList(effectUnit.ID);
                    var triggerDatas = actionData.TriggerDatas[effectUnit.ID];
                
                    var triggerData = BattleBuffManager.Instance.BuffTrigger(triggerBuffData.BuffData.BuffTriggerType, triggerBuffData.BuffData,
                        triggerBuffData.ValueList, attackUnitID,
                        attackUnitID,
                        effectUnit.ID, null, triggerDatas);

                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        isSubCurHP = true;
                    }
                
                    if (isSubCurHP)
                    {
                        BattleBuffManager.Instance.AttackTrigger(triggerDatas[0], triggerDatas);
                        BattleUnitStateManager.Instance.CheckUnitState(attackUnitID, triggerDatas);
                    }
                    
                }

                CacheUnitActiveMoveDatas(attackUnitID, effectGridPosIdx, triggerBuffData.BuffData, actionData);

            }
        }

        private void CacheUnitActiveMoveDatas(int actionUnitID, int effectGridPosIdx, BuffData buffData, ActionData actionData)
        {
            if(effectGridPosIdx == -1)
                return;
            
            var actionUnit = GetUnitByID(actionUnitID);
            var effectUnit = GetUnitByGridPosIdx(effectGridPosIdx);
            

            var actionUnitCoord = GameUtility.GridPosIdxToCoord(actionUnit.GridPosIdx);

            
            var flyDirect = Vector2Int.zero;
            List<int> flyPaths;
            List<int> newFlyPaths = new List<int>();
            var moveActionDatas = new Dictionary<int, MoveActionData>();
            
            //var buffData = BattleBuffManager.Instance.GetBuffData(buffID);
            actionData.MoveData.FlyType = buffData.FlyType;

            if (effectUnit == null &&
                (buffData.FlyType != EFlyType.SelfCross))
                return;
            
            var effectUnitCoord = GameUtility.GridPosIdxToCoord(effectGridPosIdx);
            
            switch (buffData.FlyType)
            {
                case EFlyType.OtherBack:
                    flyDirect = effectUnitCoord - actionUnitCoord;
                        
                    flyPaths = GetFlyPaths(effectUnit.GridPosIdx, flyDirect);
                        
                    
                    CacheUnitMoveDatas(effectUnit.ID, flyPaths, moveActionDatas);

                    if (moveActionDatas.Count > 0)
                    {
                        actionData.MoveData.MoveUnitDatas.Add(effectUnit.ID, new MoveUnitData()
                        {
                            UnitID = effectUnit.ID,
                            MoveActionData = moveActionDatas[effectUnit.ID],
                            UnitActionState = EUnitActionState.Fly,
                        });
                        
                    }

                    break;
                case EFlyType.SelfBack:
                    flyDirect = actionUnitCoord - effectUnitCoord;
                    flyPaths = GetFlyPaths(actionUnit.GridPosIdx, flyDirect);

                    CacheUnitMoveDatas(actionUnit.ID, flyPaths, moveActionDatas);
                    if (moveActionDatas.Count > 0)
                    {
                        actionData.MoveData.MoveUnitDatas.Add(actionUnit.ID, new MoveUnitData()
                        {
                            UnitID = actionUnit.ID,
                            MoveActionData = moveActionDatas[actionUnit.ID],
                            UnitActionState = EUnitActionState.Fly,
                        });

                    }
                    break;
                case EFlyType.OtherClose:
                    flyDirect = actionUnitCoord - effectUnitCoord;
                        
                    flyPaths = GetFlyPaths(effectUnit.GridPosIdx, flyDirect);
                    
                    for (int i = 0; i < flyPaths.Count; i++)
                    {
                        if (flyPaths[i] == actionUnit.GridPosIdx)
                        {
                            break;
                        }
                        newFlyPaths.Add(flyPaths[i]);
                    }
                        
                    
                    CacheUnitMoveDatas(effectUnit.ID, newFlyPaths, moveActionDatas);

                    if (moveActionDatas.Count > 0)
                    {
                        actionData.MoveData.MoveUnitDatas.Add(effectUnit.ID, new MoveUnitData()
                        {
                            UnitID = effectUnit.ID,
                            MoveActionData = moveActionDatas[effectUnit.ID],
                            UnitActionState = EUnitActionState.Fly,
                        });
                        
                    }
                    break;
                case EFlyType.SelfClose:
                    flyDirect = effectUnitCoord - actionUnitCoord;
                        
                    flyPaths = GetFlyPaths(actionUnit.GridPosIdx, flyDirect);
                    
                    for (int i = 0; i < flyPaths.Count; i++)
                    {
                        if (flyPaths[i] == effectUnit.GridPosIdx)
                        {
                            break;
                        }
                        newFlyPaths.Add(flyPaths[i]);
                    }
                        
                    
                    CacheUnitMoveDatas(actionUnit.ID, newFlyPaths, moveActionDatas);

                    if (moveActionDatas.Count > 0)
                    {
                        actionData.MoveData.MoveUnitDatas.Add(actionUnit.ID, new MoveUnitData()
                        {
                            UnitID = actionUnit.ID,
                            MoveActionData = moveActionDatas[actionUnit.ID],
                            UnitActionState = EUnitActionState.Fly,
                        });
                        
                    }
                    break;
                // case EFlyType.AllBack:
                //     break;
                // case EFlyType.AllClose:
                //     break;
                // case EFlyType.CrossOtherBack1:
                //     break;
                case EFlyType.Exchange:
                    
                    actionData.MoveData.MoveUnitDatas.Add(actionUnit.ID, new MoveUnitData()
                    {
                        UnitID = actionUnit.ID,
                        MoveActionData = new MoveActionData()
                        {
                            ActionUnitID = actionUnit.ID,
                            MoveGridPosIdxs = new List<int>()
                            {
                                actionUnit.GridPosIdx,
                                effectUnit.GridPosIdx,
                            }
                        },
                        UnitActionState = EUnitActionState.Fly,
                    });
                    actionData.MoveData.MoveUnitDatas.Add(effectUnit.ID, new MoveUnitData()
                    {
                        UnitID = effectUnit.ID,
                        MoveActionData = new MoveActionData()
                        {
                            ActionUnitID = effectUnit.ID,
                            MoveGridPosIdxs = new List<int>()
                            {
                                effectUnit.GridPosIdx,
                                actionUnit.GridPosIdx,
                            }
                        },
                        UnitActionState = EUnitActionState.Fly,
                    });
                    
                    break;
                case EFlyType.SelfCross:
                    flyDirect = effectUnitCoord - actionUnitCoord;
                    flyPaths = GetFlyPaths(actionUnit.GridPosIdx, effectGridPosIdx);

                    CacheUnitMoveDatas(actionUnit.ID, flyPaths, moveActionDatas);
                    if (moveActionDatas.Count > 0)
                    {
                        actionData.MoveData.MoveUnitDatas.Add(actionUnit.ID, new MoveUnitData()
                        {
                            UnitID = actionUnit.ID,
                            MoveActionData = moveActionDatas[actionUnit.ID],
                            UnitActionState = EUnitActionState.Fly,
                        });

                    }
                    break;
                case EFlyType.BackToSelf:
                    flyDirect = actionUnitCoord - effectUnitCoord;
                    flyPaths = GetFlyPaths(effectUnit.GridPosIdx, flyDirect);

                    CacheUnitMoveDatas(effectUnit.ID, flyPaths, moveActionDatas);
                    if (moveActionDatas.Count > 0)
                    {
                        actionData.MoveData.MoveUnitDatas.Add(effectUnit.ID, new MoveUnitData()
                        {
                            UnitID = effectUnit.ID,
                            MoveActionData = moveActionDatas[effectUnit.ID],
                            UnitActionState = EUnitActionState.Fly,
                        });

                    }
                    break;
                case EFlyType.OtherBack1:
                    flyDirect = effectUnitCoord - actionUnitCoord;
                    flyDirect = GameUtility.GetDirect(flyDirect);
                    
                    flyPaths = GetFlyPaths(effectUnit.GridPosIdx, GameUtility.GridCoordToPosIdx(effectUnitCoord + flyDirect));
                    
                    CacheUnitMoveDatas(effectUnit.ID, flyPaths, moveActionDatas);

                    if (moveActionDatas.Count > 0)
                    {
                        actionData.MoveData.MoveUnitDatas.Add(effectUnit.ID, new MoveUnitData()
                        {
                            UnitID = effectUnit.ID,
                            MoveActionData = moveActionDatas[effectUnit.ID],
                            UnitActionState = EUnitActionState.Fly,
                        });
                        
                    }
                    break;
                case EFlyType.SelfBack1:
                    flyDirect = actionUnitCoord - effectUnitCoord;
                    flyDirect = GameUtility.GetDirect(flyDirect);
                    flyPaths = GetFlyPaths(actionUnit.GridPosIdx, GameUtility.GridCoordToPosIdx(actionUnitCoord + flyDirect));

                    CacheUnitMoveDatas(actionUnit.ID, flyPaths, moveActionDatas);
                    if (moveActionDatas.Count > 0)
                    {
                        actionData.MoveData.MoveUnitDatas.Add(actionUnit.ID, new MoveUnitData()
                        {
                            UnitID = actionUnit.ID,
                            MoveActionData = moveActionDatas[actionUnit.ID],
                            UnitActionState = EUnitActionState.Fly,
                        });

                    }
                    break;
                case EFlyType.OtherClose1:
                    flyDirect = actionUnitCoord - effectUnitCoord;
                    flyDirect = GameUtility.GetDirect(flyDirect);    
                    flyPaths = GetFlyPaths(effectUnit.GridPosIdx, GameUtility.GridCoordToPosIdx(effectUnitCoord + flyDirect));

                    CacheUnitMoveDatas(effectUnit.ID, flyPaths, moveActionDatas);

                    if (moveActionDatas.Count > 0)
                    {
                        actionData.MoveData.MoveUnitDatas.Add(effectUnit.ID, new MoveUnitData()
                        {
                            UnitID = effectUnit.ID,
                            MoveActionData = moveActionDatas[effectUnit.ID],
                            UnitActionState = EUnitActionState.Fly,
                        });
                        
                    }
                    break;
                case EFlyType.SelfClose1:
                    flyDirect = effectUnitCoord - actionUnitCoord;
                    flyDirect = GameUtility.GetDirect(flyDirect);    
                    flyPaths = GetFlyPaths(actionUnit.GridPosIdx, GameUtility.GridCoordToPosIdx(actionUnitCoord + flyDirect));
                    
                    for (int i = 0; i < flyPaths.Count; i++)
                    {
                        if (flyPaths[i] == effectUnit.GridPosIdx)
                        {
                            break;
                        }
                        newFlyPaths.Add(flyPaths[i]);
                    }
                        
                    
                    CacheUnitMoveDatas(actionUnit.ID, newFlyPaths, moveActionDatas);

                    if (moveActionDatas.Count > 0)
                    {
                        actionData.MoveData.MoveUnitDatas.Add(actionUnit.ID, new MoveUnitData()
                        {
                            UnitID = actionUnit.ID,
                            MoveActionData = moveActionDatas[actionUnit.ID],
                            UnitActionState = EUnitActionState.Fly,
                        });
                        
                    }
                    break;
                case EFlyType.Empty:
                    break;
                default:
                    break;
            }
            Dictionary<int, List<int>> uniPaths = new ();
            CalculateUnitPaths(EUnitCamp.Third, RoundFightData.ThirdUnitMovePaths);
        }
        
        private void CacheSoliderAttackDatas()
        {
            CacheLinks();
            foreach (var kv in BattleUnitDatas)
            {
                var soliderData = kv.Value as Data_BattleSolider;
                if (soliderData == null)
                    continue;

                if (kv.Value.CurHP <= 0)
                    continue;

                //kv.Value.GetAllStateCount(EUnitState.UnAction) > 0 ||
                if (kv.Value.GetAllStateCount(EUnitState.UnAtk) > 0 &&
                    !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    continue;
                
                var actionData = new ActionData();
                actionData.ActionUnitID = soliderData.ID;
                RoundFightData.SoliderAttackDatas.Add(soliderData.ID, actionData);

                BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, kv.Value,
                    out List<BuffValue> triggerBuffDatas);
                CacheAttackData(BattleManager.Instance.CurUnitCamp, triggerBuffDatas, kv.Value.GridPosIdx, actionData,
                    soliderData.ID, EBuffTriggerType.ActionEnd);
                CalculateHeroHPDelta(actionData);

            }

        }

        private void CacheEnemyAttackData(Data_BattleMonster enemyData)
        {
            if (enemyData == null)
                return;

            if (enemyData.CurHP <= 0)
                return;

            if ((enemyData.GetAllStateCount(EUnitState.UnAtk) > 0) &&
                !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                return;
            
            var actionData = new ActionData();
            actionData.ActionUnitID = enemyData.ID;

            RoundFightData.EnemyAttackDatas.Add(enemyData.ID, actionData);
            var drEnemy = GameEntry.DataTable.GetEnemy(enemyData.MonsterID);
            var buffData = BattleBuffManager.Instance.GetBuffData(drEnemy.OwnBuffs[0]);
                
            var attackRange = GameUtility.GetRange(enemyData.GridPosIdx, buffData.TriggerRange, EUnitCamp.Enemy,
                buffData.TriggerUnitCamps, true);

            if (attackRange.Count > 0)
            {
                BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, enemyData,
                    out List<BuffValue> triggerBuffDatas);
                CacheAttackData(EUnitCamp.Enemy, triggerBuffDatas, enemyData.GridPosIdx, actionData, enemyData.ID, EBuffTriggerType.ActionEnd);
            }
            else
            {
                BattleUnitManager.Instance.GetSecondaryBuffValue(RoundFightData.GamePlayData, enemyData,
                    out List<BuffValue> secondaryTriggerBuffDatas);
                CacheAttackData(EUnitCamp.Enemy, secondaryTriggerBuffDatas, enemyData.GridPosIdx, actionData, enemyData.ID, EBuffTriggerType.ActionEnd);
            }

                
                

            CalculateHeroHPDelta(actionData);

        }

        private void CacheEnemyAttackDatas()
        {

            foreach (var kv in BattleUnitDatas)
            {
                if (kv.Value.UnitCamp != EUnitCamp.Enemy)
                    continue;

                if (kv.Value.CurHP <= 0)
                    continue;


                if (!RoundFightData.EnemyMovePaths.ContainsKey(kv.Key) ||
                    RoundFightData.EnemyMovePaths[kv.Key] == null || RoundFightData.EnemyMovePaths[kv.Key].Count <= 0)
                    continue;

                kv.Value.GridPosIdx =
                    RoundFightData.EnemyMovePaths[kv.Key][RoundFightData.EnemyMovePaths[kv.Key].Count - 1];

            }

            CacheLinks();

            foreach (var kv in BattleUnitDatas)
            {
                if (kv.Value.UnitCamp != EUnitCamp.Enemy)
                    continue;

                var enemyData = kv.Value as Data_BattleMonster;
                if (enemyData == null)
                    continue;

                if (kv.Value.CurHP <= 0)
                    continue;

                //kv.Value.GetAllStateCount(EUnitState.UnAction) > 0 ||
                if ((kv.Value.GetAllStateCount(EUnitState.UnAtk) > 0) &&
                    !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    continue;


                var actionData = new ActionData();
                actionData.ActionUnitID = enemyData.ID;

                RoundFightData.EnemyAttackDatas.Add(enemyData.ID, actionData);
                var drEnemy = GameEntry.DataTable.GetEnemy(enemyData.MonsterID);
                var buffData = BattleBuffManager.Instance.GetBuffData(drEnemy.OwnBuffs[0]);
                
                var attackRange = GameUtility.GetRange(enemyData.GridPosIdx, buffData.TriggerRange, EUnitCamp.Enemy,
                    buffData.TriggerUnitCamps, true);

                if (attackRange.Count > 0)
                {
                    BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, kv.Value,
                        out List<BuffValue> triggerBuffDatas);
                    CacheAttackData(EUnitCamp.Enemy, triggerBuffDatas, kv.Value.GridPosIdx, actionData, enemyData.ID, EBuffTriggerType.ActionEnd);
                }
                else
                {
                    BattleUnitManager.Instance.GetSecondaryBuffValue(RoundFightData.GamePlayData, kv.Value,
                        out List<BuffValue> secondaryTriggerBuffDatas);
                    CacheAttackData(EUnitCamp.Enemy, secondaryTriggerBuffDatas, kv.Value.GridPosIdx, actionData, enemyData.ID, EBuffTriggerType.ActionEnd);
                }

                
                

                CalculateHeroHPDelta(actionData);

            }
        }


        private void CacheThirdUnitAttackDatas()
        {
            foreach (var kv in BattleUnitDatas)
            {
                if (kv.Value.UnitCamp != EUnitCamp.Third)
                    continue;

                if (kv.Value.CurHP <= 0)
                    continue;


                if (!RoundFightData.ThirdUnitMovePaths.ContainsKey(kv.Key) ||
                    RoundFightData.ThirdUnitMovePaths[kv.Key] == null ||
                    RoundFightData.ThirdUnitMovePaths[kv.Key].Count <= 0)
                    continue;

                kv.Value.GridPosIdx =
                    RoundFightData.ThirdUnitMovePaths[kv.Key][RoundFightData.ThirdUnitMovePaths[kv.Key].Count - 1];

            }

            CacheLinks();

            foreach (var kv in BattleUnitDatas)
            {
                if (kv.Value.UnitCamp != EUnitCamp.Third)
                    continue;

                var enemyData = kv.Value as Data_BattleMonster;
                if (enemyData == null)
                    continue;

                if (kv.Value.CurHP <= 0)
                    continue;

                //kv.Value.GetAllStateCount(EUnitState.UnAction) > 0 ||
                if ((kv.Value.GetAllStateCount(EUnitState.UnAtk) > 0) &&
                    !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    continue;

                var actionData = new ActionData();
                actionData.ActionUnitID = enemyData.ID;

                RoundFightData.ThirdUnitAttackDatas.Add(enemyData.ID, actionData);

                BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, kv.Value,
                    out List<BuffValue> triggerBuffDatas);
                CacheAttackData(EUnitCamp.Third, triggerBuffDatas, kv.Value.GridPosIdx, actionData, enemyData.ID, EBuffTriggerType.ActionEnd);
                CalculateHeroHPDelta(actionData);
            }
        }
        
        private void CacheSoliderActiveAttackDatas()
        {
            var unitData = RoundFightData.TempTriggerData.UnitData;
            
            if (unitData == null)
                return;

            // if (effectUnitData == null)
            //     return;

            RoundFightData.SoliderActiveAttackDatas.Clear();
            if (unitData.CurHP <= 0)
                return;
            
            if(RoundFightData.TempTriggerData.TriggerType != ETempUnitType.ActiveAtk)
                return;
            
            // if (unitData.GetStateCount(EUnitState.ActiveAtk) <= 0)
            //     return;

            //soliderData.GetAllStateCount(EUnitState.UnAction) > 0 ||
            if ((unitData.GetAllStateCount(EUnitState.UnAtk) > 0) &&
                !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                return;
                
            var actionData = new ActionData();
            actionData.ActionUnitID = unitData.ID;
            RoundFightData.SoliderActiveAttackDatas.Add(unitData.ID, actionData);

            BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, unitData,
                out List<BuffValue> triggerBuffDatas);
            CacheUnitActiveAttackData(BattleManager.Instance.CurUnitCamp, triggerBuffDatas, unitData.GridPosIdx, actionData,
                unitData.ID, RoundFightData.TempTriggerData.TargetGridPosIdx);
            CalculateHeroHPDelta(actionData);

        }
        
        private void CacheSoliderAutoAttackDatas()
        {
            var unitData = RoundFightData.TempTriggerData.UnitData;
            
            if (unitData == null)
                return;

            if (!GameUtility.CheckUnitCamp(new List<ERelativeCamp>()
            {
                ERelativeCamp.Us
            }, BattleManager.Instance.CurUnitCamp, unitData.UnitCamp))
            {
                return;
            }
            
            if(RoundFightData.TempTriggerData.TriggerType != ETempUnitType.AutoAtk)
                return;
            
            // if (unitData.GetStateCount(EUnitState.AutoAtk) <= 0)
            //     return;

            if (unitData.CurHP <= 0)
                return;

            if ((unitData.GetAllStateCount(EUnitState.UnAtk) > 0) &&
                !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                return;
                
            var actionData = new ActionData();
            actionData.ActionUnitID = unitData.ID;
            RoundFightData.SoliderActiveAttackDatas.Add(unitData.ID, actionData);

            BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, unitData,
                out List<BuffValue> triggerBuffDatas);
            CacheUnitAutoAttackDatas(BattleManager.Instance.CurUnitCamp, triggerBuffDatas, unitData.GridPosIdx, actionData,
                unitData.ID);
            CalculateHeroHPDelta(actionData);

        }


        public class MoveUnitStateData
        {
            public EUnitState UnitState;
            public bool IsBePass;

            public MoveUnitStateData(EUnitState unitState, bool isBePass)
            {
                UnitState = unitState;
                IsBePass = isBePass;
            }


        }

        private static bool ContainsMoveUnitStateData(List<MoveUnitStateData> moveUnitStateDatas, EUnitState unitState,
            bool isBePass)
        {
            if (moveUnitStateDatas == null)
                return false;

            return moveUnitStateDatas.Any(data => data.UnitState == unitState && data.IsBePass == isBePass);
        }

        private List<MoveUnitStateData> MoveTrigger(int moveIdx, Data_BattleUnit passUnit, Data_BattleUnit bePassUnit,
            List<TriggerData> triggerDatas, List<MoveUnitStateData> preMoveUnitStateDatas = null)
        {
            var moveUnitStateDatas = new List<MoveUnitStateData>();

            if (passUnit == null || bePassUnit == null)
                return moveUnitStateDatas;

            var bePassUnitAttackPassUs = moveIdx != 0 && bePassUnit != null
                ? bePassUnit.GetAllStateCount(EUnitState.AtkPassUs)
                : 0;
            var passUnitAttackPassUs =
                moveIdx != 0 && passUnit != null ? passUnit.GetAllStateCount(EUnitState.AtkPassUs) : 0;
            var bePassUnitAttackPassEnemy = moveIdx != 0 && bePassUnit != null
                ? bePassUnit.GetAllStateCount(EUnitState.AtkPassEnemy)
                : 0;
            var passUnitAttackPassEnemy = moveIdx != 0 && passUnit != null
                ? passUnit.GetAllStateCount(EUnitState.AtkPassEnemy)
                : 0;

            if (bePassUnitAttackPassUs > 0 && passUnit.UnitCamp == bePassUnit.UnitCamp)
            {

                if (!ContainsMoveUnitStateData(preMoveUnitStateDatas, EUnitState.AtkPassUs, true) &&
                    !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                {

                    //passUnit.GetAllStateCount(EUnitState.UnAction) > 0 ||
                    if ((passUnit.GetAllStateCount(EUnitState.UnMove) > 0))
                        return null;

                    var value = -bePassUnitAttackPassUs;
                    var battlePlayerData =
                        BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(bePassUnit
                            .UnitCamp);
                    if (battlePlayerData.RoundBuffs.Contains(EBuffID.Spec_AtkPassEnemyAddDmg_AtkPassUsAddDmg))
                    {
                        value += value < 0 ? 1 : 0;
                    }

                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(bePassUnit.ID,
                        bePassUnit.ID, passUnit.ID,
                        EUnitAttribute.HP, value, ETriggerDataSubType.Unit);
                    triggerData.UnitState = EUnitState.AtkPassUs;
                    bePassUnit.RemoveState(EUnitState.AtkPassUs);

                    BattleBuffManager.Instance.PostTrigger(triggerData, triggerDatas);

                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        BattleBuffManager.Instance.AttackTrigger(triggerData, triggerDatas);
                        BattleUnitStateManager.Instance.CheckUnitState(bePassUnit.ID, triggerDatas);
                    }

                    moveUnitStateDatas.Add(new MoveUnitStateData(EUnitState.AtkPassUs, true));

                }

            }

            if (passUnitAttackPassUs > 0 && passUnit.UnitCamp == bePassUnit.UnitCamp)
            {
                if (!ContainsMoveUnitStateData(preMoveUnitStateDatas, EUnitState.AtkPassUs, false) &&
                    !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                {
                    var value = -passUnitAttackPassUs;
                    var battlePlayerData =
                        BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(
                            passUnit.UnitCamp);
                    if (battlePlayerData.RoundBuffs.Contains(EBuffID.Spec_AtkPassEnemyAddDmg_AtkPassUsAddDmg))
                    {
                        value += value < 0 ? 1 : 0;
                    }

                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(passUnit.ID,
                        passUnit.ID, bePassUnit.ID,
                        EUnitAttribute.HP, value, ETriggerDataSubType.Unit);
                    triggerData.UnitState = EUnitState.AtkPassUs;
                    passUnit.RemoveState(EUnitState.AtkPassUs);

                    BattleBuffManager.Instance.PostTrigger(triggerData, triggerDatas);

                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        BattleBuffManager.Instance.AttackTrigger(triggerData, triggerDatas);
                        BattleUnitStateManager.Instance.CheckUnitState(passUnit.ID, triggerDatas);
                    }

                    moveUnitStateDatas.Add(new MoveUnitStateData(EUnitState.AtkPassUs, false));

                }
            }

            if (bePassUnitAttackPassEnemy > 0 && passUnit.UnitCamp != bePassUnit.UnitCamp)
            {
                if (!ContainsMoveUnitStateData(preMoveUnitStateDatas, EUnitState.AtkPassEnemy, true))
                {
                    var value = -bePassUnitAttackPassEnemy;
                    var battlePlayerData =
                        BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(bePassUnit
                            .UnitCamp);
                    if (battlePlayerData.RoundBuffs.Contains(EBuffID.Spec_AtkPassEnemyAddDmg_AtkPassUsAddDmg))
                    {
                        value += value < 0 ? -1 : 0;
                    }

                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(bePassUnit.ID, bePassUnit.ID,
                        passUnit.ID, EUnitAttribute.HP, value, ETriggerDataSubType.Unit);
                    triggerData.UnitState = EUnitState.AtkPassEnemy;
                    bePassUnit.RemoveState(EUnitState.AtkPassEnemy);

                    BattleBuffManager.Instance.PostTrigger(triggerData, triggerDatas);

                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        BattleBuffManager.Instance.AttackTrigger(triggerData, triggerDatas);
                        BattleUnitStateManager.Instance.CheckUnitState(bePassUnit.ID, triggerDatas);
                    }

                    moveUnitStateDatas.Add(new MoveUnitStateData(EUnitState.AtkPassEnemy, true));

                }
            }

            if (passUnitAttackPassEnemy > 0 && passUnit.UnitCamp != bePassUnit.UnitCamp)
            {
                if (!ContainsMoveUnitStateData(preMoveUnitStateDatas, EUnitState.AtkPassEnemy, false))
                {
                    var value = -bePassUnitAttackPassEnemy;
                    var battlePlayerData =
                        BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(
                            passUnit.UnitCamp);
                    if (battlePlayerData.RoundBuffs.Contains(EBuffID.Spec_AtkPassEnemyAddDmg_AtkPassUsAddDmg))
                    {
                        value += value < 0 ? -1 : 0;
                    }

                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(passUnit.ID, passUnit.ID,
                        bePassUnit.ID, EUnitAttribute.HP, value, ETriggerDataSubType.Unit);
                    triggerData.UnitState = EUnitState.AtkPassEnemy;
                    bePassUnit.RemoveState(EUnitState.AtkPassEnemy);

                    BattleBuffManager.Instance.PostTrigger(triggerData, triggerDatas);
                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        BattleBuffManager.Instance.AttackTrigger(triggerData, triggerDatas);
                        BattleUnitStateManager.Instance.CheckUnitState(passUnit.ID, triggerDatas);
                    }

                    moveUnitStateDatas.Add(new MoveUnitStateData(EUnitState.AtkPassEnemy, false));

                }
            }

            return moveUnitStateDatas;
        }


        public List<int> CacheUnitMoveDatas(int unitID, List<int> movePaths,
            Dictionary<int, MoveActionData> unitMoveDatas)
        {
            var passUnit = GetUnitByID(unitID);
            if (passUnit == null)
                return null;
            if (passUnit.CurHP <= 0)
                return null;

            var battlePlayerData = RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(BattleManager.Instance
                .CurUnitCamp);
            //passUnit.GetAllStateCount(EUnitState.UnAction) > 0 ||
            if ((passUnit.GetAllStateCount(EUnitState.UnMove) > 0) &&
                !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                return null;

            var moveActionData = new MoveActionData();
            moveActionData.ActionUnitID = passUnit.ID;
            unitMoveDatas.Add(passUnit.ID, moveActionData);

            // var tempFirstEnemyActionSoliders = new List<int>();
            // var hpRefreshTriggerTypeSoliders = new List<int>();


            var minFullPaths = new List<int>();

            for (int i = 0; i < movePaths.Count; i++)
            {
                var preGridPosIdx = passUnit.GridPosIdx;
                var gridPosIdx = movePaths[i];
                var nextGridPosIdx = -1;
                if (i + 1 < movePaths.Count)
                {
                    nextGridPosIdx = movePaths[i + 1];
                }
                
                passUnit.GridPosIdx = gridPosIdx;
                CacheLinks();

                var triggerDatas = new List<TriggerData>();

                // if (moveUnitCamp == EUnitCamp.Enemy)
                // {
                //     TriggerPassAuxiliary(gridPosIdx, unitID, triggerDatas);
                // }
                var bePassUnit = GetUnitByGridPosIdx(gridPosIdx, null, null, null, unitID);
                //var isPassTrigger = false;

                var hurtEachMoveCount = passUnit.GetAllStateCount(EUnitState.HurtEachMove);

                var preMoveTriggerDatas = MoveTrigger(i, passUnit, bePassUnit, triggerDatas);

                if (hurtEachMoveCount > 0 && i > 0)
                {
                    if (!GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    {
                        var hurtEachMoveTriggerData = BattleFightManager.Instance.BattleRoleAttribute(passUnit.ID,
                            passUnit.ID,
                            passUnit.ID,
                            EUnitAttribute.HP, -hurtEachMoveCount, ETriggerDataSubType.Unit);
                        hurtEachMoveTriggerData.UnitState = EUnitState.HurtEachMove;
                        SimulateTriggerData(hurtEachMoveTriggerData, triggerDatas);
                        triggerDatas.Add(hurtEachMoveTriggerData);
                    }

                    passUnit.RemoveState(EUnitState.HurtEachMove);
                }

                if (bePassUnit != null)
                {
                    BattleUnitStateManager.Instance.HurtRoundStartMoveTrigger(passUnit.ID, bePassUnit.ID, triggerDatas);

                    // BattleBuffManager.Instance.BuffsTrigger(unit, triggerDatas, ETriggerType.Pass);
                    // BattleBuffManager.Instance.BuffsTrigger(bePassUnit, triggerDatas, ETriggerType.BePass);

                    BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, passUnit,
                        out List<BuffValue> passUnitTriggerBuffDatas);

                    if (passUnitTriggerBuffDatas != null)
                    {
                        foreach (var triggerBuffData in passUnitTriggerBuffDatas)
                        {
                            BattleBuffManager.Instance.BuffTrigger(EBuffTriggerType.Pass,
                                triggerBuffData.BuffData, triggerBuffData.ValueList, unitID, unitID, bePassUnit.ID,
                                triggerDatas, gridPosIdx, preGridPosIdx);
                        }
                    }

                    BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, bePassUnit,
                        out List<BuffValue> bePassUnitTriggerBuffDatas);

                    if (bePassUnitTriggerBuffDatas != null)
                    {
                        var idx = 0;
                        foreach (var triggerBuffData in bePassUnitTriggerBuffDatas)
                        {
                            BattleBuffManager.Instance.BuffTrigger(EBuffTriggerType.BePass,
                                triggerBuffData.BuffData, triggerBuffData.ValueList, bePassUnit.ID, bePassUnit.ID,
                                passUnit.ID,
                                triggerDatas, gridPosIdx, preGridPosIdx);
                            idx++;
                        }
                    }
                }

                var gridProp = BattleGridPropManager.Instance.GetGridProp(gridPosIdx);

                if (gridProp != null)
                {
                    var drGridProp = GameEntry.DataTable.GetGridProp(gridProp.GridPropID);
                    var idx = 1;
                    
                    foreach (var buffIDStr in drGridProp.GridPropIDs)
                    {
                        var buffData = BattleBuffManager.Instance.GetBuffData(buffIDStr);
                        BattleBuffManager.Instance.BuffTrigger(EBuffTriggerType.BePass,
                            buffData, BattleGridPropManager.Instance.GetValues(gridProp.GridPropID, idx), unitID, unitID, unitID,
                            triggerDatas, gridPosIdx, preGridPosIdx);
                        idx++;
                    }
                }

                MoveTrigger(i, passUnit, bePassUnit, triggerDatas, preMoveTriggerDatas);

                TriggerUnitData(unitID, bePassUnit == null ? passUnit.ID : bePassUnit.ID, gridPosIdx, EBuffTriggerType.Move, triggerDatas);
                
                CacheUnitRangeDatas(passUnit.ID, preGridPosIdx, gridPosIdx, triggerDatas);

                if (nextGridPosIdx == preGridPosIdx && bePassUnit != null)
                {
                    var collisionTriggerData = BattleFightManager.Instance.BattleRoleAttribute(unitID, unitID,
                        bePassUnit.ID, EUnitAttribute.HP, -1, ETriggerDataSubType.Unit);
                    BattleBuffManager.Instance.CacheTriggerData(collisionTriggerData, triggerDatas);
                    
                    var collisionTriggerData2 = BattleFightManager.Instance.BattleRoleAttribute(bePassUnit.ID, bePassUnit.ID,
                        unitID, EUnitAttribute.HP, -1, ETriggerDataSubType.Unit);
                    BattleBuffManager.Instance.CacheTriggerData(collisionTriggerData2, triggerDatas);
                }
                
                if (triggerDatas.Count > 0)
                {
                    moveActionData.TriggerDatas.Add(i, triggerDatas);
                }

                minFullPaths.Add(gridPosIdx);

                if (passUnit.CurHP <= 0)
                {
                    //i为终点，表现上，敌人行动终点和我方单位重合，所以，让敌人多走一格，绕过我方单位
                    // if (i == movePaths.Count - 1)
                    // {
                    //     minFullPaths.Add(movePaths[i + 1]);
                    // }
                    break;
                }

                // if ((passUnit.GetAllStateCount(EUnitState.UnAction) > 0 ||
                //      passUnit.GetAllStateCount(EUnitState.UnMove) > 0) &&
                //     !GameUtility.ContainRoundState(RoundFightData.GamePlayData, ECardID.DeBuffUnEffect))
                //     break;

                

            }
            
            CalculateHeroHPDelta(moveActionData);

            moveActionData.MoveGridPosIdxs = new List<int>(minFullPaths);
            // if (tempFirstEnemyActionSoliders.Count > 0)
            // {
            //     firstUnitActionSoliders.AddRange(tempFirstEnemyActionSoliders);
            // }

            if (minFullPaths.Count > 0)
            {
                passUnit.GridPosIdx = minFullPaths[minFullPaths.Count - 1];
                RefreshUnitGridPosIdx();
                if (passUnit.UnitCamp == EUnitCamp.Enemy)
                {
                    if (!RoundFightData.EnemyAttackDatas.ContainsKey(passUnit.ID))
                    {
                        CacheEnemyAttackData(passUnit as Data_BattleMonster);
                    }
                    
                }
            }

            // foreach (var kv in unitMoveDatas)
            // {
            //     CalculateHeroHPDelta(kv.Value);
            // }



            return minFullPaths;
        }

        public void CacheUnitRangeDatas(int actionUnitID, int preGridPosIdx, int curGridPosIdx, List<TriggerData> triggerDatas)
        {
            var unitDisplacementDatas =
                new List<BattleBuffManager.UnitDisplacementData>();
            unitDisplacementDatas.Add(new BattleBuffManager.UnitDisplacementData(actionUnitID, preGridPosIdx, curGridPosIdx));
            var unitDisplacementResults = BattleBuffManager.Instance.CacheUnitRangeTrigger(RoundFightData.GamePlayData, unitDisplacementDatas);

            foreach (var unitDisplacementResult in unitDisplacementResults)
            {
                var buffTriggerType = unitDisplacementResult.BuffTriggerType;
                var unit = GameUtility.GetUnitByID(unitDisplacementResult.TriggerUnitID);

                BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, unit,
                    out List<BuffValue> triggerBuffDatas);
 
                if (triggerBuffDatas != null)
                {
                    foreach (var triggerBuffData in triggerBuffDatas)
                    {
                        BattleBuffManager.Instance.BuffTrigger(buffTriggerType,
                            triggerBuffData.BuffData, triggerBuffData.ValueList,
                            unitDisplacementResult.TriggerUnitID, unitDisplacementResult.TriggerUnitID,
                            unitDisplacementResult.BeTriggerUnitID,
                            triggerDatas);
                    }

                }

            }
        }

        public void TriggerUnitData(int triggerUnitID, int effectUnitID, int gridPosIdx, EBuffTriggerType buffTriggerType,
            List<TriggerData> triggerDatas)
        {
            var triggerSoliders = GetTriggerUnits(gridPosIdx);

            if (triggerSoliders != null)
            {
                foreach (var soliderActionRange in triggerSoliders)
                {
                    if (soliderActionRange.ActionUnitID == triggerUnitID)
                        continue;

                    if (soliderActionRange.BuffTriggerType != buffTriggerType)
                        continue;

                    var ownUnit = BattleUnitDatas[soliderActionRange.OwnUnitID];

                    BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, ownUnit,
                        out List<BuffValue> triggerBuffDatas);

                    if (triggerBuffDatas != null)
                    {
                        foreach (var triggerBuffData in triggerBuffDatas)
                        {
                            BattleBuffManager.Instance.BuffTrigger(buffTriggerType,
                                triggerBuffData.BuffData, triggerBuffData.ValueList, soliderActionRange.OwnUnitID,
                                soliderActionRange.ActionUnitID, effectUnitID,
                                triggerDatas);
                        }

                    }


                }

            }
        }

        // private void CacheUnitUnitBeMoveDatas(ActionData actionData)
        // {
        //     foreach (var kv in actionData.UnitMovePaths)
        //     {
        //         actionData.UnitMovePaths[kv.Key] = CacheUnitMoveDatas(kv.Key,
        //             kv.Value, actionData.UnitBeMoveDatas, actionData);
        //     }
        //     
        // }

        private void CacheSoliderMoveDatas()
        {
            if (RoundFightData.TempTriggerData.UnitData != null &&
                RoundFightData.TempTriggerData.UnitData.UnitCamp == PlayerManager.Instance.PlayerData.UnitCamp &&
                RoundFightData.TempTriggerData.TriggerType == ETempUnitType.MoveUnit)
            {
                //var firstUnitActionSoliders = new List<int>();
                RoundFightData.TempTriggerData.TempUnitMovePaths = CacheUnitMoveDatas(
                    RoundFightData.TempTriggerData.UnitData.ID, RoundFightData.TempTriggerData.TempUnitMovePaths,
                    RoundFightData.SoliderMoveDatas);
            }

        }
        
        

        private void CacheEnemyMoveDatas()
        {
            CacheUnitMoveDatas(RoundFightData.EnemyMovePaths, RoundFightData.EnemyMoveDatas);
        }

        private void CacheThirdUnitMoveDatas()
        {
            CacheUnitMoveDatas(RoundFightData.ThirdUnitMovePaths, RoundFightData.ThirdUnitMoveDatas);

        }

        private void CacheUnitMoveDatas(Dictionary<int, List<int>> movePaths, Dictionary<int, MoveActionData> moveDatas)
        {
            var keys = movePaths.Keys.ToList();
            //var firstUnitActionSoliders = new List<int>();

            for (int i = 0; i < movePaths.Count; i++)
            {
                movePaths[keys[i]] = CacheUnitMoveDatas(keys[i],
                    movePaths[keys[i]], moveDatas);
            }

        }

        private int GetHurt(BattleUnitEntity unit, int hpValue, ref int addDefenseCount)
        {
            var hurt = 0;
            if (hpValue > unit.MaxHP - unit.CurHP)
            {
                hurt += unit.MaxHP - unit.CurHP;
            }
            else
            {
                if (hpValue < 0)
                {
                    if (Math.Abs(hpValue) >= addDefenseCount)
                    {
                        addDefenseCount = 0;
                        hpValue += addDefenseCount;
                    }
                    else
                    {
                        addDefenseCount += hpValue;
                        hpValue = 0;
                    }
                }

                hurt += hpValue;
            }

            return hurt;
        }


        public int GetTotalDelta(int unitID, EHeroAttribute heroAttribute)
        {
            var fightUnitData = GameUtility.GetUnitByID(unitID, true);
            var unitData = GameUtility.GetUnitByID(unitID, false);

            if (fightUnitData == null)
                return 0;

            if (unitData == null)
                return 0;

            var fightHeroData = fightUnitData as Data_BattleHero;
            var heroData = unitData as Data_BattleHero;

            switch (heroAttribute)
            {
                case EHeroAttribute.CurHeart:

                    break;
                case EHeroAttribute.MaxHeart:
                    break;
                case EHeroAttribute.CurHP:

                    if (fightHeroData != null && heroData != null)
                    {
                        // var hpDelta = fightHeroData.RoundHeroHPDelta;
                        // hpDelta = Mathf.Clamp(hpDelta, -unitData.CurHP, heroData.MaxHP - heroData.CurHP);
                        //
                        // return hpDelta;

                        //return deltaHeart * fightHeroData.MaxHP + (deltaHeart == 0 ? fightHeroData.CurHP - curHeroData.CurHP : 

                        // if (fightHeroData.Attribute.GetAttribute(EHeroAttribute.CurHeart) <= 0)
                        // {
                        //     return - curHeroData.CurHP;
                        // }
                        // else 

                        var deltaHeart = fightHeroData.Attribute.GetAttribute(EHeroAttribute.CurHeart) -
                                         heroData.Attribute.GetAttribute(EHeroAttribute.CurHeart);

                        if (fightHeroData.Attribute.GetAttribute(EHeroAttribute.CurHeart) <
                            heroData.Attribute.GetAttribute(EHeroAttribute.CurHeart))
                        {
                            return -((fightHeroData.MaxHP - fightHeroData.CurHP) + heroData.CurHP);
                        }
                        else
                        {
                            return fightHeroData.CurHP - heroData.CurHP;
                        }
                    }
                    else
                    {
                        return fightUnitData.CurHP - unitData.CurHP;
                    }
                case EHeroAttribute.MaxHP:
                    break;

                case EHeroAttribute.MaxCardCountEachRound:
                    break;
                case EHeroAttribute.Coin:
                    return (int) (
                        fightHeroData.Attribute.GetAttribute(EHeroAttribute.Coin) -
                        heroData.Attribute.GetAttribute(EHeroAttribute.Coin));
                case EHeroAttribute.Refresh:
                    break;
                case EHeroAttribute.Gem:
                    break;
                case EHeroAttribute.Empty:
                    break;


                default:
                    throw new ArgumentOutOfRangeException(nameof(heroAttribute), heroAttribute, null);
            }

            return 0;
        }



        private void CurHPTriggerData(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            var effectUnitData = GetUnitByID(triggerData.EffectUnitID);
            if (effectUnitData == null)
                return;
            
            var actionUnitData = GetUnitByID(triggerData.ActionUnitID);

            var effectUnitOldHP = effectUnitData.CurHP;

            var triggerValue = triggerData.Value + triggerData.DeltaValue;
            if (triggerValue < 0)
            {
               
                if (effectUnitData.GetStateCount(EUnitState.UnHurt) > 0)
                {
                    triggerData.TriggerResult = ETriggerResult.UnHurt;

                }
                // else if (effectUnitData.GetStateCount(EUnitState.Dodge) > 0)
                // {
                //     effectUnitData.RemoveState(EUnitState.Dodge);
                //     triggerData.TriggerResult = ETriggerResult.Dodge;
                //
                // }
                else
                {
                    if (triggerData.OwnUnitID > 0 && triggerData.OwnUnitID == triggerData.ActionUnitID)
                    {
                        var ownUnit = GetUnitByID(triggerData.OwnUnitID);

                        if (ownUnit != null && triggerData.UnitState == EUnitState.Empty)
                        {
                            var hurtAddDamageCount = effectUnitData.GetAllStateCount(EUnitState.HurtAddDmg);

                            triggerData.DeltaValue = -actionUnitData.GetAllStateCount(EUnitState.AddDmg);
                            if (!GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                            {
                                triggerData.DeltaValue += actionUnitData.GetAllStateCount(EUnitState.SubDmg);
                            }

                            if (!GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                            {
                                triggerData.DeltaValue += -hurtAddDamageCount;
                            }

                            triggerData.DeltaValue += -actionUnitData.FuneCount(EBuffID.Spec_AddBaseDmg);

                            if (hurtAddDamageCount > 0)
                            {
                                var subHurtAddDamageData = BattleFightManager.Instance.Unit_State(triggerDatas, effectUnitData.ID,
                                    effectUnitData.ID, effectUnitData.ID, EUnitState.HurtAddDmg, -1,
                                    ETriggerDataType.RoleState);
                                SimulateTriggerData(subHurtAddDamageData, triggerDatas);
                                triggerDatas.Add(subHurtAddDamageData);
                            }
                        }

                        if (actionUnitData is Data_BattleSolider solider &&
                            actionUnitData.UnitCamp == BattleManager.Instance.CurUnitCamp)
                        {

                            // var energy0CardAddDamageCount =
                            //     RoundFightData.GamePlayData.BlessCount(EBlessID.Energy0CardAddDamage,
                            //         BattleManager.Instance.CurUnitCamp);
                            // if (energy0CardAddDamageCount > 0)
                            // {
                            //     var energy = BattleCardManager.Instance.GetCardEnergy(solider.CardID);
                            //     if (energy == 0)
                            //     {
                            //         var drEnergy0CardAddDamage =
                            //             GameEntry.DataTable.GetBless(EBlessID.Energy0CardAddDamage);
                            //         triggerData.DeltaValue +=
                            //             energy0CardAddDamageCount * drEnergy0CardAddDamage.Values1[0];
                            //     }
                            // }

                            var eachRoundDoubleDamageCount =
                                RoundFightData.GamePlayData.BlessCount(EBlessID.EachRoundDoubleDamage,
                                    BattleManager.Instance.CurUnitCamp);
                            var drEachRoundDoubleDamage = GameEntry.DataTable.GetBless(EBlessID.EachRoundDoubleDamage);
                            if (eachRoundDoubleDamageCount > 0 &&
                                !RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(solider.UnitCamp)
                                    .RoundIsAttack &&
                                RoundFightData.GamePlayData.BattleData.Round > 0 &&
                                RoundFightData.GamePlayData.BattleData.Round % BattleBuffManager.Instance.GetBuffValue(drEachRoundDoubleDamage.Values1[0]) == 0)
                            {

                                triggerData.DeltaValue +=
                                    eachRoundDoubleDamageCount * BattleBuffManager.Instance.GetBuffValue(drEachRoundDoubleDamage.Values1[1]);
                            }

                            RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(solider.UnitCamp).RoundIsAttack =
                                true;


                        }


                        if (triggerData.Value + triggerData.DeltaValue > 0)
                        {
                            triggerData.DeltaValue = -triggerData.Value;
                        }
                    }


                    var hpDelta = (int) triggerValue;
                    
                    BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, effectUnitData,
                        out List<BuffValue> effectTriggerBuffDatas);
                    if (effectTriggerBuffDatas != null)
                    {
                        foreach (var triggerBuffData in effectTriggerBuffDatas)
                        {
                            // if (triggerBuffData.DrBuff.BuffID == EBuffID.Round_UnHurt &&
                            //     (RoundFightData.GamePlayData.BattleData.Round + 1) % triggerBuffData.ValueList[0] ==
                            //     0 &&
                            //     RoundFightData.GamePlayData.BattleData.Round > 0)
                            // {
                            //     triggerData.DeltaValue = -triggerData.Value;
                            // }
                            // else if (triggerBuffData.DrBuff.BuffID == EBuffID.UnHurtDamage &&
                            //          hpDelta == triggerBuffData.ValueList[0])
                            // {
                            //     triggerData.DeltaValue = -triggerData.Value;
                            // }

                        }
                    }

                    BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, actionUnitData,
                        out List<BuffValue> actionTriggerBuffDatas);

                    if (actionTriggerBuffDatas != null)
                    {
                        foreach (var triggerBuffData in actionTriggerBuffDatas)
                        {
                            // if (triggerBuffData.DrBuff.BuffID == EBuffID.Round_DoubleDamage &&
                            //     (RoundFightData.GamePlayData.BattleData.Round + 1) % triggerBuffData.ValueList[0] ==
                            //     triggerBuffData.ValueList[1] && hpDelta < 0)
                            // {
                            //     triggerData.DeltaValue = hpDelta + triggerData.DeltaValue;
                            // }
                        }
                    }
                    
                    hpDelta = (int) (triggerData.Value + triggerData.DeltaValue);

                    if (effectUnitData.GetAllStateCount(EUnitState.CounterAtk) > 0)
                    {
                        var counterValue = hpDelta;
                        var battlePlayerData =
                            BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(
                                effectUnitData.UnitCamp);
                        // if (battlePlayerData.RoundBuffs.Contains(EBuffID.RoundCounterAttackAddDamage))
                        // {
                        //     counterValue += -1;
                        // }

                        var counterAttackTriggerData = BattleFightManager.Instance.BattleRoleAttribute(effectUnitData.ID,
                            effectUnitData.ID, triggerData.ActionUnitID, EUnitAttribute.HP, counterValue,
                            ETriggerDataSubType.Unit);
                        SimulateTriggerData(counterAttackTriggerData, triggerDatas);
                        triggerDatas.Add(counterAttackTriggerData);

                        var subCounterAttackTriggerData = BattleFightManager.Instance.Unit_State(triggerDatas, effectUnitData.ID,
                            effectUnitData.ID, effectUnitData.ID, EUnitState.CounterAtk, -1,
                            ETriggerDataType.RoleState);
                        SimulateTriggerData(subCounterAttackTriggerData, triggerDatas);
                        triggerDatas.Add(subCounterAttackTriggerData);
                    }


                    if (actionUnitData != null && actionUnitData.GetStateCount(EUnitState.RecoverHP) > 0)
                    {
                        var recoverHPValue = -hpDelta;
                        var recoverHPTriggerData = BattleFightManager.Instance.BattleRoleAttribute(actionUnitData.ID,
                            actionUnitData.ID, actionUnitData.ID, EUnitAttribute.HP, recoverHPValue,
                            ETriggerDataSubType.Unit);
                        SimulateTriggerData(recoverHPTriggerData, triggerDatas);
                        triggerDatas.Add(recoverHPTriggerData);
                    }

                    // if (effectUnitData is Data_BattleSolider)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
                    // {
                    //     var oldSoliderHP = effectUnitData.CurHP;
                    //     FightManager.Instance.ChangeHP(effectUnitData, hpDelta);
                    //     var newSoliderHP = effectUnitData.CurHP;
                    //     var deltaSoliderHP = newSoliderHP - oldSoliderHP;
                    //     if (deltaSoliderHP < 0)
                    //     {
                    //         FightManager.Instance.ChangeHP(RoundFightData.GamePlayData.BattleHero, -deltaSoliderHP);
                    //     }
                    // }
                    // else
                    // {
                    //     FightManager.Instance.ChangeHP(effectUnitData, hpDelta);
                    // }

                    if ( actionUnitData!= null  && actionUnitData.GetStateCount(EUnitState.DoubleDmg) > 0 && !GameUtility.ContainRoundState(
                        GamePlayManager.Instance.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    {
                        triggerValue *= 2;
                    }

                    BattleFightManager.Instance.ChangeHP(effectUnitData, triggerValue, EHPChangeType.Unit, true,
                        triggerData.ChangeHPInstantly);
                    
                    
                    
                }

                // if (actionUnitData.GetStateCount(EUnitState.HurtSubDamage) > 0)
                // {
                //     var hurtSubDamageTriggerData = FightManager.Instance.Unit_State(actionUnitData.ID,
                //         actionUnitData.ID, actionUnitData.ID, EUnitState.HurtSubDamage, 1, ETriggerDataType.RoleState);
                //     SimulateTriggerData(hurtSubDamageTriggerData, triggerDatas);
                //     triggerDatas.Add(hurtSubDamageTriggerData);
                //
                // }

                if (effectUnitData.GetStateCount(EUnitState.AddDmg) > 0)
                {
                    var addDamageTriggerData = BattleFightManager.Instance.Unit_State(triggerDatas, effectUnitData.ID,
                        effectUnitData.ID, effectUnitData.ID, EUnitState.AddDmg, 1, ETriggerDataType.RoleState);
                    SimulateTriggerData(addDamageTriggerData, triggerDatas);
                    triggerDatas.Add(addDamageTriggerData);
                }

                // if (actionUnitData.GetStateCount(EUnitState.HurtAddDamage) > 0)
                // {
                //     var hurtAddDamageTriggerData = FightManager.Instance.Unit_State(actionUnitData.ID,
                //         actionUnitData.ID, actionUnitData.ID, EUnitState.HurtAddDamage, -1, ETriggerDataType.RoleState);
                //     SimulateTriggerData(hurtAddDamageTriggerData, triggerDatas);
                //     triggerDatas.Add(hurtAddDamageTriggerData);
                // }



                if (effectUnitData.GetStateCount(EUnitState.SubDmg) > 0)
                {
                    var subDamageTriggerData = BattleFightManager.Instance.Unit_State(triggerDatas, effectUnitData.ID,
                        effectUnitData.ID, effectUnitData.ID, EUnitState.SubDmg, -1, ETriggerDataType.RoleState);
                    SimulateTriggerData(subDamageTriggerData, triggerDatas);
                    triggerDatas.Add(subDamageTriggerData);
                }

                // if (actionUnitData.GetStateCount(EUnitState.SubDamage) > 0)
                // {
                //     var subDamageTriggerData = FightManager.Instance.Unit_State(actionUnitData.ID,
                //         actionUnitData.ID, actionUnitData.ID, EUnitState.SubDamage, -1, ETriggerDataType.RoleState);
                //     SimulateTriggerData(subDamageTriggerData, triggerDatas);
                //     triggerDatas.Add(subDamageTriggerData);
                // }
            }
            else
            {
                if (BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.HP1UnRecover) &&
                      effectUnitData.CurHP == 1 && effectUnitData.UnitCamp == EUnitCamp.Player1)
                {
                    triggerData.Value = 0;
                    triggerData.DeltaValue = 0;
                }
                
                if ( actionUnitData!= null  && actionUnitData.GetStateCount(EUnitState.DoubleDmg) > 0 && !GameUtility.ContainRoundState(
                    GamePlayManager.Instance.GamePlayData, EBuffID.Spec_CurseUnEffect))
                {
                    triggerValue *= 2;
                }
                
                BattleFightManager.Instance.ChangeHP(effectUnitData, triggerValue,
                    EHPChangeType.Unit, true, triggerData.ChangeHPInstantly);
                
            }


            if (effectUnitData.CurHP <= 0)
            {
                BattleCurseManager.Instance.CacheUnitDeadRecoverLessHPUnit(effectUnitOldHP, effectUnitData.CurHP,
                    triggerDatas);
                DeadTrigger(triggerData, triggerDatas);
                KillTrigger(triggerData, triggerDatas);
                CacheLinks();

            }

        }

        public void SimulateTriggerData(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            var effectUnit = GetUnitByID(triggerData.EffectUnitID);

            var triggerValue = triggerData.Value + triggerData.DeltaValue;
            
            switch (triggerData.TriggerDataType)
            {
                case ETriggerDataType.Hero:
                    var dataBattleHero = effectUnit as Data_BattleHero;
                    switch (triggerData.HeroAttribute)
                    {
                        // case EHeroAttribute.CurHP:
                        //     CurHPTriggerData(triggerData, effectUnit, triggerDatas);
                        //     break;

                        // case EHeroAttribute.CurEnergy:
                        //     if (dataBattleHero != null)
                        //     {
                        //         dataBattleHero.Attribute.SetAttribute(EHeroAttribute.CurEnergy,
                        //             dataBattleHero.Attribute.GetAttribute(EHeroAttribute.CurEnergy) +
                        //             triggerValue);
                        //     }
                        //
                        //     break;
                        case EHeroAttribute.CurHeart:
                            break;
                        case EHeroAttribute.MaxHeart:
                            break;
                        case EHeroAttribute.MaxHP:
                            break;
                        case EHeroAttribute.Coin:
                            if (dataBattleHero != null)
                            {
                                dataBattleHero.Attribute.SetAttribute(EHeroAttribute.Coin,
                                    dataBattleHero.Attribute.GetAttribute(EHeroAttribute.Coin) + triggerValue);
                            }

                            break;
                        case EHeroAttribute.Damage:
                            if (dataBattleHero != null)
                            {
                                dataBattleHero.Attribute.SetAttribute(EHeroAttribute.Damage,
                                    dataBattleHero.Attribute.GetAttribute(EHeroAttribute.Damage) + triggerValue);
                            }

                            break;
                        default:
                            break;
                    }

                    break;
                case ETriggerDataType.RoleAttribute:
                    switch (triggerData.BattleUnitAttribute)
                    {
                        case EUnitAttribute.HP:
                            CurHPTriggerData(triggerData, triggerDatas);
                            break;
                        case EUnitAttribute.MaxHP:
                            effectUnit.BaseMaxHP += (int) triggerValue;
                            CurHPTriggerData(triggerData, triggerDatas);
                            break;
                        case EUnitAttribute.BaseDamage:
                            effectUnit.BaseDamage += (int) triggerValue;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ETriggerDataType.RoleState:
                    effectUnit.ChangeState(triggerData.UnitState, (int) triggerValue);
                    break;
                case ETriggerDataType.RoundRoleState:
                    effectUnit.ChangeRoundState(triggerData.UnitState, (int) triggerValue);
                    break;
                case ETriggerDataType.Card:
                    break;
                case ETriggerDataType.RemoveUnit:
                    effectUnit.RemoveAllState();
                    CurHPTriggerData(triggerData, triggerDatas);
                    break;
                case ETriggerDataType.RoundBuff:
                    var battlePlayerData =
                        RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(effectUnit.UnitCamp);
                    battlePlayerData.RoundBuffs.Add(triggerData.BuffID);
                    break;
                default:
                    break;
            }

        }

        public List<UnitActionRange> GetTriggerUnits(int gridPosIdx)
        {
            if (!UnitActionRange.ContainsKey(gridPosIdx))
                return null;

            // if (!UnitActionRange[gridPosIdx].ContainsKey(triggerType))
            //     return null;

            return UnitActionRange[gridPosIdx];

        }

        



        private void CacheUnitActionRange(Data_BattleUnit ownUnitData, Data_BattleUnit actionUnitData,
            EUnitActionType actionType,
            List<BuffData> buffDatas)
        {
            if (buffDatas == null)
                return;

            foreach (var buffData in buffDatas)
            {
                var triggerRange = buffData.TriggerRange;
                if (buffData.BuffTriggerType == EBuffTriggerType.Pass)
                    continue;

                if (!buffData.RangeTrigger)
                    continue;

                List<ERelativeCamp> unitCamps = triggerRange == EActionType.UnitMaxDirect
                    ? buffData.TriggerUnitCamps
                    : null;
                //var attackTypes = triggerRange == EActionType.MaxDirect8 ? drBuff.TriggerUnitAttackTypes : null;
                var range = GameUtility.GetRange(actionUnitData.GridPosIdx, triggerRange, actionUnitData.UnitCamp,
                    unitCamps,true);

                foreach (var gridPosIdx in range)
                {
                    var soliderActionRange = new UnitActionRange()
                    {
                        OwnUnitID = ownUnitData.ID,
                        ActionUnitID = actionUnitData.ID,
                        UnitActionType = actionType,
                        BuffTriggerType = buffData.BuffTriggerType,
                    };

                    if (UnitActionRange.ContainsKey(gridPosIdx))
                    {
                        UnitActionRange[gridPosIdx].Add(soliderActionRange);
                    }
                    else
                    {
                        var soliders = new List<UnitActionRange>();
                        UnitActionRange.Add(gridPosIdx, soliders);
                        soliders.Add(soliderActionRange);
                    }
                }
            }
        }



        private void CacheLinks()
        {
            foreach (var kv in BattleUnitDatas)
            {
                kv.Value.LinkIDs.Clear();
            }
            foreach (var kv in BattleUnitDatas)
            {
                kv.Value.Links.Clear();
            }
            UnitActionRange.Clear();
            
            if(BattleCurseManager.Instance.IsLinkUnEffect())
                return;
            
            CacheUnitLinkIDs();
            CacheUnitLinks();
            CacheUnitActionRange();

        }

        private void CacheUnitLinkIDs()
        {
            var player1UnLinkPosIdx =
                BattleBuffManager.Instance.GetUnLinkPosIdxs(RoundFightData.GamePlayData, EUnitCamp.Player1);
            var player2UnLinkPosIdx =
                BattleBuffManager.Instance.GetUnLinkPosIdxs(RoundFightData.GamePlayData, EUnitCamp.Player2);
            var enemyUnLinkPosIdx =
                BattleBuffManager.Instance.GetUnLinkPosIdxs(RoundFightData.GamePlayData, EUnitCamp.Enemy);

            
            var propLinkDict = new GameFrameworkMultiDictionary<int, int>();

            foreach (var kv in BattleGridPropManager.Instance.GridPropDatas)
            {
                var drGridProp = GameEntry.DataTable.GetGridProp(kv.Value.GridPropID);
                foreach (var buffID in drGridProp.GridPropIDs)
                {
                    var buffStr = Enum.GetName(typeof(EBuffID), buffID);

                    var isLink = Enum.TryParse(buffStr, out ELinkID linkID);

                    if (isLink)
                    {
                        var drLink = GameEntry.DataTable.GetLink(linkID);
                        var range = GameUtility.GetRange(kv.Value.GridPosIdx, drLink.LinkRange);
                        foreach (var gridPosIdx in range)
                        { 
                            propLinkDict.Add(gridPosIdx, drGridProp.Id);
                        }

                    }
                }
            }

            foreach (var kv in BattleUnitDatas)
            {
                if (kv.Value.UnitCamp == EUnitCamp.Player1 && player1UnLinkPosIdx.Contains(kv.Value.GridPosIdx))
                    continue;
                if (kv.Value.UnitCamp == EUnitCamp.Player2 && player2UnLinkPosIdx.Contains(kv.Value.GridPosIdx))
                    continue;

                if (kv.Value.UnitCamp == EUnitCamp.Enemy && enemyUnLinkPosIdx.Contains(kv.Value.GridPosIdx))
                    continue;


                foreach (var funeID in kv.Value.FuneIdxs)
                {
                    var buffData = BattleBuffManager.Instance.GetBuffData(funeID);
                    if (buffData == null)
                        continue;
                    // if (buffData.BuffTriggerType == EBuffTriggerType.Link)
                    // {
                    //     var linkID = GameUtility.FuneIDToLinkID(FuneManager.Instance.GetFuneID(funeID));
                    //     kv.Value.LinkIDs.Add(linkID);
                    // }
                }

                if (propLinkDict.Contains(kv.Value.GridPosIdx))
                {
                    foreach (var gridPropID in propLinkDict[kv.Value.GridPosIdx])
                    {
                        var drGridProp = GameEntry.DataTable.GetGridProp(gridPropID);
                        foreach (var buffIDStr in drGridProp.GridPropIDs)
                        {
                            var buffStr = Enum.GetName(typeof(EBuffID), buffIDStr);
                            var linkID = Enum.Parse<ELinkID>(buffStr);
                            if (linkID != null)
                            {
                                kv.Value.LinkIDs.Add(linkID);
                            }
                        }
                    }

                }

                foreach (var linkID in kv.Value.BattleLinkIDs)
                {
                    kv.Value.LinkIDs.Add(linkID);
                }
                
                var buffDatas = BattleUnitManager.Instance.GetBuffDatas(kv.Value);
                foreach (var buffData in buffDatas)
                {
                    // if (buffData.BuffTriggerType == EBuffTriggerType.Link)
                    // {
                    //     var buffStr = Enum.GetName(typeof(EBuffID), buffData.BuffID);
                    //     var linkID = Enum.Parse<ELinkID>(buffStr);
                    //     kv.Value.LinkIDs.Add(linkID);
                    // }
                }

                // if (kv.Value is Data_BattleSolider solider)
                // {
                //     var drBuffs = CardManager.Instance.GetBuffTable(solider.CardID);
                //     foreach (var drBuff in drBuffs)
                //     {
                //         if (drBuff.BuffTriggerType == EBuffTriggerType.Link)
                //         {
                //             var buffStr = Enum.GetName(typeof(EBuffID), drBuff.BuffID);
                //             var linkID = Enum.Parse<ELinkID>(buffStr);
                //             kv.Value.LinkIDs.Add(linkID);
                //         }
                //     }
                // }
                // else if (kv.Value is Data_BattleMonster monster)
                // {
                //     var drBuffs = BattleEnemyManager.Instance.GetBuffTable(monster.MonsterID);
                //     foreach (var drBuff in drBuffs)
                //     {
                //         if (drBuff.BuffTriggerType == EBuffTriggerType.Link)
                //         {
                //             var buffStr = Enum.GetName(typeof(EBuffID), drBuff.BuffID);
                //             var linkID = Enum.Parse<ELinkID>(buffStr);
                //             kv.Value.LinkIDs.Add(linkID);
                //         }
                //     }
                // }


            }
        }

        private void CacheUnitLinks()
        {
            foreach (var kv in BattleUnitDatas)
            {
                foreach (var linkID in kv.Value.LinkIDs)
                {
                    var drLink = GameEntry.DataTable.GetLink(linkID);

                    var linkageRange = GameUtility.GetRange(kv.Value.GridPosIdx, drLink.LinkRange, kv.Value.UnitCamp,
                        drLink.LinkUnitCamps, true);

                    foreach (var gridPosIdx in linkageRange)
                    {
                        if (gridPosIdx == kv.Value.GridPosIdx)
                            continue;

                        var battleUnitData =
                            GetUnitByGridPosIdxMoreCamps(gridPosIdx, kv.Value.UnitCamp, drLink.LinkUnitCamps);
                        if (battleUnitData == null)
                            continue;

                        var linkUnit =
                            GetUnitByGridPosIdxMoreCamps(gridPosIdx, kv.Value.UnitCamp, drLink.LinkUnitCamps);
                        if (linkUnit == null)
                            continue;

                        if (drLink.LinkType == ELinkType.Send)
                        {
                            linkUnit.Links.Add(kv.Value.ID);
                        }
                        else if (drLink.LinkType == ELinkType.Receive)
                        {
                            kv.Value.Links.Add(linkUnit.ID);
                        }
                    }
                }

            }
        }
        
        private void CacheUnitActionRange()
        {
            foreach (var kv in BattleUnitDatas)
            {
                var buffDatas =  BattleUnitManager.Instance.GetBuffDatas(kv.Value);
                CacheUnitActionRange(kv.Value, kv.Value, EUnitActionType.Own, buffDatas);

                // foreach (var unitID in kv.Value.Links)
                // {
                //     var linkDrBuffs = BattleUnitManager.Instance.GetBuffDatas(BattleUnitDatas[unitID]);
                //     CacheUnitActionRange(BattleUnitDatas[unitID], kv.Value, EUnitActionType.Linkage, linkDrBuffs);
                // }
            }
        }

        public void RefreshUnitGridPosIdx()
        {
            BattleUnitDatasByGridPosIdx.Clear();
            
            foreach (var kv in BattleUnitDatas)
            {
                if (!BattleUnitDatasByGridPosIdx.ContainsKey(kv.Value.GridPosIdx))
                {
                    BattleUnitDatasByGridPosIdx.Add(kv.Value.GridPosIdx, kv.Value);
                }

            }
        }

        // public Data_BattleUnit GetLastRoundUnitByGridPosIdx(int gridPosIdx, EUnitCamp? selfUnitCamp = null,
        //     ERelativeCamp? unitCamp = null, EUnitRole? unitRole = null)
        // {
        //     return InternalGetUnitByGridPosIdx(RoundFightData.GamePlayData.LastBattleData.BattleUnitDatas, gridPosIdx,
        //         selfUnitCamp, unitCamp, unitRole);
        //
        // }

        public Data_BattleUnit GetUnitByGridPosIdx(int gridPosIdx, EUnitCamp? selfUnitCamp = null,
            ERelativeCamp? unitCamp = null, EUnitRole? unitRole = null, int exceptUnitID = -1)
        {

            return InternalGetUnitByGridPosIdx(BattleUnitDatas, gridPosIdx, selfUnitCamp, unitCamp, unitRole,
                exceptUnitID);
        }

        private Data_BattleUnit InternalGetUnitByGridPosIdx(Dictionary<int, Data_BattleUnit> battleUnitDatas,
            int gridPosIdx, EUnitCamp? selfUnitCamp = null, ERelativeCamp? unitCamp = null, EUnitRole? unitRole = null,
            int exceptUnitID = -1)
        {
            if (BattleUnitDatasByGridPosIdx.ContainsKey(gridPosIdx))
            {
                var unit = BattleUnitDatasByGridPosIdx[gridPosIdx];
                if (unitCamp == ERelativeCamp.Us && selfUnitCamp != unit.UnitCamp)
                    return null;

                if (unitCamp == ERelativeCamp.Enemy && selfUnitCamp == unit.UnitCamp)
                    return null;

                if (unitRole != null && unit.UnitRole != unitRole)
                {
                    return null;
                }

                if (unit.ID == exceptUnitID)
                {
                    return null;
                }

                return unit;
            }
            else
            {
                return null;
            }
            
            // foreach (var kv in battleUnitDatas)
            // {
            //
            //     if (unitCamp == ERelativeCamp.Us && selfUnitCamp != kv.Value.UnitCamp)
            //         continue;
            //
            //     if (unitCamp == ERelativeCamp.Enemy && selfUnitCamp == kv.Value.UnitCamp)
            //         continue;
            //
            //     if (unitRole != null && kv.Value.UnitRole != unitRole)
            //     {
            //         continue;
            //     }
            //
            //     if (kv.Value.ID == exceptUnitID)
            //     {
            //         continue;
            //     }
            //
            //
            //     if (kv.Value.GridPosIdx == gridPosIdx)
            //     {
            //         return kv.Value;
            //     }
            // }
            //
            //
            //
            // return null;
        }

        public Data_BattleUnit GetUnitByID(int id)
        {
            if (BattleUnitDatas.ContainsKey(id))
            {
                return BattleUnitDatas[id];
            }

            return null;

            // foreach (var kv in BattleUnitDatas)
            // {
            //     // if (unitCamp != null && kv.Value.UnitCamp != unitCamp)
            //     //     continue;
            //
            //     // if (attackType != null && kv.Value is Data_BattleSolider solider)
            //     // {
            //     //     var drBuff = CardManager.Instance.GetBuffTable(solider.CardID);
            //     //     var soliderAttackType = drBuff.TriggerRange;
            //     //     if (soliderAttackType != attackType)
            //     //     {
            //     //         continue;
            //     //     }
            //     // }
            //     // else if (attackType != null && kv.Value is Data_BattleMonster enemy)
            //     // {
            //     //     var drEnemy = GameEntry.DataTable.GetEnemy(enemy.EnemyTypeID);
            //     //     var drBuff = GameEntry.DataTable.GetBuff(drEnemy.OwnBuffs[0]);
            //     //     var enemyAttackType = drBuff.TriggerRange;
            //     //     if (enemyAttackType != attackType)
            //     //     {
            //     //         continue;
            //     //     }
            //     // }
            //
            //     if (kv.Value.ID == id)
            //     {
            //         return kv.Value;
            //     }
            // }
            //
            //
            // return null;
        }

        public Data_BattleUnit GetUnitByGridPosIdxMoreCamps(int gridPosIdx, EUnitCamp? selfUnitCamp = null,
            List<ERelativeCamp> unitCamps = null)
        {
            if (unitCamps == null)
                return null;
            
            foreach (var unitCamp in unitCamps)
            {
                var unit = GetUnitByGridPosIdx(gridPosIdx, selfUnitCamp, unitCamp);
                if (unit != null)
                {
                    return unit;
                }
            }

            return null;
        }

        public int GetUnitIDByGridPosIdx(int gridPosIdx, EUnitCamp? selfUnitCamp = null, ERelativeCamp? unitCamp = null,
            EUnitRole? unitRole = null)
        {
            var unit = GetUnitByGridPosIdx(gridPosIdx, selfUnitCamp, unitCamp, unitRole);
            if (unit != null)
                return unit.ID;

            return -1;
        }



        // public TriggerData Unit_UnitAttribute(int triggerSoliderID, int actionSoliderID, int effectUnitID,
        //     EUnitAttribute attribute, float attributeValue)
        // {
        //
        //     return BattleRoleAttribute(triggerSoliderID, actionSoliderID, effectUnitID, attribute, attributeValue);
        // }



        public TriggerData BattleRoleAttribute(int ownSoliderID, int actionSoliderID, int effectUnitID,
            EUnitAttribute attribute, float attributeValue, ETriggerDataSubType triggerDataSubType)
        {
            var triggerData = new TriggerData();
            triggerData.TriggerDataType = ETriggerDataType.RoleAttribute;
            triggerData.TriggerDataSubType = triggerDataSubType;
            triggerData.OwnUnitID = ownSoliderID;
            triggerData.ActionUnitID = actionSoliderID;
            triggerData.EffectUnitID = effectUnitID;
            triggerData.BattleUnitAttribute = attribute;
            triggerData.Value = attributeValue;

            return triggerData;
        }



        public TriggerData Unit_HeroAttribute(int triggerSoliderID, int actionSoliderID, int effectUnitID,
            EHeroAttribute attribute, float attributeValue)
        {
            var cardTriggerData = new TriggerData();
            cardTriggerData.TriggerDataType = ETriggerDataType.Hero;

            cardTriggerData.OwnUnitID = triggerSoliderID;
            cardTriggerData.ActionUnitID = actionSoliderID;
            cardTriggerData.EffectUnitID = effectUnitID;
            cardTriggerData.HeroAttribute = attribute;
            cardTriggerData.Value = attributeValue;

            return cardTriggerData;
        }

        public TriggerData Unit_State(List<TriggerData> triggerDatas, int triggerSoliderID, int actionSoliderID, int effectUnitID,
            EUnitState unitState, float attributeValue, ETriggerDataType triggerDataType)
        {
            var effectUnit = GameUtility.GetUnitByID(effectUnitID);
            if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.Negative].Contains(unitState))
            {
                if (BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.AddDebuffRecoverHP))
                {
                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(effectUnit.ID, effectUnit.ID,
                        effectUnit.ID, EUnitAttribute.HP, 1, ETriggerDataSubType.Curse);
                    triggerDatas.Add(triggerData);
                }
            }
            
            return Unit_State(triggerSoliderID, actionSoliderID, effectUnitID,
                unitState, attributeValue, triggerDataType);
        }

        private TriggerData Unit_State(int triggerSoliderID, int actionSoliderID, int effectUnitID,
            EUnitState unitState, float value, ETriggerDataType triggerDataType)
        {
            var cardTriggerData = new TriggerData();
            var unit = GetUnitByID(effectUnitID);
            if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.Negative].Contains(unitState))
            {
                if (unit.GetStateCount(EUnitState.DeBuffUnEffect) > 0)
                {
                    cardTriggerData.TriggerDataType = ETriggerDataType.RoleState;
                    cardTriggerData.OwnUnitID = effectUnitID;
                    cardTriggerData.ActionUnitID = effectUnitID;
                    cardTriggerData.EffectUnitID = effectUnitID;
                    cardTriggerData.UnitState = EUnitState.DeBuffUnEffect;
                    cardTriggerData.Value = -1;
                    return cardTriggerData;
                }
                else if (unit.GetRoundStateCount(EUnitState.DeBuffUnEffect) > 0)
                {
                    cardTriggerData.TriggerDataType = ETriggerDataType.Empty;
                    return cardTriggerData;
                }
                
            }
            else if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.Positive].Contains(unitState))
            {
                if (unit.GetAllStateCount(EUnitState.BuffAddMore) > 0)
                {
                    value += 1;
                }
                
            }
            
            cardTriggerData.TriggerDataType = triggerDataType;
            cardTriggerData.OwnUnitID = triggerSoliderID;
            cardTriggerData.ActionUnitID = actionSoliderID;
            cardTriggerData.EffectUnitID = effectUnitID;
            cardTriggerData.UnitState = unitState;
            cardTriggerData.Value = value;
        
            return cardTriggerData;
        }
        
        public TriggerData  Hero_Card(int triggerSoliderID, int actionSoliderID, int effectUnitID, float value,
            ECardTriggerType cardTriggerType)
        {

            var cardTriggerData = new TriggerData();
            cardTriggerData.TriggerDataType = ETriggerDataType.Card;
            cardTriggerData.CardTriggerType = cardTriggerType;
            cardTriggerData.OwnUnitID = triggerSoliderID;
            cardTriggerData.ActionUnitID = actionSoliderID;
            cardTriggerData.EffectUnitID = effectUnitID;
        
            cardTriggerData.Value = value;
        
            return cardTriggerData;
        
        }
        
        public async Task TriggerAction(TriggerData triggerData)
        {
            // var ownUnitEntity = BattleUnitManager.Instance.GetUnitByID(triggerData.OwnUnitID);
            // if (ownUnitEntity == null)
            //     return;
            //
            
            var effectUnitEntity = BattleUnitManager.Instance.GetUnitByID(triggerData.EffectUnitID);
            if (effectUnitEntity == null)
                return;
            
            var actionUnitEntity = BattleUnitManager.Instance.GetUnitByID(triggerData.ActionUnitID);
            
            if (triggerData.TriggerResult == ETriggerResult.UnHurt)
            {
                effectUnitEntity.Dodge();
                return;
            }
            
            // if (triggerData.TriggerResult == ETriggerResult.Dodge)
            // {
            //     effectUnitEntity.BattleUnit.RemoveState(EUnitState.Dodge);
            //     effectUnitEntity.Dodge();
            //     return;
            // }
            
            var triggerValue = (int) (triggerData.Value + triggerData.DeltaValue);

            if (triggerData.BattleUnitAttribute == EUnitAttribute.HP && triggerValue < 0 &&
                !triggerData.ChangeHPInstantly)
            {
                return;
            }
                
            
            switch (triggerData.TriggerDataType)
            {
                case ETriggerDataType.Hero:
                    var battleHeroEntity = effectUnitEntity as BattleHeroEntity;
                    switch (triggerData.HeroAttribute)
                    {
                        // case EHeroAttribute.CurHP:
                        //     var hurt = (int) (triggerData.Value + triggerData.DeltaValue);
                        //     if (hurt < 0)
                        //     {
                        //         effectUnitEntity.LastCurHPDelta();
                        //     } 
                        //     else if (hurt > 0)
                        //     {
                        //         effectUnitEntity.Recover();
                        //     }
                        //     effectUnitEntity.ChangeCurHP(hurt);
                        //     break;

                        // case EHeroAttribute.CurEnergy:
                        //     battleHeroEntity.BattleHeroEntityData.BattleHeroData.Attribute.SetAttribute(
                        //         EHeroAttribute.CurEnergy,
                        //         +battleHeroEntity.BattleHeroEntityData.BattleHeroData.Attribute.GetAttribute(
                        //             EHeroAttribute.CurEnergy) + triggerData.Value);
                        //     break;
                        case EHeroAttribute.CurHeart:
                            break;
                        case EHeroAttribute.MaxHeart:
                            break;

                        case EHeroAttribute.MaxHP:
                            break;
                        
                        case EHeroAttribute.Coin:
                            battleHeroEntity.BattleHeroEntityData.BattleHeroData.Attribute.SetAttribute(
                                EHeroAttribute.Coin,
                                +battleHeroEntity.BattleHeroEntityData.BattleHeroData.Attribute.GetAttribute(
                                    EHeroAttribute.Coin) + triggerData.Value);
                            break;
                        case EHeroAttribute.Damage:
                            battleHeroEntity.BattleHeroEntityData.BattleHeroData.Attribute.SetAttribute(
                                EHeroAttribute.Damage,
                                +battleHeroEntity.BattleHeroEntityData.BattleHeroData.Attribute.GetAttribute(
                                    EHeroAttribute.Damage) + triggerData.Value);
                            break;
                        
                        
                        
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ETriggerDataType.RoleAttribute:
                    switch (triggerData.BattleUnitAttribute)
                    {
                        case EUnitAttribute.HP:
                            
                            effectUnitEntity.ChangeCurHP(triggerValue, true, true, triggerData.ChangeHPInstantly);
                            if (triggerValue < 0)
                            {
                                
                                effectUnitEntity.BattleUnit.HurtTimes += 1;
                                
                                effectUnitEntity.Hurt();
                            } 
                            else if (triggerValue > 0)
                            {
                                effectUnitEntity.Recover();
                            }

                            if (triggerData.UnitState == EUnitState.HurtEachMove || triggerData.UnitState == EUnitState.HurtRoundStart)
                            {
                                effectUnitEntity.BattleUnit.RemoveState(triggerData.UnitState);
                            }
                            
                            if (actionUnitEntity != null)
                            {
                                if (triggerData.UnitState == EUnitState.AtkPassUs ||
                                    triggerData.UnitState == EUnitState.AtkPassEnemy &&
                                    actionUnitEntity.BattleUnit.GetStateCount(triggerData.UnitState) > 0)
                                {
                                    actionUnitEntity.BattleUnit.RemoveState(triggerData.UnitState);
                                }
                            }
                            
                            break;
                        case EUnitAttribute.MaxHP:
                            var recover = (int) (triggerData.Value + triggerData.DeltaValue);
                            effectUnitEntity.BattleUnit.BaseMaxHP += recover;
                            effectUnitEntity.ChangeCurHP(recover, true, true, true);
                            break;
                        case EUnitAttribute.Empty:
                            break;
                        case EUnitAttribute.BaseDamage:
                            var damage = (int) (triggerData.Value + triggerData.DeltaValue);
                            effectUnitEntity.BattleUnit.BaseDamage += damage;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ETriggerDataType.RoleState:
        
                    if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.Negative].Contains(triggerData.UnitState))
                    {
                        effectUnitEntity.Hurt();    
                    }
                    if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.Positive].Contains(triggerData.UnitState))
                    {
                        effectUnitEntity.Recover();    
                    }
                    
                    effectUnitEntity.BattleUnit.ChangeState(triggerData.UnitState, triggerValue);
                    break;
                case ETriggerDataType.RoundRoleState:
                    // if (actionUnitEntity != null && actionUnitEntity.ID != effectUnitEntity.ID)
                    // {
                    //     actionUnitEntity.Attack();
                    // }
                    effectUnitEntity.Hurt();
                    effectUnitEntity.BattleUnit.ChangeRoundState(triggerData.UnitState, (int)(triggerData.Value + triggerData.DeltaValue));
                    break;
                case ETriggerDataType.Card:
                    switch (triggerData.CardTriggerType)
                    {
     
                        case ECardTriggerType.AcquireCard:
                            BattleCardManager.Instance.AcquireCards((int)(triggerData.Value + triggerData.DeltaValue));
                            break;
                        case ECardTriggerType.ToHand:
                            var actionUnit = GetUnitByID(triggerData.ActionUnitID);
                            if (actionUnit is Data_BattleSolider solider)
                            {
                                BattleCardManager.Instance.ToHandCards(solider.CardID);
                            }
                            
                            break;
                        
                        case ECardTriggerType.ConsumeCard:
                            
                            BattleCardManager.Instance.ConsumeCardForms();
                            break;
                        
                        case ECardTriggerType.AddSpecificCard:
                            var newCardID = BattleCardManager.Instance.AddTempNewCard((int)triggerData.Value);
                            BattleCardManager.Instance.NewCardToHand(newCardID);
                            break;
                        case ECardTriggerType.ConsumeSpecificCard:
                            BattleCardManager.Instance.ConsumeCard((int)triggerData.Value);
                            break;
                        case ECardTriggerType.StandByToPass:
                            BattleCardManager.Instance.RandomStandByToPass();
                            break;
                        case ECardTriggerType.Empty:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ETriggerDataType.Link:
                    effectUnitEntity.BattleUnit.BattleLinkIDs.Add(triggerData.LinkID);
                    break;
                case ETriggerDataType.RemoveUnit:
                    effectUnitEntity.BattleUnit.RemoveAllState();
                    effectUnitEntity.ChangeCurHP(triggerValue, true, true, triggerData.ChangeHPInstantly);
                    if (triggerValue < 0)
                    {
                        effectUnitEntity.BattleUnit.HurtTimes += 1;
                        effectUnitEntity.Hurt();
                    }
                    break;
                case ETriggerDataType.RoundBuff:
                    var battlePlayerData = GamePlayManager.Instance.GamePlayData.BattleData.GetBattlePlayerData(effectUnitEntity.UnitCamp);
                    battlePlayerData.RoundBuffs.Add(triggerData.BuffID);
                    break;
                default:
                    break;
            }

        }


        

        // public void RoundStartTrigger()
        // {
        //     if(ActionProgress != EActionProgress.RoundStart)
        //         return;
        //     
        //     foreach (var kv in RoundFightData.RoundStartDatas)
        //     {
        //         foreach (var triggerDatas in kv.Value.TriggerDatas.Values)
        //         {
        //             foreach (var triggerData in triggerDatas)
        //             {
        //                 TriggerAction(triggerData);
        //             }
        //             
        //         }
        //         
        //         
        //     }
        //
        //     GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        //     GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
        //
        //
        //     GameUtility.DelayExcute(1f, () =>
        //     {
        //         AcitonUnitIdx = 0;
        //         BattleManager.Instance.NextAction();
        //         BattleManager.Instance.ContinueAction();
        //     });
        // }
        
        public void RoundStartBuffTrigger()
        {
            if(ActionProgress != EActionProgress.RoundStartBuff)
                return;
            
            var buffDataKeys = RoundFightData.RoundStartBuffDatas.Keys.ToList();
            while (true)
            {
                if (AcitonUnitIdx >= buffDataKeys.Count)
                {
                    AcitonUnitIdx = 0;
                    BattleManager.Instance.NextAction();
                    break;
                }
                

                if (RoundFightData.RoundStartBuffDatas.ContainsKey(buffDataKeys[AcitonUnitIdx]))
                {
                    break;
                }
                else
                {
                    AcitonUnitIdx++;
                }
            }


            if(ActionProgress != EActionProgress.RoundStartBuff)
                return;
            
            var actionData = RoundFightData.RoundStartBuffDatas[buffDataKeys[AcitonUnitIdx]];
            
            var isAttack = false;
            foreach (var trigger in actionData.TriggerDatas)
            {
                foreach (var triggerData in trigger.Value)
                {
                    isAttack = true;
                    TriggerAction(triggerData);
                }
            }

            if (isAttack)
            {

                GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
            }

            AcitonUnitIdx++;

            GameUtility.DelayExcute(isAttack ? 1.5f : 0.5f, () =>
            {
                BattleManager.Instance.ContinueAction();
            });

            
        }
        
        public void RoundStartUnitTrigger()
        {
            UnitAttack(RoundFightData.RoundStartUnitDatas, EActionProgress.RoundStartUnit);
        }
        
        public void RoundEndTrigger()
        {
            if(ActionProgress != EActionProgress.RoundEnd)
                return;
            
            foreach (var kv in RoundFightData.RoundEndDatas)
            {
                foreach (var triggerDatas in kv.Value.TriggerDatas.Values)
                {
                    foreach (var triggerData in triggerDatas)
                    {
                        TriggerAction(triggerData);
                    }
                    
                }
                
                
            }

            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());

 
            GameUtility.DelayExcute(1f, () =>
            {
                AcitonUnitIdx = 0;
                BattleManager.Instance.NextAction();
                BattleManager.Instance.ContinueAction();
            });
        }


        public void UnitAttack(Dictionary<int, ActionData> unitAttackDatas, EActionProgress actionProgress)
        {
            var unitKeys = BattleUnitDatas.Keys.ToList();
            while (true)
            {
                if (AcitonUnitIdx >= unitKeys.Count)
                {
                    AcitonUnitIdx = 0;
                    BattleManager.Instance.NextAction();
                    break;
                }
                

                if (unitAttackDatas.ContainsKey(unitKeys[AcitonUnitIdx]))
                {
                    break;
                }
                else
                {
                    AcitonUnitIdx++;
                }
            }


            if(ActionProgress != actionProgress)
                return;
            
            var actionData = unitAttackDatas[unitKeys[AcitonUnitIdx]];
            
            var isAttack = false;
            foreach (var trigger in actionData.TriggerDatas)
            {
                foreach (var triggerData in trigger.Value)
                {
                    isAttack = true;
                    TriggerAction(triggerData);
                }
            }

            var time = 0.5f;
            if (isAttack)
            {
                time += 1.5f;
                
                var unit = GetUnitByID(unitKeys[AcitonUnitIdx]);
                unit.AttackInRound = true;
                BattleUnitManager.Instance.BattleUnitEntities[unitKeys[AcitonUnitIdx]].Attack();
                GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());

            }
            
            var moveTime = 0f;
            var maxMoveTime = 0f;
            foreach (var kv in actionData.MoveData.MoveUnitDatas)
            {
                if (kv.Value.UnitActionState == EUnitActionState.Fly)
                {
                    moveTime = BattleUnitManager.Instance.BattleUnitEntities[kv.Value.UnitID].Fly(kv.Value.MoveActionData);
                }
                else if (kv.Value.UnitActionState == EUnitActionState.Run)
                {
                    moveTime = BattleUnitManager.Instance.BattleUnitEntities[kv.Value.UnitID].Run(kv.Value.MoveActionData);
                }

                if (moveTime > maxMoveTime)
                {
                    maxMoveTime = moveTime;
                }
            }

            time += maxMoveTime;

            AcitonUnitIdx++;
            

            GameUtility.DelayExcute(time, () =>
            {
                BattleManager.Instance.ContinueAction();
            });
        }

        

        

        public void SoliderAttack()
        {
            UnitAttack(RoundFightData.SoliderAttackDatas, EActionProgress.SoliderAttack);
            
        }
        
        public void SoliderActiveAttack()
        {
            BattleFightManager.Instance.ActionProgress = EActionProgress.SoliderActiveAttack;
            UnitAttack(RoundFightData.SoliderActiveAttackDatas, EActionProgress.SoliderActiveAttack);
            
        }
        
        public void SoliderAutoAttack()
        {
            BattleFightManager.Instance.ActionProgress = EActionProgress.SoliderAutoAttack;
            UnitAttack(RoundFightData.SoliderActiveAttackDatas, EActionProgress.SoliderAutoAttack);
            
        }
        
        public void ThirdUnitAttack()
        {
            UnitAttack(RoundFightData.ThirdUnitAttackDatas, EActionProgress.ThirdUnitAttack);

        }
        
        public void EnemyMove()
        {
            UnitMove(RoundFightData.EnemyMovePaths, RoundFightData.EnemyMoveDatas, RoundFightData.EnemyAttackDatas,
                EActionProgress.EnemyMove);
            
            
        }
        
        public void ThirdUnitMove()
        {
            UnitMove(RoundFightData.ThirdUnitMovePaths, RoundFightData.ThirdUnitMoveDatas, RoundFightData.ThirdUnitAttackDatas,
                EActionProgress.ThirdUnitMove);
            
        }


        private void UnitMove(Dictionary<int, List<int>> unitMovePaths, Dictionary<int, MoveActionData> unitMoveDatas, Dictionary<int, ActionData> unitAttackDatas, EActionProgress actionProgress)
        {
            var unitKeys = unitMovePaths.Keys.ToList();
            while (true)
            {
                if (AcitonUnitIdx >= unitKeys.Count)
                {
                    AcitonUnitIdx = 0;
                    BattleManager.Instance.NextAction();
                    break;
                }
                
                var unit = BattleUnitManager.Instance.GetUnitByID(unitKeys[AcitonUnitIdx]);
                
                if (unit.CurHP <= 0)
                {
                    AcitonUnitIdx++;
                    continue;
                }
                
                if (unitMovePaths[unitKeys[AcitonUnitIdx]] == null || unitMovePaths[unitKeys[AcitonUnitIdx]].Count <= 0)
                    continue;

                if (unitMoveDatas.ContainsKey(unitKeys[AcitonUnitIdx]))
                {
                    break;
                }
                else
                {
                    AcitonUnitIdx++;
                }
            }
            
            if(ActionProgress != actionProgress)
                return;
            
            var unitID = unitKeys[AcitonUnitIdx];
            var moveActionData = unitMoveDatas[unitKeys[AcitonUnitIdx]];

            var runTime = 0f;

            if (moveActionData.MoveGridPosIdxs.Count > 1)
            {
                runTime = BattleUnitManager.Instance.BattleUnitEntities[unitKeys[AcitonUnitIdx]].Run(moveActionData);
            }

            AcitonUnitIdx++;
            
            GameUtility.DelayExcute(runTime, () =>
            {
                if (unitAttackDatas.ContainsKey(unitID))
                {
                    var attackTime = UnitAttack(unitID, unitAttackDatas[unitID]);
                    GameUtility.DelayExcute(attackTime, () =>
                    {
                        BattleManager.Instance.ContinueAction();
                    });
                }
                else
                {
                    BattleManager.Instance.ContinueAction();
                }
                
                
            });
        }
        
        private float UnitAttack(int unitID, ActionData actionData)
        {

            var isAttack = false;
            foreach (var trigger in actionData.TriggerDatas)
            {
                foreach (var triggerData in trigger.Value)
                {
                    isAttack = true;
                    TriggerAction(triggerData);
                }
            }

            var time = 0.5f;
            if (isAttack)
            {
                time += 0.5f;
                
                var unit = GetUnitByID(unitID);
                unit.AttackInRound = true;
                BattleUnitManager.Instance.BattleUnitEntities[unitID].Attack();
                GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());

            }
            
            var moveTime = 0f;
            var maxMoveTime = 0f;
            foreach (var kv in actionData.MoveData.MoveUnitDatas)
            {
                if (kv.Value.UnitActionState == EUnitActionState.Fly)
                {
                    moveTime = BattleUnitManager.Instance.BattleUnitEntities[kv.Value.UnitID].Fly(kv.Value.MoveActionData);
                }
                else if (kv.Value.UnitActionState == EUnitActionState.Run)
                {
                    moveTime = BattleUnitManager.Instance.BattleUnitEntities[kv.Value.UnitID].Run(kv.Value.MoveActionData);
                }

                if (moveTime > maxMoveTime)
                {
                    maxMoveTime = moveTime;
                }
            }

            time += maxMoveTime;

            return time;
        }

        public void EnemyAttack()
        {
            UnitAttack(RoundFightData.EnemyAttackDatas, EActionProgress.EnemyAttack);
            
        }

        public void CalculateHeroHPDelta(MoveActionData moveActionData)
        {
            CalculateHeroHPDelta(moveActionData.ActionUnitID, moveActionData.TriggerDatas, true);
        }
        
        public void CalculateHeroHPDelta(ActionData actionData)
        {
            CalculateHeroHPDelta(actionData.ActionUnitID, actionData.TriggerDatas);
        }

        private class HPDeltaData
        {
            public int Value;
            public int Key;
        }
        
        public void CalculateHeroHPDelta(int actionUnitID, Dictionary<int, List<TriggerData>> triggerDatas, bool isMoveTriggerData = false)
        {
            
            var hpDeltaDict = new Dictionary<EUnitCamp, HPDeltaData>()
            {
                [EUnitCamp.Player1] = new HPDeltaData(),
                [EUnitCamp.Player2] = new HPDeltaData(),
            };

            foreach (var kv in triggerDatas)
            {
                foreach (var triggerData in kv.Value)
                {
                    var effectUnit = GameUtility.GetUnitByID(triggerData.EffectUnitID);
                    if(effectUnit == null)
                        continue;

                    var triggerValue = triggerData.Value + triggerData.DeltaValue;
                    var value = effectUnit.AddHeroHP;
                    
                    if(BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.UnitDeadUnRecoverHeroHP) && effectUnit.CurHP <= 0)
                        continue;

                    if (effectUnit.GetStateCount(EUnitState.UnRecover) > 0 && ! GameUtility.ContainRoundState(GamePlayManager.Instance.GamePlayData,
                            EBuffID.Spec_CurseUnEffect))
                        continue;
                    
                    effectUnit.AddHeroHP = 0;
                    //triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
                    if (!(triggerData.BattleUnitAttribute == EUnitAttribute.HP && triggerValue < 0))
                        continue;

                    var unit = GameUtility.GetUnitByID(triggerData.EffectUnitID, true);
                    if(unit == null)
                        continue;
                    
                    if (unit.UnitCamp == EUnitCamp.Enemy)
                    {
                        triggerData.ChangeHPInstantly = true;
                        continue;
                    }

                    if (unit.UnitRole != EUnitRole.Hero)
                    {
                        triggerData.ChangeHPInstantly = true;
                    }

                    var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(unit.UnitCamp);
                    var isHeroUnit = playerData != null && playerData.BattleHero != null &&
                                     playerData.BattleHero.ID == unit.ID;
                    
                    hpDeltaDict[unit.UnitCamp].Value += (int) (isHeroUnit ? triggerValue : Math.Abs(value));
                    hpDeltaDict[unit.UnitCamp].Key = isMoveTriggerData ? kv.Key : playerData.BattleHero.ID;
                }
                

                
            }
            
            foreach (var kv2 in hpDeltaDict)
            {
                var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(kv2.Key);
                if (playerData != null && playerData.BattleHero != null && kv2.Value.Value != 0)
                {
                    var triggerData = new TriggerData()
                    {
                        EffectUnitID = playerData.BattleHero.ID,
                        TriggerDataType = ETriggerDataType.RoleAttribute,
                        BattleUnitAttribute = EUnitAttribute.HP,
                        Value = kv2.Value.Value,
                        ChangeHPInstantly = true,
                    };
                    triggerData.ActionUnitID = triggerData.Value < 0 ? actionUnitID : -1;
                    triggerData.OwnUnitID = triggerData.Value < 0 ? actionUnitID : -1;
                    
                    if(!triggerDatas.ContainsKey(kv2.Value.Key))
                    {
                        triggerDatas.Add(kv2.Value.Key, new List<TriggerData>());
                    }

                    BattleBuffManager.Instance.CacheTriggerData(triggerData, triggerDatas[kv2.Value.Key]);

                }
            }
        }

        // public void CheckHPDeltaTriggerData(int actionUnitID, TriggerData triggerData, Dictionary<int, List<TriggerData>> triggerDatas)
        // {
        //     var hpDelta = 0;
        //     
        //     var effectUnit = GameUtility.GetUnitByID(triggerData.EffectUnitID);
        //     if(effectUnit == null)
        //         return;
        //
        //     var triggerValue = triggerData.Value + triggerData.DeltaValue;
        //     var value = effectUnit.AddHeroHP;
        //             
        //     if(BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.UnitDeadUnRecoverHeroHP) && effectUnit.CurHP <= 0)
        //         return;
        //
        //     if (effectUnit.GetStateCount(EUnitState.UnRecover) > 0 && ! GameUtility.ContainRoundState(GamePlayManager.Instance.GamePlayData,
        //         EBuffID.Spec_CurseUnEffect))
        //         return;
        //             
        //     effectUnit.AddHeroHP = 0;
        //     //triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
        //     if (!(triggerData.BattleUnitAttribute == EUnitAttribute.HP && triggerValue < 0))
        //         return;
        //
        //     var unit = GameUtility.GetUnitByID(triggerData.EffectUnitID, true);
        //     if(unit == null)
        //         return;
        //             
        //     if (unit.UnitCamp == EUnitCamp.Enemy)
        //     {
        //         triggerData.ChangeHPInstantly = true;
        //         return;
        //     }
        //
        //     if (unit.UnitRole != EUnitRole.Hero)
        //     {
        //         triggerData.ChangeHPInstantly = true;
        //     }
        //
        //     var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(unit.UnitCamp);
        //     var isHeroUnit = playerData != null && playerData.BattleHero != null &&
        //                      playerData.BattleHero.ID == unit.ID;
        //             
        //     hpDelta = (int) (isHeroUnit ? triggerValue : Math.Abs(value));
        //     
        //     
        //     if (playerData != null && playerData.BattleHero != null && hpDelta != 0)
        //     {
        //         var hpDeltaTriggerData = new TriggerData()
        //         {
        //             EffectUnitID = playerData.BattleHero.ID,
        //             TriggerDataType = ETriggerDataType.RoleAttribute,
        //             BattleUnitAttribute = EUnitAttribute.HP,
        //             Value = hpDelta,
        //             ChangeHPInstantly = true,
        //         };
        //         hpDeltaTriggerData.ActionUnitID = hpDeltaTriggerData.Value < 0 ? actionUnitID : -1;
        //         hpDeltaTriggerData.OwnUnitID = hpDeltaTriggerData.Value < 0 ? actionUnitID : -1;
        //             
        //         if(!triggerDatas.ContainsKey(playerData.BattleHero.ID))
        //         {
        //             triggerDatas.Add(playerData.BattleHero.ID, new List<TriggerData>());
        //         }
        //
        //         BattleBuffManager.Instance.CacheTriggerData(hpDeltaTriggerData, triggerDatas[playerData.BattleHero.ID]);
        //
        //     }
        //     
        // }
        
        public void UseCardTrigger()
        {
            BattleFightManager.Instance.ActionProgress = EActionProgress.UseCardTrigger;
            
            var unitKeys = BattleUnitDatas.Keys.ToList();
            while (true)
            {
                if (AcitonUnitIdx >= unitKeys.Count)
                {
                    AcitonUnitIdx = 0;
                    BattleManager.Instance.NextAction();
                    break;
                }
                
                if (RoundFightData.UseCardTriggerDatas.ContainsKey(unitKeys[AcitonUnitIdx]))
                {
                    break;
                }
                else
                {
                    AcitonUnitIdx++;
                }
            }
            
            if(ActionProgress != EActionProgress.UseCardTrigger)
                return;
            
            var actionData = RoundFightData.UseCardTriggerDatas[unitKeys[AcitonUnitIdx]];
            
            var isAttack = false;
            foreach (var trigger in actionData.TriggerDatas)
            {
                foreach (var triggerData in trigger.Value)
                {
                    isAttack = true;
                    TriggerAction(triggerData);
                }
            }

            if (isAttack)
            {
                Log.Debug("AA:" + unitKeys[AcitonUnitIdx]);
                BattleUnitManager.Instance.BattleUnitEntities[unitKeys[AcitonUnitIdx]].Attack();
                GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
            }
            
            
            AcitonUnitIdx++;

            GameUtility.DelayExcute(isAttack ? 1.5f : 0.5f, () =>
            {
                UseCardTrigger();
            });
        }

        public List<TriggerData> GetMoveTriggerDatas(MoveActionData moveActionData, int moveIdx)
        {
            if(moveActionData == null)
                return null;
            
            if(moveActionData.TriggerDatas.ContainsKey(moveIdx))
                return moveActionData.TriggerDatas[moveIdx];
            
            return null;
        }

        public void MoveEffectAction(EUnitActionState unitActionState, MoveActionData moveActionData, int moveIdx, int moveUnitID)
        {
            var triggerDatas = GetMoveTriggerDatas(moveActionData, moveIdx);//unitActionState == EUnitActionState.Run ? GetRunTriggerDatas(effectID, moveIdx) : GetFlyTriggerDatas(actionID, effectID, moveIdx));
            if(triggerDatas == null)
                return;
            
            foreach (var triggerData in triggerDatas)
            {
                
                if (triggerData.ActionUnitID != -1 && triggerData.EffectUnitID != triggerData.ActionUnitID)
                {
                    var actionUnit = BattleUnitManager.Instance.GetUnitByID(triggerData.ActionUnitID);
                    Log.Debug("ActionUnitID:" + triggerData.ActionUnitID);
                    if (!(!triggerData.ChangeHPInstantly && HeroManager.Instance.IsHero(triggerData.EffectUnitID)))
                    {
                        if (actionUnit.ID == moveUnitID)
                        {
                            actionUnit.RunAttack();
                        }
                        else
                        {
                            actionUnit.Attack();
                        }
                        
                    }
                }
                
                // if (unit.CurHP <= 0)
                // {
                //     unit.Dead();
                // }
                
                TriggerAction(triggerData);
            }
            GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        }

        
        public List<int> GetGridObstacles()
        {
            var obstacles = new List<int>(Constant.Area.ObstacleCount);
            
            foreach (var kv in BattleAreaManager.Instance.GridEntities)
            {
                if (kv.Value.BattleGridEntityData.GridType == EGridType.Obstacle)
                {
                    obstacles.Add(kv.Value.BattleGridEntityData.GridPosIdx);
                    
                }
            }
            
            return obstacles;
        }
        
        public void CalculateEnemyPaths()
        {
            CalculateUnitPaths(EUnitCamp.Enemy, RoundFightData.EnemyMovePaths);
        }
        
        public void CalculateThirdUnitPaths()
        {
            CalculateUnitPaths(EUnitCamp.Third, RoundFightData.ThirdUnitMovePaths);
        }

        public bool InObstacle(Dictionary<int, EGridType> obstacleMask, List<int> movePaths)
        {
            if (movePaths[0] == movePaths[movePaths.Count - 1])
                return false;
            
            foreach (var gridPosIdx in movePaths)
            {
                //(gridPosIdx != movePaths[movePaths.Count - 1] && obstacleMask[gridPosIdx] == EGridType.Obstacle)
                if(obstacleMask[gridPosIdx] == EGridType.Obstacle ||
                   (gridPosIdx == movePaths[movePaths.Count - 1] && obstacleMask[gridPosIdx] == EGridType.Unit))
                    return true;
            }
            

            
            return false;

        }
        
        
        public List<int> GetRunPaths(int startPosIdx, int endPosIdx, List<int> runPaths)
        {
            var startCoord = GameUtility.GridPosIdxToCoord(startPosIdx);
            var endCoord = GameUtility.GridPosIdxToCoord(endPosIdx);
            
            runPaths.Clear();
            runPaths.Add(startPosIdx);

            if (RoundFightData.GamePlayData.BattleData.GridTypes[endPosIdx] != EGridType.Empty)
                return runPaths;
            
            var deltaX = endCoord.x - startCoord.x;
            var deltaY = endCoord.y - startCoord.y;
            var signX = 0;
            var signY = 0;
            if (deltaX < 0)
            {
                signX = -1;
            }
            else if (deltaX > 0)
            {
                signX = 1;
            }
            
            if (deltaY < 0)
            {
                signY = -1;
            }
            else if (deltaY > 0)
            {
                signY = 1;
            }

            if (deltaX == 0 && deltaY == 0)
                return runPaths;


            var idx = 0;
            var isMoveDirect = false;
            var targetCoord = startCoord;
            while (true)
            {

                targetCoord = targetCoord +
                              new Vector2Int(signX, signY);
                
                if (!GameUtility.InGridRange(targetCoord))
                    break;
                
                var gridPosIdx =
                    GameUtility.GridCoordToPosIdx(targetCoord);
                
                if (RoundFightData.GamePlayData.BattleData.GridTypes[gridPosIdx] == EGridType.Obstacle)
                    break;
                
                
                runPaths.Add(gridPosIdx);

                var gridProp = RoundFightData.GamePlayData.BattleData.Contain(EGridPropID.MoveDirect, gridPosIdx);
                if (gridProp != null && !MoveDirectPropUseDict[gridProp.ID].UseInRound)
                {
                    MoveDirectPropUseDict[gridProp.ID].UseInRound = true;
                    var newDirectCoord = Constant.Battle.EPos2CoordMap[MoveDirectPropUseDict[gridProp.ID].Direct];
                    signX = newDirectCoord.x;
                    signY = newDirectCoord.y;
                    isMoveDirect = true;

                }
                // var gridProp = (
                //     EGridPropID.MoveDirect, gridPosIdx);
                // if (gridProp != null && gridProp is Data_GridPropMoveDirect moveDirect && !moveDirect.UseInRound)
                // {
                //     moveDirect.UseInRound = true;
                //     var newDirectCoord = Constant.Battle.EPos2CoordMap[moveDirect.Direct];
                //     signX = newDirectCoord.x;
                //     signY = newDirectCoord.y;
                //     isMoveDirect = true;
                //}
                
                if (!isMoveDirect && targetCoord == endCoord)
                {
                    break;
                }

                idx++;
            }
            
            
            // if (deltaX != 0)
            // {
            //     
            //
            // }
            // else if (deltaY != 0)
            // {
            //     var signY = deltaY < 0 ? -1 : 1;
            //     for (int i = 0; i < deltaY * signY; i++)
            //     {
            //         var gridPosIdx =
            //             GameUtility.GridCoordToPosIdx(startCoord + new Vector2Int(0, (i + 1) * signY));
            //         movePosIdxs.Add(gridPosIdx);
            //
            //
            //     }
            // }

            return runPaths;
        }

        public List<int> GetFlyPaths(int startPosIdx, Vector2Int direct)
        {
            var startCoord = GameUtility.GridPosIdxToCoord(startPosIdx);
            
            var flyPosIdxs = new List<int>();
            flyPosIdxs.Add(startPosIdx);
            
            var signX = 0;
            var signY = 0;
            if (direct.x < 0)
            {
                signX = -1;
            }
            else if (direct.x > 0)
            {
                signX = 1;
            }
            
            if (direct.y < 0)
            {
                signY = -1;
            }
            else if (direct.y > 0)
            {
                signY = 1;
            }


            var idx = 0;
            var isMoveDirect = false;
            var targetCoord = startCoord;
            var lastGridPosIdx = startPosIdx;
            while (true)
            {
                targetCoord = targetCoord +
                              new Vector2Int(signX, signY);
                
                if (!GameUtility.InGridRange(targetCoord))
                    break;
                
                var gridPosIdx =
                    GameUtility.GridCoordToPosIdx(targetCoord);
                
                
                
                flyPosIdxs.Add(gridPosIdx);

                var unit = GameUtility.GetUnitByGridPosIdx(gridPosIdx);
                // && unit.GetStateCount(EUnitState.UnBePass) > 0
                if (unit != null)
                {
                    flyPosIdxs.Add(lastGridPosIdx);
                    break;
                }
                
                if (BattleManager.Instance.BattleData.GridTypes[gridPosIdx] != EGridType.Empty)
                {
                    break;
                }
                
                var gridProp = RoundFightData.GamePlayData.BattleData.Contain(EGridPropID.MoveDirect, gridPosIdx);
                if (gridProp != null && !MoveDirectPropUseDict[gridProp.ID].UseInRound)
                {
                    MoveDirectPropUseDict[gridProp.ID].UseInRound = true;
                    var newDirectCoord = Constant.Battle.EPos2CoordMap[MoveDirectPropUseDict[gridProp.ID].Direct];
                    signX = newDirectCoord.x;
                    signY = newDirectCoord.y;
                    isMoveDirect = true;

                }
                idx++;
                
                lastGridPosIdx = gridPosIdx;
            }

            return flyPosIdxs;
        }
        
        public List<int> GetFlyPaths(int startPosIdx, int endPosIdx)
        {
            var startCoord = GameUtility.GridPosIdxToCoord(startPosIdx);
            var endCoord = GameUtility.GridPosIdxToCoord(endPosIdx);
            var direct = endCoord - startCoord;
            
            var flyPosIdxs = new List<int>();
            flyPosIdxs.Add(startPosIdx);
            
            var signX = 0;
            var signY = 0;
            if (direct.x < 0)
            {
                signX = -1;
            }
            else if (direct.x > 0)
            {
                signX = 1;
            }
            
            if (direct.y < 0)
            {
                signY = -1;
            }
            else if (direct.y > 0)
            {
                signY = 1;
            }


            var idx = 0;
            //var isMoveDirect = false;
            var targetCoord = startCoord;
            var lastGridPosIdx = startPosIdx;
            while (true)
            {
                targetCoord = targetCoord +
                              new Vector2Int(signX, signY);
                
                if (!GameUtility.InGridRange(targetCoord))
                    break;
                
                var gridPosIdx =
                    GameUtility.GridCoordToPosIdx(targetCoord);
                
                flyPosIdxs.Add(gridPosIdx);
                

                var unit = GameUtility.GetUnitByGridPosIdx(gridPosIdx);
                if (unit != null && unit.GetStateCount(EUnitState.UnBePass) > 0)
                {
                    flyPosIdxs.Add(lastGridPosIdx);
                    break;
                }
                
                
                var gridProp = RoundFightData.GamePlayData.BattleData.Contain(EGridPropID.MoveDirect, gridPosIdx);
                var isMoveDirect = gridProp != null && !MoveDirectPropUseDict[gridProp.ID].UseInRound;
                if (isMoveDirect)
                {
                    MoveDirectPropUseDict[gridProp.ID].UseInRound = true;
                    var newDirectCoord = Constant.Battle.EPos2CoordMap[MoveDirectPropUseDict[gridProp.ID].Direct];
                    signX = newDirectCoord.x;
                    signY = newDirectCoord.y;

                }
                
                

                if (targetCoord == endCoord)
                {
                    var gridType = RoundFightData.GamePlayData.BattleData.GridTypes[gridPosIdx];
                    if (gridType == EGridType.Obstacle)
                    {
                        flyPosIdxs.Remove(gridPosIdx);
                        break;
                    }
                    
                    if (gridType == EGridType.Unit || gridType == EGridType.TemporaryUnit)
                    {
                        flyPosIdxs.Add(lastGridPosIdx);
                        break;
                    }
                    
                    if(!isMoveDirect)
                        break;

                }
                    
                lastGridPosIdx = gridPosIdx;   
                idx++;
            }

            return flyPosIdxs;
        }
        
        public void RefreshPropMoveDirectUseInRound()
        {
            foreach (var kv in MoveDirectPropUseDict)
            {
                if(RoundFightData.GamePlayData.BattleData.GridPropDatas.ContainsKey(kv.Key))
                
                MoveDirectPropUseDict[kv.Key].UseInRound = (RoundFightData.GamePlayData.BattleData.GridPropDatas[kv.Key] as Data_GridPropMoveDirect).UseInRound;

            }
        }
        
        

        public void ChangeHP(Data_BattleUnit unit, float value, EHPChangeType hpChangeType, bool useDefense = true, bool changeHPInstantly = false)
        {
            BattleManager.Instance.ChangeHP(unit, (int)value, RoundFightData.GamePlayData, hpChangeType, useDefense, true, changeHPInstantly);

        }
        
        private bool IsEnemy(int unit1ID, int unit2ID)
        {
            var unit1 = BattleFightManager.Instance.GetUnitByID(unit1ID);
            var unit2 = BattleFightManager.Instance.GetUnitByID(unit2ID);

            return unit1.UnitCamp != unit2.UnitCamp;
        }
        
        public List<int> GetEffectUnitIDs(BuffData buffData, int ownUnitID, int actionUnitID, int effectUnitID, int actionUnitGridPosIdx, int actionUnitPreGridPosIdx)
        {
            var realEffectUnitIDs = new List<int>();
            
            var actionUnit = BattleFightManager.Instance.GetUnitByID(actionUnitID);
            //var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
           
            
            foreach (var triggerTarget in buffData.TriggerTargets)
            {
                switch (triggerTarget)
                {
                    case ETriggerTarget.Effect:
                        if (effectUnitID != -1)
                        {
                            var isEnemy = IsEnemy(actionUnitID, effectUnitID);
                            
                            if (isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy))
                            {
                                realEffectUnitIDs.Add(effectUnitID);
                            }
                            else if (!isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us))
                            {
                                realEffectUnitIDs.Add(effectUnitID);
                            }
                        }

                        break;
                    case ETriggerTarget.Action:
                        if (effectUnitID != -1)
                        {
                            var isEnemy = IsEnemy(actionUnitID, effectUnitID);
                            
                            if (isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy))
                            {
                                realEffectUnitIDs.Add(actionUnitID);
                            }
                            else if (!isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us))
                            {
                                realEffectUnitIDs.Add(actionUnitID);
                            }
                        }


                        break;
                    case ETriggerTarget.Hero:
                        realEffectUnitIDs.Add(PlayerData.BattleHero.ID);
                        break;
                    case ETriggerTarget.All:
                        foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
                        {
                            if (buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy) && kv.Value.UnitCamp != actionUnit.UnitCamp)
                            {
                                realEffectUnitIDs.Add(kv.Value.ID);
                            }
                            else if (buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us) && kv.Value.UnitCamp == actionUnit.UnitCamp)
                            {
                                realEffectUnitIDs.Add(kv.Value.ID);
                            }
                        }
                        
                        break;
                    case ETriggerTarget.InRange:
                        var range = GameUtility.GetRange(actionUnit.GridPosIdx, buffData.TriggerRange,
                            actionUnit.UnitCamp, buffData.TriggerUnitCamps);
                        foreach (var gridPosIdx in range)
                        {
                            var unit = BattleFightManager.Instance.GetUnitByGridPosIdx(gridPosIdx);
                            if (unit != null)
                            {
                                realEffectUnitIDs.Add(unit.ID);
                            }
                        }
                        break;
                    case ETriggerTarget.Vertical:
                        var actionUnitCoord = GameUtility.GridPosIdxToCoord(actionUnitGridPosIdx);
                        var actionUnitLastCoord = GameUtility.GridPosIdxToCoord(actionUnitPreGridPosIdx);
                        var direct = actionUnitCoord - actionUnitLastCoord;
 
                        var vertical1 = new Vector2Int(-direct.y, direct.x);
                        var vertical2 = new Vector2Int(direct.y, -direct.x);
                        var gridPosIdx1 = GameUtility.GridCoordToPosIdx(actionUnitCoord + vertical1);
                        var gridPosIdx2 = GameUtility.GridCoordToPosIdx(actionUnitCoord + vertical2);
                        var unit1 = BattleFightManager.Instance.GetUnitByGridPosIdx(gridPosIdx1);
                        if (unit1 != null)
                        {
                            realEffectUnitIDs.Add(unit1.ID);
                        }
                        var unit2 = BattleFightManager.Instance.GetUnitByGridPosIdx(gridPosIdx2);
                        if (unit2 != null)
                        {
                            realEffectUnitIDs.Add(unit2.ID);
                        }

                        break;
                    default:
                        break;
                }
            }

            GameUtility.SortHeroIDToLast(realEffectUnitIDs);
            
            return realEffectUnitIDs;
        }
        
        public int GetDamage(int unitID)
        {
            Data_BattleUnit unit  = null;
            if (RoundFightData.GamePlayData.BattleData.BattleUnitDatas.ContainsKey(unitID))
            {
                unit  = BattleUnitManager.Instance.BattleUnitDatas[unitID];
            }
            
            BattleUnitManager.Instance.GetBuffValue(RoundFightData.GamePlayData, unitID, out List<BuffValue> triggerBuffDatas);
            var damage = 0;
            
            foreach (var triggerBuffData in triggerBuffDatas)
            {
                if (triggerBuffData.BuffData.UnitAttribute == EUnitAttribute.HP &&  triggerBuffData.ValueList[0] <= 0)
                {
                    damage += (int) triggerBuffData.ValueList[0];
                }
            }
        
            return damage + unit.BaseDamage;
        }
        
        public void KillTrigger(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            var unit = BattleFightManager.Instance.GetUnitByID(triggerData.ActionUnitID);
  
            BattleBuffManager.Instance.BuffsTrigger(RoundFightData.GamePlayData, unit, triggerData, triggerDatas, EBuffTriggerType.Kill);

        }

        public void DeadTrigger(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            var effectUnit = BattleFightManager.Instance.GetUnitByID(triggerData.EffectUnitID);
            var actionUnit = BattleFightManager.Instance.GetUnitByID(triggerData.ActionUnitID);
            if(effectUnit == null || effectUnit.CurHP > 0)
                return;

            
            BattleBuffManager.Instance.BuffsTrigger(RoundFightData.GamePlayData, effectUnit, triggerData, triggerDatas, EBuffTriggerType.Dead);

            TriggerUnitData(effectUnit.ID, actionUnit.ID, effectUnit.GridPosIdx, EBuffTriggerType.Dead, triggerDatas);
            
            var enemyDeadDebuffToOtherEnemy = BattleFightManager.Instance.RoundFightData.GamePlayData.GetUsefulBless(EBlessID.EnemyDeadDebuffToOtherEnemy, BattleManager.Instance.CurUnitCamp);
            
            if (actionUnit != null && effectUnit.UnitCamp != actionUnit.UnitCamp && enemyDeadDebuffToOtherEnemy != null)
            {
                var otherEnemies = new List<Data_BattleUnit>();
                foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
                {
                    if (kv.Value.UnitCamp != actionUnit.UnitCamp && kv.Value.CurHP > 0)
                    {
                        otherEnemies.Add(kv.Value);
                    }
                }

                if (otherEnemies.Count > 0)
                {
                    var randomEnemyIdx = Random.Next(0, otherEnemies.Count);
                    foreach (var kv in effectUnit.UnitState.UnitStates)
                    {
                        if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.Negative].Contains(kv.Key))
                        {
                            otherEnemies[randomEnemyIdx].ChangeState(kv.Key, kv.Value);
                        }
                    }
                }

            }

            
        }

        public void CalculateUnitPaths(EUnitCamp unitCamp, Dictionary<int, List<int>> movePaths)
        {
            RefreshPropMoveDirectUseInRound();

            movePaths.Clear();

            var unitIDs = new List<int>();

            foreach (var kv in RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
                var battleUnitData = kv.Value;

                if (battleUnitData.UnitCamp != unitCamp)
                    continue;


                if (battleUnitData.GetAllStateCount(EUnitState.UnMove) > 0 &&
                    !GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    continue;

                // var battleEnemyData = battleUnitData as Data_BattleMonster;
                // var drEnemy = GameEntry.DataTable.GetEnemy(battleEnemyData.MonsterID);
                // var buffData = BattleBuffManager.Instance.GetBuffData(drEnemy.OwnBuffs[0]);
                //
                // var attackRange = GameUtility.GetRange(battleEnemyData.GridPosIdx, buffData.TriggerRange, unitCamp,
                //     buffData.TriggerUnitCamps, true);
                //
                // if (attackRange.Contains(BattleHeroManager.Instance.BattleHeroData.GridPosIdx))
                // {
                //     movePaths.Add(battleUnitData.ID, new List<int>()
                //     {
                //         battleUnitData.GridPosIdx
                //     });
                //     continue;
                // }

                unitIDs.Add(battleUnitData.ID);
            }

            // unitIDs.Sort((unit1ID, unit2ID) =>
            // {
            //     var unit1 = BattleUnitManager.Instance.GetUnitByID(unit1ID).BattleUnit;
            //     var unit2 = BattleUnitManager.Instance.GetUnitByID(unit2ID).BattleUnit;
            //     
            //     
            //     if (unit1.UnitCamp != EUnitCamp.Enemy || unit2.UnitCamp != EUnitCamp.Enemy)
            //     {
            //         return 0;
            //     }
            //
            //     var drEnemy1 = GameEntry.DataTable.GetEnemy(unit1.ID);
            //     var drEnemy2 = GameEntry.DataTable.GetEnemy(unit2.ID);
            //     
            //     if (drEnemy2.AttackType == EAttackType.Lock && drEnemy1.AttackType == EAttackType.Dynamic)
            //         return -1;
            //
            //
            //     return 1;
            //
            // });

            
            List<int> obstacleEnemies = new List<int>();
            foreach (var kv in RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            {
                if(kv.Value.UnitCamp != unitCamp)
                    continue;
                
                obstacleEnemies.Add(kv.Value.GridPosIdx);
            }

            // for (int i = unitIDs.Count - 1; i >= 0; i--)
            // {
            //     if(i >= Constant.Enemy.EnemyActionCount)
            //         unitIDs.RemoveAt(i);
            // }

            if (unitIDs.Count > 0)
            {
                CalculateUnitPaths(unitCamp, unitIDs, obstacleEnemies, movePaths);
            }

        }

        public void  CalculateUnitPaths(EUnitCamp unitCamp, List<int> actionUnitIDs, List<int> obstacleEnemies, Dictionary<int, List<int>> movePaths)
        {
            var curObstacleMask = new Dictionary<int, EGridType>();

            //var gridObstacles = GetGridObstacles();

            foreach (var kv in RoundFightData.GamePlayData.BattleData.GridTypes)
            {
                if (kv.Value == EGridType.Obstacle)
                {
                    curObstacleMask[kv.Key] = EGridType.Obstacle; 
                }
                else if (kv.Value == EGridType.TemporaryUnit)
                {
                    curObstacleMask[kv.Key] = EGridType.Unit; 
                }
                else
                {
                    curObstacleMask[kv.Key] = EGridType.Empty; 
                }
            }
            
            foreach (var kv in  BattleUnitDatas)
            {
                if (kv.Value.UnitCamp == (unitCamp == EUnitCamp.Enemy ? EUnitCamp.Third : EUnitCamp.Enemy) || kv.Value.UnitCamp == EUnitCamp.Player1 || kv.Value.UnitCamp == EUnitCamp.Player2)
                {
                    curObstacleMask[kv.Value.GridPosIdx] =  EGridType.Unit;    
                }
            }
            
            foreach (var enemyGridPosIdx in obstacleEnemies)
            {
                curObstacleMask[enemyGridPosIdx] =  EGridType.Unit;
            }

            var playerData = RoundFightData.GamePlayData.GetPlayerData(EUnitCamp.Player1);
            curObstacleMask[playerData.BattleHero.GridPosIdx] =  EGridType.Unit; 

            //var heroCoord = GameUtility.GridPosIdxToCoord(playerData.BattleHero.GridPosIdx);
            var unitPaths = new Dictionary<int, Dictionary<int, PathState>>();
            
            var cacheBuffDatas = new Dictionary<int, BuffData>();
            foreach (var enemyKey in actionUnitIDs)
            {
                var battleUnit = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[enemyKey] as Data_BattleMonster;
                var drEnemy = GameEntry.DataTable.GetEnemy(battleUnit.MonsterID);
                var buffData = BattleBuffManager.Instance.GetBuffData(drEnemy.OwnBuffs[0]);
                cacheBuffDatas.Add(enemyKey, buffData);
            }
            
            var oriGridPosIdxs = new Dictionary<int, int>();
            foreach (var key in  actionUnitIDs)
            {
                var battleUnitData = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[key];
                oriGridPosIdxs.Add(key, battleUnitData.GridPosIdx);
            }
            
            var retGetRange = new List<int>(50);
            var retGetRange2 = new List<int>(50);
            
            actionUnitIDs.Sort((actionUnitID1, actionUnitID2) =>
            {
                var unit1 = GetUnitByID(actionUnitID1);
                var unit2 = GetUnitByID(actionUnitID2);

                return unit1.GridPosIdx - unit2.GridPosIdx;

            });


            foreach (var actionUnitID in actionUnitIDs)
            {
                var buffData = cacheBuffDatas[actionUnitID];
                var battleUnitData = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[actionUnitID] as Data_BattleMonster;
                    
                var enemyData = GameEntry.DataTable.GetEnemy(battleUnitData.MonsterID);
                
                retGetRange.Add(battleUnitData.GridPosIdx);
                var intersectList = GameUtility.GetActionGridPosIdxs(battleUnitData.GridPosIdx, enemyData.MoveType,
                    buffData.TriggerRange, retGetRange, retGetRange2, true);
                retGetRange.Clear();
                retGetRange2.Clear();

                for (int i = 0; i < intersectList.Count; i++)
                {
                    var intersectGridPosIdx = intersectList[i];

                    if (curObstacleMask[intersectGridPosIdx] == EGridType.Obstacle)
                    {
                        if (i == intersectList.Count - 1)
                        {
                            SearchPath(curObstacleMask, actionUnitID, battleUnitData.GridPosIdx,
                                playerData.BattleHero.GridPosIdx, movePaths, enemyData.MoveType.ToString().Contains("Direct8"));
                        }
                        continue;
                    }
                    
                    var runPaths = new List<int>(16);
                    
                    var realPaths =
                        BattleFightManager.Instance.GetRunPaths(battleUnitData.GridPosIdx, intersectGridPosIdx, runPaths);
                    var realTargetPosIdx = realPaths[realPaths.Count - 1];
                    
                    if (InObstacle(curObstacleMask, realPaths))
                    { 
                        if (i == intersectList.Count - 1)
                        {
                            SearchPath(curObstacleMask, actionUnitID, battleUnitData.GridPosIdx,
                                playerData.BattleHero.GridPosIdx, movePaths, enemyData.MoveType.ToString().Contains("Direct8"));
                        }
                        continue;
                    }
                    
                    battleUnitData.GridPosIdx = realTargetPosIdx;
                    RefreshUnitGridPosIdx();
                    
                    var actionGridPosIdx = GameUtility.GetActionGridPosIdx(realTargetPosIdx, buffData.TriggerRange, true);
                    if (actionGridPosIdx == -1)
                    {
                        battleUnitData.GridPosIdx = oriGridPosIdxs[actionUnitID];
                        RefreshUnitGridPosIdx();
                        if (i == intersectList.Count - 1)
                        {
                            SearchPath(curObstacleMask, actionUnitID, battleUnitData.GridPosIdx,
                                playerData.BattleHero.GridPosIdx, movePaths, enemyData.MoveType.ToString().Contains("Direct8"));
                        }
                        continue;
                    }
                    
                    curObstacleMask[oriGridPosIdxs[actionUnitID]] = EGridType.Empty;
                    curObstacleMask[realTargetPosIdx] = EGridType.Unit;
                    battleUnitData.GridPosIdx = realTargetPosIdx;
                    RefreshUnitGridPosIdx();
                    
                    movePaths.Add(actionUnitID, realPaths);
                    break;
                }
            }
            
            RefreshPropMoveDirectUseInRound();
            
            foreach (var key in  actionUnitIDs)
            {
                var battleEnemy = BattleUnitDatas[key];
                battleEnemy.GridPosIdx = oriGridPosIdxs[key];
            }
            RefreshUnitGridPosIdx();

            // foreach (var actionUnitID in actionUnitIDs)
            // {
            //     var unitPath = new Dictionary<int, PathState>();
            //     unitPaths.Add(actionUnitID, unitPath);
            //
            //     var buffData = cacheBuffDatas[actionUnitID];
            //     var battleUnitData = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[actionUnitID] as Data_BattleMonster;
            //         
            //     var enemyData = GameEntry.DataTable.GetEnemy(battleUnitData.MonsterID);
            //     
            //     retGetRange.Add(battleUnitData.GridPosIdx);
            //     var intersectList = GameUtility.GetActionGridPosIdxs(battleUnitData.GridPosIdx, enemyData.MoveType,
            //         buffData.TriggerRange, retGetRange, retGetRange2, true);
            //     retGetRange.Clear();
            //     retGetRange2.Clear();
            //
            //     for (int i = intersectList.Count - 1; i >= 0; i--)
            //     {
            //         var intersectGridPosIdx = intersectList[i];
            //
            //         if (BattleManager.Instance.BattleData.GridTypes[intersectGridPosIdx] == EGridType.Obstacle)
            //         {
            //             intersectList.RemoveAt(i);
            //         }
            //
            //     }
            //
            // }
            //
            // foreach (var key in  actionUnitIDs)
            // {
            //     var battleEnemy = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[key];
            //     battleEnemy.GridPosIdx = oriGridPosIdxs[key];
            // }
            //
            // var matchPaths = new Dictionary<int, UnitTargetCoord>();
            // var matchAllUnitTargetCoord = new UnitTargetCoord(Vector2Int.zero,  0);
            // var allUnitTargetCoord = new UnitTargetCoord(Vector2Int.zero, 0);
            //
            // var actionCount = actionUnitIDs.Count < Constant.Enemy.EnemyActionCount ? actionUnitIDs.Count : Constant.Enemy.EnemyActionCount;
            //
            // actionUnitIDs.Sort((actionUnitID1, actionUnitID2) =>
            // {
            //     var unit1 = GetUnitByID(actionUnitID1);
            //     var unit2 = GetUnitByID(actionUnitID2);
            //
            //     return unit1.GridPosIdx - unit2.GridPosIdx;
            //
            // });
            //
            //
            //
            // //从n中取m的组合
            // //var enemyActionQueues = GameUtility.GetPermutation(actionUnitIDs.Count, actionCount);
            //
            //
            // //排序中 避免反复创建
            // var count = 1;
            // var queueLength = 0;
            // foreach (var kv in unitPaths)
            // {
            //     queueLength += 1;
            //     count *= kv.Value.Count;
            // }
            // var listMask = new List<List<int>>(count);
            // for (int i = 0; i < count; i++)
            // {
            //     var subList = new List<int>(actionCount);
            //     for (int j = 0; j < actionCount; j++)
            //     {
            //         subList.Add(-1);
            //     }
            //     listMask.Add(subList);
            // }
            //
            // var newCoords = new List<int>(actionUnitIDs.Count);
            //
            //
            // var unitPathsByQueue = new List<List<int>>(actionUnitIDs.Count);
            //     
            // foreach (var actionUnitID in actionUnitIDs)
            // {
            //     var buffData = cacheBuffDatas[actionUnitID];
            //     var battleUnitData = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[actionUnitID] as Data_BattleMonster;
            //     
            //     var enemyData = GameEntry.DataTable.GetEnemy(battleUnitData.MonsterID);
            //     
            //
            //     retGetRange.Add(battleUnitData.GridPosIdx);
            //     var actionGridPosIdxs = GameUtility.GetActionGridPosIdxs(battleUnitData.GridPosIdx, enemyData.MoveType,
            //         buffData.TriggerRange, retGetRange, retGetRange2, true);
            //     retGetRange.Clear();
            //     retGetRange2.Clear();
            //     
            //     for (int i = actionGridPosIdxs.Count - 1; i >= 0; i--)
            //     {
            //         var intersectGridPosIdx = actionGridPosIdxs[i];
            //
            //         if (BattleManager.Instance.BattleData.GridTypes[intersectGridPosIdx] == EGridType.Obstacle)
            //         {
            //             actionGridPosIdxs.RemoveAt(i);
            //         }
            //
            //     }
            //     
            //     unitPathsByQueue.Add(actionGridPosIdxs);
            // }
            //
            // for (int i = 0; i < count; i++)
            // {
            //     for (int j = 0; j < listMask[i].Count; j++)
            //     {
            //         listMask[i][j] = -1;
            //     }
            // }
            //
            // //排序
            // var newUnitPathsByQueue =
            //     GameUtility.listSort(0, unitPathsByQueue, new List<int>(), ref listMask);
            //
            //
            // var index = 0;
            // foreach (var newUnitPaths in newUnitPathsByQueue)
            // {
            //     if(index >= 1)
            //         break;
            //
            //     if (newUnitPaths[0] == -1)
            //         continue;
            //
            //     var hasDuplicates = false;
            //     for (int i = 0; i < newUnitPaths.Count; i++)
            //     {
            //         for (int j = i+1; j < newUnitPaths.Count; j++)
            //         {
            //             if (i != j && newUnitPaths[i] == newUnitPaths[j])
            //             {
            //                 hasDuplicates = true;
            //                 break;
            //             }
            //         }
            //     }
            //     
            //
            //     if (hasDuplicates)
            //         continue;
            //
            //     index++;
            //
            //     allUnitTargetCoord.AttackCount = 0;
            //     allUnitTargetCoord.MoveDis = 0;
            //
            //     var isBreak = false;
            //     RefreshPropMoveDirectUseInRound();
            //     for (int i = 0; i < newUnitPaths.Count; i++)
            //     {
            //         var posIdx = newUnitPaths[i];
            //         //var enemyPath = unitPaths[actionUnitIDs[enemyActionQueue[i]]];
            //         var battleUnit = BattleUnitDatas[actionUnitIDs[i]];
            //         var buffData = cacheBuffDatas[actionUnitIDs[i]];
            //
            //         var realPaths =
            //             FightManager.Instance.GetRunPaths(battleUnit.GridPosIdx, posIdx, runPaths);
            //         var realTargetPosIdx = realPaths[realPaths.Count - 1];
            //         
            //
            //         if (InObstacle(curObstacleMask, realPaths))
            //         { 
            //             continue;
            //         }
            //
            //
            //         curObstacleMask[battleUnit.GridPosIdx] = EGridType.Empty;
            //         curObstacleMask[realTargetPosIdx] = EGridType.Unit;
            //         battleUnit.GridPosIdx = realTargetPosIdx;
            //         
            //         newCoords.Add(realTargetPosIdx);
            //
            //         allUnitTargetCoord.AttackCount += GameUtility.GetActionGridPosIdx(realTargetPosIdx, buffData.TriggerRange, true) == -1 ? 0 : 1;
            //
            //         RefreshUnitGridPosIdx();
            //         
            //         //allUnitTargetCoord.MoveDis += realPaths.Count;
            //
            //         if (allUnitTargetCoord.AttackCount >= actionCount)
            //         {
            //             isBreak = true;
            //             break;
            //         }
            //
            //     }
            //     
            //     foreach (var key in  actionUnitIDs)
            //     {
            //         var battleEnemy = BattleUnitDatas[key];
            //         battleEnemy.GridPosIdx = oriGridPosIdxs[key];
            //     }
            //     RefreshUnitGridPosIdx();
            //
            //     foreach (var kv in unitPaths)
            //     {
            //         curObstacleMask[RoundFightData.GamePlayData.BattleData.BattleUnitDatas[kv.Key].GridPosIdx] = EGridType.Unit;
            //     }
            //
            //     foreach (var newCoord in newCoords)
            //     {
            //         curObstacleMask[newCoord] = EGridType.Empty;
            //     }
            //
            //     newCoords.Clear();
            //
            //     
            //
            //     
            //     // ||
            //     //(allUnitTargetCoord.AttackCount == matchAllUnitTargetCoord.AttackCount &&
            //     // allUnitTargetCoord.MoveDis > matchAllUnitTargetCoord.MoveDis)
            //     if ((allUnitTargetCoord.AttackCount > matchAllUnitTargetCoord.AttackCount))
            //     {
            //         matchAllUnitTargetCoord.AttackCount = allUnitTargetCoord.AttackCount;
            //         matchAllUnitTargetCoord.MoveDis = allUnitTargetCoord.MoveDis;
            //
            //         matchPaths.Clear();
            //
            //         for (int i = 0; i < newUnitPaths.Count; i++)
            //         {
            //             var posIdx = newUnitPaths[i];
            //             var unitPath = unitPaths[actionUnitIDs[i]];
            //             matchPaths.Add(actionUnitIDs[i],
            //                 new UnitTargetCoord(unitPath[posIdx].Coord,
            //                     unitPath[posIdx].MoveDis));
            //             
            //         }
            //
            //     }
            //
            //     if (isBreak)
            //     {
            //         break;
            //     }
            // }
            //
            // RefreshPropMoveDirectUseInRound();
            // foreach (var kv in matchPaths)
            // {
            //     var unitData = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[kv.Key];
            //     var tmpRunPaths = new List<int>();
            //     var minFullPaths =
            //         FightManager.Instance.GetRunPaths(unitData.GridPosIdx, GameUtility.GridCoordToPosIdx(kv.Value.Coord), tmpRunPaths);
            //     
            //     movePaths.Add(kv.Key, minFullPaths);
            //
            // }


        }

        private void SearchPath(Dictionary<int, EGridType> gridTypes, int actionUnitID, int startGridPosIdx, int endGridPosIdx, Dictionary<int, List<int>> movePaths, bool isQblique)
        {
            var paths = GameUtility.GetPaths(gridTypes, startGridPosIdx, endGridPosIdx, isQblique);
            if (paths.Count > 0)
            {
                movePaths.Add(actionUnitID, paths);
            }

        }

        public void TriggerRangeSort(Data_BattleMonster enemy, BuffData buffData, EUnitCamp unitCamp, List<int> unitActionRange)
        {
            var drEnemy = GameEntry.DataTable.GetEnemy(enemy.MonsterID);
            
            for (int i = 0; i < drEnemy.AttackTargets.Count; i++)
            {
                switch (drEnemy.AttackTargets[i])
                {
                    case EAttackTarget.Hero:
                        unitActionRange.Sort((triggerGridPosIdx1, triggerGridPosIdx2) =>
                        {
                            var triggerRange1 = GameUtility.GetRange(triggerGridPosIdx1, buffData.TriggerRange, unitCamp,
                                buffData.TriggerUnitCamps, true);
                            var inRange1 = triggerRange1.Contains(HeroManager.Instance.BattleHeroData.GridPosIdx) ? 1 : 0;
                            
                            var triggerRange2 = GameUtility.GetRange(triggerGridPosIdx2, buffData.TriggerRange, unitCamp,
                                buffData.TriggerUnitCamps, true);
                            var inRange2 = triggerRange2.Contains(HeroManager.Instance.BattleHeroData.GridPosIdx) ? 1 : 0;
                            
                            return inRange2 - inRange1;

                        });
                        break;
                    case EAttackTarget.MoreEnemy:
                        unitActionRange.Sort((triggerGridPosIdx1, triggerGridPosIdx2) =>
                        {
  
                            var triggerRange1 = GameUtility.GetRange(triggerGridPosIdx1, buffData.TriggerRange, unitCamp,
                            buffData.TriggerUnitCamps, true);
                            //var trigger1Count = triggerRange1.Count;
                            // foreach (var triggerGridPosIdx in triggerRange1)
                            // {
                            //     var unit = GameUtility.GetUnitByGridPosIdxMoreCamps(triggerGridPosIdx,true, unitCamp, buffData.TriggerUnitCamps);
                            //     if (unit != null)
                            //         trigger1Count += 1;
                            // }
                            
                            var triggerRange2 = GameUtility.GetRange(triggerGridPosIdx2, buffData.TriggerRange, unitCamp,
                                buffData.TriggerUnitCamps, true);
                            //var trigger2Count = triggerRange2.Count;
                            // foreach (var triggerGridPosIdx in triggerRange2)
                            // {
                            //     var unit = GameUtility.GetUnitByGridPosIdxMoreCamps(triggerGridPosIdx,true, unitCamp, buffData.TriggerUnitCamps);
                            //     if (unit != null)
                            //         trigger2Count += 1;
                            // }

                            return triggerRange2.Count - triggerRange1.Count;


                        });
                        break;
                    case EAttackTarget.MoreUs:
                        break;
                    case EAttackTarget.MoreUnit:
                        break;
                    case EAttackTarget.LessEnemy:
                        break;
                    case EAttackTarget.LessUs:
                        break;
                    case EAttackTarget.LessUnit:
                        break;
                    case EAttackTarget.PassMoreEnemy:
                        break;
                    case EAttackTarget.PassMoreUs:
                        break;
                    case EAttackTarget.PassMoreUnit:
                        break;
                    case EAttackTarget.FastEnemy:
                        break;
                    case EAttackTarget.FastUs:
                        break;
                    case EAttackTarget.FastUnit:
                        break;
                    case EAttackTarget.CloseEnemy:
                        break;
                    case EAttackTarget.CloseUs:
                        break;
                    case EAttackTarget.CloseUnit:
                        break;
                    case EAttackTarget.LessHPEnemy:
                        break;
                    case EAttackTarget.LessHPUs:
                        break;
                    case EAttackTarget.LessHPUnit:
                        break;
                    default:
                        break;
                }
            }
        }
        

        // public void CalculateUnitPaths(EUnitCamp unitCamp, List<int> actionUnitIDs, List<int> obstacleEnemies, Dictionary<int, List<int>> movePaths)
        // {
        //     var curObstacleMask = new Dictionary<int, EGridType>();
        //
        //     var gridObstacles = GetGridObstacles();
        //
        //     foreach (var kv in RoundFightData.GamePlayData.BattleData.GridTypes)
        //     {
        //         if (kv.Value == EGridType.Obstacle)
        //         {
        //             curObstacleMask[kv.Key] = EGridType.Obstacle; 
        //         }
        //         else if (kv.Value == EGridType.TemporaryUnit)
        //         {
        //             curObstacleMask[kv.Key] = EGridType.Unit; 
        //         }
        //         else
        //         {
        //             curObstacleMask[kv.Key] = EGridType.Empty; 
        //         }
        //     }
        //
        //     foreach (var kv in  RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
        //     {
        //         if (kv.Value.UnitCamp == (unitCamp == EUnitCamp.Enemy ? EUnitCamp.Third : EUnitCamp.Enemy))
        //         {
        //             curObstacleMask[kv.Value.GridPosIdx] =  EGridType.Unit;    
        //         }
        //     }
        //     
        //     foreach (var kv in  RoundFightData.GamePlayData.BattleData.GridPropDatas)
        //     {
        //         var drGridProp = GameEntry.DataTable.GetGridProp(kv.Value.GridPropID);
        //         // if (drGridProp.GridType == EGridType.Obstacle)
        //         // {
        //         //     curObstacleMask[kv.Value.GridPosIdx] = EGridType.Obstacle;    
        //         // }
        //     }
        //     
        //     foreach (var enemyGridPosIdx in obstacleEnemies)
        //     {
        //         curObstacleMask[enemyGridPosIdx] =  EGridType.Unit;
        //     }
        //
        //     var playerData = RoundFightData.GamePlayData.GetPlayerData(EUnitCamp.Player1);
        //     curObstacleMask[playerData.BattleHero.GridPosIdx] =  EGridType.Unit; 
        //
        //     var heroCoord = GameUtility.GridPosIdxToCoord(playerData.BattleHero.GridPosIdx);
        //     var unitPaths = new Dictionary<int, Dictionary<int, PathState>>();
        //     
        //     var cacheBuffDatas = new Dictionary<int, BuffData>();
        //     foreach (var enemyKey in actionUnitIDs)
        //     {
        //         var battleUnit = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[enemyKey] as Data_BattleMonster;
        //         var drEnemy = GameEntry.DataTable.GetEnemy(battleUnit.MonsterID);
        //         var buffData = BattleBuffManager.Instance.GetBuffData(drEnemy.OwnBuffs[0]);
        //         cacheBuffDatas.Add(enemyKey, buffData);
        //     }
        //     
        //     var oriGridPosIdxs = new Dictionary<int, int>();
        //     foreach (var key in  actionUnitIDs)
        //     {
        //         var battleUnitData = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[key];
        //         oriGridPosIdxs.Add(key, battleUnitData.GridPosIdx);
        //
        //     }
        //
        //     foreach (var key in actionUnitIDs)
        //     {
        //         var battleUnit = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[key] as Data_BattleMonster;
        //         
        //         var pathStates = new Dictionary<int, PathState>();
        //         unitPaths.Add(key, pathStates);
        //             
        //         var enemyCoord = GameUtility.GridPosIdxToCoord(battleUnit.GridPosIdx);
        //         var drEnemy = GameEntry.DataTable.GetEnemy(battleUnit.MonsterID);
        //         
        //         var unitActionRange =
        //             GameUtility.GetRange(battleUnit.GridPosIdx,
        //                 drEnemy.MoveType, null, null,  true);
        //         foreach (var actionGridPosIdx in unitActionRange)
        //         {
        //             battleUnit.GridPosIdx = actionGridPosIdx;
        //             var unitActionCoord = GameUtility.GridPosIdxToCoord(actionGridPosIdx);
        //
        //             var drBuff = cacheBuffDatas[key];
        //             
        //             var range = GameUtility.GetRange(actionGridPosIdx, drBuff.TriggerRange, unitCamp,
        //                 drBuff.TriggerUnitCamps, true);
        //             var inRange = range.Contains(BattleHeroManager.Instance.BattleHeroData.GridPosIdx);
        //             if(!inRange && battleUnit.GridPosIdx != actionGridPosIdx)
        //                 continue;
        //
        //             if (gridObstacles.Contains(actionGridPosIdx) && battleUnit.GridPosIdx != actionGridPosIdx)
        //                 continue;
        //     
        //             if (unitActionCoord == heroCoord)
        //                 continue;
        //                 
        //             var enemyMoveDis = GameUtility.GetDis(unitActionCoord, enemyCoord);
        //             pathStates.Add(GameUtility.GridCoordToPosIdx(unitActionCoord),
        //                 new PathState(unitActionCoord, 0, enemyMoveDis));
        //         }
        //             
        //     }
        //     
        //     foreach (var key in  actionUnitIDs)
        //     {
        //         var battleEnemy = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[key];
        //         battleEnemy.GridPosIdx = oriGridPosIdxs[key];
        //     }
        //     
        //     foreach (var actionUnitID in actionUnitIDs)
        //     {
        //         var unitPath = unitPaths[actionUnitID];
        //         var battleUnit = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[actionUnitID];
        //         var buffData = cacheBuffDatas[actionUnitID];
        //
        //         var maxUnitTargetCoord = new unitTargetCoord();
        //         maxUnitTargetCoord.GridPosIdx = battleUnit.GridPosIdx;
        //         foreach (var path in unitPath)
        //         {
        //             var posIdx = path.Key;
        //
        //             var realPaths =
        //                 FightManager.Instance.GetRunPaths(battleUnit.GridPosIdx, posIdx);
        //             var realTargetPosIdx = realPaths[realPaths.Count - 1];
        //
        //             if (InObstacle(curObstacleMask, realPaths))
        //             {
        //                 continue;
        //             }
        //
        //             var attackCount = GameUtility.GetAttackCount(realTargetPosIdx, buffData.TriggerRange, unitCamp,
        //                 buffData.TriggerUnitCamps, true);
        //             if (attackCount > maxUnitTargetCoord.AttackCount)
        //             {
        //                 maxUnitTargetCoord.AttackCount = attackCount;
        //                 maxUnitTargetCoord.GridPosIdx = realTargetPosIdx;
        //                 if (movePaths.ContainsKey(actionUnitID))
        //                 {
        //                     movePaths[actionUnitID] = new List<int>(realPaths);
        //                 }
        //                 else
        //                 {
        //                     movePaths.Add(actionUnitID, new List<int>(realPaths));
        //                 }
        //             }
        //
        //         }
        //
        //         
        //
        //         curObstacleMask[battleUnit.GridPosIdx] = EGridType.Empty;
        //         battleUnit.GridPosIdx = maxUnitTargetCoord.GridPosIdx;
        //         curObstacleMask[maxUnitTargetCoord.GridPosIdx] = EGridType.Unit;
        //
        //         
        //
        //     }
        //     
        //     foreach (var key in  actionUnitIDs)
        //     {
        //         var battleEnemy = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[key];
        //         battleEnemy.GridPosIdx = oriGridPosIdxs[key];
        //     }
        //
        //     // //从n中取m的组合
        //     // var enemyActionQueues = GameUtility.GetPermutation(actionUnitIDs.Count, actionUnitIDs.Count);
        //     //
        //     // foreach (var enemyActionQueue in enemyActionQueues)
        //     // {
        //     //
        //     //     foreach (var queue in enemyActionQueue)
        //     //     {
        //     //         var actionUnitID = actionUnitIDs[queue];
        //     //         var unitPath = unitPaths[actionUnitID];
        //     //         var battleUnit = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[actionUnitID];
        //     //         var drBuff = cacheDrBuffs[actionUnitID];
        //     //
        //     //         var maxUnitTargetCoord = new unitTargetCoord();
        //     //         maxUnitTargetCoord.GridPosIdx = battleUnit.GridPosIdx;
        //     //         foreach (var path in unitPath)
        //     //         {
        //     //             var posIdx = path.Key;
        //     //
        //     //             var realPaths =
        //     //                 FightManager.Instance.GetRunPaths(battleUnit.GridPosIdx, posIdx);
        //     //             var realTargetPosIdx = realPaths[realPaths.Count - 1];
        //     //
        //     //
        //     //             if (InObstacle(curObstacleMask, realPaths))
        //     //             {
        //     //                 continue;
        //     //             }
        //     //
        //     //             var attackCount = GameUtility.GetAttackCount(realTargetPosIdx, drBuff.TriggerRange, unitCamp,
        //     //                 drBuff.TriggerUnitCamps, drBuff.HeroInRangeTrigger, drBuff.TriggerRange == EActionType.Self,
        //     //                 true);
        //     //             if (attackCount > maxUnitTargetCoord.AttackCount)
        //     //             {
        //     //                 maxUnitTargetCoord.AttackCount = attackCount;
        //     //                 maxUnitTargetCoord.GridPosIdx = realTargetPosIdx;
        //     //                 if (movePaths.ContainsKey(actionUnitID))
        //     //                 {
        //     //                     movePaths[actionUnitID] = new List<int>(realPaths);
        //     //                 }
        //     //                 else
        //     //                 {
        //     //                     movePaths.Add(actionUnitID, new List<int>(realPaths));
        //     //                 }
        //     //             }
        //     //
        //     //         }
        //     //
        //     //         
        //     //
        //     //         curObstacleMask[battleUnit.GridPosIdx] = EGridType.Empty;
        //     //         battleUnit.GridPosIdx = maxUnitTargetCoord.GridPosIdx;
        //     //         curObstacleMask[maxUnitTargetCoord.GridPosIdx] = EGridType.Unit;
        //     //
        //     //
        //     //     }
        //     //     
        //     //     foreach (var key in  actionUnitIDs)
        //     //     {
        //     //         var battleEnemy = RoundFightData.GamePlayData.BattleData.BattleUnitDatas[key];
        //     //         battleEnemy.GridPosIdx = oriGridPosIdxs[key];
        //     //     }
        //     // }
        //
        // }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RPGCharacterAnims.Actions;
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
        public int ActionUnitIdx;
        public int EffectGridPosIdx;
        public int UnitIdx;

        public EUnitActionState UnitActionState = EUnitActionState.Empty;

        //public int TargetGridPosIdx;
        public MoveActionData MoveActionData;

        public bool IsTrigger;
        //public EFlyType FlyType = EFlyType.Empty;

        public MoveUnitData Copy()
        {
            var moveUnitData = new MoveUnitData();
            moveUnitData.ActionUnitIdx = ActionUnitIdx;
            moveUnitData.EffectGridPosIdx = EffectGridPosIdx;
            moveUnitData.UnitIdx = UnitIdx;
            moveUnitData.UnitActionState = UnitActionState;
            //moveUnitData.FlyType = FlyType;
            moveUnitData.MoveActionData = MoveActionData.Copy();
            moveUnitData.IsTrigger = IsTrigger;

            return moveUnitData;
        }

    }

    public class MoveData
    {

        public Dictionary<int, MoveUnitData> MoveUnitDatas = new Dictionary<int, MoveUnitData>();

        public MoveData Copy()
        {
            var _moveData = new MoveData();
            foreach (var kv in MoveUnitDatas)
            {
                _moveData.MoveUnitDatas.Add(kv.Key, kv.Value.Copy());
            }

            return _moveData;
        }
        
        public void Clear()
        {
            MoveUnitDatas.Clear();
        }

    }
    
    public class PassCardData
    {
        public CardCirculation AcquireCardCirculation = new();
        public List<TriggerData> PassCardDatas = new();
        
        public void Clear()
        {
            AcquireCardCirculation.Clear();
            PassCardDatas.Clear();
        }
    }
    
    public class CardActionData : ActionData
    {
        public int CardIdx;
        public int CardEnergy;
        public ECardDestination CardDestination;
        public Dictionary<int, List<TriggerData>> ConsumeCardDatas = new();
        public Dictionary<int, List<TriggerData>> AcquireCardDatas = new();
        public CardCirculation UseCardCirculation = new();
        public override void Clear()
        {
            base.Clear();
            ConsumeCardDatas.Clear();
        }
    }

    public class TriggerCollection
    {
        public int ActionUnitIdx;
        public int EffectTagIdx;
        public List<TriggerData> TriggerDatas = new List<TriggerData>();
            
        public MoveData MoveData = new MoveData();
        
        public bool IsTrigger;

        public TriggerCollection Copy()
        {
            var _triggerCollection = new TriggerCollection();
            foreach (var triggerData in TriggerDatas)
            {
                _triggerCollection.TriggerDatas.Add(triggerData.Copy());
            }

            _triggerCollection.MoveData = MoveData.Copy();
            
            _triggerCollection.ActionUnitIdx = ActionUnitIdx;
            _triggerCollection.EffectTagIdx = EffectTagIdx;
            _triggerCollection.IsTrigger = IsTrigger;
            
            return _triggerCollection;
        }

        public TriggerData GetNormalTriggerData()
        {
            foreach (var triggerData in TriggerDatas)
            {
                if (triggerData.BuffValue != null &&
                    triggerData.BuffValue.BuffData.BuffEquipType == EBuffEquipType.Normal)
                {
                    return triggerData;
                }
            }

            return null;
        }
        
        public void Clear()
        {
            TriggerDatas.Clear();
            MoveData.Clear();
        }
    }

    public class ActionData
    {
        public int ActionUnitIdx;
        public EActionDataType ActionDataType = EActionDataType.Unit;
        public Dictionary<int, TriggerCollection> TriggerDataDict = new();
        //public MoveData MoveData = new MoveData();

        public void AddEmptyTriggerDataList(int id)
        {
            if (!TriggerDataDict.ContainsKey(id))
            {
                TriggerDataDict.Add(id, new TriggerCollection());

            }
        }

        public void AddTriggerData(int id, TriggerData triggerData, Data_BattleUnit effectUnit)
        {
            if (triggerData == null)
                return;

            if (!TriggerDataDict.ContainsKey(id))
            {
                TriggerDataDict.Add(id, new TriggerCollection());

            }

            TriggerDataDict[id].TriggerDatas.Add(triggerData);

            if (triggerData.TriggerDataType == ETriggerDataType.Atrb &&
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

        public virtual void Clear()
        {
            TriggerDataDict.Clear();
            //MoveData.Clear();
        }
    }

    public class MoveActionData
    {
        public int MoveUnitIdx;
        public List<int> MoveGridPosIdxs = new();
        public Dictionary<int, TriggerCollection> TriggerDataDict = new();

        // public int InterrelatedActionUnitIdx;
        // public int InterrelatedEffectUnitIdx;
        
        public MoveActionData Copy()
        {
            var moveActionData = new MoveActionData();
            moveActionData.MoveUnitIdx = MoveUnitIdx;
            moveActionData.MoveGridPosIdxs = new List<int>(MoveGridPosIdxs);


            foreach (var kv in TriggerDataDict)
            {
                moveActionData.TriggerDataDict.Add(kv.Key, kv.Value.Copy());
            }

            return moveActionData;
        }

        public void Clear()
        {
            MoveGridPosIdxs.Clear();
            TriggerDataDict.Clear();
        }
    }

    public class TriggerData
    {
        public int OwnUnitIdx = -1;
        public int ActionUnitIdx = -1;
        public int EffectUnitIdx = -1;
        public EUnitAttribute BattleUnitAttribute = EUnitAttribute.Empty;

        public EHeroAttribute HeroAttribute = EHeroAttribute.Empty;

        //public EUnitState UnitState = EUnitState.Empty;
        public ELinkID LinkID = ELinkID.Empty;

        public BuffValue BuffValue = null;
        public ECardTriggerType CardTriggerType = ECardTriggerType.Empty;
        public ETriggerDataSubType TriggerDataSubType = ETriggerDataSubType.Empty;
        public ETriggerDataType TriggerDataType = ETriggerDataType.Empty;
        public float Value;
        public UnitStateDetail UnitStateDetail = new UnitStateDetail();
        public float DeltaValue;
        public float ActualValue;
        public ETriggerResult TriggerResult = ETriggerResult.Continue;
        public EBuffTriggerType BuffTriggerType = EBuffTriggerType.Empty;
        public List<EUnitStateEffectType> UnitStateEffectTypes = new List<EUnitStateEffectType>();
        public int TriggerBlessIdx = -1;
        public int TriggerFuneIdx = -1;
        public int TriggerCardIdx = -1;
        //public int CardIdx;

        public bool ChangeHPInstantly = true;

        public int CoreHPDelta = 0;

        public int ActionUnitGridPosIdx = -1;
        public int EffectUnitGridPosIdx = -1;
        public bool IsTrigger = false;
        public CardCirculation AcquireCardCirculation = new();

        public int Idx;

        public static int generateIdx;
        public int InterrelatedActionUnitIdx;
        public int InterrelatedEffectUnitIdx;
            
        //public bool AddHeroHP = true;

        public TriggerData()
        {
            Idx = generateIdx++;
        }

        public TriggerData Copy()
        {
            var triggerData = new TriggerData();
            triggerData.OwnUnitIdx = OwnUnitIdx;
            triggerData.ActionUnitIdx = ActionUnitIdx;
            triggerData.EffectUnitIdx = EffectUnitIdx;
            triggerData.ActionUnitGridPosIdx = ActionUnitGridPosIdx;
            triggerData.EffectUnitGridPosIdx = EffectUnitGridPosIdx;
            triggerData.BattleUnitAttribute = BattleUnitAttribute;
            triggerData.HeroAttribute = HeroAttribute;
            triggerData.LinkID = LinkID;
            triggerData.BuffValue = BuffValue?.Copy();
            //triggerData.BuffID = BuffID;
            triggerData.CardTriggerType = CardTriggerType;
            triggerData.TriggerDataType = TriggerDataType;
            triggerData.TriggerDataSubType = TriggerDataSubType;
            triggerData.Value = Value;
            triggerData.DeltaValue = DeltaValue;
            triggerData.TriggerResult = TriggerResult;
            triggerData.ChangeHPInstantly = ChangeHPInstantly;
            triggerData.BuffTriggerType = BuffTriggerType;
            triggerData.CoreHPDelta = CoreHPDelta;
            triggerData.IsTrigger = IsTrigger;
            triggerData.UnitStateDetail = UnitStateDetail.Copy();
            triggerData.UnitStateEffectTypes = new List<EUnitStateEffectType>(UnitStateEffectTypes);
            triggerData.ActualValue = ActualValue;
            triggerData.TriggerBlessIdx = TriggerBlessIdx;
            triggerData.TriggerFuneIdx = TriggerFuneIdx;
            triggerData.TriggerCardIdx = TriggerCardIdx;
            triggerData.AcquireCardCirculation = AcquireCardCirculation.Copy();
            
            triggerData.Idx = Idx;
            triggerData.InterrelatedActionUnitIdx = InterrelatedActionUnitIdx;
            triggerData.InterrelatedEffectUnitIdx = InterrelatedEffectUnitIdx;

            
            return triggerData;
        }
    }

    public class CardCirculation
    {
        public List<int> PassCards = new();
        public List<int> HandCards = new();
        public List<int> StandByCards = new();
        public List<int> ConsumeCards = new();
        
        public Dictionary<int, List<TriggerData>> TriggerDatas = new();

        public CardCirculation Copy()
        {
            var cardCirculation = new CardCirculation();
            cardCirculation.PassCards = new List<int>(PassCards);
            cardCirculation.HandCards = new List<int>(HandCards);
            cardCirculation.StandByCards = new List<int>(StandByCards);
            cardCirculation.ConsumeCards = new List<int>(ConsumeCards);

            foreach (var kv in TriggerDatas)
            {
                var triggerDatas = new List<TriggerData>();
                foreach (var triggerData in kv.Value)
                {
                    triggerDatas.Add(triggerData.Copy());
                }
                
                cardCirculation.TriggerDatas.Add(kv.Key, triggerDatas);
            }
            
            return cardCirculation;
        }
        
        public void Clear()
        {
            PassCards.Clear();
            HandCards.Clear();
            StandByCards.Clear();
            ConsumeCards.Clear();
        }
    }

    public class RoundFightData
    {
        public Dictionary<int, ActionData> UseCardTriggerDatas = new();
        public Dictionary<int, ActionData> RoundStartBuffDatas = new();
        public Dictionary<int, ActionData> RoundStartUnitDatas = new();
        public Dictionary<int, ActionData> PreRoundStartDatas = new();
        public Dictionary<int, ActionData> BlessTriggerDatas = new();
        public Dictionary<int, ActionData> CurseTriggerDatas = new();
        public Dictionary<int, ActionData> RoundEndDatas = new();
        public Dictionary<int, ActionData> SoliderAttackDatas = new();
        public Dictionary<int, ActionData> SoliderActiveAttackDatas = new();
        public Dictionary<int, ActionData> ThirdUnitAttackDatas = new();
        public Dictionary<int, MoveActionData> EnemyMoveDatas = new();
        public Dictionary<int, MoveActionData> ThirdUnitMoveDatas = new();
        public Dictionary<int, MoveActionData> SoliderMoveDatas = new();
        public Dictionary<int, ActionData> EnemyAttackDatas = new();

        //public List<TriggerData> UseCardDatas = new();
        public CardActionData BuffData_Use = new();

        public Data_GamePlay GamePlayData;
        public Dictionary<int, List<int>> EnemyMovePaths = new();
        public Dictionary<int, List<int>> ThirdUnitMovePaths = new();

        public TempTriggerData TempTriggerData;

        public Dictionary<EUnitCamp, List<HPDeltaData>> HPDeltaDict = new Dictionary<EUnitCamp, List<HPDeltaData>>();

        public CardCirculation RoundPassCardCirculation = new();
        
        public CardCirculation RoundAcquireCardCirculation = new();

        public PassCardData PassCardData = new();
        
        public void Clear()
        {
            RoundStartBuffDatas.Clear();
            RoundStartUnitDatas.Clear();
            PreRoundStartDatas.Clear();
            RoundEndDatas.Clear();
            SoliderAttackDatas.Clear();
            SoliderActiveAttackDatas.Clear();
            EnemyMoveDatas.Clear();
            ThirdUnitMoveDatas.Clear();
            SoliderMoveDatas.Clear();
            EnemyMoveDatas.Clear();
            EnemyAttackDatas.Clear();
            EnemyMovePaths.Clear();
            ThirdUnitMovePaths.Clear();
            ThirdUnitAttackDatas.Clear();
            BuffData_Use.Clear();
            BlessTriggerDatas.Clear();
            UseCardTriggerDatas.Clear();
            HPDeltaDict.Clear();
            PassCardData.Clear();
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

    public partial class BattleFightManager : Singleton<BattleFightManager>
    {
        public RoundFightData RoundFightData = new();

        private Dictionary<int, List<UnitActionRange>> UnitActionRange = new();

        // private Dictionary<int, Data_BattleSolider> Soliders;
        // private Dictionary<int, Data_BattleMonster> Enemies;

        public Dictionary<int, Data_BattleUnit> BattleUnitDatas = new Dictionary<int, Data_BattleUnit>();
        public Dictionary<int, Data_BattleUnit> BattleUnitDatasByGridPosIdx = new Dictionary<int, Data_BattleUnit>();

        private Dictionary<int, Data_BattleUnit> Roles;
        public EActionProgress ActionProgress;
        public bool IsAction;
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
            AcitonUnitIdx = 0;
            RoundFightData.Clear();
        }

        public Data_Player PlayerData;

        


        

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
                    if (battlePlayerData != null && battlePlayerData.BattleBuffs.Contains(EBuffID.Spec_AtkPassEnemyAddDmg_AtkPassUsAddDmg))
                    {
                        value += value < 0 ? 1 : 0;
                    }

                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(bePassUnit.Idx,
                        bePassUnit.Idx, passUnit.Idx,
                        EUnitAttribute.HP, value, ETriggerDataSubType.Unit);
                    triggerData.ChangeHPInstantly = false;
                    triggerData.ActionUnitGridPosIdx = triggerData.EffectUnitGridPosIdx = bePassUnit.GridPosIdx;
                    triggerData.UnitStateDetail.UnitState = EUnitState.AtkPassUs;
                    //bePassUnit.RemoveState(EUnitState.AtkPassUs);

                    BattleBuffManager.Instance.CacheTriggerData(triggerData, triggerDatas);

                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        BattleBuffManager.Instance.AttackTrigger(triggerData, triggerDatas);
                        BattleUnitStateManager.Instance.CheckUnitState(bePassUnit.Idx, triggerDatas);
                    }

                    moveUnitStateDatas.Add(new MoveUnitStateData(EUnitState.AtkPassUs, true));
                    
                    var actualBePassUnitAttackPassUs = bePassUnitAttackPassUs;
                    if (bePassUnit.FuneCount(EBuffID.Spec_UnitStateSubOne) > 0  && actualBePassUnitAttackPassUs > 1)
                    {
                        actualBePassUnitAttackPassUs = 1;
                    }
                    
                    var subAtkPassUsData = BattleFightManager.Instance.Unit_State(triggerDatas, bePassUnit.Idx,
                        bePassUnit.Idx, bePassUnit.Idx, EUnitState.AtkPassUs, -actualBePassUnitAttackPassUs,
                        ETriggerDataType.State);
                    subAtkPassUsData.ActionUnitGridPosIdx =
                        subAtkPassUsData.EffectUnitGridPosIdx = passUnit.GridPosIdx;
                    BattleBuffManager.Instance.CacheTriggerData(subAtkPassUsData, triggerDatas);

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
                    if (battlePlayerData != null && battlePlayerData.BattleBuffs.Contains(EBuffID.Spec_AtkPassEnemyAddDmg_AtkPassUsAddDmg))
                    {
                        value += value < 0 ? 1 : 0;
                    }

                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(passUnit.Idx,
                        passUnit.Idx, bePassUnit.Idx,
                        EUnitAttribute.HP, value, ETriggerDataSubType.Unit);
                    triggerData.ChangeHPInstantly = false;
                    triggerData.ActionUnitGridPosIdx = triggerData.EffectUnitGridPosIdx = bePassUnit.GridPosIdx;
                    triggerData.UnitStateDetail.UnitState = EUnitState.AtkPassUs;
                    //passUnit.RemoveState(EUnitState.AtkPassUs);

                    BattleBuffManager.Instance.CacheTriggerData(triggerData, triggerDatas);

                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        BattleBuffManager.Instance.AttackTrigger(triggerData, triggerDatas);
                        BattleUnitStateManager.Instance.CheckUnitState(passUnit.Idx, triggerDatas);
                    }

                    moveUnitStateDatas.Add(new MoveUnitStateData(EUnitState.AtkPassUs, false));
                    
                    var actualPassUnitAttackPassUs = passUnitAttackPassUs;
                    if (passUnit.FuneCount(EBuffID.Spec_UnitStateSubOne) > 0  && actualPassUnitAttackPassUs > 1)
                    {
                        actualPassUnitAttackPassUs = 1;
                    }
                    
                    var subAtkPassUsData = BattleFightManager.Instance.Unit_State(triggerDatas, bePassUnit.Idx,
                        bePassUnit.Idx, passUnit.Idx, EUnitState.AtkPassUs, -actualPassUnitAttackPassUs,
                        ETriggerDataType.State);
                    subAtkPassUsData.ActionUnitGridPosIdx =
                        subAtkPassUsData.EffectUnitGridPosIdx = passUnit.GridPosIdx;
                    BattleBuffManager.Instance.CacheTriggerData(subAtkPassUsData, triggerDatas);

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
                    if (battlePlayerData != null && battlePlayerData.BattleBuffs.Contains(EBuffID.Spec_AtkPassEnemyAddDmg_AtkPassUsAddDmg))
                    {
                        value += value < 0 ? -1 : 0;
                    }

                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(bePassUnit.Idx, bePassUnit.Idx,
                        passUnit.Idx, EUnitAttribute.HP, value, ETriggerDataSubType.Unit);
                    triggerData.ChangeHPInstantly = false;
                    triggerData.ActionUnitGridPosIdx = triggerData.EffectUnitGridPosIdx = bePassUnit.GridPosIdx;
                    triggerData.UnitStateDetail.UnitState = EUnitState.AtkPassEnemy;
                    //bePassUnit.RemoveState(EUnitState.AtkPassEnemy);

                    BattleBuffManager.Instance.CacheTriggerData(triggerData, triggerDatas);
    
                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        BattleBuffManager.Instance.AttackTrigger(triggerData, triggerDatas);
                        BattleUnitStateManager.Instance.CheckUnitState(bePassUnit.Idx, triggerDatas);
                    }

                    moveUnitStateDatas.Add(new MoveUnitStateData(EUnitState.AtkPassEnemy, true));
                    
                    var actualBePassUnitAttackPassEnemy = bePassUnitAttackPassEnemy;
                    if (bePassUnit.FuneCount(EBuffID.Spec_UnitStateSubOne) > 0  && actualBePassUnitAttackPassEnemy > 1)
                    {
                        actualBePassUnitAttackPassEnemy = 1;
                    }
                    
                    var subAtkPassEnemyData = BattleFightManager.Instance.Unit_State(triggerDatas, bePassUnit.Idx,
                        bePassUnit.Idx, bePassUnit.Idx, EUnitState.AtkPassEnemy, -actualBePassUnitAttackPassEnemy,
                        ETriggerDataType.State);
                    subAtkPassEnemyData.ActionUnitGridPosIdx =
                        subAtkPassEnemyData.EffectUnitGridPosIdx = passUnit.GridPosIdx;
                    BattleBuffManager.Instance.CacheTriggerData(subAtkPassEnemyData, triggerDatas);

                }
            }

            if (passUnitAttackPassEnemy > 0 && passUnit.UnitCamp != bePassUnit.UnitCamp)
            {
                if (!ContainsMoveUnitStateData(preMoveUnitStateDatas, EUnitState.AtkPassEnemy, false))
                {
                    var value = -passUnitAttackPassEnemy;
                    var battlePlayerData =
                        BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(
                            passUnit.UnitCamp);
                    if (battlePlayerData != null && battlePlayerData.BattleBuffs.Contains(EBuffID.Spec_AtkPassEnemyAddDmg_AtkPassUsAddDmg))
                    {
                        value += value < 0 ? -1 : 0;
                    }

                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(passUnit.Idx, passUnit.Idx,
                        bePassUnit.Idx, EUnitAttribute.HP, value, ETriggerDataSubType.Unit);
                    triggerData.ChangeHPInstantly = false;
                    triggerData.ActionUnitGridPosIdx = triggerData.EffectUnitGridPosIdx = bePassUnit.GridPosIdx;
                    triggerData.UnitStateDetail.UnitState = EUnitState.AtkPassEnemy;
                    //passUnit.RemoveState(EUnitState.AtkPassEnemy);

                    BattleBuffManager.Instance.CacheTriggerData(triggerData, triggerDatas);
                    if (GameUtility.IsSubCurHPTrigger(triggerData))
                    {
                        BattleBuffManager.Instance.AttackTrigger(triggerData, triggerDatas);
                        BattleUnitStateManager.Instance.CheckUnitState(passUnit.Idx, triggerDatas);
                    }

                    moveUnitStateDatas.Add(new MoveUnitStateData(EUnitState.AtkPassEnemy, false));
                    
                    var actualPassUnitAttackPassEnemy = passUnitAttackPassEnemy;
                    if (passUnit.FuneCount(EBuffID.Spec_UnitStateSubOne) > 0  && actualPassUnitAttackPassEnemy > 1)
                    {
                        actualPassUnitAttackPassEnemy = 1;
                    }

                    var subAtkPassEnemyData = BattleFightManager.Instance.Unit_State(triggerDatas, passUnit.Idx,
                        passUnit.Idx, passUnit.Idx, EUnitState.AtkPassEnemy, -actualPassUnitAttackPassEnemy,
                        ETriggerDataType.State);
                    subAtkPassEnemyData.ActionUnitGridPosIdx =
                        subAtkPassEnemyData.EffectUnitGridPosIdx = passUnit.GridPosIdx;
                    BattleBuffManager.Instance.CacheTriggerData(subAtkPassEnemyData, triggerDatas);
                }
            }

            return moveUnitStateDatas;
        }


        
        public void TriggerUnitData(int triggerUnitID, int effectUnitID, int gridPosIdx, EBuffTriggerType buffTriggerType,
            List<TriggerData> triggerDatas)
        {
            var triggerUnits = GetTriggerUnits(gridPosIdx);

            if (triggerUnits != null)
            {
                foreach (var soliderActionRange in triggerUnits)
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


        
        private void CurHPTriggerData(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            Data_BattleUnit effectUnitData;
            if (triggerData.EffectUnitIdx == BattleFightManager.Instance.PlayerData.BattleHero.Idx)
            {
                effectUnitData = BattleFightManager.Instance.PlayerData.BattleHero;
            }
            else
            {
                effectUnitData = GetUnitByIdx(triggerData.EffectUnitIdx);
            }
            
            if (effectUnitData == null)
                return;
            
            var actionUnitData = GetUnitByIdx(triggerData.ActionUnitIdx);

            var effectUnitOldHP = effectUnitData.CurHP;

            var triggerValue = triggerData.Value + triggerData.DeltaValue;
            var subDmgCount = 0;
            if (actionUnitData != null)
            {
                subDmgCount = actionUnitData.GetAllStateCount(EUnitState.SubDmg);
            }

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
                    if (triggerData.OwnUnitIdx == Constant.Battle.UnUnitTriggerIdx || (triggerData.OwnUnitIdx > 0 &&
                            triggerData.OwnUnitIdx == triggerData.ActionUnitIdx))
                    {
                        //var ownUnit = GetUnitByIdx(triggerData.OwnUnitIdx);
                        //ownUnit != null && 
                        if (triggerData.UnitStateDetail.UnitState == EUnitState.Empty)
                        {
                            BattleGridPropManager.Instance.TriggerStayPropState(triggerData.EffectUnitGridPosIdx,
                                effectUnitData, EUnitState.HurtAddDmg);

                            if (actionUnitData != null)
                            {
                                BattleGridPropManager.Instance.TriggerStayPropState(triggerData.ActionUnitGridPosIdx,
                                    actionUnitData, EUnitState.SubDmg);
                            }

                            if (actionUnitData != null)
                            {
                                BattleGridPropManager.Instance.TriggerStayPropState(triggerData.ActionUnitGridPosIdx,
                                    actionUnitData, EUnitState.AddDmg);
                            }

                            BattleGridPropManager.Instance.TriggerStayPropState(triggerData.EffectUnitGridPosIdx,
                                effectUnitData, EUnitState.HurtSubDmg);
                            
                            
                            var hurtSubDmgCount = effectUnitData.GetAllStateCount(EUnitState.HurtSubDmg);
                            var hurtAddDmgCount = effectUnitData.GetAllStateCount(EUnitState.HurtAddDmg);
                            
                            //var addDmgCount = actionUnitData.GetAllStateCount(EUnitState.AddDmg);
                            
                            

                            var atkAddSelfHP = 0;
                            if (actionUnitData != null)
                            {
                                atkAddSelfHP = actionUnitData.GetAllStateCount(EUnitState.AtkAddSelfHP);
                            }
                            
                            var addDmgCount = 0;
                            if (actionUnitData != null)
                            {
                                addDmgCount = actionUnitData.GetAllStateCount(EUnitState.AddDmg);
                            }
                            
                            triggerData.DeltaValue += -addDmgCount;
                            
                            if (!GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                            {
                                triggerData.DeltaValue += subDmgCount;
                            }

                            if (!GameUtility.ContainRoundState(RoundFightData.GamePlayData, EBuffID.Spec_CurseUnEffect))
                            {
                                triggerData.DeltaValue += -hurtAddDmgCount;
                            }

                            if (actionUnitData != null)
                            {
                                triggerData.DeltaValue += -actionUnitData.FuneCount(EBuffID.Spec_AddBaseDmg);
                            }

                            if (hurtAddDmgCount > 0)
                            {
                                var actualHurtAddDmgCount = hurtAddDmgCount;
                                if (effectUnitData.FuneCount(EBuffID.Spec_UnitStateSubOne) > 0  && actualHurtAddDmgCount > 1)
                                {
                                    actualHurtAddDmgCount = 1;
                                }
                                
                                var subHurtAddDmgData = BattleFightManager.Instance.Unit_State(triggerDatas, triggerData.OwnUnitIdx,
                                    triggerData.ActionUnitIdx, effectUnitData.Idx, EUnitState.HurtAddDmg, -actualHurtAddDmgCount,
                                    ETriggerDataType.State);
                                subHurtAddDmgData.ActionUnitGridPosIdx =
                                    subHurtAddDmgData.EffectUnitGridPosIdx = effectUnitData.GridPosIdx;
                                SimulateTriggerData(subHurtAddDmgData, triggerDatas);
                                triggerDatas.Add(subHurtAddDmgData);
                            }
                            
                            // if (addDmgCount > 0)
                            // {
                            //     var actualAddDmgCount = addDmgCount;
                            //     if (actionUnitData.FuneCount(EBuffID.Spec_UnitStateSubOne) > 0  && actualAddDmgCount > 1)
                            //     {
                            //         actualAddDmgCount = 1;
                            //     }
                            //     
                            //     
                            //     var subAddDmgCountData = BattleFightManager.Instance.Unit_State(triggerDatas, actionUnitData.Idx,
                            //         actionUnitData.Idx, actionUnitData.Idx, EUnitState.AddDmg, -actualAddDmgCount,
                            //         ETriggerDataType.RoleState);
                            //     subAddDmgCountData.ActionUnitGridPosIdx =
                            //         subAddDmgCountData.EffectUnitGridPosIdx = actionUnitData.GridPosIdx;
                            //     SimulateTriggerData(subAddDmgCountData, triggerDatas);
                            //     triggerDatas.Add(subAddDmgCountData);
                            // }

                            if (actionUnitData != null && atkAddSelfHP > 0)
                            {
                        
                                var _triggerValue = (int)(triggerData.Value + triggerData.DeltaValue);
                                _triggerValue = _triggerValue > 0 ? 0 : _triggerValue;
                                var atkAddSelfHP_hpDelta = actionUnitData.MaxHP - actionUnitData.CurHP;

                                // var maxAddHPCount = 0;
                                // if(Mathf.Abs(_triggerValue) > effectUnitData.CurHP)
                                // {
                                //     maxAddHPCount = effectUnitData.CurHP > atkAddSelfHP
                                //         ? atkAddSelfHP
                                //         : effectUnitData.CurHP;
                                // }
                                // else
                                // {
                                //     
                                //     
                                //     maxAddHPCount = effectUnitData.CurHP > atkAddSelfHP
                                //         ? atkAddSelfHP
                                //         : Mathf.Abs(_triggerValue);
                                //     
                                //     var blessData = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.SubHPAddSelfHPAddMore,
                                //         PlayerManager.Instance.PlayerData.UnitCamp);
                                //
                                //     if (blessData != null && _triggerValue + effectUnitData.CurHP == 1 &&
                                //         actionUnitData.UnitCamp == PlayerManager.Instance.PlayerData.UnitCamp)
                                //     {
                                //         triggerData.DeltaValue += -1;
                                //         maxAddHPCount += 1;
                                //     }
                                //     
                                //     
                                // }
                                
                                var maxAddHPCount = effectUnitData.CurHP > atkAddSelfHP
                                    ? atkAddSelfHP
                                    : effectUnitData.CurHP;

                                maxAddHPCount = maxAddHPCount > Mathf.Abs(_triggerValue)
                                    ? Mathf.Abs(_triggerValue)
                                    : maxAddHPCount;
                                
                                maxAddHPCount = maxAddHPCount > atkAddSelfHP_hpDelta
                                    ? atkAddSelfHP_hpDelta
                                    : maxAddHPCount;

                                maxAddHPCount = maxAddHPCount > atkAddSelfHP ? atkAddSelfHP : maxAddHPCount;
                                
                                var actualSubHPAddSelfHPCount = -maxAddHPCount;
                                
                                var blessData = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.SubHPAddSelfHPAddMore,
                                    PlayerManager.Instance.PlayerData.UnitCamp);
                                
                                if (blessData != null && _triggerValue + effectUnitData.CurHP == 1 &&
                                    actionUnitData.UnitCamp == PlayerManager.Instance.PlayerData.UnitCamp)
                                {
                                    triggerData.DeltaValue += -1;
                                    maxAddHPCount += 1;
                                }
                                
                                // var actualSubHPAddSelfHPCount = maxAddHPCount > atkAddSelfHP ? -atkAddSelfHP : -maxAddHPCount;
                                
                                
                                if (actionUnitData.FuneCount(EBuffID.Spec_UnitStateSubOne) > 0  && actualSubHPAddSelfHPCount < -1)
                                {
                                    actualSubHPAddSelfHPCount = -1;
                                }

                                if (maxAddHPCount > 0)
                                {
                                    var subHPAddSelfHPData = BattleFightManager.Instance.Unit_State(triggerDatas, actionUnitData.Idx,
                                        actionUnitData.Idx, actionUnitData.Idx, EUnitState.AtkAddSelfHP, actualSubHPAddSelfHPCount,
                                        ETriggerDataType.State);
                                    subHPAddSelfHPData.ActionUnitGridPosIdx = subHPAddSelfHPData.EffectUnitGridPosIdx =
                                        actionUnitData.GridPosIdx;
                                    SimulateTriggerData(subHPAddSelfHPData, triggerDatas);
                                    triggerDatas.Add(subHPAddSelfHPData);
                                    
                                    var addSelfHPTriggerData = BattleFightManager.Instance.BattleRoleAttribute(actionUnitData.Idx,
                                        actionUnitData.Idx, actionUnitData.Idx, EUnitAttribute.HP, maxAddHPCount,
                                        ETriggerDataSubType.Unit);
                                    addSelfHPTriggerData.ActionUnitGridPosIdx = actionUnitData.GridPosIdx;
                                    addSelfHPTriggerData.EffectUnitGridPosIdx = actionUnitData.GridPosIdx;
                                
                                    SimulateTriggerData(addSelfHPTriggerData, triggerDatas);
                                    triggerDatas.Add(addSelfHPTriggerData);
                                    
                                    
                                    var subHPAddSelfHPAcquireCard = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.SubHPAddSelfHPAcquireCard,
                                        actionUnitData.UnitCamp);

                                    if (subHPAddSelfHPAcquireCard != null)
                                    {
                                        var drSubHPAddSelfHPAcquireCard = GameEntry.DataTable.GetBless(EBlessID.SubHPAddSelfHPAcquireCard);

                                        BattleCardManager.Instance.CacheAcquireCards(addSelfHPTriggerData, triggerDatas,
                                            int.Parse(drSubHPAddSelfHPAcquireCard.GetValues(0)[0]));

                                    }
                                }
                                
                                
                                
                                
                                
                            }
                            
                            if (hurtSubDmgCount > 0)
                            {
                                var useDefenseCount = 0;
                                var _triggerValue = triggerData.Value + triggerData.DeltaValue;
                                if (Mathf.Abs(_triggerValue) >= hurtSubDmgCount)
                                {
                                    useDefenseCount = hurtSubDmgCount;
                                    triggerData.DeltaValue += hurtSubDmgCount;
                                }
                                else
                                {
                                    useDefenseCount = (int)-_triggerValue;
                                    triggerData.DeltaValue = (int)-_triggerValue;
                                }
                                
                                var actualUseDefenseCount = useDefenseCount;
                                if (effectUnitData.FuneCount(EBuffID.Spec_UnitStateSubOne) > 0  && actualUseDefenseCount > 1)
                                {
                                    actualUseDefenseCount = 1;
                                }

                                var subHurtSubDmgData = BattleFightManager.Instance.Unit_State(triggerDatas, triggerData.OwnUnitIdx,
                                    triggerData.ActionUnitIdx, effectUnitData.Idx, EUnitState.HurtSubDmg, -actualUseDefenseCount,
                                    ETriggerDataType.State);
                                subHurtSubDmgData.ActionUnitGridPosIdx =
                                    subHurtSubDmgData.EffectUnitGridPosIdx = effectUnitData.GridPosIdx;
                                SimulateTriggerData(subHurtSubDmgData, triggerDatas);
                                triggerDatas.Add(subHurtSubDmgData);

                                var hurtSubDmgCounterAtk =
                                    RoundFightData.GamePlayData.GetUsefulBless(EBlessID.HurtSubDmgCounterAtk,
                                        effectUnitData.UnitCamp);
                                if (hurtSubDmgCounterAtk != null && actionUnitData != null)
                                {
                                    var counterAttackTriggerData = BattleFightManager.Instance.BattleRoleAttribute(effectUnitData.Idx,
                                        effectUnitData.Idx, triggerData.ActionUnitIdx, EUnitAttribute.HP, -useDefenseCount,
                                        ETriggerDataSubType.Unit);
                                    counterAttackTriggerData.ActionUnitGridPosIdx = effectUnitData.GridPosIdx;
                                    counterAttackTriggerData.EffectUnitGridPosIdx = actionUnitData.GridPosIdx;
                                    
                                    SimulateTriggerData(counterAttackTriggerData, triggerDatas);
                                    triggerDatas.Add(counterAttackTriggerData);
                                }
                            }
                        }
                        
                        

                        if (actionUnitData != null && actionUnitData is Data_BattleSolider solider &&
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
                                RoundFightData.GamePlayData.BattleData.Round % BattleBuffManager.Instance.GetBuffValue(drEachRoundDoubleDamage.Values0[0]) == 0)
                            {

                                triggerData.DeltaValue +=
                                    eachRoundDoubleDamageCount * BattleBuffManager.Instance.GetBuffValue(drEachRoundDoubleDamage.Values0[1]);
                            }

                            RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(solider.UnitCamp).RoundIsAttack =
                                true;


                        }


                        if (triggerData.Value + triggerData.DeltaValue > 0)
                        {
                            triggerData.DeltaValue = -triggerData.Value;
                        }
                        
                        triggerValue = triggerData.Value + triggerData.DeltaValue;
                    }


                    var hpDelta = (int) triggerValue;
                    
                    
                    //hpDelta = (int) (triggerData.Value + triggerData.DeltaValue);
                    


                    if (actionUnitData != null && actionUnitData.GetStateCount(EUnitState.RecoverHP) > 0)
                    {
                        var recoverHPValue = -hpDelta;
                        var recoverHPTriggerData = BattleFightManager.Instance.BattleRoleAttribute(actionUnitData.Idx,
                            actionUnitData.Idx, actionUnitData.Idx, EUnitAttribute.HP, recoverHPValue,
                            ETriggerDataSubType.Unit);
                        recoverHPTriggerData.ActionUnitGridPosIdx =
                            recoverHPTriggerData.EffectUnitGridPosIdx = actionUnitData.GridPosIdx;
                        SimulateTriggerData(recoverHPTriggerData, triggerDatas);
                        triggerDatas.Add(recoverHPTriggerData);
                    }

                    if ( actionUnitData!= null  && actionUnitData.GetStateCount(EUnitState.DoubleDmg) > 0 && !GameUtility.ContainRoundState(
                        GamePlayManager.Instance.GamePlayData, EBuffID.Spec_CurseUnEffect))
                    {
                        triggerData.Value *= 2;
                        triggerData.DeltaValue *= 2;
 
                    }
                    
                    triggerValue = triggerData.Value + triggerData.DeltaValue;
                    

                    triggerValue = BattleFightManager.Instance.ChangeHP(effectUnitData, triggerValue, EHPChangeType.Unit, true,
                        triggerData.ChangeHPInstantly, triggerData);
                    
                    var counterAtkCount = effectUnitData.GetAllStateCount(EUnitState.CounterAtk);
                    if (counterAtkCount > 0 && actionUnitData != null && actionUnitData.Idx != effectUnitData.Idx)
                    {
                        var counterValue = -counterAtkCount;
                        
                        var actualCounterAtkCount = counterAtkCount;
                        
                        if (actualCounterAtkCount > actionUnitData.CurHP)
                        {
                            actualCounterAtkCount = actionUnitData.CurHP;
                            counterValue = -actionUnitData.CurHP;
                        }
                        
                        if (actualCounterAtkCount > 0 )
                        {
                            if (effectUnitData.FuneCount(EBuffID.Spec_UnitStateSubOne) > 0  && actualCounterAtkCount > 1)
                            {
                                actualCounterAtkCount = 1;
                            }
                            
                            var subCounterAttackTriggerData = BattleFightManager.Instance.Unit_State(triggerDatas, effectUnitData.Idx,
                                actionUnitData.Idx, effectUnitData.Idx, EUnitState.CounterAtk, -actualCounterAtkCount,
                                ETriggerDataType.State);
                            subCounterAttackTriggerData.ActionUnitGridPosIdx = effectUnitData.GridPosIdx;
                            subCounterAttackTriggerData.EffectUnitGridPosIdx = effectUnitData.GridPosIdx;
          
                            //subCounterAttackTriggerData.ActionUnitGridPosIdx
                            SimulateTriggerData(subCounterAttackTriggerData, triggerDatas);
                            triggerDatas.Add(subCounterAttackTriggerData);

                            var counterAttackTriggerData = BattleFightManager.Instance.BattleRoleAttribute(effectUnitData.Idx,
                                effectUnitData.Idx, triggerData.ActionUnitIdx, EUnitAttribute.HP, counterValue,
                                ETriggerDataSubType.Unit);
                            counterAttackTriggerData.ActionUnitGridPosIdx = effectUnitData.GridPosIdx;
                            counterAttackTriggerData.EffectUnitGridPosIdx = actionUnitData.GridPosIdx;
                            SimulateTriggerData(counterAttackTriggerData, triggerDatas);
                            triggerDatas.Add(counterAttackTriggerData);
                            
                            var counterAtkAddCurHP = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.CounterAtkAddCurHP,
                                effectUnitData.UnitCamp);
                            
                            if (counterAtkAddCurHP != null)
                            {
                                var counterAtkAddCurHPTriggerData = BattleFightManager.Instance.BattleRoleAttribute(effectUnitData.Idx,
                                    effectUnitData.Idx, effectUnitData.Idx, EUnitAttribute.HP, -counterValue,
                                    ETriggerDataSubType.Bless);
                                counterAtkAddCurHPTriggerData.ActionUnitGridPosIdx = actionUnitData.GridPosIdx;
                                counterAtkAddCurHPTriggerData.EffectUnitGridPosIdx = effectUnitData.GridPosIdx;
                                //BattleBuffManager.Instance.CacheTriggerData(counterAtkAddCurHPTriggerData, triggerDatas);
                                SimulateTriggerData(counterAtkAddCurHPTriggerData, triggerDatas);
                                triggerDatas.Add(counterAtkAddCurHPTriggerData);
                            }

                            var counterAtkAcquireCard = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.CounterAtkAcquireCard,
                                effectUnitData.UnitCamp);

                            if (counterAtkAcquireCard != null)
                            {
                                
                                BattleCardManager.Instance.CacheAcquireCards(counterAttackTriggerData, triggerDatas, 1);

                            }
                        }
                        
                        
                        
                    }
                    
                }

                //受击增加强化
                if (effectUnitData.GetStateCount(EUnitState.AddDmg) > 0)
                {

                }


                // if (actionUnitData != null && subDmgCount > 0)
                // {
                //     var actualSubDmgCount = subDmgCount;
                //     if (actionUnitData.FuneCount(EBuffID.Spec_UnitStateSubOne) > 0  && actualSubDmgCount > 1)
                //     {
                //         actualSubDmgCount = 1;
                //     }
                //     
                //     var subDamageTriggerData = BattleFightManager.Instance.Unit_State(triggerDatas, actionUnitData.Idx,
                //         actionUnitData.Idx, actionUnitData.Idx, EUnitState.SubDmg, -actualSubDmgCount, ETriggerDataType.RoleState);
                //
                //     subDamageTriggerData.ActionUnitGridPosIdx =
                //         subDamageTriggerData.EffectUnitGridPosIdx = actionUnitData.GridPosIdx;
                //     SimulateTriggerData(subDamageTriggerData, triggerDatas);
                //     triggerDatas.Add(subDamageTriggerData);
                // }
                
            }
            else if (triggerValue > 0)
            {
                
                
                
                if (BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.HP1UnRecover) &&
                      effectUnitData.CurHP == 1 && effectUnitData.UnitCamp == EUnitCamp.Player1)
                {
                    triggerData.Value = 0;
                    triggerData.DeltaValue = 0;
                }
                else
                {
                    var addDmgToRecover = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.AddDmgToRecover,
                        PlayerManager.Instance.PlayerData.UnitCamp);
                    var hpDelta = effectUnitData.MaxHP - effectUnitData.CurHP;
                    
                    if (addDmgToRecover != null && hpDelta > 0)
                    {
                        var addDmgCount = effectUnitData.GetAllStateCount(EUnitState.AddDmg);
                    
                        if (addDmgCount > 0)
                        {
                            if (hpDelta > triggerValue)
                            {
                                var actualAddValue = hpDelta > addDmgCount ? addDmgCount : hpDelta;
                                if (actualAddValue > 0)
                                {
                                    triggerData.DeltaValue += actualAddValue;
                                    
                                    var actualAddDmgCount = actualAddValue;
                                    if (effectUnitData.FuneCount(EBuffID.Spec_UnitStateSubOne) > 0 && actualAddDmgCount > 1)
                                    {
                                        actualAddDmgCount = 1;
                                    }
                        
                                    var subAddDmgCountData = BattleFightManager.Instance.Unit_State(triggerDatas, effectUnitData.Idx,
                                        effectUnitData.Idx, effectUnitData.Idx, EUnitState.AddDmg, -actualAddDmgCount,
                                        ETriggerDataType.State);
                                    SimulateTriggerData(subAddDmgCountData, triggerDatas);
                                    triggerDatas.Add(subAddDmgCountData);
                                }
                            }
                            
                            
                            
                        }
                    }
                }
                
                if ( actionUnitData!= null  && actionUnitData.GetStateCount(EUnitState.DoubleDmg) > 0 && !GameUtility.ContainRoundState(
                    GamePlayManager.Instance.GamePlayData, EBuffID.Spec_CurseUnEffect))
                {
                    triggerData.Value *= 2;
                    triggerData.DeltaValue *= 2;
                    
                }
                
                triggerValue = triggerData.Value + triggerData.DeltaValue;

                BattleFightManager.Instance.ChangeHP(effectUnitData, triggerValue,
                    EHPChangeType.Unit, true, triggerData.ChangeHPInstantly, triggerData);

            }


            if (triggerData.EffectUnitIdx == PlayerManager.Instance.PlayerData.BattleHero.Idx)
            {
                triggerData.ActualValue = triggerValue;     
            }
            else if (triggerValue < 0)
            {
                if (Mathf.Abs(triggerValue) > effectUnitOldHP && effectUnitOldHP > 0)
                {
                    triggerData.ActualValue = -effectUnitOldHP;
                }
                else
                {
                    triggerData.ActualValue = triggerValue;
                }
            }
            else
            {
                
                triggerData.ActualValue = effectUnitData.CurHP - effectUnitOldHP;
            }
            
            if (!effectUnitData.Exist())
            {
                BattleCurseManager.Instance.CacheUnitDeadRecoverLessHPUnit(effectUnitOldHP, effectUnitData.CurHP,
                    triggerDatas);
                // DeadTrigger(triggerData, triggerDatas);
                // KillTrigger(triggerData, triggerDatas);
                //
                //
                CacheLinks();

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


        public void RefreshUnitGridPosIdx()
        {
            // BattleUnitDatasByGridPosIdx.Clear();
            //
            // foreach (var kv in BattleUnitDatas)
            // {
            //     if (!BattleUnitDatasByGridPosIdx.ContainsKey(kv.Value.GridPosIdx))
            //     {
            //         BattleUnitDatasByGridPosIdx.Add(kv.Value.GridPosIdx, kv.Value);
            //     }
            //
            // }
        }


        public TriggerData BattleRoleAttribute(int ownSoliderIdx, int actionSoliderIdx, int effectUnitIdx,
            EUnitAttribute attribute, float attributeValue, ETriggerDataSubType triggerDataSubType)
        {
            var triggerData = new TriggerData();
            triggerData.TriggerDataType = ETriggerDataType.Atrb;
            triggerData.TriggerDataSubType = triggerDataSubType;
            triggerData.OwnUnitIdx = ownSoliderIdx;
            triggerData.ActionUnitIdx = actionSoliderIdx;
            triggerData.EffectUnitIdx = effectUnitIdx;
            triggerData.BattleUnitAttribute = attribute;
            triggerData.Value = attributeValue;

            return triggerData;
        }



        public TriggerData Unit_HeroAttribute(int triggerUnitIdx, int actionUnitIdx, int effectUnitIdx,
            EHeroAttribute attribute, float attributeValue)
        {
            var cardTriggerData = new TriggerData();
            cardTriggerData.TriggerDataType = ETriggerDataType.HeroAtrb;

            cardTriggerData.OwnUnitIdx = triggerUnitIdx;
            cardTriggerData.ActionUnitIdx = actionUnitIdx;
            cardTriggerData.EffectUnitIdx = effectUnitIdx;
            cardTriggerData.HeroAttribute = attribute;
            cardTriggerData.Value = attributeValue;

            return cardTriggerData;
        }

        public TriggerData Unit_State(List<TriggerData> triggerDatas, int triggerUnitIdx, int actionUnitIdx, int effectUnitIdx,
            EUnitState unitState, int value, ETriggerDataType triggerDataType)
        {
            var effectUnit = GameUtility.GetUnitDataByIdx(effectUnitIdx);
            var actionUnit = GameUtility.GetUnitDataByIdx(actionUnitIdx);
            if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.DeBuff].Contains(unitState))
            {
                //BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.AddDebuffRecoverHP)
                var addDebuffAddCurHP = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.AddDebuffAddCurHP,
                    effectUnit.UnitCamp);
                if (addDebuffAddCurHP != null && value > 0)
                {
                    var triggerData = BattleFightManager.Instance.BattleRoleAttribute(effectUnit.Idx, effectUnit.Idx,
                        effectUnit.Idx, EUnitAttribute.HP, 1, ETriggerDataSubType.Bless);
                    
                    BattleBuffManager.Instance.CacheTriggerData(triggerData, triggerDatas);
                }

            }
            
            return Unit_State(triggerUnitIdx, actionUnitIdx, effectUnitIdx,
                unitState, value, triggerDataType);
        }

        private TriggerData Unit_State(int triggerSoliderID, int actionSoliderID, int effectUnitID,
            EUnitState unitState, float value, ETriggerDataType triggerDataType)
        {
            var triggerData = new TriggerData();
            var unit = GetUnitByIdx(effectUnitID);
            if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.DeBuff].Contains(unitState))
            {
                if (unit.GetStateCount(EUnitState.DeBuffUnEffect) > 0)
                {
                    triggerData.TriggerDataType = ETriggerDataType.State;
                    triggerData.OwnUnitIdx = effectUnitID;
                    triggerData.ActionUnitIdx = effectUnitID;
                    triggerData.EffectUnitIdx = effectUnitID;
                    triggerData.UnitStateDetail.UnitState = EUnitState.DeBuffUnEffect;
                    triggerData.Value = -1;
                    triggerData.ActualValue = triggerData.Value;
                    return triggerData;
                }
                else if (unit.GetRoundStateCount(EUnitState.DeBuffUnEffect) > 0)
                {
                    triggerData.TriggerDataType = ETriggerDataType.Empty;
                    return triggerData;
                }
                
            }
            else if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.Buff].Contains(unitState))
            {
                if (unit.GetAllStateCount(EUnitState.BuffAddMore) > 0)
                {
                    value += 1;
                }
                
            }
            
            triggerData.TriggerDataType = triggerDataType;
            triggerData.OwnUnitIdx = triggerSoliderID;
            triggerData.ActionUnitIdx = actionSoliderID;
            triggerData.EffectUnitIdx = effectUnitID;
            triggerData.UnitStateDetail.UnitState = unitState;
            triggerData.UnitStateDetail.Value = (int)value;
            triggerData.Value = value;
            triggerData.ActualValue = triggerData.Value;
            return triggerData;
        }
        
        public TriggerData  Hero_Card(int triggerSoliderID, int actionSoliderID, int effectUnitID, float value,
            ECardTriggerType cardTriggerType)
        {

            var cardTriggerData = new TriggerData();
            cardTriggerData.TriggerDataType = ETriggerDataType.Card;
            cardTriggerData.CardTriggerType = cardTriggerType;
            cardTriggerData.OwnUnitIdx = triggerSoliderID;
            cardTriggerData.ActionUnitIdx = actionSoliderID;
            cardTriggerData.EffectUnitIdx = effectUnitID;
        
            cardTriggerData.Value = value;
        
            return cardTriggerData;
        
        }
        
        public async Task TriggerAction(TriggerData triggerData)
        {
            // var ownUnitEntity = BattleUnitManager.Instance.GetUnitByID(triggerData.OwnUnitID);
            // if (ownUnitEntity == null)
            //     return;
            //
            
            var effectUnitEntity = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
            // if (effectUnitEntity == null && triggerData.TriggerDataType != ETriggerDataType.Card)
            //     return;
            
            var actionUnitEntity = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
            
            
            if (triggerData.TriggerResult == ETriggerResult.UnHurt)
            {
                effectUnitEntity?.Dodge();
                return;
            }


            if (triggerData.BuffTriggerType == EBuffTriggerType.TacticClearBuff)
            {
                BattleBuffManager.Instance.Spec_ClearBuff(triggerData);
            }
            // if (triggerData.TriggerResult == ETriggerResult.Dodge)
            // {
            //     effectUnitEntity.BattleUnit.RemoveState(EUnitState.Dodge);
            //     effectUnitEntity.Dodge();
            //     return;
            // }

            var triggerValue = (int)triggerData.ActualValue;// (int) (triggerData.Value + triggerData.DeltaValue);
            
            

            // if (effectUnitEntity.BattleUnit.AddHeroHP != 0)
            // {
            //     var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(effectUnitEntity.UnitCamp);
            //     playerData.BattleHero.CacheHPDelta += effectUnitEntity.BattleUnit.AddHeroHP;
            //     effectUnitEntity.BattleUnit.AddHeroHP = 0;
            //     return;
            // }
                
            
            switch (triggerData.TriggerDataType)
            {
                case ETriggerDataType.HeroAtrb:
                    //var battleHeroEntity = effectUnitEntity as BattleHeroEntity;
                    switch (triggerData.HeroAttribute)
                    {
                        case EHeroAttribute.HP:
                            HeroManager.Instance.BattleHeroData.CacheHPDelta += triggerValue;

                            if (triggerData.TriggerDataSubType == ETriggerDataSubType.Bless)
                            {
                                var blessData = BlessManager.Instance.GetBless(triggerData.TriggerBlessIdx);
                                if (blessData.BlessID == EBlessID.ShuffleCardAddCurHP)
                                {
                                    BlessManager.Instance.AnimationAddCurHP((int)triggerData.ActualValue,
                                        BattleController.Instance.PassCardPos.gameObject, EBlessID.ShuffleCardAddCurHP);
                                    
                                }
                                else if (blessData.BlessID == EBlessID.ConsumeCardAddCurHP)
                                {
                                    BlessManager.Instance.AnimationAddCurHP((int)triggerData.ActualValue,
                                        BattleController.Instance.ConsumeCardPos.gameObject,
                                        EBlessID.ConsumeCardAddCurHP);

                                }

                                // var coreHurtAcquireCard = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.CoreHurtAcquireCard,
                                //     PlayerManager.Instance.PlayerData.UnitCamp);
                                //
                                // if (coreHurtAcquireCard != null && triggerValue < 0)
                                // {
                                //     var drCoreHurtAcquireCard = GameEntry.DataTable.GetBless(EBlessID.CoreHurtAcquireCard);
                                //     triggerData.AcquireCardCirculation.HandCards = BattleCardManager.Instance.CacheAcquireHandCards(
                                //         RoundFightData.GamePlayData, int.Parse(drCoreHurtAcquireCard.GetValues(0)[0]));
                                //
                                // }
                                
                            }
                            else
                            {
                                var endValue = BlessManager.Instance.AddCurHPByAttackDamage()
                                    ? (int)(triggerData.Value + triggerData.DeltaValue)
                                    : (int)triggerData.ActualValue;
                            
                                var moveParams = new MoveParams()
                                {
                                    FollowGO = actionUnitEntity.gameObject,
                                    DeltaPos = new Vector2(0, 25f),
                                    IsUIGO = false,
                                };
            
                                var targetMoveParams = new MoveParams()
                                {
                                    FollowGO = AreaController.Instance.UICore,
                                    DeltaPos = new Vector2(0, -25f),
                                    IsUIGO = true,
                                };

                                GameEntry.Entity.ShowBattleMoveValueEntityAsync((int)triggerData.ActualValue, endValue, -1, false,
                                    false, moveParams, targetMoveParams);
                            }
                            
                            HeroManager.Instance.UpdateCacheHPDelta();
                            
                            break;
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
                            HeroManager.Instance.BattleHeroData.Attribute.SetAttribute(
                                EHeroAttribute.Coin,
                                + HeroManager.Instance.BattleHeroData.Attribute.GetAttribute(
                                    EHeroAttribute.Coin) + triggerData.Value);
                            break;
                        case EHeroAttribute.Damage:
                            HeroManager.Instance.BattleHeroData.Attribute.SetAttribute(
                                EHeroAttribute.Damage,
                                + HeroManager.Instance.BattleHeroData.Attribute.GetAttribute(
                                    EHeroAttribute.Damage) + triggerData.Value);
                            break;
                        
                        
                        
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ETriggerDataType.Atrb:
                    switch (triggerData.BattleUnitAttribute)
                    {
                        case EUnitAttribute.HP:
                            
                            var hpDelta = effectUnitEntity.ChangeCurHP(triggerValue, true, triggerData.CoreHPDelta > 0, triggerData.ChangeHPInstantly, true);
                            
                            if (triggerData.BattleUnitAttribute == EUnitAttribute.HP &&
                                !triggerData.ChangeHPInstantly && hpDelta != 0)
                            {
                                //effectUnitEntity.BattleUnit.CacheHPDelta += hpDelta;
                                //effectUnitEntity.BattleUnitData.AddHeroHP += hpDelta;
                                HeroManager.Instance.BattleHeroData.CacheHPDelta += hpDelta;
                                effectUnitEntity.Hurt(actionUnitEntity);
                                //HeroManager.Instance.HeroEntity.AddHurts(hpDelta);
                                // var coreHurtAcquireCard = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.CoreHurtAcquireCard,
                                //     PlayerManager.Instance.PlayerData.UnitCamp);
                                //
                                // if (coreHurtAcquireCard != null && hpDelta < 0)
                                // {
                                //     var drCoreHurtAcquireCard = GameEntry.DataTable.GetBless(EBlessID.CoreHurtAcquireCard);
                                //     BattleCardManager.Instance.CacheAcquireCards(triggerData, int.Parse(drCoreHurtAcquireCard.GetValues(0)[0]));
                                //
                                // }
                                
                    
                            }
                            //triggerData.HeroHPDelta && hpDelta != 0
                            else if (triggerData.CoreHPDelta > 0)
                            {
                                
                                //effectUnitEntity.BattleUnitData.AddHeroHP += hpDelta;\
                                //-hpDelta;
                                HeroManager.Instance.BattleHeroData.CacheHPDelta +=
                                    triggerData.CoreHPDelta;
                                //HeroManager.Instance.BattleHeroData.CacheHPDelta += -hpDelta;
                                //triggerData.HeroHPDelta = 0;
                                // var heroEntity = HeroManager.Instance.GetHeroEntity(effectUnitEntity.UnitCamp);
                                //
                                // if (heroEntity != null)
                                // {
                                //     heroEntity.BattleUnit.CacheHPDelta += -hpDelta;
                                //     triggerData.HeroHPDelta = false;
                                //
                                // }

                            }

                            
                            
                            if (triggerValue < 0)
                            {
                                
                                effectUnitEntity.BattleUnitData.HurtTimes += 1;
                                
                                if (triggerData.UnitStateDetail.UnitState == EUnitState.HurtRoundStart)
                                {
                                    BattleEffectManager.Instance.ShowHurtRoundStartEffect(effectUnitEntity.Position);
                                }
                                
                                effectUnitEntity.Hurt(actionUnitEntity);
                            } 
                            else if (triggerValue > 0)
                            {
                                effectUnitEntity.Recover();
                            }

                            // if (triggerData.UnitStateDetail.UnitState == EUnitState.HurtEachMove || triggerData.UnitStateDetail.UnitState == EUnitState.HurtRoundStart)
                            // {
                            //     effectUnitEntity.BattleUnitData.RemoveState(triggerData.UnitStateDetail.UnitState);
                            // }
                            
                            if (actionUnitEntity != null)
                            {
                                // if (triggerData.UnitStateDetail.UnitState == EUnitState.AtkPassUs ||
                                //     triggerData.UnitStateDetail.UnitState == EUnitState.AtkPassEnemy &&
                                //     actionUnitEntity.BattleUnitData.GetStateCount(triggerData.UnitStateDetail.UnitState) > 0)
                                // {
                                //     actionUnitEntity.BattleUnitData.RemoveState(triggerData.UnitStateDetail.UnitState);
                                // }
                            }
                            
                            break;
                        case EUnitAttribute.MaxHP:
                            var recover = (int) (triggerData.Value + triggerData.DeltaValue);
                            effectUnitEntity.BattleUnitData.BaseMaxHP += recover;
                            effectUnitEntity.ChangeCurHP(recover, true, false, true, true);
                            break;
                        case EUnitAttribute.Empty:
                            break;
                        case EUnitAttribute.BaseDamage:
                            var damage = (int) (triggerData.Value + triggerData.DeltaValue);
                            effectUnitEntity.BattleUnitData.BaseDamage += damage;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ETriggerDataType.State:

                    if (triggerData.UnitStateDetail.Value > 0)
                    {
                        if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.DeBuff].Contains(triggerData.UnitStateDetail.UnitState))
                        {
                            effectUnitEntity.Hurt(actionUnitEntity);    
                        }
                        if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.Buff].Contains(triggerData.UnitStateDetail.UnitState))
                        {
                            effectUnitEntity.Recover();    
                        }
                        
                        if (triggerData.UnitStateDetail.UnitState == EUnitState.HurtRoundStart)
                        {
                            BattleEffectManager.Instance.ShowHurtRoundStartEffect(effectUnitEntity.Position, effectUnitEntity.transform);
                        
                        }
                    }
                    
                    
                    BattleIconValueManager.Instance.AnimtionChangeUnitState(triggerData.UnitStateDetail.UnitState, triggerValue,
                        triggerData,  -1, false);

                    effectUnitEntity.BattleUnitData.ChangeState(triggerData.UnitStateDetail.UnitState, triggerValue);
                    break;
                case ETriggerDataType.RoundState:
                    // if (actionUnitEntity != null && actionUnitEntity.ID != effectUnitEntity.ID)
                    // {
                    //     actionUnitEntity.Attack();
                    // }
                    effectUnitEntity.Hurt(actionUnitEntity);
                    effectUnitEntity.BattleUnitData.ChangeRoundState(triggerData.UnitStateDetail.UnitState, (int)(triggerData.Value + triggerData.DeltaValue));
                    break;
                case ETriggerDataType.Card:
                    triggerValue = (int)(triggerData.ActualValue = triggerData.Value + triggerData.DeltaValue);
                    switch (triggerData.CardTriggerType)
                    {
                        case ECardTriggerType.AcquireCard:
                            //BattleCardManager.Instance.AcquireCards((int)(triggerData.Value + triggerData.DeltaValue));
                            // if (triggerData.AcquireCardCirculation.HandCards.Count > 0)
                            // {
                            //     BattleCardManager.Instance.AcquireCards(triggerData);
                            // }
                            
                            break;
                        case ECardTriggerType.ToHand:
                            
                            if (actionUnitEntity is BattleSoliderEntity ToHand_solider)
                            {
                                BattleCardManager.Instance.AnimationToHandCards(ToHand_solider.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                            }
                            
                            break;
                        case ECardTriggerType.ToConsume:
                            if (triggerData.TriggerCardIdx != -1)
                            {
                                var cardData = BattleCardManager.Instance.GetCardData(triggerData.TriggerCardIdx);
                                cardData.CardDestination = ECardDestination.Consume;
                                
                            }
                            else if (actionUnitEntity is BattleSoliderEntity ToConsume_solider)
                            {
                                BattleCardManager.Instance.AnimationToConsumeCards(ToConsume_solider.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                            }
                            break;
                        case ECardTriggerType.ToStandBy:
                            if (triggerData.TriggerCardIdx != -1)
                            {
                                var cardData = BattleCardManager.Instance.GetCardData(triggerData.TriggerCardIdx);
                                cardData.CardDestination = ECardDestination.StandBy;
                                
                            }
                            else if (actionUnitEntity is BattleSoliderEntity ToStandBy_solider)
                            {
                                BattleCardManager.Instance.ToStandByCards(BattlePlayerManager.Instance.BattlePlayerData,
                                    ToStandBy_solider.BattleSoliderEntityData.BattleSoliderData.CardIdx);
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
                            //BattleCardManager.Instance.ConsumeCard((int)triggerData.Value);
                            break;
                        case ECardTriggerType.StandByToPass:
                            BattleCardManager.Instance.RandomStandByToPass();
                            break;
                        case ECardTriggerType.Empty:
                            break;
                        case ECardTriggerType.ConsumeToHand:
                            BattleCardManager.Instance.AnimationConsumeToHand();
                            break;
                        case ECardTriggerType.PassToHand:
                            BattleCardManager.Instance.AnimationPassToHand(triggerValue);
                            break;
                        case ECardTriggerType.CardEnergy:
                            if (triggerData.TriggerCardIdx != -1)
                            {
                                var cardEntity = CardManager.Instance.GetCard(triggerData.TriggerCardIdx);
                                cardEntity.EnergyDelta +=
                                    (int)(triggerData.Value + triggerData.DeltaValue);

                            }
                            else if (actionUnitEntity is BattleSoliderEntity CardEnergy_solider)
                            {
                                var cardEntity = CardManager.Instance.GetCard(CardEnergy_solider.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                                cardEntity.EnergyDelta +=
                                    (int)(triggerData.Value + triggerData.DeltaValue);
                            }
                            GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
                            break;
                        case ECardTriggerType.MaxHP:
                            if (triggerData.TriggerCardIdx != -1)
                            {
                                var cardEntity = CardManager.Instance.GetCard(triggerData.TriggerCardIdx);
                                cardEntity.MaxHPDelta +=
                                    (int)(triggerData.Value + triggerData.DeltaValue);

                            }
                            else if (actionUnitEntity is BattleSoliderEntity MaxHP_solider)
                            {
                                var cardData = CardManager.Instance.GetCard(MaxHP_solider.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                                cardData.MaxHPDelta +=
                                    (int)(triggerData.Value + triggerData.DeltaValue);
                            }
                            GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ETriggerDataType.Link:
                    effectUnitEntity.BattleUnitData.BattleLinkIDs.Add(triggerData.LinkID);
                    break;
                case ETriggerDataType.RemoveUnit:
                    effectUnitEntity.BattleUnitData.RemoveAllState();
                    effectUnitEntity.ChangeCurHP(triggerValue, true, true, triggerData.ChangeHPInstantly, true);
                    if (triggerValue < 0)
                    {
                        effectUnitEntity.BattleUnitData.HurtTimes += 1;
                        effectUnitEntity.Hurt(actionUnitEntity);
                    }
                    break;
                case ETriggerDataType.RoundBuff:
                    //var battlePlayerData = GamePlayManager.Instance.GamePlayData.BattleData.GetBattlePlayerData(effectUnitEntity.UnitCamp);
                    //battlePlayerData.RoundBuffs.Add(triggerData.BuffID);
                    break;
                case ETriggerDataType.ClearBuff:
                    effectUnitEntity.BattleUnitData.RemoveAllState(triggerData.UnitStateEffectTypes);
                    break;
                case ETriggerDataType.TransferBuff:
                    
                    BattleBuffManager.Instance.TransferBuff(actionUnitEntity.BattleUnitData, effectUnitEntity.BattleUnitData, triggerData);
                    break;
                default:
                    break;
            }
            
            if (triggerData.AcquireCardCirculation.HandCards.Count > 0)
            {
                BattleCardManager.Instance.AcquireCards(triggerData);
            }

            // if (triggerData.HeroHPDelta)
            // {
            //     // var heroEntity = HeroManager.Instance.GetHeroEntity(effectUnitEntity.UnitCamp);
            //     //
            //     // if (heroEntity != null)
            //     // {
            //     //     heroEntity.BattleUnit.CacheHPDelta += ;
            //     //     triggerData.HeroHPDelta = false;
            //     //
            //     //
            //     // }
            //     
            //     // var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(effectUnitEntity.UnitCamp);
            //     // if (playerData != null && playerData.BattleHero != null)
            //     // {
            //     //     playerData.BattleHero.ChangeHP(triggerData.HeroHPDelta);
            //     //     triggerData.HeroHPDelta = 0;
            //     //
            //     //
            //     // }
            // }
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        }
        
        
        public void RoundStartBuffTrigger()
        {
            BattleFightManager.Instance.IsAction = true;
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
            foreach (var triggerCollection in actionData.TriggerDataDict)
            {
                foreach (var triggerData in triggerCollection.Value.TriggerDatas)
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

            GameUtility.DelayExcute(isAttack ? 1f : 0.5f, () =>
            {
                HeroManager.Instance.UpdateCacheHPDelta();
                BattleManager.Instance.ContinueAction();
            });

            
        }
        
        public void RoundStartUnitTrigger()
        {
            UnitAttack(RoundFightData.RoundStartUnitDatas, EActionProgress.RoundStartUnit);
        }
        
        public void PreRoundStartUnitTrigger()
        {
            BattleFightManager.Instance.ActionProgress = EActionProgress.PreRoundStart;
            UnitAttack(RoundFightData.PreRoundStartDatas, EActionProgress.PreRoundStart);
        }
        
        public void RoundEndTrigger()
        {
            BattleFightManager.Instance.IsAction = true;
            if(ActionProgress != EActionProgress.RoundEnd)
                return;
            
            foreach (var kv in RoundFightData.RoundEndDatas)
            {
                foreach (var triggerCollection in kv.Value.TriggerDataDict.Values)
                {
                    foreach (var triggerData in triggerCollection.TriggerDatas)
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
            //var unitKeys = BattleUnitDatas.Keys.ToList();
            BattleFightManager.Instance.IsAction = true;
            var unitKeys = unitAttackDatas.Keys.ToList();
            while (true)
            {
                if (AcitonUnitIdx >= unitAttackDatas.Count)
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
            BattleBulletManager.Instance.AddTriggerCollection(actionData);
            
            foreach (var triggerCollection in actionData.TriggerDataDict)
            {
                foreach (var triggerData in triggerCollection.Value.TriggerDatas)
                {
                    isAttack = true;
                    //BattleBulletManager.Instance.AddTriggerData(triggerData);
                    //TriggerAction(triggerData);
                }
            }

            var time = 0.1f;
            
            var moveTime = 0f;
            var maxMoveTime = 0f;
            
            //BattleBulletManager.Instance.AddMoveActionData(unitKeys[AcitonUnitIdx], actionData.MoveData);
            foreach (var kv in actionData.TriggerDataDict)
            {
                foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
                {
                    var effectUnitEntity = BattleUnitManager.Instance.GetUnitByIdx(kv2.Value.UnitIdx);
                    moveTime = effectUnitEntity.GetMoveTime(kv2.Value.UnitActionState, kv2.Value.MoveActionData);

                    if (moveTime > maxMoveTime)
                    {
                        maxMoveTime = moveTime;
                    }
                }
            }
            
            //|| maxMoveTime > 0
            if (isAttack  || maxMoveTime > 0 || actionProgress == EActionProgress.SoliderActiveAttack)
            {
                time += 2f;
                
                var unit = GetUnitByIdx(unitKeys[AcitonUnitIdx]);
                unit.AttackInRound = true;
                BattleUnitManager.Instance.BattleUnitEntities[unitKeys[AcitonUnitIdx]].Attack(actionData);
                GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());

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
            BattleFightManager.Instance.ActionProgress = EActionProgress.EnemyMove;
            UnitMove(RoundFightData.EnemyMovePaths, RoundFightData.EnemyMoveDatas, RoundFightData.EnemyAttackDatas,
                EActionProgress.EnemyMove);
            
            
        }
        
        


        private void UnitMove(Dictionary<int, List<int>> unitMovePaths, Dictionary<int, MoveActionData> unitMoveDatas, Dictionary<int, ActionData> unitAttackDatas, EActionProgress actionProgress)
        {
            //BattleFightManager.Instance.IsAction = true;
            var unitKeys = unitMovePaths.Keys.ToList();
            while (true)
            {
                if (AcitonUnitIdx >= unitKeys.Count)
                {
                    AcitonUnitIdx = 0;
                    BattleManager.Instance.NextAction();
                    break;
                }
                
                var unit = BattleUnitManager.Instance.GetUnitByIdx(unitKeys[AcitonUnitIdx]);
                
                if (!unit.BattleUnitData.Exist())
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
            var moveActionData = unitMoveDatas[unitKeys[AcitonUnitIdx]].Copy();

            var runTime = 0f;

            if (moveActionData.MoveGridPosIdxs.Count > 1)
            {
                var effectUnitEntity = BattleUnitManager.Instance.GetUnitByIdx(unitKeys[AcitonUnitIdx]);
                runTime = effectUnitEntity.GetMoveTime(EUnitActionState.Run, moveActionData);
                effectUnitEntity.Run(moveActionData);
                //runTime = BattleUnitManager.Instance.BattleUnitEntities[unitKeys[AcitonUnitIdx]].Run(moveActionData);
            }

            AcitonUnitIdx++;
            
            GameUtility.DelayExcute(runTime, () =>
            {
                // if (unitAttackDatas.ContainsKey(unitID))
                // {
                //     var attackTime = UnitAttack(unitID, unitAttackDatas[unitID]);
                //     GameUtility.DelayExcute(attackTime, () =>
                //     {
                //         HeroManager.Instance.HeroEntity.UpdateCacheHPDelta();
                //         BattleManager.Instance.ContinueAction();
                //     });
                // }
                // else
                // {
                //     BattleManager.Instance.ContinueAction();
                // }
                BattleManager.Instance.ContinueAction();
                
            });
        }
        
        
        public void EnemyAttack()
        {
            UnitAttack(RoundFightData.EnemyAttackDatas, EActionProgress.EnemyAttack);
        }

        
        
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
            //BattleBulletManager.Instance.AddTriggerCollection(actionData);
            foreach (var triggerCollection in actionData.TriggerDataDict)
            {
                foreach (var triggerData in triggerCollection.Value.TriggerDatas)
                {
                    isAttack = true;
                    //TriggerAction(triggerData);
                    //BattleBulletManager.Instance.AddTriggerData(triggerData);
                }
            }

            if (isAttack)
            {
                Log.Debug("AA:" + unitKeys[AcitonUnitIdx]);
                BattleUnitManager.Instance.BattleUnitEntities[unitKeys[AcitonUnitIdx]].Attack(actionData);
                GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
            }
            
            
            AcitonUnitIdx++;

            GameUtility.DelayExcute(isAttack ? 1f : 0.5f, () =>
            {
                UseCardTrigger();
            });
        }

        // public List<TriggerData> GetMoveTriggerDatas(MoveActionData moveActionData, int moveIdx)
        // {
        //     if(moveActionData == null)
        //         return null;
        //     
        //     if(moveActionData.TriggerDataDict.ContainsKey(moveIdx))
        //         return moveActionData.TriggerDataDict[moveIdx].TriggerDatas;
        //     
        //     return null;
        // }
        
        public TriggerCollection GetMoveTriggerCollection(MoveActionData moveActionData, int moveIdx)
        {
            if(moveActionData == null)
                return null;
            
            if(moveActionData.TriggerDataDict.ContainsKey(moveIdx))
                return moveActionData.TriggerDataDict[moveIdx];
            
            return null;
        }

        public void MoveEffectAction(EUnitActionState unitActionState, MoveActionData moveActionData, int moveIdx, int moveUnitID)
        {
            var triggerCollection = GetMoveTriggerCollection(moveActionData, moveIdx);//unitActionState == EUnitActionState.Run ? GetRunTriggerDatas(effectID, moveIdx) : GetFlyTriggerDatas(actionID, effectID, moveIdx));
            if(triggerCollection == null)
                return;
            
            BattleBulletManager.Instance.AddTriggerCollection(triggerCollection);
            
            //var normalTriggerData = triggerCollection.GetNormalTriggerData();

            foreach (var triggerData in triggerCollection.TriggerDatas)
            {
                var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
                //Log.Debug("ActionUnitID:" + triggerData.ActionUnitIdx);
                //!(!triggerData.ChangeHPInstantly && HeroManager.Instance.IsHero(triggerData.EffectUnitID))
                if (triggerData.TriggerDataSubType == ETriggerDataSubType.Collision)
                {
                    //effectUnit?.Hurt();
                    //triggerData.IsTrigger = true;
                    BattleBulletManager.Instance.UseTriggerCollection(triggerData.ActionUnitIdx, triggerData.EffectUnitGridPosIdx);
                    BattleEffectManager.Instance.ShowCollideEffect(effectUnit.EffectHurtPos.position);
                    actionUnit.BattleUnitData.CollideCount += 1;
                    effectUnit.BattleUnitData.CollideCount += 1;
                }
            
                else if (triggerData.TriggerDataSubType == ETriggerDataSubType.State)
                {
                    //triggerData.IsTrigger = true;
                    BattleBulletManager.Instance.UseTriggerCollection(triggerData.ActionUnitIdx, triggerData.EffectUnitGridPosIdx);

                }
                else
                {
                    if (triggerData.ActionUnitIdx != -1 && triggerData.ActionUnitIdx != Constant.Battle.UnUnitTriggerIdx)
                    {
                        
                    
                        //BattleBulletManager.Instance.AddTriggerData(triggerData); 
                        actionUnit?.MoveAttack(triggerData.EffectUnitGridPosIdx);
                        //effectUnit.Hurt();
                    
                    }
                    else
                    {
                        BattleBulletManager.Instance.UseTriggerCollection(triggerData.ActionUnitIdx, triggerData.EffectUnitGridPosIdx);

                        // triggerData.IsTrigger = true;
                        //
                        // BattleFightManager.Instance.TriggerAction(triggerData.Copy());
                    }
                       
                }
            }
            

            HeroManager.Instance.UpdateCacheHPDelta();
            
 
            GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        }


        
        public Dictionary<int, MoveUnitData> GetHurtMoveDatas([CanBeNull] List<int> actionUnitIdxs, int effectGridPosIdx)
        {
            var moveDatas = new Dictionary<int, MoveUnitData>();

            foreach (var kv in RoundFightData.EnemyAttackDatas)
            {
                foreach (var kv2 in kv.Value.TriggerDataDict)
                {
                    foreach (var kv3 in kv2.Value.MoveData.MoveUnitDatas)
                    {
                        if(!(actionUnitIdxs!= null && actionUnitIdxs.Contains(kv2.Key) && kv3.Value.EffectGridPosIdx == effectGridPosIdx))
                            continue;
                    
                        moveDatas.Add(kv2.Key, kv3.Value);
                    }
  
                }
               
            }
            
            foreach (var kv in RoundFightData.SoliderActiveAttackDatas)
            {
                foreach (var kv2 in kv.Value.TriggerDataDict)
                {
                    foreach (var kv3 in kv2.Value.MoveData.MoveUnitDatas)
                    {
                        if(!(actionUnitIdxs!= null && actionUnitIdxs.Contains(kv2.Key) && kv3.Value.EffectGridPosIdx == effectGridPosIdx))
                            continue;
                    
                        moveDatas.Add(kv2.Key, kv3.Value);
                    }
  
                }
  
            }
            
            foreach (var kv2 in RoundFightData.BuffData_Use.TriggerDataDict)
            {
                foreach (var kv3 in kv2.Value.MoveData.MoveUnitDatas)
                {
                    if(!(actionUnitIdxs!= null && actionUnitIdxs.Contains(kv2.Key) && kv3.Value.EffectGridPosIdx == effectGridPosIdx))
                        continue;
                    
                    moveDatas.Add(kv2.Key, kv3.Value);
                }
  
            }
            

            return moveDatas;
        }
       
        public Dictionary<int, List<int>> GetAttackHurtFlyPaths(int actionUnitIdx, int effectUnitIdx)
        {
            Dictionary<int, TriggerCollection> moveDataDict = new Dictionary<int, TriggerCollection>();
            var unitFlyDict = new Dictionary<int, List<int>>();
            
            if (RoundFightData.EnemyAttackDatas.ContainsKey(actionUnitIdx))
            {
                moveDataDict = BattleFightManager.Instance.RoundFightData.EnemyAttackDatas[actionUnitIdx]
                    .TriggerDataDict;
                //[effectUnitIdx].MoveData
                //.MoveUnitDatas;
                
                foreach (var kv in moveDataDict)
                {
                    foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
                    {
                        if(effectUnitIdx != kv2.Value.MoveActionData.MoveUnitIdx)
                            continue;
                        
                        unitFlyDict.Add(kv2.Key, kv2.Value.MoveActionData.MoveGridPosIdxs);
                        
                    }
 
                }
            }
            else if (RoundFightData.SoliderActiveAttackDatas.ContainsKey(actionUnitIdx))
            {
                moveDataDict = BattleFightManager.Instance.RoundFightData.SoliderActiveAttackDatas[actionUnitIdx]
                    .TriggerDataDict;
                //[effectUnitIdx].MoveData
                //.MoveUnitDatas;

                foreach (var kv in moveDataDict)
                {
                    foreach (var kv2 in kv.Value.MoveData.MoveUnitDatas)
                    {
                        if (effectUnitIdx != kv2.Value.MoveActionData.MoveUnitIdx)
                            continue;

                        unitFlyDict.Add(kv2.Key, kv2.Value.MoveActionData.MoveGridPosIdxs);

                    }
                }

                // moveDataDict = BattleFightManager.Instance.RoundFightData.SoliderActiveAttackDatas[actionUnitIdx]
                //     .TriggerDataDict[effectUnitIdx].MoveData
                //     .MoveUnitDatas;
                //
                // foreach (var kv in moveDataDict)
                // {
                //     // if(effectUnitIdx != kv.Value.MoveActionData.MoveUnitIdx && actionUnitIdx != kv.Value.MoveActionData.MoveUnitIdx)
                //     //     continue;
                //
                //     //选择一个单位，对单位造成{0}点伤害，并击退相邻单位
                //     // &&
                //     // (actionUnitIdx == kv.Value.MoveActionData.MoveUnitIdx ||
                //     //  effectUnitIdx == kv.Value.MoveActionData.MoveUnitIdx)
                //     if (actionUnitIdx == kv.Value.ActionUnitIdx)
                //
                //     {
                //         var moveGridPosIdxs = kv.Value.MoveActionData.MoveGridPosIdxs;
                //         unitFlyDict.Add(kv.Key, moveGridPosIdxs);
                //     }
                //
                // }
            }
            else if (actionUnitIdx == Constant.Battle.UnUnitTriggerIdx)
            {
                moveDataDict = RoundFightData.BuffData_Use.TriggerDataDict;

                foreach (var kv2 in moveDataDict)
                {
                    foreach (var kv in kv2.Value.MoveData.MoveUnitDatas)
                    {
                        var moveGridPosIdxs = kv.Value.MoveActionData.MoveGridPosIdxs;
                
                        // if(effectUnitIdx != kv.Value.MoveActionData.MoveUnitIdx)
                        //     continue;
 
                        unitFlyDict.Add(kv.Key, moveGridPosIdxs);

                    }
                }
                
            }

            return unitFlyDict;
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
        
        
        public List<int> GetRunPaths(Dictionary<int, EGridType> gridTypes, int startPosIdx, int endPosIdx, List<int> runPaths)
        {
            var startCoord = GameUtility.GridPosIdxToCoord(startPosIdx);
            var endCoord = GameUtility.GridPosIdxToCoord(endPosIdx);
            
            runPaths.Clear();
            runPaths.Add(startPosIdx);

            if (gridTypes[endPosIdx] != EGridType.Empty)
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
                
                if (gridTypes[gridPosIdx] == EGridType.Obstacle)
                    break;
                
                
                runPaths.Add(gridPosIdx);

                var gridProp = RoundFightData.GamePlayData.BattleData.Contain(EGridPropID.MoveDirect, gridPosIdx);
                if (gridProp != null && !MoveDirectPropUseDict[gridProp.Idx].UseInRound)
                {
                    MoveDirectPropUseDict[gridProp.Idx].UseInRound = true;
                    var newDirectCoord = Constant.Battle.EPos2CoordMap[MoveDirectPropUseDict[gridProp.Idx].Direct];
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

        public List<int> GetFlyPaths(int startPosIdx, Vector2Int direct, int dis, EUnitActionState unitActionState)
        {
            direct = GameUtility.GetDirect(direct);
            //var startCoord = GameUtility.GridPosIdxToCoord(startPosIdx);

            var endPosIdx = GameUtility.GetEndPosIdx(startPosIdx, direct, dis);

            return GetFlyPaths(startPosIdx, endPosIdx, unitActionState);
            
            // var flyPosIdxs = new List<int>();
            // flyPosIdxs.Add(startPosIdx);
            //
            // direct = GameUtility.GetDirect(direct);
            //
            // // var signX = 0;
            // // var signY = 0;
            // // if (direct.x < 0)
            // // {
            // //     signX = -1;
            // // }
            // // else if (direct.x > 0)
            // // {
            // //     signX = 1;
            // // }
            // //
            // // if (direct.y < 0)
            // // {
            // //     signY = -1;
            // // }
            // // else if (direct.y > 0)
            // // {
            // //     signY = 1;
            // // }
            //
            //
            // var idx = 0;
            // var isMoveDirect = false;
            // var targetCoord = startCoord;
            // var lastGridPosIdx = startPosIdx;
            // while (true)
            // {
            //     targetCoord = targetCoord +
            //                   direct;
            //     
            //     if (!GameUtility.InGridRange(targetCoord))
            //         break;
            //     
            //     var gridPosIdx =
            //         GameUtility.GridCoordToPosIdx(targetCoord);
            //     
            //     if (BattleManager.Instance.BattleData.GridTypes[gridPosIdx] == EGridType.Obstacle)
            //     {
            //         break;
            //     }
            //     
            //     flyPosIdxs.Add(gridPosIdx);
            //
            //     var unit = GameUtility.GetUnitByGridPosIdx(gridPosIdx);
            //     // && unit.GetStateCount(EUnitState.UnBePass) > 0
            //     if (unit != null)
            //     {
            //         flyPosIdxs.Add(lastGridPosIdx);
            //         break;
            //     }
            //     
            //     
            //     
            //     var gridProp = RoundFightData.GamePlayData.BattleData.Contain(EGridPropID.MoveDirect, gridPosIdx);
            //     if (gridProp != null && !MoveDirectPropUseDict[gridProp.ID].UseInRound)
            //     {
            //         MoveDirectPropUseDict[gridProp.ID].UseInRound = true;
            //         var newDirectCoord = Constant.Battle.EPos2CoordMap[MoveDirectPropUseDict[gridProp.ID].Direct];
            //         direct.x = newDirectCoord.x;
            //         direct.y = newDirectCoord.y;
            //         isMoveDirect = true;
            //
            //     }
            //     idx++;
            //     
            //     lastGridPosIdx = gridPosIdx;
            // }
            //
            // return flyPosIdxs;
        }
        
        public List<int> GetFlyPaths(int startPosIdx, int endPosIdx, EUnitActionState unitActionState)
        {
            var startCoord = GameUtility.GridPosIdxToCoord(startPosIdx);
            var endCoord = GameUtility.GridPosIdxToCoord(endPosIdx);
            var direct = endCoord - startCoord;
            
            var flyPosIdxs = new List<int>();

            if (startPosIdx == endPosIdx)
            {
                return flyPosIdxs;
            }
            
            flyPosIdxs.Add(startPosIdx);
            
            
            
            // var signX = 0;
            // var signY = 0;
            // if (direct.x < 0)
            // {
            //     signX = -1;
            // }
            // else if (direct.x > 0)
            // {
            //     signX = 1;
            // }
            //
            // if (direct.y < 0)
            // {
            //     signY = -1;
            // }
            // else if (direct.y > 0)
            // {
            //     signY = 1;
            // }
            
            direct = GameUtility.GetDirect(direct);


            var idx = 0;
            //var isMoveDirect = false;
            var targetCoord = startCoord;
            var lastGridPosIdx = startPosIdx;
            while (true)
            {
                targetCoord = targetCoord +
                              direct;
                
                if (!GameUtility.InGridRange(targetCoord))
                    break;
                
                var gridPosIdx =
                    GameUtility.GridCoordToPosIdx(targetCoord);
                
                flyPosIdxs.Add(gridPosIdx);
                

                var unit = GameUtility.GetUnitByGridPosIdx(gridPosIdx);
                // && unit.GetStateCount(EUnitState.UnBePass) > 0
                if (unitActionState != EUnitActionState.Throw && ((unit != null && unit.Exist()) ||
                                                                  RoundFightData.GamePlayData.BattleData.GridTypes[
                                                                      gridPosIdx] == EGridType.Obstacle))
                {
                    flyPosIdxs.Add(lastGridPosIdx);
                    break;
                }
                if (unitActionState != EUnitActionState.Throw && unit != null && !unit.Exist())
                {
                    break;
                }
                else if (unitActionState == EUnitActionState.Throw &&
                         RoundFightData.GamePlayData.BattleData.GridTypes[gridPosIdx] == EGridType.Empty)
                {
                    break;
                }
                
                var gridProp = RoundFightData.GamePlayData.BattleData.Contain(EGridPropID.MoveDirect, gridPosIdx);
                var isMoveDirect = gridProp != null && !MoveDirectPropUseDict[gridProp.Idx].UseInRound;
                if (isMoveDirect)
                {
                    MoveDirectPropUseDict[gridProp.Idx].UseInRound = true;
                    var newDirectCoord = Constant.Battle.EPos2CoordMap[MoveDirectPropUseDict[gridProp.Idx].Direct];
                    direct.x = newDirectCoord.x;
                    direct.y = newDirectCoord.y;

                }

                if (targetCoord == endCoord && !isMoveDirect)
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
        
        

        public int ChangeHP(Data_BattleUnit unit, float value, EHPChangeType hpChangeType, bool useDefense = true, bool changeHPInstantly = false, TriggerData triggerData = null)
        {
            return BattleManager.Instance.ChangeHP(unit, (int)value, RoundFightData.GamePlayData, hpChangeType, useDefense, true, changeHPInstantly, triggerData);

        }
        
        private bool IsEnemy(int unit1Idx, int unit2Idx)
        {
            var unit1 = BattleFightManager.Instance.GetUnitByIdx(unit1Idx);
            var unit2 = BattleFightManager.Instance.GetUnitByIdx(unit2Idx);

            return unit1.UnitCamp != unit2.UnitCamp;
        }
        
        public List<int> GetEffectUnitIdxs(BuffData buffData, int ownUnitIdx, int actionUnitIdx, int effectUnitIdx, int actionUnitGridPosIdx, int actionUnitPreGridPosIdx)
        {
            var realEffectUnitIdxs = new List<int>();
            
            var actionUnit = BattleFightManager.Instance.GetUnitByIdx(actionUnitIdx);
            if (actionUnit != null && actionUnitGridPosIdx == -1)
            {
                actionUnitGridPosIdx = actionUnit.GridPosIdx;
            }
            var ownUnit = BattleFightManager.Instance.GetUnitByIdx(ownUnitIdx);
            var ownUnitGridPosIdx = -1;
            if (ownUnit != null)
            {
                ownUnitGridPosIdx = ownUnit.GridPosIdx;
            }
            else
            {
                ownUnitGridPosIdx = actionUnitGridPosIdx;

            }
            
            //var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
            var actionUnitCoord = GameUtility.GridPosIdxToCoord(actionUnitGridPosIdx);
            var actionUnitLastCoord = GameUtility.GridPosIdxToCoord(actionUnitPreGridPosIdx);
            
            var effectUnit = BattleFightManager.Instance.GetUnitByIdx(effectUnitIdx);
            var effectUnitCoord = Vector2Int.zero;
            if (effectUnit != null)
            {
                effectUnitCoord = GameUtility.GridPosIdxToCoord(effectUnit.GridPosIdx);
            }
            
            ;
            
            foreach (var triggerTarget in buffData.TriggerTargets)
            {
                switch (triggerTarget)
                {
                    case ETriggerTarget.Staff:
                        // if (buffData.BuffStr.Contains("Tactic"))
                        // {
                        //     
                        // }
                        foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
                        {
                            if(kv.Value.UnitRole != EUnitRole.Staff)
                                continue;
                                
                            if (buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy) && kv.Value.UnitCamp != EUnitCamp.Player1)
                            {
                                realEffectUnitIdxs.Add(kv.Value.Idx);
                            }
                            else if (buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us) && kv.Value.UnitCamp == EUnitCamp.Player1)
                            {
                                realEffectUnitIdxs.Add(kv.Value.Idx);
                            }
                        }
                        break;
                    
                    case ETriggerTarget.DeBuff:
                        foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
                        {
                            if(kv.Value.GetStateCountByEffectType(EUnitStateEffectType.DeBuff) <= 0)
                                continue;
                                
                            if (buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy) && kv.Value.UnitCamp != EUnitCamp.Player1)
                            {
                                realEffectUnitIdxs.Add(kv.Value.Idx);
                            }
                            else if (buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us) && kv.Value.UnitCamp == EUnitCamp.Player1)
                            {
                                realEffectUnitIdxs.Add(kv.Value.Idx);
                            }
                        }
                        break;

                    case ETriggerTarget.Effect:
                        if (effectUnitIdx != -1)
                        {
                            var isEnemy = false;
                            if (actionUnitIdx != -1 && actionUnitIdx != Constant.Battle.UnUnitTriggerIdx)
                            {
                                isEnemy = IsEnemy(actionUnitIdx, effectUnitIdx);
                            }
                            else
                            {
                                isEnemy = true;
                            }
                            
                            
                            if (isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy))
                            {
                                realEffectUnitIdxs.Add(effectUnitIdx);
                            }
                            else if (!isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us))
                            {
                                realEffectUnitIdxs.Add(effectUnitIdx);
                            }
                        }

                        break;
                    case ETriggerTarget.LineExtend:
                        if (effectUnitIdx != -1 && actionUnitIdx  != -1)
                        {
                            var coords = GameUtility.GetRelatedCoords(EActionType.LineExtend, actionUnit.GridPosIdx,
                                effectUnit.GridPosIdx);
                            
                            for (int i = 0; i < coords.Count; i++)
                            {
                                var endPosIdx = GameUtility.GridCoordToPosIdx(coords[i]);
                                if (endPosIdx != actionUnit.GridPosIdx)
                                {
                                    var endPosUnit = GameUtility.GetUnitByGridPosIdx(endPosIdx);
                                    if (endPosUnit != null)
                                    {
                                        var isEndPosEnemy = IsEnemy(actionUnitIdx, endPosUnit.Idx);
                            
                                        if (isEndPosEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy))
                                        {
                                            realEffectUnitIdxs.Add(endPosUnit.Idx);
                                            break;
                                        }
                                        else if (!isEndPosEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us))
                                        {
                                            realEffectUnitIdxs.Add(endPosUnit.Idx);
                                            break;
                                        }

                                    }
                                }
                                
                            }
                            
                            // var isEnemy = IsEnemy(actionUnitIdx, effectUnitIdx);
                            //
                            // if (isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy))
                            // {
                            //     realEffectUnitIdxs.Add(effectUnitIdx);
                            // }
                            // else if (!isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us))
                            // {
                            //     realEffectUnitIdxs.Add(effectUnitIdx);
                            // }
                        }

                        break;
                    case ETriggerTarget.Action:
                        var triggerUnitCamps = buffData.TriggerUnitCamps;
                        
                        EUnitCamp actionUnitCamp;
                        ERelativeCamp relativeCamp;
                        if (effectUnit != null)
                        {
                            if (actionUnitIdx == Constant.Battle.UnUnitTriggerIdx)
                            {
                                actionUnitCamp = EUnitCamp.Player1;
                                relativeCamp = GameUtility.GetRelativeCamp(actionUnitCamp, effectUnit.UnitCamp);
                                if (triggerUnitCamps.Contains(relativeCamp))
                                {
                                    realEffectUnitIdxs.Add(actionUnitIdx);

                                }  
                                
                            }
                            else
                            {
                                actionUnitCamp = actionUnit.UnitCamp;
                            
                                relativeCamp = GameUtility.GetRelativeCamp(actionUnitCamp, effectUnit.UnitCamp);
                                if (triggerUnitCamps.Contains(relativeCamp))
                                {
                                    realEffectUnitIdxs.Add(actionUnitIdx);

                                }
                            }
                        }
                        else if (triggerUnitCamps.Count <= 0 || triggerUnitCamps.Contains(ERelativeCamp.Empty))
                        {
                            realEffectUnitIdxs.Add(actionUnitIdx);
                        }


                        break;
                    case ETriggerTarget.Hero:
                        realEffectUnitIdxs.Add(PlayerData.BattleHero.Idx);
                        break;
                    case ETriggerTarget.All:
                        foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
                        {
                            if (buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy) && kv.Value.UnitCamp != actionUnit.UnitCamp)
                            {
                                realEffectUnitIdxs.Add(kv.Value.Idx);
                            }
                            else if (buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us) && kv.Value.UnitCamp == actionUnit.UnitCamp)
                            {
                                realEffectUnitIdxs.Add(kv.Value.Idx);
                            }
                        }
                        
                        break;
                    case ETriggerTarget.InRange:
                        var range = GameUtility.GetRange(ownUnitGridPosIdx, buffData.TriggerRange,
                            ownUnit != null ? ownUnit.UnitCamp : BattleManager.Instance.CurUnitCamp, buffData.TriggerUnitCamps);
                        foreach (var gridPosIdx in range)
                        {
                            var unit = BattleFightManager.Instance.GetUnitByGridPosIdx(gridPosIdx);
                            if (unit != null)
                            {
                                realEffectUnitIdxs.Add(unit.Idx);
                            }
                        }
                        break;
                    case ETriggerTarget.Vertical:
                        var relatedDirect2 = effectUnitCoord - actionUnitCoord;
                        relatedDirect2 = GameUtility.GetDirect(relatedDirect2);

                        var verticals = GameUtility.GetRelatedVerticalCoords(relatedDirect2, effectUnitCoord);
                        var vertical1Coord = effectUnitCoord + verticals[0];
                        var vertical2Coord = effectUnitCoord + verticals[1];
                        var vertical1GridPosIdx = GameUtility.GridCoordToPosIdx(vertical1Coord);
                        var vertical2GridPosIdx = GameUtility.GridCoordToPosIdx(vertical2Coord);
                        var vertical1Unit = BattleFightManager.Instance.GetUnitByGridPosIdx(vertical1GridPosIdx);
                        if (GameUtility.InGridRange(vertical1Coord) && vertical1Unit != null)
                        {
                            realEffectUnitIdxs.Add(vertical1Unit.Idx);
                        }
                        var vertical2Unit = BattleFightManager.Instance.GetUnitByGridPosIdx(vertical2GridPosIdx);
                        if (GameUtility.InGridRange(vertical2Coord) && vertical2Unit != null)
                        {
                            realEffectUnitIdxs.Add(vertical2Unit.Idx);
                        }

                        break;
                        // var vertical1 = new Vector2Int(-actionUnitDirect.y, actionUnitDirect.x);
                        // var vertical2 = new Vector2Int(actionUnitDirect.y, -actionUnitDirect.x);
                        // var vertical1GridPosIdx = GameUtility.GridCoordToPosIdx(actionUnitCoord + vertical1);
                        // var vertical2GridPosIdx = GameUtility.GridCoordToPosIdx(actionUnitCoord + vertical2);
                        // var vertical1Unit = BattleFightManager.Instance.GetUnitByGridPosIdx(vertical1GridPosIdx);
                        // if (vertical1Unit != null)
                        // {
                        //     realEffectUnitIdxs.Add(vertical1Unit.Idx);
                        // }
                        // var vertical2Unit = BattleFightManager.Instance.GetUnitByGridPosIdx(vertical2GridPosIdx);
                        // if (vertical2Unit != null)
                        // {
                        //     realEffectUnitIdxs.Add(vertical2Unit.Idx);
                        // }
                        

                        break;
                    case ETriggerTarget.Horizontal:

                        var relatedDirect = effectUnitCoord - actionUnitCoord;
                        relatedDirect = GameUtility.GetDirect(relatedDirect);

                        var horizontals = GameUtility.GetRelatedHorizontalCoords(relatedDirect, effectUnitCoord);
                        var horizontal1Coord = effectUnitCoord + horizontals[0];
                        var horizontal2Coord = effectUnitCoord + horizontals[1];
                        var horizontal1GridPosIdx = GameUtility.GridCoordToPosIdx(horizontal1Coord);
                        var horizontal2GridPosIdx = GameUtility.GridCoordToPosIdx(horizontal2Coord);
                        var horizontal1Unit = BattleFightManager.Instance.GetUnitByGridPosIdx(horizontal1GridPosIdx);
                        if (GameUtility.InGridRange(horizontal1Coord) && horizontal1Unit != null)
                        {
                            realEffectUnitIdxs.Add(horizontal1Unit.Idx);
                        }
                        var horizontal2Unit = BattleFightManager.Instance.GetUnitByGridPosIdx(horizontal2GridPosIdx);
                        if (GameUtility.InGridRange(horizontal2Coord) && horizontal2Unit != null)
                        {
                            realEffectUnitIdxs.Add(horizontal2Unit.Idx);
                        }

                        break;
                    case ETriggerTarget.Select:
                        realEffectUnitIdxs.Add(effectUnitIdx);
                        break;
                    case ETriggerTarget.EffectChain:
                        if (effectUnitIdx != -1 && actionUnitIdx  != -1)
                        {
                            var chains = new List<int>();
                            chains.Add(effectUnit.GridPosIdx);
                            ColllectChain(effectUnit.GridPosIdx, actionUnit.UnitCamp, buffData.TriggerUnitCamps,
                                new List<int>(){ actionUnit.GridPosIdx }, chains);

                            foreach (var chain in chains)
                            {
                                var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(chain);
                                if (unit != null)
                                {
                                    realEffectUnitIdxs.Add(unit.UnitIdx);
                                }
                            }

                            
                            
                            // var isEnemy = IsEnemy(actionUnitIdx, effectUnitIdx);
                            //
                            // if (isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Enemy))
                            // {
                            //     realEffectUnitIdxs.Add(effectUnitIdx);
                            // }
                            // else if (!isEnemy && buffData.TriggerUnitCamps.Contains(ERelativeCamp.Us))
                            // {
                            //     realEffectUnitIdxs.Add(effectUnitIdx);
                            // }
                        }

                        break;
                    default:
                        break;
                }
            }

            //GameUtility.SortHeroIDToLast(realEffectUnitIdxs);
            
            return realEffectUnitIdxs;
        }


        public void ColllectChain(int gridPosIdx, EUnitCamp unitCamp, List<ERelativeCamp> relativeCamps, List<int> exceptGridPosIdxs, List<int> chains)
        {
            var excepts = new List<int>();
            excepts.AddRange(exceptGridPosIdxs);
            excepts.AddRange(chains);
            var chainGridPosIdxs = GameUtility.GetRange(gridPosIdx, EActionType.Cross2Short, unitCamp, relativeCamps,
                true, false, excepts);
            
            
            
            foreach (var linkGridPosIdx in chainGridPosIdxs)
            {
                chains.Add(linkGridPosIdx);
                ColllectChain(linkGridPosIdx, unitCamp, relativeCamps, exceptGridPosIdxs, chains);
            }
            
        }
        
        
        
        public void StartMoveTrigger(Data_BattleUnit unit, List<TriggerData> triggerDatas)
        {

            BattleBuffManager.Instance.BuffsTrigger(RoundFightData.GamePlayData, unit, triggerDatas, EBuffTriggerType.StartMove);

        }
        
        public void KillTrigger(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            var unit = BattleFightManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
  
            BattleBuffManager.Instance.BuffsTrigger(RoundFightData.GamePlayData, unit, triggerData, triggerDatas, EBuffTriggerType.Kill);

        }

        public void DeadTrigger(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            var effectUnit = BattleFightManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
            var actionUnit = BattleFightManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
            if(effectUnit == null || effectUnit.Exist())
                return;

            
            BattleBuffManager.Instance.BuffsTrigger(RoundFightData.GamePlayData, effectUnit, triggerData, triggerDatas, EBuffTriggerType.Dead);


            TriggerUnitData(effectUnit.Idx, actionUnit == null ? -1 : actionUnit.Idx, effectUnit.GridPosIdx, EBuffTriggerType.Dead, triggerDatas);


            BlessDeadTransferUnitState(EBlessID.EnemyDeadDeBuffToEnemy, EUnitCamp.Player1, EUnitCamp.Enemy,
                EUnitCamp.Enemy, EUnitStateEffectType.DeBuff, effectUnit, triggerDatas);
            BlessDeadTransferUnitState(EBlessID.EnemyDeadDeBuffToEnemy, EUnitCamp.Enemy, EUnitCamp.Player1,
                EUnitCamp.Enemy, EUnitStateEffectType.DeBuff, effectUnit, triggerDatas);
            
            BlessDeadTransferUnitState(EBlessID.UsDeadDeBuffToEnemy, EUnitCamp.Player1, EUnitCamp.Player1,
                EUnitCamp.Enemy, EUnitStateEffectType.DeBuff, effectUnit, triggerDatas);
            BlessDeadTransferUnitState(EBlessID.UsDeadDeBuffToEnemy, EUnitCamp.Enemy, EUnitCamp.Enemy,
                EUnitCamp.Player1, EUnitStateEffectType.DeBuff, effectUnit, triggerDatas);
            
            BlessDeadTransferUnitState(EBlessID.UsDeadBuffToUs, EUnitCamp.Player1, EUnitCamp.Player1,
                EUnitCamp.Player1, EUnitStateEffectType.Buff, effectUnit, triggerDatas);
            BlessDeadTransferUnitState(EBlessID.UsDeadBuffToUs, EUnitCamp.Enemy, EUnitCamp.Enemy,
                EUnitCamp.Enemy, EUnitStateEffectType.Buff, effectUnit, triggerDatas);

            // var usDeadDeBuffToEnemy = BattleFightManager.Instance.RoundFightData.GamePlayData.GetUsefulBless(EBlessID.UsDeadDeBuffToEnemy, effectUnit.UnitCamp);
            //
            // if (actionUnit != null && usDeadDeBuffToEnemy != null)
            // {
            //     var otherEnemies = new List<Data_BattleUnit>();
            //     foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
            //     {
            //         if (kv.Value.UnitCamp != actionUnit.UnitCamp && kv.Value.Exist())
            //         {
            //             otherEnemies.Add(kv.Value);
            //         }
            //     }
            //
            //     if (otherEnemies.Count > 0)
            //     {
            //         var randomEnemyIdx = Random.Next(0, otherEnemies.Count);
            //         foreach (var kv in effectUnit.UnitStateData.UnitStates)
            //         {
            //             if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.DeBuff].Contains(kv.Value.UnitState))
            //             {
            //                 var usDeadDeBuffToEnemyTriggerData = BattleFightManager.Instance.Unit_State(triggerDatas,
            //                     effectUnit.Idx, effectUnit.Idx, otherEnemies[randomEnemyIdx].Idx, kv.Key,
            //                     kv.Value.Value,
            //                     ETriggerDataType.RoleState);
            //                 BattleBuffManager.Instance.CacheTriggerData(usDeadDeBuffToEnemyTriggerData, triggerDatas);
            //             }
            //         }
            //     }
            //
            // }
            
            
        }

        public void BlessDeadTransferUnitState(EBlessID blessID, EUnitCamp blessUnitCamp, EUnitCamp effectUnitCamp,
            EUnitCamp targetUnitCamp, EUnitStateEffectType unitStateEffectType, Data_BattleUnit effectUnit, List<TriggerData> triggerDatas)
        {
            var enemyDeadDebuffToEnemy =
                BattleFightManager.Instance.RoundFightData.GamePlayData.GetUsefulBless(blessID,
                    blessUnitCamp);
            
            if(effectUnit.UnitCamp != effectUnitCamp)
                return;

            if (enemyDeadDebuffToEnemy != null)
            {
                var otherEnemies = new List<Data_BattleUnit>();
                foreach (var kv in BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.BattleUnitDatas)
                {
                    if (kv.Value.UnitCamp == targetUnitCamp && kv.Value.Exist())
                    {
                        otherEnemies.Add(kv.Value);
                    }
                }

                // BlessDeadTransferUnitState(otherEnemies, effectUnit, triggerDatas);
                if (otherEnemies.Count > 0)
                {
                    var randomEnemyIdx = Random.Next(0, otherEnemies.Count);
                    foreach (var kv in effectUnit.UnitStateData.UnitStates)
                    {
                        if (Constant.Battle.EffectUnitStates[unitStateEffectType].Contains(kv.Value.UnitState))
                        {
                            var triggerData = BattleFightManager.Instance.Unit_State(triggerDatas,
                                effectUnit.Idx, effectUnit.Idx, otherEnemies[randomEnemyIdx].Idx, kv.Key, kv.Value.Value,
                                ETriggerDataType.State);
                            BattleBuffManager.Instance.CacheTriggerData(triggerData, triggerDatas);
                        }

                    }
                }
            }
        }

        public void BlessDeadTransferUnitState(EUnitStateEffectType unitStateEffectType, List<Data_BattleUnit> otherEnemies, Data_BattleUnit unit, List<TriggerData> triggerDatas)
        {
            if (otherEnemies.Count > 0)
            {
                var randomEnemyIdx = Random.Next(0, otherEnemies.Count);
                foreach (var kv in unit.UnitStateData.UnitStates)
                {
                    if (Constant.Battle.EffectUnitStates[unitStateEffectType].Contains(kv.Value.UnitState))
                    {
                        var triggerData = BattleFightManager.Instance.Unit_State(triggerDatas,
                            unit.Idx, unit.Idx, otherEnemies[randomEnemyIdx].Idx, kv.Key, kv.Value.Value,
                            ETriggerDataType.State);
                        BattleBuffManager.Instance.CacheTriggerData(triggerData, triggerDatas);
                    }

                }
            }
        }

        public void CollideTrigger(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            var unit = BattleFightManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
  
            BattleBuffManager.Instance.BuffsTrigger(RoundFightData.GamePlayData, unit, triggerData, triggerDatas, EBuffTriggerType.Collide);

        }

        public void AddDeBuffTrigger(TriggerData triggerData, List<TriggerData> triggerDatas)
        {
            var effectUnit = BattleFightManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
            
            if (effectUnit == null || !effectUnit.Exist())
                return;


            BattleBuffManager.Instance.BuffsTrigger(RoundFightData.GamePlayData, effectUnit, triggerData, triggerDatas,
                EBuffTriggerType.AddDeBuff);
        }

        
        
        private Dictionary<int, EGridType> curObstacleMask = new Dictionary<int, EGridType>();

        private void RefreshObstacleMask()
        {
            curObstacleMask.Clear();
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
                curObstacleMask[kv.Value.GridPosIdx] =  EGridType.Unit;
            }
            
            // foreach (var kv in  BattleUnitDatas)
            // {
            //     if (kv.Value.UnitCamp == (unitCamp == EUnitCamp.Enemy ? EUnitCamp.Third : EUnitCamp.Enemy) || kv.Value.UnitCamp == EUnitCamp.Player1 || kv.Value.UnitCamp == EUnitCamp.Player2)
            //     {
            //         curObstacleMask[kv.Value.GridPosIdx] =  EGridType.Unit;    
            //     }
            // }
            //
            // foreach (var enemyGridPosIdx in obstacleEnemies)
            // {
            //     curObstacleMask[enemyGridPosIdx] =  EGridType.Unit;
            // }

            var playerData = RoundFightData.GamePlayData.GetPlayerData(EUnitCamp.Player1);
            curObstacleMask[playerData.BattleHero.GridPosIdx] =  EGridType.Unit; 
        }

        
        
        public int GetCollisionHurt(EUnitActionState unitActionState)
        {
            switch (unitActionState)
            {
                case EUnitActionState.Fly:
                    return Constant.Battle.FlyHurt;
                case EUnitActionState.Rush:
                    return Constant.Battle.RushHurt;
                default:
                    return 0; 
            }

        }
        
        public Data_BattleUnit GetUnitByGridPosIdx(int gridPosIdx, EUnitCamp? selfUnitCamp = null,
            ERelativeCamp? unitCamp = null, EUnitRole? unitRole = null, int exceptUnitIdx = -1)
        {

            return InternalGetUnitByGridPosIdx(BattleUnitDatas, gridPosIdx, selfUnitCamp, unitCamp, unitRole,
                exceptUnitIdx);
        }
        
        public List<Data_BattleUnit> GetUnitsByCamp(EUnitCamp? selfUnitCamp = null, ERelativeCamp? unitCamp = null)
        {
            var units = new List<Data_BattleUnit>();
            foreach (var kv in BattleUnitDatas)
            {
                if(!kv.Value.Exist())
                    continue;
                
                if (unitCamp == ERelativeCamp.Us && kv.Value.UnitCamp == selfUnitCamp ||
                    unitCamp == ERelativeCamp.Enemy && kv.Value.UnitCamp != selfUnitCamp || 
                    unitCamp == null)
                {
                    units.Add(kv.Value);
                }
            }
            
            return units;
            
            
            
        }

        public List<Data_BattleUnit> GetUnitByGridPosIdx(int actionUnitGridPosIdx, int effectUnitGridPosIdx, EActionType actionType)
        {
            var actionUnitCoord = GameUtility.GridPosIdxToCoord(actionUnitGridPosIdx);
            var effectUnitCoord = GameUtility.GridPosIdxToCoord(effectUnitGridPosIdx);

            var pointList = new List<List<Vector2Int>>();
            if (Constant.Battle.DynamicRelatedUnitFlyRanges.Contains(actionType))
            {
                pointList.Add(new List<Vector2Int>(){ new Vector2Int(0, 0)} );
                pointList.Add(GameUtility.GetRelatedCoords(actionType, actionUnitGridPosIdx, effectUnitGridPosIdx));
            }
            else
            {
                pointList = Constant.Battle.ActionTypePoints[actionType];
            }

            var units = new List<Data_BattleUnit>();

            var idx = 0;
            foreach (var points in pointList)
            {
                idx++;
                if(idx == 1)
                    continue;
                
                foreach (var point in points)
                {
                    var targetCoord = effectUnitCoord + point;
                    var targetGridPosIdx = GameUtility.GridCoordToPosIdx(targetCoord);
                    var unit = GetUnitByGridPosIdx(targetGridPosIdx);
                    if (unit != null)
                    {
                        units.Add(unit);
                    }
                    
                }
            }

            return units;
        }

        private Data_BattleUnit InternalGetUnitByGridPosIdx(Dictionary<int, Data_BattleUnit> battleUnitDatas,
            int gridPosIdx, EUnitCamp? selfUnitCamp = null, ERelativeCamp? unitCamp = null, EUnitRole? unitRole = null,
            int exceptUnitID = -1)
        {
            foreach (var kv in BattleUnitDatas)
            {
                if(kv.Value.GridPosIdx != gridPosIdx)
                    continue;
                
                if (kv.Value.Idx == exceptUnitID)
                    continue;
                
                if (unitCamp == ERelativeCamp.Us && selfUnitCamp != kv.Value.UnitCamp)
                    continue;
                
                if (unitCamp == ERelativeCamp.Enemy && selfUnitCamp == kv.Value.UnitCamp)
                    continue;

                if (unitRole != null && kv.Value.UnitRole != unitRole)
                    continue;
                
                return kv.Value;
            }

            return null;
            // if (BattleUnitDatasByGridPosIdx.ContainsKey(gridPosIdx))
            // {
            //     var unit = BattleUnitDatasByGridPosIdx[gridPosIdx];
            //     if (unitCamp == ERelativeCamp.Us && selfUnitCamp != unit.UnitCamp)
            //         return null;
            //
            //     if (unitCamp == ERelativeCamp.Enemy && selfUnitCamp == unit.UnitCamp)
            //         return null;
            //
            //     if (unitRole != null && unit.UnitRole != unitRole)
            //     {
            //         return null;
            //     }
            //
            //     if (unit.Idx == exceptUnitID)
            //     {
            //         return null;
            //     }
            //
            //     return unit;
            // }
            // else
            // {
            //     return null;
            // }

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

        public Data_BattleUnit GetUnitByIdx(int idx)
        {
            if (BattleUnitDatas.ContainsKey(idx))
            {
                return BattleUnitDatas[idx];
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

        public Data_BattleUnit GetUnitByGridPosIdxMoreCamps(int gridPosIdx, EUnitCamp selfUnitCamp = EUnitCamp.Empty,
            List<ERelativeCamp> unitCamps = null)
        {
            if (unitCamps == null || selfUnitCamp == EUnitCamp.Empty)
            {
                var unit = GetUnitByGridPosIdx(gridPosIdx);
                if (unit != null)
                {
                    return unit;
                }
            }
            else
            {
                foreach (var unitCamp in unitCamps)
                {
                    var unit = GetUnitByGridPosIdx(gridPosIdx, selfUnitCamp, unitCamp);
                    if (unit != null)
                    {
                        return unit;
                    }
                }
            }

            
            return null;
            
        }

    }
}
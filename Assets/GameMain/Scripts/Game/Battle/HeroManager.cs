using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    public enum EHPDeltaOwnerType
    {
        Enemy,
        Solider,
        Card,
        Bless,
        Fune,
    }
    
    public enum EHPDeltaType
    {
        AttackCore,
        RecoverCore,
        Core,
        AttackSolider,
        Collision,
        UseCard,
        Bless,
        Fune,
        HurtRoundStart,
    }
        
        
    public class HPDeltaData
    {
        //public EHPDeltaOwnerType HPDeltaOwnerType;
        public EHPDeltaType HPDeltaType;
        public int HPDelta;
            
    }
    public class EnemyHPDeltaData : HPDeltaData
    {
        public int EnemyIdx;
    }
        
    public class SoliderHPDeltaData : HPDeltaData
    {
        public int SoliderIdx;
    }
    
    public class CoreHPDeltaData : HPDeltaData
    {
        public int CoreIdx;
    }
        
    public class CardHPDeltaData : HPDeltaData
    {
        public int CardIdx;
    }
        
    public class BlessHPDeltaData : HPDeltaData
    {
        public int BlessIdx;
    }
        
    public class FuneHPDeltaData : HPDeltaData
    {
        public int FuneIdx;
    }
    
    public class CollisionDeltaData : HPDeltaData
    {
        public int ActionUnitIdx;
        public int EffectUnitIdx;
    }

    
    public class HeroManager : Singleton<HeroManager>
    {
        public Data_BattleHero BattleHeroData
        {
            get
            {
                return BattlePlayerManager.Instance.PlayerData?.BattleHero;
            }
            
            set
            {
                BattlePlayerManager.Instance.PlayerData.BattleHero = value;
            }

        }

        public Dictionary<EUnitCamp, Data_BattleHero> BattleHeroDatas = new();


        //private int id;
        public Random Random;
        private int randomSeed;
        
        //public Dictionary<EUnitCamp, BattleHeroEntity> HeroEntities = new ();
        
        //public BattleHeroEntity HeroEntity =>  HeroEntities[PlayerManager.Instance.PlayerData.UnitCamp];

        // public Data_BattleHero GetHeroData(EUnitCamp unitCamp)
        // {
        //     if (BattleHeroDatas.ContainsKey(unitCamp))
        //         return BattleHeroDatas[unitCamp];
        //
        //     return null;
        // }
        
        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
            //HeroEntities.Clear();
            //BattleUnitManager.Instance.BattleUnitDatas.Add(HeroManager.Instance.BattleHeroData.Idx, HeroManager.Instance.BattleHeroData);
            
            //id = 0;
        }

        public int GetAllCurHP()
        {
            //(BattleHeroData.CurHeart - 1)
            return BattleHeroData.CurHeart * BattleHeroData.MaxHP + BattleHeroData.CurHP;
        }

        public void Destory()
        {
            //HeroEntities.Clear();
        }

        // public int GetID()
        // {
        //     return id++;
        // }

        public void InitHeroData(EHeroID heroID)
        {
            BattleHeroData = new Data_BattleHero(BattleUnitManager.Instance.GetIdx(),
                heroID, 0, BattleManager.Instance.CurUnitCamp, new List<int>(), BattleManager.Instance.BattleData.Round);
            BattleHeroData.UnitRole = EUnitRole.Core;
            if (BattleManager.Instance.BattleData.GameDifficulty >= EGameDifficulty.Difficulty4)
            {
                BattleHeroData.BaseMaxHP -= 2;
                BattleHeroData.CurHP = BattleHeroData.BaseMaxHP;
            }
        }

        public void Start()
        {
            InitHeroData(0);
        }

        public async Task GenerateHero()
        {
            // BattleAreaManager.Instance.RefreshObstacles();
            //
            // //var places = BattleAreaManager.Instance.GetPlaces();
            // // var randoms = MathUtility.GetRandomNum(1, 0,
            // //     places.Count, Random);
            // //BattleHeroData.GridPosIdx = places[center];
            // BattleHeroData.GridPosIdx = GameUtility.GridCoordToPosIdx(new Vector2Int(3, 3));
            //
            // var heroEntity = await GameEntry.Entity.ShowBattleHeroEntityAsync(BattleHeroData);
            //
            // if (heroEntity is IMoveGrid moveGrid)
            // {
            //     BattleAreaManager.Instance.MoveGrids.Add(heroEntity.BattleHeroEntityData.Id, moveGrid);
            // }
            //
            // BattleUnitManager.Instance.BattleUnitEntities.Add(heroEntity.BattleHeroEntityData.BattleHeroData.Idx, heroEntity);
            // //PlayerManager.Instance.GetPlayerID(BattleManager.Instance.CurUnitCamp)
            // HeroEntities.Add(BattleManager.Instance.CurUnitCamp, heroEntity);
            
            
        }
        
        // public bool IsHero(int gridPosIdx)
        // {
        //     return HeroEntity.GridPosIdx == gridPosIdx;
        // }
        
        public void ChangeHP(int deltaHP, EHPChangeType hpChangeType, bool useDefense = true, bool addHeroHP = false, bool changeHPInstantly = false)
        {
            BattleEnergyBuffManager.Instance.HeroCurHPChanged(BattleHeroData, deltaHP);
            
            BattleManager.Instance.ChangeHP(BattleHeroData, deltaHP, GamePlayManager.Instance.GamePlayData, hpChangeType, useDefense, addHeroHP, changeHPInstantly);
            
            BattleManager.Instance.ShowGameOver();
            
            
            
            // var curEnergy = BattleHeroData.Attribute.GetAttribute(EHeroAttribute.CurEnergy);
            // var maxEnergy = BattleHeroData.Attribute.GetAttribute(EHeroAttribute.MaxEnergy);
            //
            // BattleHeroData.Attribute.SetAttribute(EHeroAttribute.CurEnergy,
            //     curEnergy + deltaHP);
            // curEnergy = BattleHeroData.Attribute.GetAttribute(EHeroAttribute.CurEnergy);
            //
            // BattleHeroData.Attribute.SetAttribute(EHeroAttribute.CurEnergy,  curEnergy > maxEnergy
            //     ? maxEnergy
            //     : curEnergy);
            // curEnergy = BattleHeroData.Attribute.GetAttribute(EHeroAttribute.CurEnergy);
            //
            // BattleHeroData.Attribute.SetAttribute(EHeroAttribute.CurEnergy, curEnergy < 0
            //     ? 0
            //     : curEnergy);
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        }
        
        public void RecoverEnergy()
        {
            //ChangeHP((int)BattleHeroData.Attribute.GetAttribute(EHeroAttribute.RecoverEnergy));
   
        }
        
        public void ClearEnergy()
        {
            // BattleHeroData.Attribute.SetAttribute(EHeroAttribute.CurEnergy,
            //     0);
            
        }

        public bool IsHero(int unitID)
        {
            var unit = BattleUnitManager.Instance.GetUnitByIdx(unitID);
            if (unit == null)
                return false;

            var playerData = GamePlayManager.Instance.GamePlayData.GetPlayerData(unit.UnitCamp);

            if (playerData == null)
                return false;

            if (playerData.BattleHero == null)
                return false;

            return playerData.BattleHero.Idx == unitID;
        }
        
        public List<float> GetHeroBuffValues()
        {
            return GetHeroBuffValues(BattleHeroData.HeroID);

        }
        
        public List<BuffData> GetBuffData(EHeroID heroID)
        {
            var drEnemy = RoundHero.GameEntry.DataTable.GetHero(heroID);
            
            var buffDatas = new List<BuffData>();

            foreach (var buffID in drEnemy.Buffs)
            {
                var buffData = BattleBuffManager.Instance.GetBuffData(buffID);
                buffData.BuffEquipType = EBuffEquipType.Normal;
                buffDatas.Add(buffData);
            }

            return buffDatas;
        }
        
        public List<float> GetHeroBuffValues(EHeroID heroID)
        {
            var values = new List<float>();
            
            var drHero = GameEntry.DataTable.GetHero(heroID);
            
            foreach (var value in drHero.Values1)
            {
                var targerValue = BattleBuffManager.Instance.GetBuffValue(value);
                values.Add(targerValue);

            }

            return values;

        }
        
        public List<List<float>> GetBuffValues(EHeroID heroID)
        {
            var drHero = GameEntry.DataTable.GetHero(heroID);
            
            var valuelist = new List<List<float>>();

            var idx = 1;
            foreach (var buffID in drHero.Buffs)
            {
                var values = new List<float>();
                foreach (var value in drHero.GetValues(idx++))
                {
                    var targetValue = BattleBuffManager.Instance.GetBuffValue(value);
                    values.Add(targetValue);
                }
                valuelist.Add(values);
            }

            return valuelist;
        }

        public void RoundStartTrigger()
        {
            var drHero = GameEntry.DataTable.GetHero(BattleHeroData.HeroID);
            foreach (var buffID in drHero.Buffs)
            {
                var buffData = BattleBuffManager.Instance.GetBuffData(buffID);
                if (buffData.BuffTriggerType == EBuffTriggerType.AutoAttack)
                {
                    BattleHeroData.UnitStateData.AddState(EUnitState.AutoAtk);
                    break;
                }
            }
            
        }
        


        public void UpdateCacheHPDelta()
        {
            BattleManager.Instance.ChangeHP(BattleHeroData, BattleHeroData.CacheHPDelta, GamePlayManager.Instance.GamePlayData,
                EHPChangeType.Action, false, false, true);

            BattleHeroData.CacheHPDelta = 0;
            
            BattleManager.Instance.RefreshView();
            BattleManager.Instance.ShowGameOver();
        }

        public HPDeltaData AddHPDelta(TriggerData triggerData)
        {

            if (!(triggerData.TriggerDataType == ETriggerDataType.Atrb &&
                  triggerData.BattleUnitAttribute == EUnitAttribute.HP) &&
                !(triggerData.TriggerDataType == ETriggerDataType.HeroAtrb &&
                  triggerData.HeroAttribute == EHeroAttribute.HP))
            {
                return null;
            }

            HPDeltaData hpDeltaData = null;

            if (triggerData.TriggerDataSubType == ETriggerDataSubType.Unit)
            {
                var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                if (actionUnit is BattleMonsterEntity)
                {
                    hpDeltaData = new EnemyHPDeltaData();
                    //hpDeltaData.HPDeltaOwnerType = EHPDeltaOwnerType.Enemy;
                    (hpDeltaData as EnemyHPDeltaData).EnemyIdx = actionUnit.UnitIdx;
                }
                else if (actionUnit is BattleSoliderEntity)
                {
                    hpDeltaData = new SoliderHPDeltaData();
                    //hpDeltaData.HPDeltaOwnerType = EHPDeltaOwnerType.Solider;
                    (hpDeltaData as SoliderHPDeltaData).SoliderIdx = actionUnit.UnitIdx;
                }
                else if (actionUnit is BattleCoreEntity)
                {
                    hpDeltaData = new CoreHPDeltaData();
                    //hpDeltaData.HPDeltaOwnerType = EHPDeltaOwnerType.Solider;
                    (hpDeltaData as CoreHPDeltaData).CoreIdx = actionUnit.UnitIdx;
                }
                
                
                var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
                if (effectUnit is BattleSoliderEntity)
                {
                    
                    hpDeltaData.HPDeltaType = EHPDeltaType.AttackSolider;
                    
                }
                else if (effectUnit is BattleCoreEntity)
                {
                    if (triggerData.ActualValue > 0)
                    {
                        hpDeltaData.HPDeltaType = EHPDeltaType.RecoverCore;
                    }
                    else if (triggerData.ActualValue < 0)
                    {
                        hpDeltaData.HPDeltaType = EHPDeltaType.AttackCore;
                    }
                    else
                    {
                        hpDeltaData.HPDeltaType = EHPDeltaType.Core;
                    }

                }
            }
            else if (triggerData.TriggerDataSubType == ETriggerDataSubType.Bless)
            {
                hpDeltaData = new BlessHPDeltaData();
                //hpDeltaData.HPDeltaOwnerType = EHPDeltaOwnerType.Bless;
                hpDeltaData.HPDeltaType = EHPDeltaType.Bless;
                (hpDeltaData as BlessHPDeltaData).BlessIdx = triggerData.TriggerBlessIdx;
            }
            else if (triggerData.TriggerDataSubType == ETriggerDataSubType.Fune)
            {
                hpDeltaData = new FuneHPDeltaData();
                //hpDeltaData.HPDeltaOwnerType = EHPDeltaOwnerType.Fune;
                hpDeltaData.HPDeltaType = EHPDeltaType.Fune;
                (hpDeltaData as FuneHPDeltaData).FuneIdx = triggerData.TriggerFuneIdx;
            }
            else if (triggerData.TriggerDataSubType == ETriggerDataSubType.Card)
            {
                hpDeltaData = new CardHPDeltaData();
                //hpDeltaData.HPDeltaOwnerType = EHPDeltaOwnerType.Card;
                hpDeltaData.HPDeltaType = EHPDeltaType.UseCard;
                (hpDeltaData as CardHPDeltaData).CardIdx = triggerData.TriggerCardIdx;
            }
            else if (triggerData.TriggerDataSubType == ETriggerDataSubType.Collision)
            {
                hpDeltaData = new CollisionDeltaData();
                //hpDeltaData.HPDeltaOwnerType = EHPDeltaOwnerType.Card;
                hpDeltaData.HPDeltaType = EHPDeltaType.Collision;
                (hpDeltaData as CollisionDeltaData).ActionUnitIdx = triggerData.ActionUnitIdx;
                (hpDeltaData as CollisionDeltaData).EffectUnitIdx = triggerData.EffectUnitIdx;
            }
            else if (triggerData.TriggerDataSubType == ETriggerDataSubType.State)
            {
                hpDeltaData = new HPDeltaData();
                //hpDeltaData.HPDeltaOwnerType = EHPDeltaOwnerType.Card;
                if (triggerData.UnitStateDetail.UnitState == EUnitState.HurtRoundStart)
                {
                    hpDeltaData.HPDeltaType = EHPDeltaType.HurtRoundStart;
                }
                
                
            }

            //
            hpDeltaData.HPDelta = (int)triggerData.ActualValue;

            return hpDeltaData; 
        }


    }
}
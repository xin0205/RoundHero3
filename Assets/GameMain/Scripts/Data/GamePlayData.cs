using System;
using System.Collections.Generic;
using System.Linq;
using CatJson;
using JetBrains.Annotations;

namespace RoundHero
{
    public class Data_Block
    {
        public int ID;
        public int RandomSeed;
        public int PosIdx;
        public EBlockType BlockType;
        public bool IsCurse = false;

        public Data_Block()
        {
        }

        public Data_Block(int id, int posIdx, EBlockType blockType, int randomSeed)
        {
            ID = id;
            PosIdx = posIdx;
            BlockType = blockType;
            RandomSeed = randomSeed;
        }

        public virtual Data_Block Copy()
        {
            var dataItem = new Data_Block();
            dataItem.BlockType = BlockType;
            dataItem.PosIdx = PosIdx;
            dataItem.IsCurse = IsCurse;
            return dataItem;
        }

    }

    public class Data_BlockEnemy : Data_Block
    {
        public int EnemyID;

        public Data_BlockEnemy()
        {
        }

        public Data_BlockEnemy(int posIdx, int id, int enemyID, int randomSeed) : base(id, posIdx, EBlockType.Enemy,
            randomSeed)
        {
            EnemyID = enemyID;
        }

        public override Data_Block Copy()
        {
            var dataBlockEnemy = new Data_BlockEnemy(PosIdx, ID, EnemyID, RandomSeed);

            return dataBlockEnemy;
        }

    }

    // public class Data_FightCard : Data_Card
    // {
    //     public EBuffID BuffID { get; }
    //
    //     public Data_FightCard(int id, EBuffID buffID) : base(id)
    //     {
    //         ID = id;
    //         BuffID = buffID;
    //     }
    //     
    //     public Data_FightCard Copy()
    //     {
    //         var dataCard = new Data_FightCard(ID, BuffID);
    //
    //         return dataCard;
    //     }
    //     
    // }

    // public class Data_TacticCard : Data_Card
    // {
    //     public ETacticCardID TacticCardID { get; }
    //
    //     public Data_TacticCard(int id, ETacticCardID tacticCardID) : base(id)
    //     {
    //         ID = id;
    //         TacticCardID = tacticCardID;
    //     }
    //     
    //     public Data_TacticCard Copy()
    //     {
    //         var dataCard = new Data_TacticCard(ID, TacticCardID);
    //
    //         return dataCard;
    //     }
    //
    // }
    public enum ECardDestination
    {
        Pass,
        Consume,
        StandBy,
    }
        

    public class Data_Card
    {
        public int CardIdx;
        public int CardID;
        public int MaxFuneCount;
        public List<int> FuneIdxs = new();
        public int RoundEnergyDelta = 0;
        public List<ELinkID> RoundLinkIDs = new();
        public int UseCardDamageRatio = 0;
        public bool UnUse = false;
        //public bool IsUseConsume = false;
        public ECardDestination CardDestination = ECardDestination.Pass;
        public ECardUseType CardUseType;
        public int EnergyDelta = 0;
        public int MaxHPDelta = 0;
        public int DmgDelta = 0;
        
        public Data_Card()
        {

        }

        public Data_Card(int cardIdx, int cardID, [CanBeNull] List<int> funeIdxs = null)
        {
            CardIdx = cardIdx;
            CardID = cardID;
            if (funeIdxs != null)
            {
                FuneIdxs = funeIdxs;
            }

            MaxFuneCount = Constant.Card.InitFuneCount;

        }

        public virtual Data_Card Copy()
        {
            var dataCard = new Data_Card(CardIdx, CardID, FuneIdxs);

            return dataCard;
        }

        //, bool unUse = false
        public int FuneCount(EBuffID funeBuffID)
        {
            var count = 0;
            foreach (var funeIdx in FuneIdxs)
            {
                var funeData = FuneManager.Instance.GetFuneData(funeIdx);
                var drBuff = GameEntry.DataTable.GetBuff(funeData.FuneID);
                
                //unUse && drBuff.BuffIDs.Contains(funeBuffID.ToString()) || !unUse
                if (drBuff.BuffIDs.Contains(funeBuffID.ToString()))
                {
                    count += 1;
                }
                
                // if (GameUtility.StringToEnum<EBuffID>(drBuff.BuffID).Contains(funeBuffID)）
                // {
                //     if (unUse && funeData.Value > 0 || !unUse)
                //     {
                //         count += 1;
                //     }
                // }

            }

            return count;
        }

        public void RoundClear()
        {
            RoundEnergyDelta = 0;
            RoundLinkIDs.Clear();
            UnUse = false;
        }

    }

    public class Data_Hero
    {
        public int PosIdx;
        public Attribute Attribute = new();
        
        public Data_Hero()
        {


        }

        public Data_Hero Copy()
        {
            var dataHero = new Data_Hero();
            dataHero.PosIdx = PosIdx;
            dataHero.Attribute = Attribute.Copy();
            
            
            return dataHero;

        }
    }

    public class Data_GridItem
    {
        public int Idx;
        public int GridPosIdx;

        //public EUnitCamp UnitCamp;
        public EUnitCamp UnitCamp;
        //public EGridType TargetGridType;

        public Data_GridItem()
        {

        }

        //, EGridType gridType
        public Data_GridItem(int idx, int gridPosIdx, EUnitCamp unitCamp)
        {
            Idx = idx;
            GridPosIdx = gridPosIdx;
            UnitCamp = unitCamp;
            //TargetGridType = gridType;
        }

        public Data_GridItem Copy()
        {
            var dataGridItem = new Data_GridItem();
            dataGridItem.Idx = Idx;
            dataGridItem.GridPosIdx = GridPosIdx;
            dataGridItem.UnitCamp = UnitCamp;
            //dataGridItem.TargetGridType = TargetGridType;

            return dataGridItem;
        }

    }

    public class Data_Fune
    {
        public int Idx;
        public int FuneID;
        public int Value;
        
        public Data_Fune()
        {

        }

        public Data_Fune(int idx, int funeID, int value = 0)
        {
            Idx = idx;
            FuneID = funeID;
            Value = value;
        }
        
        public Data_Fune Copy()
        {
            var dataBless = new Data_Fune();
            dataBless.Idx = Idx;
            dataBless.FuneID = FuneID;
            dataBless.Value = Value;

            return dataBless;
        }
        
        // public Data_Fune(int idx, int funeID, int value = 0)
        // {
        //     var drBuff = GameEntry.DataTable.GetBuff(funeID);
        //     
        //     Idx = idx;
        //     FuneID = Enum.Parse<EBuffID>(drBuff.BuffID);
        //     Value = value;
        // }
    }

    public class UnitStateDetail
    {
        public EUnitState UnitState = EUnitState.Empty;
        public int Value = 0;
        public EEffectType EffectType = EEffectType.Default;

        public UnitStateDetail()
        {
            
        }
        
        public UnitStateDetail(UnitStateDetail unitStateDetail)
        {

            UnitState = unitStateDetail.UnitState;
            Value = unitStateDetail.Value;
            EffectType = unitStateDetail.EffectType;

        }

        public UnitStateDetail Copy()
        {
            var unitStateDetail = new UnitStateDetail();
            unitStateDetail.UnitState = UnitState;
            unitStateDetail.Value = Value;
            unitStateDetail.EffectType = EffectType;

            return unitStateDetail;
        }
        
    }

    public class UnitStateData
    {
        public Dictionary<EUnitState, UnitStateDetail> UnitStates = new();
        
        public void AddState(EUnitState state, int value = 1, EEffectType effectType = EEffectType.Default)
        {
            if (UnitStates.ContainsKey(state))
            {
                UnitStates[state].Value += value;
            }
            else
            {
                UnitStates.Add(state, new UnitStateDetail()
                {
                    UnitState = state,
                    Value = value,
                    EffectType = effectType,
                });
            }
            CheckStateCount(state);
        }

        public void RemoveState(EUnitState state, int count = 1)
        {
            if (!UnitStates.ContainsKey(state))
                return;
            
            if(UnitStates[state].EffectType == EEffectType.Forever)
                return;

            // if (isAll)
            // {
            //     UnitStates[state] = 0;
            // }
            // else
            // {
            //     
            // }
            UnitStates[state].Value -= count;
            CheckStateCount(state);

        }

        private void CheckStateCount(EUnitState state)
        {
            if(UnitStates[state] == null)
                return;
            
            UnitStates[state].Value = UnitStates[state].Value < 0 ? 0 : UnitStates[state].Value;

            if (UnitStates[state].Value <= 0)
            {
                UnitStates.Remove(state);
            }
        }

        public int GetStateCount(EUnitState state)
        {
            if (UnitStates.ContainsKey(state))
            {
                return UnitStates[state].Value;
            }

            return 0;
        }
        
        public int GetStateCountByEffectType(EUnitStateEffectType effectType)
        {
            var count = 0;
            foreach (var kv in UnitStates)
            {
                if (Constant.Battle.EffectUnitStates[effectType].Contains(kv.Key))
                {
                    count += kv.Value.Value;
                }
                
                
            }

            return count;

        }
        
        public UnitStateData Copy()
        {
            var unitStateData = new UnitStateData();
            unitStateData.UnitStates = new Dictionary<EUnitState, UnitStateDetail>();
            foreach (var kv in UnitStates)
            {
                unitStateData.UnitStates.Add(kv.Key, kv.Value.Copy());
            }

            return unitStateData;
        }
    }

    // public interface IBattleUnit
    // {
    //     
    // }
    
    public class Data_BattleUnit : Data_GridItem
    {
        public virtual int CurHP { get; set; }
        public virtual int MaxHP { get => BaseMaxHP + FuneCount(EBuffID.Spec_AddMaxHP);}
        public int BaseMaxHP { get; set; }
        public UnitStateData UnitStateData = new();
        public bool AttackInRound;
        public EUnitRole UnitRole;
        public int BaseDamage;
        public int HurtTimes;
        public int LastCurHP;
        public int LastCurHPDelta;
        public List<int> FuneIdxs = new();
        public List<ELinkID> LinkIDs = new();
        public List<ELinkID> BattleLinkIDs = new List<ELinkID>();
        public List<int> Links = new();
        public int RoundGridMoveCount = 0;
        public UnitStateData RoundUnitState = new();
        public int AddHeroHP = 0;
        public int RoundMoveTimes = 0;
        public int RoundAttackTimes = 0;
        public int CacheHPDelta;

        //, EGridType.Unit
        public Data_BattleUnit(int idx, int gridPosIdx, EUnitCamp unitCamp, List<int> funeIdxs) : base(idx, gridPosIdx,
            unitCamp)
        {
            Idx = idx;
            GridPosIdx = gridPosIdx;
            UnitCamp = unitCamp;
            FuneIdxs = funeIdxs;
        }

        public Data_BattleUnit()
        {

        }

        public new Data_BattleUnit Copy()
        {
            var dataBattleUnit = new Data_BattleUnit();
            dataBattleUnit.Idx = Idx;
            dataBattleUnit.GridPosIdx = GridPosIdx;

            return dataBattleUnit;
        }

        public void ChangeState(EUnitState state, int count = 1)
        {
            // if(BuffCount(EBuffID.UnGetDebuff) > 0 && Constant.Battle.EffectUnitStates[EUnitStateEffectType.Negative].Contains(state))
            //     return;


            if (GetStateCount(EUnitState.DeBuffUnEffect) > 0  && Constant.Battle.EffectUnitStates[EUnitStateEffectType.DeBuff].Contains(state))
            {
                RemoveState(EUnitState.DeBuffUnEffect, -1);
                return;
            }

            UnitStateData.AddState(state, count);
        }

        public void RemoveState(EUnitState state, int count = 1)
        {
            if (RoundUnitState.GetStateCount(state) > 0)
            {
                RoundUnitState.RemoveState(state, count);
            }
            else if (UnitStateData.GetStateCount(state) > 0)
            {
                UnitStateData.RemoveState(state, count);
            }

        }
        
        public void RemoveAllState(List<EUnitStateEffectType> buffEffectTypes = null)
        {
            RemoveAllState(RoundUnitState, buffEffectTypes);
            RemoveAllState(UnitStateData, buffEffectTypes);

        }

        private void RemoveAllState(UnitStateData unitStateData, List<EUnitStateEffectType> buffEffectTypes = null)
        {
            foreach (var unitState in unitStateData.UnitStates.Keys.ToList())
            {
                if (buffEffectTypes != null)
                {
                    foreach (var buffEffectType in buffEffectTypes)
                    {
                        if (Constant.Battle.EffectUnitStates[buffEffectType].Contains(unitState))
                        {
                            unitStateData.RemoveState(unitState, unitStateData.GetStateCount(unitState));
                        }
                    }
                }
                else
                {
                    unitStateData.RemoveState(unitState, unitStateData.GetStateCount(unitState));
                }
            }
        }

        public int GetAllStateCount(EUnitState state)
        {
            return UnitStateData.GetStateCount(state) + RoundUnitState.GetStateCount(state);
        }
        
        public int GetStateCount(EUnitState state)
        {
            return UnitStateData.GetStateCount(state);
        }
        
        public int GetStateCountByEffectType(EUnitStateEffectType effectType)
        {
            return UnitStateData.GetStateCountByEffectType(effectType);
        }
        
        public int GetRoundStateCount(EUnitState state)
        {
            return RoundUnitState.GetStateCount(state);
        }
        
        public void ChangeRoundState(EUnitState state, int count = 1)
        {
            RoundUnitState.AddState(state, count);
        }

        public void RoundRemoveState(EUnitState state, int count = 1)
        {
            RoundUnitState.RemoveState(state, count);
        }

        public int RoundGetStateCount(EUnitState state)
        {
            return RoundUnitState.GetStateCount(state);
        }

        public virtual void ChangeHP(int changeHP)
        {
            // useDefenseCount = 0;
            // if (changeHP < 0 && useDefense)
            // {
            //     var defenseCount = GetAllStateCount(EUnitState.AddDefense);
            //     if (defenseCount > 0)
            //     {
            //         if (Math.Abs(changeHP) >= defenseCount)
            //         {
            //             useDefenseCount = defenseCount;
            //             changeHP += defenseCount;
            //         }
            //         else
            //         {
            //             useDefenseCount = defenseCount + changeHP;
            //             changeHP = 0;
            //         }
            //         RemoveState(EUnitState.AddDefense, useDefenseCount);
            //     }
            //     
            //     
            //
            // }

            IntervalChangeHP(changeHP);

        }

        protected void IntervalChangeHP(int changeHP)
        {
            CurHP += changeHP;
            CurHP = CurHP <= 0 && FuneCount(EBuffID.Spec_UnDead) <= 0 ? 0 : CurHP;
            CurHP = CurHP > MaxHP ? MaxHP : CurHP;
        }

        public void RoundClear()
        {
            RoundAttackTimes = 0;
            RoundGridMoveCount = 0;
            RoundMoveTimes = 0;
            HurtTimes = 0;
            RoundUnitState.UnitStates.Clear();
            LastCurHPDelta = CurHP - LastCurHP;
            LastCurHP = CurHP;
        }

        public void RoundInit()
        {
            
        }

        public int TargetBuffCount(EUnitStateEffectType effectType)
        {
            var targetBuffCount = 0;
            foreach (var kv in UnitStateData.UnitStates)
            {
                if (kv.Value.Value > 0 && Constant.Battle.EffectUnitStates[effectType].Contains(kv.Key))
                {
                    targetBuffCount += kv.Value.Value;
                }
            }

            return targetBuffCount;
        }
        
        //, bool unUse = false
        public int FuneCount(EBuffID funeID)
        {
            var count = 0;
            foreach (var _funeID in FuneIdxs)
            {
                var funeData = FuneManager.Instance.GetFuneData(_funeID);
                var drBuff = GameEntry.DataTable.GetBuff(funeData.FuneID);
                //unUse && drBuff.BuffIDs.Contains(funeID.ToString()) || !unUse
                if (drBuff.BuffIDs.Contains(funeID.ToString()))
                {
                    count += 1;
                }
                
                
                // if (funeData.FuneID == funeID)
                // {
                //     if (unUse && funeData.Value > 0 || !unUse)
                //     {
                //         count += 1;
                //     }
                // }
                    
            }

            return count;
        }

        public bool Exist()
        {
            return CurHP > 0 || CurHP <= 0 && FuneCount(EBuffID.Spec_UnDead) > 0;
        }
        
        
        // public Data_Fune GetFune(EBuffID funeID, bool unUse = false)
        // {
        //     foreach (var funeIdx in FuneIdxs)
        //     {
        //         var funeData = FuneManager.Instance.GetFuneData(funeIdx);
        //         if (funeData.FuneID == funeID)
        //         {
        //             if (unUse && funeData.Value > 0 || !unUse)
        //             {
        //                 return funeData;
        //             }
        //         }
        //             
        //             
        //     }
        //
        //     return null;
        // }

        // public virtual int BuffCount(int buffID)
        // {
        //     return 0;
        // }
        //
        // public virtual int BuffCount(EBuffID buffID)
        // {
        //     return 0;
        // }
        
        public virtual int BuffCount(string buffStr)
        {
            return 0;
        }
    }

    public class Data_BattleHero : Data_BattleUnit
    {
        public EHeroID HeroID = EHeroID.Empty;

        public Attribute Attribute = new();

        
        
        public int BaseMaxHeart { get; set; }

        //public int RoundHeroHPDelta;

        public void Clear()
        {
            CacheHPDelta = 0;
            HeroID = EHeroID.Empty;
            Attribute.Clear();
        }
        
        public override int CurHP
        {
            get => (int) Attribute.GetAttribute(EHeroAttribute.HP);
            set => Attribute.SetAttribute(EHeroAttribute.HP, value);
        }
        
        public int CurHeart
        {
            get => (int) Attribute.GetAttribute(EHeroAttribute.CurHeart);
            set => Attribute.SetAttribute(EHeroAttribute.CurHeart, value);
        }
        
        public int MaxHeart
        {
            // + GamePlayManager.Instance.GamePlayData.BlessCount(EBlessID.AddHeroMaxHP, BattleManager.Instance.CurUnitCamp);
            get => (int) BaseMaxHeart;
            //set => Attribute.SetAttribute(EHeroAttribute.MaxHP, value);
        }

        public override int MaxHP
        {
            // + GamePlayManager.Instance.GamePlayData.BlessCount(EBlessID.AddHeroMaxHP, BattleManager.Instance.CurUnitCamp);
            get => (int) BaseMaxHP + FuneCount(EBuffID.Spec_AddMaxHP);
            //set => Attribute.SetAttribute(EHeroAttribute.MaxHP, value);
        }

        public Data_BattleHero()
        {
        }

        public Data_BattleHero(int idx, EHeroID heroID, int gridPosIdx, EUnitCamp unitCamp, List<int> funeIdxs) :
            base(idx, gridPosIdx, unitCamp, funeIdxs)
        {
            var drHero = GameEntry.DataTable.GetHero(heroID);
            
            HeroID = heroID;
            BaseMaxHP = drHero.HP;
            BaseMaxHeart = drHero.Heart;
            CurHeart = MaxHeart;
            CurHP = MaxHP;
            LastCurHP = CurHP;
            UnitRole = EUnitRole.Hero;
            

            // Attribute.SetAttribute(EHeroAttribute.MaxHeart, drHero.Heart);
            // Attribute.SetAttribute(EHeroAttribute.CurHeart, drHero.Heart);
            // Attribute.SetAttribute(EHeroAttribute.MaxEnergy, Constant.Hero.MaxEnergy);
            // Attribute.SetAttribute(EHeroAttribute.CurEnergy, Constant.Hero.RecoverEnergy);
            //Attribute.SetAttribute(EHeroAttribute.RecoverEnergy, Constant.Hero.RecoverEnergy);
            FuneIdxs = funeIdxs;
        }

        public new Data_BattleHero Copy()
        {
            var dataBattleHero = new Data_BattleHero();
            dataBattleHero.Idx = Idx;
            dataBattleHero.BaseDamage = BaseDamage;
            dataBattleHero.HeroID = HeroID;
            dataBattleHero.GridPosIdx = GridPosIdx;
            dataBattleHero.CurHP = CurHP;
            dataBattleHero.LastCurHP = LastCurHP;
            dataBattleHero.LastCurHPDelta = LastCurHPDelta;
            dataBattleHero.BaseMaxHP = BaseMaxHP;
            dataBattleHero.Attribute = Attribute.Copy();
            dataBattleHero.UnitCamp = UnitCamp;
            dataBattleHero.UnitStateData = UnitStateData.Copy();
            dataBattleHero.AttackInRound = AttackInRound;
            dataBattleHero.UnitRole = UnitRole;
            dataBattleHero.LinkIDs = new List<ELinkID>(LinkIDs);
            dataBattleHero.FuneIdxs = new List<int>(FuneIdxs);
            dataBattleHero.Links = new List<int>(Links);
            dataBattleHero.BattleLinkIDs = new List<ELinkID>(BattleLinkIDs);
            dataBattleHero.RoundGridMoveCount = RoundGridMoveCount;
            dataBattleHero.RoundMoveTimes = RoundMoveTimes;
            dataBattleHero.RoundAttackTimes = RoundAttackTimes;
            dataBattleHero.RoundUnitState = RoundUnitState.Copy();
            dataBattleHero.HurtTimes = HurtTimes;
            dataBattleHero.UnitCamp = UnitCamp;
            dataBattleHero.AddHeroHP = AddHeroHP;
            dataBattleHero.CacheHPDelta = CacheHPDelta;
            //dataBattleHero.RoundHeroHPDelta = RoundHeroHPDelta;
            return dataBattleHero;

        }



        public override void ChangeHP(int changeHP)
        {
            //useDefenseCount = 0;
            if (changeHP < 0)
            {
                // var defenseCount = GetAllStateCount(EUnitState.AddDefense);
                // if (defenseCount > 0)
                // {
                //     if (Math.Abs(changeHP) >= defenseCount)
                //     {
                //         useDefenseCount = defenseCount;
                //         changeHP += defenseCount;
                //     }
                //     else
                //     {
                //         useDefenseCount = defenseCount + changeHP;
                //         changeHP = 0;
                //     }
                //     RemoveState(EUnitState.AddDefense, useDefenseCount);
                // }


                var deltaHeart = changeHP / MaxHP;
                var deltaHP = changeHP % MaxHP;
                var curHeart = Attribute.GetAttribute(EHeroAttribute.CurHeart);

                if (CurHP + deltaHP <= 0)
                {
                    deltaHeart -= 1;
                    if (curHeart + deltaHeart >= 0)
                    {
                        CurHP += MaxHP + deltaHP;
                    }
                    else
                    {
                        CurHP = 0;
                    }
                    
                    
                    // if (deltaHeart > 0)
                    // {
                    //     CurHP += MaxHP + deltaHP;
                    // }
                    // else
                    // {
                    //     CurHP = MaxHP;
                    // }
                    
                }
                else
                {
                    IntervalChangeHP(deltaHP);
                }

                

                if (curHeart + deltaHeart <= 0)
                {
                    Attribute.SetAttribute(EHeroAttribute.CurHeart, 0);
                }
                else
                {
                    Attribute.SetAttribute(EHeroAttribute.CurHeart,
                        curHeart + deltaHeart);
                }
                

                if (Attribute.GetAttribute(EHeroAttribute.CurHeart) < 0)
                {
                    CurHP = 0;
                }

            }
            else
            {
                IntervalChangeHP(changeHP);

            }

            
        }
    }

    public class Data_BattleSolider : Data_BattleUnit
    {
        public int CardIdx;
        //public List<BuffData> FuneDatas;
        public int Energy;
       


        public Data_BattleSolider()
        {

        }

        public Data_BattleSolider(int idx, int cardIdx, int gridPosIdx, EUnitCamp unitCamp) : base(idx, gridPosIdx,
            unitCamp, new List<int>())
        {
            CardIdx = cardIdx;
            RefreshCardData();
            
            UnitRole = EUnitRole.Staff;
        }

        public void RefreshCardData()
        {
            var card = BattleManager.Instance.GetCard(CardIdx);
            Energy = BattleCardManager.Instance.GetCardEnergy(CardIdx);
            BaseMaxHP = BattleCardManager.Instance.GetCardMaxHP(card.CardID, card.CardIdx);
            CurHP = MaxHP;
            LastCurHP = CurHP;
            FuneIdxs = card.FuneIdxs;
            BattleLinkIDs = new List<ELinkID>(card.RoundLinkIDs);
        }

        public new Data_BattleSolider Copy()
        {
            var dataBattleUnit = new Data_BattleSolider();
            dataBattleUnit.Idx = Idx;
            dataBattleUnit.BaseDamage = BaseDamage;
            dataBattleUnit.CardIdx = CardIdx;
            dataBattleUnit.GridPosIdx = GridPosIdx;
            dataBattleUnit.UnitCamp = UnitCamp;
            dataBattleUnit.Links = new List<int>(Links);
            dataBattleUnit.CurHP = CurHP;
            dataBattleUnit.LastCurHP = LastCurHP;
            dataBattleUnit.LastCurHPDelta = LastCurHPDelta;
            dataBattleUnit.BaseMaxHP = BaseMaxHP;
            dataBattleUnit.UnitStateData = UnitStateData.Copy();
            dataBattleUnit.AttackInRound = AttackInRound;
            dataBattleUnit.UnitRole = UnitRole;
            dataBattleUnit.LinkIDs = new List<ELinkID>(LinkIDs);
            dataBattleUnit.FuneIdxs = new List<int>(FuneIdxs);
            dataBattleUnit.Links = new List<int>(Links);
            dataBattleUnit.BattleLinkIDs = new List<ELinkID>(BattleLinkIDs);
            dataBattleUnit.RoundMoveTimes = RoundMoveTimes;
            dataBattleUnit.RoundAttackTimes = RoundAttackTimes;
            dataBattleUnit.RoundGridMoveCount = RoundGridMoveCount;
            dataBattleUnit.Energy = Energy;
            dataBattleUnit.HurtTimes = HurtTimes;
            dataBattleUnit.RoundUnitState = RoundUnitState.Copy();
            dataBattleUnit.UnitCamp = UnitCamp;
            dataBattleUnit.AddHeroHP = AddHeroHP;
            dataBattleUnit.CacheHPDelta = CacheHPDelta;
            return dataBattleUnit;

        }
        
        public override int BuffCount(string buffStr)
        {
            var count = 0;
            var drCard = CardManager.Instance.GetCardTable(CardIdx);
            foreach (var buffIDStr in drCard.BuffIDs)
            {
                //var buffData = BattleBuffManager.Instance.GetBuffData(buffIDStr);
                if (buffStr == buffIDStr)
                    count++;
            }

            return count;
        }

        // public bool Contain(EBuffID buffID)
        // {
        //     var drBuffs = CardManager.Instance.GetBuffTable(CardID);
        //     if (drBuffs.Any(buff => buff.BuffID == buffID))
        //     {
        //         return true;
        //     }
        //
        //     foreach (var buffData in FuneDatas)
        //     {
        //         if (buffData.BuffID == buffID)
        //             return true;
        //     }
        //
        //     return false;
        //
        // }

        public override void ChangeHP(int changeHP)
        {
            // useDefenseCount = 0;
            // if (changeHP < 0 && useDefense)
            // {
            //     var defenseCount = GetAllStateCount(EUnitState.AddDefense);
            //     if (defenseCount > 0)
            //     {
            //         if (Math.Abs(changeHP) >= defenseCount)
            //         {
            //             useDefenseCount = defenseCount;
            //             changeHP += defenseCount;
            //         }
            //         else
            //         {
            //             useDefenseCount = defenseCount + changeHP;
            //             changeHP = 0;
            //         }
            //         RemoveState(EUnitState.AddDefense, useDefenseCount);
            //         
            //     }
            //
            // }

            IntervalChangeHP(changeHP);


        }
    }

    public class Data_BattleMonster : Data_BattleUnit
    {
        public int MonsterID;

        public bool IsCalculateAction = false;
        //public int EnemyTypeID;

        public Data_BattleMonster()
        {
        }

        public Data_BattleMonster(int idx, int monsterID, int gridPosIdx, EUnitCamp unitCamp,
            List<int> funeIdxs) : base(idx, gridPosIdx, unitCamp, funeIdxs)
        {
            MonsterID = monsterID;
            //EnemyTypeID = enemyTypeID;

            var drEnemy = GameEntry.DataTable.GetEnemy(monsterID);

            BaseMaxHP = drEnemy.HP;
            CurHP = MaxHP;
            LastCurHP = CurHP;
            BaseDamage = 0;//BattleEnemyManager.Instance.GetDamage(monsterID);
            UnitRole = EUnitRole.Staff;
            FuneIdxs = funeIdxs;
        }



        public new Data_BattleMonster Copy()
        {
            var dataBattleEnemy = new Data_BattleMonster();
            dataBattleEnemy.Idx = Idx;
            dataBattleEnemy.BaseDamage = BaseDamage;
            dataBattleEnemy.MonsterID = MonsterID;
            //dataBattleEnemy.EnemyTypeID = EnemyTypeID;
            dataBattleEnemy.GridPosIdx = GridPosIdx;

            dataBattleEnemy.CurHP = CurHP;
            dataBattleEnemy.LastCurHP = LastCurHP;
            dataBattleEnemy.LastCurHPDelta = LastCurHPDelta;
            dataBattleEnemy.BaseMaxHP = BaseMaxHP;
            dataBattleEnemy.UnitCamp = UnitCamp;
            dataBattleEnemy.UnitStateData = UnitStateData.Copy();
            dataBattleEnemy.AttackInRound = AttackInRound;
            dataBattleEnemy.UnitRole = UnitRole;
            dataBattleEnemy.LinkIDs = new List<ELinkID>(LinkIDs);
            dataBattleEnemy.FuneIdxs = new List<int>(FuneIdxs);
            dataBattleEnemy.Links = new List<int>(Links);
            dataBattleEnemy.BattleLinkIDs = new List<ELinkID>(BattleLinkIDs);
            dataBattleEnemy.RoundMoveTimes = RoundMoveTimes;
            dataBattleEnemy.RoundAttackTimes = RoundAttackTimes;
            dataBattleEnemy.RoundGridMoveCount = RoundGridMoveCount;
            dataBattleEnemy.RoundUnitState = RoundUnitState.Copy();
            dataBattleEnemy.HurtTimes = HurtTimes;
            dataBattleEnemy.UnitCamp = UnitCamp;
            dataBattleEnemy.AddHeroHP = AddHeroHP;
            dataBattleEnemy.CacheHPDelta = CacheHPDelta;
            return dataBattleEnemy;

        }
        
        public override int BuffCount(string buffStr)
        {
            var count = 0;
            var drBuffs = BattleEnemyManager.Instance.GetBuffData(Idx);
            foreach (var buffData in drBuffs)
            {
                if (buffStr == buffData.BuffStr)
                    count++;
            }

            return count;
        }
    }
    
    // public class Data_GridItemCard : Data_GridItem
    // {
    //     public ECardID CardID;
    //     
    //     
    // }
    
    public class Data_GridItemUnitState : Data_GridItem
    {
        public EUnitState UnitState;
    }
    
    public class Data_BattleCore : Data_BattleUnit
    {

        public int CorID;
        
        public Data_BattleCore()
        {
            
        }
        
        public Data_BattleCore(int idx, int coreID, int gridPosIdx, EUnitCamp unitCamp) : base(idx, gridPosIdx,unitCamp, null)
        {
            Idx = idx;
            CorID = coreID;
            GridPosIdx = gridPosIdx;
            UnitCamp = unitCamp;
            
            
            
        }
        
        public new Data_BattleCore Copy()
        {
            var dataBattleCore = new Data_BattleCore();
            
            dataBattleCore.Idx = Idx;
            dataBattleCore.GridPosIdx = GridPosIdx;
            dataBattleCore.UnitCamp = UnitCamp;
            dataBattleCore.UnitStateData = UnitStateData.Copy();

            return dataBattleCore;
        }
        
        public override int CurHP
        {
            get => HeroManager.Instance.BattleHeroData.CurHP;
            set => HeroManager.Instance.BattleHeroData.CurHP = value;
        }
        
        
        public override int MaxHP
        {
            // + GamePlayManager.Instance.GamePlayData.BlessCount(EBlessID.AddHeroMaxHP, BattleManager.Instance.CurUnitCamp);
            get => HeroManager.Instance.BattleHeroData.MaxHP;
            //set => Attribute.SetAttribute(EHeroAttribute.MaxHP, value);
        }
    }
    
    public class Data_GridProp : Data_GridItem
    {
        public int GridPropID;
        
        public Data_GridProp()
        {
            
        }
        
        public Data_GridProp(int gridPropID, int idx, int gridPosIdx, EUnitCamp unitCamp) : base(idx, gridPosIdx, unitCamp)
        {
            GridPropID = gridPropID;
        }
        
        public new Data_GridProp Copy()
        {
            var dataGridProp = new Data_GridProp();
            dataGridProp.GridPropID = GridPropID;
            dataGridProp.Idx = Idx;
            dataGridProp.GridPosIdx = GridPosIdx;
            dataGridProp.UnitCamp = UnitCamp;
            //dataGridProp.TargetGridType = TargetGridType;
            


            return dataGridProp;
        }
    }
    

    public class Data_GridPropLink : Data_GridProp
    {
        public ELinkID LinkID;
        
        public Data_GridPropLink()
        {
            
        }

        public Data_GridPropLink(int gridPropID, ELinkID linkID, int idx, int gridPosIdx) : base(gridPropID, idx,
            gridPosIdx, EUnitCamp.Third)

        {
            LinkID = linkID;
        }
        
        public new Data_GridPropLink Copy()
        {
            var dataGridProp = new Data_GridPropLink();
            dataGridProp.GridPropID = GridPropID;

            dataGridProp.Idx = Idx;
            dataGridProp.GridPosIdx = GridPosIdx;
            dataGridProp.UnitCamp = UnitCamp;
            dataGridProp.LinkID = LinkID;

            return dataGridProp;
        }
    }
    
    public class Data_GridPropMoveDirect : Data_GridProp
    {
        public ERelativePos Direct;
        public bool UseInRound = false;
        public Data_GridPropMoveDirect()
        {
            
        }

        public Data_GridPropMoveDirect(int gridPropID, ERelativePos direct, int idx, int gridPosIdx, EUnitCamp unitCamp) : base(gridPropID, idx,
            gridPosIdx, unitCamp)

        {
            Direct = direct;
        }
        
        public new Data_GridPropMoveDirect Copy()
        {
            var dataGridProp = new Data_GridPropMoveDirect();
            dataGridProp.GridPropID = GridPropID;
            dataGridProp.Direct = Direct;
            dataGridProp.UseInRound = UseInRound;

            dataGridProp.Idx = Idx;
            dataGridProp.GridPosIdx = GridPosIdx;
            dataGridProp.UnitCamp = UnitCamp;
            //dataGridProp.TargetGridType = TargetGridType;

            return dataGridProp;
        }
    }
    
    
    public class Data_Area
    {
        public int AreaIndex;
        public int AreaID;
        public int RandomSeed;
        
        public Dictionary<int, Data_Block> BlockDatas = new ();
        
        public Data_Area Copy()
        {
            var dataArea = new Data_Area();
            dataArea.AreaID = AreaID;
            dataArea.RandomSeed = RandomSeed;
            
            foreach (var kv in BlockDatas)
            {
                dataArea.BlockDatas.Add(kv.Key, kv.Value.Copy());
            }

            return dataArea;
        }
    }

    public class Data_Bless
    {
        public int Idx;

        public int BlessID;
        //public EBlessID BlessID;
        public float Value;

        public Data_Bless()
        {

        }
        
        public Data_Bless(int idx, int blessID)
        {
            Idx = idx;
            BlessID = blessID;
            
            var drBless = GameEntry.DataTable.GetBless(blessID);
            Value = BattleBuffManager.Instance.GetBuffValue(drBless.Values1[0]);
        }

        public Data_Bless Copy()
        {
            var dataBless = new Data_Bless();
            dataBless.Idx = Idx;
            dataBless.BlessID = BlessID;
            dataBless.Value = Value;

            return dataBless;
        }
    }
    
    public class Data_EnergyBuff
    {
        public int Heart = -1;
        public int HP = -1;
        public int EnergyBuffIdx = -1;
        public int EffectCount = 0;
        public string BuffStr = string.Empty;
        
        public Data_EnergyBuff Copy()
        {
            var energyBuffData = new Data_EnergyBuff();
            energyBuffData.Heart = Heart;
            energyBuffData.HP = HP;
            energyBuffData.EnergyBuffIdx = EnergyBuffIdx;
            energyBuffData.EffectCount = EffectCount;
            energyBuffData.BuffStr = BuffStr;

            return energyBuffData;
        }
        
        public void Clear()
        {
            Heart = -1;
            HP = -1;
            EnergyBuffIdx = -1;
            EffectCount = 0;
            BuffStr = String.Empty;
        }
    }
    
    public class Data_Player
    {
        public int CardIdx;
        public int TmpCardIdx;
        public int FuneIdx;
        public int BlessIdx;
        public ulong PlayerID;
        public EUnitCamp UnitCamp;
        public List<int> UnusedFuneIdxs = new List<int>();
        
        
        public Data_BattleHero BattleHero = new();
        public Dictionary<int, Data_Card> CardDatas = new ();
        public Dictionary<int, Data_Fune> FuneDatas = new();
        public Dictionary<int, Data_Bless> BlessDatas = new ();
        public Dictionary<int, Data_EnergyBuff> EnergyBuffDatas = new ();
        public int Coin;
        
        public void Clear()
        {
            CardIdx = 0;
            TmpCardIdx = 0;
            FuneIdx = 0;
            BlessIdx = 0;
            BattleHero.Clear();
            UnusedFuneIdxs.Clear();
            CardDatas.Clear();
            FuneDatas.Clear();
            BlessDatas.Clear();
            EnergyBuffDatas.Clear();
        
        }
        
        public Data_Player()
        {
            
        }

        
        
        public Data_Player Copy()
        {
            var data = new Data_Player();

            data.CardIdx = CardIdx;
            data.FuneIdx = FuneIdx;
            data.BlessIdx = BlessIdx;
            data.BattleHero = BattleHero.Copy();
            
            data.PlayerID = PlayerID;
            data.UnitCamp = UnitCamp;
            data.EnergyBuffDatas = new Dictionary<int, Data_EnergyBuff>(EnergyBuffDatas);
            data.Coin = Coin;
            data.UnusedFuneIdxs = UnusedFuneIdxs;
            
            data.CardDatas.Clear();
            foreach (var kv in CardDatas)
            {
                data.CardDatas.Add(kv.Key, kv.Value.Copy());
            }
            
            foreach (var kv in BlessDatas)
            {
                data.BlessDatas.Add(kv.Key, kv.Value.Copy());
            }
            
            foreach (var kv in FuneDatas)
            {
                data.FuneDatas.Add(kv.Key, kv.Value.Copy());
            }

            return data;
        }
        
        
        
        public void RoundClear()
        {
            foreach (var kv in FuneDatas)
            {
                var drFune = GameEntry.DataTable.GetBuff(kv.Value.FuneID);
                if(drFune == null)
                    continue;
                FuneDatas[kv.Key].Value = (int)BattleBuffManager.Instance.GetBuffValue(drFune.GetValues(0)[0]);
            }

            foreach (var kv in CardDatas)
            {
                kv.Value.RoundClear();
            }

            
            foreach (var kv in BlessDatas)
            {
                var drBless = GameEntry.DataTable.GetBless(kv.Value.BlessID);
                if (drBless.BlessID == EBlessID.EachRoundUseUnitCardAddDefense ||
                    drBless.BlessID == EBlessID.EachRoundUseFightCardAttackAllEnemy ||
                    drBless.BlessID == EBlessID.EachRoundUseTacticCardAttackAllEnemy)
                {
                    
                    kv.Value.Value = BattleBuffManager.Instance.GetBuffValue(drBless.Values1[0]);
                }
            }
        }
        
        public bool Contain(EBlessID blessID)
        {
            var drBless = GameEntry.DataTable.GetBless(blessID);
            foreach (var kv in BlessDatas)
            {
                if (kv.Value.BlessID == drBless.Id)
                    return true;
            }
            
            return false;
        }
        
        public int BlessCount(EBlessID blessID)
        {
            var idx = 0;
            var drBless = GameEntry.DataTable.GetBless(blessID);
            foreach (var kv in BlessDatas)
            {
                if (kv.Value.BlessID == drBless.Id)
                    idx++;
            }
            
            return idx;
        }
        
        public Data_Bless GetUsefulBless(EBlessID blessID)
        {
            var drBless = GameEntry.DataTable.GetBless(blessID);
            foreach (var kv in BlessDatas)
            {
                if (kv.Value.BlessID == drBless.Id && kv.Value.Value >= 0)
                    return kv.Value;
            }
            
            return null;
        }

        
    }

    public class Data_Enemy
    {
        public Data_BattleCurse BattleCurseData = new ();

        public Data_Enemy Copy()
        {
            var data = new Data_Enemy();
            
            data.BattleCurseData = BattleCurseData.Copy();

            return data;
        }
        
        public void RoundClear()
        {
            BattleCurseData.RoundClear();
        }
    }

    public class Data_MapStage
    {
        public int MapIdx;
        public int StageIdx;
        public int SelectRouteIdx;
        public int StageRandomSeed;
        
        public Dictionary<int, Dictionary<int, Data_MapStep>> MapSteps = new ();

        public Data_MapStage Copy()
        {
            var data = new Data_MapStage();
            data.MapIdx = MapIdx;
            data.StageIdx = StageIdx;
            data.SelectRouteIdx = SelectRouteIdx;
            data.StageRandomSeed = StageRandomSeed;

            foreach (var kv in MapSteps)
            {
                var dict = new Dictionary<int, Data_MapStep>();
                foreach (var kv2 in kv.Value)
                {
                    dict.Add(kv2.Key, kv2.Value.Copy());
                }
                data.MapSteps.Add(kv.Key, dict);
            }

            return data;
        }
        
    }
    
    public class Data_MapStep
    {
        public Data_MapRoute MapRoute;
        public int StepIdx;
        public int RandomSeed;

        public Data_MapStep Copy()
        {
            var data = new Data_MapStep();
            data.MapRoute = MapRoute.Copy();
            data.StepIdx = StepIdx;
            data.RandomSeed = RandomSeed;

            return data;
        }
    }
    
    public class Data_MapRoute
    {
        public int MapIdx;
        public int StageIdx;
        public int RouteIdx;

        public Data_MapRoute Copy()
        {
            var data = new Data_MapRoute();
            data.MapIdx = MapIdx;
            data.StageIdx = StageIdx;
            data.RouteIdx = RouteIdx;
            
            return data;
        }

        
    }
    
    
    public class MapStageIdx
    {
        public int MapIdx;
        public int StageIdx;
        //public int RouteIdx;
        
        public int StepIdx = -1;
        public bool IsSelectRoute = false;
        //public int RadomSeed;

        public MapStageIdx()
        {
            
        }

        public MapStageIdx Copy()
        {
            var mapStageIdx = new MapStageIdx();
            
            mapStageIdx.MapIdx = MapIdx;
            mapStageIdx.StageIdx = StageIdx;
            //mapStageIdx.RouteIdx = RouteIdx;
            mapStageIdx.StepIdx = StepIdx;
            mapStageIdx.IsSelectRoute = IsSelectRoute;

            return mapStageIdx;
        }

    }


    public class Data_Map
    {
        public int RandomSeed;
        
        public Dictionary<int, Data_MapStage> MapStageDataDict = new ();
        public MapStageIdx CurMapStageIdx = new MapStageIdx();

        public Data_Map()
        {
            
        }
        

        public Data_Map Copy()
        {
            var data = new Data_Map();
            foreach (var kv in MapStageDataDict)
            {
                data.MapStageDataDict.Add(kv.Key, kv.Value.Copy());
            }
            data.CurMapStageIdx = CurMapStageIdx.Copy();
            return data;
        }

        public void Clear()
        {
            MapStageDataDict.Clear();
        }
        
    }

    

    public class Data_GamePlay
    {
        //public string GamePlayFileName;
        public Data_Player PlayerData = new ();
        public Data_Player LastActionPlayerData = new ();
        public Data_Map MapData = new ();
        public Data_Area AreaData = new ();
        public Data_Battle BattleData = new ();
        public Data_Battle LastRoundBattleData = new ();
        public Data_Battle LastActionBattleData = new ();
        public Data_Enemy EnemyData = new ();
        public EGamMode GameMode;
        public int RandomSeed = -1;
        public bool IsTutorialBattle = false;
        public bool IsTutorial = false;
        
        public List<Data_Player> PlayerDatas = new();
        public Dictionary<ulong, Data_Player> PlayerDataIDDict = new ();
        public Dictionary<EUnitCamp, Data_Player> PlayerDataCampDict = new ();
        

        public Data_GamePlay()
        {
            
        }

        public bool Contain(ulong playerID)
        {
            return PlayerDataIDDict.ContainsKey(playerID);
        }

        public void AddPlayerData(Data_Player playerData)
        {
            PlayerDatas.Add(playerData);
            PlayerDataCampDict.Add(playerData.UnitCamp, playerData);
            PlayerDataIDDict.Add(playerData.PlayerID, playerData);

        }

        public void ClearPlayerDataList()
        {
            PlayerDatas.Clear();
            PlayerDataCampDict.Clear();
            PlayerDataIDDict.Clear();
        }
        
        public Data_Player GetPlayerData(ulong userID)
        {
            if (!PlayerDataIDDict.ContainsKey(userID))
                return null;

            return PlayerDataIDDict[userID];
        }


        public Data_Player GetPlayerData(EUnitCamp unitCamp)
        {
            if (!PlayerDataCampDict.ContainsKey(unitCamp))
                return null;

            return PlayerDataCampDict[unitCamp];
        }
        
        public Data_GamePlay Copy()
        {
            var data = new Data_GamePlay();

            data.PlayerData = PlayerData.Copy();
            data.LastActionPlayerData = LastActionPlayerData?.Copy();
            data.AreaData = AreaData.Copy();
            data.BattleData = BattleData.Copy();
            data.LastRoundBattleData = LastRoundBattleData.Copy();
            data.LastActionBattleData = LastActionBattleData?.Copy();
            data.EnemyData = EnemyData.Copy();
            data.MapData = MapData.Copy();
            data.IsTutorialBattle = IsTutorialBattle;
            data.GameMode = GameMode;
            data.RandomSeed = RandomSeed;

            data.PlayerDatas.Clear();
            data.PlayerDataCampDict.Clear();
            data.PlayerDataIDDict.Clear();
            
            
            foreach (var kv in PlayerDatas)
            {
                data.AddPlayerData(kv.Copy());
            }

            return data;
        }

        public void RoundInit()
        {
            BattleData.RoundInit();
        }

        public void RecordLastAction()
        {
            LastActionBattleData = BattleData.Copy();
            LastActionPlayerData = PlayerData.Copy();
        }

        public void RoundClear()
        {
            LastRoundBattleData = BattleData.Copy();
            BattleData.RoundClear();

            EnemyData.RoundClear();
            foreach (var kv in PlayerDatas)
            {
                kv.RoundClear();
            }
        }
        
        public bool Contain(EBlessID blessID, EUnitCamp unitCamp)
        {
            if (PlayerDataCampDict.ContainsKey(unitCamp))
                return false;
            
            return PlayerDataCampDict[unitCamp].Contain(blessID);
        }
        
        public int BlessCount(EBlessID blessID, EUnitCamp unitCamp)
        {
            if (!PlayerDataCampDict.ContainsKey(unitCamp))
                return 0;
            
            return PlayerDataCampDict[unitCamp].BlessCount(blessID);
        }
        
        public Data_Bless GetUsefulBless(EBlessID blessID, EUnitCamp unitCamp)
        {
            if (!PlayerDataCampDict.ContainsKey(unitCamp))
                return null;
            
            return PlayerDataCampDict[unitCamp].GetUsefulBless(blessID);
        }
        
    }

    public class Data_BattleCurse
    {
        //public int RandomUnitUnRecover_UnitID = -1;
        public List<ECurseID> CurseIDs = new ();
        public Dictionary<int, int> AllUnitDodgeSubHeartDamage_Dict = new ();
        public List<int> AttackRowOrCol_PosIdxs = new List<int>();
        public Dictionary<ECurseID, int> RandomUnitIDs = new ();
        
        public Data_BattleCurse Copy()
        {
            var data = new Data_BattleCurse();

            //data.RandomUnitUnRecover_UnitID = RandomUnitUnRecover_UnitID;
            data.AllUnitDodgeSubHeartDamage_Dict = new Dictionary<int, int>(AllUnitDodgeSubHeartDamage_Dict);
            data.CurseIDs = new List<ECurseID>(CurseIDs);

            return data;
        }
        
        public void RoundClear()
        {
            RandomUnitIDs.Clear();
        }

    }

    public class Data_BattlePlayer
    {
        public List<int> StandByCards = new ();
        public List<int> PassCards = new ();
        public List<int> HandCards = new ();
        public List<int> ConsumeCards = new();
        public int RoundUseCardCount;
        public int LastRoundUseCardCount;
        public bool RoundIsAttack;
        
        
        public List<EBuffID> BattleBuffs = new ();

        public Data_BattlePlayer Copy()
        {
            var data = new Data_BattlePlayer();
            
            data.StandByCards = new List<int>(StandByCards);
            data.PassCards = new List<int>(PassCards);
            data.HandCards = new List<int>(HandCards);
            data.ConsumeCards = new List<int>(ConsumeCards);
            data.RoundUseCardCount = RoundUseCardCount;
            data.LastRoundUseCardCount = LastRoundUseCardCount;
            data.RoundIsAttack = RoundIsAttack;
            data.BattleBuffs = new List<EBuffID>(BattleBuffs);
            

            return data;
        }
        
        public List<int> GetAllCards()
        {
            var allCards = new List<int>();
            foreach (var handCard in HandCards)
            {
                allCards.Add(handCard);
            }
            foreach (var standByCard in StandByCards)
            {
                allCards.Add(standByCard);
            }
            foreach (var passCard in PassCards)
            {
                allCards.Add(passCard);
            }
            foreach (var removeCard in ConsumeCards)
            {
                allCards.Add(removeCard);
            }

            return allCards;
        }
        
        public void RoundClear()
        {

            LastRoundUseCardCount = RoundUseCardCount;
            RoundUseCardCount = 0;
            RoundIsAttack = false;
            BattleBuffs.Clear();
            
        }

        public void Clear()
        {
            StandByCards.Clear();
            PassCards.Clear();
            HandCards.Clear();
            ConsumeCards.Clear();
            RoundUseCardCount = 0;
            LastRoundUseCardCount = 0;
            RoundIsAttack = false;
            BattleBuffs.Clear();
        }
    }

    public class Data_Battle
    {
        public int Round;
        public int UnitIdx;
        public EEnemyType EnemyType;
        public EGameDifficulty GameDifficulty;
        public Dictionary<EUnitCamp, Data_BattlePlayer> BattlePlayerDatas = new (10);
        public int ResetActionTimes = Constant.Battle.ResetActionTimes;

        public Dictionary<int, Data_BattleUnit> BattleUnitDatas = new(10);
        public Dictionary<int, Data_GridProp> GridPropDatas = new(10);
        public Dictionary<int, EGridType> GridTypes = new (100);
        

        public Data_Battle()
        {
            BattlePlayerDatas.Add(EUnitCamp.Player1, new Data_BattlePlayer());
            BattlePlayerDatas.Add(EUnitCamp.Player2, new Data_BattlePlayer());

        }

        public void Clear()
        {
            Round = 0;
            foreach (var kv in BattlePlayerDatas)
            {
                kv.Value.Clear();
            }
            
            BattleUnitDatas.Clear();
            GridPropDatas.Clear();
            GridTypes.Clear();
            
        }
        
        public Data_BattlePlayer GetBattlePlayerData(EUnitCamp unitCamp)
        {
            if (!BattlePlayerDatas.ContainsKey(unitCamp))
                return null;

            return BattlePlayerDatas[unitCamp];
        }

        public List<Data_BattleUnit> ContainUnit(int cardID)
        {
            var units = new List<Data_BattleUnit>();
            foreach (var kv in BattleUnitDatas)
            {
                if (kv.Value is Data_BattleSolider solider)
                {
                    var card = BattleManager.Instance.GetCard(solider.CardIdx);
                    if (card.CardID == cardID)
                    {
                        units.Add(kv.Value as Data_BattleUnit);
                    }
                }
            }

            return units;
        }

        public Data_Battle Copy()
        {
            var dataBattle = new Data_Battle();

            dataBattle.Round = Round;
            dataBattle.UnitIdx = UnitIdx;
            dataBattle.EnemyType = EnemyType;
            dataBattle.GameDifficulty = GameDifficulty;
            dataBattle.ResetActionTimes = ResetActionTimes;

            foreach (var kv in BattleUnitDatas)
            {
                if (kv.Value is Data_BattleSolider solider)
                {
                    dataBattle.BattleUnitDatas.Add(kv.Key, solider.Copy());
                }
                else if (kv.Value is Data_BattleHero hero)
                {
                    dataBattle.BattleUnitDatas.Add(kv.Key, hero.Copy());
                }
                else if (kv.Value is Data_BattleMonster monster)
                {
                    dataBattle.BattleUnitDatas.Add(kv.Key, monster.Copy());
                }
                else  if (kv.Value is Data_BattleCore core)
                {
                    dataBattle.BattleUnitDatas.Add(kv.Key, core.Copy());
                }
                
            }
            
            foreach (var kv in GridTypes)
            {
                dataBattle.GridTypes.Add(kv.Key, kv.Value);
            }

            foreach (var kv in GridPropDatas)
            {
                if (kv.Value is Data_GridPropMoveDirect moveDirect)
                {
                    dataBattle.GridPropDatas.Add(kv.Key, moveDirect.Copy());
                }
                else
                {
                    dataBattle.GridPropDatas.Add(kv.Key, kv.Value.Copy());
                }
            }

            dataBattle.BattlePlayerDatas.Clear();
            foreach (var kv in BattlePlayerDatas)
            {
                dataBattle.BattlePlayerDatas.Add(kv.Key, kv.Value.Copy());
            }
            
            //dataBattle.BattlePlayerDatas = new Dictionary<EUnitCamp, Data_BattlePlayer>(BattlePlayerDatas);

            return dataBattle;

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
        
        public Data_GridProp Contain(EGridPropID gridPropID, int gridPosIdx)
        {

            if (!BattleFightManager.Instance.CacheGridPorpIDStr.ContainsKey(gridPropID))
            {
                BattleFightManager.Instance.CacheGridPorpIDStr.Add(gridPropID, gridPropID.ToString());    
            }
            
            

            return Contain(BattleFightManager.Instance.CacheGridPorpIDStr[gridPropID], gridPosIdx);

        }
        
        public Data_GridProp Contain(string gridPropIDStr, int gridPosIdx)
        {
            foreach (var kv in GridPropDatas)
            {
                var drGridProp = GameEntry.DataTable.GetGridProp(kv.Value.GridPropID);
                if (drGridProp.GridPropIDs.Contains(gridPropIDStr) && kv.Value.GridPosIdx == gridPosIdx)
                    return kv.Value;
            }

            return null;

        }
        
        public int GetUnitCount(EUnitCamp selfCamp, List<ERelativeCamp> targetCamps, List<EUnitRole> roles)
        {
            var unitCount = 0;
            foreach (var kv in BattleUnitDatas)
            {
                if (kv.Value.Exist() &&
                    ((targetCamps.Contains(ERelativeCamp.Us) && kv.Value.UnitCamp == selfCamp) ||
                     (targetCamps.Contains(ERelativeCamp.Enemy) && kv.Value.UnitCamp != selfCamp)) && 
                    roles.Contains(kv.Value.UnitRole))
                {
                    unitCount += 1;
                }

            }

            return unitCount;
        }

        public void RoundClear()
        {
            foreach (var kv in BattleUnitDatas)
            {
                kv.Value.RoundClear();
            }

            foreach (var kv in BattlePlayerDatas)
            {
                kv.Value.RoundClear();
            }

        }

        public void RoundInit()
        {
            foreach (var kv in BattleUnitDatas)
            {
                kv.Value.RoundInit();
            }
        }


    }
    
    

}
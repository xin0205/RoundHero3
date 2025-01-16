using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    public class BattleManager : Singleton<BattleManager>
    {
        public Random Random;
        private int randomSeed;

        public IBattleTypeManager BattleTypeManager;
        
        public TempTriggerData TempTriggerData = new TempTriggerData();
        
        public EBattleState BattleState { get => BattleTypeManager.BattleState;
            set => BattleTypeManager.BattleState = value;
        }

        
        public Data_Battle BattleData => GamePlayManager.Instance.GamePlayData.BattleData;

        public EUnitCamp CurUnitCamp;

        
        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new Random(randomSeed);
            
            // var randoms = MathUtility.GetRandomNum(8, 0,
            //     Constant.Game.RandomRange, Random);
            
            
            BattleSoliderManager.Instance.Init(Random.Next());
            BattleThirdUnitManager.Instance.Init(Random.Next());
            BattleEnemyManager.Instance.Init(Random.Next());
            
            BattleGridPropManager.Instance.Init(Random.Next());
            BattleBuffManager.Instance.Init(Random.Next());
            BattleFightManager.Instance.Init(Random.Next());
            BattleCurseManager.Instance.Init(Random.Next());
            BattleCardManager.Instance.Init(Random.Next());
            
            BattleAreaManager.Instance.Init(Random.Next());
            BattleUnitManager.Instance.Init(Random.Next());
            HeroManager.Instance.Init(Random.Next());
            BattleCardManager.Instance.InitCards();
        }

        public void SetBattleTypeManager(IBattleTypeManager battleTypeManager)
        {
            BattleTypeManager = battleTypeManager;
        }
        
        public void SetCurPlayer(EUnitCamp unitCamp)
        {
            CurUnitCamp = unitCamp;
            
            BattlePlayerManager.Instance.SetCurPlayer();
    
        }

        public void Update()
        {
            BattleAreaManager.Instance.Update();
            BattleCardManager.Instance.Update();
            BattleEnemyManager.Instance.Update();
        }
        

        public void Destory()
        {
            BattleData.Clear();
            BattleSoliderManager.Instance.Destory();
            BattleThirdUnitManager.Instance.Destory();
            
            BattleGridPropManager.Instance.Destory();
            BattleBuffManager.Instance.Destory();
            BattleFightManager.Instance.Destory();
            BattleCurseManager.Instance.Destory();
            BattleCardManager.Instance.Destory();
            BattleAreaManager.Instance.Destory();
            
            BattleEnemyManager.Instance.Destory();
            BattleUnitManager.Instance.Destory();
            HeroManager.Instance.Destory();
            //BlessManager.Instance.Destory();
            //BattleBuffManager.Instance.Destory();
            //FightManager.Instance.Destory();
            //BattleCurseManager.Instance.Destory();
        }

        public void Refresh()
        {

            BattleFightManager.Instance.CacheRoundFightData();
            BattleEnemyManager.Instance.ShowEnemyRoute();
            RefreshView();
        }
        
        public void RefreshView()
        {

            BattleUnitManager.Instance.RefreshDamageState();
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
        }

        private void ChangeUnitHP(Data_GamePlay gamePlayData, Data_BattleUnit unit, int hpDelta, bool changeHPInstantly = false)
        {
            // if (unit is Data_BattleHero battleHeroData && !changeHPInstantly)
            // {
            //     battleHeroData.RoundHeroHPDelta += hpDelta;
            // }
            // else
            // {
            //     unit.ChangeHP(hpDelta);
            // }
            
            if (unit is Data_BattleHero)
            {
                if (changeHPInstantly)
                {
                    unit.ChangeHP(hpDelta);
                }
            }
            else
            {
                unit.ChangeHP(hpDelta);
            }
            
            

            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());

        }

        public int ChangeHP(Data_BattleUnit unit, int value, Data_GamePlay gamePlayData,  EHPChangeType hpChangeType, bool useDefense = true, bool addHeroHP = true, bool changeHPInstantly = false)
        {
            if(unit == null)
                return 0;
            
            var useDefenseCount = 0;
            
            var oldHeart = 0f;
            var hero = unit as Data_BattleHero;
            if (hero != null)
            {
                oldHeart = hero.Attribute.GetAttribute(EHeroAttribute.CurHeart);

            }
            
            if (value < 0 && useDefense)
            {
                var defenseCount = unit.GetAllStateCount(EUnitState.HurtSubDmg);
                if (defenseCount > 0)
                {
                    if (Mathf.Abs(value) >= defenseCount)
                    {
                        useDefenseCount = defenseCount;
                        value += defenseCount;
                    }
                    else
                    {
                        useDefenseCount = -value;
                        value = 0;
                    }
                    unit.RemoveState(EUnitState.HurtSubDmg, useDefenseCount);
                    
                }

            }
            
            var oldHP = unit.CurHP;
            
            var hurtSubDamageCount = gamePlayData.BlessCount(EBlessID.HurtSubDamageSubDamage, BattleManager.Instance.CurUnitCamp);
            var drHurtSubDamage = GameEntry.DataTable.GetBless(EBlessID.HurtSubDamageSubDamage);
            
            var heroEachHurtUnDamage = gamePlayData.GetUsefulBless(EBlessID.HeroEachHurtUnDamage, BattleManager.Instance.CurUnitCamp);
            var drHeroEachHurtUnDamage = GameEntry.DataTable.GetBless(EBlessID.HeroEachHurtUnDamage);
            var value0 = BattleBuffManager.Instance.GetBuffValue(drHeroEachHurtUnDamage.Values1[0]);
            var value1 = BattleBuffManager.Instance.GetBuffValue(drHeroEachHurtUnDamage.Values1[1]);


            if (heroEachHurtUnDamage != null && unit.UnitCamp == BattleManager.Instance.CurUnitCamp && hero != null && value <= 0)
            {
                heroEachHurtUnDamage.Value -= 1;
                if (heroEachHurtUnDamage.Value <= 0)
                {
                    heroEachHurtUnDamage.Value = value0;
                    value = 0;
                }
            }
            
            if (unit.UnitCamp == BattleManager.Instance.CurUnitCamp && hurtSubDamageCount > 0 && value >= value0)
            {
                var subHP = (int) (value1 * hurtSubDamageCount);
                var hp = value +subHP >= 0 ? 0 : value + subHP;
                ChangeUnitHP(gamePlayData, unit, hp, changeHPInstantly);
            }
            else
            {
                ChangeUnitHP(gamePlayData, unit, (int)value, changeHPInstantly);
            }
            
            
            var newHP =  unit.CurHP;
            
            var deltaHP = newHP - oldHP;
            var deltaHPNoDefense = deltaHP - useDefenseCount;
            
            if (unit is Data_BattleSolider && addHeroHP && deltaHPNoDefense < 0)
            {
                var addHP = -deltaHPNoDefense;
                if (unit.UnitCamp == BattleManager.Instance.CurUnitCamp &&
                    gamePlayData.BlessCount(EBlessID.DefenseToHP, BattleManager.Instance.CurUnitCamp) > 0)
            
                {
                    addHP += useDefenseCount;
                }
            
                //var battlePlayerData = gamePlayData.BattleData.GetBattlePlayerData(BattleManager.Instance.CurUnitCamp);
                // if (unit.UnitCamp == BattleManager.Instance.CurUnitCamp &&
                //     battlePlayerData != null && 
                //     battlePlayerData.RoundBuffs.Contains(EBuffID.HurtSubDamageAddHeroCurHP))
                // {
                //     addHP += useDefenseCount;
                // }
                
                if (addHP > 0 && unit.UnitCamp == BattleManager.Instance.CurUnitCamp && unit.GetStateCount(EUnitState.UnRecover) <= 0)
                {
                    //var playerData = gamePlayData.GetPlayerData(BattleManager.Instance.CurUnitCamp);
                    //ChangeUnitHP(gamePlayData, playerData.BattleHero, addHP, changeHPInstantly);
                    unit.AddHeroHP += addHP;
                }
            }

            // var drDamageOverDefenseAddDamage =
            //     GameEntry.DataTable.GetBless(EBlessID.DamageOverDefenseAddDamage);
            // if (unit.UnitCamp != BattleManager.Instance.CurUnitCamp && gamePlayData.BlessCount(EBlessID.DamageOverDefenseAddDamage, BattleManager.Instance.CurUnitCamp) > 0 && useDefenseCount > 0 && deltaHPNoDefense < 0)
            // {
            //     ChangeUnitHP(gamePlayData, unit, (int)((drDamageOverDefenseAddDamage.Values1[0] - 1) * deltaHPNoDefense), changeHPInstantly);
            // }
            
            if (hero != null)
            {
                var curHeart = hero.Attribute.GetAttribute(EHeroAttribute.CurHeart);
            
                var heroDodgeSubHeartDamage = gamePlayData.GetUsefulBless(EBlessID.HeroDodgeSubHeartDamage, BattleManager.Instance.CurUnitCamp);

                if (curHeart < oldHeart)
                {
                    if (heroDodgeSubHeartDamage != null)
                    {
                        heroDodgeSubHeartDamage.Value -= 1;
                        hero.Attribute.SetAttribute(EHeroAttribute.CurHeart, oldHeart);
                        hero.CurHP = oldHP;

                    }
                
                    else if (gamePlayData.BlessCount(EBlessID.HPTo0FullAllUnitHP, BattleManager.Instance.CurUnitCamp) > 0)
                    {
                        foreach (var kv in gamePlayData.BattleData.BattleUnitDatas)
                        {
                            if (kv.Value.UnitCamp == BattleManager.Instance.CurUnitCamp && kv.Value.UnitRole == EUnitRole.Staff)
                            {
                                kv.Value.CurHP = kv.Value.MaxHP;
                            }
                        }
                    }
                    else
                    {
                        hero.Attribute.SetAttribute(EHeroAttribute.CurHeart, curHeart);
                    }
                }
                
                
                
                if (curHeart <= 0 &&
                    hero.CurHP <= 0)
                {
                    var heroRebirth = gamePlayData.GetUsefulBless(EBlessID.HeroRebirth, BattleManager.Instance.CurUnitCamp);
                    if (heroRebirth != null)
                    {
                        heroRebirth.Value -= 1;
                        hero.Attribute.SetAttribute(EHeroAttribute.CurHeart,1);
                        hero.CurHP = hero.MaxHP; 

                    }
                }
            }

            if (unit.CurHP <= 0 && 
                unit.UnitCamp == EUnitCamp.Enemy &&
                BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.AllUnitDodgeSubHeartDamage) &&
                BattleCurseManager.Instance.GetAllUnitDodgeSubHeartDamageValue(gamePlayData, unit.ID) > 0 && 
                !GameUtility.ContainRoundState(GamePlayManager.Instance.GamePlayData, EBuffID.Spec_CurseUnEffect))
  
            {
                gamePlayData.EnemyData.BattleCurseData.AllUnitDodgeSubHeartDamage_Dict[unit.ID] -= 1;
                unit.CurHP = oldHP;
            }

            BlessManager.Instance.DeadTrigger(gamePlayData, unit);

            var hpDelta = 0;

            if (!changeHPInstantly)
            {
                hpDelta = value;
            }
            else if (hero != null)
            {
                var curHeart = hero.Attribute.GetAttribute(EHeroAttribute.CurHeart);
                hpDelta = (int)((curHeart * hero.MaxHP + hero.CurHP) - (oldHeart * hero.MaxHP + oldHP));
 
            }
            else
            {
                hpDelta = unit.CurHP - oldHP;
            }

            return hpDelta;
        }


        public void RoundEndTrigger()
        {
            BattleUnitStateManager.Instance.RoundEndTrigger();
            
        }

        public void RoundStartTrigger()
        {
            GamePlayManager.Instance.GamePlayData.RoundClear();
            
            BattleGridPropManager.Instance.RefreshPropMoveDirectUseInRound();
            BattleUnitManager.Instance.RoundStartTrigger();
            BattleUnitStateManager.Instance.RoundStartTrigger();
            BattleCurseManager.Instance.RoundStartTrigger();
            BlessManager.Instance.RoundStartTrigger(GamePlayManager.Instance.GamePlayData);
            HeroManager.Instance.RoundStartTrigger();
        }

        public void StartAction()
        {
            BattleTypeManager.StartAction();
        }

        public void ContinueAction()
        {
            BattleTypeManager.ContinueAction();
        }
        
        public void NextAction()
        {
            BattleTypeManager.NextAction();
        }

        public void PlaceUnitCard(int cardID, int gridPosIdx, EUnitCamp playerUnitCamp)
        {
            BattleTypeManager.PlaceUnitCard(cardID, gridPosIdx, playerUnitCamp);
        }
        
        public void EndRound()
        {
            
            BattleTypeManager.EndAction();

        }
        
        public Data_Card GetCard(int cardIdx)
        {
            return BattleTypeManager.GetCard(cardIdx);
        }
        
        public Data_EnergyBuff GetEnergyBuff(EUnitCamp unitCamp, int heart, int hp)
        {
            return BattleTypeManager.GetEnergyBuff(unitCamp, heart, hp);

        }

        public void UseCard(int cardID, int posIdx)
        {
            BattleTypeManager.UseCard(cardID, posIdx);
            
            var cardData = BattleManager.Instance.GetCard(cardID);
            
            if (!Input.GetMouseButtonUp(0))
            {
                return;
            }
            
            if(BattleManager.Instance.BattleState != EBattleState.UseCard)
                return;

            
            if(cardData.UnUse)
                return;

            
            if(!BattleCardManager.Instance.PreUseCard(cardID))
                return;
        }

        public int GetUnUseCardCount()
        {
            var unUseCount =
                BattleBuffManager.Instance.GetHurtTimes(GamePlayManager.Instance.GamePlayData, EBuffID.Spec_HurtUnUseCard);
            
            if (BattleCurseManager.Instance.BattleCurseData.CurseIDs.Contains(ECurseID.RandomCardUnUse))
            {
                unUseCount += 1;
            }

            return unUseCount;
        }

        public bool IsOddRound()
        {
            return (BattleManager.Instance.BattleData.Round + 1) % 2 != 0;
        }

    }
}
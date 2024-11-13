using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    public class BattleCurseManager : Singleton<BattleCurseManager>
    {
        public Random Random;
        private int randomSeed;

        public List<ECurseID> CurseIDs => DataManager.Instance.DataGame.User.CurGamePlayData.EnemyData.BattleCurseData.CurseIDs;
        
        public Data_BattleCurse BattleCurseData => DataManager.Instance.DataGame.User.CurGamePlayData.EnemyData.BattleCurseData;
        
        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new Random(randomSeed);
            //CurseIDs.Add(ECurseID.UnitCardAddEnengy_OddRound);
        }
        
        public void Destory()
        {}

        private void RecordRandonUnitID(ECurseID curseID, int unitID)
        {
            if (!BattleCurseData.RandomUnitIDs.ContainsKey(curseID))
            {
                BattleCurseData.RandomUnitIDs.Add(curseID, unitID);
            }
            else
            {
                BattleCurseData.RandomUnitIDs[curseID] = unitID;
            }
        }

        public void CacheRoundStartDatas()
        {
            var gamePlayData = BattleFightManager.Instance.RoundFightData.GamePlayData;
            CacheAttackMostUnit_Select();
            CacheRandomUnitUnRecover(gamePlayData);
            CacheRandomUnitUnAttack(gamePlayData);
            CacheRandomUnitAttackRecoverHP(gamePlayData);
            CacheRandomUnitUnHurt(gamePlayData);
            CacheRandomUnitDoubleDamage(gamePlayData);
            CacheRandomUnitUnMove(gamePlayData);
            CacheRandomUnitClearDebuff(gamePlayData);
            //CacheEachRoundBreakEmptyGrid(gamePlayData);
        }
        
        public void RandomUnitState(Data_GamePlay gamePlayData, ECurseID curseID, EUnitCamp unitCamp, EUnitState unitState, int count = 0)
        {
            // if (GameUtility.ContainRoundState(gamePlayData, ECardID.RoundCurseUnEffect))
            //     return;
            
            if (!gamePlayData.EnemyData.BattleCurseData.CurseIDs.Contains(curseID))
                return;
            
            if(BattleCurseData.RandomUnitIDs.ContainsKey(curseID))
                return;
                
            var actionData = new ActionData();
            actionData.ActionDataType = EActionDataType.Curse;
            BattleFightManager.Instance.RoundFightData.RoundStartBuffDatas.Add((int)curseID, actionData);

            var units = new List<Data_BattleUnit>();
            foreach (var kv in gamePlayData.BattleData.BattleUnitDatas)
            {
                if (kv.Value.UnitCamp == unitCamp)
                {
                    units.Add(kv.Value);
                }
            }

            
            if (units.Count <= count)
                return;

            var randomIdx = Random.Next(0, units.Count);
            var effectUnit = units[randomIdx];
            RecordRandonUnitID(curseID, effectUnit.ID);
                
            effectUnit.ChangeState(unitState);
                
            actionData.AddEmptyTriggerDataList(effectUnit.ID);
            var triggerData = BattleFightManager.Instance.Unit_State(actionData.TriggerDatas[effectUnit.ID], -1, -1,
                effectUnit.ID, unitState, 1, ETriggerDataType.Curse);
                
                
            BattleBuffManager.Instance.CacheTriggerData(triggerData, actionData.TriggerDatas[effectUnit.ID]);
        }

        public void CacheRandomUnitUnRecover(Data_GamePlay gamePlayData)
        {
            RandomUnitState(gamePlayData, ECurseID.RandomUnitUnRecover, EUnitCamp.Player1, EUnitState.UnRecover);
        }
        
        public void CacheRandomUnitAttackRecoverHP(Data_GamePlay gamePlayData)
        {
            RandomUnitState(gamePlayData, ECurseID.RandomUnitAttackRecoverHP, EUnitCamp.Enemy, EUnitState.RecoverHP);
        }
        
        public void CacheRandomUnitUnAttack(Data_GamePlay gamePlayData)
        {
            RandomUnitState(gamePlayData, ECurseID.RandomUnitUnAttack, EUnitCamp.Player1, EUnitState.UnAtk);
        }
        
        public void CacheRandomUnitUnHurt(Data_GamePlay gamePlayData)
        {
            RandomUnitState(gamePlayData, ECurseID.RandomUnitUnRecover, EUnitCamp.Enemy, EUnitState.UnHurt, 1);
        }
        
        public void CacheRandomUnitUnMove(Data_GamePlay gamePlayData)
        {
            RandomUnitState(gamePlayData, ECurseID.RandomUnitUnMove, EUnitCamp.Player1, EUnitState.UnMove, 1);
        }

        public void CacheRandomUnitDoubleDamage(Data_GamePlay gamePlayData)
        {
            RandomUnitState(gamePlayData, ECurseID.RandomUnitDoubleDamage, EUnitCamp.Enemy, EUnitState.DoubleDmg);
        }
        
        public void CacheRandomUnitClearDebuff(Data_GamePlay gamePlayData)
        {
            if (GameUtility.ContainRoundState(gamePlayData, EBuffID.Spec_CurseUnEffect))
                return;
            
            if (!gamePlayData.EnemyData.BattleCurseData.CurseIDs.Contains(ECurseID.RandomUnitClearDebuff))
                return;
            
            Data_BattleUnit randomUnit = null;
            var actionData = new ActionData();
            
            if(BattleCurseData.RandomUnitIDs.ContainsKey(ECurseID.RandomUnitClearDebuff))
            {
                if (gamePlayData.BattleData.BattleUnitDatas.ContainsKey(
                    BattleCurseData.RandomUnitIDs[ECurseID.RandomUnitClearDebuff]))
                {
                    randomUnit =
                        gamePlayData.BattleData.BattleUnitDatas[
                            BattleCurseData.RandomUnitIDs[ECurseID.RandomUnitClearDebuff]];
                }
                
            }
            else
            {
                var units = new List<Data_BattleUnit>();
                foreach (var kv in gamePlayData.BattleData.BattleUnitDatas)
                {
                    if (kv.Value.UnitCamp == EUnitCamp.Enemy)
                    {
                        units.Add(kv.Value);
                    }
                }

                if (units.Count <= 0)
                    return;
                
                var randomIdx = Random.Next(0, units.Count);
                randomUnit = units[randomIdx];
                BattleCurseData.RandomUnitIDs.Add(ECurseID.RandomUnitClearDebuff, randomUnit.ID);
                
                
            }

            
            if (randomUnit == null)
                return;
            
            actionData.ActionDataType = EActionDataType.Curse;
            BattleFightManager.Instance.RoundFightData.RoundStartBuffDatas.Add((int)ECurseID.RandomUnitClearDebuff, actionData);
            
            
            var unitStates = new List<UnitStateData>() { randomUnit.UnitState, randomUnit.RoundUnitState };

            foreach (var unitStateData in unitStates)
            {
                foreach (var unitState in unitStateData.UnitStates.Keys.ToList())
                {
                    if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.Negative].Contains(unitState))
                    {
                        actionData.AddEmptyTriggerDataList(randomUnit.ID);
                        var triggerData = BattleFightManager.Instance.Unit_State(actionData.TriggerDatas[randomUnit.ID], -1, -1, randomUnit.ID,
                            unitState, -1, ETriggerDataType.RoleState);
                        
                        BattleBuffManager.Instance.CacheTriggerData(triggerData, actionData.TriggerDatas[randomUnit.ID]);
                    }
                }
            }
        }

        public void CacheAttackMostUnit_Select()
        {
            if (GameUtility.ContainRoundState(GamePlayManager.Instance.GamePlayData, EBuffID.Spec_CurseUnEffect))
                return;
            
            if (!CurseIDs.Contains(ECurseID.AttackMostUnit))
                return;
            
            if(BattleCurseData.RandomUnitIDs.ContainsKey(ECurseID.AttackMostUnit))
                return;
            
            BattleCurseData.RandomUnitIDs.Add(ECurseID.AttackMostUnit, 0);

            var maxUnitCount = 0;
            var unitCount = 0;
            var posIdxs = new List<int>();
            for (int i = 0; i < Constant.Area.GridSize.x; i++)
            {
                posIdxs.Clear();
                unitCount = 0;
                for (int j = 0; j < Constant.Area.GridSize.y; j++)
                {
                    var posIdx = GameUtility.GridCoordToPosIdx(new Vector2Int(i, j));
                    var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(posIdx);
                    if (unit != null)
                    {
                        unitCount += 1;
                    }
                    posIdxs.Add(posIdx);
                }
            
                if (unitCount > maxUnitCount)
                {
                    maxUnitCount = unitCount;
                    BattleCurseData.AttackRowOrCol_PosIdxs.Clear();
                    BattleCurseData.AttackRowOrCol_PosIdxs.AddRange(posIdxs);
                }
            }
            
            for (int i = 0; i < Constant.Area.GridSize.y; i++)
            {
                posIdxs.Clear();
                unitCount = 0;
                for (int j = 0; j < Constant.Area.GridSize.x; j++)
                {
                    var posIdx = GameUtility.GridCoordToPosIdx(new Vector2Int(j, i));
                    var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(posIdx);
                    if (unit != null)
                    {
                        unitCount += 1;
                    }
                    posIdxs.Add(posIdx);
                }
            
                if (unitCount > maxUnitCount)
                {
                    maxUnitCount = unitCount;
                    BattleCurseData.AttackRowOrCol_PosIdxs.Clear();
                    BattleCurseData.AttackRowOrCol_PosIdxs.AddRange(posIdxs);
                }
            }

        }
        
        public ActionData CacheAttackMostUnit_Attack()
        {
            var actionData = new ActionData();
            actionData.ActionUnitID = -1;
            var triggerDatas = new List<TriggerData>();

            foreach (var posIdx in BattleCurseData.AttackRowOrCol_PosIdxs)
            {
                var unit = GameUtility.GetUnitByGridPosIdx(posIdx);
                if(unit == null)
                    continue;

                var triggerData = BattleFightManager.Instance.BattleRoleAttribute(-1, -1,
                    unit.ID, EUnitAttribute.HP, -1, ETriggerDataSubType.Curse);
                
                BattleBuffManager.Instance.CacheTriggerData(triggerData, triggerDatas);
                
                triggerDatas.Add(triggerData);
            }
            
            if (triggerDatas.Count > 0)
            {
                BattleFightManager.Instance.RoundFightData.RoundEndDatas.Add(-1, actionData);
                actionData.TriggerDatas.Add(-1, triggerDatas);
                
            }

            return actionData;
        }

        public void CacheUnitDeadRecoverLessHPUnit(int oldHP, int curHP, List<TriggerData> triggerDatas)
        {
            if(curHP > 0)
                return;
            
            if (GameUtility.ContainRoundState(GamePlayManager.Instance.GamePlayData, EBuffID.Spec_CurseUnEffect))
                return;
            
            if (!CurseIDs.Contains(ECurseID.UnitDeadRecoverLessHPUnit))
                return;
            
            var units = new List<Data_BattleUnit>();
            foreach (var kv in GamePlayManager.Instance.GamePlayData.BattleData.BattleUnitDatas)
            {
                if (kv.Value.UnitCamp == EUnitCamp.Enemy)
                {
                    units.Add(kv.Value);
                }
            }
            
            
            if (units.Count > 0)
            {
                units.Sort((unit1, unit2) =>
                {
                    return unit1.CurHP - unit2.CurHP;
                });
                
                var triggerData = BattleFightManager.Instance.BattleRoleAttribute(units[0].ID, units[0].ID,
                    units[0].ID, EUnitAttribute.HP, oldHP - curHP, ETriggerDataSubType.Curse);
                
                triggerDatas.Add(triggerData);
            }
        }

        public void AllUnitDodgeSubHeartDamageDict_Add(int enemyID)
        {
            if (!CurseIDs.Contains(ECurseID.AllUnitDodgeSubHeartDamage))
                return;

            if (!BattleCurseData.AllUnitDodgeSubHeartDamage_Dict.ContainsKey(enemyID))
            {
                BattleCurseData.AllUnitDodgeSubHeartDamage_Dict.Add(enemyID, 1);
            }

        }
        
        public int GetAllUnitDodgeSubHeartDamageValue(Data_GamePlay gamePlayData, int enemyID)
        {
            var allUnitDodgeSubHeartDamageDict = gamePlayData.EnemyData.BattleCurseData.AllUnitDodgeSubHeartDamage_Dict;
            if (allUnitDodgeSubHeartDamageDict.ContainsKey(enemyID))
            {
                return allUnitDodgeSubHeartDamageDict[enemyID];
            }
            else
            {
                return 0;
            }
                
        }

        public int SameUnitSameCurHP_GetUnitHP(int cardID)
        {
            var card = BattleManager.Instance.GetCard(cardID);
            var drCard = CardManager.Instance.GetCardTable(cardID);
            var hp = drCard.HP + card.FuneCount(EBuffID.Spec_AddMaxHP);
            
            if (BattleManager.Instance.TempTriggerData.UnitData is Data_BattleSolider solider &&
                solider.CardIdx == cardID)
            {
                hp = BattleManager.Instance.TempTriggerData.UnitData.MaxHP;
            }
            
            if (BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.SameUnitSameCurHP) &&
                !GameUtility.ContainRoundState(GamePlayManager.Instance.GamePlayData, EBuffID.Spec_CurseUnEffect))
            {
                var units = BattleManager.Instance.BattleData.ContainUnit(drCard.Id);
                if (units.Count > 0)
                {
                    units.Sort((unit1, unit2) =>
                    {
                        return unit1.CurHP - unit2.CurHP;
                    });

                    hp =
                        hp < units[0].CurHP
                            ? hp
                            : units[0].CurHP;
                }
                
            }
            
            
            
            return hp;
        }

        // public bool IsTacticCardUnDamage(int cardID)
        // {
        //     return Constant.Card.DamageTacticCard.Contains(cardID) &&
        //            (BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.TacticCardUnDamage_OddRound) &&
        //             BattleManager.Instance.IsOddRound() ||
        //             BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.TacticCardUnDamage_EvenRound) &&
        //             !BattleManager.Instance.IsOddRound());
        //
        // }

        public bool IsLinkUnEffect()
        {
            return  BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.LinkUnEffect_OddRound) &&
                    BattleManager.Instance.IsOddRound() ||
                    BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.LinkUnEffect_EvenRound) &&
                    !BattleManager.Instance.IsOddRound();

        }

        public bool IsAddEnergyCard(ECardType cardType)
        {
            return cardType == ECardType.Unit &&
                   BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.UnitCardAddEnengy_OddRound) &&
                   BattleManager.Instance.IsOddRound() ||
                   cardType == ECardType.Unit &&
                   BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.UnitCardAddEnengy_EvenRound) &&
                   !BattleManager.Instance.IsOddRound() ||
                   cardType == ECardType.Tactic &&
                   BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.TacticCardAddEnengy_OddRound) &&
                   BattleManager.Instance.IsOddRound() ||
                   cardType == ECardType.Tactic &&
                   BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.TacticCardAddEnengy_EvenRound) &&
                   !BattleManager.Instance.IsOddRound();

        }
        
        public bool IsPlayerBuffUnEffect(ECardType cardType)
        {
            return cardType == ECardType.Unit &&
                   BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.PlayerBuffUnEffect_OddRound) &&
                   BattleManager.Instance.IsOddRound() ||
                   cardType == ECardType.Unit &&
                   BattleCurseManager.Instance.CurseIDs.Contains(ECurseID.PlayerBuffUnEffect_EvenRound) &&
                   !BattleManager.Instance.IsOddRound();

        }

        
        public void RoundStartTrigger()
        {
            //EachRoundBreakEmptyGrid();
        }
        
        public void EachRoundBreakEmptyGrid()
        {
            var emptyGrids = new List<int>();
            foreach (var kv in BattleManager.Instance.BattleData.GridTypes)
            {
                if (kv.Value == EGridType.Empty)
                {
                    emptyGrids.Add(kv.Key);
                }
            }

            var randomEmptyGridPosIdx = Random.Next(0, emptyGrids.Count);

            BattleManager.Instance.BattleData.GridTypes[emptyGrids[randomEmptyGridPosIdx]] = EGridType.Obstacle;
            var gridEntity = BattleAreaManager.Instance.GetGridEntityByGridPosIdx(emptyGrids[randomEmptyGridPosIdx]);
            gridEntity.BattleGridEntityData.GridType = EGridType.Obstacle;
            gridEntity.Show(true);
            
            // foreach (var kv in BattleManager.Instance.BattleData.GridTypes)
            // {
            //     var gridEntity = BattleAreaManager.Instance.GetGridEntityByGridPosIdx(kv.Key);
            //     gridEntity.Show(true);
            // }
            
            BattleManager.Instance.Refresh();
        }
    }
}
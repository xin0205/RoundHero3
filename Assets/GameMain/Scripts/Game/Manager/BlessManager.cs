using System;
using System.Collections.Generic;
using System.Linq;

namespace RoundHero
{
    public class BlessManager : Singleton<BlessManager>
    {
        public Dictionary<int, Data_Bless> BlessDatas => BattlePlayerManager.Instance.PlayerData.BlessDatas;
        
        public System.Random Random;
        private int randomSeed;

        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
        }

        public void Destory()
        {
            
        }

        public int GetIdx()
        {
            return BattlePlayerManager.Instance.PlayerData.BlessIdx++;
        }

        // public void AddBless(EBlessID blessID)
        // {
        //     var id = GetID();
        //     BlessDatas.Add(id, new Data_Bless(id, blessID));
        // }

        public Data_Bless GetBless(int blessID)
        {
            if (BlessDatas.ContainsKey(blessID))
                return BlessDatas[blessID];
            
            return null;
        }
        
        public void TriggerAction(List<EBlessID> blessIDs)
        {
            foreach (var kv in BlessDatas)
            {
                var drBless = GameEntry.DataTable.GetBless(kv.Value.BlessID);
                if (blessIDs.Contains(drBless.BlessID) && BattleFightManager.Instance.RoundFightData.BlessTriggerDatas.ContainsKey(kv.Value.Idx))
                {
                    foreach (var kv2 in BattleFightManager.Instance.RoundFightData.BlessTriggerDatas[kv.Value.Idx].TriggerDatas)
                    {
                        foreach (var triggerData in kv2.Value)
                        {
                            BattleFightManager.Instance.TriggerAction(triggerData);
                        }
                        
                    }
                    
                }
            }
        }
        
        public void StartActionTrigger()
        {
            TriggerAction(new List<EBlessID>(){EBlessID.UnUseCardAddCurHP});
        }
        
        public void CacheRoundStartDatas()
        {
            var gamePlayData = BattleFightManager.Instance.RoundFightData.GamePlayData;
            CacheUnUseCardAddHeroHP(gamePlayData);
        }
        
        public void CacheUnUseCardAddHeroHP(Data_GamePlay gamePlayData)
        {
            var unUseCardAddHeroHP = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.UnUseCardAddCurHP, BattleManager.Instance.CurUnitCamp);
            var playerData = gamePlayData.GetPlayerData(BattleManager.Instance.CurUnitCamp);
            
            if (unUseCardAddHeroHP != null &&
                BattlePlayerManager.Instance.BattlePlayerData.RoundUseCardCount <= 0)
            {
                var actionData = new ActionData();
                actionData.ActionDataType = EActionDataType.Bless;
                BattleFightManager.Instance.RoundFightData.RoundStartBuffDatas.Add(unUseCardAddHeroHP.Idx, actionData);
                
                var triggerBlessData = BattleFightManager.Instance.BattleRoleAttribute(-1, -1, playerData.BattleHero.Idx,
                    EUnitAttribute.HP, 1, ETriggerDataSubType.Bless);
                
                actionData.AddEmptyTriggerDataList(playerData.BattleHero.Idx);
                //actionData.AddTriggerData(playerData.BattleHero.Idx, triggerBlessData, playerData.BattleHero);
                BattleBuffManager.Instance.CacheTriggerData(triggerBlessData,
                    BattleFightManager.Instance.RoundFightData.RoundStartBuffDatas[unUseCardAddHeroHP.Idx]
                        .TriggerDatas[playerData.BattleHero.Idx]);

            }
        }
        
        public void RoundStartTrigger(Data_GamePlay gamePlayData)
        {
            EachRoundAddHeroHPInBigFight(gamePlayData);
            EachRoundAddAllEnemyDebuff(gamePlayData);
            AddBuffToNoBuffUs(gamePlayData);
            EachRoundAcquireCard();
        }

        public void EachRoundAcquireCard()
        {
            var eachRoundAcquireCardCount = GamePlayManager.Instance.GamePlayData.BlessCount(EBlessID.EachRoundAcquireCard, BattleManager.Instance.CurUnitCamp);
            if (eachRoundAcquireCardCount > 0)
            {
                var drConsumeCardAcquireNewCard = GameEntry.DataTable.GetBless(EBlessID.ConsumeCardAddRandomCard);
                var newCardIDs = BattleCardManager.Instance.AddRandomCard(BattleBuffManager.Instance.GetBuffValue(drConsumeCardAcquireNewCard.Values1[0]) * eachRoundAcquireCardCount);
                for (int i = 0; i < newCardIDs.Count; i++)
                {
                    var newCardID = newCardIDs[i];
                    GameUtility.DelayExcute(1f + 0.5f * i, () =>
                    {
                         BattleCardManager.Instance.NewCardToHand(newCardID);
                         var card = BattleManager.Instance.GetCard(newCardID);
                         card.IsUseConsume = true;
                    });
                }
                
            }
        }
        
        public void EachRoundAddAllEnemyDebuff(Data_GamePlay gamePlayData)
        {
            var eachRoundAddAllEnemyDebuffCount = gamePlayData.BlessCount(EBlessID.EachRoundAddAllEnemyDebuff, BattleManager.Instance.CurUnitCamp);
            var drEachRoundAddAllEnemyDebuff = GameEntry.DataTable.GetBless(EBlessID.EachRoundAddAllEnemyDebuff);
            if (eachRoundAddAllEnemyDebuffCount > 0 && gamePlayData.BattleData.Round > 0 &&
                gamePlayData.BattleData.Round % BattleBuffManager.Instance.GetBuffValue(drEachRoundAddAllEnemyDebuff.Values1[0]) == 0)
            {
                foreach (var kv in gamePlayData.BattleData.BattleUnitDatas)
                {
                    if (kv.Value.UnitCamp != BattleManager.Instance.CurUnitCamp)
                        continue;
                    
                    if (kv.Value.CurHP <= 0)
                        continue;
                    
                    var randomDebuffIdx = Random.Next(0, Constant.Battle.EffectUnitStates[EUnitStateEffectType.Negative].Count);
                    var randomDeBuff = Constant.Battle.EffectUnitStates[EUnitStateEffectType.Negative][randomDebuffIdx];
                    kv.Value.ChangeState(randomDeBuff, eachRoundAddAllEnemyDebuffCount);
                    
                }
                
            }
            
        }

        public void AddBuffToNoBuffUs(Data_GamePlay gamePlayData)
        {
            var addBuffToNoBuffUsCount = gamePlayData.BlessCount(EBlessID.AddBuffToNoBuffUs, BattleManager.Instance.CurUnitCamp);
            var drAddBuffToNoBuffUs = GameEntry.DataTable.GetBless(EBlessID.AddBuffToNoBuffUs);
            if (addBuffToNoBuffUsCount > 0)
            {
                foreach (var kv in gamePlayData.BattleData.BattleUnitDatas)
                {
                    if (kv.Value.UnitCamp != BattleManager.Instance.CurUnitCamp)
                        continue;
                    
                    if (kv.Value.CurHP <= 0)
                        continue;
                    
                    if (kv.Value.UnitState.UnitStates.Count > 0)
                        continue;
                    
                    var randomBuffIdx = Random.Next(0, Constant.Battle.EffectUnitStates[EUnitStateEffectType.Positive].Count);
                    var randomBuff = Constant.Battle.EffectUnitStates[EUnitStateEffectType.Positive][randomBuffIdx];
                    kv.Value.ChangeState(randomBuff, BattleBuffManager.Instance.GetBuffValue(drAddBuffToNoBuffUs.Values1[0]) * addBuffToNoBuffUsCount);
                    
                }
                
            }
        }
        
        public void EachUseCard(Data_GamePlay gamePlayData, int cardID, int unitID = -1)
        {
            var drCard = CardManager.Instance.GetCardTable(cardID);
            if(drCard == null)
                return;
            
            if (drCard.CardType == ECardType.Unit)
            {
                EachRoundUseCardAttackAllEnemy(gamePlayData, EBlessID.EachRoundUseFightCardAttackAllEnemy);
            }
            else if (drCard.CardType == ECardType.Tactic)
            {
                EachRoundUseCardAttackAllEnemy(gamePlayData, EBlessID.EachRoundUseTacticCardAttackAllEnemy);
            }
            
            EachRoundUseUnitCardAddDefense(gamePlayData, cardID);
            
            EachUseCardDoubleHPDelta(gamePlayData, cardID);
            
            EachUseCardAcquireCard(gamePlayData, cardID);
            
            UseCardSubOtherCardEnergy(gamePlayData, cardID);
            
            NoHandCardAcquireCard(gamePlayData, cardID);
            
            EachUseCardUnUseEnergy(gamePlayData, cardID, unitID);
            
            UseCardRandomCard0Energy(gamePlayData, cardID);

        }
        
        public void UseCardSubOtherCardEnergy(Data_GamePlay gamePlayData, int cardID)
        {
            var card = BattleManager.Instance.GetCard(cardID);
            var drCard = CardManager.Instance.GetCardTable(cardID);
            var playerBattleData = gamePlayData.BattleData.GetBattlePlayerData(BattleManager.Instance.CurUnitCamp);
            
            var useCardSubOtherCardEnergy = gamePlayData.GetUsefulBless(EBlessID.UseCardSubOtherCardEnergy, BattleManager.Instance.CurUnitCamp);
            if (useCardSubOtherCardEnergy != null)
            {
                foreach (var handCardID in playerBattleData.HandCards)
                {
                    var handCard = BattleManager.Instance.GetCard(handCardID);
                    var drHandCard = CardManager.Instance.GetCardTable(handCardID);
                    if (drHandCard.Energy < drCard.Energy)
                    {
                        handCard.RoundEnergyDelta += -1;
                    }
                }
                GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
                
            }
        }

        public void EachUseCardAcquireCard(Data_GamePlay gamePlayData, int cardID)
        {
            var eachUseCardAcquireCard = gamePlayData.GetUsefulBless(EBlessID.EachUseCardAcquireCard, BattleManager.Instance.CurUnitCamp);
            if (eachUseCardAcquireCard != null)
            {
                if (eachUseCardAcquireCard.Value > 0)
                {
                    eachUseCardAcquireCard.Value -= 1;

                    if (eachUseCardAcquireCard.Value <= 0)
                    {
                        var drBless = GameEntry.DataTable.GetBless(EBlessID.EachUseCardAcquireCard);
                        eachUseCardAcquireCard.Value = BattleBuffManager.Instance.GetBuffValue(drBless.Values1[0]);
                        BattleCardManager.Instance.AcquireCards(BattleBuffManager.Instance.GetBuffValue(drBless.Values1[1]));
                    }
                    
                }

                GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
                
            }
        }
        
        public void UseCardRandomCard0Energy(Data_GamePlay gamePlayData, int cardID)
        {
            var card = BattleManager.Instance.GetCard(cardID);
            var drCard = CardManager.Instance.GetCardTable(cardID);
            var cardEnergy = BattleCardManager.Instance.GetCardEnergy(cardID);
            var drBless = GameEntry.DataTable.GetBless(EBlessID.UseCardRandomCard0Energy);
            if(cardEnergy < BattleBuffManager.Instance.GetBuffValue(drBless.Values1[0]))
                return;
            
            var playerBattleData = gamePlayData.BattleData.GetBattlePlayerData(BattleManager.Instance.CurUnitCamp);
            
            var useCardRandomCard0EnergyCount = gamePlayData.BlessCount(EBlessID.UseCardRandomCard0Energy, BattleManager.Instance.CurUnitCamp);
            if (useCardRandomCard0EnergyCount > 0)
            {
                //var drConsumeCardAcquireNewCard = GameEntry.DataTable.GetBless(EBlessID.ConsumeCardAcquireNewCard);
                var cards = new List<int>();
                foreach (var _cardID in playerBattleData.HandCards)
                {
                    //var _card = CardManager.Instance.GetCard(_cardID);
                    if (BattleCardManager.Instance.GetCardEnergy(_cardID) > 0)
                    {
                        cards.Add(_cardID);
                    }
                }

                if (cards.Count > 0)
                {
                    var randomIdx = Random.Next(0, cards.Count);
                    var randomCardID = playerBattleData.HandCards[randomIdx];
                    var randomCard = BattleManager.Instance.GetCard(randomCardID);
                    randomCard.RoundEnergyDelta = -BattleCardManager.Instance.GetCardEnergy(randomCardID);
                }
                GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
            }
        }

        public void EachRoundUseCardAttackAllEnemy(Data_GamePlay gamePlayData, EBlessID blessID)
        {
            var eachRoundUseCardAttackAllEnemy =gamePlayData.GetUsefulBless(blessID, BattleManager.Instance.CurUnitCamp);
            var drBless = GameEntry.DataTable.GetBless(blessID);
            if (eachRoundUseCardAttackAllEnemy != null)
            {
                if (eachRoundUseCardAttackAllEnemy.Value > 0)
                {
                    eachRoundUseCardAttackAllEnemy.Value -= 1;
                }
                else
                {
                    foreach (var kv in gamePlayData.BattleData.BattleUnitDatas)
                    {
                        if (kv.Value.UnitCamp != BattleManager.Instance.CurUnitCamp)
                        {
                            BattleManager.Instance.ChangeHP(kv.Value, BattleBuffManager.Instance.GetBuffValue(drBless.Values1[1]), gamePlayData, EHPChangeType.Unit);
                        }
                    }
                }

            }
        }

        public void EachRoundUseUnitCardAddDefense(Data_GamePlay gamePlayData, int cardID)
        {
            var card = BattleManager.Instance.GetCard(cardID);
            var drCard = CardManager.Instance.GetCardTable(cardID);

            if (drCard.CardType == ECardType.Tactic)
                return;
            
            var playerData = gamePlayData.GetPlayerData(BattleManager.Instance.CurUnitCamp);

            var eachUseCardDoubleDamage = gamePlayData.GetUsefulBless(EBlessID.EachRoundUseUnitCardAddDefense, BattleManager.Instance.CurUnitCamp);
            var drBless = GameEntry.DataTable.GetBless(EBlessID.EachRoundUseUnitCardAddDefense);
            if (eachUseCardDoubleDamage != null)
            {
                if (eachUseCardDoubleDamage.Value > 0)
                {
                    eachUseCardDoubleDamage.Value -= 1;
                    
                    if (eachUseCardDoubleDamage.Value == 0)
                    {
                        eachUseCardDoubleDamage.Value = BattleBuffManager.Instance.GetBuffValue(drBless.Values1[0]);
                        playerData.BattleHero.ChangeState(EUnitState.HurtSubDmg, BattleBuffManager.Instance.GetBuffValue(drBless.Values1[1]));
                    }
                }

                GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
            }
        }
        
        public void EachUseCardDoubleHPDelta(Data_GamePlay gamePlayData, int cardID)
        {
            var eachUseCardDoubleHPDelta = gamePlayData.GetUsefulBless(EBlessID.EachUseCardDoubleHPDelta, BattleManager.Instance.CurUnitCamp);
            var drBless = GameEntry.DataTable.GetBless(EBlessID.EachUseCardDoubleHPDelta);
            
            var playerData = gamePlayData.GetPlayerData(BattleManager.Instance.CurUnitCamp);
            var playerBattleData = gamePlayData.BattleData.GetBattlePlayerData(BattleManager.Instance.CurUnitCamp);
            
            if (eachUseCardDoubleHPDelta != null)
            {
                var allCards = playerBattleData.GetAllCards();
                if (eachUseCardDoubleHPDelta.Value > 0)
                {
                    eachUseCardDoubleHPDelta.Value -= 1;
                    
                    if (eachUseCardDoubleHPDelta.Value == 0)
                    {
                        foreach (var _cardID in allCards)
                        {
                            var card = playerData.CardDatas[_cardID];
                            // var drCard = GameEntry.DataTable.GetCard(card.CardID);
                            // var drBuffs = CardManager.Instance.GetBuffTable(card.ID);

                            // if (card.CardID == ECardID.HurtUsDamage)
                            // {
                            //     card.UseCardDamageRatio += 1;
                            // }
                            
                            // if (drCard.CardType != ECardType.Fight)
                            // {
                            //     foreach (var drBuff in drBuffs)
                            //     {
                            //         if (drBuff.UnitAttribute == EUnitAttribute.CurHP)
                            //         {
                            //             card.UseCardDamageRatio += 1;
                            //         }
                            //     }
                            // }

                            foreach (var funeIdx in card.FuneIdxs)
                            {
                                var funeData = FuneManager.Instance.GetFuneData(funeIdx);
                                var buffDatas = BattleBuffManager.Instance.GetBuffData(funeData.FuneID);
                                if (buffDatas.Exists(buffData => buffData.UnitAttribute == EUnitAttribute.HP))
                                {
                                    card.UseCardDamageRatio += 1;
                                }

                            }

                        }

                        eachUseCardDoubleHPDelta.Value = BattleBuffManager.Instance.GetBuffValue(drBless.Values1[0]);
                    }
                    else
                    {
                        foreach (var _cardID in allCards)
                        {
                            var card = playerData.CardDatas[_cardID];
                            card.UseCardDamageRatio = 0;
                        
                        }
                    }
                }

                GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
            }
        }

        public void NoHandCardAcquireCard(Data_GamePlay gamePlayData, int cardID)
        {
            var playerData = gamePlayData.GetPlayerData(BattleManager.Instance.CurUnitCamp);
            var playerBattleData = gamePlayData.BattleData.GetBattlePlayerData(BattleManager.Instance.CurUnitCamp);
            
            var noHandCardAcquireCard = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.NoHandCardAcquireCard, BattleManager.Instance.CurUnitCamp);
            if (noHandCardAcquireCard != null && playerBattleData.HandCards.Count <= 0)
            {
                var drBless = GameEntry.DataTable.GetBless(EBlessID.NoHandCardAcquireCard);
                BattleCardManager.Instance.AcquireCards(BattleBuffManager.Instance.GetBuffValue(drBless.Values1[0]));

            }
        }

        public void EachUseCardUnUseEnergy(Data_GamePlay gamePlayData, int cardID, int unitID = -1)
        {
            
            var cardEnergy = BattleCardManager.Instance.GetCardEnergy(cardID, unitID);
            
            GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());

            if (cardEnergy > 0)
            {
                HeroManager.Instance.ChangeHP(-cardEnergy, EHPChangeType.CardConsume, true, false, true);
            }
            
            
        }

        public void EachRoundAddHeroHPInBigFight(Data_GamePlay gamePlayData)
        {
            var addHeroHPEachRoundInBigFight = gamePlayData.GetUsefulBless(EBlessID.EachRoundAddCurHPInBigBattle, BattleManager.Instance.CurUnitCamp);
            var drHeroHPEachRoundInBigFight = GameEntry.DataTable.GetBless(EBlessID.EachRoundAddCurHPInBigBattle);
            if (addHeroHPEachRoundInBigFight != null &&
                (gamePlayData.BattleData.EnemyType == EEnemyType.Boss ||
                 gamePlayData.BattleData.EnemyType == EEnemyType.Elite))
            {
                HeroManager.Instance.ChangeHP(BattleBuffManager.Instance.GetBuffValue(drHeroHPEachRoundInBigFight.Values1[0]), EHPChangeType.Bless, true, false, true);
            }
        }
        
        public void  EachRoundFightCardAddLink(Data_GamePlay gamePlayData, Data_BattleUnit unit, EBlessID blessID, ELinkID linkID)
        {
            var playerData = gamePlayData.GetPlayerData(BattleManager.Instance.CurUnitCamp);
            var playerBattleData = gamePlayData.BattleData.GetBattlePlayerData(BattleManager.Instance.CurUnitCamp);
            var eachRoundFightCardAddLink = gamePlayData.GetUsefulBless(blessID, BattleManager.Instance.CurUnitCamp);
            var drBless = GameEntry.DataTable.GetBless(blessID);
            if (eachRoundFightCardAddLink != null)
            {
                // && drCard.CardType == ECardType.Fight
                if (playerBattleData.RoundUseCardCount == 0)
                {
                    unit.BattleLinkIDs.Add(linkID);
                }
            }
        }

        
        public void DeadTrigger(Data_GamePlay gamePlayData, Data_BattleUnit unit)
        {
            if (unit.CurHP > 0)
            {
                return;
            }
            
            if (unit.UnitCamp != BattleManager.Instance.CurUnitCamp)
            {
                return;
            }
            
            var enemyDeadDebuffToOtherEnemy = gamePlayData.GetUsefulBless(EBlessID.EnemyDeadDebuffToOtherEnemy, BattleManager.Instance.CurUnitCamp);
            if (enemyDeadDebuffToOtherEnemy == null)
            {
                return;
            }
            
            
            var otherEnemies = new List<Data_BattleUnit>();
            foreach (var kv in gamePlayData.BattleData.BattleUnitDatas)
            {
                if (kv.Value.UnitCamp != BattleManager.Instance.CurUnitCamp && kv.Value.CurHP > 0)
                {
                    otherEnemies.Add(kv.Value);
                }
            }

            if (otherEnemies.Count > 0)
            {
                var randomEnemyIdx = Random.Next(0, otherEnemies.Count);
                foreach (var kv in unit.UnitState.UnitStates)
                {
                    if (Constant.Battle.EffectUnitStates[EUnitStateEffectType.Negative].Contains(kv.Key))
                    {
                        otherEnemies[randomEnemyIdx].ChangeState(kv.Key, kv.Value);
                    }
                }
            }
        }
        
        // public void RoundEnd()
        // {
        //     TriggerAction(new List<EBlessID>(){EBlessID.EachRoundAddHeroHPInBigFight});
        // }
        //
        //
        // public void CacheBeforeStartRound(Data_GamePlay gamePlayData)
        // {
        //     CacheAddHeroHPEachRoundInBigFight(gamePlayData);
        // }
        //
        // public void CacheAddHeroHPEachRoundInBigFight(Data_GamePlay gamePlayData)
        // {
        //     var addHeroHPEachRoundInBigFight = BattleManager.Instance.GamePlayData.GetUsefulBless(EBlessID.EachRoundAddHeroHPInBigFight);
        //     
        //     if (addHeroHPEachRoundInBigFight != null &&
        //         (gamePlayData.BattleData.EnemyType == EEnemyType.Boss ||
        //          gamePlayData.BattleData.EnemyType == EEnemyType.Elite))
        //     {
        //         gamePlayData.BattleHero.ChangeHP(1, out int useDefenseCount);
        //         
        //         var actionData = new ActionData();
        //         FightManager.Instance.RoundFightData.BlessTriggerDatas.Add(addHeroHPEachRoundInBigFight.ID, actionData);
        //         
        //         var triggerBlessData = FightManager.Instance.BattleRoleAttribute(-1, -1, gamePlayData.BattleHero.ID,
        //             EUnitAttribute.CurHP, 1, ETriggerDataSubType.Bless);
        //         
        //         actionData.AddTriggerData(gamePlayData.BattleHero.ID, triggerBlessData, gamePlayData.BattleHero);
        //         FightManager.Instance.SimulateTriggerData(triggerBlessData, FightManager.Instance.RoundFightData.BlessTriggerDatas[addHeroHPEachRoundInBigFight.ID].TriggerDatas[gamePlayData.BattleHero.ID]);
        //     }
        // }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using GameKit.Dependencies.Utilities;


namespace RoundHero
{
    public partial class BattleCardManager : Singleton<BattleCardManager>
    {
        public void CacheUseCardData(int cardIdx, Data_BattleUnit effectUnit, int actionUnitGridPosidx, int actionUnitIdx, bool isSwitchCard = false)
        {
            var drCard = CardManager.Instance.GetCardTable(cardIdx);
            var card = BattleFightManager.Instance.GetCard(cardIdx);
            var triggerDatas = new List<TriggerData>();

            BattleFightManager.Instance.RoundFightData.BuffData_Use.ActionUnitIdx = Constant.Battle.UnUnitTriggerIdx;
            
            var unComsumeCard = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.UnConsumeCard, PlayerManager.Instance.PlayerData.UnitCamp);
            
            if (unComsumeCard != null && card.CardDestination == ECardDestination.Consume)
            {
                var random = new Random(GetRandomSeed());

                BattleFightManager.Instance.RoundFightData.BuffData_Use.CardDestination = random.Next(0, 2) == 0 ? ECardDestination.Pass : ECardDestination.Consume;
            }
            else
            {
                BattleFightManager.Instance.RoundFightData.BuffData_Use.CardDestination = card.CardDestination;
            }
            

            var battlePlayerData =
                BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(
                    BattleFightManager.Instance.RoundFightData.GamePlayData.PlayerData
                        .UnitCamp);

            switch (BattleFightManager.Instance.RoundFightData.BuffData_Use.CardDestination)
            {
                case ECardDestination.Pass:
                    ToPassCard(battlePlayerData, cardIdx);
                    break;
                case ECardDestination.Consume:
                    ToConsumeCards(battlePlayerData, cardIdx);
                    break;
                case ECardDestination.StandBy:
                    ToStandByCards(battlePlayerData, cardIdx);
                    break;
                default:
                    break;
            }
            
            BattleFightManager.Instance.RoundFightData.BuffData_Use.UseCardCirculation
                .HandCards = new List<int>(battlePlayerData.HandCards);
            
            BattleFightManager.Instance.RoundFightData.BuffData_Use.UseCardCirculation
                .ConsumeCards = new List<int>(battlePlayerData.ConsumeCards);
            
            BattleFightManager.Instance.RoundFightData.BuffData_Use.UseCardCirculation
                .PassCards = new List<int>(battlePlayerData.PassCards);
            
            BattleFightManager.Instance.RoundFightData.BuffData_Use.UseCardCirculation
                .StandByCards = new List<int>(battlePlayerData.StandByCards);

            if (!isSwitchCard)
            {
                foreach (var buffIDStr in drCard.BuffIDs)
                {
                    var buffData = BattleBuffManager.Instance.GetBuffData(buffIDStr);
                    var values = drCard.Values0;
                    // foreach (var value in drCard.Values1)
                    // {
                    //     values.Add(BattleBuffManager.Instance.GetBuffValue(value, effectUnit != null ? effectUnit.Idx : -1));
                    // }
                
                
                    //BattleBuffManager.Instance.CacheBuffData(buffData, camp, effectUnit, values, 1 + card.UseCardDamageRatio);
                    //
                    if (drCard.CardType == ECardType.Tactic)
                    {
                        BattleBuffManager.Instance.BuffTrigger(buffData.BuffTriggerType, buffData, values, actionUnitIdx, actionUnitIdx,
                            effectUnit != null ? effectUnit.Idx : -1, triggerDatas, actionUnitGridPosidx, -1, cardIdx, -1, ETriggerDataSubType.Card);

                        foreach (var triggerData in triggerDatas)
                        {
                            if (!BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDataDict.ContainsKey(
                                    triggerData.EffectUnitIdx))
                            {
                                BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDataDict.Add(triggerData.EffectUnitIdx, new TriggerCollection());
                            }

                            BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDataDict[triggerData.EffectUnitIdx]
                                .TriggerDatas.Add(triggerData);                          
                            

                            BattleFightManager.Instance.CacheUnitActiveMoveDatas(Constant.Battle.UnUnitTriggerIdx,
                                triggerData.EffectUnitGridPosIdx, triggerData.BuffValue.BuffData,
                                BattleFightManager.Instance.RoundFightData.BuffData_Use,
                                triggerData, triggerData.ActionUnitGridPosIdx);
                        }
                    }

                }
            }
            

            //CacheUseCard(cardIdx, effectUnit, actionUnitGridPosidx, actionUnitIdx, triggerDatas);
            
            //var drCard = CardManager.Instance.GetCardTable(cardIdx);
            
            
            if (drCard.CardType == ECardType.Tactic)
            {
                BattleFightManager.Instance.RoundFightData.BuffData_Use.ActionDataType = EActionDataType.Tactic;
            }
            else if (drCard.CardType == ECardType.Unit)
            {
                BattleFightManager.Instance.RoundFightData.BuffData_Use.ActionDataType = EActionDataType.Unit;
            }

            
            BattleFightManager.Instance.RoundFightData.BuffData_Use.CardEnergy = BattleFightManager.Instance.CacheConsumeCardEnergy(cardIdx, triggerDatas);

            
            foreach (var funeIdx in card.FuneIdxs)
            {
                var drBuff = FuneManager.Instance.GetBuffTable(funeIdx);
                var idx = 0;
                foreach (var buffIDStr in drBuff.BuffIDs)
                {
                    var buffData = BattleBuffManager.Instance.GetBuffData(buffIDStr);
                    var values = drBuff.GetValues(idx++);

                    BattleBuffManager.Instance.BuffTrigger(EBuffTriggerType.Use, buffData, values, actionUnitIdx, actionUnitIdx,
                        effectUnit != null ? effectUnit.Idx : -1, triggerDatas, actionUnitGridPosidx, -1, cardIdx, funeIdx, ETriggerDataSubType.Fune);
  
                    BattleFightManager.Instance.RoundFightData.BuffData_Use.ActionDataType = EActionDataType.Fune;

                }

            }

            if (BattleFightManager.Instance.RoundFightData.BuffData_Use.CardDestination == ECardDestination.Consume)
            {
                var consumeCardAddCurHP = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.ConsumeCardAddCurHP, PlayerManager.Instance.PlayerData.UnitCamp);
                var consumeCardTriggerDatas = new List<TriggerData>();
                var addHP = BlessManager.Instance.ConsumeCardAddCurHP(GamePlayManager.Instance.GamePlayData);
                if (addHP > 0)
                {
                    var triggerData = BattleFightManager.Instance.Unit_HeroAttribute(Constant.Battle.UnUnitTriggerIdx,
                        Constant.Battle.UnUnitTriggerIdx, BattleFightManager.Instance.PlayerData.BattleHero.Idx, EHeroAttribute.HP, addHP);
                    triggerData.TriggerBlessIdx = consumeCardAddCurHP.BlessIdx;
                    triggerData.TriggerDataSubType = ETriggerDataSubType.Bless;
                    BattleBuffManager.Instance.CacheTriggerData(triggerData, consumeCardTriggerDatas);
                    BattleFightManager.Instance.RoundFightData.BuffData_Use.ConsumeCardDatas.Add(cardIdx, consumeCardTriggerDatas);
                    
                }
            }
            
            BlessManager.Instance.CacheUseCardData(EBlessID.EachRoundUseCardAttackAllEnemy, triggerDatas);
            
            if (triggerDatas.Count > 0)
            {
                // if (!BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDataDict.ContainsKey(effectUnit.Idx))
                // {
                //     BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDataDict.Add(effectUnit.Idx, new TriggerCollection());
                // }
                //
                // var triggerCollection =
                //     BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDataDict[effectUnit.Idx];
                //
                // triggerCollection.ActionUnitIdx = Constant.Battle.UnUnitTriggerIdx;
                // triggerCollection.EffectUnitIdx = effectUnit.Idx;
                // triggerCollection.TriggerDatas = triggerDatas;
                
                //BattleFightManager.Instance.CalculateHeroHPDelta(BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDataDict);
            }
            BattleFightManager.Instance.CalculateHeroHPDelta(BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDataDict);

            
            
            

        }

        // public void CacheUseCard(int cardIdx, Data_BattleUnit effectUnit, int actionUnitGridPosidx, int actionUnitIdx, List<TriggerData> triggerDatas)
        // {
        //     var drCard = CardManager.Instance.GetCardTable(cardIdx);
        //     var card = BattleFightManager.Instance.GetCard(cardIdx);
        //     
        //     if (drCard.CardType == ECardType.Tactic)
        //     {
        //         BattleFightManager.Instance.RoundFightData.BuffData_Use.ActionDataType = EActionDataType.Tactic;
        //     }
        //     else if (drCard.CardType == ECardType.Unit)
        //     {
        //         BattleFightManager.Instance.RoundFightData.BuffData_Use.ActionDataType = EActionDataType.Unit;
        //     }
        //
        //     
        //     BattleFightManager.Instance.RoundFightData.BuffData_Use.CardEnergy = BattleFightManager.Instance.CacheConsumeCardEnergy(cardIdx, triggerDatas);
        //
        //     
        //     foreach (var funeIdx in card.FuneIdxs)
        //     {
        //         var drBuff = FuneManager.Instance.GetBuffTable(funeIdx);
        //         var idx = 0;
        //         foreach (var buffIDStr in drBuff.BuffIDs)
        //         {
        //             var buffData = BattleBuffManager.Instance.GetBuffData(buffIDStr);
        //             var values = drBuff.GetValues(idx++);
        //
        //             BattleBuffManager.Instance.BuffTrigger(EBuffTriggerType.Use, buffData, values, actionUnitIdx, actionUnitIdx,
        //                 effectUnit != null ? effectUnit.Idx : -1, triggerDatas, actionUnitGridPosidx, -1, cardIdx, funeIdx, ETriggerDataSubType.Fune);
        //
        //             BattleFightManager.Instance.RoundFightData.BuffData_Use.ActionDataType = EActionDataType.Fune;
        //
        //         }
        //
        //     }
        //     
        //     
        //
        //     var unComsumeCard = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.UnConsumeCard, PlayerManager.Instance.PlayerData.UnitCamp);
        //     
        //     if (unComsumeCard != null && card.CardDestination == ECardDestination.Consume)
        //     {
        //         BattleFightManager.Instance.RoundFightData.BuffData_Use.CardDestination = Random.Next(0, 2) == 0 ? ECardDestination.Pass : ECardDestination.Consume;
        //     }
        //     else
        //     {
        //         BattleFightManager.Instance.RoundFightData.BuffData_Use.CardDestination = card.CardDestination;
        //     }
        //     
        //
        //     if (BattleFightManager.Instance.RoundFightData.BuffData_Use.CardDestination == ECardDestination.Consume)
        //     {
        //         var consumeCardAddCurHP = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.ConsumeCardAddCurHP, PlayerManager.Instance.PlayerData.UnitCamp);
        //         var consumeCardTriggerDatas = new List<TriggerData>();
        //         var addHP = BlessManager.Instance.ConsumeCardAddCurHP(GamePlayManager.Instance.GamePlayData);
        //         if (addHP > 0)
        //         {
        //             var triggerData = BattleFightManager.Instance.Unit_HeroAttribute(Constant.Battle.UnUnitTriggerIdx,
        //                 Constant.Battle.UnUnitTriggerIdx, BattleFightManager.Instance.PlayerData.BattleHero.Idx, EHeroAttribute.HP, addHP);
        //             triggerData.TriggerBlessIdx = consumeCardAddCurHP.BlessIdx;
        //             triggerData.TriggerDataSubType = ETriggerDataSubType.Bless;
        //             BattleBuffManager.Instance.CacheTriggerData(triggerData, consumeCardTriggerDatas);
        //             BattleFightManager.Instance.RoundFightData.BuffData_Use.ConsumeCardDatas.Add(cardIdx, consumeCardTriggerDatas);
        //             
        //         }
        //     }
        //
        //     
        //     
        //     BlessManager.Instance.CacheUseCardData(EBlessID.EachRoundUseCardAttackAllEnemy, triggerDatas);
        //
        //     if (triggerDatas.Count > 0)
        //     {
        //         BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDatas.Add(cardIdx, triggerDatas);
        //         BattleFightManager.Instance.CalculateHeroHPDelta(BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDatas);
        //     }
        //     
        //     var battlePlayerData =
        //         BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(
        //             BattleFightManager.Instance.RoundFightData.GamePlayData.PlayerData
        //                 .UnitCamp);
        //
        //     switch (BattleFightManager.Instance.RoundFightData.BuffData_Use.CardDestination)
        //     {
        //         case ECardDestination.Pass:
        //             ToPassCard(battlePlayerData, cardIdx);
        //             break;
        //         case ECardDestination.Consume:
        //             ToConsumeCards(battlePlayerData, cardIdx);
        //             break;
        //         case ECardDestination.StandBy:
        //             ToStandByCards(battlePlayerData, cardIdx);
        //             break;
        //         default:
        //             break;
        //     }
        //     
        //     BattleFightManager.Instance.RoundFightData.BuffData_Use.UseCardCirculation
        //         .HandCards = new List<int>(battlePlayerData.HandCards);
        //     
        //     BattleFightManager.Instance.RoundFightData.BuffData_Use.UseCardCirculation
        //         .ConsumeCards = new List<int>(battlePlayerData.ConsumeCards);
        //     
        //     BattleFightManager.Instance.RoundFightData.BuffData_Use.UseCardCirculation
        //         .PassCards = new List<int>(battlePlayerData.PassCards);
        //     
        //     BattleFightManager.Instance.RoundFightData.BuffData_Use.UseCardCirculation
        //         .StandByCards = new List<int>(battlePlayerData.StandByCards);
        //
        //    
        // }


        public void CachePassCard()
        {
            var battlePlayerData =
                BattleFightManager.Instance.RoundFightData.GamePlayData.BattleData.GetBattlePlayerData(
                    BattleFightManager.Instance.RoundFightData.GamePlayData.PlayerData
                        .UnitCamp);
            
            BattleCardManager.Instance.ToPassCard(battlePlayerData,
                BattleCardManager.Instance.SelectPassCardIdx);

            BattleFightManager.Instance.RoundFightData.PassCardData.AcquireCardCirculation.HandCards = new List<int>(battlePlayerData.HandCards);
            BattleFightManager.Instance.RoundFightData.PassCardData.AcquireCardCirculation.StandByCards = new List<int>(battlePlayerData.StandByCards);
            BattleFightManager.Instance.RoundFightData.PassCardData.AcquireCardCirculation.PassCards = new List<int>(battlePlayerData.PassCards);
            BattleFightManager.Instance.RoundFightData.PassCardData.AcquireCardCirculation.ConsumeCards = new List<int>(battlePlayerData.ConsumeCards);
            
            
            var triggerData = new TriggerData();
            triggerData.TriggerDataType = ETriggerDataType.Empty;
            BattleFightManager.Instance.RoundFightData.PassCardData.PassCardDatas.Add(triggerData);
            
            var passCardAcquireCard = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.PassCardAcquireCard,
                PlayerManager.Instance.PlayerData.UnitCamp);

            if (passCardAcquireCard != null && battlePlayerData.RoundPassCardCount == 0) 
            {
                var drPassCardAcquireCard = GameEntry.DataTable.GetBless(EBlessID.PassCardAcquireCard);

                BattleCardManager.Instance.CacheAcquireCards(triggerData, BattleFightManager.Instance.RoundFightData.PassCardData.PassCardDatas,
                    int.Parse(drPassCardAcquireCard.GetValues(0)[0]));

            }

            battlePlayerData.RoundPassCardCount += 1;

        }
    }
}
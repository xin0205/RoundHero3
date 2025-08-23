
using System.Collections.Generic;


namespace RoundHero
{
    public partial class BattleCardManager : Singleton<BattleCardManager>
    {
        public void CacheUseCardData(int cardIdx, Data_BattleUnit effectUnit, int actionUnitGridPosidx, int actionUnitIdx)
        {
            var drCard = CardManager.Instance.GetCardTable(cardIdx);
            var card = BattleManager.Instance.GetCard(cardIdx);
            var triggerDatas = new List<TriggerData>();
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
                BattleBuffManager.Instance.BuffTrigger(buffData.BuffTriggerType, buffData, values, actionUnitIdx, actionUnitIdx,
                    effectUnit != null ? effectUnit.Idx : -1, triggerDatas, actionUnitGridPosidx, -1, cardIdx, ETriggerDataSubType.Card);
                
                
                

            }

            if (drCard.CardType == ECardType.Tactic)
            {
                BattleFightManager.Instance.RoundFightData.BuffData_Use.ActionDataType = EActionDataType.Tactic;
            }
            else if (drCard.CardType == ECardType.Unit)
            {
                BattleFightManager.Instance.RoundFightData.BuffData_Use.ActionDataType = EActionDataType.Unit;
            }

            
            BattleFightManager.Instance.RoundFightData.BuffData_Use.CardEnergy = BattleFightManager.Instance.CacheConsumeCardEnergy(cardIdx, triggerDatas);
            

            var unComsumeCard = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.UnConsumeCard, PlayerManager.Instance.PlayerData.UnitCamp);
            
            if (unComsumeCard != null)
            {
                BattleFightManager.Instance.RoundFightData.BuffData_Use.CardDestination = Random.Next(0, 2) == 0 ? card.CardDestination : ECardDestination.Consume;
            }
            else
            {
                BattleFightManager.Instance.RoundFightData.BuffData_Use.CardDestination = ECardDestination.Pass;
            }
            BattleFightManager.Instance.RoundFightData.BuffData_Use.CardDestination = ECardDestination.Consume;

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
                
            
            foreach (var funeIdx in card.FuneIdxs)
            {
                var drBuff = FuneManager.Instance.GetBuffTable(funeIdx);
                var idx = 0;
                foreach (var buffIDStr in drBuff.BuffIDs)
                {
                    var buffData = BattleBuffManager.Instance.GetBuffData(buffIDStr);
                    var values = drBuff.GetValues(idx++);

                    BattleBuffManager.Instance.BuffTrigger(EBuffTriggerType.Use, buffData, values, actionUnitIdx, actionUnitIdx,
                        effectUnit != null ? effectUnit.Idx : -1, triggerDatas, actionUnitGridPosidx, -1, cardIdx, ETriggerDataSubType.Fune);
  
                    BattleFightManager.Instance.RoundFightData.BuffData_Use.ActionDataType = EActionDataType.Fune;

                }

            }
            
            BlessManager.Instance.CacheUseCardData(EBlessID.EachRoundUseCardAttackAllEnemy, triggerDatas);
            
            BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDatas.Add(cardIdx, triggerDatas);
            
            BattleFightManager.Instance.CalculateHeroHPDelta(BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDatas);
        }

    }
}

using System.Collections.Generic;


namespace RoundHero
{
    public partial class BattleCardManager : Singleton<BattleCardManager>
    {
        public void CacheUseCardData(int cardIdx, EUnitCamp camp, Data_BattleUnit effectUnit, int actionUnitGridPosidx, int actionUnitIdx)
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
                    effectUnit != null ? effectUnit.Idx : -1, triggerDatas, actionUnitGridPosidx, -1, cardIdx);
                // if (triggerDatas.Count > 0)
                // {
                //     triggerDatas[0].TriggerCardIdx = cardIdx;
                // }
                
                BattleFightManager.Instance.RoundFightData.BuffData_Use.ActionDataType = EActionDataType.Tactic;

            }
            
            foreach (var funeIdx in card.FuneIdxs)
            {
                var drBuff = FuneManager.Instance.GetBuffTable(funeIdx);
                foreach (var buffIDStr in drBuff.BuffIDs)
                {
                    var buffData = BattleBuffManager.Instance.GetBuffData(buffIDStr);
                    var values = drBuff.BuffValues;

                    BattleBuffManager.Instance.BuffTrigger(EBuffTriggerType.Use, buffData, values, actionUnitIdx, actionUnitIdx,
                        effectUnit != null ? effectUnit.Idx : -1, triggerDatas, actionUnitGridPosidx, -1, cardIdx);
                    if (triggerDatas.Count > 0)
                    {
                        triggerDatas[0].TriggerCardIdx = cardIdx;
                    }
                    BattleFightManager.Instance.RoundFightData.BuffData_Use.ActionDataType = EActionDataType.Fune;

                }

            }
            
            BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDatas.Add(cardIdx, triggerDatas);
            
            BattleFightManager.Instance.CalculateHeroHPDelta(BattleFightManager.Instance.RoundFightData.BuffData_Use.TriggerDatas);
        }

    }
}
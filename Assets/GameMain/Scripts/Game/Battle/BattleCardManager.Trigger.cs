using System;
using System.Collections.Generic;
using System.Linq;

namespace RoundHero
{
    public partial class BattleCardManager : Singleton<BattleCardManager>
    {
        public void CacheTacticCardData(int cardID, EUnitCamp camp, Data_BattleUnit effectUnit)
        {
            var drCard = CardManager.Instance.GetCardTable(cardID);
            var card = BattleManager.Instance.GetCard(cardID);

            foreach (var buffID in drCard.BuffIDs)
            {
                var buffData = BattleBuffManager.Instance.GetBuffData(buffID);
                var values = new List<float>();
                foreach (var value in drCard.Values1)
                {
                    values.Add(GameUtility.GetBuffValue(value));
                }
                
                BattleBuffManager.Instance.CacheBuffData(buffData, camp, effectUnit, values, 1 + card.UseCardDamageRatio);
            }

        }

        

        
       
        

    }
}
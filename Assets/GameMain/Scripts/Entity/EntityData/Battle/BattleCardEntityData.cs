
using UnityEngine;

namespace RoundHero
{
    public class BattleCardEntityData : EntityData
    {
        public int CardIdx;
        public Data_Card CardData;
        
        public void Init(int entityId, Vector2 pos, int cardIdx)
        {
            base.Init(entityId, pos);
            CardIdx = cardIdx;
            CardData = BattleManager.Instance.GetCard(CardIdx);
        }

        
    }
}
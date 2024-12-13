
using UnityEngine;

namespace RoundHero
{
    public class BattleCardEntityData : EntityData
    {
        public int CardIdx;
        public Data_Card CardData;
        public int HandSortingIdx;
        
        public void Init(int entityId, Vector3 pos, int cardIdx, int handSortingIdx)
        {
            base.Init(entityId, pos);
            CardIdx = cardIdx;
            CardData = BattleManager.Instance.GetCard(CardIdx);
            HandSortingIdx = handSortingIdx;
        }

        
    }
}
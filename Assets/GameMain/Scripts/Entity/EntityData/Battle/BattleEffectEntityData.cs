using UnityEngine;

namespace RoundHero
{
    public class BattleEffectEntityData : EntityData
    {
        public EColor EffectColor;
        
        public void Init(int entityId, Vector3 pos, EColor color)
        {
            base.Init(entityId, pos);
            EffectColor = color;
            
        }
    }
}
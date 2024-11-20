using UnityEngine;

namespace RoundHero
{
    public class BattleHurtEntityData : EntityData
    {
        public int Hurt;
        
        public void Init(int entityId, Vector3 pos, int hurt)
        {
            base.Init(entityId, pos);
            this.Hurt = hurt;
    
        }
    }
}
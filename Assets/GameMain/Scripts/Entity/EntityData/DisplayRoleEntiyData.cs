using UnityEngine;

namespace RoundHero
{
    public class DisplayRoleEntiyData : EntityData
    {
        public int HeroID;
        public Data_Card CardData;
        
        public new void Init(int entityId, Vector3 pos, int heroID)
        {
            base.Init(entityId, pos);
            HeroID = heroID;

        }
    }
}
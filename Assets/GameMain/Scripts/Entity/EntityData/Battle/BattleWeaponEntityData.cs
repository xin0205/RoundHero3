using RPGCharacterAnims.Lookups;
using UnityEngine;

namespace RoundHero
{
    
    
    public class BattleWeaponEntityData : EntityData
    {
        public EWeaponHoldingType WeaponHoldingType;
        public EWeaponType WeaponType;
        public int WeaponID;
        
        public void Init(int entityId, EWeaponHoldingType weaponHoldingType, EWeaponType weaponType, int weaponID)
        {
            base.Init(entityId, Vector3.zero);
            WeaponHoldingType = weaponHoldingType;
            WeaponType = weaponType;
            WeaponID = weaponID;

        }
    }
}
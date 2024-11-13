
using RPGCharacterAnims.Lookups;

using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleSoliderEntity : BattleUnitEntity
    {
        public BattleSoliderEntityData BattleSoliderEntityData { get; protected set; }

        
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            
            BattleSoliderEntityData = userData as BattleSoliderEntityData;
            if (BattleSoliderEntityData == null)
            {
                Log.Error("Error BattleSoliderEntityData");
                return;
            }
            
            BattleUnitData = BattleSoliderEntityData.BattleSoliderData;

            ShowInit();
            //animator.SetInteger(AnimationParameters.WeaponSwitch, (int)AnimatorWeapon.ARMED);
            animator.SetBool(AnimationParameters.Moving, false);
            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.TwoHandAxe);
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.WeaponUnsheathTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);

            AttachWeapon();
        }

        private async void AttachWeapon()
        {
            var drCard = CardManager.Instance.GetCardTable(BattleSoliderEntityData.BattleSoliderData.CardIdx);
            //drCard.WeaponHoldingType
            
            var weaponEntity = await GameEntry.Entity.ShowWeaponEntityAsync(drCard.WeaponHoldingType, drCard.WeaponType, drCard.WeaponID);
            
            if (drCard.WeaponHoldingType == EWeaponHoldingType.TwoHand)
            {
                var weaponEntity2 = await GameEntry.Entity.ShowWeaponEntityAsync(drCard.WeaponHoldingType, drCard.WeaponType, drCard.WeaponID);
                
    
                GameEntry.Entity.AttachEntity(weaponEntity.Entity, this.Entity, leftWeapon);
                GameEntry.Entity.AttachEntity(weaponEntity2.Entity, this.Entity, rightWeapon);
            }
            else if (drCard.WeaponHoldingType == EWeaponHoldingType.Left)
            {
                GameEntry.Entity.AttachEntity(weaponEntity.Entity, this.Entity, leftWeapon);
            }
            else if (drCard.WeaponHoldingType == EWeaponHoldingType.Right)
            {
                GameEntry.Entity.AttachEntity(weaponEntity.Entity, this.Entity, rightWeapon);
            }
            
        }

        public override void Quit()
        {
            base.Quit();
            BattleSoliderManager.Instance.RemoveSolider(BattleSoliderEntityData.BattleSoliderData.ID);
            
            
        }
        
        public override void Dead()
        {
            base.Dead();
            BattleSoliderManager.Instance.RemoveSolider(BattleSoliderEntityData.BattleSoliderData.ID);
 
        }

        

        public override void ChangeCurHP(int changeHP, bool useDefense = true, bool addHeroHP = true, bool changeHPInstantly = true)
        {

            BattleManager.Instance.ChangeHP(BattleUnitData, changeHP,  GamePlayManager.Instance.GamePlayData, EHPChangeType.Action, useDefense, addHeroHP, changeHPInstantly);
        }
    }
}
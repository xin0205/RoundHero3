
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleSoliderEntity : BattleUnitEntity
    {
        public BattleSoliderEntityData BattleSoliderEntityData { get; protected set; }

        [SerializeField] private BoxCollider boxCollider;

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

            var unitDescFormData = GetComponent<UnitDescTriggerItem>().UnitDescFormData;
            unitDescFormData.UnitCamp = BattleSoliderEntityData.BattleSoliderData.UnitCamp;
            unitDescFormData.UnitRole = EUnitRole.Staff;

            unitDescFormData.Idx = BattleSoliderEntityData.BattleSoliderData.Idx;

            
            ShowInit();
            //animator.SetInteger(AnimationParameters.WeaponSwitch, (int)AnimatorWeapon.ARMED);
            var drCard = CardManager.Instance.GetCardTable(BattleSoliderEntityData.BattleSoliderData.CardIdx);
            InitWeaponType(drCard.WeaponHoldingType, drCard.WeaponType, drCard.WeaponID);
            AttachWeapon(drCard.WeaponHoldingType, drCard.WeaponType, drCard.WeaponID);
            
            UnitAttackCastType = drCard.AttackCastType;
            
            ShowCollider(true);
        }

        public void ShowCollider(bool isShow)
        {
            boxCollider.enabled = isShow;

        }

       public override void OnPointerEnter(BaseEventData baseEventData)
        {
            base.OnPointerEnter(baseEventData);
            
            if(CurHP <= 0)
                return;
        
            if(IsMove)
                return;

            if (BattleManager.Instance.BattleState == EBattleState.TacticSelectUnit)
            {
                var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
                var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);

                if (buffData.BuffStr == EBuffID.Spec_AttackUs.ToString())
                {
                    BattleAttackTagManager.Instance.ShowAttackTag(UnitIdx, false);
                    BattleFlyDirectManager.Instance.ShowFlyDirect(UnitIdx);
                    BattleIconManager.Instance.ShowBattleIcon(UnitIdx, EBattleIconType.Collison);

                    ShowDisplayValue(UnitIdx);
                }
            }
            
            
        }

        

        public override void OnPointerExit(BaseEventData baseEventData)
        {
            base.OnPointerExit(baseEventData);
            
            // if(BattleManager.Instance.BattleState != EBattleState.TacticSelectUnit)
            //     return;
            //
            // if(CurHP <= 0)
            //     return;
            //
            // if(IsMove)
            //     return;
            
            // var buffStr = BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr;
            // var buffData = BattleBuffManager.Instance.GetBuffData(buffStr);
            //
            // if (buffData.BuffStr == EBuffID.Spec_AttackUs.ToString())
            // {
            //
            //     BattleValueManager.Instance.UnShowDisplayValues();
            //     BattleAttackTagManager.Instance.UnShowAttackTags();
            //     BattleFlyDirectManager.Instance.UnShowFlyDirects();
            //     BattleIconManager.Instance.UnShowBattleIcons();
            //     UnShowTags();
            // }

            UnShowDisplayValues();
            BattleAttackTagManager.Instance.UnShowAttackTags();
            BattleFlyDirectManager.Instance.UnShowFlyDirects();
            BattleIconManager.Instance.UnShowBattleIcons();
            UnShowTags();
        }

        public override void Quit()
        {
            base.Quit();
            BattleSoliderManager.Instance.RemoveSolider(BattleSoliderEntityData.BattleSoliderData.Idx);
            
            
        }
        
        public override void Dead()
        {
            base.Dead();
            BattleSoliderManager.Instance.RemoveSolider(BattleSoliderEntityData.BattleSoliderData.Idx);
            GameUtility.DelayExcute(3f, () =>
            {
                GameEntry.Entity.HideEntity(this);
            });
        }

        protected async override Task ShowBattleHurts(int hurt)
        {
            var pos = GameUtility.GridPosIdxToPos(BattleUnitData.GridPosIdx);
            //var heroEntity = HeroManager.Instance.GetHeroEntity(BattleUnitData.UnitCamp);
            
            await GameEntry.Entity.ShowBattleMoveValueEntityAsync(ValuePos.position,  pos, hurt);
        }

        // public override async void ChangeCurHP(int changeHP, bool useDefense = true, bool addHeroHP = true, bool changeHPInstantly = true)
        // {
        //
        //     BattleManager.Instance.ChangeHP(BattleUnitData, changeHP,  GamePlayManager.Instance.GamePlayData, EHPChangeType.Action, useDefense, addHeroHP, changeHPInstantly);
        //     await GameEntry.Entity.ShowBattleHurtEntityAsync(BattleSoliderEntityData.BattleSoliderData.GridPosIdx, changeHP);
        //     
        // }
        
        
    }
}
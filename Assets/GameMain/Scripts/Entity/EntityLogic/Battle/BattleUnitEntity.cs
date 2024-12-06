
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using GameFramework;
using RPGCharacterAnims;
using RPGCharacterAnims.Actions;
using RPGCharacterAnims.Lookups;
using UnityEngine;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleUnitEntity : Entity, IMoveGrid
    {
        [SerializeField] protected Transform roleRoot;
        [SerializeField] protected TextMesh hp;
        [SerializeField] protected TextMesh damage;
        [SerializeField] protected GameObject uiNode;
        //[SerializeField] protected RPGCharacterController Controller;
        
        protected Quaternion cameraQuaternion = Quaternion.identity;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Transform leftWeapon;
        [SerializeField] protected Transform rightWeapon;

        protected Data_BattleUnit BattleUnitData { get; set; }
        protected bool IsMove = false;
        
        public Transform EffectHurtPos;
        public Transform EffectAttackPos;
        public Transform ShootPos;
        public EAttackCastType UnitAttackCastType;

        protected Queue<int> hurtQueue = new Queue<int>();
        
        
        public Vector3 Position
        {
            get => transform.position; 
            set => transform.position = value;
        }

        public int GridPosIdx
        {
            get => BattleUnitData.GridPosIdx; 
            set => BattleUnitData.GridPosIdx = value;
        }
        
        public int ID
        {
            get => BattleUnitData.ID; 
            set => BattleUnitData.ID = value;
        }

        public Data_BattleUnit BattleUnit
        {
            get => BattleUnitData; 
            set => BattleUnitData = value;
        }

        public EUnitCamp UnitCamp
        {
            get => BattleUnitData.UnitCamp;
            set => BattleUnitData.UnitCamp = value;
        }
        
        public EUnitRole UnitRole
        {
            get => BattleUnitData.UnitRole;
            set => BattleUnitData.UnitRole = value;
        }
        
        public int CurHP
        {
            get => BattleUnitData.CurHP; 
            set => BattleUnitData.CurHP = value;
        }
        
        public int MaxHP
        {
            get => BattleUnitData.MaxHP;
        }
        
        //public EUnitActionState UnitActionState { get; set; }

        //protected int TopLayerIdx;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            //TopLayerIdx = animator.GetLayerIndex("TopLayer");
            
            // var idleName = EUnitActionState.RunAttack.ToString();
            //
            // var topLayerAnimatorClipInfo = Animator.GetCurrentAnimatorClipInfo(TopLayerIdx);
            // foreach (var clipInfo in topLayerAnimatorClipInfo)
            // {
            //     Log.Debug("CC:" + clipInfo.clip.length);
            //     if (clipInfo.clip.name == idleName)
            //     {
            //         clipInfo.clip.AddEvent(new AnimationEvent()
            //         {
            //             time = clipInfo.clip.length,
            //             objectReferenceParameter = this,
            //             functionName = "AfterRunAction"
            //         });
            //     }
            //     
            // }


        }

        // public void AfterRunAction()
        // {
        //     Log.Debug("AfterRunAction");
        //     //Animator.SetTrigger(EUnitActionState.Run.ToString());
        //     if (IsMove)
        //     {
        //         SetAction(EUnitActionState.Run);
        //     }
        //     else
        //     {
        //         SetAction(EUnitActionState.Idle);
        //     }
        // }
        

        // public void AnimatorSetLayerWeight()
        // {
        //     if (UnitActionState == EUnitActionState.RunAttack || UnitActionState == EUnitActionState.RunHurt)
        //     {
        //         //Animator.speed = 0.5f;
        //         //Animator.SetLayerWeight(TopLayerIdx, 1);
        //         //Animator.SetLayerWeight(BaseLayerIdx, 0);
        //     }
        //     else
        //     {
        //         //Animator.speed = 1f;
        //         // Animator.SetLayerWeight(BaseLayerIdx, 1);
        //         //Animator.SetLayerWeight(TopLayerIdx, 0);
        //     }
        //
        // }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            IsMove = false;
            
            
        }
        
         protected void InitWeaponType(EWeaponHoldingType weaponHoldingType, EWeaponType weaponType, int weaponID)
        {
            animator.SetBool(AnimationParameters.Moving, false);
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.WeaponUnsheathTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            
            switch (weaponHoldingType)
            {
                case EWeaponHoldingType.TwoHand:
                    switch (weaponType)
                    {
                        case EWeaponType.Sword:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.TwoHandSword);
                            break;
                        case EWeaponType.Spear:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.TwoHandSpear);
                            break;
                        case EWeaponType.Axe:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.TwoHandAxe);
                            break;
                        case EWeaponType.Bow:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.TwoHandBow);
                            break;
                        case EWeaponType.Crossbow:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.TwoHandCrossbow);
                            break;
                        case EWeaponType.Staff:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.TwoHandStaff);
                            break;
                        // case EWeaponType.Mace:
                        //     break;
                        // case EWeaponType.Dagger:
                        //     break;
                        // case EWeaponType.Item:
                        //     break;
                        // case EWeaponType.Pistol:
                        //     break;
                        // case EWeaponType.Shield:
                        //     break;
                        // case EWeaponType.Rifle:
                        //     break;
                        // case EWeaponType.Empty:
                        //     break;
                        default:
                            break;
                    }
                    
                    
                    break;
                case EWeaponHoldingType.Left:
                    switch (weaponType)
                    {
                        case EWeaponType.Sword:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.LeftSword);
                            break;
                        case EWeaponType.Mace:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.LeftMace);
                            break;
                        case EWeaponType.Dagger:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.LeftDagger);
                            break;
                        case EWeaponType.Item:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.LeftItem);
                            break;
                        case EWeaponType.Pistol:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.LeftPistol);
                            break;
                        // case EWeaponType.Spear:
                        //     break;
                        // case EWeaponType.Axe:
                        //     break;
                        // case EWeaponType.Bow:
                        //     break;
                        // case EWeaponType.Crossbow:
                        //     break;
                        // case EWeaponType.Staff:
                        //     break;
                        // case EWeaponType.Shield:
                        //     break;
                        //
                        // case EWeaponType.Rifle:
                        //     break;
                        // case EWeaponType.Empty:
                        //     break;
                        default:
                            break;
                    }
                    break;
                case EWeaponHoldingType.Right:
                    switch (weaponType)
                    {
                        case EWeaponType.Sword:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.RightSword);
                            break;
                        case EWeaponType.Spear:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.RightSpear);
                            break;
                        case EWeaponType.Mace:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.RightMace);
                            break;
                        case EWeaponType.Dagger:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.RightDagger);
                            break;
                        case EWeaponType.Item:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.RightItem);
                            break;
                        case EWeaponType.Pistol:
                            animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.RightPistol);
                            break;
                        // case EWeaponType.Axe:
                        //     break;
                        // case EWeaponType.Bow:
                        //     break;
                        // case EWeaponType.Crossbow:
                        //     break;
                        // case EWeaponType.Staff:
                        //     break;
                        // case EWeaponType.Shield:
                        //     break;
                        // case EWeaponType.Rifle:
                        //     break;
                        // case EWeaponType.Empty:
                        //     break;
                        default:
                            break;
                    }
                    
                    break;
                case EWeaponHoldingType.Empty:
                    break;
                default:
                    break;
            }
            
        }

        protected async void AttachWeapon(EWeaponHoldingType weaponHoldingType, EWeaponType weaponType, int weaponID)
        {

            var weaponEntity = await GameEntry.Entity.ShowWeaponEntityAsync(weaponHoldingType, weaponType, weaponID);

            switch (weaponHoldingType)
            {
                case EWeaponHoldingType.TwoHand:
                    GameEntry.Entity.AttachEntity(weaponEntity.Entity, this.Entity, rightWeapon);
                    break;
                case EWeaponHoldingType.Left:
                    GameEntry.Entity.AttachEntity(weaponEntity.Entity, this.Entity, leftWeapon);
                    break;
                case EWeaponHoldingType.Right:
                    GameEntry.Entity.AttachEntity(weaponEntity.Entity, this.Entity, rightWeapon);
                    break;
                case EWeaponHoldingType.Empty:
                    break;
                default:
                    break;
            }
            
            weaponEntity.transform.localPosition = Vector3.zero;
            weaponEntity.transform.localRotation = Quaternion.Euler(Vector3.zero);
            weaponEntity.gameObject.SetLayerRecursively(this.gameObject.layer);

        }

        protected void ShowInit()
        {
            gameObject.SetLayerRecursively(
                LayerMask.NameToLayer(BattleUnitData.UnitCamp == EUnitCamp.Player1
                    ? "Role"
                    : "Role2"));
            
            Idle();
            RefreshData();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            RefreshRoatation();
            ShowHurts();

        }
        
        // public void SetAction(EUnitActionState actionState)
        // {
        //     // if (actionState == EUnitActionState.Hurt)
        //     // {
        //     //     if (UnitActionState == EUnitActionState.Run)
        //     //     {
        //     //         actionState = EUnitActionState.RunHurt;
        //     //     }
        //     //     else
        //     //     {
        //     //         actionState = EUnitActionState.Hurt;
        //     //     }
        //     // }
        //     //
        //     // if (actionState == EUnitActionState.Attack)
        //     // {
        //     //     if (UnitActionState == EUnitActionState.Run)
        //     //     {
        //     //         actionState = EUnitActionState.RunAttack;
        //     //     }
        //     //     else
        //     //     {
        //     //         actionState = EUnitActionState.Attack;
        //     //     }
        //     // }
        //     
        //     //UnitActionState = actionState;
        //     // if (UnitActionState == EUnitActionState.Attack)
        //     // {
        //     //     Controller.StartAction(HandlerTypes.Attack, new AttackContext(HandlerTypes.Attack, Side.Left));
        //     // }
        //     
        //     //AnimatorSetLayerWeight();
        //
        //     // if (BattleUnitData is Data_BattleMonster)
        //     // {
        //     //     //Log.Debug("act:" + actionState.ToString());
        //     // }
        //
        //     //Animator.SetTrigger(actionState.ToString());
        //
        // }

        public void WeaponSwitch()
        {
            
        }
        
        public void Shoot()
        {
            var buffData = BattleUnitManager.Instance.GetBuffDatas(BattleUnit);
            var triggerRange = buffData[0].TriggerRange;
            var coord = GameUtility.GridPosIdxToCoord(BattleUnit.GridPosIdx);
            for (int i = 1; i < Constant.Battle.ActionTypePoints[buffData[0].TriggerRange].Count; i++)
            {
                var range = Constant.Battle.ActionTypePoints[buffData[0].TriggerRange][i];
                var bulletData = new BulletData();
                bulletData.ActionUnitID = BattleUnitData.ID;
                bulletData.MoveGridPosIdxs.Add(BattleUnit.GridPosIdx);
                foreach (var deltaPos in range)
                {
                    var deltaCoord = coord + deltaPos;
                    if(!GameUtility.InGridRange(deltaCoord))
                        continue;
                        
                    var gridPosIdx = GameUtility.GridCoordToPosIdx(deltaCoord);
                    bulletData.MoveGridPosIdxs.Add(gridPosIdx);
                    
                    var effectUnit = BattleUnitManager.Instance.GetUnitByGridPosIdx(gridPosIdx);
                    if(effectUnit == null)
                        continue;
                    
                    var triggerActionDatas =
                        BattleBulletManager.Instance.GetTriggerActionDatas(BattleUnitData.ID, effectUnit.BattleUnit.ID);
                    
                    if(triggerActionDatas == null)
                        continue;
                    
                    foreach (var triggerActionData in triggerActionDatas)
                    {
                        bulletData.TriggerDataDict.Add(gridPosIdx, triggerActionData.TriggerData);
                    }

                    if(triggerActionDatas != null && triggerRange.ToString().Contains("Extend"))
                    {
                        break;
                    }

                    
                }

                if (bulletData.MoveGridPosIdxs.Count <= 1)
                    continue;
                
                GameEntry.Entity.ShowBattleBulletEntityAsync(bulletData, ShootPos.position);
                
            }
            
        }
        
        public async void Hit()
        {
            await ShowEffectAttackEntity();

            BattleBulletManager.Instance.ActionUnitTrigger(this.BattleUnitData.ID);
        }

        public void GetHit()
        {
            Log.Debug("GetHit");
            ShowEffectHurtEntity();
        }

        public void HitTrigger()
        {
            
        }

        public void Land()
        {
            
        }

        //private EffectEntity effectAttackEntity;
        private async Task ShowEffectAttackEntity()
        {
            var triggerActionDataDict = BattleBulletManager.Instance.GetTriggerActionDatas(this.BattleUnitData.ID);
            switch (UnitAttackCastType)
            {
                case EAttackCastType.CloseSingle:
                    ShowEffectAttackEntity_CloseSingle(triggerActionDataDict);
                    break;
                case EAttackCastType.CloseMulti:
                    ShowEffectAttackEntity_CloseMulti(triggerActionDataDict);
                    break;
                case EAttackCastType.RemoteSingle:
                    break;
                case EAttackCastType.RemoteMulti:
                    ShowEffectAttackEntity_RemoteMulti(triggerActionDataDict);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // var effectIDs = triggerActionDataDict.Keys.ToList();
            // var triggerActionDatas = triggerActionDataDict.Values.ToList();
            // for (int i = 0; i < effectIDs.Count; i++)
            // {
            //     var triggerActionData = triggerActionDatas[i];
            //     var effectUnit = BattleUnitManager.Instance.GetUnitByID(triggerActionData.TriggerData.EffectUnitID);
            //
            //     var effectName = "EffectAttackEntity";
            //     var effectPos = EffectAttackPos.position;
            //     if (triggerActionData.TriggerData.BuffTriggerType == EBuffTriggerType.Pass ||
            //         triggerActionData.TriggerData.BuffTriggerType == EBuffTriggerType.BePass)
            //     {
            //         effectName = "EffectCloseSingleAttackEntity";
            //         effectPos = EffectAttackPos.position;
            //     }
            //     else
            //     {
            //         switch (UnitAttackCastType)
            //         {
            //             case EAttackCastType.CloseSingle:
            //                 effectName = "EffectCloseSingleAttackEntity";
            //                 effectPos = EffectAttackPos.position;
            //                 break;
            //             case EAttackCastType.CloseMulti:
            //                 effectName = "EffectCloseMultiAttackEntity";
            //                 effectPos = EffectHurtPos.position;
            //                 break;
            //             case EAttackCastType.RemoteSingle:
            //                 break;
            //             case EAttackCastType.RemoteMulti:
            //                 break;
            //             default:
            //                 break;
            //         }
            //     }
            //     
            //
            //     var effectAttackEntity = await GameEntry.Entity.ShowEffectEntityAsync(effectName, effectPos);
            //     
            //     var pos = effectUnit.Position;
            //     effectAttackEntity.transform.LookAt(new Vector3(pos.x, effectAttackEntity.transform.position.y, pos.z));
            //     if (!effectAttackEntity.AutoHide)
            //     {
            //         GameUtility.DelayExcute(1f, () =>
            //         {
            //             GameEntry.Entity.HideEntity(effectAttackEntity);
            //         });
            //     }
            //     
            //     
            // }
            

        }
        
        private async void ShowEffectAttackEntity_CloseSingle(GameFrameworkMultiDictionary<int, TriggerActionData> triggerActionDataDict)
        {
            var effectName = "EffectCloseSingleAttackEntity";
            var effectPos = EffectHurtPos.position;

            foreach (var kv in triggerActionDataDict)
            {
                foreach (var triggerActionData in kv.Value)
                {
                    
                    var effectUnit = BattleUnitManager.Instance.GetUnitByID(triggerActionData.TriggerData.EffectUnitID);
                    
                    if (triggerActionData.TriggerData.BuffTriggerType == EBuffTriggerType.Pass ||
                        triggerActionData.TriggerData.BuffTriggerType == EBuffTriggerType.BePass)
                    {
                        effectName = "EffectCloseSingleAttackEntity";
                        effectPos = EffectAttackPos.position;
                    }

                    
                    ShowEffectAttackEntity(effectName, effectPos, effectUnit.Position);
                }
            }
        }

        private async void ShowEffectAttackEntity_CloseMulti(GameFrameworkMultiDictionary<int, TriggerActionData> triggerActionDataDict)
        {
            var effectName = "EffectCloseMultiAttackEntity";
            var effectPos = EffectHurtPos.position;

            foreach (var kv in triggerActionDataDict)
            {
                foreach (var triggerActionData in kv.Value)
                {
                    
                    var effectUnit = BattleUnitManager.Instance.GetUnitByID(triggerActionData.TriggerData.EffectUnitID);
                    
                    if (triggerActionData.TriggerData.BuffTriggerType == EBuffTriggerType.Pass ||
                        triggerActionData.TriggerData.BuffTriggerType == EBuffTriggerType.BePass)
                    {
                        effectName = "EffectCloseSingleAttackEntity";
                        effectPos = EffectAttackPos.position;
                    }

                    
                    
                    
                    
                    
                    
                    ShowEffectAttackEntity(effectName, effectPos, effectUnit.Position);
                }
            }
        }

        private async void ShowEffectAttackEntity_RemoteMulti(GameFrameworkMultiDictionary<int, TriggerActionData> triggerActionDataDict)
        {
            
        }

        
        private async void ShowEffectAttackEntity(string effectName, Vector3 effectPos, Vector3 lookAtPos)
        {
            var effectAttackEntity = await GameEntry.Entity.ShowEffectEntityAsync(effectName, effectPos);

            if (lookAtPos != Vector3.zero)
            {
                effectAttackEntity.transform.LookAt(new Vector3(lookAtPos.x, effectAttackEntity.transform.position.y, lookAtPos.z));
            }
            
            if (!effectAttackEntity.AutoHide)
            {
                GameUtility.DelayExcute(1f, () =>
                {
                    GameEntry.Entity.HideEntity(effectAttackEntity);
                });
            }
        }
        
        public void FootL()
        {
            
        }
        
        public void FootR()
        {
            
        }
        
        public void Idle()
        {
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.InstantSwitchTrigger);
            animator.SetBool(AnimationParameters.Moving, false);
            animator.SetTrigger(AnimationParameters.Trigger);
            
            animator.SetFloat(AnimationParameters.VelocityZ, 0);
            //SetAction(EUnitActionState.Idle);
        }
        
        public void Dodge()
        {
            animator.SetInteger(AnimationParameters.Action, (int)DodgeType.Left);
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.DodgeTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            //SetAction(EUnitActionState.Dodge);
        }

        public float GetMoveTime(EUnitActionState unitActionState, MoveActionData moveActionData)
        {
            var moveGridPosIdxs = moveActionData.MoveGridPosIdxs;
            var moveCount = moveGridPosIdxs.Count > 1 ? moveGridPosIdxs.Count - 1 : 1;
            return moveCount * Constant.Unit.MoveTimes[unitActionState] + 0.1f;
        }

        public void Move(EUnitActionState unitActionState, MoveActionData moveActionData)
        {
            IsMove = true;
            //SetAction(unitActionState);

            var moveGridPosIdxs = moveActionData.MoveGridPosIdxs;
            for (int i = 0; i < moveGridPosIdxs.Count; i++)
            {
                var moveGridPosIdx = moveGridPosIdxs[i];

                var pos = GameUtility.GetMovePos(unitActionState, moveGridPosIdxs, i);
                
                var tIdx = i;

                var time = Constant.Unit.MoveTimes[unitActionState] * (tIdx - 1);
                time = time < 0 ? 0 : time;

                GameUtility.DelayExcute(time, () =>
                {
                    var moveTIdx = tIdx;
                    var nextMoveGridPosIdx = moveGridPosIdx;
                    var movePos = pos;

                    roleRoot.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                    if (unitActionState == EUnitActionState.Fly)
                    {
                        roleRoot.Rotate(new Vector3(0, 1, 0), 180);
                    }
                    
                    
                    if (BattleUnitData.CurHP > 0)
                    {
                        if (unitActionState == EUnitActionState.Fly)
                        {
                            Fly();
                        }
                        else
                        {
                            Run();
                        }
                        //Controller.StartAction(HandlerTypes.Navigation, movePos);
                        
                        transform.DOMove(movePos, moveTIdx == 0 ? 0 : Constant.Unit.MoveTimes[unitActionState]).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            GridPosIdx = nextMoveGridPosIdx;
                            
                            BattleFightManager.Instance.MoveEffectAction(unitActionState, moveActionData, moveTIdx, BattleUnitData.ID);
                        
                        });
                    }

                });
                
            }

            var moveCount = moveGridPosIdxs.Count > 1 ? moveGridPosIdxs.Count - 1 : 1;

            GameUtility.DelayExcute(moveCount * Constant.Unit.MoveTimes[unitActionState]  + 0.1f, () =>
            {
                BattleUnitData.RoundMoveCount += moveCount;
                if (BattleUnitData.CurHP > 0)
                {
                    animator.SetInteger(AnimationParameters.Jumping, 0);
                    Idle();
                    var pos = HeroManager.Instance.HeroEntity.Position;
                    roleRoot.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                    GridPosIdx = moveGridPosIdxs[moveGridPosIdxs.Count - 1];
                }
                IsMove = false;
                //BattleManager.Instance.Refresh();
                
                BattleManager.Instance.RefreshView();
                BattleAreaManager.Instance.RefreshObstacles();
            });

            //return moveCount * Constant.Unit.MoveTimes[unitActionState] + 0.1f;
        }
        
        public void Run(MoveActionData moveActionData)
        {
            //return
            Move(EUnitActionState.Run, moveActionData);

        }
        
        public void Fly(MoveActionData moveActionData)
        {
            //return
            Move(EUnitActionState.Fly, moveActionData);

        }
        
        public void Fly()
        {
            animator.SetInteger(AnimationParameters.Jumping, 2);
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.JumpTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
        }

        private void Run()
        {
            animator.SetBool(AnimationParameters.Moving, true);
            animator.SetFloat(AnimationParameters.VelocityZ, 1);
        }
        
        public virtual void Attack()
        {
            //SetAction(EUnitActionState.Attack);

            switch (UnitAttackCastType)
            {
                case EAttackCastType.CloseSingle:
                    CloseSingleAttack();
                    break;
                case EAttackCastType.CloseMulti:
                    CloseMultiAttack();
                    break;
                case EAttackCastType.RemoteSingle:
                    break;
                case EAttackCastType.RemoteMulti:
                    RemoteMultiAttack();
                    break;
                default:
                    break;
            }
            
        }
        
        public void RemoteMultiAttack()
        {
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.AttackCastTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            animator.SetInteger(AnimationParameters.Action, (int)AttackCastType.Cast1);
            GameUtility.DelayExcute(1f, () =>
            {
                animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.CastEndTrigger);
                animator.SetTrigger(AnimationParameters.Trigger);
            });
        }

        public void CloseMultiAttack()
        {
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.SpecialAttackTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            animator.SetInteger(AnimationParameters.Action, (int)AttackCastType.Cast1);
            GameUtility.DelayExcute(1f, () =>
            {
                animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.SpecialEndTrigger);
                animator.SetTrigger(AnimationParameters.Trigger);
            });
        }
        
        public void CloseSingleAttack()
        {
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.AttackTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            animator.SetInteger(AnimationParameters.Action, (int)AttackCastType.Cast1);
            // GameUtility.DelayExcute(1f, () =>
            // {
            //
            //     GameEntry.Entity.HideEntity(effectAttackEntity);
            // });
        }
        
        public void RunAttack()
        {
            CloseSingleAttack();
            // SetAction(EUnitActionState.RunAttack);
            // GameUtility.DelayExcute(0.8f, () =>
            // {
            //     AfterRunAction();
            // });
        }
        
        // public void RunHurt()
        // {
        //     
        //     // SetAction(EUnitActionState.RunHurt);
        //     // GameUtility.DelayExcute(1.2f, () =>
        //     // {
        //     //     AfterRunAction();
        //     // });
        // }
        
        public virtual void RefreshData()
        {
            RefreshHP();
            RefreshDamageState();
        }

        public void RefreshHP()
        {
            var curHP = BattleUnitData.CurHP;
            curHP = curHP < 0 ? 0 : curHP;
            // hp.text = curHP + "/" +
            //           BattleUnitData.MaxHP;
            hp.text = curHP.ToString();
        }

        public void RefreshDamageState()
        {
            var hurt = BattleFightManager.Instance.GetTotalDelta(this.ID, EHeroAttribute.CurHP);
            this.damage.text = hurt.ToString();
        }
        
        public virtual void Quit()
        {
            // SetAction(EUnitActionState.Quit);
            GameUtility.DelayExcute(1.5f, () =>
            {
                GameEntry.Entity.HideEntity(this);
            });
        }
        
        public virtual void Dead()
        {
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.DeathTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            
            
            //SetAction(EUnitActionState.Dead);
            GameUtility.DelayExcute(5f, () =>
            {
                GameEntry.Entity.HideEntity(this);
            });
        }
        
        public void RefreshRoatation()
        {
            cameraQuaternion.SetLookRotation(Camera.main.transform.forward, Camera.main.transform.up);
            uiNode.transform.rotation = cameraQuaternion;

            var dis = Mathf.Abs(AreaController.Instance.GetDistanceToPoint(hp.transform.position));
            
            uiNode.transform.localScale = Vector3.one *  dis / 12f;
            
        }
        
        private async void ShowEffectHurtEntity()
        {
            var effectHurt = await GameEntry.Entity.ShowEffectEntityAsync("EffectHurtEntity", EffectHurtPos.position);
            //effectHurt.transform.parent = EffectHurtPos;
        }

        public void Hurt()
        {
            
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.GetHitTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            animator.SetInteger(AnimationParameters.Action, (int)HitType.Back1);
            
            // if (UnitActionState == EUnitActionState.Run)
            // {
            //     RunHurt();
            // }
            // else
            // {
            //     SetAction(EUnitActionState.Hurt);
            // }
            
            

            if (CurHP == 0)
            {
                CurHP = -1;
                GameUtility.DelayExcute(1.5f, () =>
                {
                    Dead();
                });
               
            }
            
        }
        
        public void Recover()
        {
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.ActionTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            animator.SetInteger(AnimationParameters.Action, (int)EmoteType.Boost);
            //SetAction(EUnitActionState.Recover);
        }
        
        public virtual void ChangeCurHP(int changeHP, bool useDefense, bool addHeroHP, bool changeHPInstantly)
        {
            BattleManager.Instance.ChangeHP(BattleUnitData, changeHP, GamePlayManager.Instance.GamePlayData,
                EHPChangeType.Action, useDefense, addHeroHP, changeHPInstantly);
            AddHurts(changeHP);
        }

        public void UpdatePos(Vector3 pos)
        {
            Position = pos;
        }

        public void AddHurts(int hurt)
        {
            hurtQueue.Enqueue(hurt);
        }

        private float showHurtTime = 0f;
        protected async void ShowHurts()
        {
            showHurtTime += Time.deltaTime;
            if (showHurtTime > 0.4f && hurtQueue.Count > 0)
            {
                showHurtTime = 0;
                var hurt = hurtQueue.Dequeue();
                await GameEntry.Entity.ShowBattleHurtEntityAsync(BattleUnitData.GridPosIdx, hurt);
            }
        }

        protected BulletData bulletData = new BulletData();
        protected void AddBulletData()
        {
            
        }
    }
}
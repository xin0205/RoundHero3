

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using GameFramework;
using RPGCharacterAnims.Lookups;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public partial class BattleUnitEntity : Entity, IMoveGrid
    {
        [SerializeField] protected Transform roleRoot;
        [SerializeField] protected TextMesh hp;
        [SerializeField] protected TextMesh damage;
        
        [SerializeField] protected GameObject uiNode;
        [SerializeField] protected GameObject hpAndDamageNode;
        [SerializeField] protected GameObject damageNode;
        [SerializeField] protected TextMesh damage2;
        [SerializeField] private BoxCollider boxCollider;
        //[SerializeField] protected RPGCharacterController Controller;
        
        protected Quaternion cameraQuaternion = Quaternion.identity;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Transform leftWeapon;
        [SerializeField] protected Transform rightWeapon;
        
        
        public Transform Root;
        

        public virtual Data_BattleUnit BattleUnitData { get; set; }
        public bool IsMove = false;
        
        public Transform EffectHurtPos;
        public Transform EffectAttackPos;
        public Transform ShootPos;
        public EAttackCastType UnitAttackCastType;
        public Transform ValuePos;
        protected Queue<int> hurtQueue = new Queue<int>();
        protected Queue<BattleMoveValueEntityData> moveValueQueue = new();
        protected Queue<BattleUnitStateValueEntityData> unitStateIconValueQueue = new();

        public int TargetPosIdx;
        
        
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
        
        public int UnitIdx
        {
            get => BattleUnitData.Idx; 
            set => BattleUnitData.Idx = value;
        }

        // public Data_BattleUnit BattleUnit
        // {
        //     get => BattleUnitData; 
        //     set => BattleUnitData = value;
        // }

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

        [SerializeField] protected UnitDescTriggerItem UnitDescTriggerItem;
        
        //[SerializeField] private UnitDescTriggerItem UnitDescTriggerItem;

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
            showMoveValueTime = 0.8f;
            showMoveValueIconTime = 0.8f;

            
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            UnitDescTriggerItem.CloseForm();
        }

        

        protected void InitWeaponType(EWeaponHoldingType weaponHoldingType, EWeaponType weaponType, int weaponID)
        {
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

                    //animator.SetInteger(AnimationParameters.Side, (int)Side.Dual);
                    break;
                case EWeaponHoldingType.Left:
                    switch (weaponType)
                    {
                        case EWeaponType.Sword:
                            animator.SetInteger(AnimationParameters.LeftWeapon, (int)Weapon.LeftSword);
                            break;
                        case EWeaponType.Mace:
                            animator.SetInteger(AnimationParameters.LeftWeapon, (int)Weapon.LeftMace);
                            break;
                            animator.SetInteger(AnimationParameters.LeftWeapon, (int)Weapon.LeftDagger);
                            break;
                        case EWeaponType.Item:
                            animator.SetInteger(AnimationParameters.LeftWeapon, (int)Weapon.LeftItem);
                            break;
                        case EWeaponType.Pistol:
                            
                            animator.SetInteger(AnimationParameters.LeftWeapon, (int)Weapon.LeftPistol);
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
                    animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.Shield);
                    animator.SetInteger(AnimationParameters.Side, (int)Side.Left);
                    animator.SetInteger(AnimationParameters.WeaponSwitch, (int)AnimatorWeapon.ARMED);
                    break;
                case EWeaponHoldingType.Right:
                    switch (weaponType)
                    {
                        case EWeaponType.Sword:
                            animator.SetInteger(AnimationParameters.RightWeapon, (int)Weapon.RightSword);
                            break;
                        case EWeaponType.Spear:
                            animator.SetInteger(AnimationParameters.RightWeapon, (int)Weapon.RightSpear);
                            break;
                        case EWeaponType.Mace:
                            animator.SetInteger(AnimationParameters.RightWeapon, (int)Weapon.RightMace);
                            break;
                        case EWeaponType.Dagger:
                            animator.SetInteger(AnimationParameters.RightWeapon, (int)Weapon.RightDagger);
                            break;
                        case EWeaponType.Item:
                            animator.SetInteger(AnimationParameters.RightWeapon, (int)Weapon.RightItem);
                            break;
                        case EWeaponType.Pistol:
                            animator.SetInteger(AnimationParameters.RightWeapon, (int)Weapon.RightPistol);
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
                    animator.SetInteger(AnimationParameters.Weapon, (int)Weapon.Shield);
                    animator.SetInteger(AnimationParameters.Side, (int)Side.Right);
                    animator.SetInteger(AnimationParameters.WeaponSwitch, (int)AnimatorWeapon.ARMED);
                    break;
                case EWeaponHoldingType.Empty:
                    break;
                default:
                    break;
            }

            
            //animator.SetBool(AnimationParameters.Moving, true);
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.WeaponUnsheathTrigger);
            //animator.SetTrigger(AnimationParameters.Trigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            
            
        }

        protected async void AttachWeapon(EWeaponHoldingType weaponHoldingType, EWeaponType weaponType, int weaponID)
        {

            var weaponEntity = await GameEntry.Entity.ShowWeaponEntityAsync(weaponHoldingType, weaponType, weaponID);

            if (this == null || this.Entity == null || !GameEntry.Entity.HasEntity(this.Id))
            {
                GameEntry.Entity.HideEntity(weaponEntity);
                return;
            }
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
            
            
            RefreshData();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if(BattleManager.Instance.BattleState == EBattleState.EndBattle)
                return;
            
            RefreshRoatation();
            //ShowHurts();
            ShowMoveValues();
            ShowMoveValueIcons();
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

        }
        
        
        
        public void MultiHandleShoot(ActionData actionData)
        {
            foreach (var kv in actionData.TriggerDatas)
            {
                foreach (var triggerData in kv.Value)
                {
                    if (triggerData.BuffValue.BuffData.BuffEquipType != EBuffEquipType.Normal)
                    {
                        //死亡，溢出的伤害，攻击对方 需要下行代码
                        if (triggerData.TriggerDataType == ETriggerDataType.RoleAttribute &&
                            triggerData.BattleUnitAttribute == EUnitAttribute.HP)
                        {
                            HandleHit(triggerData.ActionUnitIdx, triggerData.EffectUnitIdx);
                        }
                            
                        continue;
                    }
                        
                    
                    var bulletData = new BulletData();
                    bulletData.ActionUnitIdx = triggerData.ActionUnitIdx;
                    var actionUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.ActionUnitIdx);
                    if (actionUnit != null)
                    {
                        bulletData.EffectColor = BattleUnitManager.Instance.GetEffectColor(actionUnit);
                    }
                    
                    
                    List<int> paths;
                    var triggerRange = triggerData.BuffValue.BuffData.TriggerRange.ToString(); 
                    if (triggerRange.Contains("Extend"))
                    {
                        paths = GameUtility.GetMoveIdxs(triggerData.ActionUnitGridPosIdx, triggerData.EffectUnitGridPosIdx);
                    }
                    else
                    {
                        paths = new List<int>();
                        paths.Add(triggerData.ActionUnitGridPosIdx);
                        paths.Add(triggerData.EffectUnitGridPosIdx);
                    }
                    
                    bulletData.MoveGridPosIdxs.AddRange(paths);
                    
                    var triggerActionDatas =
                        BattleBulletManager.Instance.GetTriggerActionDatas(triggerData.ActionUnitIdx, triggerData.EffectUnitIdx);
                    if(triggerActionDatas == null)
                        continue;
                    
                    foreach (var triggerActionData in triggerActionDatas)
                    {
                        bulletData.TriggerActionDataDict.Add(triggerData.EffectUnitGridPosIdx, triggerActionData);
                    }
                    
                    if (bulletData.TriggerActionDataDict.Count <= 0)
                    {
                        bulletData.MoveGridPosIdxs.Clear();
                        continue;
                    }

                    
                    if (triggerRange.Contains("Extend"))
                    {
                        foreach (var triggerActionData in bulletData.TriggerActionDataDict[triggerData.EffectUnitGridPosIdx])
                        {
                            if (triggerActionData is TriggerActionTriggerData triggerActionTriggerData)
                            {
                                if (triggerActionTriggerData.TriggerData != null)
                                {
                                    BattleBulletManager.Instance.UseTriggerData(triggerActionTriggerData.TriggerData);

                                }
                            }

                            if (triggerActionData is TriggerActionMoveData triggerActionMoveData)
                            {
                                if (triggerActionMoveData.MoveUnitData != null)
                                {
                                    BattleBulletManager.Instance.UseMoveActionData(triggerActionMoveData.MoveUnitData);
                                }
                            }

                            BattleManager.Instance.RefreshView();
                        }
                        GameEntry.Entity.ShowBattleBeamBulletEntityAsync(bulletData, ShootPos.position);
                    }
                    else if (triggerRange.Contains("Parabola"))
                    {
                        GameEntry.Entity.ShowBattleParabolaBulletEntityAsync(bulletData, ShootPos.position);
                    }
                    
                    else
                    {
                        GameEntry.Entity.ShowBattleParabolaBulletEntityAsync(bulletData, ShootPos.position);
                        //GameEntry.Entity.ShowBattleLineBulletEntityAsync(bulletData, ShootPos.position);
                    }
                    
                }
            }
            
            // var buffDatas = ;.Instance.GetBuffDatas(BattleUnit)
            // var triggerRange = buffDatas[0].TriggerRange;
            // var coord = GameUtility.GridPosIdxToCoord(BattleUnit.GridPosIdx);
            // for (int i = 0; i < Constant.Battle.ActionTypePoints[buffDatas[0].TriggerRange].Count; i++)
            // {
            //     var range = Constant.Battle.ActionTypePoints[buffDatas[0].TriggerRange][i];
            //     var bulletData = new BulletData();
            //     bulletData.ActionUnitIdx = BattleUnitData.Idx;
            //     bulletData.MoveGridPosIdxs.Add(BattleUnit.GridPosIdx);
            //     foreach (var deltaPos in range)
            //     {
            //         var deltaCoord = coord + deltaPos;
            //         if(!GameUtility.InGridRange(deltaCoord))
            //             continue;
            //             
            //         var gridPosIdx = GameUtility.GridCoordToPosIdx(deltaCoord);
            //         bulletData.MoveGridPosIdxs.Add(gridPosIdx);
            //         
            //         var effectUnit = BattleUnitManager.Instance.GetUnitByGridPosIdx(gridPosIdx);
            //         if(effectUnit == null)
            //             continue;
            //         
            //         if(!GameUtility.CheckUnitCamp(buffDatas[0].TriggerUnitCamps, BattleUnitData.UnitCamp, effectUnit.UnitCamp))
            //             continue;
            //
            //         
            //         var triggerActionDatas =
            //             BattleBulletManager.Instance.GetTriggerActionDatas(BattleUnitData.Idx, effectUnit.BattleUnit.Idx);
            //         
            //         if(triggerActionDatas == null)
            //             continue;
            //         
            //         foreach (var triggerActionData in triggerActionDatas)
            //         {
            //             bulletData.TriggerActionDataDict.Add(gridPosIdx, triggerActionData);
            //         }
            //         
            //         
            //
            //         if(triggerActionDatas != null && triggerRange.ToString().Contains("Extend"))
            //         {
            //             break;
            //         }
            //
            //         
            //     }
            //
            //     if (bulletData.TriggerActionDataDict.Count <= 0)
            //     {
            //         bulletData.MoveGridPosIdxs.Clear();
            //         continue;
            //     }
            //
            //     // if (bulletData.MoveGridPosIdxs.Count <= 1)
            //     //     continue;
            //     
            //     GameEntry.Entity.ShowBattleLineBulletEntityAsync(bulletData, ShootPos.position);
            //     
            // }
            
        }
        
        
        public void SingleHandleShoot()
        {
            var bulletData = new BulletData();
            bulletData.ActionUnitIdx = BattleUnitData.Idx;
            var moveIdxs = GameUtility.GetMoveIdxs(BattleUnitData.GridPosIdx, TargetPosIdx);
            bulletData.MoveGridPosIdxs.AddRange(moveIdxs);
            var endPosIdx = moveIdxs[moveIdxs.Count - 1];
            
            var triggerActionDatas =
                BattleBulletManager.Instance.GetTriggerActionDatas(BattleUnitData.Idx);

            if (triggerActionDatas != null)
            {
                foreach (var kv in triggerActionDatas)
                {
                    foreach (var triggerActionData in kv.Value)
                    {
                        bulletData.TriggerActionDataDict.Add(endPosIdx, triggerActionData);
                    }
                        
                }
            }
                

            
            
            
            // var buffData = BattleUnitManager.Instance.GetBuffDatas(BattleUnit);
            // var triggerRange = buffData[0].TriggerRange;
            // var endPosIdx = moveIdxs[moveIdxs.Count - 1];
            // var endCoord = GameUtility.GridPosIdxToCoord(endPosIdx);
            // for (int i = 0; i < Constant.Battle.ActionTypePoints[triggerRange].Count; i++)
            // {
            //     var range= Constant.Battle.ActionTypePoints[triggerRange][i];
            //
            //     foreach (var deltaPos in range)
            //     {
            //         var deltaCoord = endCoord + deltaPos;
            //         if (!GameUtility.InGridRange(deltaCoord))
            //             continue;
            //
            //         var gridPosIdx = GameUtility.GridCoordToPosIdx(deltaCoord);
            //
            //
            //         var effectUnit = BattleUnitManager.Instance.GetUnitByGridPosIdx(gridPosIdx);
            //         if (effectUnit == null)
            //             continue;
            //
            //         //, effectUnit.BattleUnit.Idx
            //         var triggerActionDatas =
            //             BattleBulletManager.Instance.GetTriggerActionDatas(BattleUnitData.Idx);
            //
            //         if (triggerActionDatas == null)
            //             continue;
            //
            //         foreach (var kv in triggerActionDatas)
            //         {
            //             foreach (var triggerActionData in kv.Value)
            //             {
            //                 bulletData.TriggerActionDataDict.Add(endPosIdx, triggerActionData);
            //             }
            //             
            //         }
            //
            //         if (triggerActionDatas != null && triggerRange.ToString().Contains("Extend"))
            //         {
            //             break;
            //         }
            //
            //     }
            //
            //
            // }

            GameEntry.Entity.ShowBattleParabolaBulletEntityAsync(bulletData, ShootPos.position);
            
        }
        public async void Hit()
        {

        }
        
        public async void HandleHit(EAttackCastType unitAttackCastType)
        {
            await ShowEffectAttackEntity(unitAttackCastType);

            BattleBulletManager.Instance.ActionUnitTrigger(this.BattleUnitData.Idx);
        }
        
        public async void HandleHit(int actionUnitIdx, int effectUnitIx)
        {
            await ShowEffectAttackEntity(UnitAttackCastType);

            BattleBulletManager.Instance.ActionUnitTrigger(actionUnitIdx, effectUnitIx);
        }

        public void GetHit()
        {

        }
        
        public async void HandleGetHit()
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
        private async Task ShowEffectAttackEntity(EAttackCastType unitAttackCastType)
        {
            var triggerActionDataDict = BattleBulletManager.Instance.GetTriggerActionDatas(this.BattleUnitData.Idx);
            switch (unitAttackCastType)
            {
                case EAttackCastType.CloseSingle:
                    ShowEffectAttackEntity_CloseSingle(triggerActionDataDict);
                    break;
                case EAttackCastType.CloseMulti:
                    ShowEffectAttackEntity_CloseMulti(triggerActionDataDict);
                    break;
                case EAttackCastType.RemoteSingle:
                    break;
                case EAttackCastType.ExtendMulti:
                    ShowEffectAttackEntity_Empty(triggerActionDataDict);
                    break;
                case EAttackCastType.ParabolaMulti:
                    ShowEffectAttackEntity_Empty(triggerActionDataDict);
                    break;
                case EAttackCastType.LineMulti:
                    ShowEffectAttackEntity_Empty(triggerActionDataDict);
                    break;
                case EAttackCastType.Empty:
                default:
                    break;
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
        
        private async void ShowEffectAttackEntity_CloseSingle(GameFrameworkMultiDictionary<int, ITriggerActionData> triggerActionDataDict)
        {
            var effectName = "EffectCloseSingleAttackEntity";
            var effectPos = EffectHurtPos.position;

            foreach (var kv in triggerActionDataDict)
            {
                foreach (var triggerActionData in kv.Value)
                {
                    if (triggerActionData is TriggerActionTriggerData triggerActionTriggerData)
                    {
                        if(triggerActionTriggerData.TriggerData.TriggerDataType != ETriggerDataType.RoleAttribute)
                            continue;
                        
                        var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerActionTriggerData.TriggerData.EffectUnitIdx);

                        if (triggerActionTriggerData.TriggerData.BuffTriggerType == EBuffTriggerType.Pass ||
                            triggerActionTriggerData.TriggerData.BuffTriggerType == EBuffTriggerType.BePass ||
                            triggerActionTriggerData.TriggerData.UnitStateDetail.UnitState == EUnitState.AtkPassEnemy || 
                            triggerActionTriggerData.TriggerData.UnitStateDetail.UnitState == EUnitState.AtkPassUs)
                        {
                            //effectName = "EffectCloseSingleAttackEntity";
                            //effectPos = EffectAttackPos.position;
                            effectUnit.ShowEffectHurtEntity();
                        }
                        else
                        {
                            var actionUnit =
                                BattleUnitManager.Instance.GetUnitByIdx(triggerActionTriggerData.TriggerData.ActionUnitIdx);
                            if(actionUnit == null)
                                continue;
                            
                            ShowEffectAttackEntity(effectName, effectPos,
                                BattleUnitManager.Instance.GetEffectColor(actionUnit), effectUnit.Position);
                        }

                       
    
                        
                    }
                    
                }
            }
        }
        
        private async void ShowEffectAttackEntity_LineMulti(GameFrameworkMultiDictionary<int, ITriggerActionData> triggerActionDataDict)
        {
            var effectName = "EffectLineMultiAttackEntity";
            var effectPos = EffectHurtPos.position;
            var gridPosIdxs = new List<int>();
            foreach (var kv in triggerActionDataDict)
            {
                foreach (var triggerActionData in kv.Value)
                {
                    if (triggerActionData is TriggerActionTriggerData triggerActionTriggerData)
                    {
                        var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerActionTriggerData.TriggerData.EffectUnitIdx);
                    
                        if(effectUnit != null)
                            gridPosIdxs.Add(effectUnit.GridPosIdx);
                    }
                    
                }
            }
            

            var firstTriggerData = triggerActionDataDict[triggerActionDataDict.First().Key].First() as TriggerActionTriggerData;
            var actionUnit =
                BattleUnitManager.Instance.GetUnitByIdx(firstTriggerData.TriggerData.ActionUnitIdx);
            if (actionUnit != null)
            {
                var effectAttackEntity = await GameEntry.Entity.ShowLineMultiEffectEntityAsync(effectName, effectPos, BattleUnitManager.Instance.GetEffectColor(actionUnit), gridPosIdxs);

            }
            

        }

        private async void ShowEffectAttackEntity_CloseMulti(GameFrameworkMultiDictionary<int, ITriggerActionData> triggerActionDataDict)
        {
            var effectName = "EffectCloseMultiAttackEntity";
            var effectPos = EffectHurtPos.position;

            foreach (var kv in triggerActionDataDict)
            {
                foreach (var triggerActionData in kv.Value)
                {
                    if (triggerActionData is TriggerActionTriggerData triggerActionTriggerData)
                    {
                        var effectUnit =
                            BattleUnitManager.Instance.GetUnitByIdx(triggerActionTriggerData.TriggerData.EffectUnitIdx);
                        if(effectUnit == null)
                            continue;

                        if (triggerActionTriggerData.TriggerData.BuffTriggerType == EBuffTriggerType.Pass ||
                            triggerActionTriggerData.TriggerData.BuffTriggerType == EBuffTriggerType.BePass)
                        {
                            effectName = "EffectCloseSingleAttackEntity";
                            effectPos = EffectAttackPos.position;
                        }
                        
                        var actionUnit =
                            BattleUnitManager.Instance.GetUnitByIdx(triggerActionTriggerData.TriggerData.ActionUnitIdx);
                        if(actionUnit == null)
                            continue;

                        ShowEffectAttackEntity(effectName, effectPos,
                            BattleUnitManager.Instance.GetEffectColor(actionUnit), effectPos);
                        break;
                    }
                }
                break;
            }
        }

        private async void ShowEffectAttackEntity_Empty(GameFrameworkMultiDictionary<int, ITriggerActionData> triggerActionDataDict)
        {
            
        }

        
        private async void ShowEffectAttackEntity(string effectName, Vector3 effectPos, EColor effectColor, Vector3? lookAtPos = null)
        {
            var effectAttackEntity = await GameEntry.Entity.ShowCommonEffectEntityAsync(effectName, effectPos, effectColor);

            if (lookAtPos != null)
            {
                effectAttackEntity.transform.LookAt(new Vector3(lookAtPos.Value.x, effectAttackEntity.transform.position.y, lookAtPos.Value.z));
            }
            
            // if (!effectAttackEntity.AutoHide)
            // {
            //     GameUtility.DelayExcute(1f, () =>
            //     {
            //         GameEntry.Entity.HideEntity(effectAttackEntity);
            //     });
            // }
        }
        
        public void FootL()
        {
            
        }
        
        public void FootR()
        {
            
        }
        
        public void Idle()
        {

            //animator.SetInteger(AnimationParameters.Weapon, Weapon.Shield);
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
            
            GridPosIdx = moveGridPosIdxs.Count > 0 ? moveGridPosIdxs[0] : GridPosIdx;
            transform.position = moveGridPosIdxs.Count > 0 ? GameUtility.GridPosIdxToPos(moveGridPosIdxs[0]) : transform.position;
            for (int i = 1; i < moveGridPosIdxs.Count; i++)
            {
                var moveGridPosIdx = moveGridPosIdxs[i];

                var hasUnit = BattleUnitManager.Instance.GetUnitByGridPosIdx(moveGridPosIdx, null, null, null, BattleUnitData.Idx) !=  null;
                
                var pos = GameUtility.GetMovePos(unitActionState, moveGridPosIdxs, i, hasUnit);
                
                var tIdx = i;

                var time = Constant.Unit.MoveTimes[unitActionState] * (tIdx - 1);
                time = time < 0 ? 0 : time;

                GameUtility.DelayExcute(time, () =>
                {
                    var moveTIdx = tIdx;
                    var nextMoveGridPosIdx = moveGridPosIdx;
                    var movePos = pos;
                    //Log.Debug("pos:" + this.transform.position.x);
                    //Log.Debug("LookAt:" + pos.x);

                    
                    

                    if (tIdx != moveGridPosIdxs.Count - 1 && (unitActionState == EUnitActionState.Rush ||
                                                              unitActionState == EUnitActionState.Fly))
                    {
                        roleRoot.LookAt(new Vector3(pos.x, transform.position.y - 1.5f, pos.z));
                    }
                    else
                    {
                        roleRoot.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                    }
                    
                    if (unitActionState == EUnitActionState.Fly)
                    {
                        roleRoot.Rotate(new Vector3(0, 1, 0), 180);
                    }
                    
                    
                    if (BattleUnitData.Exist())
                    {
                        if (unitActionState == EUnitActionState.Fly || unitActionState == EUnitActionState.Rush)
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
                            //Log.Debug("movePos:" + movePos);
                            BattleFightManager.Instance.MoveEffectAction(unitActionState, moveActionData, moveTIdx, BattleUnitData.Idx);
                        
                        });
                    }

                });
                
            }

            var moveCount = moveGridPosIdxs.Count > 1 ? moveGridPosIdxs.Count - 1 : 1;

            GameUtility.DelayExcute(moveCount * Constant.Unit.MoveTimes[unitActionState]  + 0.1f, () =>
            {
                BattleUnitData.RoundGridMoveCount += moveCount;
                if (BattleUnitData.Exist())
                {
                    animator.SetInteger(AnimationParameters.Jumping, 0);
                    Idle();
                    
                    LookAtHero();
                    if (moveGridPosIdxs.Count > 0)
                    {
                        GridPosIdx = moveGridPosIdxs[moveGridPosIdxs.Count - 1];
                    }
                    
                }
                IsMove = false;

                HeroManager.Instance.UpdateCacheHPDelta();
                
                if (BattleManager.Instance.BattleState == EBattleState.UseCard && unitActionState != EUnitActionState.Run)
                {
                    BattleManager.Instance.RefreshEnemyAttackData();
                }

                
                BattleManager.Instance.RefreshView();
                BattleAreaManager.Instance.RefreshObstacles();
                
            });

            //return moveCount * Constant.Unit.MoveTimes[unitActionState] + 0.1f;
        }

        public virtual void LookAtHero()
        {
            //var pos = HeroManager.Instance.HeroEntity.Position;
            //roleRoot.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
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
        
        public void Rush(MoveActionData moveActionData)
        {
            //return
            Move(EUnitActionState.Rush, moveActionData);

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
        
        public virtual void Attack(ActionData actionData)
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
                    RemoteSingleAttack();
                    break;
                case EAttackCastType.ExtendMulti:
                    ExtendMultiAttack(actionData);
                    break;
                case EAttackCastType.LineMulti:
                    LineMultiAttack(actionData);
                    break;
                case EAttackCastType.ParabolaMulti:
                    ParabolaMultiAttack(actionData);
                    break;
                case EAttackCastType.Empty:
                    EmptyAttack();
                    break;
                default:
                    break;
            }
            
        }
        
        public void RemoteSingleAttack()
        {

            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.AttackTrigger);
            animator.SetInteger(AnimationParameters.Action, (int)AttackCastType.Cast1);
            animator.SetTrigger(AnimationParameters.Trigger);
            
            // animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.AttackCastTrigger);
            // animator.SetTrigger(AnimationParameters.Trigger);
            // animator.SetInteger(AnimationParameters.Action, (int)AttackCastType.Cast1);
            // GameUtility.DelayExcute(1f, () =>
            // {
            //     animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.CastEndTrigger);
            //     animator.SetTrigger(AnimationParameters.Trigger);
            // });
            GameUtility.DelayExcute(0.15f, () =>
            {
                SingleHandleShoot();
            });
        }
        
        public void ExtendMultiAttack(ActionData actionData)
        {
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.SpecialAttackTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            animator.SetInteger(AnimationParameters.Action, (int)AttackCastType.Cast1);
            GameUtility.DelayExcute(1f, () =>
            {
                animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.SpecialEndTrigger);
                animator.SetTrigger(AnimationParameters.Trigger);
            });
            GameUtility.DelayExcute(0.15f, () =>
            {
                MultiHandleShoot(actionData);
            });
        }
        
        public void LineMultiAttack(ActionData actionData)
        {
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.AttackTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            animator.SetInteger(AnimationParameters.Action, (int)AttackCastType.Cast1);
            
            
            
            var gridPosIdxs = new List<int>();
            foreach (var kv in actionData.TriggerDatas)
            {
                foreach (var triggerData in kv.Value)
                {
                    var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(triggerData.EffectUnitIdx);
                    
                    if(effectUnit != null)
                        gridPosIdxs.Add(effectUnit.GridPosIdx);
                }
                
            }

            GameUtility.DelayExcute(0.15f, () =>
            {
                var effectName = "EffectLineMultiAttackEntity";
                GameEntry.Entity.ShowLineMultiEffectEntityAsync(effectName, transform.position, BattleUnitManager.Instance.GetEffectColor(this), gridPosIdxs);
                BattleBulletManager.Instance.ActionUnitTrigger(this.BattleUnitData.Idx);
                HeroManager.Instance.UpdateCacheHPDelta();
                //MultiHandleShoot(actionData);
            });
        }
        
        public void ParabolaMultiAttack(ActionData actionData)
        {
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.SpecialAttackTrigger);
            
            animator.SetInteger(AnimationParameters.Action, (int)AttackCastType.Cast1);
            animator.SetTrigger(AnimationParameters.Trigger);
            GameUtility.DelayExcute(1f, () =>
            {
                animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.SpecialEndTrigger);
                animator.SetTrigger(AnimationParameters.Trigger);
            });
            GameUtility.DelayExcute(0.15f, () =>
            {
                MultiHandleShoot(actionData);
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
            GameUtility.DelayExcute(0.15f, () =>
            {
                HandleHit(EAttackCastType.CloseMulti);
                HeroManager.Instance.UpdateCacheHPDelta();
            });
        }

        public void EmptyAttack()
        {
            HandleHit(EAttackCastType.Empty);
            HeroManager.Instance.UpdateCacheHPDelta();
        }
        
        public void CloseSingleAttack()
        {
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.AttackTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            animator.SetInteger(AnimationParameters.Action, (int)AttackCastType.Cast1);

            GameUtility.DelayExcute(0.15f, () =>
            {
                HandleHit(EAttackCastType.CloseSingle);
            });
        }
        
        public void MoveAttack()
        {
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.AttackTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            animator.SetInteger(AnimationParameters.Action, (int)AttackCastType.Cast1);

            HandleHit(EAttackCastType.CloseSingle);
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
            if (IsPointer)
            {
                RefreshHP();
            }
            else
            {
                RefreshDamageState();
            }
            
            
        }

        public virtual void RefreshHP()
        {
            hpAndDamageNode.SetActive(true);
            damageNode.SetActive(false);

            var curHP = BattleUnitData.CurHP;
            curHP = curHP < 0 && BattleUnitData.FuneCount(EBuffID.Spec_UnDead) <= 0 ? 0 : curHP;
            hp.text = curHP.ToString();
            // hp.text = curHP + "/" +
            //           BattleUnitData.MaxHP;
            
            //var hurt = BattleFightManager.Instance.GetTotalDelta(this.UnitIdx, EHeroAttribute.CurHP);
            //damage.text = hurt > 0 ? "+" + hurt : hurt.ToString();
            damage.text = "";
            // if (hurt != 0)
            // {
            //     
            // }
            // else
            // {
            //     damage.text = "";
            // }

        }

        public void RefreshDamageState()
        {
            hpAndDamageNode.SetActive(false);
            damageNode.SetActive(false);
            
            // hpAndDamageNode.SetActive(false);
            // damageNode.SetActive(true);
            //
            // var hurt = BattleFightManager.Instance.GetTotalDelta(this.UnitIdx, EHeroAttribute.CurHP);
            // if (hurt != 0)
            // {
            //     damage2.text = hurt.ToString();
            // }
            // else
            // {
            //     damage2.text = "";
            // }
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
            
        }
        
        public void RefreshRoatation()
        {
            cameraQuaternion.SetLookRotation(Camera.main.transform.forward, Camera.main.transform.up);
            uiNode.transform.rotation = cameraQuaternion;

            var dis = Mathf.Abs(AreaController.Instance.GetDistanceToPoint(uiNode.transform.position));
            
            uiNode.transform.localScale = Vector3.one *  dis / 12f;
            
        }
        
        private async void ShowEffectHurtEntity()
        {
            var effectHurt = await GameEntry.Entity.ShowCommonEffectEntityAsync("EffectHurtEntity",
                EffectHurtPos.position, BattleUnitManager.Instance.GetEffectColor(this));
            effectHurt.transform.parent = EffectHurtPos;
        }

        public virtual void HurtAnimation()
        {
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.GetHitTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            animator.SetInteger(AnimationParameters.Action, (int)HitType.Back1);
        }

        public void Hurt()
        {

            HurtAnimation();

            // GameUtility.DelayExcute(0.15f, () =>
            // {
            //     HandleGetHit();
            // });
            
            // if (UnitActionState == EUnitActionState.Run)
            // {
            //     RunHurt();
            // }
            // else
            // {
            //     SetAction(EUnitActionState.Hurt);
            // }
            
            

            if (CurHP == 0 && BattleUnitData.FuneCount(EBuffID.Spec_UnDead) <= 0)
            {
                CurHP = -1;
                GameUtility.DelayExcute(1f, () =>
                {
                    Dead();
                });
               
            }
            
        }
        
        public void Recover()
        {
            // var animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            // if(animatorClipInfo[0].clip.name == "Unarmed-Idle")
            //     return;
            
            
            animator.SetInteger(AnimationParameters.TriggerNumber, (int)AnimatorTrigger.ActionTrigger);
            animator.SetTrigger(AnimationParameters.Trigger);
            animator.SetInteger(AnimationParameters.Action, (int)EmoteType.Boost);
            //SetAction(EUnitActionState.Recover);
        }
        
        public virtual int  ChangeCurHP(int changeHP, bool useDefense, bool addHeroHP, bool changeHPInstantly, bool showValue = true)
        {
            var hpDelta = BattleManager.Instance.ChangeHP(BattleUnitData, changeHP, GamePlayManager.Instance.GamePlayData,
                EHPChangeType.Action, useDefense, addHeroHP, changeHPInstantly);

            if (showValue)
            {
                var moveParams = new MoveParams()
                {
                    FollowGO = this.gameObject,
                    DeltaPos = new Vector2(0, 25f),
                    IsUIGO = false,
                };

                var isAddHP = (this is BattleSoliderEntity && changeHP < 0) || this is BattleCoreEntity;
            
                var targetMoveParams = new MoveParams()
                {
                    FollowGO = isAddHP ? AreaController.Instance.UICore : this.gameObject,
                    DeltaPos = isAddHP ? new Vector2(0, -25f) : new Vector2(0, 100f),
                    IsUIGO = isAddHP,
                };


                if (!changeHPInstantly)
                {
                    AddMoveValue(changeHP, changeHP, CurValueEntityIdx++, false,
                        this is BattleSoliderEntity && changeHP < 0, moveParams,
                        targetMoveParams);
                    //AddHurts(changeHP);
                }
                // if (hpDelta != 0)
                else
                {
                    AddMoveValue(changeHP, changeHP, CurValueEntityIdx++, false,
                        this is BattleSoliderEntity && changeHP < 0, moveParams,
                        targetMoveParams);
                    //AddHurts(hpDelta);
                }
                        
            }
            

            return hpDelta;
        }

        public void UpdatePos(Vector3 pos)
        {
            Position = pos;
        }

        // public void AddHurts(int hurt)
        // {
        //     hurtQueue.Enqueue(hurt);
        // }
        
        public void AddMoveValue(int startValue, int endValue, int entityIdx = -1, bool isLoop = false, bool isAdd = false,
             MoveParams moveParams = null, MoveParams targetMoveParams = null)
        {
            var data = ReferencePool.Acquire<BattleMoveValueEntityData>();
            data.Init(GameEntry.Entity.GenerateSerialId(), startValue, endValue, entityIdx, isLoop,
                isAdd, moveParams, targetMoveParams);

            moveValueQueue.Enqueue(data);
        }
        
        public void AddUnitStateMoveValue(EUnitState unitState, int startValue, int endValue, int entityIdx = -1, bool isLoop = false, bool isAdd = false,
            MoveParams moveParams = null, MoveParams targetMoveParams = null)
        {
            var data = ReferencePool.Acquire<BattleUnitStateValueEntityData>();
            data.Init(GameEntry.Entity.GenerateSerialId(), startValue, endValue, unitState, entityIdx, isLoop, isAdd, moveParams,
                targetMoveParams);

            unitStateIconValueQueue.Enqueue(data);
        }

        private float showMoveValueTime = 0.8f;
        protected async void ShowMoveValues()
        {
            if(moveValueQueue.Count <= 0)
                return;
            
            showMoveValueTime += Time.deltaTime;
            if (showMoveValueTime > 0.8f)
            {
                showMoveValueTime = 0;
                
                BattleMoveValueEntity entity = null;

                BattleMoveValueEntityData data = null;
                do
                {
                    data = null;
                    if (moveValueQueue.Count > 0)
                    {
                        data = moveValueQueue.Dequeue();
                    }
                    if (moveValueQueue.Count <= 0)
                    {
                        showMoveValueTime = 0.8f;
                    }
                } while(data != null && data.EntityIdx < ShowValueEntityIdx);
                
                if(data == null)
                    return;
                
                //var data = moveValueQueue.Dequeue();

                entity = await GameEntry.Entity.ShowBattleMoveValueEntityAsync(data);
                
                // if (data is UnitStateIconValueEntityData unitStateIconValueEntityData)
                // {
                //     entity = await GameEntry.Entity.ShowBattleUnitStateMoveValueEntityAsync(unitStateIconValueEntityData);
                // }
                // else 
                
                if (GameEntry.Entity.HasEntity(entity.Id))
                {
                    var entityIdx = entity.BattleMoveValueEntityData.EntityIdx;
                    if (entityIdx == -1)
                    {
                    }
                    else if (entityIdx < ShowValueEntityIdx)
                    {
                
                        GameEntry.Entity.HideEntity(entity);
                    }
                    else
                    {
                
                        BattleValueEntities.Add(entity.Entity.Id, entity);
                    }
                }
            }
        }
        
        private float showMoveValueIconTime = 0.8f;
        protected async void ShowMoveValueIcons()
        {
            if(unitStateIconValueQueue.Count <= 0)
                return;
            
            showMoveValueIconTime += Time.deltaTime;
            if (showMoveValueIconTime > 0.8f)
            {
                showMoveValueIconTime = 0;
     
                BattleUnitStateValueEntityData data = null;
                do
                {
                    data = null;
                    if (unitStateIconValueQueue.Count > 0)
                    {
                        data = unitStateIconValueQueue.Dequeue();
                    }
                    
                    if (unitStateIconValueQueue.Count <= 0)
                    {
                        showMoveValueIconTime = 0.8f;
                    }

                } while(data != null && data.EntityIdx < ShowValueEntityIdx);
                
                if(data == null)
                    return;
                
                //var data = moveValueQueue.Dequeue();

                var entity = await GameEntry.Entity.ShowUnitStateIconValueEntityAsync(data);
                
                
                if (GameEntry.Entity.HasEntity(entity.Id))
                {
                    var entityIdx = entity.BattleUnitStateValueEntityData.EntityIdx;
                    if (entityIdx == -1)
                    {
                    }
                    else if (entityIdx < ShowValueEntityIdx)
                    {
                
                        GameEntry.Entity.HideEntity(entity);
                    }
                    else
                    {
                
                        BattleValueEntities.Add(entity.Entity.Id, entity);
                    }
                }
            }
        }

        //private float showHurtTime = 0f;
        // protected async void ShowHurts()
        // {
        //     showHurtTime += Time.deltaTime;
        //     if (showHurtTime > 0.4f && hurtQueue.Count > 0)
        //     {
        //         showHurtTime = 0;
        //         var hurt = hurtQueue.Dequeue();
        //         await ShowBattleHurts(hurt);
        //     }
        // }
        
        // protected async virtual Task ShowBattleHurts(int hurt)
        // {
        //     var effectUnitPos = Root.position;
        //
        //     // effectUnitPos.y += 1f;
        //     //
        //     // var uiCorePos = AreaController.Instance.UICore.transform.position;
        //     // uiCorePos.y -= 0.4f;
        //     //
        //     // var pos = RectTransformUtility.WorldToScreenPoint(AreaController.Instance.UICamera,
        //     //     uiCorePos);
        //     //
        //     // Vector3 position = new Vector3(pos.x, pos.y, Camera.main.transform.position.z);
        //     // Vector3 uiCoreWorldPos = Camera.main.ScreenToWorldPoint(position);
        //     
        //     // var uiCorePos = AreaController.Instance.UICore.transform.localPosition;
        //     // uiCorePos.y -= 25f;
        //     // var uiLocalPoint = PositionConvert.WorldPointToUILocalPoint(
        //     //     AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), effectUnitPos);
        //     // var uiLocalPoint2 = uiLocalPoint;
        //     // uiLocalPoint.y += 25f;
        //     // uiLocalPoint2.y += 75f;
        //
        //     var moveParams = new MoveParams()
        //     {
        //         FollowGO = this.gameObject,
        //         DeltaPos = new Vector2(0, 125f),
        //         IsUIGO = false,
        //     };
        //     
        //     var targetMoveParams = new MoveParams()
        //     {
        //         FollowGO = AreaController.Instance.UICore,
        //         DeltaPos = new Vector2(0, -25f),
        //         IsUIGO = true,
        //     };
        //     
        //     AddMoveValue(hurt, hurt, -1, false,
        //         this is BattleSoliderEntity && hurt < 0, moveParams, targetMoveParams);
        //
        //     
        //     // await GameEntry.Entity.ShowBattleMoveValueEntityAsync(hurt, hurt, 0, -1, false,
        //     //     this is BattleSoliderEntity && hurt < 0, moveParams, targetMoveParams);
        //     //
        //     // var hurtEntity = await GameEntry.Entity.ShowBattleHurtEntityAsync(BattleUnitData.GridPosIdx, hurt);
        //     // hurtEntity.transform.parent = Root;
        // }

        public bool IsPointer = false;
        
        public virtual void OnPointerEnter(BaseEventData baseEventData)
        {
            //IsPointer = true;
            //GameEntry.Event.Fire(null, SelectGridEventArgs.Create(BattleUnitData.GridPosIdx, true));
            //GameEntry.Event.Fire(null, ShowGridDetailEventArgs.Create(BattleUnitData.GridPosIdx, EShowState.Show)); 
            //RefreshHP();
            
            
        }
        
        public virtual void OnPointerExit(BaseEventData baseEventData)
        {
            //IsPointer = false;
            //GameEntry.Event.Fire(null, SelectGridEventArgs.Create(BattleUnitData.GridPosIdx, false));
            //GameEntry.Event.Fire(null, ShowGridDetailEventArgs.Create(BattleUnitData.GridPosIdx, EShowState.Unshow)); 
            //RefreshDamageState();
        }
        
        public virtual void OnPointerClick(BaseEventData baseEventData)
        {
            if (Input.GetMouseButtonUp(0))
            {
                GameEntry.Event.Fire(null, ClickGridEventArgs.Create(BattleUnitData.GridPosIdx)); 
            }
            
        }

        public void OnPointerEnter()
        {
            IsPointer = true;
            RefreshHP();
            UnitDescTriggerItem.OnPointerEnter();
        }
        
        public void OnPointerExit()
        {
            IsPointer = false;
            RefreshDamageState();
            UnitDescTriggerItem.OnPointerExit();
        }
        
        public void ShowTags(int actionUnitIdx, bool isShowAttackPos = true)
        {
            if(BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
                return;

            BattleStaticAttackTagManager.Instance.UnshowStaticAttackTags();
            ShowAttackTag(actionUnitIdx, isShowAttackPos);
            ShowFlyDirect(actionUnitIdx);
            ShowBattleIcon(actionUnitIdx, EBattleIconType.Collision);
            ShowDisplayValue(actionUnitIdx);
            ShowDisplayIcon(actionUnitIdx);
        }

        public void ShowHurtTags(int effectUnitIdx, int actionUnitIdx = -1)
        {
            if(BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
                return;

            // ShowHurtAttackTag(effectUnitIdx, actionUnitIdx);
            // ShowHurtFlyDirect(effectUnitIdx, actionUnitIdx);
            // ShowHurtBattleIcon(effectUnitIdx, actionUnitIdx, EBattleIconType.Collision);
            // ShowHurtDisplayValue(effectUnitIdx, actionUnitIdx);
            // ShowHurtDisplayIcon(effectUnitIdx, actionUnitIdx);
        }
        
        public void UnShowTags()
        {
            showEntityIdx = 0;
            UnShowAttackTags();
            UnShowFlyDirects();
            UnShowBattleIcons();
            UnShowDisplayValues();
            UnShowDisplayIcons();
            
            
        }
        
        // public void ShowCollider(bool isShow)
        // {
        //     boxCollider.enabled = isShow;
        //
        // }

        public void SetPosition(int gridPosIdx)
        {
            this.Position = GameUtility.GridPosIdxToPos(gridPosIdx);
            
        }

    }
}
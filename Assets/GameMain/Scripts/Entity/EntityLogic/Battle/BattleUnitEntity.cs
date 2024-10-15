using System.Collections.Generic;
using DG.Tweening;
using RPGCharacterAnims;
using RPGCharacterAnims.Lookups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleUnitEntity : Entity, IMoveGrid
    {
        [SerializeField] protected Animator Animator;
        [SerializeField] protected Transform roleRoot;
        [SerializeField] protected TextMesh hp;
        [SerializeField] protected TextMesh damage;
        [SerializeField] protected GameObject uiNode;
        [SerializeField] protected RPGCharacterController Controller;
        
        protected Quaternion cameraQuaternion = Quaternion.identity;
        
        protected Data_BattleUnit BattleUnitData { get; set; }
        protected bool IsMove = false;
        
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
        
        public EUnitActionState UnitActionState { get; set; }

        protected int TopLayerIdx;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            TopLayerIdx = Animator.GetLayerIndex("TopLayer");
            
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

        public void AfterRunAction()
        {
            Log.Debug("AfterRunAction");
            //Animator.SetTrigger(EUnitActionState.Run.ToString());
            if (IsMove)
            {
                SetAction(EUnitActionState.Run);
            }
            else
            {
                SetAction(EUnitActionState.Idle);
            }
        }
        

        public void AnimatorSetLayerWeight()
        {
            if (UnitActionState == EUnitActionState.RunAttack || UnitActionState == EUnitActionState.RunHurt)
            {
                //Animator.speed = 0.5f;
                Animator.SetLayerWeight(TopLayerIdx, 1);
                //Animator.SetLayerWeight(BaseLayerIdx, 0);
            }
            else
            {
                //Animator.speed = 1f;
                // Animator.SetLayerWeight(BaseLayerIdx, 1);
                Animator.SetLayerWeight(TopLayerIdx, 0);
            }

        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            IsMove = false;
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
            
            
        }
        
        public void SetAction(EUnitActionState actionState)
        {
            // if (actionState == EUnitActionState.Hurt)
            // {
            //     if (UnitActionState == EUnitActionState.Run)
            //     {
            //         actionState = EUnitActionState.RunHurt;
            //     }
            //     else
            //     {
            //         actionState = EUnitActionState.Hurt;
            //     }
            // }
            //
            // if (actionState == EUnitActionState.Attack)
            // {
            //     if (UnitActionState == EUnitActionState.Run)
            //     {
            //         actionState = EUnitActionState.RunAttack;
            //     }
            //     else
            //     {
            //         actionState = EUnitActionState.Attack;
            //     }
            // }
            
            UnitActionState = actionState;
            AnimatorSetLayerWeight();

            // if (BattleUnitData is Data_BattleMonster)
            // {
            //     //Log.Debug("act:" + actionState.ToString());
            // }
  
            Animator.SetTrigger(actionState.ToString());

        }

        
        
        public void Idle()
        {
            
            SetAction(EUnitActionState.Idle);
        }
        
        public void Dodge()
        {
            SetAction(EUnitActionState.Dodge);
        }

        public float Move(EUnitActionState unitActionState, MoveActionData moveActionData)
        {
            IsMove = true;
            SetAction(unitActionState);

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
                        transform.DOMove(movePos, moveTIdx == 0 ? 0 : Constant.Unit.MoveTimes[unitActionState]).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            GridPosIdx = nextMoveGridPosIdx;
                            
                            FightManager.Instance.MoveEffectAction(unitActionState, moveActionData, moveTIdx, BattleUnitData.ID);
   
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
                    Idle();
                    var pos = BattleHeroManager.Instance.HeroEntity.Position;
                    roleRoot.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                    GridPosIdx = moveGridPosIdxs[moveGridPosIdxs.Count - 1];
                }
                IsMove = false;
            });

            return moveCount * Constant.Unit.MoveTimes[unitActionState] + 0.1f;
        }
        
        public float Run(MoveActionData moveActionData)
        {
            return Move(EUnitActionState.Run, moveActionData);

        }
        
        public float Fly(MoveActionData moveActionData)
        {
            return Move(EUnitActionState.Fly, moveActionData);

        }
        
        public void Attack()
        {
            SetAction(EUnitActionState.Attack);
        }
        
        public void RunAttack()
        {
            
            SetAction(EUnitActionState.RunAttack);
            GameUtility.DelayExcute(0.8f, () =>
            {
                AfterRunAction();
            });
        }
        
        public void RunHurt()
        {
            
            SetAction(EUnitActionState.RunHurt);
            GameUtility.DelayExcute(1.2f, () =>
            {
                AfterRunAction();
            });
        }
        
        public void RefreshData()
        {
            RefreshHP();
            RefreshDamageState();
        }

        public void RefreshHP()
        {
            var curHP = BattleUnitData.CurHP;
            curHP = curHP < 0 ? 0 : curHP;
            hp.text = curHP + "/" +
                      BattleUnitData.MaxHP;

        }

        public void RefreshDamageState()
        {
            var hurt = FightManager.Instance.GetTotalDelta(this.ID, EHeroAttribute.CurHP);
            this.damage.text = "hurt:" + hurt;
        }
        
        public virtual void Quit()
        {
            SetAction(EUnitActionState.Quit);
            GameUtility.DelayExcute(1.5f, () =>
            {
                GameEntry.Entity.HideEntity(this);
            });
        }
        
        public virtual void Dead()
        {
            SetAction(EUnitActionState.Dead);
            GameUtility.DelayExcute(1.5f, () =>
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

        public void Hurt()
        {
            if (UnitActionState == EUnitActionState.Run)
            {
                RunHurt();
            }
            else
            {
                SetAction(EUnitActionState.Hurt);
            }

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
            SetAction(EUnitActionState.Recover);
        }
        
        public virtual void ChangeCurHP(int changeHP, bool useDefense, bool addHeroHP, bool changeHPInstantly)
        {
            BattleManager.Instance.ChangeHP(BattleUnitData, changeHP, GamePlayManager.Instance.GamePlayData,
                EHPChangeType.Action, useDefense, addHeroHP, changeHPInstantly);
        }

        public void UpdatePos(Vector3 pos)
        {
            Position = pos;
        }
    }
}
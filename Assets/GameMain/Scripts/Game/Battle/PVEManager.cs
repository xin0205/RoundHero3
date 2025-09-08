using UnityGameFramework.Runtime;
using GameFramework.Event;
using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    public enum EBattleResult
    {
        Success,
        Failed,
        Empty,        
    }
    public interface IBattleTypeManager
    {
        public EBattleState BattleState { get; set; }

        public void StartAction();

        public void ContinueAction();

        public void NextAction();

        public void EndAction();

        //public void UseCard(int cardID, int posIdx);

        public Data_Card GetCard(int cardIdx);

        public Data_EnergyBuff GetEnergyBuff(EUnitCamp unitCamp, int heart, int hp);

        public void PlaceUnitCard(int cardID, int gridPosIdx, EUnitCamp playerUnitCamp);

        public void PlaceProp(int propID, int gridPosIdx, EUnitCamp playerUnitCamp);

        
        public void Destory();

        public void ShowGameOver();

        public EBattleResult CheckGameOver();


    }
    public class PVEManager : Singleton<PVEManager>, IBattleTypeManager
    {
        public Random Random;
        private int randomSeed;

        public EBattleState BattleState { get; set; }

        
        public Data_GamePlay GamePlayData => DataManager.Instance.DataGame.User.CurGamePlayData;
        public Data_Battle BattleData => GamePlayData.BattleData;
        
        public void Init()
        {
            this.randomSeed = GamePlayManager.Instance.GamePlayData.RandomSeed;
            
            Log.Debug(randomSeed);
            
            Random = new Random(randomSeed);
            
            BattleManager.Instance.SetBattleTypeManager(this);

            //GamePlayData.GameMode = EGamMode.PVE;
            
            // var randoms = MathUtility.GetRandomNum(1, 0,
            //     Constant.Game.RandomRange, Random);

        }

        public void Enter()
        {
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Subscribe(RefreshBattleStateEventArgs.EventId, OnRefreshBattleState);

        }

        public void StartBattle()
        {
            
            //GameEntry.UI.OpenUIForm(UIFormId.BattleForm);
        }

        public void SetCurPlayer()
        {
            BattleManager.Instance.SetCurPlayer(EUnitCamp.Player1);

        }

        public void Exit()
        {
            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Unsubscribe(RefreshBattleStateEventArgs.EventId, OnRefreshBattleState);

        }
        
        public void Destory()
        {
  
        }
        
        private void OnRefreshBattleState(object sender, GameEventArgs e)
        {
            var ne = e as RefreshBattleStateEventArgs;
            BattleState = ne.BattleState;

            if (BattleState == EBattleState.EndRound)
            {

                if (CheckGameOver() == EBattleResult.Empty)
                {
                    StartTurn();
                }

                GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                GameEntry.Event.Fire(null, RefreshUnitDataEventArgs.Create());
            }
        }
        
        private async void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            // OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            // if (ne.UIForm.Logic is BattleForm)
            // {
            //     
            //     await BattleAreaManager.Instance.InitArea();
            //     await BattleHeroManager.Instance.GenerateHero();
            //     
            //     await BattleEnemyManager.Instance.GenerateNewEnemies();
            //     
            //     //await BattleThirdUnitManager.Instance.GenerateNewThirdUnits();
            //     
            //     //await BattleGridPropManager.Instance.GenerateGridItems(EGridPropID.Action_AcquireCard_Prop);
            //     BattleState = EBattleState.UseCard;
            //     BattleCardManager.Instance.RoundAcquireCards(true);
            //     
            //     
            //     BattleManager.Instance.RoundStartTrigger();
            //     BattleManager.Instance.Refresh();
            //     
            // }

        }
        
        public async void StartTurn()
        {
            BattleManager.Instance.RoundEndTrigger();
            
            BattleData.Round += 1;
            BattleState = EBattleState.UseCard;
            
            
            BattleManager.Instance.RoundStartTrigger();
            
            await BattleEnemyManager.Instance.GenerateNewEnemies();
            BattleManager.Instance.RefreshAll();
            BattleCardManager.Instance.RoundAcquireCards(false);

            
            
            
            GameEntry.Event.Fire(null, RefreshRoundEventArgs.Create());
            BattleManager.Instance.SwitchActionCamp(false);
            GameUtility.DelayExcute(1f, () =>
            {
                GameEntry.Event.Fire(null, RefreshActionCampEventArgs.Create(false));
                BattleFightManager.Instance.ActionProgress = EActionProgress.EnemyMove;
                BattleFightManager.Instance.EnemyMove();
            });
 
            
            
            // if (GamePlayManager.Instance.GamePlayData.GameMode == EGamMode.PVE)
            // {
            //     GameEntry.Event.Fire(null, SwitchActionCampEventArgs.Create(EUnitCamp.Player1));
            // }
            // else
            // {
            //     if (BattleManager.Instance.CurUnitCamp == EUnitCamp.Player1)
            //     {
            //         GameEntry.Event.Fire(null, SwitchActionCampEventArgs.Create(EUnitCamp.Player2));
            //     }
            //     else if (BattleManager.Instance.CurUnitCamp == EUnitCamp.Player2)
            //     {
            //         GameEntry.Event.Fire(null, SwitchActionCampEventArgs.Create(EUnitCamp.Player1));
            //     }
            // }
        }

        public void StartAction()
        {
            if (BattleState != EBattleState.UseCard)
                return;
            
            BattleFightManager.Instance.AcitonUnitIdx = 0;
            BattleManager.Instance.SetBattleState(EBattleState.ActionExcuting);
            BattleFightManager.Instance.ActionProgress = EActionProgress.RoundStartBuff;
            ContinueAction();
            
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            

        }


        public void ContinueAction()
        {
            if(BattleManager.Instance.BattleState == EBattleState.EndBattle)
                return;
            
            if (BattleFightManager.Instance.ActionProgress == EActionProgress.RoundStartBuff)
            {
                BattleFightManager.Instance.RoundStartBuffTrigger();
            }
            if (BattleFightManager.Instance.ActionProgress == EActionProgress.RoundStartUnit)
            {
                BattleFightManager.Instance.RoundStartUnitTrigger();
            }
            if (BattleFightManager.Instance.ActionProgress == EActionProgress.SoliderAttack)
            {
                BattleFightManager.Instance.SoliderAttack();
            }
            if (BattleFightManager.Instance.ActionProgress == EActionProgress.SoliderActiveAttack)
            {
                BattleFightManager.Instance.SoliderActiveAttack();
            }
            if (BattleFightManager.Instance.ActionProgress == EActionProgress.EnemyMove)
            {
                BattleFightManager.Instance.EnemyMove();
            }
            if (BattleFightManager.Instance.ActionProgress == EActionProgress.EnemyAttack)
            {
                BattleFightManager.Instance.EnemyAttack();
            }
            
            // if (FightManager.Instance.ActionProgress == EActionProgress.ThirdUnitMove)
            // {
            //     FightManager.Instance.ThirdUnitMove();
            // }
            // if (FightManager.Instance.ActionProgress == EActionProgress.ThirdUnitAttack)
            // {
            //     FightManager.Instance.ThirdUnitAttack();
            // }
            
            if (BattleFightManager.Instance.ActionProgress == EActionProgress.RoundEnd)
            {
                BattleFightManager.Instance.RoundEndTrigger();
            }
            if (BattleFightManager.Instance.ActionProgress == EActionProgress.NotifyRoundEnd)
            {

                // GameUtility.DelayExcute(3f, () =>
                // {
                //     GameEntry.Event.Fire(null, RefreshBattleStateEventArgs.Create(EBattleState.EndRound));
                // });

                GameEntry.Event.Fire(null, RefreshBattleStateEventArgs.Create(EBattleState.EndRound));


            }

            if (BattleFightManager.Instance.ActionProgress == EActionProgress.ActionEnd)
            {
                BattleManager.Instance.RefreshEnemyAttackData();
            }
            
        }

        public void NextAction()
        {
            if (BattleFightManager.Instance.ActionProgress == EActionProgress.EnemyMove)
            {
                BattleFightManager.Instance.ActionProgress = EActionProgress.RoundStart;
                BattleManager.Instance.RefreshEnemyAttackData();
                BattleManager.Instance.SwitchActionCamp(true);
                GameUtility.DelayExcute(1.5f, () =>
                {
                    GameEntry.Event.Fire(null, RefreshActionCampEventArgs.Create(true));
                });
                
                //TutorialManager.Instance.SwitchStep(ETutorialStep.UnitHurt);
            }
            else if (BattleFightManager.Instance.ActionProgress == EActionProgress.RoundStartBuff)
            {
                BattleFightManager.Instance.ActionProgress = EActionProgress.RoundStartUnit;
            }
            else if (BattleFightManager.Instance.ActionProgress == EActionProgress.RoundStartUnit)
            {
                BattleFightManager.Instance.ActionProgress = EActionProgress.SoliderAttack;
            }
            else if (BattleFightManager.Instance.ActionProgress == EActionProgress.SoliderAttack)
            {
                BattleFightManager.Instance.ActionProgress = EActionProgress.EnemyAttack;
            }
            else if (BattleFightManager.Instance.ActionProgress == EActionProgress.EnemyAttack)
            {
                BattleFightManager.Instance.ActionProgress = EActionProgress.RoundEnd;

                
            }
            
            // else if (FightManager.Instance.ActionProgress == EActionProgress.EnemyAttack)
            // {
            //     FightManager.Instance.ActionProgress = EActionProgress.ThirdUnitMove;
            //     
            // }
            // else if (FightManager.Instance.ActionProgress == EActionProgress.ThirdUnitMove)
            // {
            //     FightManager.Instance.ActionProgress = EActionProgress.ThirdUnitAttack;
            // }
            //
            // else if (FightManager.Instance.ActionProgress == EActionProgress.ThirdUnitAttack)
            // {
            //     FightManager.Instance.ActionProgress = EActionProgress.RoundEnd;
            // }

            else if (BattleFightManager.Instance.ActionProgress == EActionProgress.RoundEnd)
            {
                BattleFightManager.Instance.ActionProgress = EActionProgress.NotifyRoundEnd;
                
            }
            else
            {
                BattleFightManager.Instance.ActionProgress = EActionProgress.ActionEnd;
                
                
                BattleManager.Instance.RefreshEnemyAttackData();
            }
        }
        
        public void EndAction()
        {
            if(BattleState != EBattleState.UseCard)
                return;
            
            BattleRouteManager.Instance.UnShowEnemyRoutes();
            if (GamePlayData.BlessCount(EBlessID.UnPassCards, BattleManager.Instance.CurUnitCamp) <= 0)
            {
                BattleCardManager.Instance.PassCards();
            }
            
            StartAction();
            
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            BattleManager.Instance.SwitchActionCamp(false);
            GameEntry.Event.Fire(null, RefreshActionCampEventArgs.Create(false));
        }
        
        // public void UseCard(int cardID, int posIdx)
        // {
        //     // var cardData = GetCard(cardID);
        //     //
        //     // if (cardData == null)
        //     //     return;
        //     //
        //     // if (!Input.GetMouseButtonUp(0))
        //     // {
        //     //     return;
        //     // }
        //     //
        //     // if(BattleManager.Instance.BattleState != EBattleState.UseCard)
        //     //     return;
        //     //
        //     //
        //     // if(cardData.UnUse)
        //     //     return;
        //     //
        //     //
        //     // if(!BattleCardManager.Instance.PreUseCard(cardID))
        //     //     return;
        // }
        
        public Data_Card GetCard(int cardIdx)
        {
            return CardManager.Instance.GetCard(cardIdx);

        }
        
        public Data_EnergyBuff GetEnergyBuff(EUnitCamp unitCamp, int heart, int hp)
        {
            return BattleEnergyBuffManager.Instance.GetEnergyBuff(unitCamp, heart, hp);

        }

        public void PlaceUnitCard(int cardID, int gridPosIdx, EUnitCamp playerUnitCamp)
        {
            BattleAreaManager.Instance.PlaceUnitCard(cardID, gridPosIdx, playerUnitCamp);
        }
        
        public void PlaceProp(int propID, int gridPosIdx, EUnitCamp playerUnitCamp)
        {
            BattleAreaManager.Instance.PlaceProp(propID, gridPosIdx, playerUnitCamp);
        }

        public void ShowGameOver()
        {
            if(BattleManager.Instance.BattleState == EBattleState.EndBattle)
                return;
            
            var gameResult = CheckGameOver();
            
            if(gameResult == EBattleResult.Success)
            {
                GameEntry.UI.OpenConfirm(new ConfirmFormParams()
                {
                    IsCloseAvailable = false,
                    IsShowCancel = false,
                    Message = GameEntry.Localization.GetString(Constant.Localization.Message_BattleTestSuccess),
                    OnConfirm = () =>
                    {
                        BattleManager.Instance.EndBattle(gameResult);
                    
                    }
                
                });
                BattleManager.Instance.SetBattleState(EBattleState.EndBattle);
            }

            else if (gameResult == EBattleResult.Failed)
            {
                GameEntry.UI.OpenConfirm(new ConfirmFormParams()
                {
                    IsShowCancel = false,
                    IsCloseAvailable = false,
                    Message = GameEntry.Localization.GetString(Constant.Localization.Message_BattleTestFailed),
                    OnConfirm = () =>
                    {
                        BattleManager.Instance.EndBattle(gameResult);
                    
                    },
 
                });
                BattleManager.Instance.SetBattleState(EBattleState.EndBattle);
            }

            

        }
        
        public EBattleResult CheckGameOver()
        {
            if (BattleManager.Instance.BattleState == EBattleState.EndBattle)
                return EBattleResult.Empty;
            
            var enemyCount = 0;
            foreach (var kv in BattleEnemyManager.Instance.EnemyGenerateData.RoundGenerateUnitCount)
            {
                enemyCount += kv.Value;
            }

            enemyCount += BattleUnitManager.Instance.GetUnitsByCamp(EUnitCamp.Player1, ERelativeCamp.Enemy).Count;
            
            if(enemyCount <= 0)
            {
                return EBattleResult.Success;
            }

            if (HeroManager.Instance.BattleHeroData.CurHP <= 0)
            {
                return EBattleResult.Failed;
            }

            return EBattleResult.Empty;

        }
    }
}
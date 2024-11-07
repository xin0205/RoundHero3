using UnityGameFramework.Runtime;
using GameFramework.Event;
using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    public interface IBattleTypeManager
    {
        public EBattleState BattleState { get; set; }

        public void StartAction();

        public void ContinueAction();

        public void NextAction();

        public void EndAction();

        public void UseCard(int cardID, int posIdx);

        public Data_Card GetCard(int cardID);

        public Data_EnergyBuff GetEnergyBuff(EUnitCamp unitCamp, int heart, int hp);

        public void PlaceUnitCard(int cardID, int gridPosIdx, EUnitCamp playerUnitCamp);

        public void Destory();
    }
    public class PVEManager : Singleton<PVEManager>, IBattleTypeManager
    {
        public Random Random;
        private int randomSeed;

        public EBattleState BattleState { get; set; }

        
        public Data_GamePlay GamePlayData => DataManager.Instance.DataGame.User.CurGamePlayData;
        public Data_Battle BattleData => GamePlayData.BattleData;
        
        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            
            Log.Debug(randomSeed);
            
            Random = new Random(randomSeed);
            
            BattleManager.Instance.SetBattleTypeManager(this);
            
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Subscribe(RefreshBattleStateEventArgs.EventId, OnRefreshBattleState);
            
            
            GamePlayData.GameMode = EGamMode.PVE;
            
            // var randoms = MathUtility.GetRandomNum(1, 0,
            //     Constant.Game.RandomRange, Random);

            BattleManager.Instance.Init(randomSeed);

            BattleCardManager.Instance.InitCards();
            BattleEnemyManager.Instance.Init(Random.Next());
        }

        public void StartBattle()
        {
            GameEntry.UI.OpenUIForm(UIFormId.BattleForm);
        }

        public void SetCurPlayer()
        {
            BattleManager.Instance.SetCurPlayer(EUnitCamp.Player1);

        }
        
        public void Destory()
        {
            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Unsubscribe(RefreshBattleStateEventArgs.EventId, OnRefreshBattleState);
            BattleManager.Instance.Destory();
                
        }
        
        private void OnRefreshBattleState(object sender, GameEventArgs e)
        {
            var ne = e as RefreshBattleStateEventArgs;
            BattleState = ne.BattleState;

            if (BattleState == EBattleState.EndRound)
            {
                
                StartTurn();
                
                BattleEnemyManager.Instance.GenerateNewEnemies();
                
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
        
        public void StartTurn()
        {
            BattleManager.Instance.RoundEndTrigger();
            
            BattleData.Round += 1;
            BattleState = EBattleState.UseCard;
            BattleCardManager.Instance.RoundAcquireCards(false);
            
            BattleManager.Instance.RoundStartTrigger();
            GameEntry.Event.Fire(null, RefreshRoundEventArgs.Create());
            GameUtility.DelayExcute(1.5f, () =>
            {
                GameEntry.Event.Fire(null, RefreshActionCampEventArgs.Create(true));
            });
        }

        public void StartAction()
        {
            if (BattleState != EBattleState.UseCard)
                return;
            
            FightManager.Instance.AcitonUnitIdx = 0;
            BattleManager.Instance.BattleState = EBattleState.ActionExcuting;
            FightManager.Instance.ActionProgress = EActionProgress.RoundStartBuff;
            ContinueAction();
            
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            

        }


        public void ContinueAction()
        {
            if (FightManager.Instance.ActionProgress == EActionProgress.RoundStartBuff)
            {
                FightManager.Instance.RoundStartBuffTrigger();
            }
            if (FightManager.Instance.ActionProgress == EActionProgress.RoundStartUnit)
            {
                FightManager.Instance.RoundStartUnitTrigger();
            }
            
            if (FightManager.Instance.ActionProgress == EActionProgress.SoliderAttack)
            {
                FightManager.Instance.SoliderAttack();
            }
            
            if (FightManager.Instance.ActionProgress == EActionProgress.SoliderActiveAttack)
            {
                FightManager.Instance.SoliderActiveAttack();
            }
            
            if (FightManager.Instance.ActionProgress == EActionProgress.EnemyMove)
            {
                FightManager.Instance.EnemyMove();
            }
            // if (FightManager.Instance.ActionProgress == EActionProgress.EnemyAttack)
            // {
            //     FightManager.Instance.EnemyAttack();
            // }
            
            // if (FightManager.Instance.ActionProgress == EActionProgress.ThirdUnitMove)
            // {
            //     FightManager.Instance.ThirdUnitMove();
            // }
            // if (FightManager.Instance.ActionProgress == EActionProgress.ThirdUnitAttack)
            // {
            //     FightManager.Instance.ThirdUnitAttack();
            // }
            
            if (FightManager.Instance.ActionProgress == EActionProgress.RoundEnd)
            {
                FightManager.Instance.RoundEndTrigger();
            }
            if (FightManager.Instance.ActionProgress == EActionProgress.NotifyRoundEnd)
            {
                GameEntry.Event.Fire(null, RefreshBattleStateEventArgs.Create(EBattleState.EndRound));
            }
            
        }

        public void NextAction()
        {
            if (FightManager.Instance.ActionProgress == EActionProgress.RoundStartBuff)
            {
                FightManager.Instance.ActionProgress = EActionProgress.RoundStartUnit;
            }
            else if (FightManager.Instance.ActionProgress == EActionProgress.RoundStartUnit)
            {
                FightManager.Instance.ActionProgress = EActionProgress.SoliderAttack;
            }
            else if (FightManager.Instance.ActionProgress == EActionProgress.SoliderAttack)
            {
                FightManager.Instance.ActionProgress = EActionProgress.EnemyMove;
            }
            else if (FightManager.Instance.ActionProgress == EActionProgress.EnemyMove)
            {
                FightManager.Instance.ActionProgress = EActionProgress.RoundEnd;
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

            else if (FightManager.Instance.ActionProgress == EActionProgress.RoundEnd)
            {
                FightManager.Instance.ActionProgress = EActionProgress.NotifyRoundEnd;
                
            }
            else
            {
                FightManager.Instance.ActionProgress = EActionProgress.ActionEnd;
                BattleManager.Instance.Refresh();
            }
        }
        
        public void EndAction()
        {
            if(BattleState != EBattleState.UseCard)
                return;
            
            if (GamePlayData.BlessCount(EBlessID.UnPassCards, BattleManager.Instance.CurUnitCamp) <= 0)
            {
                BattleCardManager.Instance.PassCards();
            }
            
            StartAction();

            
            
            
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            GameEntry.Event.Fire(null, RefreshActionCampEventArgs.Create(false));
        }
        
        public void UseCard(int cardID, int posIdx)
        {
            var cardData = GetCard(cardID);

            if (cardData == null)
                return;
            
            if (!Input.GetMouseButtonUp(0))
            {
                return;
            }
            
            if(BattleManager.Instance.BattleState != EBattleState.UseCard)
                return;

            
            if(cardData.UnUse)
                return;

            
            if(!BattleCardManager.Instance.PreUseCard(cardID))
                return;
        }
        
        public Data_Card GetCard(int cardID)
        {
            return CardManager.Instance.GetCard(cardID);

        }
        
        public Data_EnergyBuff GetEnergyBuff(EUnitCamp unitCamp, int heart, int hp)
        {
            return BattleEnergyBuffManager.Instance.GetEnergyBuff(unitCamp, heart, hp);

        }

        public void PlaceUnitCard(int cardID, int gridPosIdx, EUnitCamp playerUnitCamp)
        {
            BattleAreaManager.Instance.PlaceUnitCard(cardID, gridPosIdx, playerUnitCamp);
        }
    }
}
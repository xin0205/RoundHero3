using System;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Transporting;
using GameFramework;
using GameFramework.Event;
using UGFExtensions.Await;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace RoundHero
{
    public class ProcedureStart : ProcedureBase
    {
        //private bool InitSuccess = false;
        public SceneEntity StartEntity;
        public SceneEntity StartSelectEntity;
        
        
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Subscribe(GamePlayStartGameEventArgs.EventId, OnGamePlayStartGame);
            GameEntry.Event.Subscribe(GamePlayContinueGameEventArgs.EventId, OnGamePlayContinueGame);
            //GameEntry.Sound.PlayMusic(0);

            //InitSuccess = false;

            // DRScene drScene = GameEntry.DataTable.GetScene(2);
            // GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(drScene.AssetName), Constant.AssetPriority.SceneAsset);
            
            
        }

        protected UIForm StartForm;
        
        public async void Start()
        {
            StartEntity = await GameEntry.Entity.ShowSceneEntityAsync("Start");
            StartForm = await GameEntry.UI.OpenUIFormAsync(UIFormId.StartForm, this);
        }

        public void CloseStartForm()
        {
            if (StartForm != null && GameEntry.UI.IsValidUIForm(StartForm))
            {
                (StartForm.Logic as StartForm).CloseForm();
            }

            
        }

        
        public void StartSelect()
        {
            StartGame();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            // if (InitSuccess)
            // {
            //     BattleManager.Instance.Update();
            // }

        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Sound.StopMusic();

            base.OnLeave(procedureOwner, isShutdown);
            GameEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Unsubscribe(GamePlayStartGameEventArgs.EventId, OnGamePlayStartGame);
            GameEntry.Event.Unsubscribe(GamePlayContinueGameEventArgs.EventId, OnGamePlayContinueGame);
            // DRScene drScene = GameEntry.DataTable.GetScene(2);
            // GameEntry.Scene.UnloadScene(AssetUtility.GetSceneAsset(drScene.AssetName));
        }

        
        
        public async void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            // StartEntity = await GameEntry.Entity.ShowSceneEntityAsync("Start");
            // GameEntry.UI.OpenUIForm(UIFormId.StartForm, this);

        }

        public void OnGamePlayStartGame(object sender, GameEventArgs e)
        {
            var ne = e as GamePlayStartGameEventArgs;

            if (ne.GamePlayInitData.GameMode == EGamMode.PVE)
            {
                GameManager.Instance.IsStartBattle = true;
                ChangeState<ProcedureGamePlay>(procedureOwner);

                var gamePlayProcedure = procedureOwner.CurrentState as ProcedureGamePlay;
                //var random = new Random(GamePlayManager.Instance.GamePlayData.RandomSeed);
                gamePlayProcedure.StartBattleMode(GamePlayManager.Instance.GamePlayData.RandomSeed);
     
                DataManager.Instance.Save();
            }
            else if (ne.GamePlayInitData.GameMode == EGamMode.PVP)
            {
                //ChangeState<ProcedureBattle>(procedureOwner);
            }

        }

        public void OnGamePlayContinueGame(object sender, GameEventArgs e)
        {

            if (BattleManager.Instance.BattleData.IsNewBattle &&
                GamePlayManager.Instance.GamePlayData.BattleModeProduce.BattleModeStage == BattleModeStage.Battle)
            {
                GameManager.Instance.IsStartBattle = true;
                ChangeState<ProcedureGamePlay>(procedureOwner);
                
                var gamePlayProcedure = procedureOwner.CurrentState as ProcedureGamePlay;
                GamePlayManager.Instance.GamePlayData.RandomSeed = UnityEngine.Random.Range(0, Constant.Game.RandomRange);
                gamePlayProcedure.StartBattleMode(GamePlayManager.Instance.GamePlayData.RandomSeed);
                BattleManager.Instance.BattleData.IsNewBattle = false;
            }
            else
            {
                GameManager.Instance.IsStartBattle = false;
                ContinueGame();
            }
            
            DataManager.Instance.Save();

        }
        
        
        private async void StartGame()
        {
            StartSelectEntity = await GameEntry.Entity.ShowSceneEntityAsync("StartSelect");
            GameEntry.UI.OpenUIForm(UIFormId.StartSelectForm, this);
        }

        public void ContinueGame()
        {
            var random = new System.Random(GamePlayManager.Instance.GamePlayData.RandomSeed);
            GamePlayManager.Instance.Continue(random.Next());
            

            if (GamePlayManager.Instance.GamePlayData.GameMode == EGamMode.PVE)
            {
                if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.Tutorial)
                {
                    BattleManager.Instance.Continue(random.Next());
                    PVEManager.Instance.Continue(random.Next());
                    ContinueBattle();
                }
                else if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.BattleMode)
                {
                    if (GamePlayManager.Instance.GamePlayData.BattleModeProduce.BattleModeStage == BattleModeStage.Battle)
                    {
                        BattleManager.Instance.Continue(random.Next());
                        PVEManager.Instance.Continue(random.Next());
                        ContinueBattle();
                    }
                    else if (GamePlayManager.Instance.GamePlayData.BattleModeProduce.BattleModeStage ==
                             BattleModeStage.Reward)
                    {
                        BattleModeReward();
                    }


                }
                else if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.Test)
                {
                    BattleManager.Instance.Continue(random.Next());
                    PVEManager.Instance.Continue(random.Next());
                    ContinueBattle();
                }
            }
            
            
            
        }
        
        public void ContinueBattle()
        {
            
            ChangeState<ProcedureGamePlay>(procedureOwner);
            var gamePlayProcedure = procedureOwner.CurrentState as ProcedureGamePlay;

            gamePlayProcedure.ContinueBattleMode();

            
            
        }

        public void RestartGameTest()
        {
            Reset(EPVEType.Test);
            StartSelect();
        }

        public void Reset(EPVEType pveType)
        {
            DataManager.Instance.DataGame.Clear(pveType);
            DataManager.Instance.Save();
        }
        
        public async void BattleModeReward()
        {
            

            
            GameEntry.UI.OpenUIForm(UIFormId.BattleModeRewardForm, this);
        }
        
        public void RestartBattleMode()
        {
            
            GameEntry.UI.OpenUIForm(UIFormId.SelectDifficultyForm, this);
            
            

        }
        
        public void ContinueBattleMode()
        {
            CloseStartForm();
            
            //GamePlayManager.Instance.GamePlayData.PVEType = EPVEType.Battle;
            //GamePlayManager.Instance.GamePlayData.PVEType
            DataManager.Instance.DataGame.User.SetCurGamePlayData(EPVEType.BattleMode);
            
            GameEntry.Event.Fire(null,
                GamePlayContinueGameEventArgs.Create());
            
            

        }
        
    }

}
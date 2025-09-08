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
            GameEntry.Event.Subscribe(GamePlayInitGameEventArgs.EventId, OnGamePlayInitGame);
            GameEntry.Event.Subscribe(GamePlayStartGameEventArgs.EventId, OnGamePlayStartGame);
            GameEntry.Sound.PlayMusic(0);

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
            GameEntry.Event.Unsubscribe(GamePlayInitGameEventArgs.EventId, OnGamePlayInitGame);
            GameEntry.Event.Unsubscribe(GamePlayStartGameEventArgs.EventId, OnGamePlayStartGame);
            // DRScene drScene = GameEntry.DataTable.GetScene(2);
            // GameEntry.Scene.UnloadScene(AssetUtility.GetSceneAsset(drScene.AssetName));
        }

        
        
        public async void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            // StartEntity = await GameEntry.Entity.ShowSceneEntityAsync("Start");
            // GameEntry.UI.OpenUIForm(UIFormId.StartForm, this);

        }

        public void OnGamePlayInitGame(object sender, GameEventArgs e)
        {
            var ne = e as GamePlayInitGameEventArgs;

            if (ne.GamePlayInitData.GameMode == EGamMode.PVE)
            {
                // GamePlayManager.Instance.GamePlayData.RandomSeed = ne.GamePlayInitData.RandomSeed;
                //
                // GamePlayManager.Instance.GamePlayData.GameMode = EGamMode.PVE;
                // GamePlayManager.Instance.GamePlayData.BattleData.GameDifficulty = ne.GamePlayInitData.GameDifficulty;
                //
                // GamePlayManager.Instance.GamePlayData.PVEType = EPVEType.Battle;
                // GamePlayManager.Instance.GamePlayData.BattleModeProduce.Session = 0;
                // GamePlayManager.Instance.GamePlayData.BattleModeProduce.BattleModeStage = BattleModeStage.Battle;
                
                GamePlayManager.Instance.Start();
                ContinueGame();
                DataManager.Instance.Save();
            }
            else if (ne.GamePlayInitData.GameMode == EGamMode.PVP)
            {
                //ChangeState<ProcedureBattle>(procedureOwner);
            }

        }

        public void OnGamePlayStartGame(object sender, GameEventArgs e)
        {
            var ne = e as GamePlayStartGameEventArgs;

            ContinueGame();
            DataManager.Instance.Save();

        }
        
        
        private async void StartGame()
        {
            StartSelectEntity = await GameEntry.Entity.ShowSceneEntityAsync("StartSelect");
            GameEntry.UI.OpenUIForm(UIFormId.StartSelectForm, this);
        }

        public void ContinueGame()
        {

            if (GamePlayManager.Instance.GamePlayData.GameMode == EGamMode.PVE)
            {
                
                
                if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.Battle)
                {
                    if (GamePlayManager.Instance.GamePlayData.BattleModeProduce.BattleModeStage == BattleModeStage.Battle)
                    {
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
                    ContinueBattle();
                }
            }
            
            
            
        }
        
        public void ContinueBattle()
        {
            
            
            PVEManager.Instance.Enter();
            PVEManager.Instance.Init();
            GamePlayManager.Instance.Contitnue();
            
            var random = new Random(GamePlayManager.Instance.GamePlayData.RandomSeed);
            ChangeState<ProcedureGamePlay>(procedureOwner);
            var gamePlayProcedure = procedureOwner.CurrentState as ProcedureGamePlay;
            gamePlayProcedure.StartBattle(random.Next());
            
            
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
            GamePlayManager.Instance.GamePlayData.BattleModeProduce.BattleModeStage = BattleModeStage.Reward;
            DataManager.Instance.Save();

            
            GameEntry.UI.OpenUIForm(UIFormId.BattleModeRewardForm, this);
        }
        
        public void RestartBattleMode()
        {
            
            GameEntry.UI.OpenUIForm(UIFormId.SelectDifficultyForm, this);
            
            

        }
        
        public void ContinueBattleMode()
        {
            CloseStartForm();
            GamePlayManager.Instance.GamePlayData.PVEType = EPVEType.Battle;
            DataManager.Instance.DataGame.User.SetCurGamePlayData(GamePlayManager.Instance.GamePlayData.PVEType);
            
            GameEntry.Event.Fire(null,
                GamePlayStartGameEventArgs.Create());
            
            

        }
        
    }

}
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Transporting;
using GameFramework;
using GameFramework.Event;

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

            GameEntry.Sound.PlayMusic(0);

            //InitSuccess = false;

            // DRScene drScene = GameEntry.DataTable.GetScene(2);
            // GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(drScene.AssetName), Constant.AssetPriority.SceneAsset);
            
            
        }

        public async void Start()
        {
            StartEntity = await GameEntry.Entity.ShowSceneEntityAsync("Start");
            GameEntry.UI.OpenUIForm(UIFormId.StartForm, this);
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
            // var data = new ();
            // data.SetValue(ne.GamVarGamePlayInitDataePlayInitData);
            //
            // procedureOwner.SetData("GamePlayInitData", data);
            
            
            
            if (ne.GamePlayInitData.GameMode == EGamMode.PVE)
            {
                GamePlayManager.Instance.GamePlayData.RandomSeed = ne.GamePlayInitData.RandomSeed;
                
                GamePlayManager.Instance.GamePlayData.GameMode = EGamMode.PVE;
                GamePlayManager.Instance.GamePlayData.BattleData.GameDifficulty = ne.GamePlayInitData.GameDifficulty;
                GamePlayManager.Instance.Start();
                ContinueGame();
                DataManager.Instance.Save();
            }
            else if (ne.GamePlayInitData.GameMode == EGamMode.PVP)
            {
                //ChangeState<ProcedureBattle>(procedureOwner);
            }

        }

        private async void StartGame()
        {
            StartSelectEntity = await GameEntry.Entity.ShowSceneEntityAsync("StartSelect");
            GameEntry.UI.OpenUIForm(UIFormId.StartSelectForm, this);
        }

        public void ContinueGame()
        {
            PVEManager.Instance.Enter();
            PVEManager.Instance.Init();
            GamePlayManager.Instance.Contitnue();
            
            ChangeState<ProcedureGamePlay>(procedureOwner);
                
            var gamePlayProcedure = procedureOwner.CurrentState as ProcedureGamePlay;
            gamePlayProcedure.ShowMap();
        }

        public void RestartGame()
        {
            DataManager.Instance.DataGame.Clear();
            DataManager.Instance.Save();
            StartSelect();
        }

        
    }

}
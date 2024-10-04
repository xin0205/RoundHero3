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
        private bool InitSuccess = false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Subscribe(GamePlayInitGameEventArgs.EventId, OnGamePlayInitGame);

            GameEntry.Sound.PlayMusic(0);

            InitSuccess = false;

            // DRScene drScene = GameEntry.DataTable.GetScene(2);
            // GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(drScene.AssetName), Constant.AssetPriority.SceneAsset);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (InitSuccess)
            {
                BattleManager.Instance.Update();
            }

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

        public SceneEntity StartEntity;
        public SceneEntity StartSelectEntity;
        public async void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            StartEntity = await GameEntry.Entity.ShowSceneEntityAsync("Start");
            GameEntry.UI.OpenUIForm(UIFormId.StartForm, this);

        }

        public void OnGamePlayInitGame(object sender, GameEventArgs e)
        {
            var ne = e as GamePlayInitGameEventArgs;
            var data = new VarGamePlayInitData();
            data.SetValue(ne.GamePlayInitData);
            
            procedureOwner.SetData("GamePlayInitData", data);
            GamePlayManager.Instance.Init(ne.GamePlayInitData);
            
            if (data.Value.GameMode == EGamMode.PVE)
            {
                ChangeState<ProcedureGamePlay>(procedureOwner);
            }
            else if (data.Value.GameMode == EGamMode.PVP)
            {
                ChangeState<ProcedureBattle>(procedureOwner);
            }
            
            
            

        }
    }

}
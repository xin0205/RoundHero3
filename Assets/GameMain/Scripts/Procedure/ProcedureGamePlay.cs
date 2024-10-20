using System;
using GameFramework;
using GameFramework.Event;
using UGFExtensions.Await;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace RoundHero
{
    public class ProcedureGamePlay : ProcedureBase
    {

        private bool IsStartBattle = false;
        public SceneEntity MapEntity;
        private PlayerInfoForm playerInfoForm;
        private MapForm mapForm;
        
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            IsStartBattle = false;
            
            //GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            
            GameEntry.Sound.PlayMusic(0);

        }

        public async void ShowMap()
        {
            GamePlayManager.Instance.SetProcedureGamePlay(this);
            var mapFormResult = await GameEntry.UI.OpenUIFormAsync(UIFormId.MapForm, this);
            mapForm = mapFormResult.Logic as MapForm;
            var playerInfoFormResult = await GameEntry.UI.OpenUIFormAsync(UIFormId.PlayerInfoForm, this);
            playerInfoForm = playerInfoFormResult.Logic as PlayerInfoForm;
                
            // var initData = procedureOwner.GetData<VarGamePlayInitData>("GamePlayInitData");
            // PVEManager.Instance.Init(initData.Value.RandomSeed, initData.Value.EnemyType);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            // if (IsStartBattle)
            // {
            //     BattleManager.Instance.Update();
            // }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Sound.StopMusic();
            base.OnLeave(procedureOwner, isShutdown);
            //GameEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
        }
        
        public void Back()
        {
            GamePlayManager.Instance.Destory(EGamMode.PVE);
            
            GameEntry.Entity.HideEntity(MapEntity);
            GameEntry.UI.CloseUIForm(playerInfoForm);
            GameEntry.UI.CloseUIForm(mapForm);
        }

        public void BackToStartSelect()
        {
            Back();
            
            ChangeState<ProcedureStart>(procedureOwner);
            var gamePlayProcedure = procedureOwner.CurrentState as ProcedureStart;
            gamePlayProcedure.StartSelect();
        }
        
        public void BackToStart()
        {
            Back();
            
            ChangeState<ProcedureStart>(procedureOwner);
            var gamePlayProcedure = procedureOwner.CurrentState as ProcedureStart;
            gamePlayProcedure.Start();
        }


        public void StartBattle()
        {
            // DRScene drScene = GameEntry.DataTable.GetScene(1);
            // GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(drScene.AssetName), Constant.AssetPriority.SceneAsset);
            
            ChangeState<ProcedureBattle>(procedureOwner);
        }
        
        
        // public void EndBattle()
        // {
        //     BattleManager.Instance.BattleTypeManager.Destory();
        //     DRScene drScene = GameEntry.DataTable.GetScene(1);
        //     GameEntry.Scene.UnloadScene(AssetUtility.GetSceneAsset(drScene.AssetName));
        //     ChangeState<ProcedureGamePlay>(procedureOwner);
        //}
        
        // public void OnLoadSceneSuccess(object sender, GameEventArgs e)
        // {
        //     AreaController.Instance.RefreshCameraPlane();
        //     var initData = procedureOwner.GetData<VarGamePlayInitData>("GamePlayInitData");
        //     
        //     if (initData.Value.GameMode == EGamMode.PVP)
        //     {
        //         //PVPManager.Instance.Init(initData.Value.RandomSeed, initData.Value.PlayerDatas);
        //     }
        //     else if (initData.Value.GameMode == EGamMode.PVE)
        //     {
        //         PVEManager.Instance.Init(initData.Value.RandomSeed, initData.Value.EnemyType);
        //     }
        //     
        //     
        //     IsStartBattle = true;
        //
        // }

        public void StartBattle(bool isStartBattle)
        {
            IsStartBattle = isStartBattle;
        }

    }


}
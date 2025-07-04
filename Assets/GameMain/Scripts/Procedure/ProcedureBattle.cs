﻿using System.Collections.Generic;
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
    

    public class ProcedureBattle : ProcedureBase
    {
        private bool InitSuccess = false;
        private BattleForm battleForm;
        private PlayerInfoForm playerInfoForm;
        
        protected override async void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);

            //GameEntry.Sound.PlayMusic(0);

            BattleManager.Instance.ProcedureBattle = this;
            InitSuccess = false;

            var sceneName = "Scene" + BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx;
            //DRScene drScene = GameEntry.DataTable.GetScene(1);
            GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(sceneName), Constant.AssetPriority.SceneAsset);
            
            
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
            
        }

        public async void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            AreaController.Instance.RefreshCameraPlane();
            // var initData = procedureOwner.GetData<VarGamePlayInitData>("GamePlayInitData");
            
            // if (initData.Value.GameMode == EGamMode.PVP)
            // {
            //     PVPManager.Instance.Init(initData.Value.RandomSeed, initData.Value.PlayerDatas);
            // }
            // else if (initData.Value.GameMode == EGamMode.PVE)
            // {
            //     PVEManager.Instance.Init(initData.Value.RandomSeed, initData.Value.EnemyType);
            // }
            
            
            InitSuccess = true;
            
            // var playerInfoFormTask = await GameEntry.UI.OpenUIFormAsync(UIFormId.PlayerInfoForm, this);
            // playerInfoForm = playerInfoFormTask.Logic as PlayerInfoForm;
            
            if (GamePlayManager.Instance.GamePlayData.IsTutorialBattle)
            {
                await GameEntry.UI.OpenUIFormAsync(UIFormId.TutorialForm, this);
            }
            
            var battleFormTask = await GameEntry.UI.OpenUIFormAsync(UIFormId.BattleForm, this);
            battleForm = battleFormTask.Logic as BattleForm;

            
            
            //await HeroManager.Instance.GenerateHero();
            await BattleAreaManager.Instance.InitArea();
            await BattleCoreManager.Instance.GenerateCores();
            await BattleEnemyManager.Instance.GenerateNewEnemies();
            
            PVEManager.Instance.BattleState = EBattleState.UseCard;
            BattleCardManager.Instance.RoundAcquireCards(true);
                
            BattleAreaManager.Instance.RefreshObstacles();
            BattleManager.Instance.RoundStartTrigger();
            BattleManager.Instance.RefreshAll();
            

            GameEntry.Event.Fire(null, RefreshRoundEventArgs.Create());
            BattleManager.Instance.SwitchActionCamp(false);
            GameUtility.DelayExcute(1f, () =>
            {
                GameEntry.Event.Fire(null, RefreshActionCampEventArgs.Create(false));

                BattleFightManager.Instance.ActionProgress = EActionProgress.EnemyMove;
                BattleFightManager.Instance.EnemyMove();
            });

            GamePlayManager.Instance.GamePlayData.LastActionBattleData = null;

            GamePlayManager.Instance.GamePlayData.LastActionPlayerData = null;



        }
        
        public void EndBattle()
        {
            HeroManager.Instance.BattleHeroData.CurHP = HeroManager.Instance.BattleHeroData.MaxHP; 
            
            //GameEntry.UI.CloseUIForm(playerInfoForm);
            GameEntry.UI.CloseUIForm(battleForm);
            BattleManager.Instance.Destory();
            
            //BattleManager.Instance.BattleTypeManager.Destory();
            
            //DRScene drScene = GameEntry.DataTable.GetScene(1);
            var sceneName = "Scene" + BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx;
            GameEntry.Scene.UnloadScene(AssetUtility.GetSceneAsset(sceneName));
            ChangeState<ProcedureGamePlay>(procedureOwner);
            
            var procedureGamePlay = procedureOwner.CurrentState as ProcedureGamePlay;
            procedureGamePlay.ShowMap();
        }
        
        public void EndBattleTest()
        {
            if (GamePlayManager.Instance.GamePlayData.GameMode == EGamMode.PVE)
            {
                PVEManager.Instance.Exit();
            }
            //GameEntry.UI.CloseUIForm(playerInfoForm);
            GameEntry.UI.CloseUIForm(battleForm);
            BattleManager.Instance.Destory();

            var sceneName = "Scene" + BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx;
            GameEntry.Scene.UnloadScene(AssetUtility.GetSceneAsset(sceneName));
            ChangeState<ProcedureStart>(procedureOwner);
            
            var procedureStart = procedureOwner.CurrentState as ProcedureStart;
            if (TutorialManager.Instance.IsTutorial())
            {
                procedureStart.Start();
            }
            else
            {
                procedureStart.RestartGame();
            }
            
        }

    }
}
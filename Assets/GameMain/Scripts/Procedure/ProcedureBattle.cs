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
            

            InitSuccess = false;

            var sceneName = "Scene" + BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx;
            //DRScene drScene = GameEntry.DataTable.GetScene(1);
            GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(sceneName), Constant.AssetPriority.SceneAsset);
            
            var playerInfoFormTask = await GameEntry.UI.OpenUIFormAsync(UIFormId.PlayerInfoForm, this);
            playerInfoForm = playerInfoFormTask.Logic as PlayerInfoForm;
            
            var battleFormTask = await GameEntry.UI.OpenUIFormAsync(UIFormId.BattleForm, this);
            battleForm = battleFormTask.Logic as BattleForm;
            
            await BattleAreaManager.Instance.InitArea();
            await HeroManager.Instance.GenerateHero();
                
            await BattleEnemyManager.Instance.GenerateNewEnemies();
                

            PVEManager.Instance.BattleState = EBattleState.UseCard;
            BattleCardManager.Instance.RoundAcquireCards(true);
                
            BattleAreaManager.Instance.RefreshObstacles();    
            BattleManager.Instance.RoundStartTrigger();
            BattleManager.Instance.Refresh();
            
            GameEntry.Event.Fire(null, RefreshRoundEventArgs.Create());
            GameUtility.DelayExcute(1.5f, () =>
            {
                GameEntry.Event.Fire(null, RefreshActionCampEventArgs.Create(true));
            });
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

        public void OnLoadSceneSuccess(object sender, GameEventArgs e)
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

        }
        
        public void EndBattle()
        {
            HeroManager.Instance.BattleHeroData.CurHP = HeroManager.Instance.BattleHeroData.MaxHP; 
            
            GameEntry.UI.CloseUIForm(playerInfoForm);
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

    }
}
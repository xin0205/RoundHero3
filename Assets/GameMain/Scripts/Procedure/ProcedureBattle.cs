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
    

    public class ProcedureBattle : ProcedureBase
    {
        private bool InitSuccess = false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);

            GameEntry.Sound.PlayMusic(0);

            InitSuccess = false;

            DRScene drScene = GameEntry.DataTable.GetScene(1);
            GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(drScene.AssetName), Constant.AssetPriority.SceneAsset);
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
        

    }
}
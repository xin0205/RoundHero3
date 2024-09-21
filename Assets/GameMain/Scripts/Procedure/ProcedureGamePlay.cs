using System;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace RoundHero
{
    public class ProcedureGamePlay : ProcedureBase
    {

        private bool InitSuccess = false;
        
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            InitSuccess = false;
            
            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            
            GameEntry.Sound.PlayMusic(0);

            GamePlayManager.Instance.SetProcedureGamePlay(this);
            GameEntry.UI.OpenUIForm(UIFormId.MapForm, this);

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


        public void StartBattle()
        {
            DRScene drScene = GameEntry.DataTable.GetScene(1);
            GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(drScene.AssetName), Constant.AssetPriority.SceneAsset);
            
            //ChangeState<ProcedureBattle>(procedureOwner);
        }
        
        
        public void EndBattle()
        {
            BattleManager.Instance.BattleTypeManager.Destory();
            DRScene drScene = GameEntry.DataTable.GetScene(1);
            GameEntry.Scene.UnloadScene(AssetUtility.GetSceneAsset(drScene.AssetName));
            ChangeState<ProcedureGamePlay>(procedureOwner);
        }
        
        public void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            AreaController.Instance.RefreshCameraPlane();
            var initData = procedureOwner.GetData<VarGamePlayInitData>("GamePlayInitData");
            
            if (initData.Value.GameMode == EGamMode.PVP)
            {
                //PVPManager.Instance.Init(initData.Value.RandomSeed, initData.Value.PlayerDatas);
            }
            else if (initData.Value.GameMode == EGamMode.PVE)
            {
                PVEManager.Instance.Init(initData.Value.RandomSeed, initData.Value.EnemyType);
            }
            
            
            InitSuccess = true;

        }

    }


}
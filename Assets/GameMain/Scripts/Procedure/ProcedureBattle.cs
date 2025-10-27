
using GameFramework.Event;
using UGFExtensions.Await;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace RoundHero
{
    

    public class ProcedureBattle : ProcedureBase
    {
        private bool InitSuccess = false;
        private BattleForm battleForm;
        private TutorialForm tutorialForm;
        //private PlayerInfoForm playerInfoForm;
        
        protected override async void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);

            //GameEntry.Sound.PlayMusic(0);

            BattleManager.Instance.ProcedureBattle = this;
            InitSuccess = false;

            var sceneName = "Scene0"; //+ BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx;
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
            InitSuccess = true;
            AreaController.Instance.RefreshCameraPlane();

            if (GamePlayManager.Instance.GamePlayData.IsTutorialBattle)
            {
                var tutorialFormTask = await GameEntry.UI.OpenUIFormAsync(UIFormId.TutorialForm, this);
                tutorialForm = tutorialFormTask.Logic as TutorialForm;
            }
            
            var battleFormTask = await GameEntry.UI.OpenUIFormAsync(UIFormId.BattleForm, this);
            battleForm = battleFormTask.Logic as BattleForm;

            if (GameManager.Instance.IsStartBattle)
            {
                await BattleAreaManager.Instance.GenerateArea();
                await BattleCoreManager.Instance.GenerateCores();
                await BattleEnemyManager.Instance.GenerateEnemies();
            
                PVEManager.Instance.BattleState = EBattleState.UseCard;
            
                
                BattleAreaManager.Instance.RefreshObstacles();
                BattleManager.Instance.RoundStartTrigger();
                BattleManager.Instance.RefreshAll();
                BattleCardManager.Instance.RoundAcquireCards(true);

                GameEntry.Event.Fire(null, RefreshRoundEventArgs.Create());
                BattleManager.Instance.SwitchActionCamp(false);
                GameUtility.DelayExcute(1f, () =>
                {
                    GameEntry.Event.Fire(null, RefreshActionCampEventArgs.Create(false));

                    BattleFightManager.Instance.PreRoundStartUnitTrigger();
                    BattleManager.Instance.ContinueAction();
                });
                DataManager.Instance.Save();

            }
            else
            {
                var battleData = GamePlayManager.Instance.GamePlayData.BattleData.Copy();
                await GamePlayManager.Instance.ShowBattleData(battleData);
                //GameEntry.Event.Fire(null, RefreshActionCampEventArgs.Create(true));
                if (battleData.CurUnitCamp == EUnitCamp.Player1)
                {
                    GameEntry.Event.Fire(null, RefreshActionCampEventArgs.Create(true));
                }
                else
                {
                    BattleManager.Instance.RefreshAll();
                    GameUtility.DelayExcute(1f, () =>
                    {
                        GameEntry.Event.Fire(null, RefreshActionCampEventArgs.Create(false));

                        BattleFightManager.Instance.PreRoundStartUnitTrigger();
                        BattleManager.Instance.ContinueAction();
                    });
                    
                }
                
                
            }

            

        }
        
        // public void EndBattle()
        // {
        //     HeroManager.Instance.BattleHeroData.CurHP = HeroManager.Instance.BattleHeroData.MaxHP; 
        //     
        //     //GameEntry.UI.CloseUIForm(playerInfoForm);
        //     GameEntry.UI.CloseUIForm(battleForm);
        //     BattleManager.Instance.Destory();
        //     
        //     //BattleManager.Instance.BattleTypeManager.Destory();
        //     
        //     //DRScene drScene = GameEntry.DataTable.GetScene(1);
        //     var sceneName = "Scene" + BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx;
        //     GameEntry.Scene.UnloadScene(AssetUtility.GetSceneAsset(sceneName));
        //     ChangeState<ProcedureGamePlay>(procedureOwner);
        //     
        //     var procedureGamePlay = procedureOwner.CurrentState as ProcedureGamePlay;
        //     procedureGamePlay.ShowMap();
        // }
        
        public void EndBattle(EBattleResult battleResult)
        {
            if (GamePlayManager.Instance.GamePlayData.GameMode == EGamMode.PVE)
            {
                PVEManager.Instance.Exit();
            }
            //GameEntry.UI.CloseUIForm(playerInfoForm);
            GameEntry.UI.CloseUIForm(battleForm);
            BattleManager.Instance.Destory();
            //BattleManager.Instance.SetBattleState(EBattleState.EndBattle);
            var sceneName = "Scene0";// + BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx;
            GameEntry.Scene.UnloadScene(AssetUtility.GetSceneAsset(sceneName));
            ChangeState<ProcedureStart>(procedureOwner);
            
            var procedureStart = procedureOwner.CurrentState as ProcedureStart;
            if (TutorialManager.Instance.IsTutorial())
            {
                GamePlayManager.Instance.GamePlayData.IsTutorialBattle = false;
                BattleManager.Instance.TutorialStep = ETutorialStep.Start;
                GameEntry.UI.CloseUIForm(tutorialForm);
                procedureStart.Start();
            }
            else
            {
                procedureStart.RestartGameTest();
            }
            
        }
        
        
        
        public void EndBattleMode(EBattleResult battleResult)
        {
            //var gameOver = BattleManager.Instance.CheckGameOver();
            if (GamePlayManager.Instance.GamePlayData.GameMode == EGamMode.PVE)
            {
                PVEManager.Instance.Exit();
            }
            
            //GameEntry.UI.CloseUIForm(playerInfoForm);
            GameEntry.UI.CloseUIForm(battleForm);
            BattleManager.Instance.Destory();

            var sceneName = "Scene0";// + BattleMapManager.Instance.MapData.CurMapStageIdx.MapIdx;
            GameEntry.Scene.UnloadScene(AssetUtility.GetSceneAsset(sceneName));
            ChangeState<ProcedureStart>(procedureOwner);
            
            var procedureStart = procedureOwner.CurrentState as ProcedureStart;
            
            
            if (TutorialManager.Instance.IsTutorial())
            {
                GameEntry.UI.CloseUIForm(tutorialForm);
                procedureStart.Start();
            }
            else if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.BattleMode)
            {
                if (battleResult == EBattleResult.Failed)
                {
                    procedureStart.Start();
                }
                else if(battleResult == EBattleResult.Success)
                {
                    if (GamePlayManager.Instance.GamePlayData.BattleModeProduce.Session + 1 ==
                        Constant.BattleMode.MaxBattleCount)
                    {
                        GamePlayManager.Instance.GamePlayData.IsGamePlaying = false;
                        procedureStart.Start();
                    }
                    else
                    {
                        
                        GamePlayManager.Instance.GamePlayData.BattleModeProduce.RewardRandomSeed =
                            BattleModeManager.Instance.GetRandomSeed();
                        procedureStart.BattleModeReward();
                    }
                    
                }

                else
                {
                    procedureStart.Start();
                }
            }
            else if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.Test)
            {
                procedureStart.Start();
            }

        }

    }
}
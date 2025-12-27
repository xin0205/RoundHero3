
using CatJson;
using HeathenEngineering.SteamworksIntegration.API;
using Steamworks;
using UGFExtensions.Await;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using Input = UnityEngine.Input;

namespace RoundHero
{
    public class StartForm : UGuiForm
    {
        private ProcedureStart procedureStart;

        // [SerializeField] private GameObject startGame;
        // [SerializeField] private GameObject continueGame;
        // [SerializeField] private GameObject restartGame;
        
        [SerializeField] private GameObject battleMode_startGame;
        [SerializeField] private GameObject battleMode_continueGame;
        [SerializeField] private GameObject battleMode_restartGame;

        [SerializeField] private Text userName;
        [SerializeField] private Text version;
        
        [SerializeField] private GameObject testGO;
        protected override async void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            procedureStart = (ProcedureStart)userData;
            if (procedureStart == null)
            {
                Log.Warning("ProcedureStart is null.");
                return;
            }

            if (GlobalManager.Instance.SteamInitialized)
            {
                userName.text = User.Client.Id.Nickname;
            }
            
            // var isStartGame = DataManager.Instance.DataGame.User.CurGamePlayData.PlayerData.BattleHero.HeroID !=
            //                   EHeroID.Empty;
            //
            // startGame.SetActive(!isStartGame);
            // continueGame.SetActive(isStartGame);
            // restartGame.SetActive(isStartGame);

            BattleModeOnPointerExit();
            version.text= Application.version.ToString();
            
            DataManager.Instance.ReloadData();
            
            testGO.SetActive(false);
            #if DEVELOPMENT_BUILD || UNITY_EDITOR
                testGO.SetActive(true);
            #endif
        }

        public void BattleModeOnPointerEnter()
        {
            GameUtility.DelayExcute(0.1f, () =>
            {
                var gamePlayData = DataManager.Instance.DataGame.User.GamePlayDatas[DataManager.Instance.DataGame.User.CurFileIdx][
                    EPVEType.BattleMode];

                if (gamePlayData != null)
                {
                    battleMode_startGame.SetActive(!gamePlayData.IsGamePlaying);
                    battleMode_continueGame.SetActive(gamePlayData.IsGamePlaying);
                    battleMode_restartGame.SetActive(gamePlayData.IsGamePlaying);

                }
            });
            
        }
        
        public void BattleModeOnPointerExit()
        {
            GameUtility.DelayExcute(0.1f, () =>
            {
                battleMode_startGame.SetActive(true);
                battleMode_continueGame.SetActive(false);
                battleMode_restartGame.SetActive(false);
            });
            
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Exit();
            }
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void CloseForm()
        {
            GameEntry.Entity.HideEntity(procedureStart.StartEntity);
            GameEntry.UI.CloseUIForm(this);
        }

        
        // public void StartGame()
        // {
        //
        //     CloseForm();
        //     procedureStart.StartSelect();
        // }
        //
        //
        // public void ContinueGame()
        // {
        //     CloseForm();
        //     //GameEntry.Event.Fire(null, GamePlayInitGameEventArgs.Create(GamePlayManager.Instance.GamePlayData.RandomSeed, EEnemyType.Normal));
        //
        //     GamePlayManager.Instance.InitPlayerData();
        //     procedureStart.ContinueGame();
        // }
        //
        // public void RestartGame()
        // {
        //     CloseForm();
        //     procedureStart.RestartGameTest();
        // }

        public void StartTest()
        {
            // if (!GamePlayManager.Instance.GamePlayData.IsEndTutorial)
            // {
            //     GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            //     {
            //         IsShowCancel = true,
            //         Message = GameEntry.Localization.GetString(Constant.Localization.Message_TutorailConfirm),
            //         ConfirmStr = GameEntry.Localization.GetString(Constant.Localization.UI_Tutorial),
            //         CancelStr = GameEntry.Localization.GetString(Constant.Localization.UI_BattleMode),
            //         
            //         OnConfirm = () =>
            //         {
            //             GamePlayManager.Instance.GamePlayData.IsTutorialBattle = true;
            //             Tutorial();
            //
            //         },
            //         OnClose = () =>
            //         {
            //             CloseForm();
            //             //procedureStart.RestartGameTest();
            //         }
            //
            //     });
            // }
            // else
            // {
            //     CloseForm();
            //     procedureStart.RestartGameTest();
            // }
            CloseForm();
            procedureStart.RestartGameTest();
            
        }
        
        public void StartBattleMode()
        {
            if (!GameManager.Instance.GameData.User.IsEndTutorial)
            {
                GamePlayManager.Instance.GamePlayData.IsTutorialBattle = true;
                Tutorial();
            }
            else
            {
                procedureStart.RestartBattleMode();
            }
            //procedureStart.RestartBattleMode();
        }
        
        public void ContinueBattleMode()
        {
            procedureStart.ContinueBattleMode();
        }

        public void Tutorial()
        {
            CloseForm();
            procedureStart.Reset(EPVEType.Tutorial);
            //GamePlayManager.Instance.GamePlayData.IsTutorial = true;
            GamePlayManager.Instance.GamePlayData.
                IsTutorialBattle = true;

            //Constant.Tutorial.RandomSeed;//
            int startGameRandomSeed = Random.Range(0, Constant.Game.RandomRange);
            
            GamePlayManager.Instance.GamePlayData.RandomSeed = startGameRandomSeed;
            GamePlayManager.Instance.GamePlayData.BattleData.GameDifficulty = EGameDifficulty.Difficulty0;
            GamePlayManager.Instance.GamePlayData.GameMode = EGamMode.PVE;
            GamePlayManager.Instance.GamePlayData.PVEType = EPVEType.Tutorial;
            
            Log.Debug("randomSeed:" + startGameRandomSeed);
            //GamePlayManager.Instance.GamePlayData.RandomSeed = startGameRandomSeed;
            GameEntry.Event.Fire(null, GamePlayStartGameEventArgs.Create());
        }

        public void StartAdventure()
        {
            GameEntry.UI.OpenMessage(GameEntry.Localization.GetString(Constant.Localization.Message_Developing));

        }
        
        public void Setting()
        {
            GameEntry.UI.OpenUIFormAsync(UIFormId.SettingForm, this);

        }
        
        public void Advise()
        {
            GameEntry.UI.OpenUIFormAsync(UIFormId.AdviseForm, this);

        }
    }
}
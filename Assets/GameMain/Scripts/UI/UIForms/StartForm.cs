﻿
using CatJson;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class StartForm : UGuiForm
    {
        private ProcedureStart procedureStart;

        [SerializeField] private GameObject startGame;
        [SerializeField] private GameObject continueGame;
        [SerializeField] private GameObject restartGame;

        protected override async void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            procedureStart = (ProcedureStart)userData;
            if (procedureStart == null)
            {
                Log.Warning("ProcedureStart is null.");
                return;
            }
            
            
            // var isStartGame = DataManager.Instance.DataGame.User.CurGamePlayData.PlayerData.BattleHero.HeroID !=
            //                   EHeroID.Empty;
            //
            // startGame.SetActive(!isStartGame);
            // continueGame.SetActive(isStartGame);
            // restartGame.SetActive(isStartGame);
            
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
        }

        private void CloseForm()
        {
            GameEntry.Entity.HideEntity(procedureStart.StartEntity);
            GameEntry.UI.CloseUIForm(this);
        }

        [JsonCareDefaultValue]
        public class testD
        {
            public int a = -1;

            public testD()
            {
                
            }
        }
        
        public void StartGame()
        {
            // var testD = new testD();
            //
            // GameEntry.Setting.SetObject("a", testD);
            // GameEntry.Setting.Save();
            // Log.Debug(GameEntry.Setting.GetObject<testD>("a").a);
            //
            // testD.a = 0;
            // GameEntry.Setting.SetObject("a", testD);
            // GameEntry.Setting.Save();
            // Log.Debug(GameEntry.Setting.GetObject<testD>("a").a);
            //
            // testD.a = 1;
            // GameEntry.Setting.SetObject("a", testD);
            // GameEntry.Setting.Save();
            // Log.Debug(GameEntry.Setting.GetObject<testD>("a").a);
            //
            // // testD.a = -2;
            // // GameEntry.Setting.SetObject("a", testD);
            // // GameEntry.Setting.Save();
            // // Log.Debug(GameEntry.Setting.GetObject<testD>("a").a);
            
            CloseForm();
            procedureStart.StartSelect();
        }


        public void ContinueGame()
        {
            CloseForm();
            //GameEntry.Event.Fire(null, GamePlayInitGameEventArgs.Create(GamePlayManager.Instance.GamePlayData.RandomSeed, EEnemyType.Normal));

            GamePlayManager.Instance.InitPlayerData();
            procedureStart.ContinueGame();
        }

        public void RestartGame()
        {
            CloseForm();
            procedureStart.RestartGame();
        }

        public void StartTest()
        {
            
            CloseForm();
            procedureStart.RestartGame();
        }

        public void Tutorial()
        {
            GamePlayManager.Instance.GamePlayData.IsTutorial = true;
            Close();

            int startGameRandomSeed = Constant.Tutorial.RandomSeed;

            
            Log.Debug("randomSeed:" + startGameRandomSeed);
            GamePlayManager.Instance.GamePlayData.RandomSeed = startGameRandomSeed;
            GameEntry.Event.Fire(null, GamePlayInitGameEventArgs.Create(startGameRandomSeed, EGameDifficulty.Difficulty1));
        }
    }
}
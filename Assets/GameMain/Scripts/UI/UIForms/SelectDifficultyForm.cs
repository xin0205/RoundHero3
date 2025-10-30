using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class SelectDifficultyForm : UGuiForm
    {
        private ProcedureStart procedureStart;
        [SerializeField] private Text desc;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            procedureStart = (ProcedureStart)userData;
            if (procedureStart == null)
            {
                Log.Warning("ProcedureStart is null.");
                return;
            }

            UnShowDiffcultyDesc();
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
        }
        
        public void Difficulty0()
        {
            Diffculty(EGameDifficulty.Difficulty0);
        }
 

        public void Difficulty1()
        {
            Diffculty(EGameDifficulty.Difficulty1);
        }

        public void Difficulty2()
        {
            Diffculty(EGameDifficulty.Difficulty2);
        }

        public void Diffculty3()
        {
            Diffculty(EGameDifficulty.Difficulty3);
        }
        
        public void Diffculty4()
        {
            Diffculty(EGameDifficulty.Difficulty4);
        }
        
        public void Diffculty5()
        {
            Diffculty(EGameDifficulty.Difficulty5);
        }

        public void Diffculty(EGameDifficulty difficulty)
        {
            procedureStart.Reset(EPVEType.BattleMode);
            GameManager.Instance.InitCards = new List<int>(Constant.BattleMode.InitCards);

            DataManager.Instance.DataGame.User.SetCurGamePlayData(EPVEType.BattleMode);
            
            // DataManager.Instance.DataGame.User.DefaultInitSelectCards =
            //     new List<int>(GameManager.Instance.InitCards);
            
            GameEntry.UI.CloseUIForm(this);
            procedureStart.CloseStartForm();

            var radomSeed = UnityEngine.Random.Range(0, Constant.Game.RandomRange);
            Log.Debug(radomSeed);
            GamePlayManager.Instance.GamePlayData.RandomSeed = radomSeed;
            GamePlayManager.Instance.GamePlayData.GameMode = EGamMode.PVE;
            GamePlayManager.Instance.GamePlayData.BattleData.GameDifficulty = difficulty;
            GamePlayManager.Instance.GamePlayData.PVEType = EPVEType.BattleMode;
            
            GamePlayManager.Instance.GamePlayData.BattleModeProduce.Session = 0;
            GamePlayManager.Instance.GamePlayData.BattleModeProduce.BattleModeStage = BattleModeStage.Battle;

            
            GameEntry.Event.Fire(null,
                GamePlayStartGameEventArgs.Create());
            
            
        }

        public void ShowDiffculty0Desc()
        {
            ShowDiffcultyDesc(EGameDifficulty.Difficulty0);
        }
        
        public void ShowDiffculty1Desc()
        {
            ShowDiffcultyDesc(EGameDifficulty.Difficulty1);
        }
        
        public void ShowDiffculty2Desc()
        {
            ShowDiffcultyDesc(EGameDifficulty.Difficulty2);
        }
        
        public void ShowDiffculty3Desc()
        {
            ShowDiffcultyDesc(EGameDifficulty.Difficulty3);
        }
        
        public void ShowDiffculty4Desc()
        {
            ShowDiffcultyDesc(EGameDifficulty.Difficulty4);
        }
        
        public void ShowDiffculty5Desc()
        {
            ShowDiffcultyDesc(EGameDifficulty.Difficulty5);
        }

        public void ShowDiffcultyDesc(EGameDifficulty gameDifficulty)
        {
            desc.gameObject.SetActive(true);
            desc.text = GameEntry.Localization.GetConnectString(Constant.Localization.DifficultyDesc, gameDifficulty);
        }

        public void UnShowDiffcultyDesc()
        {
            desc.gameObject.SetActive(false);
        }
    }
}
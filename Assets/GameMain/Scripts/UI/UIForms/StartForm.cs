
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
            
            procedureStart.StartEntity = await GameEntry.Entity.ShowSceneEntityAsync("Start");

            var isStartGame = DataManager.Instance.DataGame.User.CurGamePlayData.PlayerData.BattleHero.HeroID !=
                              EHeroID.Empty;
            
            startGame.SetActive(!isStartGame);
            continueGame.SetActive(isStartGame);
            restartGame.SetActive(isStartGame);
            
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
        
        public void StartGame()
        {
            CloseForm();
            procedureStart.StartSelect();
        }


        public void ContinueGame()
        {
            CloseForm();
            procedureStart.ContinueGame();
        }

        public void RestartGame()
        {
            CloseForm();
            procedureStart.RestartGame();
        }

        
    }
}
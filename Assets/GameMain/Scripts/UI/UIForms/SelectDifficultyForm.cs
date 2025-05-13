using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class SelectDifficultyForm : UGuiForm
    {
        private UGuiForm form;
        

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            form = (UGuiForm)userData;
            if (form == null)
            {
                Log.Warning("form is null.");
                return;
            }

        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
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

        public void Diffculty(EGameDifficulty difficulty)
        {
            GameEntry.UI.CloseUIForm(form);
            Close();

            //7071044;//
            int startGameRandomSeed = UnityEngine.Random.Range(0, Constant.Game.RandomRange);
            
            // GamePlayManager.Instance.GamePlayData.IsTutorial = true;
            // if (GamePlayManager.Instance.GamePlayData.IsTutorial)
            // {
            //     startGameRandomSeed = Constant.Tutorial.RandomSeed;
            // }
            // else
            // {
            //     startGameRandomSeed = UnityEngine.Random.Range(0, Constant.Game.RandomRange);
            // }
            
            Log.Debug("randomSeed:" + startGameRandomSeed);
            GamePlayManager.Instance.GamePlayData.RandomSeed = startGameRandomSeed;
            GameEntry.Event.Fire(null, GamePlayInitGameEventArgs.Create(startGameRandomSeed, difficulty));

        }
    }
}
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

        public void D1()
        {
            D(EGameDifficulty.Difficulty1);
        }

        public void D2()
        {
            D(EGameDifficulty.Difficulty2);
        }

        public void D3()
        {
            D(EGameDifficulty.Difficulty3);
        }

        public void D(EGameDifficulty difficulty)
        {
            GameEntry.UI.CloseUIForm(form);
            Close();

            //7071044;//
            var startGameRandomSeed = UnityEngine.Random.Range(0, Constant.Game.RandomRange);
            Log.Debug("randomSeed:" + startGameRandomSeed);
            GamePlayManager.Instance.GamePlayData.RandomSeed = startGameRandomSeed;
            GameEntry.Event.Fire(null, GamePlayInitGameEventArgs.Create(startGameRandomSeed, difficulty));

        }
    }
}
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class StartSelectForm : UGuiForm
    {
        private ProcedureStart procedureStart;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            procedureStart = (ProcedureStart)userData;
            if (procedureStart == null)
            {
                Log.Warning("ProcedureStart is null.");
                return;
            }

        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            
        }
        
        public void PVEStartGame()
        {
            GameEntry.Entity.HideEntity(procedureStart.StartSelectEntity);
            GameEntry.UI.CloseUIForm(this);
            
            var randomSeed = UnityEngine.Random.Range(0, Constant.Game.RandomRange);
            randomSeed = 94204398;//2198030
            Log.Debug("randomSeed:" + randomSeed);
            GamePlayManager.Instance.GamePlayData.RandomSeed = randomSeed;
            GameEntry.Event.Fire(null, GamePlayInitGameEventArgs.Create(randomSeed, EEnemyType.Normal));
        }

        
        

        
    }
}
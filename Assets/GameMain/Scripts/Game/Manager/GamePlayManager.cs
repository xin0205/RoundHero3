using System;

namespace RoundHero
{
    public class GamePlayManager : Singleton<GamePlayManager>
    {
        public Data_GamePlay GamePlayData => DataManager.Instance.DataGame.User.CurGamePlayData;
        //public ProcedureGamePlay ProcedureGamePlay;
        
        public void Start()
        {
            if (GamePlayData.GameMode == EGamMode.PVE)
            {
                GamePlayManager.Instance.GamePlayData.AddPlayerData(PlayerManager.Instance.PlayerData);
                PlayerManager.Instance.PlayerData.Clear();
                
                
                PVEManager.Instance.SetCurPlayer();

                var drHero = GameEntry.DataTable.GetHero(GameManager.Instance.TmpHeroID);
                BattleHeroManager.Instance.InitHeroData(drHero.HeroID);
                
                BattlePlayerManager.Instance.InitData(EUnitCamp.Player1);

            }
            
        }

        public void Contitnue()
        {
            if (GamePlayData.GameMode == EGamMode.PVE)
            {
                var random = new Random(GamePlayData.RandomSeed);

                BattleMapManager.Instance.Init(random.Next());
                //BattleEventManager.Instance.Init(random.Next());
                BattleAreaManager.Instance.Init(random.Next());
                BattleUnitManager.Instance.Init(random.Next());
                BattleHeroManager.Instance.Init(random.Next());

                BattleEnergyBuffManager.Instance.Init(random.Next());
                BattleEnergyBuffManager.Instance.InitHero(BattleHeroManager.Instance.BattleHeroData);
                
                PVEManager.Instance.Init(random.Next());
                
            }
        }

        public void Destory(EGamMode gameMode)
        {
            if (gameMode == EGamMode.PVE)
            {
                PVEManager.Instance.Destory();

            }
        }

        public void SetProcedureGamePlay(ProcedureGamePlay procedureGamePlay)
        {
            //ProcedureGamePlay = procedureGamePlay;
        }

    }
}
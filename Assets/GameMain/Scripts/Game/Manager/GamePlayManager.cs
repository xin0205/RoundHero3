using System;
using Animancer;

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
                GamePlayManager.Instance.GamePlayData.ClearPlayerDataList();
                GamePlayManager.Instance.GamePlayData.AddPlayerData(PlayerManager.Instance.PlayerData);
                GamePlayManager.Instance.GamePlayData.IsStartGame = true;
                InitPlayerData();
                //PlayerManager.Instance.PlayerData.Clear();
                
                var drHero = GameEntry.DataTable.GetHero(GameManager.Instance.TmpHeroID);
                HeroManager.Instance.InitHeroData(drHero.HeroID);
                
                BattlePlayerManager.Instance.InitData(EUnitCamp.Player1);

            }
            
        }

        public void InitPlayerData()
        {
            PVEManager.Instance.SetCurPlayer();
        }

        public void Contitnue()
        {
            if (GamePlayData.GameMode == EGamMode.PVE)
            {
                PVEManager.Instance.SetCurPlayer();
                
                var random = new Random(GamePlayData.RandomSeed);

                BlessManager.Instance.Init(random.Next());
                BattleMapManager.Instance.Init(random.Next());
                BattleModeManager.Instance.Init(random.Next());
                //BattleEventManager.Instance.Init(random.Next());
                
                
                

                // BattleEnergyBuffManager.Instance.Init(random.Next());
                // BattleEnergyBuffManager.Instance.InitHero(BattleHeroManager.Instance.BattleHeroData);
                
                //PVEManager.Instance.Init(random.Next());
                
            }
        }

        public void Back()
        {
            BlessManager.Instance.Destory();
            BattleMapManager.Instance.Destory();
            BattleModeManager.Instance.Destory();
            //GamePlayManager.Instance.GamePlayData.Clear();
            // BattleEnergyBuffManager.Instance.Destory();
            // BattleHeroManager.Instance.Destory();
        }

        public void Destory(EGamMode gameMode)
        {
            if (gameMode == EGamMode.PVE)
            {
                //PVEManager.Instance.Destory();

            }
        }

        public void SetProcedureGamePlay(ProcedureGamePlay procedureGamePlay)
        {
            //ProcedureGamePlay = procedureGamePlay;
        }

    }
}
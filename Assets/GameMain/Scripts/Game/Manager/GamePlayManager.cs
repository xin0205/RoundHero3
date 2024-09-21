using System;

namespace RoundHero
{
    public class GamePlayManager : Singleton<GamePlayManager>
    {
        public Data_GamePlay GamePlayData => DataManager.Instance.CurUser.GamePlayData;
        public ProcedureGamePlay ProcedureGamePlay;
        
        public void Init(GamePlayInitData gamePlayInitData)
        {
            if (gamePlayInitData.GameMode == EGamMode.PVE)
            {
                var random = new Random(GamePlayData.RandomSeed);
                // var randoms = MathUtility.GetRandomNum(6, 0,
                //     Constant.Game.RandomRange, random);
            
                GamePlayManager.Instance.GamePlayData.AddPlayerData(PlayerManager.Instance.PlayerData);
                
                PVEManager.Instance.SetCurPlayer();
                
                BattleHeroManager.Instance.InitHeroData();
                
                BattlePlayerManager.Instance.InitData(EUnitCamp.Player1);

                BattleMapManager.Instance.Init(random.Next());
                BattleEventManager.Instance.Init(random.Next());
                BattleAreaManager.Instance.Init(random.Next());
                BattleUnitManager.Instance.Init(random.Next());
                BattleHeroManager.Instance.Init(random.Next());

                BattleEnergyBuffManager.Instance.Init(random.Next());
                BattleEnergyBuffManager.Instance.InitHero(BattleHeroManager.Instance.BattleHeroData);
                
            }
            
        }

        public void SetProcedureGamePlay(ProcedureGamePlay procedureGamePlay)
        {
            ProcedureGamePlay = procedureGamePlay;
        }

    }
}
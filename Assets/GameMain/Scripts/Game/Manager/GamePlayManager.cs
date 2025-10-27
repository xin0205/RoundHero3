using System.Threading.Tasks;
using Animancer;
using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    public class GamePlayManager : Singleton<GamePlayManager>
    {
        public Data_GamePlay GamePlayData => DataManager.Instance.DataGame.User.CurGamePlayData;
        //public ProcedureGamePlay ProcedureGamePlay;
        
        public void Init(int randomSeed)
        {
            if (GamePlayData.GameMode == EGamMode.PVE)
            {
                PVEManager.Instance.SetCurPlayer(BattleManager.Instance.CurUnitCamp);
                
                var random = new Random(randomSeed);
                
                BlessManager.Instance.Init(random.Next());
                BattleCardManager.Instance.Init(random.Next());
                
                if (GamePlayData.GameMode == EGamMode.PVE)
                {
                    GamePlayManager.Instance.GamePlayData.ClearPlayerDataList();
                    GamePlayManager.Instance.GamePlayData.AddPlayerData(PlayerManager.Instance.PlayerData);
                    GamePlayManager.Instance.GamePlayData.IsGamePlaying = true;
                    //InitPlayerData();
                    //PlayerManager.Instance.PlayerData.Clear();
                
                    // var drHero = GameEntry.DataTable.GetHero(GameManager.Instance.TmpHeroID);
                    

                }
                
                if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.BattleMode)
                {
                    BattleModeManager.Instance.Init(random.Next());
                }
                else if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.Adventure)
                {
                    BattleMapManager.Instance.Init(random.Next());
                }
                else if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.Test)
                {
  
                }
                else if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.Tutorial)
                {

                }
                

                
                
                
                

            }
        }
        
        public void Start(int randomSeed)
        {
            GamePlayManager.Instance.Init(randomSeed);
            BattlePlayerManager.Instance.Start();
            HeroManager.Instance.Start();
            BattleCardManager.Instance.Start();
           
        }

        public void Continue(int randomSeed)
        {
            GamePlayManager.Instance.Init(randomSeed);
            BattlePlayerManager.Instance.Continue();
            BattleCardManager.Instance.Continue();
        }

        public void InitPlayerData()
        {
            PVEManager.Instance.SetCurPlayer(BattleManager.Instance.CurUnitCamp);
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

        public async Task ShowBattleData(Data_Battle battleData)
        {
            foreach (var kv in battleData.GridTypes)
            {
                BattleAreaManager.Instance.GenerateGridEntity(kv.Key, kv.Value);
            }
            
            GamePlayManager.Instance.GamePlayData.BattleData.BattleUnitDatas.Clear();
            foreach (var kv in battleData.BattleUnitDatas)
            {
                if (kv.Value is Data_BattleCore battleCore)
                {
                    var battleCoreData = battleCore.Copy();
                    await BattleCoreManager.Instance.GenerateCoreEntity(battleCoreData);


                }
                else if (kv.Value is Data_BattleSolider battleSolider)
                {
                    var battleSoliderData = battleSolider.Copy();
                    await BattleAreaManager.Instance.GenerateSolider(battleSoliderData);
                    // BattleUnitManager.Instance.BattleUnitDatas.Remove(soliderEntity.BattleUnitData.Idx);
                    // soliderEntity.BattleUnitData = battleSolider.Copy();
                    // BattleUnitManager.Instance.BattleUnitDatas.Add(soliderEntity.BattleUnitData.Idx, soliderEntity.BattleUnitData);
                }
                else if (kv.Value is Data_BattleMonster battleMonster)
                {

                    var battleEnemyData = battleMonster.Copy();
                    
                    await BattleEnemyManager.Instance.GenerateEnemy(battleEnemyData);
                    
                    // BattleUnitManager.Instance.BattleUnitDatas.Remove(enemyEntity.BattleUnitData.Idx);
                    // enemyEntity.BattleUnitData = battleMonster.Copy();
                    // BattleUnitManager.Instance.BattleUnitDatas.Add(enemyEntity.BattleUnitData.Idx, enemyEntity.BattleUnitData);
                }
                
            }
            
            GamePlayManager.Instance.GamePlayData.BattleData.GridPropDatas.Clear();
            foreach (var kv in battleData.GridPropDatas)
            {
                var gridPropData = kv.Value.Copy();
                await BattleGridPropManager.Instance.GenerateGridProp(gridPropData);
                
                
                
            }

            GamePlayManager.Instance.GamePlayData.BattleData.BattlePlayerDatas[EUnitCamp.Player1].HandCards.Clear();
            var battlePlayerData = battleData.BattlePlayerDatas[EUnitCamp.Player1];
            BattleCardManager.Instance.SetCardPosList(battlePlayerData.HandCards.Count);
            var idx = 0;
            foreach (var  cardIdx in battlePlayerData.HandCards)
            {
                var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardIdx, idx);

                card.transform.position = BattleController.Instance.StandByCardPos.position;
                card.SetSortingOrder(idx * 10);
                card.AcquireCard(new Vector2(BattleCardManager.Instance.CardPosList[idx], BattleController.Instance.HandCardPos.localPosition.y),
                    idx * 0.15f + 0.15f);
                
                BattleCardManager.Instance.AddHandCard(card);
                GamePlayManager.Instance.GamePlayData.BattleData.BattlePlayerDatas[EUnitCamp.Player1].HandCards.Add(cardIdx);
                
                
                idx++;
            }

            GamePlayManager.Instance.InitPlayerData();
            BattleManager.Instance.SetBattleState(EBattleState.UseCard);
            BattleManager.Instance.RefreshEnemyAttackData();
        }

    }
}
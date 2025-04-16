using System.Collections.Generic;
using Unity.VisualScripting;
using Random = System.Random;

namespace RoundHero
{
    public class BattleRouteManager: Singleton<BattleRouteManager>
    {
        public Dictionary<int, BattleRouteEntity>  BattleRouteEntities = new ();
        //private bool isShowEntity = false;
        private int curEntityIdx = 0;
        private int showEntityIdx = 0;
        
        public Random Random;
        private int randomSeed;

        

        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
            curEntityIdx = 0;
            showEntityIdx = 0;

        }
        
        public void ShowEnemyRoute()
        {
            UnShowEnemyRoutes();
            ShowEnemyRoutes();
        }


        public async void ShowEnemyRoutes()
        {
            // ||BattleManager.Instance.BattleState == EBattleState.End
            if (BattleManager.Instance.BattleState == EBattleState.ActionExcuting)
            {
                return;
            }

            // if (isShowRoute)
            // {
            //     UnShowEnemyRoutes();
            // }
            
            //isShowEntity = true;
            BattleRouteEntities.Clear();
            
            var enemyMovePaths = new Dictionary<int, List<int>>(BattleFightManager.Instance.RoundFightData.EnemyMovePaths);
            enemyMovePaths.AddRange(BattleFightManager.Instance.RoundFightData.ThirdUnitMovePaths);
            
            var entityIdx = curEntityIdx;
            foreach (var kv in enemyMovePaths)
            {
                if (kv.Value == null || kv.Value.Count <= 0)
                {
                    continue;
                }

                curEntityIdx++;
            }

            
            foreach (var kv in enemyMovePaths)
            {
                if (kv.Value == null || kv.Value.Count <= 0)
                {
                    continue;
                }
                
                var battleRouteEntity = await GameEntry.Entity.ShowBattleRouteEntityAsync(kv.Value, entityIdx);
                entityIdx++;
                //battleRouteEntity.SetCurrent(kv.Value.First() == BattleAreaManager.Instance.CurPointGridPosIdx);
                
                // !isShowRoute || 
                if (battleRouteEntity.BattleRouteEntityData.EntityIdx < showEntityIdx)
                {
                    
                    GameEntry.Entity.HideEntity(battleRouteEntity);
                    //break;
                }
                else
                {
                    BattleRouteEntities.Add(battleRouteEntity.BattleRouteEntityData.Id, battleRouteEntity);
                }

            }

        }

        public void UnShowEnemyRoutes()
        {
            
            //isShowEntity = false;
            showEntityIdx = curEntityIdx;
            
            foreach (var kv in BattleRouteEntities)
            {
                GameEntry.Entity.HideEntity(kv.Value.Entity);
            }
            BattleRouteEntities.Clear();
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFramework.Event;

namespace RoundHero
{
    public class BattleThirdUnitManager : Singleton<BattleThirdUnitManager>
    {
        public Random Random;
        private int randomSeed;
        
        public Dictionary<int, BattleMonsterEntity> ThirdUnitEntities = new ();
        
        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
        }
        
        public async Task GenerateNewThirdUnits()
        {
            await GenerateThirdUnits();
            BattleManager.Instance.Refresh();
            //BattleSoliderManager.Instance.CacheSoliderActionRange();
        }
        
        public async Task GenerateThirdUnits()
        {
            BattleAreaManager.Instance.RefreshObstacles();
            var places = BattleAreaManager.Instance.GetPlaces();

            var enemyIdxs = MathUtility.GetRandomNum(
                3, 0,
                places.Count, Random);
            
            for (int i = 0; i < 1; i++)
            {
                var battleEnemyEntity = await GameEntry.Entity.ShowBattleMonsterEntityAsync(0, 3,  places[enemyIdxs[i]], EUnitCamp.Third, new List<int>());
                
                BattleUnitManager.Instance.BattleUnitEntities.Add(battleEnemyEntity.BattleMonsterEntityData.BattleMonsterData.ID, battleEnemyEntity);
                RefreshEntities();
                
                if (battleEnemyEntity is IMoveGrid moveGrid)
                {
                    BattleAreaManager.Instance.MoveGrids.Add(battleEnemyEntity.BattleMonsterEntityData.Id, moveGrid);
                }
            }
        }
        
        public void RefreshEntities()
        {
            ThirdUnitEntities.Clear();
            foreach (var kv in BattleUnitManager.Instance.BattleUnitEntities)
            {
                if (kv.Value.UnitCamp == EUnitCamp.Third)
                {
                    ThirdUnitEntities.Add(kv.Key, kv.Value as BattleMonsterEntity);
                }
            }

        }
        
        // public void RefreshDamageState()
        // {
        //     foreach (var kv in ThirdUnitEntities)
        //     {
        //         kv.Value.RefreshDamageState();
        //     }
        // }

        public void Destory()
        {
        }
    }
}
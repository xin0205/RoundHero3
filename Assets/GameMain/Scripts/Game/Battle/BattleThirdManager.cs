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
            BattleManager.Instance.RefreshEnemyAttackData();
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
                var battleEnemyData = new Data_BattleMonster(BattleUnitManager.Instance.GetIdx(), 0,
                    places[enemyIdxs[i]], EUnitCamp.Third, new List<int>(), BattleManager.Instance.BattleData.Round);
                
                var battleEnemyEntity = await GameEntry.Entity.ShowBattleMonsterEntityAsync(battleEnemyData);
                
                BattleUnitManager.Instance.BattleUnitDatas.Add(battleEnemyData.Idx, battleEnemyData);
                BattleUnitManager.Instance.BattleUnitEntities.Add(battleEnemyEntity.BattleMonsterEntityData.BattleMonsterData.Idx, battleEnemyEntity);
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
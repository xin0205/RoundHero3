
using System.Collections.Generic;

using System.Threading.Tasks;

using GameFramework.Event;

using UnityEngine;

using Random = System.Random;

namespace RoundHero
{
    public class BattleEnemyManager : Singleton<BattleEnemyManager>
    {
        
        
        //public Dictionary<int, Data_BattleMonster> BattleEnemyDatas => DataManager.Instance.CurUser.GamePlayData.BattleData.BattleEnemies;
        //public Dictionary<int, List<int>> EnemyMovePaths = new ();
        
        
        
        //private int id;
        public Random Random;
        private int randomSeed;

        

        public void Init(int randomSeed)
        {
            GameEntry.Event.Subscribe(ShowGridDetailEventArgs.EventId, OnShowGridDetail);
            //GameEntry.Event.Subscribe(RefreshUnitDataEventArgs.EventId, OnRefreshUnitData);
            
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);

            //BattleManager.Instance.BattleData.EnemyType = enemyType;
            
            //id = 0;

            // var ranges = new Dictionary<int, bool>(Constant.Area.GridSize.x * Constant.Area.GridSize.y);
            // for (int i = 0; i < Constant.Area.GridSize.x * Constant.Area.GridSize.y; i++)
            // {
            //     ranges.Add(i, false);
            //}
            buffValuelist = new List<List<float>>(10);
            for (int i = 0; i < 10; i++)
            {
                var values = new List<float>(10);
                for (int j = 0; j < 10; j++)
                {
                    values.Add(0);
                }
                buffValuelist.Add(values);
            }


        }

        

        public void Destory()
        {
            GameEntry.Event.Unsubscribe(ShowGridDetailEventArgs.EventId, OnShowGridDetail);
            //GameEntry.Event.Unsubscribe(RefreshUnitDataEventArgs.EventId, OnRefreshUnitData);
        }
        
        // public int GetID()
        // {
        //     return id++;
        // }

        public void OnShowGridDetail(object sender, GameEventArgs e)
        {
            var ne = e as ShowGridDetailEventArgs;
            var isEnemy = BattleUnitManager.Instance.GetUnitID(ne.GridPosIdx, BattleManager.Instance.CurUnitCamp,ERelativeCamp.Enemy) != -1;
            

            if (isEnemy)
            {
                if (ne.ShowState == EShowState.Show)
                {
                    //ShowEnemyRoutes();
                }    
                else if(ne.ShowState == EShowState.Unshow)
                {
                    //UnShowEnemyRoutes();
                }
            }
        }

        public void OnRefreshUnitData(object sender, GameEventArgs e)
        {
            foreach (var kv in BattleUnitManager.Instance.BattleUnitEntities)
            {
                if (kv.Value.UnitCamp == EUnitCamp.Enemy)
                {
                    (kv.Value as BattleMonsterEntity).RefreshData();
                }
                
            }
        }

        

        public void Update()
        {

        }
        
        // public int GetEnemyID(int posIdx)
        // {
        //     var entityID = -1;
        //
        //     foreach (var kv in EnemyEntities)
        //     {
        //         if (kv.Value.GridPosIdx == posIdx)
        //         {
        //             entityID = kv.Value.BattleMonsterEntityData.battleMonsterData.ID;
        //         }
        //     }
        //
        //     return entityID;
        // }
        
        public async Task GenerateEnemies()
        {
            if(BattleManager.Instance.BattleData.Round >= Constant.Enemy.EnemyGenerateTurns[BattleManager.Instance.BattleData.EnemyType])
                return;

            BattleAreaManager.Instance.RefreshObstacles();
            var places = BattleAreaManager.Instance.GetPlaces();

            var enemyIdxs = MathUtility.GetRandomNum(
                Constant.Enemy.EachTurnGenerateEnemyCounts[BattleManager.Instance.BattleData.EnemyType], 0,
                places.Count, Random);
            
            for (int i = 0; i < Constant.Enemy.EachTurnGenerateEnemyCounts[BattleManager.Instance.BattleData.EnemyType]; i++)
            {
                var randomEnemyType = 0;//Random.Next(0, 3);
                var battleEnemyEntity = await GameEntry.Entity.ShowBattleMonsterEntityAsync(0, randomEnemyType,  places[enemyIdxs[i]], EUnitCamp.Enemy, new List<int>());

                BattleCurseManager.Instance.AllUnitDodgeSubHeartDamageDict_Add(battleEnemyEntity.BattleMonsterEntityData.BattleMonsterData.Idx);
                
                var enemyGenerateAddDebuff = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.EnemyGenerateAddDebuff, BattleManager.Instance.CurUnitCamp);
                if (enemyGenerateAddDebuff != null )
                {
                    var randomDebuffIdx = Random.Next(0, Constant.Battle.EffectUnitStates[EUnitStateEffectType.Negative].Count);
                    var randomDeBuff = Constant.Battle.EffectUnitStates[EUnitStateEffectType.Negative][randomDebuffIdx];
                    battleEnemyEntity.BattleMonsterEntityData.BattleMonsterData.ChangeState(randomDeBuff);
                }
                
                BattleUnitManager.Instance.BattleUnitEntities.Add(battleEnemyEntity.BattleMonsterEntityData.BattleMonsterData.Idx, battleEnemyEntity);
                //RefreshEnemyEntities();
                
                if (battleEnemyEntity is IMoveGrid moveGrid)
                {
                    BattleAreaManager.Instance.MoveGrids.Add(battleEnemyEntity.BattleMonsterEntityData.Id, moveGrid);
                }
            }
        }


        
        
        
        public int GetDis(int enemyID,Vector2Int enemyCoord)
        {
            // if (EnemyEntities[monsterID].BattleMonsterEntityData.battleMonsterData.CurHP <=
            //     0)
            // {
            //     return 0;
            // }
            
            var heroCoord = GameUtility.GridPosIdxToCoord(HeroManager.Instance.HeroEntity.BattleHeroEntityData.BattleHeroData.GridPosIdx);
            var dis = GameUtility.GetDis(enemyCoord, heroCoord);
            return dis;
        }
        
        

        public void StartAction()
        {
            if (BattleManager.Instance.BattleState != EBattleState.UseCard)
            {
                return;
            }
            
            //BattleHeroManager.Instance.AddHP();
            
            BattleManager.Instance.StartAction();

        }

        public async Task GenerateNewEnemies()
        {
            await GenerateEnemies();
            BattleManager.Instance.Refresh();
            //BattleSoliderManager.Instance.CacheSoliderActionRange();
        }

        
        
        

        // public void RefreshDamageState()
        // {
        //     foreach (var kv in EnemyEntities)
        //     {
        //         kv.Value.RefreshDamageState();
        //     }
        // }

        public void RemoveEnemy(int enemyID)
        {
            var monsterEntity = BattleUnitManager.Instance.BattleUnitEntities[enemyID] as BattleMonsterEntity;
            //EnemyMovePaths.Remove(monsterEntity.BattleMonsterEntityData.Id);
            BattleAreaManager.Instance.MoveGrids.Remove(monsterEntity.BattleMonsterEntityData.Id);
            BattleUnitManager.Instance.BattleUnitEntities.Remove(enemyID);
            BattleUnitManager.Instance.BattleUnitDatas.Remove(enemyID); 
            BattleAreaManager.Instance.MoveGrids.Remove(enemyID);
            
            //RefreshEnemyEntities();
            BattleAreaManager.Instance.RefreshObstacles();
            BattleManager.Instance.Refresh();
        }

        // public bool InEnemyPaths(int gridPosIdx)
        // {
        //     foreach (var kv in EnemyMovePaths)
        //     {
        //         foreach (var pathGridPosIdx in kv.Value)
        //         {
        //             if (pathGridPosIdx == gridPosIdx)
        //             {
        //                 return true;
        //             }
        //         }
        //     }
        //
        //     return false;
        // }
        
        public List<BuffData> GetBuffData(int monsterID)
        {
            var drEnemy = RoundHero.GameEntry.DataTable.GetEnemy(monsterID);
            
            var buffDatas = new List<BuffData>();

            foreach (var buffID in drEnemy.OwnBuffs)
            {
                var buffData = BattleBuffManager.Instance.GetBuffData(buffID);
                buffDatas.Add(buffData);
            }
            
            foreach (var buffID in drEnemy.SecondaryBuffs)
            {
                var buffData = BattleBuffManager.Instance.GetBuffData(buffID);
                buffDatas.Add(buffData);
            }

            return buffDatas;
        }
        
        // public List<DRBuff> GetSecondaryBuffTable(int monsterID)
        // {
        //     var drEnemy = RoundHero.GameEntry.DataTable.GetEnemy(monsterID);
        //     
        //     var drBuffs = new List<DRBuff>();
        //
        //     foreach (var buffID in drEnemy.SecondaryBuffs)
        //     {
        //         var drBuff = GameEntry.DataTable.GetBuff(buffID);
        //         drBuffs.Add(drBuff);
        //     }
        //
        //     return drBuffs;
        // }
        
        public List<string> GetSecondaryBuff(int monsterID)
        {
            var drEnemy = RoundHero.GameEntry.DataTable.GetEnemy(monsterID);
            
            var buffStrs = new List<string>();

            foreach (var buffStr in drEnemy.SecondaryBuffs)
            {
                buffStrs.Add(buffStr);
            }

            return buffStrs;
        }
        
        private List<List<float>> buffValuelist = new List<List<float>>();
        public List<List<float>> GetBuffValues(int monsterID)
        {
            var drEnemy = RoundHero.GameEntry.DataTable.GetEnemy(monsterID);

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    buffValuelist[i][j] = 0;
                }
            }
            var idx = 0;
            var idx2 = 0;

            foreach (var buffID in drEnemy.OwnBuffs)
            {
                //var values = new List<float>();
                foreach (var value in drEnemy.OwnBuffValues1)
                {
                    buffValuelist[idx][idx2++] = BattleBuffManager.Instance.GetBuffValue(value);
                    //buffValuelist[idx].Add(BattleBuffManager.Instance.GetBuffValue(value));
                }

                idx++;
                idx2 = 0;
                //valuelist.Add(values);
            }
            
            foreach (var buffID in drEnemy.SecondaryBuffs)
            {
                //var values = new List<float>();
                foreach (var value in drEnemy.SecondaryValues)
                {
                    buffValuelist[idx][idx2++] = BattleBuffManager.Instance.GetBuffValue(value);
                }

                idx++;
                idx2 = 0;
                //valuelist.Add(values);
            }

            return buffValuelist;
        }

        public List<List<float>> GetSecondaryBuffValues(int monsterID)
        {
            var drEnemy = RoundHero.GameEntry.DataTable.GetEnemy(monsterID);
            
            var valuelist = new List<List<float>>();

            //var idx = 2;
            foreach (var buffID in drEnemy.SecondaryBuffs)
            {
                var values = new List<float>();
                foreach (var value in drEnemy.SecondaryValues)
                {
                    values.Add(BattleBuffManager.Instance.GetBuffValue(value));
                }
                valuelist.Add(values);
            }

            return valuelist;
        }

    }
}
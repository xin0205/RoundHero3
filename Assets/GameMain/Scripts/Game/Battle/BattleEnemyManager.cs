
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using Steamworks;
using UnityEngine;
using UnityGameFramework.Runtime;
using Random = System.Random;

namespace RoundHero
{
    public class EnemyGenerateData
    {
        // public List<int> EliteUnitTypeList = new List<int>();
        // public List<int>
        public List<int> GlobalDebuffList = new List<int>();
        public Dictionary<int, int> RoundGenerateUnitCount = new Dictionary<int, int>();
        public List<int> UnitList = new List<int>();
        public int UnitIdx = 0;

        public void Clear()
        {
            GlobalDebuffList.Clear();
            RoundGenerateUnitCount.Clear();
            UnitList.Clear();
            UnitIdx = 0;
        }
        
    }
    public class BattleEnemyManager : Singleton<BattleEnemyManager>
    {

        //public Dictionary<int, Data_BattleMonster> BattleEnemyDatas => DataManager.Instance.CurUser.GamePlayData.BattleData.BattleEnemies;

        //private int id;
        public Random Random;
        private int randomSeed;
        public EnemyGenerateData EnemyGenerateData = new EnemyGenerateData();
        

        public void Init(int randomSeed)
        {
            GameEntry.Event.Subscribe(ShowGridDetailEventArgs.EventId, OnShowGridDetail);
            //GameEntry.Event.Subscribe(RefreshUnitDataEventArgs.EventId, OnRefreshUnitData);
            
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
            EnemyGenerateData.Clear(); 
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

            InitGenerateRole(Random.Next());
        }

        public void InitGenerateRole(int randomSeed)
        {
            Log.Debug("InitGenerateRole:" + randomSeed);
            var random = new Random(randomSeed);
            
            var rule = Constant.Enemy.EnemyGenerateRules[BattleManager.Instance.BattleData.GameDifficulty];
            
            EnemyGenerateData.RoundGenerateUnitCount = new Dictionary<int, int>(rule.RoundGenerateUnitCount);
            var normalUnitList = GameEntry.DataTable.GetEnemys(EEnemyType.Normal);
            var normalUnitTypeRandoms = MathUtility.GetRandomNum(rule.NormalUnitTypeCount, 0, normalUnitList.Count,
                new Random(random.Next()));
            var normalUnitRandoms = MathUtility.GetRandomNum(rule.NormalUnitCount, 0, normalUnitTypeRandoms.Count,
                new Random(random.Next()), true);
            
            var eliteUnitList = GameEntry.DataTable.GetEnemys(EEnemyType.Elite);
            var eliteUnitTypeRandoms = MathUtility.GetRandomNum(rule.EliteUnitTypeCount, 0, eliteUnitList.Count,
                new Random(random.Next()));
            var eliteUnitRandoms = MathUtility.GetRandomNum(rule.EliteUnitCount, 0, eliteUnitTypeRandoms.Count,
                new Random(random.Next()), true);

            var unitList = new List<int>(normalUnitRandoms.Count + eliteUnitRandoms.Count);
            foreach (var idx in normalUnitRandoms)
            {
                unitList.Add(normalUnitList[normalUnitTypeRandoms[idx]].Id);
            }
            
            foreach (var idx in eliteUnitRandoms)
            {
                unitList.Add(eliteUnitList[eliteUnitTypeRandoms[idx]].Id);
            }
            
            var unitListRandoms = MathUtility.GetRandomNum(unitList.Count, 0, unitList.Count,
                new Random(random.Next()));

            foreach (var idx in unitListRandoms)
            {
                EnemyGenerateData.UnitList.Add(unitList[idx]);
            }
            
            var globalBuffList = GameEntry.DataTable.GetBuffs(EBuffType.EnemyGlobal);
            var globalBuffRandoms = MathUtility.GetRandomNum(rule.GlobalDebuffCount, 0, globalBuffList.Count, new Random(random.Next()));
            foreach (var idx in globalBuffRandoms)
            {
                EnemyGenerateData.GlobalDebuffList.Add(globalBuffRandoms[idx]);
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
            var isEnemy = BattleUnitManager.Instance.GetUnitIdx(ne.GridPosIdx, BattleManager.Instance.CurUnitCamp,ERelativeCamp.Enemy) != -1;
            

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
            // if(BattleManager.Instance.BattleData.Round >= Constant.Enemy.EnemyGenerateTurns[BattleManager.Instance.BattleData.EnemyType])
            //     return;

            
            BattleAreaManager.Instance.RefreshObstacles();
            var places = BattleAreaManager.Instance.GetPlaces();
            
            var rule = Constant.Enemy.EnemyGenerateRules[BattleManager.Instance.BattleData.GameDifficulty];


            var enemyCurCount = BattleUnitManager.Instance.GetUnitCount(EUnitCamp.Enemy);
            var enemyGenerateCount = 0;
            

            if (rule.RoundGenerateUnitCount.ContainsKey(BattleManager.Instance.BattleData.Round))
            {
                if (EnemyGenerateData.RoundGenerateUnitCount[BattleManager.Instance.BattleData.Round] > 0)
                {
                    enemyGenerateCount =
                        EnemyGenerateData.RoundGenerateUnitCount[BattleManager.Instance.BattleData.Round];
                    EnemyGenerateData.RoundGenerateUnitCount[BattleManager.Instance.BattleData.Round] -=
                        enemyGenerateCount;
                }
            }
            else
            {
                if (enemyCurCount < rule.EachRoundUnitCount)
                {
                    var needCount = rule.EachRoundUnitCount - enemyCurCount;
                    var curNeedCount = needCount;
                    var keys = EnemyGenerateData.RoundGenerateUnitCount.Keys.ToList();
                    
                    
                    for (int i = 0; i < EnemyGenerateData.RoundGenerateUnitCount.Count; i++)
                    {
                        var idx = keys[i];
                        var roundCount = EnemyGenerateData.RoundGenerateUnitCount[idx];
                        if (i >= BattleManager.Instance.BattleData.Round)
                        {
                            if (roundCount >= curNeedCount)
                            {
                                EnemyGenerateData.RoundGenerateUnitCount[idx] -= curNeedCount;
                                enemyGenerateCount += curNeedCount;
                                curNeedCount = 0;

                            }
                            else if(roundCount < curNeedCount)
                            {
                                EnemyGenerateData.RoundGenerateUnitCount[idx] = 0;
                                enemyGenerateCount += roundCount;
                                curNeedCount -= roundCount;
                            }
                            
                            if (enemyGenerateCount >= needCount)
                            {
                                break;
                            }
                        }
                    }
                    
                }
            }

            var enemyIdxs = MathUtility.GetRandomNum(
                enemyGenerateCount, 0,
                places.Count, Random);
            
            for (int i = 0; i < enemyGenerateCount; i++)
            {
                var enemyId = EnemyGenerateData.UnitList[EnemyGenerateData.UnitIdx];//Random.Next(0, 3);
                var battleEnemyEntity = await GameEntry.Entity.ShowBattleMonsterEntityAsync(enemyId, places[enemyIdxs[i]], EUnitCamp.Enemy, new List<int>());
                EnemyGenerateData.UnitIdx++;
                battleEnemyEntity.LookAtHero();
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


        
        
        
        // public int GetDis(int enemyID,Vector2Int enemyCoord)
        // {
        //     // if (EnemyEntities[monsterID].BattleMonsterEntityData.battleMonsterData.CurHP <=
        //     //     0)
        //     // {
        //     //     return 0;
        //     // }
        //     
        //     var heroCoord = GameUtility.GridPosIdxToCoord(HeroManager.Instance.BattleHeroData.GridPosIdx);
        //     var dis = GameUtility.GetDis(enemyCoord, heroCoord);
        //     return dis;
        // }
        
        // public void EnemyMove()
        // {
        //     BattleFightManager.Instance.EnemyMove();
        // }
        //
        // public void EnemyAttack()
        // {
        //     BattleFightManager.Instance.EnemyAttack();
        // }
        

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
            BattleManager.Instance.RefreshEnemyAttackData();
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
            
            // foreach (var buffID in drEnemy.SecondaryBuffs)
            // {
            //     var buffData = BattleBuffManager.Instance.GetBuffData(buffID);
            //     buffDatas.Add(buffData);
            // }

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
        
        // public List<string> GetSecondaryBuff(int monsterID)
        // {
        //     var drEnemy = RoundHero.GameEntry.DataTable.GetEnemy(monsterID);
        //     
        //     var buffStrs = new List<string>();
        //
        //     foreach (var buffStr in drEnemy.SecondaryBuffs)
        //     {
        //         buffStrs.Add(buffStr);
        //     }
        //
        //     return buffStrs;
        // }
        
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
            
            // foreach (var buffID in drEnemy.SecondaryBuffs)
            // {
            //     //var values = new List<float>();
            //     foreach (var value in drEnemy.SecondaryValues)
            //     {
            //         buffValuelist[idx][idx2++] = BattleBuffManager.Instance.GetBuffValue(value);
            //     }
            //
            //     idx++;
            //     idx2 = 0;
            //     //valuelist.Add(values);
            // }

            return buffValuelist;
        }

        // public List<List<float>> GetSecondaryBuffValues(int monsterID)
        // {
        //     var drEnemy = RoundHero.GameEntry.DataTable.GetEnemy(monsterID);
        //     
        //     var valuelist = new List<List<float>>();
        //
        //     //var idx = 2;
        //     foreach (var buffID in drEnemy.SecondaryBuffs)
        //     {
        //         var values = new List<float>();
        //         foreach (var value in drEnemy.SecondaryValues)
        //         {
        //             values.Add(BattleBuffManager.Instance.GetBuffValue(value));
        //         }
        //         valuelist.Add(values);
        //     }
        //
        //     return valuelist;
        // }

    }
}
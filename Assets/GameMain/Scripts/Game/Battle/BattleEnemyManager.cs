
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
            Subscribe();
            
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
            //Log.Debug("InitGenerateRole:" + randomSeed);
            var random = new Random(randomSeed);

            var rule = GameEntry.DataTable.GetEnemyGenerateRule(BattleManager.Instance.BattleData.GameDifficulty,
                GamePlayManager.Instance.GamePlayData.BattleModeProduce.Session);
            
            // var rules = Constant.Enemy.EnemyGenerateRules[BattleManager.Instance.BattleData.GameDifficulty];
            // var rule = rules[GamePlayManager.Instance.GamePlayData.BattleModeProduce.Session];
            //EnemyGenerateData.RoundGenerateUnitCount = new Dictionary<int, int>(rule.RoundGenerateUnitCount);
            var roundGenerateUnitCounts = rule.RoundGenerateUnitCount.Split(";");
            string[] strArray;
            foreach (var str in roundGenerateUnitCounts)
            {
                strArray = str.Split(",");
                EnemyGenerateData.RoundGenerateUnitCount.Add(int.Parse(strArray[0]), int.Parse(strArray[1]));
            }
            
            var enemyTypeCounts = new List<int>(10);
            strArray = rule.EnemyTypeCounts.Split(";");
            foreach (var str in strArray)
            {
                enemyTypeCounts.Add(int.Parse(str));
            }
            
            var enemyLevelCounts = new List<int>(10);
            strArray = rule.EnemyLevelCounts.Split(";");
            foreach (var str in strArray)
            {
                enemyLevelCounts.Add(int.Parse(str));
            }
            
            
            var level0UnitList = GameEntry.DataTable.GetEnemys(0);
            var level0UnitTypeRandoms = MathUtility.GetRandomNum(enemyTypeCounts[0], 0, level0UnitList.Count,
                new Random(random.Next()));
            var level0UnitRandoms = MathUtility.GetRandomNum(enemyLevelCounts[0], 0, level0UnitTypeRandoms.Count,
                new Random(random.Next()), true);
            
            var level1UnitList = GameEntry.DataTable.GetEnemys(1);
            var level1UnitTypeRandoms = MathUtility.GetRandomNum(enemyTypeCounts[1], 0, level1UnitList.Count,
                new Random(random.Next()));
            var level1UnitRandoms = MathUtility.GetRandomNum(enemyLevelCounts[1], 0, level1UnitTypeRandoms.Count,
                new Random(random.Next()), true);
            
            var level2UnitList = GameEntry.DataTable.GetEnemys(2);
            var level2UnitTypeRandoms = MathUtility.GetRandomNum(enemyTypeCounts[2], 0, level2UnitList.Count,
                new Random(random.Next()));
            var level2UnitRandoms = MathUtility.GetRandomNum(enemyLevelCounts[2], 0, level2UnitTypeRandoms.Count,
                new Random(random.Next()), true);

            var unitList = new List<int>(level0UnitRandoms.Count + level1UnitRandoms.Count +  + level2UnitRandoms.Count);
            foreach (var idx in level0UnitRandoms)
            {
                unitList.Add(level0UnitList[level0UnitTypeRandoms[idx]].Id);
            }
            
            foreach (var idx in level1UnitRandoms)
            {
                unitList.Add(level1UnitList[level1UnitTypeRandoms[idx]].Id);
            }
            
            foreach (var idx in level2UnitRandoms)
            {
                unitList.Add(level2UnitList[level2UnitTypeRandoms[idx]].Id);
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
            Unsubscribe();
            //GameEntry.Event.Unsubscribe(RefreshUnitDataEventArgs.EventId, OnRefreshUnitData);
        }
        
        public void Subscribe()
        {
            GameEntry.Event.Subscribe(ShowGridDetailEventArgs.EventId, OnShowGridDetail);
        }

        public void Unsubscribe()
        {
            GameEntry.Event.Unsubscribe(ShowGridDetailEventArgs.EventId, OnShowGridDetail);
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
            if (Input.GetKeyDown(KeyCode.Space)) {
                ShowActionSort(true);
            }
            else if (Input.GetKeyUp(KeyCode.Space)) {
                ShowActionSort(false);
            }
            
            if (Input.GetKeyDown(KeyCode.A))
            {
                var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(BattleAreaManager.Instance
                    .CurPointGridPosIdx);

                if (unit is BattleMonsterEntity battleMonsterEntity )
                {
                    battleMonsterEntity.ShowMoveRange(true);
                }
                
            }
            else if (Input.GetKeyUp(KeyCode.A)) {
                BattleAreaManager.Instance.ShowBackupGrids(null);
            } 
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
            
            // var rules = Constant.Enemy.EnemyGenerateRules[BattleManager.Instance.BattleData.GameDifficulty];
            // var rule = rules[GamePlayManager.Instance.GamePlayData.BattleModeProduce.Session];
            
            var rule = GameEntry.DataTable.GetEnemyGenerateRule(BattleManager.Instance.BattleData.GameDifficulty,
                GamePlayManager.Instance.GamePlayData.BattleModeProduce.Session);
            
            var roundGenerateUnitCount = new Dictionary<int, int>();
            var roundGenerateUnitCounts = rule.RoundGenerateUnitCount.Split(";");
            string[] strArray;
            foreach (var str in roundGenerateUnitCounts)
            {
                strArray = str.Split(",");
                roundGenerateUnitCount.Add(int.Parse(strArray[0]), int.Parse(strArray[1]));
            }


            var enemyCurCount = BattleUnitManager.Instance.GetUnitCount(EUnitCamp.Enemy);
            var enemyGenerateCount = 0;
            

            if (EnemyGenerateData.RoundGenerateUnitCount.ContainsKey(BattleManager.Instance.BattleData.Round) &&
                EnemyGenerateData.RoundGenerateUnitCount[BattleManager.Instance.BattleData.Round] >= roundGenerateUnitCount[BattleManager.Instance.BattleData.Round])
            {
                enemyGenerateCount =
                    EnemyGenerateData.RoundGenerateUnitCount[BattleManager.Instance.BattleData.Round];
                EnemyGenerateData.RoundGenerateUnitCount[BattleManager.Instance.BattleData.Round] -=
                    enemyGenerateCount;
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
                        if (idx >= BattleManager.Instance.BattleData.Round)
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
                var enemyID = EnemyGenerateData.UnitList[EnemyGenerateData.UnitIdx++];//Random.Next(0, 3);

                var battleEnemyData = new Data_BattleMonster(BattleUnitManager.Instance.GetIdx(), enemyID,
                    places[enemyIdxs[i]], EUnitCamp.Enemy, new List<int>(), BattleManager.Instance.BattleData.Round);
                battleEnemyData.UnitRole = EUnitRole.Staff;
                
                await GenerateEnemy(battleEnemyData);

                
            }
        }


        public async Task<BattleMonsterEntity> GenerateEnemy(Data_BattleMonster battleMonsterData)
        {
            if (BattleManager.Instance.BattleData.GameDifficulty >= EGameDifficulty.Difficulty2)
            {
                battleMonsterData.BaseMaxHP += 1;
                battleMonsterData.CurHP = battleMonsterData.MaxHP;
            }
            
            if (GamePlayManager.Instance.GamePlayData.IsTutorialBattle)
            {
                battleMonsterData.BaseMaxHP = 3;
                battleMonsterData.CurHP = battleMonsterData.MaxHP;
            }
            
            var battleEnemyEntity = await GameEntry.Entity.ShowBattleMonsterEntityAsync(battleMonsterData);
            
            battleEnemyEntity.LookAtHero();
            BattleCurseManager.Instance.AllUnitDodgeSubHeartDamageDict_Add(battleEnemyEntity.BattleMonsterEntityData.BattleMonsterData.Idx);
                
            var enemyGenerateAddDebuff = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.EnemyGenerateAddDebuff, BattleManager.Instance.CurUnitCamp);
            if (enemyGenerateAddDebuff != null )
            {
                var randomDebuffIdx = Random.Next(0, Constant.Battle.EffectUnitStates[EUnitStateEffectType.DeBuff].Count);
                var randomDeBuff = Constant.Battle.EffectUnitStates[EUnitStateEffectType.DeBuff][randomDebuffIdx];
                battleEnemyEntity.BattleMonsterEntityData.BattleMonsterData.ChangeState(randomDeBuff);
            }
                
            BattleUnitManager.Instance.BattleUnitDatas.Add(battleMonsterData.Idx, battleMonsterData);
            BattleUnitManager.Instance.BattleUnitEntities.Add(battleEnemyEntity.BattleMonsterEntityData.BattleMonsterData.Idx, battleEnemyEntity);
            //RefreshEnemyEntities();
                
            if (battleEnemyEntity is IMoveGrid moveGrid)
            {
                BattleAreaManager.Instance.MoveGrids.Add(battleEnemyEntity.BattleMonsterEntityData.Id, moveGrid);
            }

            return battleEnemyEntity;
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

        
        public DREnemy GetEnemyTable(int enemyIdx)
        {
            var monsterEntity = BattleUnitManager.Instance.GetUnitByIdx(enemyIdx) as BattleMonsterEntity;
            if (monsterEntity == null)
                return null;

            return GameEntry.DataTable.GetEnemy(monsterEntity.BattleMonsterEntityData.BattleMonsterData.MonsterID);

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
            //BattleManager.Instance.RefreshEnemyAttackData();
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
        
        
        
        public List<BuffData> GetBuffData(int unitIdx)
        {
            var buffDatas = new List<BuffData>();
            var unitEntity = BattleUnitManager.Instance.GetUnitByIdx(unitIdx) as BattleMonsterEntity;
            if (unitEntity == null)
                return buffDatas;
            
            
            var drEnemy =
                GameEntry.DataTable.GetEnemy(unitEntity.BattleMonsterEntityData.BattleMonsterData.MonsterID);

            foreach (var buffID in drEnemy.OwnBuffs)
            {
                var buffData = BattleBuffManager.Instance.GetBuffData(buffID);
                buffData.BuffEquipType = EBuffEquipType.Normal;
                buffDatas.Add(buffData);
            }
            
            foreach (var buffID in drEnemy.SpecBuffs)
            {
                var buffData = BattleBuffManager.Instance.GetBuffData(buffID);
                buffData.BuffEquipType = EBuffEquipType.Special;
                buffDatas.Add(buffData);
            }
            
            foreach (var funeIdx in unitEntity.BattleUnitData.FuneIdxs)
            {
                var drBuff = FuneManager.Instance.GetBuffTable(funeIdx);
                foreach (var buffIDStr in drBuff.BuffIDs)
                {
                    var buffData = BattleBuffManager.Instance.GetBuffData(buffIDStr);
                    buffData.BuffEquipType = EBuffEquipType.Fune;
                    buffDatas.Add(buffData);
                }
                
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
        public List<List<float>> GetBuffValues(int enemyIdx)
        {
            var effectUnit = BattleUnitManager.Instance.GetUnitByIdx(enemyIdx) as BattleMonsterEntity;
            
            var drEnemy = GameEntry.DataTable.GetEnemy(effectUnit.BattleMonsterEntityData.BattleMonsterData.MonsterID);

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
                    buffValuelist[idx][idx2++] = BattleBuffManager.Instance.GetBuffValue(value, enemyIdx);
                    //buffValuelist[idx].Add(BattleBuffManager.Instance.GetBuffValue(value));
                }

                idx++;
                idx2 = 0;
                //valuelist.Add(values);
            }
            
            foreach (var buffID in drEnemy.SpecBuffs)
            {
                //var values = new List<float>();
                foreach (var value in drEnemy.SpecBuffValues)
                {
                    buffValuelist[idx][idx2++] = BattleBuffManager.Instance.GetBuffValue(value, enemyIdx);
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


        public void ShowActionSort(bool isShow) 
        {
            var idx = 1;
            
            
            
            foreach (var kv in BattleFightManager.Instance.RoundFightData.EnemyAttackDatas)
            {
                var unit = BattleUnitManager.Instance.GetUnitByIdx(kv.Key);

                if (isShow)
                {
                    unit.ShowActionSort(idx++);
                }
                else
                {
                    unit.UnShowTags();
                }
            }
        }
        

    }
}
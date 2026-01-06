using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    public class BattleCoreManager : Singleton<BattleCoreManager>
    {

        //private int id;
        
        private int randomSeed;
        public List<int> RandomCaches = new List<int>();
        
        public Dictionary<EUnitCamp, Dictionary<int, BattleCoreEntity>> CoreEntities = new ();
        
        
        
        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            var random = new System.Random(this.randomSeed);
            RandomCaches.Clear();
            for (int i = 0; i < 100; i++)
            {
                RandomCaches.Add(random.Next());
            }
        }

        

        public void Destory()
        {
            CoreEntities.Clear();
        }

        // public int GetID()
        // {
        //     return id++;
        // }

        public bool IsCoreIdx(int idx)
        {
            foreach (var kv in CoreEntities)
            {
                foreach (var kv2 in kv.Value)
                {
                    if (kv2.Value.UnitIdx == idx)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        public async Task Start()
        {
            await GenerateCores();
        }

        public async Task GenerateCores()
        {
            BattleAreaManager.Instance.RefreshObstacles();
            
            var places = BattleAreaManager.Instance.GetPlaces();
            var places2 = new List<int>();
            foreach (var place in places)
            {
                var coord = GameUtility.GridPosIdxToCoord(place);
                if (!(coord.x == 0 || coord.y == 0 || coord.x == Constant.Area.GridSize.x - 1 ||
                    coord.y == Constant.Area.GridSize.y - 1))
                {
                    places2.Add(place);
                }
            }

            List<int> coreGridPosIdxs = null; 

             if (GamePlayManager.Instance.GamePlayData.IsTutorialBattle)
             {
                 coreGridPosIdxs = Constant.Tutorial.Cores;
             }
             else
             {
                 var coreCount = Constant.Battle.CoreCount;
                 if (GamePlayManager.Instance.GamePlayData.PVEType == EPVEType.BattleMode)
                 {
                     if (GamePlayManager.Instance.GamePlayData.BattleModeProduce.Session == 0)
                     {
                         coreCount = 1;
                     }
                     else if (GamePlayManager.Instance.GamePlayData.BattleModeProduce.Session == 1)
                     {
                         coreCount = 2;
                     }
                 }
                 
                 var randomList = MathUtility.GetRandomNum(coreCount                             , 0,
                     places2.Count, new Random(GetRandomSeed()));
                 coreGridPosIdxs = new List<int>();
                 foreach (var idx in randomList)
                 {
                     coreGridPosIdxs.Add(places2[idx]);
                 }
             }

             foreach (var coreGridPosIdx in coreGridPosIdxs)
             {
                 var battleCoreData = new Data_BattleCore(BattleUnitManager.Instance.GetIdx(), 0, coreGridPosIdx,
                     BattleManager.Instance.CurUnitCamp, BattleManager.Instance.BattleData.Round);
                 await GenerateCoreEntity(battleCoreData);
                 //BattleUnitManager.Instance.BattleUnitDatas.Add(coreEntity.BattleCoreEntityData.BattleCoreData.Idx, coreEntity.BattleCoreEntityData.BattleCoreData);

             }
            
            // BattleAreaManager.Instance.RefreshObstacles();
            //
            // var places = BattleAreaManager.Instance.GetPlaces();
            //  var randoms = MathUtility.GetRandomNum(3, 0,
            //      places.Count, Random);
            // //BattleHeroData.GridPosIdx = places[center];
            // BattleHeroData.GridPosIdx = GameUtility.GridCoordToPosIdx(new Vector2Int(3, 3));
            //
            // var heroEntity = await GameEntry.Entity.ShowBattleHeroEntityAsync(BattleHeroData);
            //
            // if (heroEntity is IMoveGrid moveGrid)
            // {
            //     BattleAreaManager.Instance.MoveGrids.Add(heroEntity.BattleHeroEntityData.Id, moveGrid);
            // }
            //
            // BattleUnitManager.Instance.BattleUnitEntities.Add(heroEntity.BattleHeroEntityData.BattleHeroData.Idx, heroEntity);
            // //PlayerManager.Instance.GetPlayerID(BattleManager.Instance.CurUnitCamp)
            // HeroEntities.Add(BattleManager.Instance.CurUnitCamp, heroEntity);
            
            
        }

        public async Task<BattleCoreEntity> GenerateCoreEntity(Data_BattleCore battleCoreData)
        {
            var coreEntity = await GameEntry.Entity.ShowBattleCoreEntityAsync(battleCoreData);
            if (coreEntity is IMoveGrid moveGrid)
            {
                BattleAreaManager.Instance.MoveGrids.Add(coreEntity.BattleCoreEntityData.Id, moveGrid);
            }
            
            BattleUnitManager.Instance.BattleUnitDatas.Add(battleCoreData.Idx, battleCoreData);
            BattleUnitManager.Instance.BattleUnitEntities.Add(coreEntity.BattleCoreEntityData.BattleCoreData.Idx, coreEntity);
            //PlayerManager.Instance.GetPlayerID(BattleManager.Instance.CurUnitCamp)
            if (!CoreEntities.ContainsKey(BattlePlayerManager.Instance.PlayerData.UnitCamp))
            {
                CoreEntities.Add(BattlePlayerManager.Instance.PlayerData.UnitCamp, new Dictionary<int, BattleCoreEntity>());
            }
            CoreEntities[BattlePlayerManager.Instance.PlayerData.UnitCamp].Add(coreEntity.BattleCoreEntityData.BattleCoreData.Idx, coreEntity);
            
            return coreEntity;
        }
        
        public int GetRandomSeed()
        {
            return RandomCaches[RandomIdx++ % RandomCaches.Count];
        }
        
        public int RandomIdx
        {
            get
            {
                return BattleManager.Instance.BattleData.CoreRandomIdx;
            }

            set
            {
                BattleManager.Instance.BattleData.CoreRandomIdx = value;
            }
        }
    }
}
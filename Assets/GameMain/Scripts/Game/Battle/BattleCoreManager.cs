using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    public class BattleCoreManager : Singleton<BattleCoreManager>
    {

        //private int id;
        public Random Random;
        private int randomSeed;
        
        public Dictionary<EUnitCamp, Dictionary<int, BattleCoreEntity>> CoreEntities = new ();
        
        
        
        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
            
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

        public async Task GenerateCores()
        {
            BattleAreaManager.Instance.RefreshObstacles();
            
            var places = BattleAreaManager.Instance.GetPlaces();
             var randomList = MathUtility.GetRandomNum(3, 0,
                 places.Count, Random);

             foreach (var randomIdx in randomList)
             {
                 var coreEntity = await GameEntry.Entity.ShowBattleCoreEntityAsync(0, places[randomIdx], BattleManager.Instance.CurUnitCamp);
                 if (coreEntity is IMoveGrid moveGrid)
                 {
                     BattleAreaManager.Instance.MoveGrids.Add(coreEntity.BattleCoreEntityData.Id, moveGrid);
                 }
                 
                 BattleUnitManager.Instance.BattleUnitEntities.Add(coreEntity.BattleCoreEntityData.BattleCoreData.Idx, coreEntity);
                 //PlayerManager.Instance.GetPlayerID(BattleManager.Instance.CurUnitCamp)
                 if (!CoreEntities.ContainsKey(BattleManager.Instance.CurUnitCamp))
                 {
                     CoreEntities.Add(BattleManager.Instance.CurUnitCamp, new Dictionary<int, BattleCoreEntity>());
                 }
                 CoreEntities[BattleManager.Instance.CurUnitCamp].Add(coreEntity.BattleCoreEntityData.BattleCoreData.Idx, coreEntity);
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

        
        

    }
}
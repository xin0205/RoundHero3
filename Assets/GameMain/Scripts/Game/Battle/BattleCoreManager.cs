using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using Random = System.Random;

namespace RoundHero
{
    public class BattleCoreManager : Singleton<HeroManager>
    {
        


        //private int id;
        public Random Random;
        private int randomSeed;
        
        public Dictionary<EUnitCamp, BattleCoreEntity> CoreEntities = new ();
        
        
        
        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);
            //HeroEntities.Clear();
            BattleUnitManager.Instance.BattleUnitDatas.Add(HeroManager.Instance.BattleHeroData.Idx, HeroManager.Instance.BattleHeroData);
            
            //id = 0;
        }

        

        public void Destory()
        {
            //HeroEntities.Clear();
        }

        // public int GetID()
        // {
        //     return id++;
        // }

        public void InitHeroData(EHeroID heroID)
        {
            // BattleHeroData = new Data_BattleHero(BattleUnitManager.Instance.GetIdx(),
            //     heroID, 0, BattleManager.Instance.CurUnitCamp, new List<int>());
            // BattleHeroData.UnitRole = EUnitRole.Hero;
            
        }

        public async Task GenerateCores()
        {
            BattleAreaManager.Instance.RefreshObstacles();
            
            var places = BattleAreaManager.Instance.GetPlaces();
             var randoms = MathUtility.GetRandomNum(3, 0,
                 places.Count, Random);
            
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
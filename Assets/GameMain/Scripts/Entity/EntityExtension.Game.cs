using System.Collections.Generic;
using System.Threading.Tasks;

using GameFramework;
using RPGCharacterAnims.Lookups;
using UGFExtensions.Await;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public static partial class EntityExtension
    {
        public static void ShowGridEntity(this EntityComponent entityComponent, int posIdx, EGridType gridType)
        {
            var data = ReferencePool.Acquire<BattleGridEntityData>();
            var pos = GameUtility.GridPosIdxToPos(posIdx);
            data.Init(entityComponent.GenerateSerialId(), pos, posIdx, gridType);
            GameEntry.Entity.ShowEntity(data.Id, typeof(BattleGridEntity),
                AssetUtility.GetGridPrefab(), Constant.EntityGroup.Grid, 0, data);
        }
        
        public static void ShowBattleCardEntity(this EntityComponent entityComponent, int cardID)
        {
            var data = ReferencePool.Acquire<BattleCardEntityData>();
            
            data.Init(entityComponent.GenerateSerialId(), Vector2.zero, cardID);
            GameEntry.Entity.ShowEntity(data.Id, typeof(BattleCardEntity),
                AssetUtility.GetBattleCardPrefab(), Constant.EntityGroup.Card, 0, data);
        }
        
        public static async Task<SceneEntity> ShowSceneEntityAsync(this EntityComponent entityComponent, string sceneName)
        {
            var data = ReferencePool.Acquire<EntityData>();
            data.Init(entityComponent.GenerateSerialId(), Vector2.zero);
            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(SceneEntity),
                AssetUtility.GetScenePrefab(sceneName), Constant.EntityGroup.Scene, 0, data);
            
            return (SceneEntity)task.Logic;
        }
        
        public static async Task<BattleGridEntity> ShowGridEntityAsync(this EntityComponent entityComponent, int gridPosIdx, EGridType gridType)
        {
            var data = ReferencePool.Acquire<BattleGridEntityData>();
            var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
            data.Init(entityComponent.GenerateSerialId(), pos, gridPosIdx, gridType);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleGridEntity),
                AssetUtility.GetGridPrefab(), Constant.EntityGroup.Grid, 0, data);
            
            return (BattleGridEntity)task.Logic;
        }
        
        public static async Task<BattleCardEntity> ShowBattleCardEntityAsync(this EntityComponent entityComponent, int cardIdx, int handSortingIdx = 0)
        {
            var data = ReferencePool.Acquire<BattleCardEntityData>();
            
            data.Init(entityComponent.GenerateSerialId(), Vector2.zero, cardIdx, handSortingIdx);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleCardEntity),
                AssetUtility.GetBattleCardPrefab(), Constant.EntityGroup.Card, 0, data);
            
            return (BattleCardEntity)task.Logic;
        }

        public static async Task<BattleMonsterEntity> ShowBattleMonsterEntityAsync(this EntityComponent entityComponent,
            int monsterID, int enemyTypeID, int gridPosIdx, EUnitCamp unitCamp, List<int> funeIDs)
        {
            var data = ReferencePool.Acquire<BattleMonsterEntityData>();
            var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
            var battleEnemyData = new Data_BattleMonster(BattleUnitManager.Instance.GetID(), monsterID, gridPosIdx, unitCamp, funeIDs);
            battleEnemyData.UnitRole = EUnitRole.Staff;
            //battleEnemyData.ChangeState(EUnitState.HurtRoundStart);
            BattleUnitManager.Instance.BattleUnitDatas.Add(battleEnemyData.ID, battleEnemyData);
            data.Init(entityComponent.GenerateSerialId(), pos, battleEnemyData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleMonsterEntity),
                AssetUtility.GetBattleEnemyPrefab(enemyTypeID), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleMonsterEntity)task.Logic;
        }


        public static async Task<BattleHeroEntity> ShowBattleHeroEntityAsync(this EntityComponent entityComponent,
            Data_BattleHero battleHeroData)
        {

            var data = ReferencePool.Acquire<BattleHeroEntityData>();
            var pos = GameUtility.GridPosIdxToPos(battleHeroData.GridPosIdx);
            
            data.Init(entityComponent.GenerateSerialId(), pos, HeroManager.Instance.BattleHeroData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleHeroEntity),
                AssetUtility.GetBattleHeroPrefab(battleHeroData.ID), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleHeroEntity)task.Logic;
        }
        
        public static async Task<BattleRouteEntity> ShowBattleRouteEntityAsync(this EntityComponent entityComponent, List<int> gridPosIdxs, int routeIdx)
        {
            var data = ReferencePool.Acquire<BattleRouteEntityData>();

            data.Init(entityComponent.GenerateSerialId(), Vector3.zero, gridPosIdxs, routeIdx);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleRouteEntity),
                AssetUtility.GetBattleRoutePrefab(), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleRouteEntity)task.Logic;
        }
        
        public static async Task<BattleSoliderEntity> ShowBattleSoliderEntityAsync(this EntityComponent entityComponent, int cardID, int gridPosIdx, EUnitCamp unitCamp, List<int> funeIDs)
        {
            var data = ReferencePool.Acquire<BattleSoliderEntityData>();
            var card = BattleManager.Instance.GetCard(cardID);
            var cardEnergy =
                BattleCardManager.Instance.GetCardEnergy(cardID);
            var battleSoliderData = new Data_BattleSolider(BattleUnitManager.Instance.GetID(), cardID, gridPosIdx, cardEnergy, unitCamp, funeIDs);
            battleSoliderData.UnitRole = EUnitRole.Staff;
            
            BattleUnitManager.Instance.BattleUnitDatas.Add(battleSoliderData.ID, battleSoliderData);

            var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
            data.Init(entityComponent.GenerateSerialId(), pos, battleSoliderData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleSoliderEntity),
                AssetUtility.GetBattleSoliderPrefab(card.CardID), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleSoliderEntity)task.Logic;
        }
        
        public static async Task<BattleSoliderEntity> ShowBattleSoliderEntityAsync(this EntityComponent entityComponent, Data_BattleSolider battleSoliderData)
        {
            var data = ReferencePool.Acquire<BattleSoliderEntityData>();
            
            // var cardEnergy =
            //     BattleCardManager.Instance.GetCardEnergy(cardID);
            var newBattleSoliderData = battleSoliderData.Copy();
            var card = BattleManager.Instance.GetCard(newBattleSoliderData.CardIdx);
            newBattleSoliderData.UnitRole = EUnitRole.Staff;
            newBattleSoliderData.ID = BattleUnitManager.Instance.GetID();
            
            
            BattleUnitManager.Instance.BattleUnitDatas.Add(newBattleSoliderData.ID, newBattleSoliderData);
            //BattleUnitStateManager.Instance.AddActiveAttack(newBattleSoliderData);

            var pos = GameUtility.GridPosIdxToPos(newBattleSoliderData.GridPosIdx);
            data.Init(entityComponent.GenerateSerialId(), pos, newBattleSoliderData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleSoliderEntity),
                AssetUtility.GetBattleSoliderPrefab(card.CardID), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleSoliderEntity)task.Logic;
        }
        
        public static async Task<GridPropEntity> ShowBattleGridPropEntityAsync(this EntityComponent entityComponent,
            int gridPropID, int gridPosIdx)
        {
            var data = ReferencePool.Acquire<GridPropEntityData>();
            var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
            var gridPropData = new Data_GridProp(gridPropID, BattleUnitManager.Instance.GetID(), gridPosIdx, EUnitCamp.Third);

            BattleGridPropManager.Instance.GridPropDatas.Add(gridPropData.ID, gridPropData);
            data.Init(entityComponent.GenerateSerialId(), pos, gridPropData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(GridPropEntity),
                AssetUtility.GetGridPropPrefab(gridPropID), Constant.EntityGroup.Unit, 0, data);
            
            return (GridPropEntity)task.Logic;
        }
        
        public static async Task<GridPropMoveDirectEntity> ShowGridPropMoveDirectEntityAsync(this EntityComponent entityComponent,
            int gridPropID, ERelativePos direct, int gridPosIdx)
        {
            var data = ReferencePool.Acquire<GridPropMoveDirectEntityData>();
            var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
            var gridPropData = new Data_GridPropMoveDirect(gridPropID, direct, BattleUnitManager.Instance.GetID(), gridPosIdx, EUnitCamp.Third);

            BattleGridPropManager.Instance.GridPropDatas.Add(gridPropData.ID, gridPropData);
            data.Init(entityComponent.GenerateSerialId(), pos, gridPropData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(GridPropMoveDirectEntity),
                AssetUtility.GetGridPropPrefab(gridPropID), Constant.EntityGroup.Unit, 0, data);
            
            return (GridPropMoveDirectEntity)task.Logic;
        }
        
        public static async Task<HeroSceneEntity> ShowHeroSceneEntityAsync(this EntityComponent entityComponent)
        {
            var data = ReferencePool.Acquire<EntityData>();
            data.Init(entityComponent.GenerateSerialId(), Vector2.zero);
            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(HeroSceneEntity),
                AssetUtility.GetScenePrefab("HeroScene"), Constant.EntityGroup.Scene, 0, data);
            
            return (HeroSceneEntity)task.Logic;
        }
        
        public static async Task<DisplayHeroEntity> ShowDisplayHeroEntityAsync(this EntityComponent entityComponent, int heroID)
        {
            var data = ReferencePool.Acquire<EntityData>();
            data.Init(entityComponent.GenerateSerialId(), Vector2.zero);
            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(DisplayHeroEntity),
                AssetUtility.GetDisplayHeroPrefab(heroID), Constant.EntityGroup.Unit, 0, data);
            
            return (DisplayHeroEntity)task.Logic;
        }
        
        public static async Task<BattleWeaponEntity> ShowWeaponEntityAsync(this EntityComponent entityComponent, EWeaponHoldingType weaponHoldingType, EWeaponType weaponType, int weaponID)
        {
            var data = ReferencePool.Acquire<BattleWeaponEntityData>();
            data.Init(entityComponent.GenerateSerialId(), weaponHoldingType, weaponType, weaponID);
            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleWeaponEntity),
                AssetUtility.GetBattleWeaponPrefab(weaponHoldingType, weaponType, weaponID), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleWeaponEntity)task.Logic;
        }

        public static async Task<BattleHurtEntity> ShowBattleHurtEntityAsync(this EntityComponent entityComponent,
            int gridPosIdx, int hurt)
        {
            var data = ReferencePool.Acquire<BattleHurtEntityData>();
            var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
            data.Init(entityComponent.GenerateSerialId(), pos + new Vector3(0, 2f, 0), hurt);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleHurtEntity),
                AssetUtility.GetBattleHurtPrefab(), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleHurtEntity)task.Logic;
        }
        
        public static async Task<BattleValueEntity> ShowBattleValueEntityAsync(this EntityComponent entityComponent,
            Vector3 pos, Vector3 targetPos, int value)
        {
            var data = ReferencePool.Acquire<BattleValueEntityData>();

            data.Init(entityComponent.GenerateSerialId(), pos, targetPos, value);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleValueEntity),
                AssetUtility.GetBattleValuePrefab(), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleValueEntity)task.Logic;
        }
        
        public static async Task<EffectEntity> ShowEffectEntityAsync(this EntityComponent entityComponent, string assetName, Vector3 pos)
        {
            var data = ReferencePool.Acquire<EntityData>();
            
            data.Init(entityComponent.GenerateSerialId(), pos);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(EffectEntity),
                AssetUtility.GetEffectPrefab(assetName), Constant.EntityGroup.Unit, 0, data);
            
            return (EffectEntity)task.Logic;
        }
        
        public static async Task<BattleBulletEntity> ShowBattleBulletEntityAsync(this EntityComponent entityComponent, BulletData bulletData, Vector3 shootPos)
        {
            var data = ReferencePool.Acquire<BattleBulletEntityData>();

            data.Init(entityComponent.GenerateSerialId(), shootPos, bulletData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleBulletEntity),
                AssetUtility.GetBattleBulletPrefab(), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleBulletEntity)task.Logic;
        }
    }
}
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
            Data_BattleMonster battleMonsterData)
        {
            var data = ReferencePool.Acquire<BattleMonsterEntityData>();
            var pos = GameUtility.GridPosIdxToPos(battleMonsterData.GridPosIdx);
            //var battleEnemyData = new Data_BattleMonster(BattleUnitManager.Instance.GetIdx(), monsterID, gridPosIdx, unitCamp, funeIDs);
            //battleEnemyData.UnitRole = EUnitRole.Staff;
            
            //battleMonsterData.ChangeState(EUnitState.AtkPassEnemy, 1);
            //battleMonsterData.ChangeState(EUnitState.HurtRoundStart, 1);
            
            data.Init(entityComponent.GenerateSerialId(), pos, battleMonsterData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleMonsterEntity),
                AssetUtility.GetBattleEnemyPrefab(battleMonsterData.MonsterID), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleMonsterEntity)task.Logic;
        }
        
        //int enemyTypeID, 
        // public static async Task<BattleMonsterEntity> ShowBattleMonsterEntityAsync(this EntityComponent entityComponent,
        //     int monsterID, int gridPosIdx, EUnitCamp unitCamp, List<int> funeIDs)
        // {
        //     var data = ReferencePool.Acquire<BattleMonsterEntityData>();
        //     var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
        //     var battleEnemyData = new Data_BattleMonster(BattleUnitManager.Instance.GetIdx(), monsterID, gridPosIdx, unitCamp, funeIDs);
        //     battleEnemyData.UnitRole = EUnitRole.Staff;
        //     //battleEnemyData.ChangeState(EUnitState.HurtRoundStart);
        //     BattleUnitManager.Instance.BattleUnitDatas.Add(battleEnemyData.Idx, battleEnemyData);
        //     data.Init(entityComponent.GenerateSerialId(), pos, battleEnemyData);
        //
        //     var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleMonsterEntity),
        //         AssetUtility.GetBattleEnemyPrefab(monsterID), Constant.EntityGroup.Unit, 0, data);
        //     
        //     return (BattleMonsterEntity)task.Logic;
        // }


        // public static async Task<BattleHeroEntity> ShowBattleHeroEntityAsync(this EntityComponent entityComponent,
        //     Data_BattleHero battleHeroData)
        // {
        //
        //     var data = ReferencePool.Acquire<BattleHeroEntityData>();
        //     var pos = GameUtility.GridPosIdxToPos(battleHeroData.GridPosIdx);
        //     
        //     data.Init(entityComponent.GenerateSerialId(), pos, HeroManager.Instance.BattleHeroData);
        //
        //     var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleHeroEntity),
        //         AssetUtility.GetBattleHeroPrefab(battleHeroData.Idx), Constant.EntityGroup.Unit, 0, data);
        //     
        //     return (BattleHeroEntity)task.Logic;
        // }
        
        public static async Task<BattleCoreEntity> ShowBattleCoreEntityAsync(this EntityComponent entityComponent,
            Data_BattleCore battleCoreData)
        {
            var data = ReferencePool.Acquire<BattleCoreEntityData>();
            var pos = GameUtility.GridPosIdxToPos(battleCoreData.GridPosIdx);
            
            //battleCoreData.UnitStateData.AddState(EUnitState.UnMove, 1, EEffectType.Forever);
            
            data.Init(entityComponent.GenerateSerialId(), pos, battleCoreData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleCoreEntity),
                AssetUtility.GetBattleCorePrefab(battleCoreData.CorID), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleCoreEntity)task.Logic;
        }
        
        // public static async Task<BattleCoreEntity> ShowBattleCoreEntityAsync(this EntityComponent entityComponent,
        //     int coreID, int gridPosIdx, EUnitCamp unitCamp)
        // {
        //     var data = ReferencePool.Acquire<BattleCoreEntityData>();
        //     var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
        //     var battleCoreData = new Data_BattleCore(BattleUnitManager.Instance.GetIdx(), coreID, gridPosIdx, unitCamp);
        //     //battleCoreData.UnitStateData.AddState(EUnitState.UnMove, 1, EEffectType.Forever);
        //     BattleUnitManager.Instance.BattleUnitDatas.Add(battleCoreData.Idx, battleCoreData);
        //     data.Init(entityComponent.GenerateSerialId(), pos, battleCoreData);
        //
        //     var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleCoreEntity),
        //         AssetUtility.GetBattleCorePrefab(coreID), Constant.EntityGroup.Unit, 0, data);
        //     
        //     return (BattleCoreEntity)task.Logic;
        // }
        
        public static async Task<BattleRouteEntity> ShowBattleRouteEntityAsync(this EntityComponent entityComponent, List<int> gridPosIdxs, int entityIdx)
        {
            var data = ReferencePool.Acquire<BattleRouteEntityData>();

            data.Init(entityComponent.GenerateSerialId(), Vector3.zero, gridPosIdxs, entityIdx);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleRouteEntity),
                AssetUtility.GetBattleRoutePrefab(), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleRouteEntity)task.Logic;
        }
        
        // public static async Task<BattleSoliderEntity> ShowBattleSoliderEntityAsync(this EntityComponent entityComponent, int cardID, int gridPosIdx, EUnitCamp unitCamp, List<int> funeIDs)
        // {
        //     var data = ReferencePool.Acquire<BattleSoliderEntityData>();
        //     var card = BattleManager.Instance.GetCard(cardID);
        //     var cardEnergy =
        //         BattleCardManager.Instance.GetCardEnergy(cardID);
        //     var battleSoliderData = new Data_BattleSolider(BattleUnitManager.Instance.GetIdx(), cardID, gridPosIdx, cardEnergy, unitCamp, funeIDs);
        //     battleSoliderData.UnitRole = EUnitRole.Staff;
        //     
        //     BattleUnitManager.Instance.BattleUnitDatas.Add(battleSoliderData.Idx, battleSoliderData);
        //
        //     var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
        //     data.Init(entityComponent.GenerateSerialId(), pos, battleSoliderData);
        //
        //     var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleSoliderEntity),
        //         AssetUtility.GetBattleSoliderPrefab(card.CardID), Constant.EntityGroup.Unit, 0, data);
        //     
        //     return (BattleSoliderEntity)task.Logic;
        // }
        
        public static async Task<BattleSoliderEntity> ShowBattleSoliderEntityAsync(this EntityComponent entityComponent, Data_BattleSolider battleSoliderData)
        {
            var data = ReferencePool.Acquire<BattleSoliderEntityData>();
            

            // var newBattleSoliderData = battleSoliderData.Copy();
            // var card = BattleManager.Instance.GetCard(newBattleSoliderData.CardIdx);
            // newBattleSoliderData.UnitRole = EUnitRole.Staff;
            // newBattleSoliderData.Idx = BattleUnitManager.Instance.GetIdx();
            
            var card = BattleManager.Instance.GetCard(battleSoliderData.CardIdx);
            
            
            //BattleUnitStateManager.Instance.AddActiveAttack(newBattleSoliderData);
            battleSoliderData.ChangeState(EUnitState.AtkPassEnemy, 1);
            var pos = GameUtility.GridPosIdxToPos(battleSoliderData.GridPosIdx);
            data.Init(entityComponent.GenerateSerialId(), pos, battleSoliderData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleSoliderEntity),
                AssetUtility.GetBattleSoliderPrefab(card.CardID), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleSoliderEntity)task.Logic;
        }
        
        public static async Task<GridPropEntity> ShowBattleGridPropEntityAsync(this EntityComponent entityComponent,
            Data_GridProp gridPropData)
        {
            var data = ReferencePool.Acquire<GridPropEntityData>();
            var pos = GameUtility.GridPosIdxToPos(gridPropData.GridPosIdx);
            //var gridPropData = new Data_GridProp(gridPropID, BattleUnitManager.Instance.GetIdx(), gridPosIdx, EUnitCamp.Third);

            //BattleGridPropManager.Instance.GridPropDatas.Add(gridPropData.Idx, gridPropData);
            data.Init(entityComponent.GenerateSerialId(), pos, gridPropData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(GridPropEntity),
                AssetUtility.GetGridPropPrefab(gridPropData.GridPropID), Constant.EntityGroup.Unit, 0, data);
            
            return (GridPropEntity)task.Logic;
        }
        
        // public static async Task<GridPropMoveDirectEntity> ShowGridPropMoveDirectEntityAsync(this EntityComponent entityComponent,
        //     int gridPropID, ERelativePos direct, int gridPosIdx)
        // {
        //     var data = ReferencePool.Acquire<GridPropMoveDirectEntityData>();
        //     var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
        //     var gridPropData = new Data_GridPropMoveDirect(gridPropID, direct, BattleUnitManager.Instance.GetIdx(), gridPosIdx, EUnitCamp.Third);
        //
        //     BattleGridPropManager.Instance.GridPropDatas.Add(gridPropData.Idx, gridPropData);
        //     data.Init(entityComponent.GenerateSerialId(), pos, gridPropData);
        //
        //     var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(GridPropMoveDirectEntity),
        //         AssetUtility.GetGridPropPrefab(gridPropID), Constant.EntityGroup.Unit, 0, data);
        //     
        //     return (GridPropMoveDirectEntity)task.Logic;
        // }
        
        public static async Task<GridPropObstacleEntity> ShowGridPropObstacleEntityAsync(this EntityComponent entityComponent,
            int gridPropID, int gridPosIdx)
        {
            var data = ReferencePool.Acquire<GridPropEntityData>();
            var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
            var gridPropData = new Data_GridProp(gridPropID, BattleUnitManager.Instance.GetIdx(), gridPosIdx, EUnitCamp.Third);

            
            data.Init(entityComponent.GenerateSerialId(), pos, gridPropData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(GridPropObstacleEntity),
                AssetUtility.GetGridPropPrefab(gridPropID), Constant.EntityGroup.Unit, 0, data);
            
            return (GridPropObstacleEntity)task.Logic;
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

        // public static async Task<BattleHurtEntity> ShowBattleHurtEntityAsync(this EntityComponent entityComponent,
        //     int gridPosIdx, int hurt)
        // {
        //     var data = ReferencePool.Acquire<BattleHurtEntityData>();
        //     var pos = GameUtility.GridPosIdxToPos(gridPosIdx);
        //     data.Init(entityComponent.GenerateSerialId(), pos + new Vector3(0, 2f, 0), hurt);
        //
        //     var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleHurtEntity),
        //         AssetUtility.GetBattleHurtPrefab(), Constant.EntityGroup.Unit, 0, data);
        //     
        //     return (BattleHurtEntity)task.Logic;
        // }

        public static async Task<BattleMoveValueEntity> ShowBattleMoveValueEntityAsync(
            this EntityComponent entityComponent,
            int startValue, int endValue, int entityIdx = -1, bool isLoop = false, bool isAdd = false,
            MoveParams moveParams = null, MoveParams targetMoveParams = null)
        {
            var data = ReferencePool.Acquire<BattleMoveValueEntityData>();

            data.Init(entityComponent.GenerateSerialId(), startValue, endValue, entityIdx, isLoop, isAdd, moveParams,
                targetMoveParams);

            //Log.Debug("task1");
            return await ShowBattleMoveValueEntityAsync(data);
        }
        
        public static async Task<BattleMoveValueEntity> ShowBattleMoveValueEntityAsync(BattleMoveValueEntityData battleMoveValueEntityData)
        {
            //Log.Debug("task1");
            var task = await GameEntry.Entity.ShowEntityAsync(battleMoveValueEntityData.Id, typeof(BattleMoveValueEntity),
                AssetUtility.GetBattleMoveValuePrefab(), Constant.EntityGroup.Unit, 0, battleMoveValueEntityData);
            //Log.Debug("task2:" + ((BattleMoveValueEntity)task.Logic).Id);
            return (BattleMoveValueEntity)task.Logic;
        }
        
        public static async Task<BattleMoveValueEntity> ShowBattleBlessMoveValueEntityAsync(
            this EntityComponent entityComponent,
            int startValue, int endValue, EBlessID blessID, int entityIdx = -1, bool isLoop = false, bool isAdd = false,
            MoveParams moveParams = null, MoveParams targetMoveParams = null)
        {
            var data = ReferencePool.Acquire<BlessIconValueEntityData>();

            data.Init(entityComponent.GenerateSerialId(), startValue, endValue, blessID, entityIdx, isLoop, isAdd, moveParams,
                targetMoveParams);

            return await ShowBattleMoveValueEntityAsync(data);
        }

        
        public static async Task<BattleMoveIconEntity> ShowBattleMoveIconEntityAsync(this EntityComponent entityComponent,
            EUnitState unitState, int value, int entityIdx = -1, bool isLoop = false, MoveParams moveParams = null, MoveParams targetMoveParams = null)
        {
            var data = ReferencePool.Acquire<BattleMoveIconEntityData>();

            data.Init(entityComponent.GenerateSerialId(), unitState, value, entityIdx, isLoop, moveParams,
                targetMoveParams);

            //Log.Debug("task1");
            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleMoveIconEntity),
                AssetUtility.GetBattleMoveIconPrefab(), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleMoveIconEntity)task.Logic;
        }
        
        public static async Task<EffectEntity> ShowEffectEntityAsync(this EntityComponent entityComponent, string assetName, Vector3 pos)
        {
            var data = ReferencePool.Acquire<EntityData>();
            
            data.Init(entityComponent.GenerateSerialId(), pos);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(EffectEntity),
                AssetUtility.GetEffectPrefab(assetName), Constant.EntityGroup.Unit, 0, data);
            
            return (EffectEntity)task.Logic;
        }
        
        public static async Task<CommonEffectEntity> ShowCommonEffectEntityAsync(this EntityComponent entityComponent, string assetName, Vector3 pos, EColor color)
        {
            var data = ReferencePool.Acquire<CommonEffectEntityData>();
            
            data.Init(entityComponent.GenerateSerialId(), pos, color);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(CommonEffectEntity),
                AssetUtility.GetEffectPrefab(assetName), Constant.EntityGroup.Unit, 0, data);
            
            return (CommonEffectEntity)task.Logic;
        }
        
        public static async Task<LineMultiEffectEntity> ShowLineMultiEffectEntityAsync(this EntityComponent entityComponent, string assetName, Vector3 pos, EColor color, List<int> gridPosIdxs)
        {
            var data = ReferencePool.Acquire<LineMultiEffectEntityData>();
            
            data.Init(entityComponent.GenerateSerialId(), pos, color, gridPosIdxs);
        
            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(LineMultiEffectEntity),
                AssetUtility.GetEffectPrefab(assetName), Constant.EntityGroup.Unit, 0, data);
            
            return (LineMultiEffectEntity)task.Logic;
        }
        
        public static async Task<BattleLineBulletEntity> ShowBattleLineBulletEntityAsync(this EntityComponent entityComponent, BulletData bulletData, Vector3 shootPos)
        {
            var data = ReferencePool.Acquire<BattleBulletEntityData>();

            data.Init(entityComponent.GenerateSerialId(), shootPos, bulletData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleLineBulletEntity),
                AssetUtility.GetBattleLineBulletPrefab(), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleLineBulletEntity)task.Logic;
        }
        
        public static async Task<BattleBeamBulletEntity> ShowBattleBeamBulletEntityAsync(this EntityComponent entityComponent, BulletData bulletData, Vector3 shootPos)
        {
            var data = ReferencePool.Acquire<BattleBulletEntityData>();

            data.Init(entityComponent.GenerateSerialId(), shootPos, bulletData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleBeamBulletEntity),
                AssetUtility.GetBattleBeamBulletPrefab(), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleBeamBulletEntity)task.Logic;
        }
        
        public static async Task<BattleParabolaBulletEntity> ShowBattleParabolaBulletEntityAsync(this EntityComponent entityComponent, BulletData bulletData, Vector3 shootPos)
        {
            var data = ReferencePool.Acquire<BattleBulletEntityData>();

            data.Init(entityComponent.GenerateSerialId(), shootPos, bulletData);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleParabolaBulletEntity),
                AssetUtility.GetBattleParabolaBulletPrefab(), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleParabolaBulletEntity)task.Logic;
        }
        
        public static async Task<BattleDisplayValueEntity> ShowBattleDisplayValueEntityAsync(this EntityComponent entityComponent, Vector3 targetPos, int value, int entityIdx)
        {
            var data = ReferencePool.Acquire<BattleDisplayValueEntityData>();

            data.Init(entityComponent.GenerateSerialId(), targetPos, value, entityIdx);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleDisplayValueEntity),
                AssetUtility.GetBattleDisplayValuePrefab(), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleDisplayValueEntity)task.Logic;
        }
        
        public static async Task<BattleValueEntity> ShowBattleValueEntityAsync(this EntityComponent entityComponent, Vector3 targetPos, int value, int entityIdx)
        {
            var data = ReferencePool.Acquire<BattleValueEntityData>();

            data.Init(entityComponent.GenerateSerialId(), targetPos, value, entityIdx);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleValueEntity),
                AssetUtility.GetBattleValuePrefab(), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleValueEntity)task.Logic;
        }
        
        public static async Task<BattleAttackTagEntity> ShowBattleAttackTagEntityAsync(this EntityComponent entityComponent, Vector3 pos, Vector3 startPos, Vector3 targetPos, EAttackTagType attackTagType,
            EUnitState unitState, BuffValue buffValue, int entityIdx = -1, bool showAttackLine = true, bool showAttackPos = true)
        {
            var data = ReferencePool.Acquire<BattleAttackTagEntityData>();

            data.Init(entityComponent.GenerateSerialId(), pos, startPos, targetPos, attackTagType, unitState, buffValue, entityIdx, showAttackLine, showAttackPos);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleAttackTagEntity),
                AssetUtility.GetBattleAttackTagPrefab(), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleAttackTagEntity)task.Logic;
        }
        
        public static async Task<BattleFlyDirectEntity> ShowBattleFlyDirectEntityAsync(this EntityComponent entityComponent, int gridPosIdx, ERelativePos direct, int entityIdx)
        {
            var data = ReferencePool.Acquire<BattleFlyDirectEntityData>();

            data.Init(entityComponent.GenerateSerialId(), gridPosIdx, direct, entityIdx);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleFlyDirectEntity),
                AssetUtility.GetBattleFlyDirectEntityPrefab(), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleFlyDirectEntity)task.Logic;
        }
        
        public static async Task<BattleIconEntity> ShowBattleIconEntityAsync(this EntityComponent entityComponent, Vector3 pos, EBattleIconType  battleIconType, int entityIdx)
        {
            var data = ReferencePool.Acquire<BattleIconEntityData>();
            
            var uiPos = PositionConvert.WorldPointToUILocalPoint(
                AreaController.Instance.BattleFormRoot.GetComponent<RectTransform>(), pos);
            
            data.Init(entityComponent.GenerateSerialId(), uiPos, battleIconType, entityIdx);

            var task = await GameEntry.Entity.ShowEntityAsync(data.Id, typeof(BattleIconEntity),
                AssetUtility.GetBattleIconEntityPrefab(), Constant.EntityGroup.Unit, 0, data);
            
            return (BattleIconEntity)task.Logic;
        }
        
        
    }
}
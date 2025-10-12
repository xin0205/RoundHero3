using System;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

using UnityEngine.UI;

namespace RoundHero
{
    [Serializable]
    public class UnitDescFormData
    {
        public EUnitRole UnitRole;
        public EUnitCamp UnitCamp;
        public int Idx;
        public EGridType GridType;
        public int EntityID = 0;
    }
    
    public class UnitDescForm : UGuiForm
    {
        public UnitDescFormData UnitDescFormData;

        [SerializeField]
        private GameObject root;

        [SerializeField] private VideoPlayItem videoPlayItem;
        
        [SerializeField] private PlayerCardItem playerCardItem;
        [SerializeField] private UnitDescItem unitDescItem;
        [SerializeField] private GridDescItem gridDescItem;
        [SerializeField] private GameObject unitBattleData;
        
        [SerializeField] private Text actionTimeStr;
        
        [SerializeField] private List<PlayerCommonItem> funeList = new List<PlayerCommonItem>();
        [SerializeField] private List<PlayerCommonItem> unitStateList = new List<PlayerCommonItem>();
        [SerializeField] private List<UnitStateIconItem> unitStateIconList = new List<UnitStateIconItem>();
        
        
        [SerializeField]
        private GameObject funeListGO;
        [SerializeField]
        private GameObject unitStateListGO;
        [SerializeField]
        private GameObject unitStateIconListGO;
        [SerializeField]
        private GameObject keyshortcutGO;

        private bool hasDetail;

        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            UnitDescFormData = (UnitDescFormData)userData;
            
            // if(BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) == null)
            //     return;
            hasDetail = false;

            var animationPlayData = new AnimationPlayData();
            if (UnitDescFormData.UnitCamp == EUnitCamp.Enemy)
            {
                var enemyEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleMonsterEntity;
                animationPlayData.GifType = EGIFType.Enemy;
                var drEnemy =
                    GameEntry.DataTable.GetEnemy(enemyEntity.BattleMonsterEntityData.BattleMonsterData.MonsterID);
                animationPlayData.ID = drEnemy.GIFIdx;

            }
            else if (UnitDescFormData.UnitCamp == EUnitCamp.Player1 || UnitDescFormData.UnitCamp == EUnitCamp.Player2)
            {
                if (UnitDescFormData.UnitRole == EUnitRole.Core)
                {
                    animationPlayData.GifType = EGIFType.Hero;
                }
                else if (UnitDescFormData.UnitRole == EUnitRole.Staff)
                {
                    var unitEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleSoliderEntity;
                    //
                    //var cardData = CardManager.Instance.GetCard(unitEntity.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                    var drCard = CardManager.Instance.GetCardTable(unitEntity.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                    //
                    animationPlayData.ID = drCard.GIFIdx;
                    animationPlayData.GifType = EGIFType.Solider;
                }
            }
            
            
            videoPlayItem.gameObject.SetActive(false);
            unitDescItem.gameObject.SetActive(false);
            playerCardItem.gameObject.SetActive(false);
            unitBattleData.SetActive(false);
            gridDescItem.gameObject.SetActive(false);
            foreach (var playerCommonItem in funeList)
            {
                playerCommonItem.gameObject.SetActive(false);
            }
            
            if (UnitDescFormData.UnitCamp == EUnitCamp.Enemy)
            {
                unitDescItem.gameObject.SetActive(true);
                unitBattleData.SetActive(true);
                videoPlayItem.gameObject.SetActive(true);
                videoPlayItem.SetVideo(animationPlayData);
                
                var name = "";
                var desc = "";
                
                var unitEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleMonsterEntity;
 
                GameUtility.GetEnemyText(unitEntity.BattleMonsterEntityData.BattleMonsterData.MonsterID, ref name,
                    ref desc);
                
                var enemyEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleMonsterEntity;
                var power = enemyEntity.BattleMonsterEntityData.BattleMonsterData.CurHP + "/" +
                            enemyEntity.BattleMonsterEntityData.BattleMonsterData.MaxHP;
                
                unitDescItem.SetDesc(name, power, desc);
                
                var actionTime = (unitEntity.BattleMonsterEntityData.BattleMonsterData.RoundMoveTimes +
                                  unitEntity.BattleMonsterEntityData.BattleMonsterData.RoundAttackTimes);
                
                actionTimeStr.text = GameEntry.Localization.GetLocalizedString(Constant.Localization.UI_ActionTime,
                    actionTime);
                
                
                
                var idx = 0;
                foreach (var funeIdx in  enemyEntity.BattleMonsterEntityData.BattleMonsterData.FuneIdxs)
                {
                    var funeData = FuneManager.Instance.GetFuneData(funeIdx);
                    if(idx >= funeList.Count)
                        break;

                    hasDetail = true;
                    funeList[idx].gameObject.SetActive(true);
                    funeList[idx].SetItemData(new PlayerCommonItemData()
                    {
                        ItemIdx = funeIdx,
                        CommonItemData = new CommonItemData()
                        {
                            ItemType = EItemType.Fune,
                            ItemID = funeData.FuneID,

                        }
                    }, null, null);
                    idx++;
                }
                
            }
            else if (UnitDescFormData.UnitCamp == EUnitCamp.Player1 || UnitDescFormData.UnitCamp == EUnitCamp.Player2)
            {
                unitBattleData.SetActive(true);
                videoPlayItem.gameObject.SetActive(true);
                videoPlayItem.SetVideo(animationPlayData);
                
                if (UnitDescFormData.UnitRole == EUnitRole.Core)
                {
                    unitDescItem.gameObject.SetActive(true);
                    animationPlayData.GifType = EGIFType.Hero;
                    unitDescItem.SetDesc(GameEntry.Localization.GetString(Constant.Localization.UI_CoreName),
                        BattlePlayerManager.Instance.PlayerData.BattleHero.CurHP + "/" +
                        BattlePlayerManager.Instance.PlayerData.BattleHero.MaxHP,
                        GameEntry.Localization.GetString(Constant.Localization.UI_CoreDesc));
                }
                else if (UnitDescFormData.UnitRole == EUnitRole.Staff)
                {
                    playerCardItem.gameObject.SetActive(true);
                    animationPlayData.GifType = EGIFType.Solider;
                    var unitEntity =
                        BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleSoliderEntity;
                    var cardData =
                        CardManager.Instance.GetCard(unitEntity.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                    var playerCardData = new PlayerCardData()
                    {
                        CardIdx = cardData.CardIdx,
                        CardID = cardData.CardID,
                    };

                    playerCardItem.SetItemData(playerCardData, false);
                    var actionTime = (unitEntity.BattleSoliderEntityData.BattleSoliderData.RoundMoveTimes +
                                      unitEntity.BattleSoliderEntityData.BattleSoliderData.RoundAttackTimes);
                
                    actionTimeStr.text = GameEntry.Localization.GetLocalizedString(Constant.Localization.UI_ActionTime,
                        actionTime);

                    
                    var idx = 0;
                    foreach (var funeIdx in cardData.FuneIdxs)
                    {
                        var funeData = FuneManager.Instance.GetFuneData(funeIdx);
                        if(idx >= funeList.Count)
                            break;
                        
                        hasDetail = true;
                        funeList[idx].gameObject.SetActive(true);
                        funeList[idx].SetItemData(new PlayerCommonItemData()
                        {
                            ItemIdx = funeIdx,
                            CommonItemData = new CommonItemData()
                            {
                                ItemType = EItemType.Fune,
                                ItemID = funeData.FuneID,

                            }
                        }, null, null);
                        idx++;
                    }

                }
            }
            
            else if (UnitDescFormData.UnitCamp == EUnitCamp.Empty)
            {
                //gridDescItem.gameObject.SetActive(true);
                
                var gridTypeName =
                    Utility.Text.Format(Constant.Localization.GridTypeName, UnitDescFormData.GridType);
                
                var gridTypeDesc =
                    Utility.Text.Format(Constant.Localization.GridTypeDesc, UnitDescFormData.GridType); 

 
                // gridDescItem.SetDesc(GameEntry.Localization.GetString(gridTypeName),
                //     GameEntry.Localization.GetString(gridTypeDesc));
            }
            
           
           
            //Vector3 mousePosition = Input.mousePosition;
            
            // var gifPos = AreaController.Instance.UICamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mousePosition.z));
            // var delta = 2f;
            //
            // if (mousePosition.x < Screen.width / 2)
            // {
            //     gifPos.x += delta;
            //     if (mousePosition.y < Screen.height / 2)
            //     {
            //         gifPos.y += delta;
            //     }
            //     else
            //     {
            //         gifPos.y -= delta;
            //     }
            // }
            // else
            // {
            //     gifPos.x -= delta;
            //     if (mousePosition.y < Screen.height / 2)
            //     {
            //         gifPos.y += delta;
            //     }
            //     else
            //     {
            //         gifPos.y -= delta;
            //     }
            // }
            //
            // root.transform.position = gifPos;

            RefreshUnitStates();
            
            funeListGO.SetActive(false);
            unitStateListGO.SetActive(false);
            unitStateIconListGO.SetActive(true);
            keyshortcutGO.SetActive(hasDetail);
        }

        private void RefreshUnitStates()
        {
            foreach (var item in unitStateIconList)
            {
                item.gameObject.SetActive(false);
            }
            
            foreach (var item in unitStateList)
            {
                item.gameObject.SetActive(false);
            }
            
            if (UnitDescFormData.UnitCamp == EUnitCamp.Enemy)
            {
                var enemyEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleMonsterEntity;

                var idx = 0;
                foreach (var kv in  enemyEntity.BattleMonsterEntityData.BattleMonsterData.UnitStateData.UnitStates)
                {
                    unitStateIconList[idx].gameObject.SetActive(false);
                    if(idx >= unitStateIconList.Count)
                        break;

                    hasDetail = true;
                    unitStateIconList[idx].gameObject.SetActive(true);
                    unitStateIconList[idx].SetIcon(kv.Value.UnitState, kv.Value.Value);
                    idx++;
                }
                
                idx = 0;
                foreach (var kv in  enemyEntity.BattleMonsterEntityData.BattleMonsterData.UnitStateData.UnitStates)
                {
                    unitStateList[idx].gameObject.SetActive(false);
                    if(idx >= unitStateList.Count)
                        break;
                        
                    hasDetail = true;
                    unitStateList[idx].gameObject.SetActive(true);
                    unitStateList[idx].SetItemData(new PlayerCommonItemData()
                    {
                        ItemIdx = (int)kv.Value.UnitState,
                        CommonItemData = new CommonItemData()
                        {
                            ItemType = EItemType.UnitState,
                            ItemID = (int)kv.Value.UnitState,

                        }
                    }, null, null);
                    idx++;
                }
                
            }
            else if (UnitDescFormData.UnitCamp == EUnitCamp.Player1 || UnitDescFormData.UnitCamp == EUnitCamp.Player2)
            {
                if (UnitDescFormData.UnitRole == EUnitRole.Core)
                {
                    var coreEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleCoreEntity;
                    
                    var idx = 0;
                    foreach (var kv in  coreEntity.BattleCoreEntityData.BattleCoreData.UnitStateData.UnitStates)
                    {
                        unitStateIconList[idx].gameObject.SetActive(false);
                        if(idx >= unitStateIconList.Count)
                            break;
                        
                        hasDetail = true;
                        unitStateIconList[idx].gameObject.SetActive(true);
                        unitStateIconList[idx].SetIcon(kv.Value.UnitState, kv.Value.Value);
                        idx++;
                    }
                    
                    idx = 0;
                    foreach (var kv in  coreEntity.BattleCoreEntityData.BattleCoreData.UnitStateData.UnitStates)
                    {
                        unitStateList[idx].gameObject.SetActive(false);
                        if(idx >= unitStateList.Count)
                            break;
                        
                        hasDetail = true;
                        unitStateList[idx].gameObject.SetActive(true);
                        unitStateList[idx].SetItemData(new PlayerCommonItemData()
                        {
                            ItemIdx = (int)kv.Value.UnitState,
                            CommonItemData = new CommonItemData()
                            {
                                ItemType = EItemType.UnitState,
                                ItemID = (int)kv.Value.UnitState,

                            }
                        }, null, null);
                        idx++;
                    }
                }
                else if (UnitDescFormData.UnitRole == EUnitRole.Staff)
                {
                    var soliderEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleSoliderEntity;
                    
                    var idx = 0;
                    foreach (var kv in  soliderEntity.BattleSoliderEntityData.BattleSoliderData.UnitStateData.UnitStates)
                    {
                        unitStateIconList[idx].gameObject.SetActive(false);
                        if(idx >= unitStateIconList.Count)
                            break;
                        
                        hasDetail = true;
                        unitStateIconList[idx].gameObject.SetActive(true);
                        unitStateIconList[idx].SetIcon(kv.Value.UnitState, kv.Value.Value);
                        idx++;
                    }

                    idx = 0;
                    foreach (var kv in  soliderEntity.BattleSoliderEntityData.BattleSoliderData.UnitStateData.UnitStates)
                    {
                        unitStateList[idx].gameObject.SetActive(false);
                        if(idx >= unitStateList.Count)
                            break;
                        
                        hasDetail = true;
                        unitStateList[idx].gameObject.SetActive(true);
                        unitStateList[idx].SetItemData(new PlayerCommonItemData()
                        {
                            ItemIdx = (int)kv.Value.UnitState,
                            CommonItemData = new CommonItemData()
                            {
                                ItemType = EItemType.UnitState,
                                ItemID = (int)kv.Value.UnitState,

                            }
                        }, null, null);
                        idx++;
                    }

                }
            }
        }
        
        public void Update()
        {
            if (BattleAreaManager.Instance.CurPointGridPosIdx == -1)
            {
                Close();

            }

            if (hasDetail && Input.GetKeyDown(KeyCode.Q))
            {
                funeListGO.SetActive(true);
                unitStateListGO.SetActive(true);
                unitStateIconListGO.SetActive(false);
                keyshortcutGO.SetActive(false);
            }
            
            if (hasDetail && Input.GetKeyUp(KeyCode.Q))
            {
                funeListGO.SetActive(false);
                unitStateListGO.SetActive(false);
                unitStateIconListGO.SetActive(true);
                keyshortcutGO.SetActive(true);
            }
        }
    }
}
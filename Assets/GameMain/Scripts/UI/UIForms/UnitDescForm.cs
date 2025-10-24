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
        [SerializeField] private ExplainList explainList;
        
        [SerializeField] private PlayerCardItem playerCardItem;
        [SerializeField] private UnitDescItem unitDescItem;
        [SerializeField] private GridDescItem gridDescItem;
        [SerializeField] private GameObject unitBattleData;
        
        [SerializeField] private Text actionTimeStr;
        
        
        
        [SerializeField]
        private GameObject funeListGO;
        [SerializeField]
        private GameObject unitStateListGO;
        [SerializeField]
        private GameObject unitStateIconListGO;
        [SerializeField]
        private GameObject keyshortcutGO;
        

        private bool hasDetail;
        private bool hasFune;

        [SerializeField]
        private Transform explainPos1;
        [SerializeField]
        private Transform explainPos2;
        
        [SerializeField] private List<PlayerCommonItem> funeList = new List<PlayerCommonItem>();
        [SerializeField] private List<PlayerCommonItem> unitStateList = new List<PlayerCommonItem>();
        [SerializeField] private List<UnitStateIconItem> unitStateIconList = new List<UnitStateIconItem>();


        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            UnitDescFormData = (UnitDescFormData)userData;
            
            // if(BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) == null)
            //     return;
            
            videoPlayItem.gameObject.SetActive(false);
            unitDescItem.gameObject.SetActive(false);
            playerCardItem.gameObject.SetActive(false);
            unitBattleData.SetActive(false);
            gridDescItem.gameObject.SetActive(false);
            
            funeListGO.SetActive(false);
            unitStateListGO.SetActive(false);
            unitStateIconListGO.SetActive(true);

            var items = funeListGO.GetComponentsInChildren<PlayerCommonItem>(true);
            funeList.Clear();
            funeList.AddRange(items);
            
            var items2 = unitStateListGO.GetComponentsInChildren<PlayerCommonItem>(true);
            unitStateList.Clear();
            unitStateList.AddRange(items2);
            
            var items3 = unitStateIconListGO.GetComponentsInChildren<UnitStateIconItem>(true);
            unitStateIconList.Clear();
            unitStateIconList.AddRange(items3);
            
            
            
            hasDetail = false;

            RefreshVideo();
            RefreshDesc();
            RefreshUnitStates();
            RefreshExplain(false);
            
            explainList.transform.localPosition = explainPos1.localPosition;
            
            keyshortcutGO.SetActive(hasDetail);
        }

        private void RefreshVideo()
        {
            var animationPlayData = new AnimationPlayData();
            if (UnitDescFormData.UnitCamp == EUnitCamp.Enemy)
            {
                var enemyEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleMonsterEntity;
                //animationPlayData.GifType = EGIFType.Enemy;
                var drEnemy =
                    GameEntry.DataTable.GetEnemy(enemyEntity.BattleMonsterEntityData.BattleMonsterData.MonsterID);
                animationPlayData.ID = drEnemy.GIFIdx;

            }
            else if (UnitDescFormData.UnitCamp == EUnitCamp.Player1 || UnitDescFormData.UnitCamp == EUnitCamp.Player2)
            {
                
                
                if (UnitDescFormData.UnitRole == EUnitRole.Core)
                {
                    //animationPlayData.GifType = EGIFType.Hero;
                }
                else if (UnitDescFormData.UnitRole == EUnitRole.Staff)
                {
                    var unitEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleSoliderEntity;
                    //
                    //var cardData = CardManager.Instance.GetCard(unitEntity.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                    var drCard = CardManager.Instance.GetCardTable(unitEntity.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                    //
                    animationPlayData.ID = drCard.GIFIdx;
                    //animationPlayData.GifType = EGIFType.Solider;
                }
            }
            
            videoPlayItem.gameObject.SetActive(true);
            videoPlayItem.SetVideo(animationPlayData);
        }

        private void RefreshDesc()
        {
            foreach (var playerCommonItem in funeList)
            {
                playerCommonItem.gameObject.SetActive(false);
            }
            
            if (UnitDescFormData.UnitCamp == EUnitCamp.Enemy)
            {
                unitDescItem.gameObject.SetActive(true);
                unitBattleData.SetActive(true);
                
                
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
                
                if (UnitDescFormData.UnitRole == EUnitRole.Core)
                {
                    unitDescItem.gameObject.SetActive(true);
                    unitDescItem.SetDesc(GameEntry.Localization.GetString(Constant.Localization.UI_CoreName),
                        BattlePlayerManager.Instance.PlayerData.BattleHero.CurHP + "/" +
                        BattlePlayerManager.Instance.PlayerData.BattleHero.MaxHP,
                        GameEntry.Localization.GetString(Constant.Localization.UI_CoreDesc));
                }
                else if (UnitDescFormData.UnitRole == EUnitRole.Staff)
                {
                    playerCardItem.gameObject.SetActive(true);
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

                        hasFune = true;
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

        public void RefreshExplain(bool isShowDetail)
        {
            var explainListData = new List<CommonItemData>();
            if (UnitDescFormData.UnitCamp == EUnitCamp.Enemy)
            {
                var enemyEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleMonsterEntity;
                explainListData.AddRange(
                    BattleEnemyManager.Instance.GetEnemyExplainList(enemyEntity.BattleMonsterEntityData
                        .BattleMonsterData.MonsterID));
                
                
            }
            else if (UnitDescFormData.UnitCamp == EUnitCamp.Player1 || UnitDescFormData.UnitCamp == EUnitCamp.Player2)
            {
                if (UnitDescFormData.UnitRole == EUnitRole.Core)
                {
                    
                }
                else if (UnitDescFormData.UnitRole == EUnitRole.Staff)
                {
                    var soliderEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleSoliderEntity;
                    var drCard = CardManager.Instance.GetCardTable(soliderEntity.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                    
                    explainListData.AddRange(BattleCardManager.Instance.GetCardExplainList(drCard.Id));
                    
                    if (isShowDetail)
                    {
                        var cardData =
                            CardManager.Instance.GetCard(soliderEntity.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                        
                        var idx = 0;
                        foreach (var funeIdx in cardData.FuneIdxs)
                        {
                            if(idx >= funeList.Count)
                                continue;

                            var funeData = FuneManager.Instance.GetFuneData(funeIdx);
                            explainListData.AddRange(
                                BattleBuffManager.Instance.GetBuffExplainList(funeData.FuneID));;
                        
                            idx++;
                        }
                    }
                    

                }
            }
            
            explainList.SetData(explainListData);
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
                //explainList.gameObject.SetActive(false);
                RefreshExplain(true);
                explainList.transform.localPosition = explainPos2.localPosition;
            }
            
            if (hasDetail && Input.GetKeyUp(KeyCode.Q))
            {
                funeListGO.SetActive(false);
                unitStateListGO.SetActive(false);
                unitStateIconListGO.SetActive(hasDetail);
                keyshortcutGO.SetActive(true);
                //explainList.gameObject.SetActive(true);
                RefreshExplain(false);
                explainList.transform.localPosition = explainPos1.localPosition;
            }
        }
    }
}
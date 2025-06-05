using System;
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

        [SerializeField] private GIFPlayItem gifPlayItem;
        
        [SerializeField] private PlayerCardItem playerCardItem;
        [SerializeField] private UnitDescItem unitDescItem;
        [SerializeField] private GridDescItem gridDescItem;
        [SerializeField] private GameObject unitBattleData;
        
        [SerializeField] private Text actionTimeStr;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            UnitDescFormData = (UnitDescFormData)userData;
            
            // if(BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) == null)
            //     return;

            var gifPlayData = new GIFPlayData();
            if (UnitDescFormData.UnitCamp == EUnitCamp.Enemy)
            {
                var enemyEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleMonsterEntity;
                gifPlayData.ItemType = EGIFType.Enemy;
                gifPlayData.ID = enemyEntity.BattleMonsterEntityData.BattleMonsterData.MonsterID;

            }
            else if (UnitDescFormData.UnitCamp == EUnitCamp.Player1 || UnitDescFormData.UnitCamp == EUnitCamp.Player2)
            {
                if (UnitDescFormData.UnitRole == EUnitRole.Hero)
                {
                    gifPlayData.ItemType = EGIFType.Hero;
                }
                else if (UnitDescFormData.UnitRole == EUnitRole.Staff)
                {
                    // var unitEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleSoliderEntity;
                    //
                    // var drCard = CardManager.Instance.GetCard(unitEntity.BattleSoliderEntityData.BattleSoliderData.CardIdx);
                    //drCard.CardID
                    gifPlayData.ID = 0;
                    gifPlayData.ItemType = EGIFType.Solider;
                }
            }
            
            gifPlayItem.SetGIF(gifPlayData);
            
            unitDescItem.gameObject.SetActive(false);
            playerCardItem.gameObject.SetActive(false);
            unitBattleData.SetActive(false);
            gridDescItem.gameObject.SetActive(false);
            
            if (UnitDescFormData.UnitCamp == EUnitCamp.Enemy)
            {
                unitDescItem.gameObject.SetActive(true);
                unitBattleData.SetActive(true);
                
                gifPlayData.ItemType = EGIFType.Enemy;
                
                var name = "";
                var desc = "";
                
                var unitEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleMonsterEntity;
 
                GameUtility.GetEnemyText(unitEntity.BattleMonsterEntityData.BattleMonsterData.MonsterID, ref name,
                    ref desc);
                
                var enemyEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleMonsterEntity;
                var power = enemyEntity.BattleMonsterEntityData.BattleMonsterData.CurHP + "/" +
                            enemyEntity.BattleMonsterEntityData.BattleMonsterData.MaxHP;
                
                unitDescItem.SetDesc(name, power, desc);
                actionTimeStr.text = (unitEntity.BattleMonsterEntityData.BattleMonsterData.RoundMoveTimes +
                                      unitEntity.BattleMonsterEntityData.BattleMonsterData.RoundAttackTimes).ToString();
                
            }
            else if (UnitDescFormData.UnitCamp == EUnitCamp.Player1 || UnitDescFormData.UnitCamp == EUnitCamp.Player2)
            {
                unitBattleData.SetActive(true);
                

                if (UnitDescFormData.UnitRole == EUnitRole.Hero)
                {
                    unitDescItem.gameObject.SetActive(true);
                    gifPlayData.ItemType = EGIFType.Hero;
                    unitDescItem.SetDesc(GameEntry.Localization.GetString(Constant.Localization.UI_CoreName),
                        BattlePlayerManager.Instance.PlayerData.BattleHero.CurHP + "/" +
                        BattlePlayerManager.Instance.PlayerData.BattleHero.MaxHP,
                        GameEntry.Localization.GetString(Constant.Localization.UI_CoreDesc));
                }
                else if (UnitDescFormData.UnitRole == EUnitRole.Staff)
                {
                    playerCardItem.gameObject.SetActive(true);
                    gifPlayData.ItemType = EGIFType.Solider;
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
                    actionTimeStr.text = (unitEntity.BattleSoliderEntityData.BattleSoliderData.RoundMoveTimes +
                                          unitEntity.BattleSoliderEntityData.BattleSoliderData.RoundAttackTimes)
                        .ToString();
                }
            }
            
            else if (UnitDescFormData.UnitCamp == EUnitCamp.Empty)
            {
                gridDescItem.gameObject.SetActive(true);
                
                var gridTypeName =
                    Utility.Text.Format(Constant.Localization.GridTypeName, UnitDescFormData.GridType);
                
                var gridTypeDesc =
                    Utility.Text.Format(Constant.Localization.GridTypeDesc, UnitDescFormData.GridType); 

 
                gridDescItem.SetDesc(GameEntry.Localization.GetString(gridTypeName),
                    GameEntry.Localization.GetString(gridTypeDesc));
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
        }
    }
}
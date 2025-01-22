using System;
using GameFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoundHero
{
    [Serializable]
    public class UnitDescFormData
    {
        public EUnitRole UnitRole;
        public EUnitCamp UnitCamp;
        public int Idx;
        
    }
    
    public class UnitDescForm : UGuiForm
    {
        private UnitDescFormData UnitDescFormData;

        [SerializeField]
        private GameObject root;

        [SerializeField] private GIFPlayItem gifPlayItem;
        
        [SerializeField] private PlayerCardItem playerCardItem;
        [SerializeField] private UnitDescItem unitDescItem;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            UnitDescFormData = (UnitDescFormData)userData;

            var gifPlayData = new GIFPlayData();
            if (UnitDescFormData.UnitCamp == EUnitCamp.Enemy)
            {
                gifPlayData.ItemType = EGIFType.Enemy;
                var enemyEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleMonsterEntity;
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
                    var drCard = CardManager.Instance.GetCard(UnitDescFormData.Idx);
                    //drCard.CardID
                    gifPlayData.ID = 0;
                    gifPlayData.ItemType = EGIFType.Solider;
                }
            }
            
            gifPlayItem.SetGIF(gifPlayData);
            
            if (UnitDescFormData.UnitCamp == EUnitCamp.Enemy)
            {
                unitDescItem.gameObject.SetActive(true);
                playerCardItem.gameObject.SetActive(false);
                
                gifPlayData.ItemType = EGIFType.Enemy;
                
                var name = GameEntry.Localization.GetString(Utility.Text.Format(Constant.Localization.EnemyName,
                    gifPlayData.ID));
                var desc = GameEntry.Localization.GetString(Utility.Text.Format(Constant.Localization.EnemyDesc,
                    gifPlayData.ID));
                
                var enemyEntity = BattleUnitManager.Instance.GetUnitByIdx(UnitDescFormData.Idx) as BattleMonsterEntity;
                var power = enemyEntity.BattleMonsterEntityData.BattleMonsterData.CurHP + "/" +
                            enemyEntity.BattleMonsterEntityData.BattleMonsterData.MaxHP;
                
                unitDescItem.SetDesc(name, power, desc);
                
            }
            else if (UnitDescFormData.UnitCamp == EUnitCamp.Player1 || UnitDescFormData.UnitCamp == EUnitCamp.Player2)
            {
                if (UnitDescFormData.UnitRole == EUnitRole.Hero)
                {
                    unitDescItem.gameObject.SetActive(true);
                    playerCardItem.gameObject.SetActive(false);
                    
                    gifPlayData.ItemType = EGIFType.Hero;
                    unitDescItem.SetDesc("1", "5", "2323");
                }
                else if (UnitDescFormData.UnitRole == EUnitRole.Staff)
                {
                    playerCardItem.gameObject.SetActive(true);
                    unitDescItem.gameObject.SetActive(false);
                    
                    gifPlayData.ItemType = EGIFType.Solider;
                    var drCard = CardManager.Instance.GetCard(UnitDescFormData.Idx);
                    var playerCardData = new PlayerCardData()
                    {
                        CardIdx = UnitDescFormData.Idx,
                        CardID = drCard.CardID,
                    };
                    
                    playerCardItem.SetItemData(playerCardData, true);
                }
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
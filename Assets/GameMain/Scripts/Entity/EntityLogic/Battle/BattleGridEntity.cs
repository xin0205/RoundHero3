
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class BattleGridEntity : Entity, IMoveGrid
    {
        public BattleGridEntityData BattleGridEntityData { get; protected set; }

        [SerializeField] private TextMesh posTag;
        [SerializeField] private GameObject selectionGrid_empty;
        [SerializeField] private GameObject selectionGrid_us;
        [SerializeField] private GameObject selectionGrid_enemy;
        
        [SerializeField] private GameObject backupGrid;
        [SerializeField] private GameObject grid;
        
        [SerializeField] private UnitDescTriggerItem UnitDescTriggerItem;

        public Vector3 Position
        {
            get => transform.position; 
            set => transform.position = value;
        }
        
        public int GridPosIdx
        {
            get => BattleGridEntityData.GridPosIdx;
            set => BattleGridEntityData.GridPosIdx = value;
        }

        public void SetSelectionGridActive(bool active)
        {
            selectionGrid_empty.SetActive(active);
            selectionGrid_us.SetActive(active); 
            selectionGrid_enemy.SetActive(active);
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            BattleGridEntityData = userData as BattleGridEntityData;
            if (BattleGridEntityData == null)
            {
                Log.Error("Error BattleGridEntityData");
                return;
            }

            Show(false);
            backupGrid.SetActive(false);
            SetSelectionGridActive(false);
            Refresh();
            
            var unitDescFormData = GetComponent<UnitDescTriggerItem>().UnitDescFormData;
            unitDescFormData.GridType = BattleGridEntityData.GridType;
            unitDescFormData.UnitCamp = EUnitCamp.Empty;

        }

        public void Show(bool active)
        {
            // && (backupGrid.activeSelf || selectionGrid.activeSelf)
            if(active)
                return;
            //BattleGridEntityData.GridType != EGridType.Obstacle && 
            grid.SetActive(active);
        }

        public void Refresh()
        {
            //posTag.text = "";
            var coord = GameUtility.GridPosIdxToCoord(BattleGridEntityData.GridPosIdx);
            posTag.text = coord.x + "," + coord.y + "-" + BattleGridEntityData.GridPosIdx;
        }

        public void ShowBackupGrid(bool isShow)
        {
            if (isShow)
            {
                grid.SetActive(false);
            }
            else
            {
                grid.SetActive(true);
            }
            
            backupGrid.SetActive(isShow);
            
            
        }

        public void ShowSelectGrid(bool isShow)
        {
            if (isShow)
            {
                grid.SetActive(false);
            }
            else
            {
                grid.SetActive(true);
            }

            SetSelectionGridActive(false);
            if (!isShow)
            {
                return;
            }
            
            
            var unit = BattleUnitManager.Instance.GetUnitByGridPosIdx(BattleGridEntityData.GridPosIdx);
            
            // if (BattleAreaManager.Instance.TmpUnitEntity != null)
            // {
            //     selectionGrid_us.SetActive(true);
            // }
            // else 
            if (unit == null)
            {
                selectionGrid_empty.SetActive(true);
            }
            else
            {
                if (unit is BattleSoliderEntity ||
                    unit is BattleCoreEntity)
                {
                    selectionGrid_us.SetActive(true);
                }
                else if (unit is BattleMonsterEntity)
                {
                    selectionGrid_enemy.SetActive(true);
                }
                else
                {
                    selectionGrid_empty.SetActive(true);
                }
            }


           
        }
        
        public void OnPointerEnter(BaseEventData baseEventData)
        {
            ShowSelectGrid(true);
            //UnitDescTriggerItem.OnPointerEnter();
            GameEntry.Event.Fire(null, ShowGridDetailEventArgs.Create(BattleGridEntityData.GridPosIdx, EShowState.Show)); 
        }
        
        public void OnPointerExit(BaseEventData baseEventData)
        {
            //Log.Debug("OnPointerExit" + BattleGridEntityData.Id);
            ShowSelectGrid(false);
            //UnitDescTriggerItem.OnPointerExit();
            GameEntry.Event.Fire(null, ShowGridDetailEventArgs.Create(BattleGridEntityData.GridPosIdx, EShowState.Unshow)); 
        }
        
        public void OnPointerEnter()
        {

            //UnitDescTriggerItem.OnPointerEnter();
            
        }
        
        public void OnPointerExit()
        {

            //UnitDescTriggerItem.OnPointerExit();
            
        }
        
        public void OnPointerClick(BaseEventData baseEventData)
        {
            if (Input.GetMouseButtonUp(0))
            {
                GameEntry.Event.Fire(null, ClickGridEventArgs.Create(BattleGridEntityData.GridPosIdx)); 
            }
            
        }
    }
}
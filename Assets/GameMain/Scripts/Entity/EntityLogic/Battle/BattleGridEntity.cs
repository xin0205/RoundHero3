
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

        [SerializeField] private TextMeshPro posTag;
        [SerializeField] private GameObject selectionGrid;
        [SerializeField] private GameObject backupGrid;
        [SerializeField] private GameObject grid;

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

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            BattleGridEntityData = userData as BattleGridEntityData;
            if (BattleGridEntityData == null)
            {
                Log.Error("Error BattleGridEntityData");
                return;
            }

            Show(true);
            Refresh();
        }

        public void Show(bool active)
        {
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
            backupGrid.SetActive(isShow);
        }

        public void ShowSelectGrid(bool isShow)
        {
            if (isShow)
            {
                grid.SetActive(false);
            }
            
            selectionGrid.SetActive(isShow);
        }
        
        public void OnPointerEnter(BaseEventData baseEventData)
        {
            ShowSelectGrid(true);
            GameEntry.Event.Fire(null, ShowGridDetailEventArgs.Create(BattleGridEntityData.GridPosIdx, EShowState.Show)); 
        }
        
        public void OnPointerExit(BaseEventData baseEventData)
        {
            ShowSelectGrid(false);
            GameEntry.Event.Fire(null, ShowGridDetailEventArgs.Create(BattleGridEntityData.GridPosIdx, EShowState.Unshow)); 
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
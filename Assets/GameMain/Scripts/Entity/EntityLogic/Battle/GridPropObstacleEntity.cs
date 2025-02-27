using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class GridPropObstacleEntity : GridPropEntity
    {
        public GridPropEntityData GridPropEntityData { get; protected set; }

        [SerializeField]
        private List<GameObject> ObstacleGOs;
        
        
        public override int GridPosIdx
        {
            get => GridPropEntityData.GridPropData.GridPosIdx; 
            set => GridPropEntityData.GridPropData.GridPosIdx = value;
        }
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            GridPropEntityData = userData as GridPropEntityData;
            if (GridPropEntityData == null)
            {
                Log.Error("Error GridPropEntityData");
                return;
            }

            var randomIdx = Random.Range(0, ObstacleGOs.Count);
            for (int i = 0; i < ObstacleGOs.Count; i++)
            {
                ObstacleGOs[i].SetActive(i == randomIdx);
            }

        }
        
        public virtual void OnPointerEnter(BaseEventData baseEventData)
        {

            GameEntry.Event.Fire(null, SelectGridEventArgs.Create(GridPropEntityData.GridPropData.GridPosIdx, true));
            GameEntry.Event.Fire(null, ShowGridDetailEventArgs.Create(GridPropEntityData.GridPropData.GridPosIdx, EShowState.Show)); 
  
        }
        
        public virtual void OnPointerExit(BaseEventData baseEventData)
        {

            GameEntry.Event.Fire(null, SelectGridEventArgs.Create(GridPropEntityData.GridPropData.GridPosIdx, false));
            GameEntry.Event.Fire(null, ShowGridDetailEventArgs.Create(GridPropEntityData.GridPropData.GridPosIdx, EShowState.Unshow)); 

        }
    }
}
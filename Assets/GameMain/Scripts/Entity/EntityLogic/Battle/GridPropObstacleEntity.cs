using System.Collections.Generic;
using UnityEngine;
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
    }
}
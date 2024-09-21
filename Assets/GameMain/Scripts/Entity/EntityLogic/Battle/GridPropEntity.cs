using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class GridPropEntity : Entity, IMoveGrid
    {
        public GridPropEntityData GridPropEntityData { get; protected set; }
        
        public virtual Vector3 Position
        {
            get => transform.position; 
            set => transform.position = value;
        }

        public virtual int GridPosIdx
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
        }

    }
}
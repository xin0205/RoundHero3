using UnityEngine;

namespace RoundHero
{
    public class GridPropEntityData : EntityData
    {
        public Data_GridProp GridPropData;

        public void Init(int entityId, Vector3 pos, Data_GridProp gridPropData)
        {
            base.Init(entityId, pos);
            GridPropData = gridPropData;

        }
    }
    
    public class GridPropMoveDirectEntityData : GridPropEntityData
    {
        public Data_GridPropMoveDirect GridPropMoveDirectData;
        

        public void Init(int entityId, Vector3 pos, Data_GridPropMoveDirect gridPropMoveDirectData)
        {
            base.Init(entityId, pos);
            GridPropMoveDirectData = gridPropMoveDirectData;

        }
    }
    
    
}

using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class GridPropMoveDirectEntity : GridPropEntity
    {
        public GridPropMoveDirectEntityData GridPropMoveDirectEntityData { get; protected set; }

        [SerializeField]
        private MoveDirectGODictionary MoveDirectGODict;
        
        
        public override int GridPosIdx
        {
            get => GridPropMoveDirectEntityData.GridPropMoveDirectData.GridPosIdx; 
            set => GridPropMoveDirectEntityData.GridPropMoveDirectData.GridPosIdx = value;
        }
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            GridPropMoveDirectEntityData = userData as GridPropMoveDirectEntityData;
            if (GridPropMoveDirectEntityData == null)
            {
                Log.Error("Error GridPropMoveDirectEntityData");
                return;
            }

            foreach (var kv in MoveDirectGODict)
            {
                MoveDirectGODict[kv.Key]
                    .SetActive(kv.Key == GridPropMoveDirectEntityData.GridPropMoveDirectData.Direct);

            }
        }
    }
}
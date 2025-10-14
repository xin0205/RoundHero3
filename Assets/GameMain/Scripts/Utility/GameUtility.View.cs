using UnityEngine;

namespace RoundHero
{
    public static partial class GameUtility
    {
        public static Vector2Int GridPosIdxToCoord(int posIdx)
        {
            return new Vector2Int( posIdx % Constant.Area.GridSize.x, posIdx / Constant.Area.GridSize.y);
            
        }

        public static Vector3 GridPosIdxToPos(int posIdx)
        {
            var coord = GridPosIdxToCoord(posIdx);

            return GridCoordToPos(coord);
        }
        
        public static Vector3 GridCoordToPos(Vector2Int coord)
        {
            return AreaController.Instance.GridRoot.position + new Vector3(coord.x * Constant.Area.GridRange.x, 0, coord.y * Constant.Area.GridRange.y);
        }
        
        public static Vector2Int GridPosToCoord(Vector3 pos)
        {
            var deltaX = pos.x - AreaController.Instance.GridRoot.position.x + Constant.Area.GridLength.x / 2;
            var deltaZ = pos.z - AreaController.Instance.GridRoot.position.z + Constant.Area.GridLength.y / 2;
            deltaX = deltaX < 0 ? -Constant.Area.GridRange.x : deltaX;
            deltaZ = deltaZ < 0 ? -Constant.Area.GridRange.y : deltaZ;
            
            
            return new Vector2Int((int)(deltaX / Constant.Area.GridRange.x),
                (int)(deltaZ / Constant.Area.GridRange.y));
        }
        
        public static int GridCoordToPosIdx(Vector2Int coord)
        {
            return coord.x + coord.y * Constant.Area.GridSize.x;
        }
        
        public static int GridPosToPosIdx(Vector3 pos)
        {
            var coord = GridPosToCoord(pos);

            return GridCoordToPosIdx(coord);
        }
    }
}
using GameFramework;
using UnityEngine;

namespace RoundHero
{
    public class GridDescData
    {
        public int GridPosIdx;
    }
    
    public class GridDescForm : UGuiForm
    {
        public GridDescData GridDescData;

        [SerializeField] private GridDescItem gridDescItem;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            GridDescData = (GridDescData)userData;

            RefreshDesc();
            
        }

        private void RefreshDesc()
        {
            var gridType = GameUtility.GetGridType(GridDescData.GridPosIdx, false);

            var gridProp = BattleGridPropManager.Instance.GetGridProp(GridDescData.GridPosIdx);
            

            if (gridProp != null)
            {
                var propName = "";
                var propDesc = "";
                GameUtility.GetPropText(gridProp.GridPropID, ref propName, ref propDesc);
                
                gridDescItem.SetDesc(propName,
                    propDesc);
            }
            else
            {
                var gridTypeName =
                    Utility.Text.Format(Constant.Localization.GridTypeName, gridType);
                
                var gridTypeDesc =
                    Utility.Text.Format(Constant.Localization.GridTypeDesc, gridType); 
            
                gridDescItem.SetDesc(GameEntry.Localization.GetString(gridTypeName),
                    GameEntry.Localization.GetString(gridTypeDesc));
            }
            
            
            
        }
        
        public void Update()
        {

            if (BattleAreaManager.Instance.CurPointGridPosIdx == -1 ||
                BattleAreaManager.Instance.CurPointGridPosIdx != GridDescData.GridPosIdx)
            {
                GameEntry.UI.CloseUIForm(this);
            }
        }

    }

}
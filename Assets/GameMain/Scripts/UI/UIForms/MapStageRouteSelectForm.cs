using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace RoundHero
{

    // public class MapStageRouteSelectFormData
    // {
    //     public int RandomSeed;
    // }
    
    public class MapStageRouteSelectForm : UGuiForm
    {
        //[SerializeField] private MapStageRouteSelectFormData mapStageRouteSelectFormData;

        [SerializeField] private List<MapStageRouteItem> mapStageRouteItems;
        
        private int randomSeed;
        private int selectRouteIdx;

        private ToggleGroup toggleGroup;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            //mapStageRouteSelectFormData = (MapStageRouteSelectFormData)userData;
            // if (mapStageRouteSelectFormData == null)
            // {
            //     Log.Warning("mapStageRouteSelectFormData is null.");
            //     return;
            // }

            //Refresh();

        }

        

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            
        }
    }
}
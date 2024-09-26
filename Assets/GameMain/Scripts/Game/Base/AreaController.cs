
using UGFExtensions.Await;
using UnityEngine;

namespace RoundHero
{
    public class AreaController : TMonoSingleton<AreaController>
    {
        public Transform GridRoot;
        [SerializeField]
        private Plane CameraPlane;

        public Camera UICamera;

        [SerializeField] public HeroIcon HeroIcon;
        

        public void Awake() 
        {
        }

        public void RefreshCameraPlane()
        {
            CameraPlane = new Plane(Camera.main.transform.forward, Camera.main.transform.position);
            var a= GameEntry.Resource.LoadAssetAsync<Sprite>("");
        }

        public float GetDistanceToPoint(Vector3 point)
        {
            return CameraPlane.GetDistanceToPoint(point);
        }

    }
}
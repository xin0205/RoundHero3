using UnityEngine;

namespace RoundHero
{
    public class AreaController : TMonoSingleton<AreaController>
    {
        public Transform GridRoot;
        [SerializeField]
        private Plane CameraPlane;

        public Camera UICamera;

        public Canvas Canvas;

        [SerializeField] public HeroIcon HeroIcon;

        public GameObject UICore;

        public void Awake() 
        {
        }

        public void RefreshCameraPlane()
        {
            CameraPlane = new Plane(Camera.main.transform.forward, Camera.main.transform.position);
            
        }

        public float GetDistanceToPoint(Vector3 point)
        {
            return CameraPlane.GetDistanceToPoint(point);
        }

    }
}
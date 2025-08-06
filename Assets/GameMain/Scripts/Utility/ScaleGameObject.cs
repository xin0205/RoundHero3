using UnityEngine;

namespace RoundHero
{
    public class ScaleGameObject : MonoBehaviour
    {
        private Vector3 startScale;
        private Vector3 endScale;
        private float scaleTime;
        private float time;
        private float scaleSpeed;
        private bool isLoop;
        private Vector3 scaleDelta;
        
        public void Scale(Vector3 startScale, Vector3 endScale, float scaleTime, bool isLoop = false)
        {
            this.startScale = startScale;
            this.endScale = endScale;
            this.scaleTime = scaleTime;
            this.isLoop = isLoop;
            this.time = 0;
            
            scaleDelta = (endScale - startScale) / scaleTime;
            this.transform.localScale = startScale;
        }

        public void Update()
        {
            if(time == -1)
                return;
            
            time += Time.deltaTime;
            this.transform.localScale += scaleDelta * Time.deltaTime;
            
  
            if(this.time >= scaleTime)
            {
                if (isLoop)
                {
                    time = 0;
                    this.transform.localScale = startScale;
                }
                else
                {
                    time = -1;
                    this.transform.localScale = endScale;
                }

            }
            
        }
    }
}
using UnityEngine;

namespace RoundHero
{
    public class MoveGameObject : MonoBehaviour
    {
        private Vector3 startPos;
        private Vector3 endPos;
        private float moveTime;
        private float time;
        private float moveSpeed;
        private bool isLoop;
        private float disRatio;
        private Vector3 moveDelta;
        
        public void Move(Vector3 startPos, Vector3 endPos,  float moveTime, bool isLoop = false)
        {
            this.startPos = startPos;
            this.endPos = endPos;
            this.moveTime = moveTime;
            this.moveSpeed = moveSpeed;
            this.isLoop = isLoop;
            this.time = 0;
            
            //var dis = Vector3.Distance(startPos, endPos);
            moveDelta = (endPos - startPos) / moveTime;
            this.transform.localPosition = startPos;
            //disRatio = dis / 100f;
        }

        public void Update()
        {
            if(time == -1)
                return;
            
            time += Time.deltaTime;
            
            this.transform.localPosition += moveDelta * Time.deltaTime;
            
            //this.transform.localPosition = Vector2.Lerp(startPos, endPos,  4 * moveSpeed * time / disRatio);
            
            if(this.time >= moveTime)
            {
                
                if (isLoop)
                {
                    time = 0;
                    this.transform.localPosition = startPos;
                }
                else
                {
                    time = -1;
                    this.transform.localPosition = endPos;
                }

            }
            
        }
    }
}
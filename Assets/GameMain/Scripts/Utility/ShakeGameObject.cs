
namespace RoundHero
{
    using UnityEngine;

    public class ShakeGameObject : MonoBehaviour
    {
        [Header("抖动设置")]
        [Tooltip("抖动强度")]
        public float shakeStrength = 0.1f;
        [Tooltip("抖动时长")]
        public float shakeDuration = 0.1f;
        [Tooltip("抖动频率")]
        public float shakeFrequency = 10f;

        // 初始位置
        private Vector3 originalPosition;
        // 是否正在抖动
        private bool isShaking = false;
        // 抖动计时器
        private float shakeTimer;

        void Start()
        {
            // 记录初始位置
            originalPosition = transform.localPosition;
        }

        void Update()
        {
            if (isShaking)
            {
                // 更新抖动计时器
                shakeTimer += Time.deltaTime;

                if (shakeTimer < shakeDuration)
                {
                    // 计算抖动偏移量（在三个轴上随机抖动）
                    float x = Random.Range(-1f, 1f) * shakeStrength;
                    float y = Random.Range(-1f, 1f) * shakeStrength;
                    float z = Random.Range(-1f, 1f) * shakeStrength;

                    // 应用抖动位置
                    transform.localPosition = originalPosition + new Vector3(x, y, z);
                }
                else
                {
                    // 抖动结束，回到初始位置
                    transform.localPosition = originalPosition;
                    isShaking = false;
                    shakeTimer = 0;
                }
            }
        }

        /// <summary>
        /// 触发抖动效果
        /// </summary>
        public void Shake()
        {
            // 如果正在抖动，重置计时器
            if (isShaking)
            {
                shakeTimer = 0;
            }
            else
            {
                isShaking = true;
            }
        }
    }
}

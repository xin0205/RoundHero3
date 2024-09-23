using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine;

public class ScreenShoot : MonoBehaviour
{
    
    public Camera cropCamera; //待截图的目标摄像机
    RenderTexture renderTexture;
    Texture2D texture2D;
    
    void Start()
    {
        renderTexture = new RenderTexture(256, 256, 16);//尺寸可调节
        texture2D = new Texture2D(256, 256);
        cropCamera.targetTexture = renderTexture;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cropCamera.Render();
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = null;
            byte[] bytes = texture2D.EncodeToPNG();
            //DataTime.UtcNow获取的是世界标准时区的当前时间不受电脑配置影响
            //DateTime.Now获取的是电脑上的当前时间是可以自己进行调整的
            File.WriteAllBytes(Application.dataPath + "/../Assets/SaveData/" + (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds + ".png", bytes);
        }
    }



}




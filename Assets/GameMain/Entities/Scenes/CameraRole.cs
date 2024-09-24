using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRole : MonoBehaviour
{
    public Camera shotCam;
    public RawImage texture;
    private Texture2D tex = null;
    void OnPostRender()
    {
        //在每次相机渲染完成时再删除上一帧的texture
        if(tex != null)
        {
            Destroy(tex);
        }
        //设定当前RenderTexture为快照相机的targetTexture
        RenderTexture rt = shotCam.targetTexture;
        RenderTexture.active = rt;
        tex = new Texture2D(rt.width, rt.height);
        //读取缓冲区像素信息
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        texture.texture = tex;
    }
}

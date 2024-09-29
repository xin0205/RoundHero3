
using UnityEngine;

public class SpriteFull : MonoBehaviour {
 
    //设置SpriteRender和摄像机的距离
    public float distance = 1.0f;
    public Camera camera;
 
 
    private SpriteRenderer spriteRenderer = null;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer> ();
        spriteRenderer.material.renderQueue =2980; //这段代码非常重要！！！大家务必要加上，不然透明的渲染层级会出错
    }
 
    void Update()
    {
        // camera.transform.rotation = Quaternion.Euler (Vector3.zero);
        //
        // float width = spriteRenderer.sprite.bounds.size.x;
        // float height = spriteRenderer.sprite.bounds.size.y;
 
        // float worldScreenHeight,worldScreenWidth;
        // //这里分别处理正交和非正交摄像机
        // if (camera.orthographic) {
        //     worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        //     worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        // } else {
        //     worldScreenHeight = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        //     worldScreenWidth = worldScreenHeight * camera.aspect;
        // }
        // transform.localPosition = new Vector3 (camera.transform.position.x, camera.transform.position.y, distance);
        // transform.localScale = new Vector3 (worldScreenWidth / width, worldScreenHeight / height, 0f);
    }
}

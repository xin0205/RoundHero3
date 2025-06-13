Shader "Custom/AlwaysOnTop" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        // 添加 _Color 属性，使其能被 SpriteRenderer 控制
        _Color ("Tint", Color) = (1,1,1,1) 
    }
    SubShader {
        Tags { 
            "Queue" = "Overlay"  // 使用最高渲染队列
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off  // 关闭深度写入
        ZTest Always // 总是通过深度测试
        Blend SrcAlpha OneMinusSrcAlpha // 标准透明度混合
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR; // 添加顶点颜色输入
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR; // 传递颜色到片段着色器
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            // 声明 _Color 变量，使 SpriteRenderer 可以控制它
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)
            
            v2f vert (appdata_t v) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                
                // 将实例化的 _Color 与顶点颜色相乘
                fixed4 instanceColor = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                o.color = v.color * instanceColor;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                // 采样纹理并与颜色相乘
                fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
                
                // 应用简单透明度裁剪（可选）
                #ifdef UNITY_UI_ALPHACLIP
                clip(col.a - 0.001);
                #endif
                
                return col;
            }
            ENDCG
        }
    }
}
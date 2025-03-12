Shader "Custom/TextMeshOutline" {
    Properties {
        _MainTex ("Font Texture", 2D) = "white" {}
        _Color ("Text Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.0, 0.1)) = 0.01
    }
    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
        }
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        // 第一个Pass绘制描边
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _OutlineColor;
            float _OutlineWidth;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 采样8个方向的像素
                float2 offsets[8] = {
                    float2(-1, 0), float2(1, 0), float2(0, 1), float2(0, -1),
                    float2(-1, 1), float2(1, 1), float2(-1, -1), float2(1, -1)
                };
                float outlineA = 0;
                for(int j = 0; j < 8; j++) {
                    float2 offset = offsets[j] * _OutlineWidth;
                    outlineA += tex2D(_MainTex, i.uv + offset).a;
                }
                outlineA = saturate(outlineA); // 限制在0-1范围

                fixed4 col = _OutlineColor;
                col.a *= outlineA;
                return col;
            }
            ENDCG
        }

        // 第二个Pass绘制原始文本
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= _Color.rgb;
                col.a *= _Color.a;
                return col;
            }
            ENDCG
        }
    }
}
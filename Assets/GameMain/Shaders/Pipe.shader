Shader "Custom/Dynamic Route"
{
    Properties
    {
        _Cutoff( "Mask Clip Value", Float ) = 0.5
		_Speed("Speed", Range( 0 , 150)) = 0.01
		_MainTex("MainTex", 2D) = "white" {}
		_Number("Number", Range( 0 , 100)) = 3.082893
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _AlphaScale("Alpha_Scale", Range(0, 1)) = 0.5

    }
    SubShader
    {
        Tags { "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType"="TransparentCutout" }
        LOD 100
        Cull Off
//        Pass
//        {
//            ZWrite On
//            ColorMask 0
//        }

        Pass
        {

            //ZWrite Off
//            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

		    uniform float _Speed;
		    uniform float _Number;
		    uniform float _Cutoff = 0.5;
		    fixed4 _Color;
            uniform float _AlphaScale;
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 temp_cast_0 = (_Number).xx;
                float2 uv_TexCoord = i.uv * temp_cast_0;
                float4 tex2DNode = tex2D( _MainTex, ( ( -_Speed * _Time ) + uv_TexCoord.x ).xy );

                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Color;
                col.a = col.a * _AlphaScale;
                clip( tex2DNode.a - _Cutoff );
                
                return col;
            }
            ENDCG
        }
    }
}

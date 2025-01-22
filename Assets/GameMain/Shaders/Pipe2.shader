
Shader "Custom/Pipe2"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Speed("Speed", Range( 0 , 150)) = 0.01
		_MainTex("MainTex", 2D) = "white" {}
		_Number("Number", Range( 0 , 100)) = 3.082893
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		_AlphaScale("Alpha_Scale", Range(0, 1)) = 0.5
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
 
	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};
 
		uniform sampler2D _MainTex;
		uniform float _Speed;
		uniform float _Number;
		uniform float _Cutoff = 0.5;
		uniform float _AlphaScale = 0.5;
		fixed4 _Color;
 
		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (_Number).xx;
			float2 uv_TexCoord158 = i.uv_texcoord * temp_cast_0;
			float4 tex2DNode174 = tex2D( _MainTex, ( ( _Speed * _Time ) + uv_TexCoord158.x ).xy );
			//tex2DNode174.rgb
			o.Albedo = _Color;
			o.Alpha = _AlphaScale;
			clip( tex2DNode174.a - _Cutoff );
		}
 
		ENDCG
	}
	Fallback "Diffuse"
}
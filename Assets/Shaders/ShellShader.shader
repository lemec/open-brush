// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ShellShader"
{
Properties
{
	_Color ("Color", Color) = (0,0,0,0)
}

SubShader
{
	Blend OneMinusDstColor OneMinusSrcAlpha
	ZWrite Off
	Cull Off

	Tags
	{
		"RenderType"="Transparent"
		"Queue"="Transparent"
	}

	Pass
	{
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
		};

		struct v2f
		{
			float2 screenuv : TEXCOORD0;
			float4 vertex : SV_POSITION;
			float depth : DEPTH;
		};

		sampler2D _MainTex;

		v2f vert (appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);

			o.screenuv = ((o.vertex.xy / o.vertex.w) + 1)/2;
			o.screenuv.y = 1 - o.screenuv.y;
			o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z *_ProjectionParams.w;

			return o;
		}
		
		sampler2D _CameraDepthNormalsTexture;
		fixed4 _Color;

		fixed4 frag (v2f i) : SV_Target
		{
			float screenDepth = DecodeFloatRG(tex2D(_CameraDepthNormalsTexture, i.screenuv).zw);
			float diff = abs(screenDepth - i.depth);
			float intersect = 1 - smoothstep(0, _ProjectionParams.w * 0.5, diff);

			fixed4 glowColor = fixed4(lerp(pow(_Color.rgb, 4), _Color.rgb, pow(intersect, 4)), 1);
			
			fixed4 col = glowColor * intersect * _Color.a;
			return col;
		}
		ENDCG
	}
  }
}
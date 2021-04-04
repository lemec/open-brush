// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Highlights intersections with other objects
 // https://chrismflynn.wordpress.com/2012/09/06/fun-with-shaders-and-the-depth-buffer/
Shader "Custom/DepthRipple"
{
	Properties
	{
		_Color("Main Color", Color) = (1, 1, 1, .5) //Color when not intersecting
		_MaxDistance("Max Distance", Float) = 10
		_Spacing("Spacing", Float) = 1
		_Threshold("Threshold", Float) = 0.8
	}
	SubShader
	{
		Tags {
			"Queue" = "Transparent"
			"RenderType"="Transparent"  
		}
 
		Pass
		{

		// Result = FG * BF + BG * BF
			Blend OneMinusDstColor OneMinusSrcAlpha

			ZWrite Off
			Cull Off
 
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
 
			sampler2D _CameraDepthTexture; //Depth Texture
			float4 _Color;
			float _MaxDistance;
			float _Spacing;
			float _Threshold;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 projPos : TEXCOORD0; //Screen position of pos
				float4 viewPos : TEXCOORD1;
			};
 
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.projPos = ComputeScreenPos(o.pos);
				o.viewPos = v.vertex;
				return o;
			}
 
			half4 frag(v2f i) : COLOR
			{
				float4 finalColor = float4(0, 0, 0, 0);

				float viewDot = -normalize(ObjSpaceViewDir(i.viewPos)).z;
				//Get the distance to the camera from the depth buffer for this point
				float pixelDistance = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r) / viewDot;
				//If the two are similar, then there is an object intersecting with our object
				float diff = frac(pixelDistance / _Spacing);
				diff = step(_Threshold, (0.5 - min(diff, 1 - diff)) * 2);
				diff = lerp(diff, 0, saturate(pixelDistance / _MaxDistance));
				finalColor = diff * _Color;

				half4 c;
				c.r = finalColor.r * finalColor.a;//finalColor.r;
				c.g = finalColor.g * finalColor.a;//finalColor.g;
				c.b = finalColor.b * finalColor.a;//finalColor.b;
				c.a = finalColor.a;
 
				return c;
			}
 
			ENDCG
		}
	}
    FallBack "VertexLit"
}
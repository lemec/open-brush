// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Highlights intersections with other objects
 // https://chrismflynn.wordpress.com/2012/09/06/fun-with-shaders-and-the-depth-buffer/
Shader "Custom/ShellShader"
{
	Properties
	{
		_Color("Main Color", Color) = (1, 1, 1, .5) //Color when not intersecting
		_HighlightThresholdMax("Highlight Threshold Max", Float) = 1 //Max difference for intersections
		_HighlightPower("Highlight Power", Float) = 1
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
			#pragma target 2.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
 
			uniform sampler2D _CameraDepthTexture; //Depth Texture
			uniform float4 _Color;
			uniform float _HighlightThresholdMax;
			uniform float _HighlightPower;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 projPos : TEXCOORD1; //Screen position of pos
			};
 
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.projPos = ComputeScreenPos(o.pos);
 
				return o;
			}
 
			half4 frag(v2f i) : COLOR
			{
				float4 finalColor = float4(0, 0, 0, 0);

				//Get the distance to the camera from the depth buffer for this point
				float sceneZ = LinearEyeDepth (tex2Dproj(_CameraDepthTexture,
															UNITY_PROJ_COORD(i.projPos)).r);
 
				//Actual distance to the camera
				float partZ = i.projPos.z;
 
				//If the two are similar, then there is an object intersecting with our object
				float diff = (abs(sceneZ - partZ)) /
					_HighlightThresholdMax;
 
				if(diff <= 1)
				{
					float colorLerp = pow(diff, _HighlightPower);
					finalColor = lerp(_Color, finalColor, (colorLerp - 0.1) * 1.5) + (colorLerp > 0.9 && colorLerp < 0.95);
					finalColor.a = colorLerp * _Color.a;
				}
 
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
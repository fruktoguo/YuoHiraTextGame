Shader "URP2D/ShowPixel"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
	}
		SubShader
	{
		Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off
		Pass
		{
			Tags { "LightMode" = "UniversalForward" "Queue" = "Transparent" "RenderType" = "Transparent"}

			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#pragma vertex UnlitVertex
			#pragma fragment UnlitFragment

			#pragma multi_compile_fragment _ DEBUG_DISPLAY

			struct Attributes
			{
				float3 positionOS   : POSITION;
				float4 color        : COLOR;
				float2 uv           : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4  positionCS      : SV_POSITION;
				float4  color           : COLOR;
				float2  uv              : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			float4 _MainTex_ST;
			float4 _pixel[900];
			Varyings UnlitVertex(Attributes attributes)
			{
				Varyings o = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(attributes);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.positionCS = TransformObjectToHClip(attributes.positionOS);
				o.uv = TRANSFORM_TEX(attributes.uv, _MainTex);
				o.color = attributes.color;
				return o;
			}

			float4 UnlitFragment(Varyings i) : SV_Target
			{
				return _pixel[i.uv.x* 30 * i.uv.y + i.uv.y * 30];
				float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
				return mainTex;
			}
			ENDHLSL
		}
	}
		Fallback "Sprites/Default"
}
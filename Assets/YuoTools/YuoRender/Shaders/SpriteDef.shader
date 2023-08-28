Shader "URP2D/SpriteDef"
{
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		// Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
		[HideInInspector] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
		[HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}
	SubShader
		{
			Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZWrite Off

			Pass
			{
				Tags { "LightMode" = "Universal2D" }

				HLSLPROGRAM
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

				#pragma vertex UnlitVertex
				#pragma fragment UnlitFragment

				#pragma multi_compile _ DEBUG_DISPLAY

				struct Attributes
				{
					float3 positionOS   : POSITION;
					float4 color        : COLOR;
					float2 uv           : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct Varyings
				{
					float4  positionCS  : SV_POSITION;
					half4   color       : COLOR;
					float2  uv          : TEXCOORD0;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				TEXTURE2D(_MainTex);
				SAMPLER(sampler_MainTex);
				half4 _MainTex_ST;

				Varyings UnlitVertex(Attributes v)
				{
					Varyings o = (Varyings)0;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

					o.positionCS = TransformObjectToHClip(v.positionOS);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.color = v.color;
					return o;
				}

				half4 UnlitFragment(Varyings i) : SV_Target
				{
					float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
					return mainTex;
				}
				ENDHLSL
			}

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
					float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
					return mainTex;
				}
				ENDHLSL
			}
		}
		Fallback "Sprites/Default"
}

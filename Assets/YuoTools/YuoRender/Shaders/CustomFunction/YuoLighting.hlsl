#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"//函数库：主要用于各种的阴影计算
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl"//函数库：主要用于各种的阴影计算

void YupLighting_half(float3 WorldPos, out half3 Direction, out half3 Color, out half DistanceAtten,
                      out half ShadowAtten)

{
    #ifdef SHADERGRAPH_PREVIEW

   Direction = half3(0.5, 0.5, 0);

   Color = 1;

   DistanceAtten = 1;

   ShadowAtten = 1;

    #else

    #if SHADOWS_SCREEN

   half4 clipPos = TransformWorldToHClip(WorldPos);

   half4 shadowCoord = ComputeScreenPos(clipPos);

    #else

    half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);

    #endif

    Light mainLight = GetMainLight(shadowCoord);

    Direction = mainLight.direction;

    Color = mainLight.color;

    DistanceAtten = mainLight.distanceAttenuation;

    ShadowAtten = mainLight.shadowAttenuation;

    #endif
}

/**
 * 计算光照的函数。
 * 
 * @param worldNormal 物体的世界空间法线。
 * @param viewDir 观察方向。
 * @param lightDirection 光源的方向。
 * @param lightColor 光源的颜色。
 * @param glossiness 光泽度，控制镜面反射的大小。
 * @param rimThreshold 边缘阈值，控制边缘在接近未照亮的表面部分时的平滑混合方式。
 * @param rimAmount 边缘量。
 * @param specularColor 镜面颜色。
 * @param rimColor 边缘颜色。
 * @param ambientColor 环境颜色。
 * @param shadow 阴影值，采样自阴影图，范围在0到1之间，其中0在阴影中，1不在阴影中。
 * 
 * @return 返回计算出的光照颜色。
 */
float4 CalculateLighting(float3 worldNormal, float3 viewDir, float3 lightDirection, float4 lightColor, float glossiness, float rimThreshold, float rimAmount, float4 specularColor, float4 rimColor, float4 ambientColor, float shadow)
{
    float3 normal = normalize(worldNormal);
    float3 viewDirection = normalize(viewDir);

    // 计算来自定向光的照明。
    float NdotL = dot(lightDirection, normal);

    // 将强度划分为亮和暗，平滑地插值
    // 两者之间，以避免出现锯齿状的断裂。
    float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);
    // 乘以主定向光的强度和颜色。
    float4 light = lightIntensity * lightColor;

    // 计算镜面反射。
    float3 halfVector = normalize(lightDirection + viewDirection);
    float NdotH = dot(normal, halfVector);
    float specularIntensity = pow(NdotH * lightIntensity, glossiness * glossiness);
    float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
    float4 specular = specularIntensitySmooth * specularColor;

    // 计算边缘照明。
    float rimDot = 1 - dot(viewDirection, normal);
    float rimIntensity = rimDot * pow(NdotL, rimThreshold);
    rimIntensity = smoothstep(rimAmount - 0.01, rimAmount + 0.01, rimIntensity);
    float4 rim = rimIntensity * rimColor;

    return light + ambientColor + specular + rim;
}
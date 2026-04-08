#ifndef PITFALL_LEGACY_COMMON_INCLUDED
#define PITFALL_LEGACY_COMMON_INCLUDED

#include "UnityCG.cginc"
#include "Lighting.cginc"

inline half LegacyDirectionalLight(float3 worldPos, half3 worldNormal)
{
	half3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
	return saturate(dot(normalize(worldNormal), lightDir) * 0.5h + 0.5h);
}

inline half LegacyMildDirectionalLight(float3 worldPos, half3 worldNormal)
{
	return lerp(0.92h, 1.08h, LegacyDirectionalLight(worldPos, worldNormal));
}

inline half LegacyRecoveredModelLight(float3 worldPos, half3 worldNormal, half floorLift, half wrapLift)
{
	half3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
	half3 normalDir = normalize(worldNormal);
	half wrapped = saturate(dot(normalDir, lightDir) * 0.42h + 0.58h + wrapLift);
	return max(wrapped, floorLift);
}

inline fixed4 SamplePaletteStrip(sampler2D palette, float2 uv)
{
	return tex2D(palette, float2(saturate(uv.x), 0.5));
}

inline half LegacyUvStrength(float2 uv)
{
	return saturate(max(abs(uv.x), abs(uv.y)) * 32.0h);
}

inline fixed3 LiftLegacyVertexColor(fixed3 vertexColor)
{
	vertexColor = saturate(vertexColor);
	return lerp(vertexColor, sqrt(vertexColor), 0.35h);
}

inline fixed SampleRampGrey(sampler2D ramp, half shade)
{
	return tex2D(ramp, float2(saturate(shade), 0.5)).r;
}

inline fixed LegacyRecoveredRamp(sampler2D ramp, half shade, half minBrightness, half maxBrightness)
{
	return lerp(minBrightness, maxBrightness, SampleRampGrey(ramp, shade));
}

inline fixed3 LegacyRecoveredTintLift(fixed3 color, half amount)
{
	return lerp(saturate(color), sqrt(saturate(color)), amount);
}

inline fixed3 LegacyRecoveredNeutralizedTint(fixed3 color, half neutralGrey, half amount)
{
	fixed3 neutralized = saturate(color / max(neutralGrey, 0.001h));
	return lerp(saturate(color), neutralized, amount);
}

inline fixed3 LegacyRecoveredRim(float3 worldPos, half3 worldNormal, fixed3 color, half strength, half power)
{
	half3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
	half rim = pow(saturate(1.0h - dot(normalize(worldNormal), viewDir)), power);
	return color * rim * strength;
}

inline fixed3 LegacyRecoveredAmbientTerm(half3 worldNormal, fixed3 albedo, half strength, half floorLift)
{
	fixed3 ambient = max(ShadeSH9(half4(normalize(worldNormal), 1.0h)), fixed3(floorLift, floorLift, floorLift));
	return albedo * ambient * strength;
}

inline fixed3 LegacyRecoveredSpecularHighlight(float3 worldPos, half3 worldNormal, half gloss, half strength)
{
	half3 normalDir = normalize(worldNormal);
	half3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
	half3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
	half3 halfDir = normalize(lightDir + viewDir);
	half specularPower = lerp(8.0h, 56.0h, saturate(gloss));
	half highlight = pow(saturate(dot(normalDir, halfDir)), specularPower) * strength;
	return highlight.xxx;
}

inline fixed3 ApplyPaletteShading(fixed4 paletteSample, half lightShade)
{
	half paletteBrightness = lerp(0.35h, 1.0h, paletteSample.a);
	half directLight = lerp(0.55h, 1.0h, lightShade);
	return paletteSample.rgb * paletteBrightness * directLight;
}

inline fixed LegacyShadowMultiplier(sampler2D shadowTex, float2 uv)
{
	return lerp(1.0h, 0.62h, tex2D(shadowTex, uv).a);
}

inline fixed3 RecoveredTileColor(fixed4 paletteSample, fixed3 vertexColor, float2 uv)
{
	fixed3 baseColor = LiftLegacyVertexColor(vertexColor);
	half paletteStrength = LegacyUvStrength(uv) * 0.25h;
	fixed3 paletteTint = lerp(fixed3(1.0h, 1.0h, 1.0h), paletteSample.rgb, paletteStrength);
	return baseColor * paletteTint;
}

inline fixed RecoveredTileShadow(sampler2D shadowTex, float2 uv)
{
	half shadowStrength = LegacyUvStrength(uv) * 0.25h;
	return lerp(1.0h, LegacyShadowMultiplier(shadowTex, uv), shadowStrength);
}

inline fixed AlphaFromTextureSample(fixed4 sample)
{
	return max(sample.a, sample.r);
}

#endif

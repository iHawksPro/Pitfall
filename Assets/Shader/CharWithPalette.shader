Shader "Pitfall/CharWithPalette" {
Properties {
 _Color ("Tint", Color) = (1,1,1,1)
 _MainTex ("Palette Texture", 2D) = "white" {}
 _Ramp ("Shading Ramp", 2D) = "white" {}
 _Shininess ("Shininess", Range(0,1)) = 0.35
 _LightColor0 ("Light Color 0", Color) = (1,1,1,1)
 _WorldSpaceLightPos0 ("World Space Light Pos 0", Vector) = (1,0,0,0)
}
 SubShader{
  Tags { "RenderType" = "Opaque" }
  LOD 200
  Pass {
   CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "PitfallLegacyCommon.cginc"
   sampler2D _MainTex;
   sampler2D _Ramp;
   float4 _MainTex_ST;
   fixed4 _Color;
   half _Shininess;
   struct appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
   };
   struct v2f {
    float4 pos : SV_POSITION;
    float3 worldPos : TEXCOORD0;
    half3 worldNormal : TEXCOORD1;
    float2 uv : TEXCOORD2;
   };
   v2f vert(appdata v)
   {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    o.worldNormal = UnityObjectToWorldNormal(v.normal);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    return o;
   }
   fixed4 frag(v2f i) : SV_Target
   {
    fixed4 palette = SamplePaletteStrip(_MainTex, i.uv);
    half lightShade = LegacyRecoveredModelLight(i.worldPos, i.worldNormal, 0.8h, 0.08h);
    fixed ramp = LegacyRecoveredRamp(_Ramp, lightShade, 0.94h, 1.08h);
    fixed3 rawTint = LegacyRecoveredNeutralizedTint(_Color.rgb, 0.5882353h, 0.95h);
    fixed3 tint = LegacyRecoveredTintLift(rawTint, 0.3h);
    fixed3 baseColor = palette.rgb * tint;
    fixed3 ambient = LegacyRecoveredAmbientTerm(i.worldNormal, baseColor, 0.22h, 0.22h);
    fixed3 rim = LegacyRecoveredRim(i.worldPos, i.worldNormal, baseColor, 0.1h, 2.2h);
    fixed3 specular = LegacyRecoveredSpecularHighlight(i.worldPos, i.worldNormal, _Shininess, 0.07h);
    return fixed4(saturate(baseColor * ramp + ambient + rim + specular), _Color.a);
   }
   ENDCG
  }
 }
 Fallback Off
}

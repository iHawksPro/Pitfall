Shader "Pitfall/BasicWithAdditiveRamp" {
Properties {
 _Color ("Base Color", Color) = (1,1,1,1)
 _Ramp ("Shading Ramp", 2D) = "white" {}
 _RampAdditive ("Additive Shading Ramp", 2D) = "black" {}
 _Shininess ("Shininess", Range(0,1)) = 0.35
 _LightColor0 ("Light Color 0", Color) = (1,1,1,1)
}
 SubShader{
  Tags { "RenderType" = "Opaque" }
  LOD 200
  Pass {
   CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "PitfallLegacyCommon.cginc"
   sampler2D _Ramp;
   sampler2D _RampAdditive;
   fixed4 _Color;
   half _Shininess;
   struct appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
   };
   struct v2f {
    float4 pos : SV_POSITION;
    float3 worldPos : TEXCOORD0;
    half3 worldNormal : TEXCOORD1;
   };
   v2f vert(appdata v)
   {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    o.worldNormal = UnityObjectToWorldNormal(v.normal);
    return o;
   }
   fixed4 frag(v2f i) : SV_Target
   {
    half lightShade = LegacyRecoveredModelLight(i.worldPos, i.worldNormal, 0.66h, 0.05h);
    fixed ramp = LegacyRecoveredRamp(_Ramp, lightShade, 0.83h, 1.08h);
    fixed3 baseColor = LegacyRecoveredTintLift(_Color.rgb, 0.36h);
    fixed3 additive = tex2D(_RampAdditive, float2(saturate(lightShade), 0.5)).rgb * lerp(0.22h, 0.32h, lightShade);
    fixed3 ambient = LegacyRecoveredAmbientTerm(i.worldNormal, baseColor, 0.18h, 0.18h);
    fixed3 rim = LegacyRecoveredRim(i.worldPos, i.worldNormal, baseColor, 0.08h, 2.3h);
    fixed3 specular = LegacyRecoveredSpecularHighlight(i.worldPos, i.worldNormal, _Shininess, 0.09h);
    return fixed4(saturate(baseColor * ramp + additive + ambient + rim + specular), _Color.a);
   }
   ENDCG
  }
 }
 Fallback Off
}

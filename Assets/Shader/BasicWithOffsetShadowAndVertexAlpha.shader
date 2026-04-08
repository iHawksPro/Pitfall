Shader "Pitfall/BasicWithOffsetShadowAndVertexAlpha" {
Properties {
 _Color ("Base Color", Color) = (1,1,1,1)
 _Ramp ("Shading Ramp", 2D) = "white" {}
 _Cast ("Cast Shadow", 2D) = "white" {}
 _Shininess ("Shininess", Range(0,1)) = 0.5
}
 SubShader{
  Tags { "Queue"="Transparent" "RenderType" = "Transparent" }
  LOD 200
  Blend SrcAlpha OneMinusSrcAlpha
  ZWrite Off
  Pass {
   CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "PitfallLegacyCommon.cginc"
   sampler2D _Ramp;
   sampler2D _Cast;
   float4 _Cast_ST;
   fixed4 _Color;
   half _Shininess;
   struct appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
    fixed4 color : COLOR;
   };
   struct v2f {
    float4 pos : SV_POSITION;
    float3 worldPos : TEXCOORD0;
    half3 worldNormal : TEXCOORD1;
    float2 uv : TEXCOORD2;
    fixed4 color : COLOR;
   };
   v2f vert(appdata v)
   {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    o.worldNormal = UnityObjectToWorldNormal(v.normal);
    o.uv = TRANSFORM_TEX(v.uv, _Cast);
    o.color = v.color;
    return o;
   }
   fixed4 frag(v2f i) : SV_Target
   {
    half lightShade = LegacyRecoveredModelLight(i.worldPos, i.worldNormal, 0.78h, 0.08h);
    fixed ramp = LegacyRecoveredRamp(_Ramp, lightShade, 0.92h, 1.08h);
    fixed shadow = lerp(1.0h, LegacyShadowMultiplier(_Cast, i.uv), 0.55h);
    fixed alpha = _Color.a * i.color.a;
    fixed3 baseColor = LegacyRecoveredTintLift(_Color.rgb * i.color.rgb, 0.28h);
    fixed3 ambient = LegacyRecoveredAmbientTerm(i.worldNormal, baseColor, 0.18h, 0.18h);
    fixed3 rim = LegacyRecoveredRim(i.worldPos, i.worldNormal, baseColor, 0.12h, 2.0h);
    fixed3 specular = LegacyRecoveredSpecularHighlight(i.worldPos, i.worldNormal, _Shininess, 0.1h);
    fixed3 rgb = saturate((baseColor * ramp + ambient + rim + specular) * shadow);
    return fixed4(rgb, alpha);
   }
   ENDCG
  }
 }
 Fallback Off
}

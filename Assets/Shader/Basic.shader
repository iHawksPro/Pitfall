Shader "Pitfall/Basic" {
Properties {
 _Color ("Base Color", Color) = (1,1,1,1)
 _Ramp ("Shading Ramp", 2D) = "white" {}
 _Shininess ("Shininess", Range(0,1)) = 0.3
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
    half lightShade = LegacyRecoveredModelLight(i.worldPos, i.worldNormal, 0.64h, 0.04h);
    fixed ramp = LegacyRecoveredRamp(_Ramp, lightShade, 0.82h, 1.1h);
    fixed3 baseColor = LegacyRecoveredTintLift(_Color.rgb, 0.42h);
    fixed3 ambient = LegacyRecoveredAmbientTerm(i.worldNormal, baseColor, 0.2h, 0.2h);
    fixed3 rim = LegacyRecoveredRim(i.worldPos, i.worldNormal, baseColor, 0.1h, 2.2h);
    fixed3 specular = LegacyRecoveredSpecularHighlight(i.worldPos, i.worldNormal, _Shininess, 0.08h);
    return fixed4(saturate(baseColor * ramp + ambient + rim + specular), _Color.a);
   }
   ENDCG
  }
 }
 Fallback Off
}

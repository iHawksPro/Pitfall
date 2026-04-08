Shader "Pitfall/TilesWithShadow" {
Properties {
 _Color ("Tint", Color) = (1,1,1,1)
 _MainTex ("Ramp Texture", 2D) = "white" {}
 _ShadowTex ("Shadow Texture", 2D) = "white" {}
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
   sampler2D _MainTex;
   sampler2D _ShadowTex;
   float4 _MainTex_ST;
   float4 _ShadowTex_ST;
   fixed4 _Color;
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
    float2 uvMain : TEXCOORD2;
    float2 uvShadow : TEXCOORD3;
    fixed4 color : COLOR;
   };
   v2f vert(appdata v)
   {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    o.worldNormal = UnityObjectToWorldNormal(v.normal);
    o.uvMain = TRANSFORM_TEX(v.uv, _MainTex);
    o.uvShadow = TRANSFORM_TEX(v.uv, _ShadowTex);
    o.color = v.color;
    return o;
   }
   fixed4 frag(v2f i) : SV_Target
   {
    fixed4 palette = tex2D(_MainTex, i.uvMain);
    fixed3 rgb = RecoveredTileColor(palette, i.color.rgb, i.uvMain);
    rgb *= RecoveredTileShadow(_ShadowTex, i.uvShadow);
    fixed3 tinted = rgb * _Color.rgb;
    fixed3 ambient = LegacyRecoveredAmbientTerm(i.worldNormal, tinted, 0.13h, 0.16h);
    tinted *= LegacyMildDirectionalLight(i.worldPos, i.worldNormal);
    return fixed4(saturate(tinted + ambient), _Color.a);
   }
   ENDCG
  }
 }
 Fallback Off
}

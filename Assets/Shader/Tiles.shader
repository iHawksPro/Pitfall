Shader "Pitfall/Tiles" {
Properties {
 _Color ("Tint", Color) = (1,1,1,1)
 _MainTex ("Palette Texture", 2D) = "white" {}
 _Ramp ("Shading Ramp", 2D) = "white" {}
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
   float4 _MainTex_ST;
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
    float2 uv : TEXCOORD2;
    fixed4 color : COLOR;
   };
   v2f vert(appdata v)
   {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    o.worldNormal = UnityObjectToWorldNormal(v.normal);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.color = v.color;
    return o;
   }
   fixed4 frag(v2f i) : SV_Target
   {
    fixed4 palette = tex2D(_MainTex, i.uv);
    fixed3 rgb = RecoveredTileColor(palette, i.color.rgb, i.uv);
    fixed3 tinted = rgb * _Color.rgb;
    fixed3 ambient = LegacyRecoveredAmbientTerm(i.worldNormal, tinted, 0.12h, 0.14h);
    tinted *= LegacyMildDirectionalLight(i.worldPos, i.worldNormal);
    return fixed4(saturate(tinted + ambient), _Color.a);
   }
   ENDCG
  }
 }
 Fallback Off
}

Shader "Transparent/Cutout/VertexLit" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _SpecColor ("Spec Color", Color) = (1,1,1,0)
 _Emission ("Emissive Color", Color) = (0,0,0,0)
 _Shininess ("Shininess", Range(0.1,1)) = 0.7
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
 _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
 SubShader{
  Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType" = "TransparentCutout" }
  LOD 200
  Pass {
   CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "Lighting.cginc"
   sampler2D _MainTex;
   float4 _MainTex_ST;
   fixed4 _Color;
   float _Cutoff;
   struct appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
   };
   struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 worldPos : TEXCOORD1;
    half3 worldNormal : TEXCOORD2;
   };
   v2f vert(appdata v)
   {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    o.worldNormal = UnityObjectToWorldNormal(v.normal);
    return o;
   }
   fixed4 frag(v2f i) : SV_Target
   {
    fixed4 c = tex2D(_MainTex, i.uv) * _Color;
    clip(c.a - _Cutoff);
    half3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
    fixed lit = saturate(dot(normalize(i.worldNormal), lightDir) * 0.5h + 0.5h);
    c.rgb *= lerp(0.55h, 1.0h, lit);
    return c;
   }
   ENDCG
  }
 }
 Fallback Off
}

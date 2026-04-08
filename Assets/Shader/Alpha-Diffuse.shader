Shader "Transparent/Diffuse" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}
 SubShader{
  Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent" }
  LOD 200
  Blend SrcAlpha OneMinusSrcAlpha
  ZWrite Off
  Pass {
   CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "Lighting.cginc"
   sampler2D _MainTex;
   float4 _MainTex_ST;
   fixed4 _Color;
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
    half3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
    fixed lit = saturate(dot(normalize(i.worldNormal), lightDir) * 0.5h + 0.5h);
    c.rgb *= lerp(0.6h, 1.0h, lit);
    return c;
   }
   ENDCG
  }
 }
 Fallback Off
}

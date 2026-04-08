Shader "Transparent/Cutout/Soft Edge Unlit" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
 _Cutoff ("Base Alpha cutoff", Range(0,0.9)) = 0.5
}
 SubShader{
  Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType" = "TransparentCutout" }
  LOD 200
  Cull Off
  Pass {
   CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
   sampler2D _MainTex;
   float4 _MainTex_ST;
   fixed4 _Color;
   float _Cutoff;
   struct appdata {
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
   };
   struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
   };
   v2f vert(appdata v)
   {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    return o;
   }
   fixed4 frag(v2f i) : SV_Target
   {
    fixed4 c = tex2D(_MainTex, i.uv) * _Color;
    clip(c.a - _Cutoff);
    return c;
   }
   ENDCG
  }
 }
 Fallback Off
}

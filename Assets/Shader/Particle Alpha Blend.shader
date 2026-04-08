Shader "Particles/Alpha Blended" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Particle Texture", 2D) = "white" {}
 _InvFade ("Soft Particles Factor", Range(0.01,3)) = 1
}
 SubShader{
  Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent" }
  Blend SrcAlpha OneMinusSrcAlpha
  ZWrite Off
  Cull Off
  Pass {
   CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
   sampler2D _MainTex;
   float4 _MainTex_ST;
   fixed4 _TintColor;
   struct appdata {
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    fixed4 color : COLOR;
   };
   struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    fixed4 color : COLOR;
   };
   v2f vert(appdata v)
   {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.color = v.color;
    return o;
   }
   fixed4 frag(v2f i) : SV_Target
   {
    return tex2D(_MainTex, i.uv) * _TintColor * i.color;
   }
   ENDCG
  }
 }
 Fallback Off
}

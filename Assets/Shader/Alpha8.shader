Shader "Sprite/Alpha8" {
Properties {
 _Color ("Main Color", Color) = (0,0,0,1)
 _MainTex ("Texture", 2D) = "white" {}
}
 SubShader{
  Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent" }
  LOD 100
  Blend SrcAlpha OneMinusSrcAlpha
  ZWrite Off
  Cull Off
  Pass {
   CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "PitfallLegacyCommon.cginc"
   sampler2D _MainTex;
   float4 _MainTex_ST;
   fixed4 _Color;
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
    fixed4 sample = tex2D(_MainTex, i.uv);
    fixed alpha = AlphaFromTextureSample(sample) * _Color.a * i.color.a;
    fixed3 rgb = _Color.rgb * i.color.rgb;
    return fixed4(rgb, alpha);
   }
   ENDCG
  }
 }
 Fallback Off
}

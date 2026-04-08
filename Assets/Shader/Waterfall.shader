Shader "Pitfall/Waterfall" {
Properties {
 _MainTex ("Main Texture", 2D) = "" {}
 _MainCol ("Main Colour", Color) = (1,1,1,1)
 _MainScrollSpeed ("Main Texture Scroll Speed", Float) = 10
 _BlendTex ("Blend Texture", 2D) = "" {}
 _BlendCol ("Blend Colour", Color) = (1,1,1,1)
 _BlendScrollSpeed ("Blend Texture Scroll Speed", Float) = 20
}
 SubShader{
  Tags { "Queue"="Transparent" "RenderType" = "Transparent" }
  LOD 200
  Blend SrcAlpha OneMinusSrcAlpha
  ZWrite Off
  Cull Off
  Pass {
   CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
   sampler2D _MainTex;
   sampler2D _BlendTex;
   float4 _MainTex_ST;
   float4 _BlendTex_ST;
   fixed4 _MainCol;
   fixed4 _BlendCol;
   float _MainScrollSpeed;
   float _BlendScrollSpeed;
   struct appdata {
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
   };
   struct v2f {
    float4 pos : SV_POSITION;
    float2 uvMain : TEXCOORD0;
    float2 uvBlend : TEXCOORD1;
   };
   v2f vert(appdata v)
   {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uvMain = TRANSFORM_TEX(v.uv, _MainTex);
    o.uvBlend = TRANSFORM_TEX(v.uv, _BlendTex);
    return o;
   }
   fixed4 frag(v2f i) : SV_Target
   {
    float2 mainUv = i.uvMain + float2(0, _Time.y * _MainScrollSpeed * 0.1);
    float2 blendUv = i.uvBlend + float2(0, _Time.y * _BlendScrollSpeed * 0.1);
    fixed4 mainCol = tex2D(_MainTex, mainUv) * _MainCol;
    fixed4 blendCol = tex2D(_BlendTex, blendUv) * _BlendCol;
    fixed4 col = mainCol + blendCol * blendCol.a;
    col.a = saturate(mainCol.a + blendCol.a);
    return col;
   }
   ENDCG
  }
 }
 Fallback Off
}

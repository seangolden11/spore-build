Shader "Custom/OutlineShader"
{
   Properties {
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _Outline ("Outline width", Range (0.01, 0.1)) = 0.03
    }

CGINCLUDE
#include "UnityCG.cginc"

struct appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
};

struct v2f {
    float4 pos : POSITION;
    float4 color : COLOR;
};

uniform float _Outline;
uniform float4 _OutlineColor;

v2f vert(appdata v) {
    v2f o;
    float3 worldNormal = UnityObjectToWorldNormal(v.normal);
v.vertex.xyz += worldNormal * _Outline;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.color = _OutlineColor;
    return o;
}
ENDCG

    SubShader {
        Tags { "DisableBatching" = "True" "Queue"="Geometry-1" }
        Pass {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Front
            ZWrite On
            ZTest Always
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            half4 frag(v2f i) : SV_Target {
                return i.color;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}

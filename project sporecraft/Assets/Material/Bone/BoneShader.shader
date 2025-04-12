Shader "Custom/BoneShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Alpha ("Alpha", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        //Pass 1: 깊이 정보만 기록
        Pass
        {
            ZWrite On
            ColorMask 0
        }

        //Pass 2: 실제 색상 렌더링 (투명)
        CGPROGRAM
        #pragma surface surf Lambert alpha:fade
        sampler2D _MainTex;
        fixed4 _Color;
        float _Alpha;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            tex *= _Color;
            tex.a *= _Alpha;

            o.Albedo = tex.rgb;
            o.Alpha = tex.a;
        }
        ENDCG
    }

    Fallback "Transparent/Diffuse"
}

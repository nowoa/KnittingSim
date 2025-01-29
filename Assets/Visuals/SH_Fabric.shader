Shader "Custom/SH_Fabric"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        [NoScaleOffset] _NormalTex ("Normal Map", 2D) = "bump" {}
        _NormalScale ("Bump Scale", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        // Disable backface culling
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalTex;

        struct Input
        {
            float2 uv_MainTex;
            float vface : VFACE;
            float4 color : COLOR;
        };

        half _Glossiness;
        fixed4 _Color;
        float _NormalScale;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb * IN.color.rgb;
            o.Metallic = 0;
            o.Smoothness = _Glossiness;
            
            if (IN.vface < 0)
            {
                o.Albedo *= 0.8;
                o.Smoothness = 0.0;
            }
            float3 normal = UnpackScaleNormal(tex2D(_NormalTex, IN.uv_MainTex), _NormalScale);
            o.Normal = normal;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
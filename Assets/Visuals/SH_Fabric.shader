Shader "Custom/SH_Fabric"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
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

        struct Input
        {
            float2 uv_MainTex;
            float vface : VFACE;
            float4 color : COLOR;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb * IN.color.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            
            if (IN.vface < 0)
            {
                o.Albedo *= 0.8;
                o.Smoothness = 0.0;
                // o.Metallic = 1.0; // give the backside a velet-like appearance
            }
            
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
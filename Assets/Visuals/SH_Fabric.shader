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
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalTex;

        struct Input
        {
            float2 uv_MainTex;
            float vface : VFACE;
            float4 color : COLOR;
            float hovered;
        };

        half _Glossiness;
        fixed4 _Color;
        float _NormalScale;

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.hovered = v.texcoord1.x;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 mask = tex2D(_MainTex, IN.uv_MainTex).r;
            float4 c = lerp(_Color * 0.2f, _Color, mask);
            o.Albedo = c.rgb * IN.color.rgb;
            o.Metallic = 0;
            o.Smoothness = _Glossiness;

            // if IN.vface
            o.Albedo *= lerp(0.6, 1, IN.vface);
            o.Smoothness *= IN.vface;

            // // if IN.hovered
            // o.Albedo *= lerp(1, 0.3, IN.hovered);
            
            float3 normal = UnpackScaleNormal(tex2D(_NormalTex, IN.uv_MainTex), _NormalScale);
            o.Normal = normal;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
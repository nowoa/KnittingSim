Shader "Custom/SH_Pin"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 color : COLOR;
        };
        
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = (0.9, 0.9, 0.9);
            o.Smoothness = 0.7;
            o.Metallic = 1.0;
            o.Alpha = 1;
            float pinBall = IN.color;
            if(pinBall > 0.5)
            {
                o.Albedo = _Color;
                o.Smoothness = 0.6;
                o.Metallic = 0.0;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}

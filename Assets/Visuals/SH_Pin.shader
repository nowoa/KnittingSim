Shader "Custom/SH_Pin"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Highlight ("Highlight", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 color : COLOR;
            float3 wsPos;
            float3 wsNormal;
        };
        
        fixed4 _Color;
        fixed _Highlight;

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.wsNormal = UnityObjectToWorldNormal(v.normal);
            o.wsPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            IN.wsNormal = normalize(IN.wsNormal);
            
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

            float3 cameraPos = _WorldSpaceCameraPos;
            float3 incoming = normalize(cameraPos - IN.wsPos);
            float fresnel = 1 - dot(IN.wsNormal, incoming);
            fresnel = saturate(pow(fresnel, 3) * 20);

            o.Albedo *= lerp(0.7, 1, _Highlight);

            o.Albedo += fresnel * _Highlight;
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}

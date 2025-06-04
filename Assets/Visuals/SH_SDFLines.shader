Shader "Unlit/SH_SDFLines"
{
    Properties
    {
        // Unused but cannot be removed because the Raw Image requires a texture property
        _MainTex ("-Unused-", 2D) = "white" {}   
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            StructuredBuffer<float2> _Points;
            uint _PointCount;
            float _Thickness;

            float sdLine(float2 p, float2 a, float2 b, float r)
            {
                float2 pa = p - a;
                float2 ba = b - a;
                float h = saturate(dot(pa, ba) / dot(ba, ba));
                return length(pa - ba * h) - r;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                if(_PointCount == 0)
                {
                    return float4(1, 1, 0, 1);
                }
                float dist = 99999;
                for(int i = 0; i < _PointCount - 1; i++)
                {
                    float lineDist = sdLine(IN.uv, _Points[i], _Points[i + 1], _Thickness);
                    dist = min(dist, lineDist);
                }
                float outline = dist < 0;
                if(1 - outline)
                {
                    discard;
                }
                
                return float4(1,0.75,0.7,1);
            }
            ENDCG
        }
    }
}

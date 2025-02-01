Shader "Hidden/SH_CompositeRender"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset] _MaskTex ("Mask Tex", 2D) = "black" {}
        _OutlineColor ("Outline Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _OutlineThickness ("Outline Thickness (Pixel)", Integer) = 2
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
            sampler2D _MaskTex;
            float4 _OutlineColor;
            int _OutlineThickness;
            
            float4 _MaskTex_TexelSize;

            float4 maxFilter(sampler2D tex, float2 uv, float2 texelSize, int kernel = 1)
            {
                float4 maxValue = 0;
                
                for(int y = -kernel; y <= kernel; y++)
                {
                    for(int x = -kernel; x <= kernel; x++)
                    {
                        float2 offset = float2(x, y) * texelSize;
                        float4 sample = tex2D(tex, uv + offset);
                        maxValue = max(maxValue, sample);
                    }
                }
                return maxValue;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                float expandedMask = maxFilter(_MaskTex, i.uv, _MaskTex_TexelSize.xy, _OutlineThickness);
                float mask = tex2D(_MaskTex, i.uv);
                float border = saturate(expandedMask - mask);
                col = lerp(col, _OutlineColor, border);
                return col;
            }
            ENDCG
        }
    }
}

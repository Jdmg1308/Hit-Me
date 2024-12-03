Shader "Custom/DebugUV"
{
    SubShader
    {
        Tags { "RenderType"="Overlay" "Queue"="Overlay" }
        Pass
        {
            Name "DebugUVPass"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;

                // Transform vertex to clip space
                o.pos = TransformObjectToHClip(v.vertex);

                // Compute UV coordinates (normalized screen space)
                o.uv = o.pos.xy / o.pos.w * 0.5 + 0.5;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return float4(i.uv.x, i.uv.y, 0, 1); // UVs as red-green gradient
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/InternalErrorShader"
}

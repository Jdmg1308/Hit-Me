Shader "Custom/WiggleWobbleScreenEffect"
{
    Properties
    {
        _WaveStrength ("Wave Strength", Range(0, 1)) = 0.1
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 1.0
        _TimeScale ("Time Scale", Range(0, 10)) = 1.0
        _MainTex ("Screen Texture", 2D) = "white" {} // Added screen texture
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" "RenderPipeline"="UniversalRenderPipeline" }
        LOD 100

        Pass
        {
            Name "WiggleWobblePass"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION; // Object space position
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION; // Homogeneous Clip Space position
                float2 screenUV : TEXCOORD0; // Normalized screen UV coordinates
            };

            // Properties
            float _WaveStrength;
            float _WaveSpeed;
            float _TimeScale;

            // Screen Texture
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Vertex Shader
            Varyings vert(Attributes v)
            {
                Varyings o;

                // Transform the object space position to homogeneous clip space
                o.positionHCS = TransformObjectToHClip(v.positionOS);

                // Compute normalized screen UVs from clip space
                o.screenUV = o.positionHCS.xy / o.positionHCS.w * 0.5 + 0.5;

                return o;
            }
            // Fragment Shader
            half4 frag(Varyings i) : SV_Target
            {
                float time = _Time.y * _TimeScale;
            
                // Correct the UVs to fix the vertical flip
                float2 correctedUV = float2(i.screenUV.x, 1.0 - i.screenUV.y);
            
                // Distort the corrected UV coordinates
                float xOffset = sin(correctedUV.y * 10.0 + time * _WaveSpeed) * _WaveStrength;
                float yOffset = cos(correctedUV.x * 10.0 + time * _WaveSpeed) * _WaveStrength;
                float2 distortedUV = correctedUV + float2(xOffset, yOffset);
            
                // Sample the screen texture with distorted UVs
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/InternalErrorShader"
}

Shader "Custom/WiggleWobbleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaveStrength ("Wave Strength", Range(0, 1)) = 0.1
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 1.0
        _TimeScale ("Time Scale", Range(0, 10)) = 1.0
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

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _WaveStrength;
            float _WaveSpeed;
            float _TimeScale;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float xOffset = sin(i.uv.y * 10.0 + _Time.y * _WaveSpeed * _TimeScale) * _WaveStrength;
                float yOffset = cos(i.uv.x * 10.0 + _Time.y * _WaveSpeed * _TimeScale) * _WaveStrength;
                float2 uv = i.uv + float2(xOffset, yOffset);
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}

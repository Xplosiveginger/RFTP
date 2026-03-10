Shader "Custom/HSBShift"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _Hue("Hue Shift", Range(-1,1)) = 0
        _Saturation("Saturation", Range(-1,1)) = 0
        _Tint("Tint Color", Color) = (1,1,1,1)
        _Invert("Invert", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "UniversalMaterialType" = "Unlit" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "HSBPass"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.5

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            float4 _MainTex_ST;
            float _Hue;
            float _Saturation;
            float4 _Tint;
            float _Invert;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            float3 RGBtoHSV(float3 c)
            {
                float4 K = float4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 HSVtoRGB(float3 c)
            {
                float4 K = float4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - 1.0), c.y);
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                float mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, IN.uv).r;

                float3 hsv = RGBtoHSV(col.rgb);
                hsv.x = frac(hsv.x + _Hue);
                hsv.y = saturate(hsv.y + _Saturation);

                if (_Invert > 0.5)
                    hsv.z = 1.0 - hsv.z;

                float3 rgb = HSVtoRGB(hsv) * _Tint.rgb;
                float alpha = col.a * _Tint.a;

                float3 finalRGB = lerp(col.rgb, rgb, mask);
                float finalAlpha = lerp(col.a, alpha, mask);

                return float4(finalRGB, finalAlpha);
            }

            ENDHLSL
        }
    }

    FallBack Off
}

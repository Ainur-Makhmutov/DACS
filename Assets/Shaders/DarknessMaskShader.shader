Shader "Custom/DarknessMaskShader"
{
    Properties
    {
        _Color("Overlay Color", Color) = (0, 0, 0, 0.6)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }

        // Настройка прозрачности
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        // Читаем Stencil
        Stencil
        {
            Ref 1
            Comp NotEqual
            Pass Keep
            Fail Keep
            ZFail Keep
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float4 _Color;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }
    }
}

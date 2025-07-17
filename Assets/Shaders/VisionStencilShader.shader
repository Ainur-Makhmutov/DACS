Shader "Custom/VisionStencilShader"
{
    Properties
    {
        _Color("Color", Color) = (0,0.5,1,0.3)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        // Включаем прозрачность
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        // Настройки Stencil
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
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

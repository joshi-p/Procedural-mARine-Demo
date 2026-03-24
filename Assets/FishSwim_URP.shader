Shader "Custom/FishSwimURP"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _Amplitude ("Swim Amplitude", Float) = 0.2
        _Frequency ("Wave Frequency", Float) = 4
        _Speed ("Swim Speed", Float) = 2
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float _Amplitude;
            float _Frequency;
            float _Speed;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _PhaseOffset)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                float phase = UNITY_ACCESS_INSTANCED_PROP(Props,_PhaseOffset);

                float3 pos = IN.positionOS.xyz;

                float time = _Time.y * _Speed + phase;

                float bodyPos = pos.z;

                float wave = sin(time + bodyPos * _Frequency);

                pos.x += wave * _Amplitude * bodyPos;

                OUT.positionHCS = TransformObjectToHClip(pos);
                OUT.uv = IN.uv;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                return SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
            }

            ENDHLSL
        }
    }
}
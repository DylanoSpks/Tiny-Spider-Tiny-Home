Shader "Hidden/Edge Detection"
{
    Properties
    {
        _OutlineThickness ("Outline Thickness", Float) = 1
        _OutlineColor     ("Outline Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "EDGE DETECTION OUTLINE"
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            float _OutlineThickness;
            float4 _OutlineColor;

            #pragma vertex Vert  // Provided by Blit.hlsl
            #pragma fragment frag

            float RobertsCross(float3 samples[4])
            {
                float3 d1 = samples[1] - samples[2];
                float3 d2 = samples[0] - samples[3];
                return sqrt(dot(d1, d1) + dot(d2, d2));
            }
            float RobertsCross(float samples[4])
            {
                float d1 = samples[1] - samples[2];
                float d2 = samples[0] - samples[3];
                return sqrt(d1 * d1 + d2 * d2);
            }
            float3 SampleSceneNormalsRemapped(float2 uv)
            {
                return SampleSceneNormals(uv) * 0.5 + 0.5;
            }
            float SampleSceneLuminance(float2 uv)
            {
                float3 col = SampleSceneColor(uv);
                return col.r * 0.3 + col.g * 0.59 + col.b * 0.11;
            }

            half4 frag(Varyings IN) : SV_TARGET
            {
                float2 uv = IN.texcoord;
                float2 texelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);

                const float half_w_f = floor(_OutlineThickness * 0.5);
                const float half_w_c = ceil(_OutlineThickness * 0.5);

                float2 uvs[4];
                uvs[0] = uv + texelSize * float2(-half_w_f,  half_w_c); // top-left
                uvs[1] = uv + texelSize * float2( half_w_c,  half_w_c); // top-right
                uvs[2] = uv + texelSize * float2(-half_w_f, -half_w_f); // bot-left
                uvs[3] = uv + texelSize * float2( half_w_c, -half_w_f); // bot-right

                float depth_samples[4];
                float3 normal_samples[4];
                float  lum_samples[4];
                for (int i = 0; i < 4; i++)
                {
                    depth_samples[i]  = SampleSceneDepth(uvs[i]);
                    normal_samples[i] = SampleSceneNormalsRemapped(uvs[i]);
                    lum_samples[i]    = SampleSceneLuminance(uvs[i]);
                }

                float edgeDepth     = RobertsCross(depth_samples);
                float edgeNormal    = RobertsCross(normal_samples);
                float edgeLuminance = RobertsCross(lum_samples);

                float depthT  = 1.0 / 200.0; // tweak these if desired
                float normalT = 1.0 /   4.0;
                float lumT    = 1.0 /   0.5;

                edgeDepth     = (edgeDepth     > depthT)  ? 1.0 : 0.0;
                edgeNormal    = (edgeNormal    > normalT) ? 1.0 : 0.0;
                edgeLuminance = (edgeLuminance > lumT)    ? 1.0 : 0.0;

                float edge = max(edgeDepth, max(edgeNormal, edgeLuminance));
                return edge * _OutlineColor;
            }
            ENDHLSL
        }
    }
}

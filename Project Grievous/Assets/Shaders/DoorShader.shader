Shader "Custom/DoorShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        [NoScaleOffset] _FlowMap("Flow (RG, A noise)", 2D) = "black" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Bump("Normal", 2D) = "bump" {}
        _Tiling("Tiling", Float) = 1
        _Speed("Speed", Float) = 1
        _Cube("Reflection Map", Cube) = "" {}
    }
        SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0

        sampler2D _MainTex, _FlowMap;
        float _Tiling, _Speed;
        uniform samplerCUBE _Cube;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        half3 FlowUVW(half2 uv, half2 flowVector, half tiling, half time, bool flowB) {
            half phaseOffset = flowB ? 0.5 : 0;
            half progress = frac(time + phaseOffset);
            half3 uvw;
            uvw.xy = uv - flowVector * progress;
            uvw.xy *= tiling;
            uvw.xy += phaseOffset;
            uvw.z = 1 - abs(1 - 2 * progress);
            return uvw;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            half2 flowVector = tex2D(_FlowMap, IN.uv_MainTex).rg * 2 - 1;
            half noise = tex2D(_FlowMap, IN.uv_MainTex).a;
            half time = _Time.y * _Speed + noise;
            half3 uvwA = FlowUVW(
                IN.uv_MainTex, flowVector, _Tiling, time, false
            );
            half3 uvwB = FlowUVW(
                IN.uv_MainTex, flowVector, _Tiling, time, true
            );
            fixed4 texA = tex2D(_MainTex, uvwA.xy) * uvwA.z;
            fixed4 texB = tex2D(_MainTex, uvwB.xy) * uvwB.z;

            fixed4 c = (texA + texB) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = clamp(c.a, 0.5, 1);
        }
        ENDCG
    }
        FallBack "Diffuse"
}

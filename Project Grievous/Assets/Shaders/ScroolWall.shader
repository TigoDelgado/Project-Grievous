Shader "Custom/ScroolWall"
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
        Tags {"RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard
        #pragma target 3.0

        sampler2D _MainTex, _FlowMap;
        float _Tiling, _Speed;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        float2 FlowUV(float2 uv, float time) {
            float2 uv_ = uv;
            uv_.x = uv.x + time;
            return uv_;
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {
            float time = _Time.y * _Speed;
            float2 uv = FlowUV(IN.uv_MainTex, time);
            fixed4 c = tex2D(_MainTex, uv) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

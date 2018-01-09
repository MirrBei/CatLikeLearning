﻿Shader "Custom/MyFirstLightingShader" {
    Properties {
        _Tint("Tint", Color) = (1, 1, 1, 1)
        _MainTex("Texture", 2D) = "white" {}
    }

    SubShader {
        Pass {
            CGPROGRAM

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"

            struct VertexData {                
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            void MyVertexProgram(VertexData v, out Interpolators i) {
                i.position = UnityObjectToClipPos(v.position);
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
            }

            void MyFragmentProgram(Interpolators i, out float4 color : SV_TARGET) {
                color = tex2D(_MainTex, i.uv) * _Tint;
            }

            ENDCG
        }
    }
}

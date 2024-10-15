// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/My First Shader"
{
    Properties
    {
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader // can have multiple for different platforms like mobile, ios, etc
    {
        Pass //always need at least one pass but can have multiple
        {
            CGPROGRAM
            #pragma vertex MyVertexProgram //pragma - direct actions of the compiler
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc" //shader include file, includes shadervariables, hlsl support, and unity instancing.

            float4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct Interpolaters
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                //float3 localPosition : TEXCOORD0;
            };

            struct VertexData
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            Interpolaters MyVertexProgram (VertexData v)
            {
                Interpolaters i;
                //i.localPosition = v.position.xyz;
                i.position = UnityObjectToClipPos(v.position);
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return i;                                //Did use mul(UNITY_MATRIX_MVP, position) but Unity
                                                        //now uses two separate matrices to do the transformation into clip space
            }

            float4 MyFragmentProgram (Interpolaters i) : SV_Target
            {
                return tex2D(_MainTex, i.uv) * _Tint; //text 2D - macro that takes uv, multiplies by scale, and adds offset
            }
            ENDCG
        }
    }
    
}
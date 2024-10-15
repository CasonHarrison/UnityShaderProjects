// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Textured With Detail"
{
    Properties
    {
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {} //dont need these brackets but compiler does, legacy feature
        _DetailTex ("Detail Texture", 2D) = "gray" {}
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
            sampler2D _MainTex, _DetailTex;
            float4 _MainTex_ST, _DetailTex_ST;

            struct Interpolaters
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvDetail : TEXCOORD1;
            };

            struct VertexData
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            Interpolaters MyVertexProgram (VertexData v)
            {
                Interpolaters i;
                i.position = UnityObjectToClipPos(v.position);
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                i.uvDetail = TRANSFORM_TEX(v.uv, _DetailTex);
                return i;                                //Did use mul(UNITY_MATRIX_MVP, position) but Unity
                                                        //now uses two separate matrices to do the transformation into clip space
            }

            float4 MyFragmentProgram (Interpolaters i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv) * _Tint;//text 2D - macro that takes uv, multiplies by scale, and adds offset
                color *= tex2D(_MainTex, i.uvDetail) * unity_ColorSpaceDouble; //UnityCG unifrom variable helps to center values around 1
                return color;                                                  //depending on color space
            }
            ENDCG
        }
    }
    
}
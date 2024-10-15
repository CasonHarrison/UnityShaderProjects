// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Texture Splatting"
{
    Properties
    {
        _MainTex ("Splat Map", 2D) = "white" {}
        [NoScaleOffset] _Texture1 ("Texture 1", 2D) = "white" {}
        [NoScaleOffset] _Texture2 ("Texture 2", 2D) = "white" {}
        [NoScaleOffset] _Texture3 ("Texture 3", 2D) = "white" {}
        [NoScaleOffset] _Texture4 ("Texture 4", 2D) = "white" {}
    }
    SubShader // can have multiple for different platforms like mobile, ios, etc
    {
        Pass //always need at least one pass but can have multiple
        {
            CGPROGRAM
            #pragma vertex MyVertexProgram //pragma - direct actions of the compiler
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc" //shader include file, includes shadervariables, hlsl support, and unity instancing.
            
            sampler2D _MainTex, _Texture1, _Texture2, _Texture3, _Texture4;
            float4 _MainTex_ST;

            struct Interpolaters
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvSplat : TEXCOORD1;
            };

            struct VertexData
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            Interpolaters MyVertexProgram (VertexData v)
            {
                Interpolaters i;
                i.position = UnityObjectToClipPos(v.position);//Did use mul(UNITY_MATRIX_MVP, position) but Unity
                                                        //now uses two separate matrices to do the transformation into clip space
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                i.uvSplat = v.uv;
                return i;                              
            }

            float4 MyFragmentProgram (Interpolaters i) : SV_Target
            {
                float4 splat = tex2D(_MainTex, i.uvSplat);
                return tex2D(_Texture1, i.uv) * splat.r
                + tex2D(_Texture2, i.uv) * splat.g
                + tex2D(_Texture3, i.uv) * splat.b
                + tex2D(_Texture4, i.uv) * (1 - splat.r - splat.g - splat.b);//text 2D - macro that takes uv, multiplies by scale, and adds offset
            }
            ENDCG
        }
    }
    
}
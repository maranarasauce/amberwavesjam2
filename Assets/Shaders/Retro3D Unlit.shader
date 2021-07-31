Shader "Retro3D/Unlit"
{
    Properties
    {
        _MainTex("Base", 2D) = "white" {}
        _Color("Color", Color) = (0.5, 0.5, 0.5, 1)
        _ScrollAmount("Texture Scroll Speed", Vector) = (0, 0, 0, 1)
        _GeoRes("Geometric Resolution", Float) = 40
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Pass
        {
            Tags{"QUEUE" = "AlphaTest" "RenderType" = "TransparentCutout" }
            AlphaToMask On
            ColorMask RGB

            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex vert 
            #pragma fragment frag
            #pragma multi_compile_fog
            #define USING_FOG (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))
             #pragma multi_compile __ LIGHTMAP_ON

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 uv0 : TEXCOORD0;
                float3 uv1 : TEXCOORD1;
#if USING_FOG
                fixed fog : TEXCOORD2;
#endif
            };

            struct appdata_lightmap {
                float4 vertex : POSITION;
                float3 texcoord : TEXCOORD2;
                float3 texcoord1 : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _ScrollAmount;
            float _GeoRes;

            v2f vert(appdata_base v, appdata_lightmap i)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                float4 wp = mul(UNITY_MATRIX_MV, v.vertex);
                wp.xyz = floor(wp.xyz * _GeoRes) / _GeoRes;

                float4 sp = mul(UNITY_MATRIX_P, wp);
                o.position = sp;

                float4 scroll = _ScrollAmount * _Time;
                float2 uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv0 = float3(uv + scroll, 1);
#if LIGHTMAP_ON
                float2 uv1 = i.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                o.uv1 = float3(uv1, 1);
#endif

#if USING_FOG
                float3 eyePos = UnityObjectToViewPos(i.vertex);
                float fogCoord = length(eyePos.xyz);  // radial fog distance
                UNITY_CALC_FOG_FACTOR_RAW(fogCoord);
                o.fog = saturate(unityFogFactor);
#endif

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv0.xy / i.uv0.z;
                fixed4 main_color = tex2D(_MainTex, uv) * _Color * 2;
#if LIGHTMAP_ON
                main_color.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv1));
#endif
#if USING_FOG
                main_color.rgb = lerp(unity_FogColor.rgb, main_color.rgb, i.fog);
#endif
                return main_color;
            }

            ENDCG
        }
    }
}

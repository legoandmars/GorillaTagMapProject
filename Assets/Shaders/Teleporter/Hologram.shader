// Shader created with Shader Forge v1.40 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.40;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,cpap:True,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33059,y:32749,varname:node_3138,prsc:2|emission-2977-RGB,alpha-3451-OUT;n:type:ShaderForge.SFN_Tex2d,id:4713,x:31826,y:32558,varname:node_4713,prsc:2,tex:5d8c54bbf99e5764e894ad4dd3a05320,ntxv:0,isnm:False|TEX-4233-TEX;n:type:ShaderForge.SFN_Tex2d,id:6079,x:32112,y:33280,varname:node_6079,prsc:2,tex:d5189c59707f94c44b864753a0dfc2cf,ntxv:0,isnm:False|UVIN-1591-OUT,TEX-2993-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:4233,x:31631,y:32541,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_4233,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5d8c54bbf99e5764e894ad4dd3a05320,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2dAsset,id:2993,x:31750,y:33557,ptovrint:False,ptlb:Gradient,ptin:_Gradient,varname:node_2993,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:d5189c59707f94c44b864753a0dfc2cf,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Code,id:9525,x:32000,y:32468,varname:node_9525,prsc:2,code:cgBlAHQAdQByAG4AIAAwAC4AMgA5ADkAIAAqACAAYwBvAGwAbwByAC4AcgAgACsAIAAwAC4ANQA4ADcAIAAqACAAYwBvAGwAbwByAC4AZwAgACsAIAAwAC4AMQAxADQAIAAqACAAYwBvAGwAbwByAC4AYgA7AA==,output:0,fname:RGBtoGrayscale,width:617,height:260,input:2,input_1_label:color|A-4713-RGB;n:type:ShaderForge.SFN_Color,id:2977,x:32751,y:32686,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_2977,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Time,id:5103,x:31276,y:33221,varname:node_5103,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:4648,x:31080,y:33368,ptovrint:False,ptlb:Gradient U speed,ptin:_GradientUspeed,varname:node_4648,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:2697,x:31080,y:33452,ptovrint:False,ptlb:Gradient V speed,ptin:_GradientVspeed,varname:_GradientUspeed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Append,id:510,x:31276,y:33368,varname:node_510,prsc:2|A-4648-OUT,B-2697-OUT;n:type:ShaderForge.SFN_Add,id:1591,x:31813,y:33229,varname:node_1591,prsc:2|A-7247-OUT,B-8489-OUT;n:type:ShaderForge.SFN_Multiply,id:8489,x:31603,y:33229,varname:node_8489,prsc:2|A-5103-T,B-510-OUT;n:type:ShaderForge.SFN_Multiply,id:3451,x:32773,y:33029,varname:node_3451,prsc:2|A-9525-OUT,B-6079-R;n:type:ShaderForge.SFN_FragmentPosition,id:3870,x:31050,y:32917,varname:node_3870,prsc:2;n:type:ShaderForge.SFN_Append,id:7247,x:31615,y:33080,varname:node_7247,prsc:2|A-7818-OUT,B-3870-Y;n:type:ShaderForge.SFN_Vector1,id:7818,x:31050,y:32825,varname:node_7818,prsc:2,v1:0;proporder:4233-2993-2977-4648-2697;pass:END;sub:END;*/

Shader "Shader Forge/Hologram" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Gradient ("Gradient", 2D) = "white" {}
        _Color ("Color", Color) = (1,0,0,1)
        _GradientUspeed ("Gradient U speed", Float ) = 0
        _GradientVspeed ("Gradient V speed", Float ) = 0.1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Gradient; uniform float4 _Gradient_ST;
            float RGBtoGrayscale( float3 color ){
            return 0.299 * color.r + 0.587 * color.g + 0.114 * color.b;
            }
            
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float4, _Color)
                UNITY_DEFINE_INSTANCED_PROP( float, _GradientUspeed)
                UNITY_DEFINE_INSTANCED_PROP( float, _GradientVspeed)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _Color_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Color );
                float3 emissive = _Color_var.rgb;
                float3 finalColor = emissive;
                float4 node_4713 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float node_9525 = RGBtoGrayscale( node_4713.rgb );
                float4 node_5103 = _Time;
                float _GradientUspeed_var = UNITY_ACCESS_INSTANCED_PROP( Props, _GradientUspeed );
                float _GradientVspeed_var = UNITY_ACCESS_INSTANCED_PROP( Props, _GradientVspeed );
                float2 node_1591 = (float2(0.0,i.posWorld.g)+(node_5103.g*float2(_GradientUspeed_var,_GradientVspeed_var)));
                float4 node_6079 = tex2D(_Gradient,TRANSFORM_TEX(node_1591, _Gradient));
                return fixed4(finalColor,(node_9525*node_6079.r));
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

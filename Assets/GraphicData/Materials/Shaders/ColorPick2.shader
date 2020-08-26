Shader "Custom/ColorPick2"
{

    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}


        [HDR]
		_AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)

		//[HDR]
		//_SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
		// Controls the size of the specular reflection.
		//_Glossiness("Glossiness", Range(0, 100)) = 32

		[HDR]
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716

		// Control how smoothly the rim blends when approaching unlit parts of the surface.
		_RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Pass
        {
            Tags
			{
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}
            LOD 200
        
            blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM


            #pragma vertex vert
			#pragma fragment frag
            
			// Compile multiple versions of this shader depending on lighting settings.
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			// Files below include macros and functions to assist
			// with lighting and shadows.
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

            
            struct appdata
			{
				float4 vertex : POSITION;				
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 viewDir : TEXCOORD1;	
				
				SHADOW_COORDS(2)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
            sampler2D _MaskTex;
			float4 _MaskTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);		
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				TRANSFER_SHADOW(o)
				return o;
			}
			
			float4 _Color;

			float4 _AmbientColor;

			//float4 _SpecularColor;
			//float _Glossiness;		

			float4 _RimColor;
			float _RimAmount;
			float _RimThreshold;

            float4 frag (v2f i) : SV_Target
			{
				float3 normal = normalize(i.worldNormal);
				float3 viewDir = normalize(i.viewDir);
				float NdotL = dot(_WorldSpaceLightPos0, normal);

				// calculate shadow light
				// where 0 is in the shadow, and 1 is not.
				float shadow = SHADOW_ATTENUATION(i);
				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);
				float4 light = lightIntensity * _LightColor0;

				// Calculate rim light
				float rimDot = 1 - dot(viewDir, normal);
				float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
				float4 rim = rimIntensity * _RimColor;

				/*
				// Calculate specular reflection.
				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);
				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
				float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
				float4 specular = specularIntensitySmooth * _SpecularColor;
				*/
				
				fixed4 col = tex2D (_MainTex, i.uv);
				float mask = tex2D (_MaskTex, i.uv).r;

				col.rgb = col.rgb * (1 - mask) + _Color * mask;
				//col.rgb *= (light + specular + _AmbientColor + rim).rgb;
				col.rgb *= (light + _AmbientColor + rim).rgb;
				
				col.a = _Color.a;

				return col;
			}
            

            ENDCG
        }        
		// Shadow casting support.
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }

}

		#include "UnityStandardBRDF.cginc"
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			float3 worldRefl;
			INTERNAL_DATA
			float2 uv2_texcoord2;
			float3 worldPos;
			float4 screenPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float4 _EmissiveColor;
		uniform sampler2D _EmissiveTex;
		uniform float4 _EmissiveTex_ST;
		uniform float _EmissiveStrength;
		uniform sampler2D _ShadowRamp;
		uniform sampler2D _Normal;
		uniform float _UseUV2forNormalsSpecular;
		uniform float4 _SimulatedLightDirection;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _ColorTint;
		uniform float _RimWidth;
		uniform float3 _Xiexe;
		uniform float _RimIntensity;
		uniform sampler2D _SpecularMap;
		uniform sampler2D _SpecularPattern;
		uniform float2 _SpecularPatternTiling;
		uniform float _SpecularIntensity;
		uniform float _SpecularArea;
		uniform float _Cutoff;
		uniform float _RimlightType;
		uniform float _RampDir;
		uniform float _NormalTiling;
		uniform float _ShadowIntensity;
		uniform float _DitherScale;
		uniform float _ColorBanding;
		uniform float _ReflSmoothness;
		uniform float _Metallic;
		uniform sampler2D _MetallicMap;
		uniform sampler2D _RoughMap;
		uniform samplerCUBE _BakedCube;
		uniform float _UseReflections;
		uniform float _UseOnlyBakedCube;
		uniform float _ShadowType;
		uniform float _ReflType;
		uniform float _StylelizedIntensity;
		uniform float _Saturation;


		float3 ShadeSH9( float3 normal )
		{
			return ShadeSH9(half4(normal, 1.0));
		}

		float3 StereoWorldViewDir( float3 worldPos )
		{
			#if UNITY_SINGLE_PASS_STEREO
			float3 cameraPos = float3((unity_StereoWorldSpaceCameraPos[1]+ unity_StereoWorldSpaceCameraPos[1])*.5); 
			#else
			float3 cameraPos = _WorldSpaceCameraPos;
			#endif
			float3 worldViewDir = normalize((cameraPos - worldPos));
			return worldViewDir;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;

			half4 c = 0;
		//light and show attenuation
			#if DIRECTIONAL
			float steppedAtten = round(data.atten);
			float ase_lightAtten = lerp(steppedAtten, data.atten, _ShadowType);//data.atten;
			#else
			float3 ase_lightAttenRGB = smoothstep(0, 0.1, (gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 )));
			float ase_lightAtten = (max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b ));
			#endif

		//assign the first and second texture coordinates
			float2 texcoord1 = i.uv_texcoord;// * float2( 1,1 ) + float2( 0,0 );
			float2 texcoord2 = i.uv2_texcoord2;// * float2( 1,1 ) + float2( 0,0 );
			
		//set up uvs for all main texture maps
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;

		
		//swap UV sets based on if we're using UV2 or not
			float2 UVSet = lerp(texcoord1,texcoord2,_UseUV2forNormalsSpecular);
			float3 normalMap = UnpackNormal( tex2D( _Normal, UVSet));
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float4 worldNormals = mul(unity_ObjectToWorld,float4( ase_vertexNormal , 0.0 ));
			float4 lerpedNormals = lerp( float4( WorldNormalVector( i , normalMap ) , 0.0 ) , worldNormals , 0.3);
			float4 vertexNormals = lerpedNormals;

			float3 shadeSH9 = ShadeSH9(float4(0,0,0,1));
			float3 lightColor = (_LightColor0); 


			float3 ase_worldPos = i.worldPos;
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			float4 simulatedLight = normalize( _SimulatedLightDirection );

		//figure out whether we are in a realtime lighting scnario, or baked, and return it as a 0, or 1 (1 for realtime, 0 for baked)
			float light_Env = float(any(_WorldSpaceLightPos0.xyz));

		//we use the simulated light direction if we're in a baked scenario
			float4 light_Dir = simulatedLight;

		//otherwise, we use the actual light direction
			if( light_Env == 1)
			{
				light_Dir = float4( ase_worldlightDir , 0.0 );
			}

			float NdotL = dot( vertexNormals , float4( light_Dir.xyz , 0.0 ) );
			float remappedRamp = ( ( NdotL + 1.0 ) * 0.5 );
			
			float2 horizontalRamp = (float2(remappedRamp , 0.0));
			float2 verticalRamp = (float2(0.0 , remappedRamp));
			
			float4 ase_vertex4Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 vertexWorldPos = mul(unity_ObjectToWorld,ase_vertex4Pos);
			float4 objPos = mul(unity_ObjectToWorld, float4(0,0,0,1));
			float3 stereoWorldViewDir = StereoWorldViewDir(vertexWorldPos);
			float VdotN = dot(vertexNormals, float4(stereoWorldViewDir, 0.0));

		//rimlight typing
			float smoothRim = (smoothstep(0, 0.9, pow((1.0 - saturate(VdotN)), (1.0 - _RimWidth))) * _RimIntensity);
			float sharpRim = (step(0.9, pow((1.0 - saturate(VdotN)), (1.0 - _RimWidth))) * _RimIntensity);
			float FinalRimLight = lerp(sharpRim, smoothRim, _RimlightType);


		//grab the reflection probes in the area
			float3 reflectedDir = reflect(-stereoWorldViewDir, vertexNormals);
		
		#ifdef _REFLECTIONS_ON
			float4 reflection = (0,0,0,0);
			float3 reflectionDir = reflect(-light_Dir, vertexNormals);
			float4 metalMap = (tex2D(_MetallicMap, uv_MainTex) * _Metallic);
			#ifdef _PBRREFL_ON
				float4 roughMap = tex2D(_RoughMap, uv_MainTex);
				reflection = (UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectedDir, ((_ReflSmoothness * 7) * roughMap.r)));
				
				if (any(reflection.xyz) == 0)
				{
					reflection = texCUBElod(_BakedCube, float4(reflectedDir, ((_ReflSmoothness * 7) * roughMap.r)));
				}
			#endif
			#ifdef _STYLIZEDREFLECTION_ON
				float reflectionUntouched = step(0.9, (pow(DotClamped(stereoWorldViewDir, reflectionDir), ((1-_ReflSmoothness))))) * metalMap;
				reflection = (round(reflectionUntouched * 10) / 10);
			#endif
		#endif


		//Recieved Shadows and lighting
			float4 shadowRamp = tex2D( _ShadowRamp, float2(remappedRamp,remappedRamp));
            //float realtimeShadowMask = shadowRamp - smoothstep(1, 0, (shadowRamp.r));
            float finalShadow = saturate((ase_lightAtten * .5) - (1-shadowRamp.r));
			float realtimeShadows = saturate(((1-finalShadow)));

		//We default to baked lighting situations, so we use these values
			float3 indirectLight = shadeSH9;
			float3 finalLight = indirectLight * (shadowRamp + ((1-_ShadowIntensity) * (1-shadowRamp)));

		//If our lighting environment matches the number for realtime lighting, use these numbers instead
			if (light_Env == 1) 
			{
				indirectLight = shadeSH9;
				lightColor = ((lightColor * finalShadow));
				finalLight = (lightColor + indirectLight);
			}
		//get the main texture and multiply it by the color tint, and do saturation on the main texture
			float4 MainTex194 = pow(tex2D( _MainTex, uv_MainTex ), _Saturation);
			float4 MainColor = MainTex194 * _ColorTint;
		
		//grab the specular map texture sample, get the dot product of the vertex normals vs the stereo correct view direction, and create specular reflections and a rimlight based on that and a texture we feed in.
			float4 specularMap = tex2D( _SpecularMap, UVSet );
			float NdotV = dot(reflect(light_Dir , vertexNormals), float4((stereoWorldViewDir * -1.0), 0.0));
		//clean this
			float specularRefl = (((specularMap.g * (1.0 - specularMap.r)) * tex2D(_SpecularPattern, (((UVSet - float2( 0.5,0.5)) * _SpecularPatternTiling) + float2(0.5,0.5))).r) * (_SpecularIntensity * 2) * saturate(pow(saturate(NdotV) , _SpecularArea)));
		
		//calculate the final lighting for our lighting model
			float3 finalAddedLight = ( (FinalRimLight + (specularRefl)) * saturate((saturate(MainColor + 0.5) * pow(finalLight, 2) * (shadowRamp)))).rgb;
		    float3 finalColor = MainColor;

		//if we have reflections turned on, return the final color with reflections
			#ifdef _REFLECTIONS_ON
				#ifdef _PBRREFL_ON
					float3 finalreflections = (reflection * (MainColor * 2));
					finalColor = (MainColor * ((1-_Metallic) * (1-metalMap.r))) + finalreflections;
				#endif
				#ifdef _STYLIZEDREFLECTION_ON
					finalColor = MainColor + ((reflection * ((MainColor) * finalLight)) * _StylelizedIntensity);
				#endif
			#endif

		//return the RGB of all the stuff from above as c	
			c.rgb = finalColor * (finalLight + finalAddedLight);
		
		//get the alpha, based on if we have cutout, or alphablending enabled from our editor script, and finally return everything
            #ifdef opaque
        		c.a = 1;
            #endif
			
		//alphablend
            #ifdef alphablend
				c.a = (MainTex194.a * _ColorTint.a);
            #endif

		//cutout
			#ifdef cutout
				clip(MainTex194.a - _Cutoff);
			#endif

		//dithered
			#ifdef dithered
				 // Screen-door transparency: Discard pixel if below threshold.
    			float4x4 thresholdMatrix =
    			{  1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
    			  13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
    			   4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
    			  16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
   				};
   				float4x4 _RowAccess = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };
				float2 screenPos = i.screenPos.xy;
				float2 pos = screenPos / i.screenPos.w;
				pos *= _ScreenParams.xy; // pixel position
   					
				 #ifdef UNITY_SINGLE_PASS_STEREO
				 	clip((MainTex194.a * _ColorTint.a) - thresholdMatrix[fmod((pos.x * 2), 4)] * _RowAccess[fmod(pos.y, 4)]);
				 #else
					clip((MainTex194.a * _ColorTint.a) - thresholdMatrix[fmod(pos.x, 4)] * _RowAccess[fmod(pos.y, 4)]);
				 #endif

			#endif

			return c;
		}
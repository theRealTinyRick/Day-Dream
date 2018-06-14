// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/LowPolyWater V2/ADVANCED"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Optimisations)] [Toggle]_PerVertexSpecular("Per Vertex Specular", Int) = 0
		[Toggle]_DisableReflection("Disable Reflection", Int) = 0
		[HideInInspector]_ReflectionTex("ReflectionTex", 2D) = "black" {}
		[Header(Surface Control)] _WaterColor("Water Color", Color) = (0.1952851,0.4153261,0.6323529,1)
		_WaterSpecular("Water Specular", Range( 0 , 10)) = 3
		_WaterGloss("Water Gloss", Range( 0 , 10)) = 3
		_SmoothNormals("Smooth Normals", Range( 0 , 1)) = 0.5
		[Header(Reflection and Refraction)][Toggle]_UseReflectionProbe("Use Reflection Probe", Int) = 0
		_FresnelPower("Fresnel Power", Range( 0 , 10)) = 2
		_DepthOffset("Depth Offset", Float) = 1
		_DepthFalloff("Depth Falloff", Float) = 3
		_AbsorptionColor("Absorption Color", Color) = (0,0.751724,1,1)
		_AbsorptionIntensity("Absorption Intensity", Range( 0 , 10)) = 2
		[Header(Edge Control)] _EdgeColor("Edge Color", Color) = (1,1,1,1)
		_EdgeIntensity("Edge Intensity", Range( 0 , 1)) = 1
		_EdgeOffset("Edge Offset", Float) = 0.95
		_EdgeFalloff("Edge Falloff", Float) = 10
		[IntRange]_EdgeLevels("Edge Levels", Range( 1 , 10)) = 3
		[Header(Wave Control)] _WaveHeight("Wave Height", Float) = 0.5
		_WaveCycles("Wave Cycles", Float) = 1.5
		_WaveSpeed("Wave Speed", Float) = 25
		_WaveDirectionZX("Wave Direction Z-X", Range( 0 , 1)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		LOD 200
		Cull Back
		GrabPass{ "GrabScreen1" }
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature _USEREFLECTIONPROBE_ON
		#pragma shader_feature _DISABLEREFLECTION_ON
		#pragma shader_feature _PERVERTEXSPECULAR_ON
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nometa vertex:vertexDataFunc 
		struct Input
		{
			float2 data553;
			float4 screenPos;
			float3 worldNormal;
			INTERNAL_DATA
			float3 data541;
			float2 data567;
			float data574;
			float3 worldPos;
			float4 data451;
			float4 data441;
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

		uniform fixed4 _WaterColor;
		uniform sampler2D GrabScreen1;
		uniform fixed _SmoothNormals;
		uniform float _DepthOffset;
		uniform sampler2D _CameraDepthTexture;
		uniform float _DepthFalloff;
		uniform fixed4 _AbsorptionColor;
		uniform float _AbsorptionIntensity;
		uniform sampler2D _ReflectionTex;
		uniform float _FresnelPower;
		uniform float _WaterGloss;
		uniform float _WaterSpecular;
		uniform fixed4 _EdgeColor;
		uniform fixed _EdgeOffset;
		uniform float _EdgeFalloff;
		uniform float _EdgeLevels;
		uniform fixed _EdgeIntensity;
		uniform fixed _WaveDirectionZX;
		uniform float _WaveSpeed;
		uniform float _WaveCycles;
		uniform float _WaveHeight;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			float4 ase_screenPos108 = ase_screenPos;
			float2 componentMask234 = ase_screenPos108.xy;
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 lerpResult6 = lerp( ase_worldNormal , fixed3(0,1,0) , _SmoothNormals);
			fixed3 NORMALS648 = lerpResult6;
			float2 componentMask117 = NORMALS648.xz;
			o.data553 = ( ( componentMask234 + componentMask117 ) / ase_screenPos108.w );
			float3 localBoxMax517517 = ( unity_SpecCube0_BoxMax.xyz );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 normalizeResult519 = normalize( ( ase_worldPos - _WorldSpaceCameraPos ) );
			float3 temp_output_522_0 = reflect( normalizeResult519 , NORMALS648 );
			float3 localBoxMin516516 = ( unity_SpecCube0_BoxMin.xyz );
			float3 localProbePosition533533 = ( unity_SpecCube0_ProbePosition.xyz );
			o.data541 = ( ( ( min( min( max( ( ( localBoxMax517517 - ase_worldPos ) / temp_output_522_0 ) , ( ( localBoxMin516516 - ase_worldPos ) / temp_output_522_0 ) ).x , max( ( ( localBoxMax517517 - ase_worldPos ) / temp_output_522_0 ) , ( ( localBoxMin516516 - ase_worldPos ) / temp_output_522_0 ) ).y ) , max( ( ( localBoxMax517517 - ase_worldPos ) / temp_output_522_0 ) , ( ( localBoxMin516516 - ase_worldPos ) / temp_output_522_0 ) ).z ) * temp_output_522_0 ) + ase_worldPos ) - localProbePosition533533 );
			float4 ase_screenPos14 = ase_screenPos;
			float2 componentMask15 = ase_screenPos14.xy;
			float2 componentMask45 = NORMALS648.xz;
			o.data567 = ( ( componentMask15 + componentMask45 ) / ase_screenPos14.w );
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelFinalVal129 = (0.0 + 1.0*pow( 1.0 - dot( fixed3(0,1,0), worldViewDir ) , _FresnelPower));
			o.data574 = abs( fresnelFinalVal129 );
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			float dotResult449 = dot( NORMALS648 , ase_worldlightDir );
			float3 indirectDiffuse468 = ShadeSH9( float4( ase_worldNormal, 1 ) );
			o.data451 = ( ( max( dotResult449 , 0.0 ) * _LightColor0 ) + float4( indirectDiffuse468 , 0.0 ) );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 normalizeResult430 = normalize( ( ase_worldlightDir + ase_worldViewDir ) );
			float dotResult432 = dot( NORMALS648 , normalizeResult430 );
			float4 temp_output_440_0 = ( ( pow( max( dotResult432 , 0.0 ) , ( _WaterGloss * 128.0 ) ) * _WaterSpecular ) * _LightColor0 );
			#ifdef _PERVERTEXSPECULAR_ON
			float4 staticSwitch655 = temp_output_440_0;
			#else
			float4 staticSwitch655 = float4( 0.0,0,0,0 );
			#endif
			o.data441 = staticSwitch655;
			float lerpResult266 = lerp( ase_worldPos.z , ase_worldPos.x , _WaveDirectionZX);
			float3 appendResult647 = (float3(0.0 , _WaveCycles , 0.0));
			float3 WAVEMOTION98 = ( sin( ( ( lerpResult266 + ( _Time.x * _WaveSpeed ) ) * appendResult647 ) ) * ( _WaveHeight * 0.1 ) );
			v.vertex.xyz += WAVEMOTION98;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#if DIRECTIONAL
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			fixed4 screenColor115 = tex2D( GrabScreen1, i.data553 );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 lerpResult6 = lerp( ase_worldNormal , fixed3(0,1,0) , _SmoothNormals);
			fixed3 NORMALS648 = lerpResult6;
			float2 componentMask414 = NORMALS648.xz;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPos161 = ase_screenPos;
			float eyeDepth162 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(( float4( componentMask414, 0.0 , 0.0 ) + ase_screenPos161 )))));
			fixed DEPTH358 = saturate( pow( ( _DepthOffset + abs( ( eyeDepth162 - ase_screenPos161.w ) ) ) , ( 1.0 - max( _DepthFalloff , 1.0 ) ) ) );
			float4 lerpResult499 = lerp( _WaterColor , screenColor115 , DEPTH358);
			float4 lerpResult362 = lerp( lerpResult499 , ( _AbsorptionColor * max( _AbsorptionIntensity , 1.0 ) ) , ( DEPTH358 * min( _AbsorptionIntensity , 1.0 ) ));
			fixed4 REFRACTION152 = lerpResult362;
			float3 reflectDir536 = i.data541;
			float4 localTEXCUBE_LOD536536 = ( UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectDir536,0) );
			float4 reflectionHDR537 = localTEXCUBE_LOD536536;
			float3 localDecodeHDR537537 = ( DecodeHDR(reflectionHDR537, unity_SpecCube0_HDR) );
			#ifdef _USEREFLECTIONPROBE_ON
			float4 staticSwitch540 = float4( localDecodeHDR537537 , 0.0 );
			#else
			float4 staticSwitch540 = tex2D( _ReflectionTex, i.data567 );
			#endif
			fixed4 REFLECTION25 = staticSwitch540;
			fixed FRESNEL304 = i.data574;
			float4 lerpResult146 = lerp( REFRACTION152 , REFLECTION25 , FRESNEL304);
			#ifdef _DISABLEREFLECTION_ON
			float4 staticSwitch556 = REFRACTION152;
			#else
			float4 staticSwitch556 = lerpResult146;
			#endif
			fixed4 ALBEDO155 = staticSwitch556;
			fixed4 LIGHTING_LAMBERT450 = ( i.data451 * ase_lightAtten );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 normalizeResult430 = normalize( ( ase_worldlightDir + ase_worldViewDir ) );
			float dotResult432 = dot( NORMALS648 , normalizeResult430 );
			float4 temp_output_440_0 = ( ( pow( max( dotResult432 , 0.0 ) , ( _WaterGloss * 128.0 ) ) * _WaterSpecular ) * _LightColor0 );
			#ifdef _PERVERTEXSPECULAR_ON
			float4 staticSwitch500 = i.data441;
			#else
			float4 staticSwitch500 = temp_output_440_0;
			#endif
			fixed4 LIGHTING_SPECULAR455 = ( ase_lightAtten * staticSwitch500 );
			float eyeDepth409 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos161))));
			float temp_output_264_0 = (0.0 + (floor( ( saturate( pow( ( _EdgeOffset + abs( ( eyeDepth409 - ase_screenPos161.w ) ) ) , ( 1.0 - max( _EdgeFalloff , 0.0 ) ) ) ) * _EdgeLevels ) ) - 0.0) * (1.0 - 0.0) / (_EdgeLevels - 0.0));
			fixed EDGE246 = ( temp_output_264_0 * _EdgeIntensity );
			float4 lerpResult492 = lerp( ( saturate( ( ALBEDO155 * LIGHTING_LAMBERT450 ) ) + LIGHTING_SPECULAR455 ) , ( _EdgeColor * ase_lightAtten ) , EDGE246);
			c.rgb = lerpResult492.rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
	}
	Fallback "Unlit/Color"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13106
1927;29;1906;1014;2279.932;328.2905;1;True;False
Node;AmplifyShaderEditor.WorldSpaceCameraPos;513;-1920,-1776;Float;False;0;1;FLOAT3
Node;AmplifyShaderEditor.WorldPosInputsNode;512;-1920,-1920;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;9;-1920,704;Fixed;False;Property;_SmoothNormals;Smooth Normals;6;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;265;-1920,384;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;104;-1920,528;Fixed;False;Constant;_Vector0;Vector 0;6;0;0,1,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;6;-1536,384;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;514;-1664,-1920;Float;False;2;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.NormalizeNode;519;-1504,-1920;Float;False;1;0;FLOAT3;0.0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;652;-1920,-1664;Float;False;648;0;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;648;-1344,384;Fixed;False;NORMALS;-1;True;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.CustomExpressionNode;516;-1280,-1600;Float;False;unity_SpecCube0_BoxMin.xyz;3;False;0;BoxMin;0;1;FLOAT3
Node;AmplifyShaderEditor.CustomExpressionNode;517;-1280,-1920;Float;False;unity_SpecCube0_BoxMax.xyz;3;False;0;BoxMax;0;1;FLOAT3
Node;AmplifyShaderEditor.WorldPosInputsNode;515;-1280,-1856;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;520;-1280,-1536;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ReflectOpNode;522;-1280,-1696;Float;False;2;0;FLOAT3;0.0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;521;-1088,-1920;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;523;-1088,-1600;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleDivideOpNode;525;-896,-1792;Float;False;2;0;FLOAT3;0.0;False;1;FLOAT3;0.0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleDivideOpNode;524;-896,-1616;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;651;-1920,-3456;Float;False;648;0;1;FLOAT3
Node;AmplifyShaderEditor.ScreenPosInputsNode;161;-1920,-3328;Float;False;1;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMaxOp;526;-640,-1920;Float;False;2;0;FLOAT3;0.0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.ComponentMaskNode;414;-1696,-3456;Float;False;True;False;True;False;1;0;FLOAT3;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;415;-1408,-3456;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT4;0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.BreakToComponentsNode;527;-512,-1920;Float;False;FLOAT3;1;0;FLOAT3;0.0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ScreenDepthNode;162;-1280,-3456;Float;False;0;1;0;FLOAT4;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMinNode;528;-256,-1920;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;654;-1920,-448;Float;False;648;0;1;FLOAT3
Node;AmplifyShaderEditor.ScreenPosInputsNode;108;-1920,-640;Float;False;1;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;163;-1024,-3360;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMinNode;529;-128,-1920;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RelayNode;530;-512,-1696;Float;False;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;356;-544,-3328;Float;False;Property;_DepthFalloff;Depth Falloff;10;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;117;-1728,-448;Float;False;True;False;True;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.ComponentMaskNode;234;-1728,-640;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;353;-896,-3456;Float;False;Property;_DepthOffset;Depth Offset;9;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ScreenPosInputsNode;14;-1920,-1152;Float;False;1;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;653;-1920,-960;Float;False;648;0;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;610;-1488,-640;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;427;-1920,-4768;Float;False;1;0;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.AbsOpNode;257;-880,-3360;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;531;64,-1920;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.WorldPosInputsNode;532;16,-1776;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;426;-1920,-4688;Float;False;World;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMaxOp;373;-288,-3328;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;428;-1664,-4768;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.OneMinusNode;370;-144,-3328;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;45;-1728,-960;Float;False;True;False;True;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.CustomExpressionNode;533;208,-1712;Float;False;unity_SpecCube0_ProbePosition.xyz;3;False;0;ProbePosition;0;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;354;-496,-3456;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;15;-1728,-1152;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;534;224,-1920;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleDivideOpNode;112;-1328,-512;Float;False;2;0;FLOAT2;0.0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;608;-1472,-1152;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.VertexToFragmentNode;553;-1184,-512;Float;False;1;0;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.NormalizeNode;430;-1536,-4768;Float;False;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.PowerNode;355;96,-3456;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;650;-1920,-4864;Float;False;648;0;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;535;384,-1920;Float;False;2;0;FLOAT3;0.0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;649;-1920,-5248;Float;False;648;0;1;FLOAT3
Node;AmplifyShaderEditor.ScreenColorNode;115;-992,-512;Fixed;False;Global;GrabScreen1;Grab Screen 1;-1;0;Object;-1;True;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;131;-1696,-4624;Float;False;Property;_WaterGloss;Water Gloss;5;0;3;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;446;-1920,-5152;Float;False;1;0;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.ScreenDepthNode;409;-1280,-3200;Float;False;0;1;0;FLOAT4;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;416;-768,-192;Float;False;Property;_AbsorptionIntensity;Absorption Intensity;12;0;2;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;20;-1328,-1024;Float;False;2;0;FLOAT2;0.0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SaturateNode;367;352,-3456;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.VertexToFragmentNode;541;544,-1920;Float;False;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;149;-1920,-3776;Float;False;Property;_FresnelPower;Fresnel Power;8;0;2;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.DotProductOpNode;432;-1328,-4864;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;576;-1920,-3968;Fixed;False;Constant;_Vector1;Vector 1;26;0;0,1,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;431;-1328,-4624;Float;False;Constant;_Float2;Float 2;1;0;128;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.CustomExpressionNode;536;752,-1920;Float;False;UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectDir,0);4;False;1;True;reflectDir;FLOAT3;0,0,0;In;TEXCUBE_LOD;1;0;FLOAT3;0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ColorNode;420;-768,-448;Fixed;False;Property;_AbsorptionColor;Absorption Color;11;0;0,0.751724,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMaxOp;642;-352,-304;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.DotProductOpNode;449;-1664,-5248;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;223;-768,-640;Fixed;False;Property;_WaterColor;Water Color;3;0;0.1952851,0.4153261,0.6323529,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;417;-768,-272;Float;False;358;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMinNode;640;-352,-208;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;411;-1024,-3072;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;358;640,-3456;Fixed;False;DEPTH;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;168;-544,-3008;Float;False;Property;_EdgeFalloff;Edge Falloff;16;0;10;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.FresnelNode;129;-1600,-3968;Float;False;4;0;FLOAT3;0,1,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.5;False;1;FLOAT
Node;AmplifyShaderEditor.VertexToFragmentNode;567;-1152,-1024;Float;False;1;0;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;433;-1168,-4720;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;128.0;False;1;FLOAT
Node;AmplifyShaderEditor.RelayNode;644;-512,-512;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMaxOp;434;-1200,-4864;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CustomExpressionNode;537;960,-1920;Float;False;DecodeHDR(reflectionHDR, unity_SpecCube0_HDR);3;False;1;True;reflectionHDR;FLOAT4;0,0,0,0;In;DecodeHDR;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;258;-1120,-4624;Float;False;Property;_WaterSpecular;Water Specular;4;0;3;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;410;-880,-3072;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;499;-128,-640;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.PowerNode;435;-1024,-4864;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;641;-128,-512;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LightColorNode;448;-1344,-5184;Float;False;0;1;COLOR
Node;AmplifyShaderEditor.SimpleMaxOp;454;-1536,-5248;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMaxOp;226;-320,-3024;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;419;-128,-416;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;22;-896,-1152;Float;True;Property;_ReflectionTex;ReflectionTex;2;1;[HideInInspector];None;True;0;False;black;Auto;False;Object;-1;MipBias;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;222;-1360,-3968;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;170;-896,-3200;Fixed;False;Property;_EdgeOffset;Edge Offset;15;0;0.95;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;169;-496,-3200;Float;False;2;2;0;FLOAT;0.0,0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;599;-144,-3024;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.IndirectDiffuseLighting;468;-1344,-5104;Float;False;Tangent;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.StaticSwitch;540;1280,-1184;Float;False;Property;_UseReflectionProbe;Use Reflection Probe;7;0;0;False;True;;2;0;FLOAT4;0.0;False;1;FLOAT4;0.0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;362;192,-640;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.VertexToFragmentNode;574;-1232,-3968;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;445;-1216,-5248;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;438;-864,-4864;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LightColorNode;452;-784,-4624;Float;False;0;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;440;-704,-4864;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;1584,-1184;Fixed;False;REFLECTION;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RegisterLocalVarNode;304;-1024,-3968;Fixed;False;FRESNEL;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;153;-1920,-2336;Float;False;25;0;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;308;-1920,-2240;Float;False;304;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;448,-640;Fixed;False;REFRACTION;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.PowerNode;294;96,-3200;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;447;-1024,-5248;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT3;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;154;-1920,-2432;Float;False;152;0;1;COLOR
Node;AmplifyShaderEditor.StaticSwitch;655;-512,-4864;Float;False;Property;_PerVertexSpecular;Per Vertex Specular;0;0;0;False;True;;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;146;-1536,-2288;Float;False;3;0;FLOAT4;0.0,0,0,0;False;1;FLOAT4;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LightAttenuation;461;0,-5184;Float;False;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;262;224,-3008;Float;False;Property;_EdgeLevels;Edge Levels;17;1;[IntRange];3;1;10;0;1;FLOAT
Node;AmplifyShaderEditor.VertexToFragmentNode;451;-208,-5248;Float;False;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SaturateNode;260;288,-3200;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;267;-1920,1184;Fixed;False;Property;_WaveDirectionZX;Wave Direction Z-X;21;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TimeNode;79;-1920,1264;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;80;-1920,1440;Float;False;Property;_WaveSpeed;Wave Speed;20;0;25;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;268;-1920,1024;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RelayNode;656;-512,-4752;Float;False;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;261;544,-3200;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0,0;False;1;FLOAT
Node;AmplifyShaderEditor.StaticSwitch;556;-1280,-2432;Float;False;Property;_DisableReflection;Disable Reflection;1;0;0;False;True;;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.VertexToFragmentNode;441;-208,-4864;Float;False;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;266;-1632,1104;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;160;-1664,1440;Float;False;Property;_WaveCycles;Wave Cycles;19;0;1.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;456;320,-5248;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-1664,1264;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;92;-1472,1200;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LightAttenuation;511;0,-4864;Float;False;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;463;-1920,-6192;Float;False;450;0;1;COLOR
Node;AmplifyShaderEditor.DynamicAppendNode;647;-1456,1408;Float;False;FLOAT3;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;156;-1920,-6272;Float;False;155;0;1;COLOR
Node;AmplifyShaderEditor.FloorOpNode;263;688,-3200;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;450;480,-5248;Fixed;False;LIGHTING_LAMBERT;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StaticSwitch;500;0,-4784;Float;False;Property;_PerVertexSpecular;Per Vertex Specular;0;0;0;False;True;;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;155;-896,-2432;Fixed;False;ALBEDO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;56;-1280,1440;Float;False;Property;_WaveHeight;Wave Height;18;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;264;832,-3200;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-1280,1200;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;444;320,-4864;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;462;-1600,-6272;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;395;832,-3008;Fixed;False;Property;_EdgeIntensity;Edge Intensity;14;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LightAttenuation;603;-1280,-5984;Float;False;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;210;-1280,-6160;Fixed;False;Property;_EdgeColor;Edge Color;13;0;1,1,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;393;1184,-3200;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;645;-1440,-6272;Float;False;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SinOpNode;53;-1088,1200;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;220;-1104,1440;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.1;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;455;496,-4864;Fixed;False;LIGHTING_SPECULAR;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;464;-1920,-6112;Float;False;455;0;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-896,1200;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;465;-1280,-6272;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;246;1376,-3200;Fixed;False;EDGE;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;602;-1056,-6160;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;491;-1280,-5888;Float;False;246;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;492;-768,-6272;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;158;-768,-5952;Float;False;98;0;1;FLOAT3
Node;AmplifyShaderEditor.OneMinusNode;397;1024,-3200;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;98;-704,1200;Float;False;WAVEMOTION;-1;True;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-384,-6272;Float;False;True;2;Float;ASEMaterialInspector;200;0;CustomLighting;BOXOPHOBIC/LowPolyWater V2/ADVANCED;False;False;False;False;True;True;True;True;True;False;True;False;False;False;True;True;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;0;SrcAlpha;OneMinusSrcAlpha;0;SrcAlpha;OneMinusSrcAlpha;OFF;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;200;Unlit/Color;-1;-1;-1;-1;0;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;277;-2176,-640;Float;False;100;100;;0;// REFRECTION;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;458;-2176,-4864;Float;False;100;100;;0;// LIGHTING SPECULAR;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;510;-2176,-5248;Float;False;100;100;;0;// LIGHTING LAMBERT;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;276;-2176,-1152;Float;False;100;100;;0;// REFLECTION MIRROR;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;303;-2176,-3968;Float;False;100;100;;0;// FRESNEL CONTROL;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;279;-2176,1024;Float;False;100;100;;0;// WAVE MOTION;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;272;-2176,-2432;Float;False;100;100;;0;// ALBEDO;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;280;-2176,-6272;Float;False;100;100;;0;// FINAL;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;457;-2176,384;Float;False;100;100;;0;// NORMALS;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;274;-2176,-3456;Float;False;100;100;;0;// DEPTH AND EDGE CONTROL;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;539;-2176,-1920;Float;False;100;100;;0;// REFLECTION CUBEMAP;1,1,1,1;0;0
WireConnection;6;0;265;0
WireConnection;6;1;104;0
WireConnection;6;2;9;0
WireConnection;514;0;512;0
WireConnection;514;1;513;0
WireConnection;519;0;514;0
WireConnection;648;0;6;0
WireConnection;522;0;519;0
WireConnection;522;1;652;0
WireConnection;521;0;517;0
WireConnection;521;1;515;0
WireConnection;523;0;516;0
WireConnection;523;1;520;0
WireConnection;525;0;521;0
WireConnection;525;1;522;0
WireConnection;524;0;523;0
WireConnection;524;1;522;0
WireConnection;526;0;525;0
WireConnection;526;1;524;0
WireConnection;414;0;651;0
WireConnection;415;0;414;0
WireConnection;415;1;161;0
WireConnection;527;0;526;0
WireConnection;162;0;415;0
WireConnection;528;0;527;0
WireConnection;528;1;527;1
WireConnection;163;0;162;0
WireConnection;163;1;161;4
WireConnection;529;0;528;0
WireConnection;529;1;527;2
WireConnection;530;0;522;0
WireConnection;117;0;654;0
WireConnection;234;0;108;0
WireConnection;610;0;234;0
WireConnection;610;1;117;0
WireConnection;257;0;163;0
WireConnection;531;0;529;0
WireConnection;531;1;530;0
WireConnection;373;0;356;0
WireConnection;428;0;427;0
WireConnection;428;1;426;0
WireConnection;370;0;373;0
WireConnection;45;0;653;0
WireConnection;354;0;353;0
WireConnection;354;1;257;0
WireConnection;15;0;14;0
WireConnection;534;0;531;0
WireConnection;534;1;532;0
WireConnection;112;0;610;0
WireConnection;112;1;108;4
WireConnection;608;0;15;0
WireConnection;608;1;45;0
WireConnection;553;0;112;0
WireConnection;430;0;428;0
WireConnection;355;0;354;0
WireConnection;355;1;370;0
WireConnection;535;0;534;0
WireConnection;535;1;533;0
WireConnection;115;0;553;0
WireConnection;409;0;161;0
WireConnection;20;0;608;0
WireConnection;20;1;14;4
WireConnection;367;0;355;0
WireConnection;541;0;535;0
WireConnection;432;0;650;0
WireConnection;432;1;430;0
WireConnection;536;0;541;0
WireConnection;642;0;416;0
WireConnection;449;0;649;0
WireConnection;449;1;446;0
WireConnection;640;0;416;0
WireConnection;411;0;409;0
WireConnection;411;1;161;4
WireConnection;358;0;367;0
WireConnection;129;0;576;0
WireConnection;129;3;149;0
WireConnection;567;0;20;0
WireConnection;433;0;131;0
WireConnection;433;1;431;0
WireConnection;644;0;115;0
WireConnection;434;0;432;0
WireConnection;537;0;536;0
WireConnection;410;0;411;0
WireConnection;499;0;223;0
WireConnection;499;1;644;0
WireConnection;499;2;417;0
WireConnection;435;0;434;0
WireConnection;435;1;433;0
WireConnection;641;0;420;0
WireConnection;641;1;642;0
WireConnection;454;0;449;0
WireConnection;226;0;168;0
WireConnection;419;0;417;0
WireConnection;419;1;640;0
WireConnection;22;1;567;0
WireConnection;222;0;129;0
WireConnection;169;0;170;0
WireConnection;169;1;410;0
WireConnection;599;0;226;0
WireConnection;540;0;537;0
WireConnection;540;1;22;0
WireConnection;362;0;499;0
WireConnection;362;1;641;0
WireConnection;362;2;419;0
WireConnection;574;0;222;0
WireConnection;445;0;454;0
WireConnection;445;1;448;0
WireConnection;438;0;435;0
WireConnection;438;1;258;0
WireConnection;440;0;438;0
WireConnection;440;1;452;0
WireConnection;25;0;540;0
WireConnection;304;0;574;0
WireConnection;152;0;362;0
WireConnection;294;0;169;0
WireConnection;294;1;599;0
WireConnection;447;0;445;0
WireConnection;447;1;468;0
WireConnection;655;0;440;0
WireConnection;146;0;154;0
WireConnection;146;1;153;0
WireConnection;146;2;308;0
WireConnection;451;0;447;0
WireConnection;260;0;294;0
WireConnection;656;0;440;0
WireConnection;261;0;260;0
WireConnection;261;1;262;0
WireConnection;556;0;154;0
WireConnection;556;1;146;0
WireConnection;441;0;655;0
WireConnection;266;0;268;3
WireConnection;266;1;268;1
WireConnection;266;2;267;0
WireConnection;456;0;451;0
WireConnection;456;1;461;0
WireConnection;82;0;79;1
WireConnection;82;1;80;0
WireConnection;92;0;266;0
WireConnection;92;1;82;0
WireConnection;647;1;160;0
WireConnection;263;0;261;0
WireConnection;450;0;456;0
WireConnection;500;0;441;0
WireConnection;500;1;656;0
WireConnection;155;0;556;0
WireConnection;264;0;263;0
WireConnection;264;2;262;0
WireConnection;84;0;92;0
WireConnection;84;1;647;0
WireConnection;444;0;511;0
WireConnection;444;1;500;0
WireConnection;462;0;156;0
WireConnection;462;1;463;0
WireConnection;393;0;264;0
WireConnection;393;1;395;0
WireConnection;645;0;462;0
WireConnection;53;0;84;0
WireConnection;220;0;56;0
WireConnection;455;0;444;0
WireConnection;57;0;53;0
WireConnection;57;1;220;0
WireConnection;465;0;645;0
WireConnection;465;1;464;0
WireConnection;246;0;393;0
WireConnection;602;0;210;0
WireConnection;602;1;603;0
WireConnection;492;0;465;0
WireConnection;492;1;602;0
WireConnection;492;2;491;0
WireConnection;397;0;264;0
WireConnection;98;0;57;0
WireConnection;0;2;492;0
WireConnection;0;11;158;0
ASEEND*/
//CHKSM=12A049B8C0C28E913EC1E1757E273144207196D2
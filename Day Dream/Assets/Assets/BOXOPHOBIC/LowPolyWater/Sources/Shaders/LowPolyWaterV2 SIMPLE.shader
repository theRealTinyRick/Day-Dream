// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/LowPolyWater V2/SIMPLE"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Optimisations)] [Toggle]_PerVertexSpecular("Per Vertex Specular", Int) = 0
		[Toggle]_DisableReflection("Disable Reflection", Int) = 0
		[HideInInspector]_ReflectionTex("ReflectionTex", 2D) = "black" {}
		[Header(Surface Control)] _WaterColor("Water Color", Color) = (0.1952852,0.4153263,0.6323529,1)
		_WaterSpecular("Water Specular", Range( 0 , 10)) = 1
		_WaterGloss("Water Gloss", Range( 0 , 10)) = 3
		_SmoothNormals("Smooth Normals", Range( 0 , 1)) = 0.5
		[Header(Reflection and Refraction)] _FresnelPower("Fresnel Power", Range( 0 , 10)) = 2
		_DepthOffset("Depth Offset", Float) = 1
		_DepthFalloff("Depth Falloff", Float) = 3
		_AbsorptionColor("Absorption Color", Color) = (0,0.751724,1,1)
		_AbsorptionIntensity("Absorption Intensity", Range( 0 , 10)) = 2
		[Header(Edge Control)] _EdgeColor("Edge Color", Color) = (1,1,1,1)
		_EdgeIntensity("Edge Intensity", Range( 0 , 1)) = 1
		_EdgeOffset("Edge Offset", Float) = 0.8
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
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature _DISABLEREFLECTION_ON
		#pragma shader_feature _PERVERTEXSPECULAR_ON
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
			float2 data567;
			float data606;
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
		uniform fixed4 _AbsorptionColor;
		uniform float _AbsorptionIntensity;
		uniform float _DepthOffset;
		uniform sampler2D _CameraDepthTexture;
		uniform fixed _SmoothNormals;
		uniform float _DepthFalloff;
		uniform sampler2D _ReflectionTex;
		uniform float _FresnelPower;
		uniform float _WaterGloss;
		uniform float _WaterSpecular;
		uniform fixed4 _EdgeColor;
		uniform float _EdgeOffset;
		uniform fixed _EdgeIntensity;
		uniform fixed _WaveDirectionZX;
		uniform float _WaveSpeed;
		uniform float _WaveCycles;
		uniform float _WaveHeight;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			float4 ase_screenPos14 = ase_screenPos;
			float2 componentMask15 = ase_screenPos14.xy;
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 lerpResult6 = lerp( ase_worldNormal , fixed3(0,1,0) , _SmoothNormals);
			fixed3 NORMALS636 = lerpResult6;
			float2 componentMask45 = NORMALS636.xz;
			o.data567 = ( ( componentMask15 + componentMask45 ) / ase_screenPos14.w );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelFinalVal604 = (0.0 + 1.0*pow( 1.0 - dot( fixed3(0,1,0), worldViewDir ) , _FresnelPower));
			o.data606 = abs( fresnelFinalVal604 );
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			float dotResult449 = dot( NORMALS636 , ase_worldlightDir );
			float3 indirectDiffuse468 = ShadeSH9( float4( ase_worldNormal, 1 ) );
			o.data451 = ( ( max( dotResult449 , 0.0 ) * _LightColor0 ) + float4( indirectDiffuse468 , 0.0 ) );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 normalizeResult430 = normalize( ( ase_worldlightDir + ase_worldViewDir ) );
			float dotResult432 = dot( NORMALS636 , normalizeResult430 );
			float4 temp_output_440_0 = ( ( pow( max( dotResult432 , 0.0 ) , ( _WaterGloss * 128.0 ) ) * _WaterSpecular ) * _LightColor0 );
			#ifdef _PERVERTEXSPECULAR_ON
			float4 staticSwitch638 = temp_output_440_0;
			#else
			float4 staticSwitch638 = float4( 0.0,0,0,0 );
			#endif
			o.data441 = staticSwitch638;
			float lerpResult266 = lerp( ase_worldPos.z , ase_worldPos.x , _WaveDirectionZX);
			float3 appendResult635 = (float3(0.0 , _WaveCycles , 0.0));
			float3 WAVEMOTION98 = ( sin( ( ( lerpResult266 + ( _Time.x * _WaveSpeed ) ) * appendResult635 ) ) * ( _WaveHeight * 0.1 ) );
			v.vertex.xyz += WAVEMOTION98;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 lerpResult6 = lerp( ase_worldNormal , fixed3(0,1,0) , _SmoothNormals);
			fixed3 NORMALS636 = lerpResult6;
			float2 componentMask414 = NORMALS636.xz;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPos161 = ase_screenPos;
			float eyeDepth162 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(( float4( componentMask414, 0.0 , 0.0 ) + ase_screenPos161 )))));
			fixed DEPTH358 = saturate( pow( ( _DepthOffset + abs( ( eyeDepth162 - ase_screenPos161.w ) ) ) , ( 1.0 - max( _DepthFalloff , 1.0 ) ) ) );
			float4 lerpResult362 = lerp( _WaterColor , ( _AbsorptionColor * max( _AbsorptionIntensity , 1.0 ) ) , ( DEPTH358 * min( _AbsorptionIntensity , 1.0 ) ));
			fixed4 REFRACTION152 = lerpResult362;
			fixed4 REFLECTION25 = tex2D( _ReflectionTex, i.data567 );
			float FRESNEL607 = i.data606;
			float4 lerpResult146 = lerp( REFRACTION152 , REFLECTION25 , FRESNEL607);
			#ifdef _DISABLEREFLECTION_ON
			float4 staticSwitch556 = REFRACTION152;
			#else
			float4 staticSwitch556 = lerpResult146;
			#endif
			fixed4 ALBEDO155 = staticSwitch556;
			fixed4 LIGHTING_LAMBERT450 = i.data451;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 normalizeResult430 = normalize( ( ase_worldlightDir + ase_worldViewDir ) );
			float dotResult432 = dot( NORMALS636 , normalizeResult430 );
			float4 temp_output_440_0 = ( ( pow( max( dotResult432 , 0.0 ) , ( _WaterGloss * 128.0 ) ) * _WaterSpecular ) * _LightColor0 );
			#ifdef _PERVERTEXSPECULAR_ON
			float4 staticSwitch500 = i.data441;
			#else
			float4 staticSwitch500 = temp_output_440_0;
			#endif
			fixed4 LIGHTING_SPECULAR455 = staticSwitch500;
			float eyeDepth409 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos161))));
			fixed EDGE246 = ( ( 1.0 - floor( saturate( ( _EdgeOffset + abs( ( eyeDepth409 - ase_screenPos161.w ) ) ) ) ) ) * _EdgeIntensity );
			float4 lerpResult492 = lerp( ( saturate( ( ALBEDO155 * LIGHTING_LAMBERT450 ) ) + LIGHTING_SPECULAR455 ) , _EdgeColor , EDGE246);
			float lerpResult619 = lerp( ( 1.0 - DEPTH358 ) , 1.0 , _AbsorptionIntensity);
			fixed OPACITY610 = saturate( ( lerpResult619 + EDGE246 ) );
			c.rgb = lerpResult492.rgb;
			c.a = OPACITY610;
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
1927;29;1906;1014;3802.165;4981.467;1.958535;True;False
Node;AmplifyShaderEditor.RangedFloatNode;9;-1920,1568;Fixed;False;Property;_SmoothNormals;Smooth Normals;6;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;265;-1920,1280;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;104;-1920,1424;Fixed;False;Constant;_Vector0;Vector 0;6;0;0,1,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;6;-1568,1280;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;636;-1376,1280;Fixed;False;NORMALS;-1;True;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;641;-1920,-1792;Float;False;636;0;1;FLOAT3
Node;AmplifyShaderEditor.ScreenPosInputsNode;161;-1920,-1664;Float;False;1;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;414;-1696,-1792;Float;False;True;False;True;False;1;0;FLOAT3;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;415;-1408,-1792;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT4;0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ScreenDepthNode;162;-1280,-1792;Float;False;0;1;0;FLOAT4;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;356;-544,-1664;Float;False;Property;_DepthFalloff;Depth Falloff;9;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;163;-1024,-1696;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMaxOp;373;-288,-1664;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;257;-864,-1696;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;353;-896,-1792;Float;False;Property;_DepthOffset;Depth Offset;8;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ScreenPosInputsNode;14;-1920,-256;Float;False;1;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;427;-1920,-3104;Float;False;1;0;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;426;-1920,-3024;Float;False;World;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;642;-1920,-64;Float;False;636;0;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;354;-480,-1792;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;370;-128,-1664;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;355;96,-1792;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;45;-1728,-64;Float;False;True;False;True;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;428;-1664,-3104;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.ComponentMaskNode;15;-1728,-256;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;624;-1488,-256;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector3Node;602;-1920,-2304;Fixed;False;Constant;_Vector2;Vector 2;26;0;0,1,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SaturateNode;367;256,-1792;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;416;-1920,704;Float;False;Property;_AbsorptionIntensity;Absorption Intensity;11;0;2;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;640;-1920,-3200;Float;False;636;0;1;FLOAT3
Node;AmplifyShaderEditor.NormalizeNode;430;-1536,-3104;Float;False;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;603;-1920,-2112;Float;False;Property;_FresnelPower;Fresnel Power;7;0;2;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.ScreenDepthNode;409;-1280,-1536;Float;False;0;1;0;FLOAT4;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;411;-1024,-1408;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMaxOp;631;-1408,512;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;358;512,-1792;Fixed;False;DEPTH;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;431;-1328,-2960;Float;False;Constant;_Float2;Float 2;1;0;128;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;420;-1920,448;Fixed;False;Property;_AbsorptionColor;Absorption Color;10;0;0,0.751724,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;417;-1920,624;Float;False;358;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMinNode;632;-1408,608;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;637;-1920,-3584;Float;False;636;0;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;131;-1696,-2960;Float;False;Property;_WaterGloss;Water Gloss;5;0;3;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.DotProductOpNode;432;-1328,-3200;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.FresnelNode;604;-1600,-2304;Float;False;4;0;FLOAT3;0,1,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.5;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;20;-1328,-160;Float;False;2;0;FLOAT2;0.0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;446;-1920,-3495;Float;False;1;0;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.AbsOpNode;605;-1360,-2304;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;419;-1232,416;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;433;-1168,-3056;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;128.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;170;-896,-1536;Float;False;Property;_EdgeOffset;Edge Offset;14;0;0.8;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;223;-1920,256;Fixed;False;Property;_WaterColor;Water Color;3;0;0.1952852,0.4153263,0.6323529,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DotProductOpNode;449;-1664,-3584;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMaxOp;434;-1200,-3200;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;410;-864,-1408;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.VertexToFragmentNode;567;-1152,-128;Float;False;1;0;FLOAT2;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;633;-1232,320;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;169;-480,-1536;Float;False;2;2;0;FLOAT;0.0,0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;258;-1120,-2960;Float;False;Property;_WaterSpecular;Water Specular;4;0;1;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;362;-1024,256;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMaxOp;454;-1536,-3584;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;22;-896,-256;Float;True;Property;_ReflectionTex;ReflectionTex;2;1;[HideInInspector];None;True;0;False;black;Auto;False;Object;-1;MipBias;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;435;-1024,-3200;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.VertexToFragmentNode;606;-1232,-2304;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LightColorNode;448;-1344,-3520;Float;False;0;1;COLOR
Node;AmplifyShaderEditor.LightColorNode;452;-784,-2960;Float;False;0;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;438;-864,-3200;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;445;-1168,-3584;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;153;-1920,-672;Float;False;25;0;1;FLOAT4
Node;AmplifyShaderEditor.IndirectDiffuseLighting;468;-1344,-3440;Float;False;Tangent;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;607;-1024,-2304;Float;False;FRESNEL;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-512,-256;Fixed;False;REFLECTION;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;154;-1920,-768;Float;False;152;0;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;599;-1920,-576;Float;False;607;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;-768,256;Fixed;False;REFRACTION;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SaturateNode;260;-352,-1536;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;267;-1920,2080;Fixed;False;Property;_WaveDirectionZX;Wave Direction Z-X;18;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TimeNode;79;-1920,2160;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;268;-1920,1920;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;440;-704,-3200;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;80;-1920,2336;Float;False;Property;_WaveSpeed;Wave Speed;17;0;25;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;146;-1536,-624;Float;False;3;0;FLOAT4;0.0,0,0,0;False;1;FLOAT4;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.FloorOpNode;263;-192,-1536;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;447;-1024,-3584;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT3;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.OneMinusNode;397;-32,-1536;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;613;-384,256;Float;False;358;0;1;FLOAT
Node;AmplifyShaderEditor.StaticSwitch;556;-1280,-768;Float;False;Property;_DisableReflection;Disable Reflection;1;0;0;False;True;;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;395;-256,-1408;Fixed;False;Property;_EdgeIntensity;Edge Intensity;13;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-1664,2160;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.VertexToFragmentNode;451;-208,-3584;Float;False;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.StaticSwitch;638;-512,-3200;Float;False;Property;_PerVertexSpecular;Per Vertex Specular;0;0;0;False;True;;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;266;-1632,2000;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;160;-1664,2336;Float;False;Property;_WaveCycles;Wave Cycles;16;0;1.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;635;-1440,2272;Float;False;FLOAT3;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;156;-2000,-4480;Float;False;155;0;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;463;-2000,-4400;Float;False;450;0;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;155;-896,-768;Fixed;False;ALBEDO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;393;128,-1536;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;92;-1472,2096;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.VertexToFragmentNode;441;-208,-3200;Float;False;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.RelayNode;634;-1408,704;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;620;-192,256;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;615;-384,320;Fixed;False;Constant;_Float1;Float 1;20;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;450;384,-3584;Fixed;False;LIGHTING_LAMBERT;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RelayNode;639;-512,-3088;Float;False;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;619;0,256;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-1280,2096;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;56;-1280,2336;Float;False;Property;_WaveHeight;Wave Height;15;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;246;512,-1536;Fixed;False;EDGE;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.StaticSwitch;500;0,-3120;Float;False;Property;_PerVertexSpecular;Per Vertex Specular;0;0;0;False;True;;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;462;-1664,-4480;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;622;0,384;Float;False;246;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;455;384,-3200;Fixed;False;LIGHTING_SPECULAR;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;464;-2000,-4320;Float;False;455;0;1;COLOR
Node;AmplifyShaderEditor.SaturateNode;466;-1504,-4480;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;621;256,256;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;220;-1104,2336;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.1;False;1;FLOAT
Node;AmplifyShaderEditor.SinOpNode;53;-1088,2096;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;465;-1344,-4480;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;491;-1344,-4160;Float;False;246;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;210;-1344,-4336;Fixed;False;Property;_EdgeColor;Edge Color;12;0;1,1,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SaturateNode;623;400,256;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-896,2096;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;98;-704,2096;Float;False;WAVEMOTION;-1;True;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;158;-896,-4208;Float;False;98;0;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;610;640,256;Fixed;False;OPACITY;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;492;-896,-4480;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;617;-896,-4288;Float;False;610;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-512,-4480;Float;False;True;2;Float;ASEMaterialInspector;200;0;CustomLighting;BOXOPHOBIC/LowPolyWater V2/SIMPLE;False;False;False;False;True;True;True;True;True;False;True;True;False;False;True;True;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;0;SrcAlpha;OneMinusSrcAlpha;0;SrcAlpha;OneMinusSrcAlpha;OFF;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;200;Unlit/Color;-1;-1;-1;-1;0;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;303;-2176,-2304;Float;False;100;100;;0;// FRESNEL CONTROL;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;274;-2176,-1792;Float;False;100;100;;0;// DEPTH AND EDGE CONTROL;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;510;-2176,-3584;Float;False;100;100;;0;// LIGHTING LAMBERT;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;276;-2176,-256;Float;False;100;100;;0;// REFLECTION MIRROR;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;272;-2176,-768;Float;False;100;100;;0;// ALBEDO;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;279;-2176,1920;Float;False;100;100;;0;// WAVE MOTION;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;457;-2176,1280;Float;False;100;100;;0;// NORMALS;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;458;-2176,-3200;Float;False;100;100;;0;// LIGHTING SPECULAR;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;280;-2176,-4480;Float;False;100;100;;0;// FINAL;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;277;-2176,256;Float;False;100;100;;0;// REFRECTION;1,1,1,1;0;0
WireConnection;6;0;265;0
WireConnection;6;1;104;0
WireConnection;6;2;9;0
WireConnection;636;0;6;0
WireConnection;414;0;641;0
WireConnection;415;0;414;0
WireConnection;415;1;161;0
WireConnection;162;0;415;0
WireConnection;163;0;162;0
WireConnection;163;1;161;4
WireConnection;373;0;356;0
WireConnection;257;0;163;0
WireConnection;354;0;353;0
WireConnection;354;1;257;0
WireConnection;370;0;373;0
WireConnection;355;0;354;0
WireConnection;355;1;370;0
WireConnection;45;0;642;0
WireConnection;428;0;427;0
WireConnection;428;1;426;0
WireConnection;15;0;14;0
WireConnection;624;0;15;0
WireConnection;624;1;45;0
WireConnection;367;0;355;0
WireConnection;430;0;428;0
WireConnection;409;0;161;0
WireConnection;411;0;409;0
WireConnection;411;1;161;4
WireConnection;631;0;416;0
WireConnection;358;0;367;0
WireConnection;632;0;416;0
WireConnection;432;0;640;0
WireConnection;432;1;430;0
WireConnection;604;0;602;0
WireConnection;604;3;603;0
WireConnection;20;0;624;0
WireConnection;20;1;14;4
WireConnection;605;0;604;0
WireConnection;419;0;417;0
WireConnection;419;1;632;0
WireConnection;433;0;131;0
WireConnection;433;1;431;0
WireConnection;449;0;637;0
WireConnection;449;1;446;0
WireConnection;434;0;432;0
WireConnection;410;0;411;0
WireConnection;567;0;20;0
WireConnection;633;0;420;0
WireConnection;633;1;631;0
WireConnection;169;0;170;0
WireConnection;169;1;410;0
WireConnection;362;0;223;0
WireConnection;362;1;633;0
WireConnection;362;2;419;0
WireConnection;454;0;449;0
WireConnection;22;1;567;0
WireConnection;435;0;434;0
WireConnection;435;1;433;0
WireConnection;606;0;605;0
WireConnection;438;0;435;0
WireConnection;438;1;258;0
WireConnection;445;0;454;0
WireConnection;445;1;448;0
WireConnection;607;0;606;0
WireConnection;25;0;22;0
WireConnection;152;0;362;0
WireConnection;260;0;169;0
WireConnection;440;0;438;0
WireConnection;440;1;452;0
WireConnection;146;0;154;0
WireConnection;146;1;153;0
WireConnection;146;2;599;0
WireConnection;263;0;260;0
WireConnection;447;0;445;0
WireConnection;447;1;468;0
WireConnection;397;0;263;0
WireConnection;556;0;154;0
WireConnection;556;1;146;0
WireConnection;82;0;79;1
WireConnection;82;1;80;0
WireConnection;451;0;447;0
WireConnection;638;0;440;0
WireConnection;266;0;268;3
WireConnection;266;1;268;1
WireConnection;266;2;267;0
WireConnection;635;1;160;0
WireConnection;155;0;556;0
WireConnection;393;0;397;0
WireConnection;393;1;395;0
WireConnection;92;0;266;0
WireConnection;92;1;82;0
WireConnection;441;0;638;0
WireConnection;634;0;416;0
WireConnection;620;0;613;0
WireConnection;450;0;451;0
WireConnection;639;0;440;0
WireConnection;619;0;620;0
WireConnection;619;1;615;0
WireConnection;619;2;634;0
WireConnection;84;0;92;0
WireConnection;84;1;635;0
WireConnection;246;0;393;0
WireConnection;500;0;441;0
WireConnection;500;1;639;0
WireConnection;462;0;156;0
WireConnection;462;1;463;0
WireConnection;455;0;500;0
WireConnection;466;0;462;0
WireConnection;621;0;619;0
WireConnection;621;1;622;0
WireConnection;220;0;56;0
WireConnection;53;0;84;0
WireConnection;465;0;466;0
WireConnection;465;1;464;0
WireConnection;623;0;621;0
WireConnection;57;0;53;0
WireConnection;57;1;220;0
WireConnection;98;0;57;0
WireConnection;610;0;623;0
WireConnection;492;0;465;0
WireConnection;492;1;210;0
WireConnection;492;2;491;0
WireConnection;0;2;492;0
WireConnection;0;9;617;0
WireConnection;0;11;158;0
ASEEND*/
//CHKSM=9E3B3BBC9ECE2F9D02E745647C02666CBF6F4662
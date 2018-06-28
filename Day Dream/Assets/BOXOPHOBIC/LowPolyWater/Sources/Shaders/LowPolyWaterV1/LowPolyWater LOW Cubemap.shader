// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/LowPolyWater/LOW Cubemap"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[HideInInspector]_SpecColor("SpecularColor",Color)=(1,1,1,1)
		[Header(Surface Control)] _WaterColor("Water Color", Color) = (0,0.5,1,1)
		_WaterSpecular("Water Specular", Range( 0 , 10)) = 1
		_WaterGloss("Water Gloss", Range( 0 , 10)) = 3
		_SmoothNormals("Smooth Normals", Range( 0 , 1)) = 0.5
		[Header(Reflection and Depth)] _FresnelPower("Fresnel Power", Range( 0 , 5)) = 0.75
		_ReflectionBoost("Reflection Boost", Range( 1 , 5)) = 0.75
		_ReflectionIntensity("Reflection Intensity", Range( 0 , 1)) = 0.75
		_DepthOffset("Depth Offset", Float) = 1
		_DepthFalloff("Depth Falloff", Float) = 2
		[Header(Edge Control)] _EdgeColor("Edge Color", Color) = (1,1,1,1)
		_EdgeIntensity("Edge Intensity", Range( 0 , 1)) = 0.5
		_EdgeFalloff("Edge Falloff", Float) = 10
		_EdgeOffset("Edge Offset", Range( 0 , 0.99)) = 0.8
		[IntRange]_EdgeLevels("Edge Levels", Range( 1 , 10)) = 5
		[Header(Motion Control)] _WaveHeight("Wave Height", Float) = 0.2
		_WaveCycles("Wave Cycles", Float) = 1.5
		_WaveSpeed("Wave Speed", Float) = 25
		_WaveDirectionZX("Wave Direction Z-X", Range( 0 , 1)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf BlinnPhong alpha:fade keepalpha vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float4 screenPos;
		};

		uniform float4 _WaterColor;
		uniform fixed _SmoothNormals;
		uniform fixed _ReflectionBoost;
		uniform fixed _ReflectionIntensity;
		uniform float _FresnelPower;
		uniform float4 _EdgeColor;
		uniform fixed _EdgeOffset;
		uniform sampler2D _CameraDepthTexture;
		uniform float _EdgeFalloff;
		uniform float _EdgeLevels;
		uniform fixed _EdgeIntensity;
		uniform float _WaterGloss;
		uniform float _WaterSpecular;
		uniform float _DepthOffset;
		uniform float _DepthFalloff;
		uniform fixed _WaveDirectionZX;
		uniform float _WaveSpeed;
		uniform float _WaveCycles;
		uniform float _WaveHeight;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float lerpResult266 = lerp( ase_worldPos.z , ase_worldPos.x , _WaveDirectionZX);
			float3 appendResult385 = (float3(0.0 , _WaveCycles , 0.0));
			float3 VERTEXANIM98 = ( sin( ( ( lerpResult266 + ( _Time.x * _WaveSpeed ) ) * appendResult385 ) ) * ( _WaveHeight * 0.1 ) );
			v.vertex.xyz += VERTEXANIM98;
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 lerpResult6 = lerp( ase_worldNormal , float3(0,1,0) , _SmoothNormals);
			half3 NORMAL97 = lerpResult6;
			v.normal = NORMAL97;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			fixed4 WATERCOLOR212 = _WaterColor;
			float3 localMyCustomExpression341341 = ( unity_SpecCube0_BoxMax.xyz );
			float3 ase_worldPos = i.worldPos;
			float3 normalizeResult340 = normalize( ( ase_worldPos - _WorldSpaceCameraPos ) );
			float3 lerpResult6 = lerp( i.worldNormal , float3(0,1,0) , _SmoothNormals);
			half3 NORMAL97 = lerpResult6;
			float3 temp_output_343_0 = reflect( normalizeResult340 , NORMAL97 );
			float3 localMyCustomExpression337337 = ( unity_SpecCube0_BoxMin.xyz );
			float3 localMyCustomExpression354354 = ( unity_SpecCube0_ProbePosition.xyz );
			float3 reflectDir359 = ( ( ( min( min( max( ( ( localMyCustomExpression341341 - ase_worldPos ) / temp_output_343_0 ) , ( ( localMyCustomExpression337337 - ase_worldPos ) / temp_output_343_0 ) ).x , max( ( ( localMyCustomExpression341341 - ase_worldPos ) / temp_output_343_0 ) , ( ( localMyCustomExpression337337 - ase_worldPos ) / temp_output_343_0 ) ).y ) , max( ( ( localMyCustomExpression341341 - ase_worldPos ) / temp_output_343_0 ) , ( ( localMyCustomExpression337337 - ase_worldPos ) / temp_output_343_0 ) ).z ) * temp_output_343_0 ) + ase_worldPos ) - localMyCustomExpression354354 );
			float4 localMyCustomExpression359359 = ( UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectDir359,0) );
			float4 reflectionHDR361 = localMyCustomExpression359359;
			float3 localMyCustomExpression361361 = ( DecodeHDR(reflectionHDR361, unity_SpecCube0_HDR) );
			float4 lerpResult363 = lerp( WATERCOLOR212 , float4( ( localMyCustomExpression361361 * _ReflectionBoost ) , 0.0 ) , _ReflectionIntensity);
			fixed4 REFLECTION25 = lerpResult363;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 normalizeResult172 = normalize( NORMAL97 );
			float fresnelFinalVal129 = (0.0 + 1.0*pow( 1.0 - dot( normalizeResult172, worldViewDir ) , _FresnelPower));
			float FRESNEL304 = ( 1.0 - abs( fresnelFinalVal129 ) );
			float4 lerpResult146 = lerp( REFLECTION25 , WATERCOLOR212 , FRESNEL304);
			fixed4 EDGECOLOR297 = _EdgeColor;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPos161 = ase_screenPos;
			float eyeDepth162 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos161))));
			float temp_output_257_0 = abs( ( eyeDepth162 - ase_screenPos161.w ) );
			fixed EDGE246 = ( ( 1.0 - (0.05 + (floor( ( saturate( pow( ( _EdgeOffset + temp_output_257_0 ) , max( _EdgeFalloff , 0.0 ) ) ) * _EdgeLevels ) ) - 0.0) * (1.0 - 0.05) / (_EdgeLevels - 0.0)) ) * _EdgeIntensity );
			float4 lerpResult298 = lerp( lerpResult146 , EDGECOLOR297 , EDGE246);
			fixed4 ALBEDO155 = lerpResult298;
			o.Albedo = ALBEDO155.rgb;
			float temp_output_378_0 = ( 1.0 - EDGE246 );
			o.Specular = ( _WaterGloss * temp_output_378_0 );
			o.Gloss = ( temp_output_378_0 * _WaterSpecular );
			fixed DEPTH371 = saturate( pow( ( _DepthOffset + temp_output_257_0 ) , ( 1.0 - max( _DepthFalloff , 1.0 ) ) ) );
			fixed OPACITY307 = saturate( ( EDGE246 + ( ( 1.0 - DEPTH371 ) * 2.0 ) ) );
			o.Alpha = OPACITY307;
		}

		ENDCG
	}
	Fallback "BOXOPHOBIC/LowPolyWater/MOBILE Cubemap"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13106
1927;29;1906;1014;2155.153;-497.9766;1;True;False
Node;AmplifyShaderEditor.WorldSpaceCameraPos;334;-1920,-624;Float;False;0;1;FLOAT3
Node;AmplifyShaderEditor.WorldPosInputsNode;333;-1920,-768;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;104;-1920,160;Float;False;Constant;_Vector0;Vector 0;6;0;0,1,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;265;-1920,0;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;9;-1920,320;Fixed;False;Property;_SmoothNormals;Smooth Normals;3;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;335;-1664,-768;Float;False;2;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;6;-1536,0;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.WorldPosInputsNode;338;-1280,-352;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CustomExpressionNode;337;-1280,-416;Float;False;unity_SpecCube0_BoxMin.xyz;3;False;0;My Custom Expression;0;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;97;-1344,0;Half;False;NORMAL;-1;True;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.CustomExpressionNode;341;-1280,-768;Float;False;unity_SpecCube0_BoxMax.xyz;3;False;0;My Custom Expression;0;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;336;-1920,-512;Float;False;97;0;1;FLOAT3
Node;AmplifyShaderEditor.NormalizeNode;340;-1504,-768;Float;False;1;0;FLOAT3;0.0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.WorldPosInputsNode;339;-1280,-704;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ReflectOpNode;343;-1280,-544;Float;False;2;0;FLOAT3;0.0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;342;-1088,-768;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;344;-1088,-416;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.ScreenPosInputsNode;161;-1920,-1536;Float;False;1;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;345;-896,-464;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleDivideOpNode;346;-896,-640;Float;False;2;0;FLOAT3;0.0;False;1;FLOAT3;0.0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.ScreenDepthNode;162;-1728,-1536;Float;False;0;1;0;FLOAT4;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMaxOp;347;-640,-768;Float;False;2;0;FLOAT3;0.0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;163;-1536,-1184;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;348;-512,-768;Float;False;FLOAT3;1;0;FLOAT3;0.0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;257;-1376,-1184;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;170;-1504,-1280;Fixed;False;Property;_EdgeOffset;Edge Offset;12;0;0.8;0;0.99;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMinNode;349;-256,-768;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;168;-1152,-1088;Float;False;Property;_EdgeFalloff;Edge Falloff;11;0;10;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;169;-1104,-1280;Float;False;2;2;0;FLOAT;0.0,0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMaxOp;226;-960,-1104;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RelayNode;351;-512,-544;Float;False;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMinNode;350;-128,-768;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;294;-896,-1280;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;352;64,-768;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;364;-1152,-1408;Float;False;Property;_DepthFalloff;Depth Falloff;8;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;353;16,-624;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMaxOp;366;-896,-1408;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;355;224,-768;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.CustomExpressionNode;354;208,-560;Float;False;unity_SpecCube0_ProbePosition.xyz;3;False;0;My Custom Expression;0;1;FLOAT3
Node;AmplifyShaderEditor.SaturateNode;260;-704,-1280;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;365;-1504,-1536;Float;False;Property;_DepthOffset;Depth Offset;7;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;262;-768,-1088;Float;False;Property;_EdgeLevels;Edge Levels;13;1;[IntRange];5;1;10;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;148;-1920,-2064;Float;False;97;0;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;356;384,-768;Float;False;2;0;FLOAT3;0.0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;368;-1104,-1536;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;261;-448,-1280;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;367;-752,-1408;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;149;-1920,-1936;Float;False;Property;_FresnelPower;Fresnel Power;4;0;0.75;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.CustomExpressionNode;359;640,-672;Float;False;UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectDir,0);4;False;1;True;reflectDir;FLOAT3;0,0,0;In;My Custom Expression;1;0;FLOAT3;0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.PowerNode;369;-512,-1536;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NormalizeNode;172;-1664,-2064;Float;False;1;0;FLOAT3;0.0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.FloorOpNode;263;-304,-1280;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TimeNode;79;-1920,880;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;267;-1920,800;Fixed;False;Property;_WaveDirectionZX;Wave Direction Z-X;17;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;264;-160,-1280;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.05;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;382;832,-592;Fixed;False;Property;_ReflectionBoost;Reflection Boost;5;0;0.75;1;5;0;1;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;268;-1920,640;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.FresnelNode;129;-1472,-2064;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.5;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;80;-1920,1072;Float;False;Property;_WaveSpeed;Wave Speed;16;0;25;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;370;-256,-1536;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CustomExpressionNode;361;848,-672;Float;False;DecodeHDR(reflectionHDR, unity_SpecCube0_HDR);3;False;1;True;reflectionHDR;FLOAT4;0,0,0,0;In;My Custom Expression;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.ColorNode;223;-1920,-3488;Float;False;Property;_WaterColor;Water Color;0;0;0,0.4980392,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;222;-1264,-2064;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-1664,880;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;266;-1632,720;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;23;832,-512;Fixed;False;Property;_ReflectionIntensity;Reflection Intensity;6;0;0.75;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;376;-160,-1088;Fixed;False;Property;_EdgeIntensity;Edge Intensity;10;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;383;1120,-672;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;371;32,-1536;Fixed;False;DEPTH;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;362;640,-768;Float;False;212;0;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;212;-1664,-3488;Fixed;False;WATERCOLOR;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;160;-1664,1072;Float;False;Property;_WaveCycles;Wave Cycles;15;0;1.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;372;-1920,-2432;Float;False;371;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;374;32,-1280;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;313;-1120,-2064;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;363;1408,-768;Float;False;3;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.DynamicAppendNode;385;-1456,1024;Float;False;FLOAT3;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.OneMinusNode;373;-1696,-2432;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;375;192,-1280;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;92;-1472,816;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;304;-944,-2064;Float;False;FRESNEL;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;210;-1920,-3712;Float;False;Property;_EdgeColor;Edge Color;9;0;1,1,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;1600,-768;Fixed;False;REFLECTION;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;251;-1920,-2560;Float;False;246;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;308;-1920,-2880;Float;False;304;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;56;-1152,896;Float;False;Property;_WaveHeight;Wave Height;14;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;154;-1920,-2976;Float;False;212;0;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;153;-1920,-3072;Float;False;25;0;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;246;384,-1280;Fixed;False;EDGE;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-1152,640;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;384;-1536,-2432;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;2.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;300;-1408,-2928;Float;False;297;0;1;COLOR
Node;AmplifyShaderEditor.SinOpNode;53;-960,640;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;146;-1408,-3072;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;316;-1280,-2560;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;325;-1920,-4416;Float;False;246;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;301;-1408,-2864;Float;False;246;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;220;-976,880;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.1;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;297;-1664,-3712;Fixed;False;EDGECOLOR;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-768,640;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;258;-1920,-4336;Float;False;Property;_WaterSpecular;Water Specular;1;0;1;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;131;-1920,-4496;Float;False;Property;_WaterGloss;Water Gloss;2;0;3;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;378;-1728,-4416;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;298;-1024,-3072;Float;False;3;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SaturateNode;317;-1152,-2560;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;98;-576,640;Float;False;VERTEXANIM;-1;True;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;330;-1536,-4496;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;332;-1920,-4224;Float;False;307;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;328;-1920,-4128;Float;False;98;0;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;155;-832,-3072;Fixed;False;ALBEDO;-1;True;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;331;-1920,-4608;Float;False;155;0;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;329;-1536,-4400;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;327;-1920,-4032;Float;False;97;0;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;307;-992,-2560;Fixed;False;OPACITY;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1152,-4608;Float;False;True;2;Float;ASEMaterialInspector;0;0;BlinnPhong;BOXOPHOBIC/LowPolyWater/LOW Cubemap;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Off;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;SrcAlpha;OneMinusSrcAlpha;0;SrcAlpha;OneMinusSrcAlpha;OFF;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;BOXOPHOBIC/LowPolyWater/MOBILE Cubemap;-1;-1;-1;-1;0;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;306;-2176,-2560;Float;False;100;100;;0;// OPACITY;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;276;-2176,-768;Float;False;100;100;;0;// REFLECTION;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;274;-2176,-1536;Float;False;100;100;;0;// DEPTH AND EDGE CONTROL;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;272;-2176,-3072;Float;False;100;100;;0;// ALBEDO;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;279;-2176,640;Float;False;100;100;;0;// WAVE ANIMATION;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;273;-2176,-3712;Float;False;100;100;;0;// WATER / EGDE COLORS;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;303;-2176,-2064;Float;False;100;100;;0;// FRESNEL CONTROL;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;280;-2176,-4608;Float;False;100;100;;0;// FINAL;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;278;-2176,0;Float;False;100;100;;0;// NORMALS;1,1,1,1;0;0
WireConnection;335;0;333;0
WireConnection;335;1;334;0
WireConnection;6;0;265;0
WireConnection;6;1;104;0
WireConnection;6;2;9;0
WireConnection;97;0;6;0
WireConnection;340;0;335;0
WireConnection;343;0;340;0
WireConnection;343;1;336;0
WireConnection;342;0;341;0
WireConnection;342;1;339;0
WireConnection;344;0;337;0
WireConnection;344;1;338;0
WireConnection;345;0;344;0
WireConnection;345;1;343;0
WireConnection;346;0;342;0
WireConnection;346;1;343;0
WireConnection;162;0;161;0
WireConnection;347;0;346;0
WireConnection;347;1;345;0
WireConnection;163;0;162;0
WireConnection;163;1;161;4
WireConnection;348;0;347;0
WireConnection;257;0;163;0
WireConnection;349;0;348;0
WireConnection;349;1;348;1
WireConnection;169;0;170;0
WireConnection;169;1;257;0
WireConnection;226;0;168;0
WireConnection;351;0;343;0
WireConnection;350;0;349;0
WireConnection;350;1;348;2
WireConnection;294;0;169;0
WireConnection;294;1;226;0
WireConnection;352;0;350;0
WireConnection;352;1;351;0
WireConnection;366;0;364;0
WireConnection;355;0;352;0
WireConnection;355;1;353;0
WireConnection;260;0;294;0
WireConnection;356;0;355;0
WireConnection;356;1;354;0
WireConnection;368;0;365;0
WireConnection;368;1;257;0
WireConnection;261;0;260;0
WireConnection;261;1;262;0
WireConnection;367;0;366;0
WireConnection;359;0;356;0
WireConnection;369;0;368;0
WireConnection;369;1;367;0
WireConnection;172;0;148;0
WireConnection;263;0;261;0
WireConnection;264;0;263;0
WireConnection;264;2;262;0
WireConnection;129;0;172;0
WireConnection;129;3;149;0
WireConnection;370;0;369;0
WireConnection;361;0;359;0
WireConnection;222;0;129;0
WireConnection;82;0;79;1
WireConnection;82;1;80;0
WireConnection;266;0;268;3
WireConnection;266;1;268;1
WireConnection;266;2;267;0
WireConnection;383;0;361;0
WireConnection;383;1;382;0
WireConnection;371;0;370;0
WireConnection;212;0;223;0
WireConnection;374;0;264;0
WireConnection;313;0;222;0
WireConnection;363;0;362;0
WireConnection;363;1;383;0
WireConnection;363;2;23;0
WireConnection;385;1;160;0
WireConnection;373;0;372;0
WireConnection;375;0;374;0
WireConnection;375;1;376;0
WireConnection;92;0;266;0
WireConnection;92;1;82;0
WireConnection;304;0;313;0
WireConnection;25;0;363;0
WireConnection;246;0;375;0
WireConnection;84;0;92;0
WireConnection;84;1;385;0
WireConnection;384;0;373;0
WireConnection;53;0;84;0
WireConnection;146;0;153;0
WireConnection;146;1;154;0
WireConnection;146;2;308;0
WireConnection;316;0;251;0
WireConnection;316;1;384;0
WireConnection;220;0;56;0
WireConnection;297;0;210;0
WireConnection;57;0;53;0
WireConnection;57;1;220;0
WireConnection;378;0;325;0
WireConnection;298;0;146;0
WireConnection;298;1;300;0
WireConnection;298;2;301;0
WireConnection;317;0;316;0
WireConnection;98;0;57;0
WireConnection;330;0;131;0
WireConnection;330;1;378;0
WireConnection;155;0;298;0
WireConnection;329;0;378;0
WireConnection;329;1;258;0
WireConnection;307;0;317;0
WireConnection;0;0;331;0
WireConnection;0;3;330;0
WireConnection;0;4;329;0
WireConnection;0;9;332;0
WireConnection;0;11;328;0
WireConnection;0;12;327;0
ASEEND*/
//CHKSM=3CFF8E9F58848CD1D92979D15670A255E406CF12
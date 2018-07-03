// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/LowPolyWater/LOW MirrorRef"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[HideInInspector]_SpecColor("SpecularColor",Color)=(1,1,1,1)
		[HideInInspector]_ReflectionTex("ReflectionTex", 2D) = "black" {}
		[Header(Surface Control)] _WaterColor("Water Color", Color) = (0,0.5,1,1)
		_WaterSpecular("Water Specular", Range( 0 , 10)) = 1
		_WaterGloss("Water Gloss", Range( 0 , 10)) = 3
		_SmoothNormals("Smooth Normals", Range( 0 , 1)) = 0.5
		[Header(Reflection and Depth)] _FresnelPower("Fresnel Power", Range( 0 , 5)) = 0.75
		_ReflectionIntensity("Reflection Intensity", Range( 0 , 1)) = 0.75
		_ReflectionBoost("Reflection Boost", Range( 1 , 5)) = 1
		_DepthOffset("Depth Offset", Float) = 1
		_DepthFalloff("Depth Falloff", Float) = 2
		[Header(Edge Control)] _EdgeColor("Edge Color", Color) = (1,1,1,1)
		_EdgeIntensity("Edge Intensity", Range( 0 , 1)) = 0.5
		_EdgeOffset("Edge Offset", Range( 0 , 0.99)) = 0.8
		_EdgeFalloff("Edge Falloff", Float) = 10
		[IntRange]_EdgeLevels("Edge Levels", Range( 1 , 10)) = 5
		[Header(Motion Control)] _WaveHeight("Wave Height", Float) = 0.2
		_WaveCycles("Wave Cycles", Float) = 1.5
		_WaveSpeed("Wave Speed", Float) = 25
		_WaveDirectionZX("Wave Direction Z-X", Range( 0 , 1)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf BlinnPhong alpha:fade keepalpha vertex:vertexDataFunc 
		struct Input
		{
			float4 screenPos;
			float3 worldNormal;
			float3 worldPos;
		};

		uniform float4 _WaterColor;
		uniform sampler2D _ReflectionTex;
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
			float3 appendResult368 = (float3(0.0 , _WaveCycles , 0.0));
			float3 VERTEXANIM98 = ( sin( ( ( lerpResult266 + ( _Time.x * _WaveSpeed ) ) * appendResult368 ) ) * ( _WaveHeight * 0.1 ) );
			v.vertex.xyz += VERTEXANIM98;
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 lerpResult6 = lerp( ase_worldNormal , float3(0,1,0) , _SmoothNormals);
			half3 NORMAL97 = lerpResult6;
			v.normal = NORMAL97;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			fixed4 WATERCOLOR212 = _WaterColor;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPos14 = ase_screenPos;
			float2 componentMask15 = ase_screenPos14.xy;
			float3 lerpResult6 = lerp( i.worldNormal , float3(0,1,0) , _SmoothNormals);
			half3 NORMAL97 = lerpResult6;
			float2 componentMask45 = NORMAL97.xz;
			float4 lerpResult213 = lerp( WATERCOLOR212 , ( tex2D( _ReflectionTex, ( ( componentMask15 / ase_screenPos14.w ) + ( componentMask45 * float2( 0.25,0.25 ) ) ) ) * _ReflectionBoost ) , _ReflectionIntensity);
			fixed4 REFLECTION25 = lerpResult213;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 normalizeResult172 = normalize( NORMAL97 );
			float fresnelFinalVal129 = (0.0 + 1.0*pow( 1.0 - dot( normalizeResult172, worldViewDir ) , _FresnelPower));
			float FRESNEL304 = ( 1.0 - abs( fresnelFinalVal129 ) );
			float4 lerpResult351 = lerp( REFLECTION25 , WATERCOLOR212 , FRESNEL304);
			fixed4 EDGECOLOR297 = _EdgeColor;
			float4 ase_screenPos161 = ase_screenPos;
			float eyeDepth162 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos161))));
			float temp_output_257_0 = abs( ( eyeDepth162 - ase_screenPos161.w ) );
			fixed EDGE246 = ( ( 1.0 - (0.05 + (floor( ( saturate( pow( ( _EdgeOffset + temp_output_257_0 ) , max( _EdgeFalloff , 0.0 ) ) ) * _EdgeLevels ) ) - 0.0) * (1.0 - 0.05) / (_EdgeLevels - 0.0)) ) * _EdgeIntensity );
			float4 lerpResult354 = lerp( lerpResult351 , EDGECOLOR297 , EDGE246);
			fixed4 ALBEDO155 = lerpResult354;
			o.Albedo = ALBEDO155.rgb;
			float temp_output_362_0 = ( 1.0 - EDGE246 );
			o.Specular = ( _WaterGloss * temp_output_362_0 );
			o.Gloss = ( temp_output_362_0 * _WaterSpecular );
			fixed DEPTH340 = saturate( pow( ( _DepthOffset + temp_output_257_0 ) , ( 1.0 - max( _DepthFalloff , 1.0 ) ) ) );
			fixed OPACITY307 = saturate( ( EDGE246 + ( ( 1.0 - DEPTH340 ) * 2.0 ) ) );
			o.Alpha = OPACITY307;
		}

		ENDCG
	}
	Fallback "BOXOPHOBIC/LowPolyWater/MOBILE MirrorRef"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13106
1927;29;1906;1014;2317.227;-263.0462;1;True;False
Node;AmplifyShaderEditor.ScreenPosInputsNode;161;-1920,-1664;Float;False;1;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ScreenDepthNode;162;-1728,-1664;Float;False;0;1;0;FLOAT4;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;163;-1536,-1312;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;9;-1920,64;Fixed;False;Property;_SmoothNormals;Smooth Normals;4;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;170;-1504,-1408;Fixed;False;Property;_EdgeOffset;Edge Offset;12;0;0.8;0;0.99;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;168;-1152,-1216;Float;False;Property;_EdgeFalloff;Edge Falloff;13;0;10;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;257;-1376,-1312;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;265;-1920,-256;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;104;-1920,-96;Float;False;Constant;_Vector0;Vector 0;6;0;0,1,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;169;-1104,-1408;Float;False;2;2;0;FLOAT;0.0,0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;6;-1536,-256;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMaxOp;226;-960,-1232;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;100;-1920,-704;Float;False;97;0;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;333;-1152,-1536;Float;False;Property;_DepthFalloff;Depth Falloff;9;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;97;-1344,-256;Half;False;NORMAL;-1;True;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.PowerNode;294;-896,-1408;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ScreenPosInputsNode;14;-1920,-896;Float;False;1;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;45;-1728,-704;Float;False;True;False;True;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.ComponentMaskNode;15;-1728,-896;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMaxOp;334;-896,-1536;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;262;-768,-1216;Float;False;Property;_EdgeLevels;Edge Levels;14;1;[IntRange];5;1;10;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;260;-704,-1408;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;335;-1504,-1664;Float;False;Property;_DepthOffset;Depth Offset;8;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;261;-448,-1408;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;296;-1472,-704;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT2;0.25,0.25;False;1;FLOAT2
Node;AmplifyShaderEditor.OneMinusNode;336;-752,-1536;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;148;-1920,-2176;Float;False;97;0;1;FLOAT3
Node;AmplifyShaderEditor.SimpleDivideOpNode;20;-1472,-832;Float;False;2;0;FLOAT2;0.0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;337;-1104,-1664;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NormalizeNode;172;-1664,-2176;Float;False;1;0;FLOAT3;0.0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-1280,-768;Float;False;2;2;0;FLOAT2;0.0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.PowerNode;338;-512,-1664;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;149;-1920,-2048;Float;False;Property;_FresnelPower;Fresnel Power;5;0;0.75;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.FloorOpNode;263;-304,-1408;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TimeNode;79;-1920,624;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;80;-1920,816;Float;False;Property;_WaveSpeed;Wave Speed;17;0;25;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;267;-1920,544;Fixed;False;Property;_WaveDirectionZX;Wave Direction Z-X;18;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;268;-1920,384;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.FresnelNode;129;-1472,-2176;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.5;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;22;-1024,-816;Float;True;Property;_ReflectionTex;ReflectionTex;0;1;[HideInInspector];None;True;0;False;black;Auto;False;Object;-1;MipBias;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;264;-160,-1408;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.05;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;365;-1024,-624;Fixed;False;Property;_ReflectionBoost;Reflection Boost;7;0;1;1;5;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;339;-256,-1664;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;223;-1920,-3488;Float;False;Property;_WaterColor;Water Color;1;0;0,0.4980392,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;345;16,-1408;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;23;-1024,-544;Fixed;False;Property;_ReflectionIntensity;Reflection Intensity;6;0;0.75;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;160;-1664,816;Float;False;Property;_WaveCycles;Wave Cycles;16;0;1.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;222;-1264,-2176;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;214;-1024,-896;Float;False;212;0;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;347;-160,-1216;Fixed;False;Property;_EdgeIntensity;Edge Intensity;11;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;366;-688,-816;Float;False;2;2;0;FLOAT4;0.0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-1664,624;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;340;32,-1664;Fixed;False;DEPTH;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;212;-1664,-3488;Fixed;False;WATERCOLOR;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;266;-1632,464;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;355;-1920,-2464;Float;False;340;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;357;-1728,-2464;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;346;192,-1408;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;368;-1456,768;Float;False;FLOAT3;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.OneMinusNode;313;-1120,-2176;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;92;-1472,560;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;213;-384,-896;Float;False;3;0;FLOAT4;0.0;False;1;FLOAT4;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;359;-1920,-2560;Float;False;246;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;56;-1280,816;Float;False;Property;_WaveHeight;Wave Height;15;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;304;-944,-2176;Float;False;FRESNEL;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;350;-1920,-3072;Float;False;25;0;1;FLOAT4
Node;AmplifyShaderEditor.RegisterLocalVarNode;246;384,-1408;Fixed;False;EDGE;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;210;-1920,-3712;Float;False;Property;_EdgeColor;Edge Color;10;0;1,1,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-224,-896;Fixed;False;REFLECTION;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;349;-1920,-2976;Float;False;212;0;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;367;-1536,-2464;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;2.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;348;-1920,-2880;Float;False;304;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-1280,560;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;360;-1280,-2560;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;325;-1920,-4416;Float;False;246;0;1;FLOAT
Node;AmplifyShaderEditor.SinOpNode;53;-1088,560;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;220;-1104,800;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.1;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;297;-1664,-3712;Fixed;False;EDGECOLOR;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;351;-1408,-3072;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;352;-1408,-2864;Float;False;246;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;353;-1408,-2928;Float;False;297;0;1;COLOR
Node;AmplifyShaderEditor.SaturateNode;361;-1152,-2560;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;131;-1920,-4496;Float;False;Property;_WaterGloss;Water Gloss;3;0;3;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;354;-1024,-3072;Float;False;3;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-896,560;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.OneMinusNode;362;-1728,-4416;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;258;-1920,-4336;Float;False;Property;_WaterSpecular;Water Specular;2;0;1;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;98;-704,560;Float;False;VERTEXANIM;-1;True;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;328;-1920,-4128;Float;False;98;0;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;327;-1920,-4032;Float;False;97;0;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;330;-1536,-4496;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;331;-1920,-4608;Float;False;155;0;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;332;-1920,-4224;Float;False;307;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;329;-1536,-4400;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;307;-992,-2560;Fixed;False;OPACITY;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;155;-832,-3072;Fixed;False;ALBEDO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1152,-4608;Float;False;True;2;Float;ASEMaterialInspector;0;0;BlinnPhong;BOXOPHOBIC/LowPolyWater/LOW MirrorRef;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;SrcAlpha;OneMinusSrcAlpha;0;SrcAlpha;OneMinusSrcAlpha;OFF;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;BOXOPHOBIC/LowPolyWater/MOBILE MirrorRef;-1;-1;-1;-1;0;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;276;-2176,-896;Float;False;100;100;;0;// REFLECTION;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;272;-2176,-3072;Float;False;100;100;;0;// ALBEDO;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;279;-2176,384;Float;False;100;100;;0;// WAVE ANIMATION;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;280;-2176,-4608;Float;False;100;100;;0;// FINAL;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;274;-2176,-1664;Float;False;100;100;;0;// DEPTH AND EDGE CONTROL;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;306;-2176,-2560;Float;False;100;100;;0;// OPACITY;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;278;-2176,-256;Float;False;100;100;;0;// NORMALS;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;273;-2176,-3712;Float;False;100;100;;0;// WATER / EGDE COLORS;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;303;-2176,-2176;Float;False;100;100;;0;// FRESNEL CONTROL;1,1,1,1;0;0
WireConnection;162;0;161;0
WireConnection;163;0;162;0
WireConnection;163;1;161;4
WireConnection;257;0;163;0
WireConnection;169;0;170;0
WireConnection;169;1;257;0
WireConnection;6;0;265;0
WireConnection;6;1;104;0
WireConnection;6;2;9;0
WireConnection;226;0;168;0
WireConnection;97;0;6;0
WireConnection;294;0;169;0
WireConnection;294;1;226;0
WireConnection;45;0;100;0
WireConnection;15;0;14;0
WireConnection;334;0;333;0
WireConnection;260;0;294;0
WireConnection;261;0;260;0
WireConnection;261;1;262;0
WireConnection;296;0;45;0
WireConnection;336;0;334;0
WireConnection;20;0;15;0
WireConnection;20;1;14;4
WireConnection;337;0;335;0
WireConnection;337;1;257;0
WireConnection;172;0;148;0
WireConnection;21;0;20;0
WireConnection;21;1;296;0
WireConnection;338;0;337;0
WireConnection;338;1;336;0
WireConnection;263;0;261;0
WireConnection;129;0;172;0
WireConnection;129;3;149;0
WireConnection;22;1;21;0
WireConnection;264;0;263;0
WireConnection;264;2;262;0
WireConnection;339;0;338;0
WireConnection;345;0;264;0
WireConnection;222;0;129;0
WireConnection;366;0;22;0
WireConnection;366;1;365;0
WireConnection;82;0;79;1
WireConnection;82;1;80;0
WireConnection;340;0;339;0
WireConnection;212;0;223;0
WireConnection;266;0;268;3
WireConnection;266;1;268;1
WireConnection;266;2;267;0
WireConnection;357;0;355;0
WireConnection;346;0;345;0
WireConnection;346;1;347;0
WireConnection;368;1;160;0
WireConnection;313;0;222;0
WireConnection;92;0;266;0
WireConnection;92;1;82;0
WireConnection;213;0;214;0
WireConnection;213;1;366;0
WireConnection;213;2;23;0
WireConnection;304;0;313;0
WireConnection;246;0;346;0
WireConnection;25;0;213;0
WireConnection;367;0;357;0
WireConnection;84;0;92;0
WireConnection;84;1;368;0
WireConnection;360;0;359;0
WireConnection;360;1;367;0
WireConnection;53;0;84;0
WireConnection;220;0;56;0
WireConnection;297;0;210;0
WireConnection;351;0;350;0
WireConnection;351;1;349;0
WireConnection;351;2;348;0
WireConnection;361;0;360;0
WireConnection;354;0;351;0
WireConnection;354;1;353;0
WireConnection;354;2;352;0
WireConnection;57;0;53;0
WireConnection;57;1;220;0
WireConnection;362;0;325;0
WireConnection;98;0;57;0
WireConnection;330;0;131;0
WireConnection;330;1;362;0
WireConnection;329;0;362;0
WireConnection;329;1;258;0
WireConnection;307;0;361;0
WireConnection;155;0;354;0
WireConnection;0;0;331;0
WireConnection;0;3;330;0
WireConnection;0;4;329;0
WireConnection;0;9;332;0
WireConnection;0;11;328;0
WireConnection;0;12;327;0
ASEEND*/
//CHKSM=BA5B75A7ACF2ED4B9F7E8C80FEC9DAE5A757B997
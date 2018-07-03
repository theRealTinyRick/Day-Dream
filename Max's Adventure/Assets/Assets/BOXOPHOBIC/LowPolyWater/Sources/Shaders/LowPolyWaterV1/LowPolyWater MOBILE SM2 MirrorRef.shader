// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/LowPolyWater/MOBILE SM2 MirrorRef"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[HideInInspector]_ReflectionTex("ReflectionTex", 2D) = "black" {}
		[Header(Surface Control)] _WaterColor("Water Color", Color) = (0,0.4980392,1,1)
		_SmoothNormals("Smooth Normals", Range( 0 , 1)) = 0.5
		[Header(Reflection and Depth)] _ReflectionIntensity("Reflection Intensity", Range( 0 , 1)) = 0.75
		_DepthOffset("Depth Offset", Float) = 1
		_DepthFalloff("Depth Falloff", Float) = 2
		[Header(Motion Control)] _WaveHeight("Wave Height", Float) = 0.2
		_WaveCycles("Wave Cycles", Float) = 1.5
		_WaveSpeed("Wave Speed", Float) = 25
		_WaveDirectionZX("Wave Direction Z-X", Range( 0 , 1)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 2.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float4 screenPos;
			float3 worldNormal;
		};

		uniform float4 _WaterColor;
		uniform sampler2D _ReflectionTex;
		uniform fixed _SmoothNormals;
		uniform fixed _ReflectionIntensity;
		uniform float _DepthOffset;
		uniform sampler2D _CameraDepthTexture;
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
			float3 appendResult408 = (float3(0.0 , _WaveCycles , 0.0));
			float3 VERTEXANIM98 = ( sin( ( ( lerpResult266 + ( _Time.x * _WaveSpeed ) ) * appendResult408 ) ) * ( _WaveHeight * 0.1 ) );
			v.vertex.xyz += VERTEXANIM98;
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 lerpResult6 = lerp( ase_worldNormal , float3(0,1,0) , _SmoothNormals);
			half3 NORMAL97 = lerpResult6;
			v.normal = NORMAL97;
		}

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPos402 = ase_screenPos;
			float2 componentMask398 = ase_screenPos402.xy;
			float3 lerpResult6 = lerp( i.worldNormal , float3(0,1,0) , _SmoothNormals);
			half3 NORMAL97 = lerpResult6;
			float2 componentMask404 = NORMAL97.xz;
			fixed4 REFLECTION25 = tex2D( _ReflectionTex, ( ( componentMask398 / ase_screenPos402.w ) + ( componentMask404 * float2( 0.25,0.25 ) ) ) );
			float4 lerpResult396 = lerp( _WaterColor , REFLECTION25 , _ReflectionIntensity);
			fixed4 ALBEDO155 = lerpResult396;
			o.Emission = ALBEDO155.xyz;
			float4 ase_screenPos333 = ase_screenPos;
			float eyeDepth334 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos333))));
			fixed DEPTH386 = saturate( pow( ( _DepthOffset + abs( ( eyeDepth334 - ase_screenPos333.w ) ) ) , ( 1.0 - max( _DepthFalloff , 1.0 ) ) ) );
			fixed OPACITY307 = saturate( ( ( 1.0 - DEPTH386 ) * 2.0 ) );
			o.Alpha = OPACITY307;
		}

		ENDCG
	}
	Fallback "Standard"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13106
1927;29;1906;1014;2180.033;-46.22095;1;True;False
Node;AmplifyShaderEditor.ScreenPosInputsNode;333;-1920,-1664;Float;False;1;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ScreenDepthNode;334;-1728,-1664;Float;False;0;1;0;FLOAT4;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;335;-1536,-1536;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;379;-1184,-1536;Float;False;Property;_DepthFalloff;Depth Falloff;5;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;265;-1920,-512;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;104;-1920,-352;Float;False;Constant;_Vector0;Vector 0;6;0;0,1,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;9;-1920,-192;Fixed;False;Property;_SmoothNormals;Smooth Normals;2;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;337;-1376,-1536;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMaxOp;381;-928,-1536;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;6;-1536,-512;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;380;-1504,-1664;Float;False;Property;_DepthOffset;Depth Offset;4;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;382;-784,-1536;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;383;-1136,-1664;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ScreenPosInputsNode;402;-1920,-1152;Float;False;1;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;97;-1344,-512;Half;False;NORMAL;-1;True;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;403;-1920,-960;Float;False;97;0;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;267;-1920,288;Fixed;False;Property;_WaveDirectionZX;Wave Direction Z-X;9;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;268;-1920,128;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;384;-544,-1664;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TimeNode;79;-1920,368;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;404;-1728,-960;Float;False;True;False;True;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.ComponentMaskNode;398;-1728,-1152;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;80;-1920,560;Float;False;Property;_WaveSpeed;Wave Speed;8;0;25;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-1664,368;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;160;-1664,560;Float;False;Property;_WaveCycles;Wave Cycles;7;0;1.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;405;-1472,-1088;Float;False;2;0;FLOAT2;0.0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;406;-1472,-960;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT2;0.25,0.25;False;1;FLOAT2
Node;AmplifyShaderEditor.SaturateNode;385;-400,-1664;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;266;-1632,208;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;92;-1472,304;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;408;-1456,512;Float;False;FLOAT3;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;387;-1920,-2048;Float;False;386;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;407;-1280,-1024;Float;False;2;2;0;FLOAT2;0.0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RegisterLocalVarNode;386;-256,-1664;Fixed;False;DEPTH;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;388;-1728,-2048;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-1280,304;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;399;-1024,-1152;Float;True;Property;_ReflectionTex;ReflectionTex;0;1;[HideInInspector];None;True;0;False;black;Auto;False;Object;-1;MipBias;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;56;-1280,560;Float;False;Property;_WaveHeight;Wave Height;6;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SinOpNode;53;-1088,304;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;393;-1536,-2048;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;2.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;223;-1920,-2688;Float;False;Property;_WaterColor;Water Color;1;0;0,0.4980392,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;23;-1920,-2416;Fixed;False;Property;_ReflectionIntensity;Reflection Intensity;3;0;0.75;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;153;-1920,-2496;Float;False;25;0;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;220;-1104,544;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.1;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-640,-1152;Fixed;False;REFLECTION;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;396;-1536,-2688;Float;False;3;0;FLOAT4;0.0;False;1;FLOAT4;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-896,304;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SaturateNode;317;-1280,-2048;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;98;-704,304;Float;False;VERTEXANIM;-1;True;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;155;-1280,-2688;Fixed;False;ALBEDO;-1;True;1;0;FLOAT4;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;331;-1920,-3456;Float;False;155;0;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;328;-1920,-3264;Float;False;98;0;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;307;-1088,-2048;Fixed;False;OPACITY;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;327;-1920,-3168;Float;False;97;0;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;332;-1920,-3360;Float;False;307;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1536,-3456;Float;False;True;0;Float;ASEMaterialInspector;0;0;Unlit;BOXOPHOBIC/LowPolyWater/MOBILE SM2 MirrorRef;False;False;False;False;True;True;True;True;True;False;True;True;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;0;SrcAlpha;OneMinusSrcAlpha;0;SrcAlpha;OneMinusSrcAlpha;OFF;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;Standard;-1;-1;-1;-1;0;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;280;-2176,-3456;Float;False;100;100;;0;// FINAL;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;274;-2176,-1664;Float;False;100;100;;0;// DEPTH AND EDGE CONTROL;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;272;-2176,-2688;Float;False;100;100;;0;// ALBEDO;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;278;-2176,-512;Float;False;100;100;;0;// NORMALS;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;306;-2176,-2048;Float;False;100;100;;0;// OPACITY;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;279;-2176,128;Float;False;100;100;;0;// WAVE ANIMATION;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;378;-2176,-1152;Float;False;100;100;;0;// REFLECTION;1,1,1,1;0;0
WireConnection;334;0;333;0
WireConnection;335;0;334;0
WireConnection;335;1;333;4
WireConnection;337;0;335;0
WireConnection;381;0;379;0
WireConnection;6;0;265;0
WireConnection;6;1;104;0
WireConnection;6;2;9;0
WireConnection;382;0;381;0
WireConnection;383;0;380;0
WireConnection;383;1;337;0
WireConnection;97;0;6;0
WireConnection;384;0;383;0
WireConnection;384;1;382;0
WireConnection;404;0;403;0
WireConnection;398;0;402;0
WireConnection;82;0;79;1
WireConnection;82;1;80;0
WireConnection;405;0;398;0
WireConnection;405;1;402;4
WireConnection;406;0;404;0
WireConnection;385;0;384;0
WireConnection;266;0;268;3
WireConnection;266;1;268;1
WireConnection;266;2;267;0
WireConnection;92;0;266;0
WireConnection;92;1;82;0
WireConnection;408;1;160;0
WireConnection;407;0;405;0
WireConnection;407;1;406;0
WireConnection;386;0;385;0
WireConnection;388;0;387;0
WireConnection;84;0;92;0
WireConnection;84;1;408;0
WireConnection;399;1;407;0
WireConnection;53;0;84;0
WireConnection;393;0;388;0
WireConnection;220;0;56;0
WireConnection;25;0;399;0
WireConnection;396;0;223;0
WireConnection;396;1;153;0
WireConnection;396;2;23;0
WireConnection;57;0;53;0
WireConnection;57;1;220;0
WireConnection;317;0;393;0
WireConnection;98;0;57;0
WireConnection;155;0;396;0
WireConnection;307;0;317;0
WireConnection;0;2;331;0
WireConnection;0;9;332;0
WireConnection;0;11;328;0
WireConnection;0;12;327;0
ASEEND*/
//CHKSM=677D1D5F352ED683DCAEF90A45E4EA2E67C70294
// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Xiexe/Toon/XSToonBOTWEye"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_ColorRamp("Color Ramp", 2D) = "white" {}
		_Highlights("Highlights", 2D) = "black" {}
		_SimulatedLightDirection("Simulated Light Direction", Vector) = (0,45,90,0)
		_RightEyeOffset("Right Eye Offset", Range( -1 , 1)) = 0
		_LeftEyeOffset("Left Eye Offset", Range( -1 , 1)) = 0
		_Shiftyness("Shiftyness", Range( 0 , 1)) = 0
		_Shine1Scale("Shine 1 Scale", Range( 1 , 3)) = 1
		_Shine2Scale("Shine 2 Scale", Range( 1 , 3)) = 1.7
		_Shine1Vertical("Shine 1 Vertical", Range( -0.2 , 0.2)) = 0
		_Shine1Horizontal("Shine 1 Horizontal", Range( -0.2 , 0.2)) = 0
		_Shine2Vertical("Shine 2 Vertical", Range( -0.2 , 0.2)) = 0
		_Shine2Horizontal("Shine 2 Horizontal", Range( -0.2 , 0.2)) = 0
		_FollowPower("FollowPower", Range( 0 , 1)) = 0.5
		_FollowLimit("FollowLimit", Range( 0 , 1)) = 0.5
		_RimWidth("Rim Width", Range( 0 , 1)) = 0
		_RimIntensity("Rim Intensity", Float) = 1
		[Toggle] _Emissive("Emissive?", Float) = 0.0
		_EmissiveTex("Emissive Tex", 2D) = "white" {}
		_EmissiveColor("Emissive Color", Color) = (1,1,1,0)
		_EmissiveStrength("Emissive Strength", Float) = 0
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature _EMISSIVE_ON
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
			INTERNAL_DATA
			float2 uv2_texcoord2;
			float3 worldPos;
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
		uniform sampler2D _ColorRamp;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _SimulatedLightDirection;
		uniform sampler2D _MainTex;
		uniform float _Shiftyness;
		uniform float _FollowPower;
		uniform float _FollowLimit;
		uniform float _RightEyeOffset;
		uniform float _LeftEyeOffset;
		uniform sampler2D _Highlights;
		uniform float _Shine1Horizontal;
		uniform float _Shine1Vertical;
		uniform float _Shine1Scale;
		uniform float _Shine2Horizontal;
		uniform float _Shine2Vertical;
		uniform float _Shine2Scale;
		uniform float _RimWidth;
		uniform float _RimIntensity;


		float3 ShadeSH9LC11_g82( float3 normal )
		{
			return ShadeSH9(half4(normal, 1.0));
		}


		float3 ShadeSH919_g83( float3 normal )
		{
			return ShadeSH9(half4(normal, 1.0));
		}


		float3 GraySH931_g83( float3 normal )
		{
			float3 ColoredSH9 = ShadeSH9(half4(normal, 1.0));
			float3 GraySH9 = (ColoredSH9.r + ColoredSH9.g + ColoredSH9.b) / 3;
			return GraySH9;
		}


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float3 EyeTrackCustomCode582( float _FollowPower , float _FollowLimit )
		{
			float3 centerEye = _WorldSpaceCameraPos.xyz;
			#if UNITY_SINGLE_PASS_STEREO
			int startIndex = unity_StereoEyeIndex;
			unity_StereoEyeIndex = 0;
			float3 leftEye = _WorldSpaceCameraPos;
			unity_StereoEyeIndex = 1;
			float3 rightEye = _WorldSpaceCameraPos;
			unity_StereoEyeIndex = startIndex;
			centerEye = lerp(leftEye, rightEye, 0.5);
			#endif
			float3 cam = mul(unity_WorldToObject, float4(centerEye, 1)).yxz;
			cam=normalize(cam);
			float inFront=dot(normalize(cam).xyz, half3(0,0,1));
			inFront=saturate((inFront-_FollowLimit)*10);
			cam = lerp(float3(0,0,1), cam, _FollowPower*inFront);
			return normalize(cam);
		}


		float3 ShadeSH9LC11_g70( float3 normal )
		{
			return ShadeSH9(half4(normal, 1.0));
		}


		float3 StereoWorldViewDir1_g79( float3 worldPos )
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
			#if DIRECTIONAL
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			float3 normal11_g82 = float3( 0,1,0 );
			float3 localShadeSH9LC11_g8211_g82 = ShadeSH9LC11_g82( normal11_g82 );
			float4 LightColor391 = ( _LightColor0 + float4( saturate( sqrt( localShadeSH9LC11_g8211_g82 ) ) , 0.0 ) );
			float LightAtt393 = ( sqrt( length( localShadeSH9LC11_g8211_g82 ) ) + ase_lightAtten );
			float2 uv2_Normal = i.uv2_texcoord2 * _Normal_ST.xy + _Normal_ST.zw;
			float3 NormalMap80 = UnpackNormal( tex2D( _Normal, uv2_Normal ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float4 transform490 = mul(unity_ObjectToWorld,float4( ase_vertexNormal , 0.0 ));
			float4 lerpResult492 = lerp( float4( WorldNormalVector( i , NormalMap80 ) , 0.0 ) , transform490 , 0.3);
			float4 vertexnorm493 = lerpResult492;
			float ifLocalVar6_g71 = 0;
			if( _WorldSpaceLightPos0.xyz.x == 0.0 )
				ifLocalVar6_g71 = 0.0;
			else
				ifLocalVar6_g71 = 1.0;
			float ifLocalVar5_g71 = 0;
			if( _WorldSpaceLightPos0.xyz.y == 0.0 )
				ifLocalVar5_g71 = 0.0;
			else
				ifLocalVar5_g71 = 1.0;
			float ifLocalVar7_g71 = 0;
			if( _WorldSpaceLightPos0.xyz.z == 0.0 )
				ifLocalVar7_g71 = 0.0;
			else
				ifLocalVar7_g71 = 1.0;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			float4 normalizeResult386 = normalize( _SimulatedLightDirection );
			float4 SimulatedLight85 = normalizeResult386;
			float4 ifLocalVar9_g71 = 0;
			if( ( ifLocalVar6_g71 * ifLocalVar5_g71 * ifLocalVar7_g71 ) == 0.0 )
				ifLocalVar9_g71 = SimulatedLight85;
			else
				ifLocalVar9_g71 = float4( ase_worldlightDir , 0.0 );
			float4 LightDir50 = ifLocalVar9_g71;
			float dotResult8_g80 = dot( vertexnorm493 , float4( LightDir50.xyz , 0.0 ) );
			float2 appendResult29_g80 = (float2(0.0 , ( ( dotResult8_g80 + 1.0 ) * 0.5 )));
			float4 tex2DNode470 = tex2D( _ColorRamp, appendResult29_g80 );
			float ShadingMask419 = tex2DNode470.r;
			float4 temp_output_6_0_g83 = ( LightColor391 * saturate( LightAtt393 ) * ShadingMask419 );
			float temp_output_16_0_g81 = saturate( tex2DNode470.r );
			float ToonShading78 = temp_output_16_0_g81;
			float4 temp_cast_11 = (ToonShading78).xxxx;
			float3 normal19_g83 = float3( 0,1,0 );
			float3 localShadeSH919_g8319_g83 = ShadeSH919_g83( normal19_g83 );
			float3 temp_output_4_0_g83 = saturate( localShadeSH919_g8319_g83 );
			float3 normal31_g83 = float3( 0,1,0 );
			float3 localGraySH931_g8331_g83 = GraySH931_g83( normal31_g83 );
			float mulTime648 = _Time.y * _Shiftyness;
			float2 temp_cast_13 = (round( mulTime648 )).xx;
			float simplePerlin2D660 = snoise( temp_cast_13 );
			float2 temp_cast_14 = (round( ( mulTime648 + 0.5 ) )).xx;
			float simplePerlin2D658 = snoise( temp_cast_14 );
			float2 appendResult670 = (float2(( simplePerlin2D660 * 0.01 ) , ( ( 1.0 - simplePerlin2D658 ) * 0.02 )));
			float2 appendResult668 = (float2(( simplePerlin2D658 * 0.01 ) , ( ( 1.0 - simplePerlin2D660 ) * 0.02 )));
			float2 lerpResult676 = lerp( appendResult670 , appendResult668 , saturate( fmod( mulTime648 , 0.2 ) ));
			float _FollowPower582 = _FollowPower;
			float _FollowLimit582 = _FollowLimit;
			float3 localEyeTrackCustomCode582582 = EyeTrackCustomCode582( _FollowPower582 , _FollowLimit582 );
			float2 appendResult585 = (float2(-localEyeTrackCustomCode582582.y , localEyeTrackCustomCode582582.x));
			float2 uv_TexCoord580 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float2 appendResult595 = (float2(( step( ase_vertex3Pos.x , 0.0 ) * _RightEyeOffset ) , 0.0));
			float2 appendResult608 = (float2(( step( -ase_vertex3Pos.x , 0.0 ) * _LeftEyeOffset ) , 0.0));
			float2 temp_output_610_0 = ( appendResult595 + appendResult608 );
			float2 appendResult593 = (float2(( ( 1.0 * appendResult585 ) + uv_TexCoord580 + temp_output_610_0 ).x , ( 1.0 - ( ( 1.0 * appendResult585 ) + uv_TexCoord580 + temp_output_610_0 ).y )));
			float temp_output_758_0 = step( ase_vertex3Pos.x , 1.0 );
			float2 appendResult757 = (float2(( temp_output_758_0 * _Shine1Horizontal ) , ( temp_output_758_0 * _Shine1Vertical )));
			float3 normal11_g70 = float3( 0,1,0 );
			float3 localShadeSH9LC11_g7011_g70 = ShadeSH9LC11_g70( normal11_g70 );
			float clampResult742 = clamp( ( _LightColor0 + float4( saturate( sqrt( localShadeSH9LC11_g7011_g70 ) ) , 0.0 ) ).a , 0.2 , 0.8 );
			float2 appendResult747 = (float2(( temp_output_758_0 * _Shine2Horizontal ) , ( temp_output_758_0 * _Shine2Vertical )));
			float4 MainTex194 = ( tex2D( _MainTex, ( lerpResult676 + appendResult593 ) ) + ( ( tex2D( _Highlights, ( ( ( ( uv_TexCoord580 + temp_output_610_0 + appendResult757 ) - float2( 0.5,0.5 ) ) * _Shine1Scale ) + float2( 0.5,0.5 ) ) ) * clampResult742 ) + ( tex2D( _Highlights, ( ( ( ( uv_TexCoord580 + temp_output_610_0 + appendResult747 ) - float2( 0.5,0.5 ) ) * _Shine2Scale ) + float2( 0.5,0.5 ) ) ) * clampResult742 ) ) );
			float4 temp_output_20_0_g83 = MainTex194;
			float4 ase_vertex4Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 transform2_g79 = mul(unity_ObjectToWorld,ase_vertex4Pos);
			float3 worldPos1_g79 = transform2_g79.xyz;
			float3 localStereoWorldViewDir1_g791_g79 = StereoWorldViewDir1_g79( worldPos1_g79 );
			float3 temp_output_790_0 = localStereoWorldViewDir1_g791_g79;
			float dotResult343 = dot( vertexnorm493 , float4( temp_output_790_0 , 0.0 ) );
			float4 RimLight229 = ( step( 0.9 , pow( ( 1.0 - saturate( dotResult343 ) ) , ( 1.0 - _RimWidth ) ) ) * MainTex194 * _RimIntensity * pow( ToonShading78 , 10.0 ) );
			c.rgb = saturate( ( ( ( temp_output_6_0_g83 + ( ( 1.0 - saturate( temp_cast_11 ) ) * float4( ( temp_output_4_0_g83 - ( 0.2 * localGraySH931_g8331_g83 ) ) , 0.0 ) ) ) * temp_output_20_0_g83 ) + saturate( ( temp_output_6_0_g83 * ( RimLight229 + float4( 0,0,0,0 ) ) * temp_output_20_0_g83 ) ) ) ).rgb;
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
			float2 uv_EmissiveTex = i.uv_texcoord * _EmissiveTex_ST.xy + _EmissiveTex_ST.zw;
			#ifdef _EMISSIVE_ON
				float4 staticSwitch131 = ( _EmissiveColor * tex2D( _EmissiveTex, uv_EmissiveTex ) * _EmissiveStrength );
			#else
				float4 staticSwitch131 = float4(0,0,0,0);
			#endif
			float4 Emissive122 = staticSwitch131;
			float4 temp_cast_1 = (0.0).xxxx;
			float4 temp_cast_2 = (2.0).xxxx;
			float4 clampResult132 = clamp( Emissive122 , temp_cast_1 , temp_cast_2 );
			o.Emission = clampResult132.rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows noshadow 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack1.zw = customInputData.uv2_texcoord2;
				o.customPack1.zw = v.texcoord1;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.uv2_texcoord2 = IN.customPack1.zw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=14101
-1410;223;1141;523;384.4511;-79.43025;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;613;-5828.836,-774.0202;Float;False;2759.605;844.5341;Comment;46;597;611;612;602;596;1;593;592;591;581;587;610;608;595;586;585;584;607;594;609;603;604;583;582;573;572;580;677;712;717;737;740;741;742;745;755;753;757;776;777;779;780;781;744;783;785;2D Eye Tracking;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;597;-5769.432,-459.4929;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;572;-5792.714,-622.786;Float;False;Property;_FollowLimit;FollowLimit;15;0;Create;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;573;-5776.368,-698.3417;Float;False;Property;_FollowPower;FollowPower;14;0;Create;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;602;-5513.043,-218.4708;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;582;-5502.475,-663.9699;Float;False;float3 centerEye = _WorldSpaceCameraPos.xyz@$#if UNITY_SINGLE_PASS_STEREO$int startIndex = unity_StereoEyeIndex@$unity_StereoEyeIndex = 0@$float3 leftEye = _WorldSpaceCameraPos@$unity_StereoEyeIndex = 1@$float3 rightEye = _WorldSpaceCameraPos@$unity_StereoEyeIndex = startIndex@$centerEye = lerp(leftEye, rightEye, 0.5)@$#endif$float3 cam = mul(unity_WorldToObject, float4(centerEye, 1)).yxz@$$cam=normalize(cam)@$float inFront=dot(normalize(cam).xyz, half3(0,0,1))@$inFront=saturate((inFront-_FollowLimit)*10)@$cam = lerp(float3(0,0,1), cam, _FollowPower*inFront)@$$return normalize(cam)@$;3;False;2;True;_FollowPower;FLOAT;0.0;In;True;_FollowLimit;FLOAT;0.0;In;Eye Track Custom Code;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;691;-5909.034,-1104.281;Float;False;Property;_Shiftyness;Shiftyness;7;0;Create;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;596;-5518.866,-373.3898;Float;False;Property;_RightEyeOffset;Right Eye Offset;5;0;Create;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;748;-5378.06,167.6624;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;611;-5519.053,-463.3992;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;604;-5507.191,-142.8054;Float;False;Property;_LeftEyeOffset;Left Eye Offset;6;0;Create;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;612;-5384.171,-230.2384;Float;False;2;0;FLOAT;0,0,0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;609;-5380.565,-68.52758;Float;False;Constant;_Float7;Float 7;3;0;Create;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;648;-5635.55,-1098.775;Float;False;1;0;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;751;-5376.284,393.8912;Float;False;Property;_Shine2Vertical;Shine 2 Vertical;12;0;Create;0;-0.2;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;758;-5185.15,189.6367;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;749;-5376.426,319.7451;Float;False;Property;_Shine2Horizontal;Shine 2 Horizontal;13;0;Create;0;-0.2;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;755;-5381.362,14.31939;Float;False;Property;_Shine1Horizontal;Shine 1 Horizontal;11;0;Create;0;-0.2;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;583;-5264.125,-631.8622;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;607;-5242.555,-211.8447;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;594;-5392.241,-299.112;Float;False;Constant;_Float5;Float 5;3;0;Create;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;603;-5257.364,-442.4293;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;756;-5380.219,88.46579;Float;False;Property;_Shine1Vertical;Shine 1 Vertical;10;0;Create;0;-0.2;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;647;-5610.977,-1029.578;Float;False;Constant;_Float9;Float 9;17;0;Create;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;595;-5126.647,-443.5059;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;608;-5116.356,-212.9212;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;649;-5292.76,-1095.248;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;584;-5010.505,-653.2122;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;753;-4822.797,7.454723;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;754;-4823.134,119.5162;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;752;-4827.318,329.177;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;750;-4826.982,217.1155;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;610;-4948.047,-340.4135;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RoundOpNode;652;-5178.067,-1097.657;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;580;-4950.665,-201.1278;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;747;-4648.057,260.0233;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;757;-4674.375,61.89484;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;2;-2973.709,1849.429;Float;True;Property;_Normal;Normal;1;0;Create;None;True;1;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;586;-4868.872,-679.1768;Float;False;Constant;_Float4;Float 4;3;0;Create;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;585;-4862.693,-607.5117;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;20;-2973.821,1681.098;Float;False;Property;_SimulatedLightDirection;Simulated Light Direction;4;0;Create;0,45,90,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RoundOpNode;653;-5215.4,-1211.406;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;783;-4401.582,-19.46941;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;488;-3020.987,-1710.635;Float;False;80;0;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;386;-2723.834,1684.543;Float;False;1;0;FLOAT4;0.0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NormalVertexDataNode;489;-3006.606,-1591.203;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;80;-2694.367,1851.876;Float;False;NormalMap;-1;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;484;-594.828,704.4927;Float;False;838.8608;190.8382;Comment;2;50;363;Realtime Light Check;1,1,1,1;0;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;660;-5051.054,-1220.44;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;776;-4430.955,-385.9811;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;671;-5403.642,-961.0176;Float;False;Constant;_Float16;Float 16;17;0;Create;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;658;-5058.091,-1098.978;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;587;-4726.012,-603.2203;Float;False;2;2;0;FLOAT;0,0;False;1;FLOAT2;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;777;-4270.613,-386.7945;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;785;-4241.239,-20.2828;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;711;-4818.575,-1097.718;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;494;-2795.358,-1422.726;Float;False;Constant;_Float2;Float 2;15;0;Create;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;85;-2580.655,1683.546;Float;False;SimulatedLight;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;784;-4420.109,105.5942;Float;False;Property;_Shine2Scale;Shine 2 Scale;9;0;Create;1.7;1;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;490;-2822.205,-1591.041;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;491;-2805.8,-1733.423;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;363;-544.7646,779.322;Float;False;85;0;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;581;-4563.896,-579.4142;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FmodOpNode;672;-5204.18,-999.1319;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;710;-4814.628,-990.0397;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;780;-4429.758,-266.8257;Float;False;Property;_Shine1Scale;Shine 1 Scale;8;0;Create;1;1;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;591;-4436.457,-577.0677;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.CommentaryNode;487;-3006.874,-1258.822;Float;False;2063.104;836.519;Comment;7;30;419;78;470;362;458;514;Toon Shadowing;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;695;-4631.697,-927.5052;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.02;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;486;-3003.168,-12.66419;Float;False;1658.42;514.8104;Comment;14;229;228;451;347;264;250;450;346;350;345;438;349;343;342;Fresnel;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;694;-4630.503,-1018.252;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;564;-331.3629,784.9867;Float;False;RealtimeLightCheck;-1;;71;c40d88c2a1d03794e8901f88410cb42c;1;11;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;697;-4630.481,-1209.004;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;492;-2546.494,-1628.291;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;779;-4105.583,-373.3772;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;786;-4076.208,-6.865494;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;696;-4628.319,-1111.831;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.02;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;740;-4090.476,-180.331;Float;False;LightColAtten;-1;;70;6e57b348da64d324db411d4d8a5d7e07;0;2;FLOAT;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;675;-5073.945,-993.1839;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;741;-3894.48,-180.031;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;787;-3935.323,-0.1566925;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;-3.764242,782.3219;Float;False;LightDir;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;342;-2955.168,51.33585;Float;False;493;0;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;670;-4474.818,-1164.898;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;493;-2236.724,-1576.12;Float;False;vertexnorm;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;781;-3964.699,-366.6684;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;790;-3099.701,145.5639;Float;False;StereoCorrectWorldViewDir;-1;;79;65d72d2730533f3419c436acef6c50e1;0;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;458;-2990.874,-954.8215;Float;False;493;0;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;668;-4486.395,-977.3119;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;362;-2974.874,-1034.822;Float;False;50;0;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;592;-4201.039,-503.3763;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;669;-4422.602,-819.0206;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;343;-2747.168,51.33585;Float;False;2;0;FLOAT4;0.0,0,0,0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;676;-4110.351,-1045.409;Float;False;3;0;FLOAT2;0.0,0,0;False;1;FLOAT2;0,0,0;False;2;FLOAT;0.0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;775;-3806.777,-23.54493;Float;True;Property;_HighlightSmall;HighlightSmall;3;0;Create;None;True;0;False;black;Auto;False;Instance;712;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;593;-4052.454,-571.6766;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;742;-3659.424,-193.886;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.2;False;2;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;537;-2782.874,-1018.822;Float;False;ToonColorRamp;-1;;80;d02e43109b627e14d9e44bcb81f6b905;2;25;FLOAT3;0,0,0;False;24;FLOAT4;0,0,0,0;False;1;FLOAT2;34
Node;AmplifyShaderEditor.SamplerNode;712;-3811.453,-389.1498;Float;True;Property;_Highlights;Highlights;3;0;Create;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;470;-2324.502,-1066.822;Float;True;Property;_ColorRamp;Color Ramp;2;0;Create;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;737;-3517.525,-330.4262;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;438;-2635.168,35.33581;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;677;-3775.057,-582.5716;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;349;-2795.168,179.3359;Float;False;Property;_RimWidth;Rim Width;19;0;Create;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;30;-2244.666,-875.7953;Float;False;Constant;_ShadowColorTint;Shadow Color Tint;4;0;Create;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;744;-3505.49,-18.76662;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;789;-2012.447,-1038.678;Float;False;ToonShading;-1;;81;473229bf9f3050e43805be7e9906d94b;2;34;FLOAT;0.0;False;22;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-3618.027,-604.8455;Float;True;Property;_MainTex;Main Tex;0;0;Create;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;350;-2523.168,131.3358;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;345;-2491.168,51.33585;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;745;-3320.842,-272.8481;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;124;-2975.163,2217.917;Float;False;Property;_EmissiveColor;Emissive Color;23;0;Create;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;121;-2976.662,2384.714;Float;True;Property;_EmissiveTex;Emissive Tex;22;0;Create;None;True;0;True;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;396;-2975.227,2576.893;Float;False;Property;_EmissiveStrength;Emissive Strength;24;0;Create;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;717;-3297.004,-508.2143;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;346;-2347.168,51.33585;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;10.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;78;-1727.313,-993.6875;Float;False;ToonShading;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;450;-2299.168,371.3358;Float;False;78;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;264;-2251.168,227.3359;Float;False;Property;_RimIntensity;Rim Intensity;20;0;Create;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;194;-3172.374,-512.399;Float;False;MainTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;250;-2251.168,147.3358;Float;False;194;0;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;451;-2000.768,313.6358;Float;False;2;0;FLOAT;0,0,0,0;False;1;FLOAT;10.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;570;-593.5505,958.4515;Float;False;527.1277;226.0378;Comment;3;393;391;566;Get Light Color and Atten;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;127;-2971.691,2050.811;Float;False;Constant;_Vector4;Vector4;14;0;Create;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;347;-2203.168,51.33585;Float;False;2;0;FLOAT;0.9;False;1;FLOAT;0.9;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;125;-2772.164,2235.017;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;228;-1771.169,99.33584;Float;True;4;4;0;FLOAT;0.0;False;1;COLOR;0;False;2;FLOAT;0,0,0,0;False;3;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;131;-2645.594,2237.411;Float;False;Property;_Emissive;Emissive?;21;0;Create;0;False;True;;Toggle;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;566;-569.7901,1043.544;Float;False;LightColAtten;-1;;82;6e57b348da64d324db411d4d8a5d7e07;0;2;FLOAT;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;542;76.48224,89.81485;Float;False;194;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;134;364.8889,119.8357;Float;False;Constant;_Float3;Float 3;14;0;Create;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;540;42.50439,565.6479;Float;False;419;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;541;44.48224,329.8148;Float;False;78;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;393;-273.4435,1030.972;Float;False;LightAtt;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;122;-2435.264,2242.416;Float;False;Emissive;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;485;-3003.694,616.1166;Float;False;1548.804;962.9491;Comment;22;283;280;286;287;214;253;215;222;223;282;454;307;453;306;293;303;299;297;313;356;304;294;Specular Highlights;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;133;320.6891,44.43568;Float;False;Constant;_Float0;Float 0;14;0;Create;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;549;4538.961,163.589;Float;False;1858.359;753.5493;Comment;24;357;359;337;428;251;394;420;395;390;423;532;531;11;409;533;422;424;336;406;8;91;252;392;230;Old;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;123;306.2181,-50.1591;Float;False;122;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;419;-2022.173,-1191.691;Float;False;ShadingMask;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;539;70.6467,489.8146;Float;False;393;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;391;-276.2355,1110.71;Float;False;LightColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;538;60.48224,409.8148;Float;False;391;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;229;-1547.169,83.33584;Float;False;RimLight;-1;True;1;0;COLOR;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;543;76.48224,169.8148;Float;False;229;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;336;5442.07,801.1921;Float;False;194;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;390;5022.951,241.4606;Float;False;391;0;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;307;-2008.382,1132.428;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;10.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;282;-2385.595,1115.396;Float;False;Property;_SpecularIntensity;Specular Intensity;18;0;Create;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;253;-2971.824,650.5543;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;313;-2923.132,1279.643;Float;False;493;0;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;531;4933.651,511.4353;Float;False;1;0;FLOAT;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;280;-2721.534,878.721;Float;True;Property;_SpecularPattern;Specular Pattern;17;0;Create;None;True;1;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;293;-2322.877,1200.862;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;453;-2261.014,1323.546;Float;False;Property;_SpecularArea;Specular Area;25;0;Create;0;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;420;4615.684,377.3103;Float;False;419;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;252;5444.018,724.6115;Float;False;215;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;215;-1682.211,732.6954;Float;False;SpecMap;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;357;5756.493,597.7551;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;356;-2917.27,1062.347;Float;False;50;0;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;454;-1855.161,1069.889;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;423;5085.083,337.5976;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;303;-2461.204,1354.685;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;545;76.48224,249.8148;Float;False;215;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;395;6219.343,356.3578;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;424;5815.274,695.3403;Float;False;393;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;337;5895.96,504.1017;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;4588.396,509.2448;Float;False;78;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;533;4935.74,595.4968;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;428;5733.9,377.2695;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;294;-2956.426,1410.734;Float;False;World;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ClampOpNode;132;501.3884,13.23568;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;297;-2728.212,1412.844;Float;False;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;5249.76,314.4272;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CustomExpressionNode;8;4740.12,593.502;Float;False;return ShadeSH9(half4(normal, 1.0))@$;3;False;1;True;normal;FLOAT3;0,1,0;In;ShadeSH9;1;0;FLOAT3;0,1,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;304;-2724.579,1494.414;Float;False;Constant;_Float1;Float 1;15;0;Create;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;306;-2180.583,1195.169;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;791;368,224;Float;False;ToonFinalGather;-1;;83;9310aafaedb10e74d8f6dd103e2962ae;7;20;COLOR;0,0,0,0;False;21;COLOR;0,0,0,0;False;22;COLOR;0,0,0,0;False;17;COLOR;0,0,0,0;False;14;COLOR;0,0,0,0;False;15;FLOAT;0.0;False;16;FLOAT;0.0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;409;5409.053,333.0302;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;406;5437.394,456.9805;Float;False;194;0;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;422;5090.277,520.6968;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT3;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;514;-2022.62,-1117.79;Float;False;ColorRamp;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;214;-2731.829,671.1281;Float;True;Property;_SpecularMap;Specular Map;16;0;Create;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;392;5694.793,508.8976;Float;False;391;0;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;251;6075.319,359.1691;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;283;-2079.473,734.5052;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;532;4792.574,511.0259;Float;False;1;0;FLOAT;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;394;4893.319,329.9084;Float;False;393;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;286;-2222.932,677.4785;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ReflectOpNode;299;-2643.147,1133.177;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;359;5668.993,709.7056;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;223;-2377.896,944.1535;Float;False;Constant;_SpecularColor;Specular Color;15;0;Create;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;287;-2406.435,664.908;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;230;5398.844,537.6535;Float;False;229;0;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;222;-1849.408,739.7094;Float;False;4;4;0;FLOAT;0.0;False;1;FLOAT;0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;666.7697,-15.27755;Float;False;True;2;Float;;0;0;CustomLighting;Xiexe/Toon/XSToonBOTWEye;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Custom;0.5;True;True;0;True;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;96;255;255;7;3;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;2E-05;0.5780931,0.235294,1,1;VertexOffset;True;False;Cylindrical;False;Relative;0;;26;-1;-1;-1;0;0;0;False;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0.0,0,0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;602;0;597;1
WireConnection;582;0;573;0
WireConnection;582;1;572;0
WireConnection;611;0;597;1
WireConnection;612;0;602;0
WireConnection;648;0;691;0
WireConnection;758;0;748;1
WireConnection;583;0;582;0
WireConnection;607;0;612;0
WireConnection;607;1;604;0
WireConnection;603;0;611;0
WireConnection;603;1;596;0
WireConnection;595;0;603;0
WireConnection;595;1;594;0
WireConnection;608;0;607;0
WireConnection;608;1;609;0
WireConnection;649;0;648;0
WireConnection;649;1;647;0
WireConnection;584;0;583;1
WireConnection;753;0;758;0
WireConnection;753;1;755;0
WireConnection;754;0;758;0
WireConnection;754;1;756;0
WireConnection;752;0;758;0
WireConnection;752;1;751;0
WireConnection;750;0;758;0
WireConnection;750;1;749;0
WireConnection;610;0;595;0
WireConnection;610;1;608;0
WireConnection;652;0;649;0
WireConnection;747;0;750;0
WireConnection;747;1;752;0
WireConnection;757;0;753;0
WireConnection;757;1;754;0
WireConnection;585;0;584;0
WireConnection;585;1;583;0
WireConnection;653;0;648;0
WireConnection;783;0;580;0
WireConnection;783;1;610;0
WireConnection;783;2;747;0
WireConnection;386;0;20;0
WireConnection;80;0;2;0
WireConnection;660;0;653;0
WireConnection;776;0;580;0
WireConnection;776;1;610;0
WireConnection;776;2;757;0
WireConnection;658;0;652;0
WireConnection;587;0;586;0
WireConnection;587;1;585;0
WireConnection;777;0;776;0
WireConnection;785;0;783;0
WireConnection;711;0;658;0
WireConnection;85;0;386;0
WireConnection;490;0;489;0
WireConnection;491;0;488;0
WireConnection;581;0;587;0
WireConnection;581;1;580;0
WireConnection;581;2;610;0
WireConnection;672;0;648;0
WireConnection;672;1;671;0
WireConnection;710;0;660;0
WireConnection;591;0;581;0
WireConnection;695;0;710;0
WireConnection;694;0;658;0
WireConnection;564;11;363;0
WireConnection;697;0;660;0
WireConnection;492;0;491;0
WireConnection;492;1;490;0
WireConnection;492;2;494;0
WireConnection;779;0;777;0
WireConnection;779;1;780;0
WireConnection;786;0;785;0
WireConnection;786;1;784;0
WireConnection;696;0;711;0
WireConnection;675;0;672;0
WireConnection;741;0;740;0
WireConnection;787;0;786;0
WireConnection;50;0;564;0
WireConnection;670;0;697;0
WireConnection;670;1;696;0
WireConnection;493;0;492;0
WireConnection;781;0;779;0
WireConnection;668;0;694;0
WireConnection;668;1;695;0
WireConnection;592;0;591;1
WireConnection;669;0;675;0
WireConnection;343;0;342;0
WireConnection;343;1;790;0
WireConnection;676;0;670;0
WireConnection;676;1;668;0
WireConnection;676;2;669;0
WireConnection;775;1;787;0
WireConnection;593;0;591;0
WireConnection;593;1;592;0
WireConnection;742;0;741;3
WireConnection;537;25;362;0
WireConnection;537;24;458;0
WireConnection;712;1;781;0
WireConnection;470;1;537;34
WireConnection;737;0;712;0
WireConnection;737;1;742;0
WireConnection;438;0;343;0
WireConnection;677;0;676;0
WireConnection;677;1;593;0
WireConnection;744;0;775;0
WireConnection;744;1;742;0
WireConnection;789;34;470;1
WireConnection;789;22;30;0
WireConnection;1;1;677;0
WireConnection;350;0;349;0
WireConnection;345;0;438;0
WireConnection;745;0;737;0
WireConnection;745;1;744;0
WireConnection;717;0;1;0
WireConnection;717;1;745;0
WireConnection;346;0;345;0
WireConnection;346;1;350;0
WireConnection;78;0;789;0
WireConnection;194;0;717;0
WireConnection;451;0;450;0
WireConnection;347;1;346;0
WireConnection;125;0;124;0
WireConnection;125;1;121;0
WireConnection;125;2;396;0
WireConnection;228;0;347;0
WireConnection;228;1;250;0
WireConnection;228;2;264;0
WireConnection;228;3;451;0
WireConnection;131;0;125;0
WireConnection;131;1;127;0
WireConnection;393;0;566;1
WireConnection;122;0;131;0
WireConnection;419;0;470;1
WireConnection;391;0;566;0
WireConnection;229;0;228;0
WireConnection;307;0;306;0
WireConnection;307;1;453;0
WireConnection;531;0;532;0
WireConnection;293;0;299;0
WireConnection;293;1;303;0
WireConnection;215;0;222;0
WireConnection;357;0;230;0
WireConnection;357;1;359;0
WireConnection;454;0;307;0
WireConnection;423;0;394;0
WireConnection;303;0;790;0
WireConnection;303;1;304;0
WireConnection;395;0;251;0
WireConnection;337;0;392;0
WireConnection;337;1;357;0
WireConnection;337;2;424;0
WireConnection;533;0;8;0
WireConnection;428;0;409;0
WireConnection;428;1;406;0
WireConnection;132;0;123;0
WireConnection;132;1;133;0
WireConnection;132;2;134;0
WireConnection;297;0;294;0
WireConnection;11;0;390;0
WireConnection;11;1;423;0
WireConnection;11;2;420;0
WireConnection;306;0;293;0
WireConnection;791;20;542;0
WireConnection;791;21;543;0
WireConnection;791;17;541;0
WireConnection;791;14;538;0
WireConnection;791;15;539;0
WireConnection;791;16;540;0
WireConnection;409;0;11;0
WireConnection;409;1;422;0
WireConnection;422;0;531;0
WireConnection;422;1;533;0
WireConnection;514;0;470;1
WireConnection;214;1;253;0
WireConnection;251;0;428;0
WireConnection;251;1;337;0
WireConnection;283;0;286;0
WireConnection;283;1;280;1
WireConnection;532;0;91;0
WireConnection;286;0;214;2
WireConnection;286;1;287;0
WireConnection;299;0;356;0
WireConnection;299;1;313;0
WireConnection;359;0;252;0
WireConnection;359;1;336;0
WireConnection;287;0;214;1
WireConnection;222;0;283;0
WireConnection;222;1;223;1
WireConnection;222;2;282;0
WireConnection;222;3;454;0
WireConnection;0;2;132;0
WireConnection;0;13;791;0
ASEEND*/
//CHKSM=A943E23DC4BBF3E3EC59E7F1E979370351FF1CC2
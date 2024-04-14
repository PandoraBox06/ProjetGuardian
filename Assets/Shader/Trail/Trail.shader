// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Trail"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_TimeScale("TimeScale", Float) = 1.2
		_Float("Float", Float) = 6
		_MainTex1("Main Tex", 2D) = "white" {}
		_FlowStrength("Flow Strength", Float) = 0
		_MainTex2("Main Tex", 2D) = "white" {}
		_ColorEnd("Color End", Color) = (0.09433961,0.09433961,0.09433961,0)
		_ColorStart("Color Start", Color) = (0.1243973,0,1,0)
		_Emission("Emission", Float) = 5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float4 _ColorStart;
		uniform float4 _ColorEnd;
		uniform float _Emission;
		uniform sampler2D _MainTex1;
		uniform float _TimeScale;
		uniform sampler2D _MainTex;
		uniform float _FlowStrength;
		uniform sampler2D _MainTex2;
		uniform float _Float;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float U38 = i.uv_texcoord.x;
			float4 lerpResult76 = lerp( _ColorStart , _ColorEnd , ( U38 * 1.0 ));
			float mulTime28 = _Time.y * _TimeScale;
			float Time29 = mulTime28;
			float2 UV31 = i.uv_texcoord;
			float2 panner32 = ( Time29 * float2( -1,0 ) + UV31);
			float2 panner54 = ( Time29 * float2( -0.8,0 ) + ( float4( UV31, 0.0 , 0.0 ) + ( ( ( (tex2D( _MainTex, panner32 )).rgba + -0.5 ) * 2.0 ) * _FlowStrength * U38 ) ).rg);
			float2 panner59 = ( Time29 * float2( -1,-0.2 ) + UV31);
			float clampResult63 = clamp( ( ( ( 1.0 - U38 ) + -0.55 ) * 2.0 ) , 0.25 , 0.75 );
			float clampResult68 = clamp( ( ( tex2D( _MainTex2, panner59 ).g + clampResult63 ) - ( U38 * 0.9 ) ) , 0.0 , 1.0 );
			float clampResult43 = clamp( ( i.uv_texcoord.y * ( 1.0 - i.uv_texcoord.y ) * ( 1.0 - U38 ) * _Float ) , 0.0 , 1.0 );
			float Mask45 = ( clampResult43 * 1.0 );
			float temp_output_70_0 = ( tex2D( _MainTex1, panner54 ).b * clampResult68 * Mask45 * i.vertexColor.a );
			o.Emission = ( ( lerpResult76 * i.vertexColor * _Emission ) * temp_output_70_0 ).rgb;
			o.Alpha = temp_output_70_0;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.SimpleTimeNode;28;-3204.535,170.6853;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-3048.534,-8.514646;Inherit;False;UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;29;-2924.294,166.1254;Inherit;False;Time;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;38;-3046.138,382.8443;Inherit;False;U;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-3308.135,-9.874695;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;40;-2752.858,388.0445;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;39;-2869.338,290.2845;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-2576.618,335.6445;Inherit;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;43;-2347.174,317.5092;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-1848.014,349.9844;Inherit;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-2091.024,314.7455;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-1349.879,-33.98555;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;55;-612.9591,-104.6308;Inherit;True;Property;_MainTex1;Main Tex;3;0;Create;True;0;0;0;False;0;False;-1;6f9400ba80b14514b89814508edfc7e1;6f9400ba80b14514b89814508edfc7e1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;52;-1406.672,-250.075;Inherit;False;31;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;-3041.487,-640.3345;Inherit;False;31;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-3041.005,-718.2863;Inherit;False;38;U;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;56;-3038.815,-560.3425;Inherit;False;29;Time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;33;-2466.934,-20.51464;Inherit;True;Property;_MainTex;Main Tex;0;0;Create;True;0;0;0;False;0;False;-1;6f9400ba80b14514b89814508edfc7e1;6f9400ba80b14514b89814508edfc7e1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;63;-2058.049,-504.8741;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.25;False;2;FLOAT;0.75;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;61;-2352.531,-513.6284;Inherit;False;ConstantBiasScale;-1;;2;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT;0;False;1;FLOAT;-0.55;False;2;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;60;-2634.679,-473.1563;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;59;-2650.869,-622.325;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-1,-0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;32;-2737.334,-12.51466;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;47;-1614.45,-39.52635;Inherit;False;ConstantBiasScale;-1;;3;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;COLOR;0,0,0,0;False;1;FLOAT;-0.5;False;2;FLOAT;2;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;46;-1884.212,-33.6403;Inherit;False;True;True;True;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-1096.39,-72.77108;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-1107.489,94.50912;Inherit;True;29;Time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;65;-1867.247,-657.9267;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-1855.247,-823.5268;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.9;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;67;-1592.846,-773.9269;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;68;-1272.046,-748.327;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;50;-1610.295,172.4073;Inherit;True;38;U;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;71;-476.6951,-364.6312;Inherit;False;45;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;72;-516.6276,-703.3678;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;54;-873.3747,-76.92673;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.8,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;69;-1114.311,-924.858;Inherit;False;38;U;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-843.1118,-918.6036;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;74;-1059.432,-1153.643;Inherit;False;Property;_ColorEnd;Color End;6;0;Create;True;0;0;0;False;0;False;0.09433961,0.09433961,0.09433961,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;75;-1058.392,-1355.403;Inherit;False;Property;_ColorStart;Color Start;7;0;Create;True;0;0;0;False;0;False;0.1243973,0,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;76;-626.792,-1263.883;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-190.6903,-496.5079;Inherit;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;357.0486,-986.2037;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-44.3919,-1029.884;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;79;4.295301,-867.6441;Inherit;False;Property;_Keyword0;;9;0;Create;False;0;0;0;False;0;False;0;1;1;True;_Keyword0;Toggle;2;Key0;Key1;Create;True;False;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;26;702.1498,-784.0085;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Trail;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SamplerNode;62;-2401.918,-717.1146;Inherit;True;Property;_MainTex2;Main Tex;5;0;Create;True;0;0;0;False;0;False;-1;6f9400ba80b14514b89814508edfc7e1;6f9400ba80b14514b89814508edfc7e1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;27;-3371.238,166.5905;Inherit;False;Property;_TimeScale;TimeScale;1;0;Create;True;0;0;0;False;0;False;1.2;1.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-2798.618,478.5245;Inherit;False;Property;_Float;Float;2;0;Create;True;0;0;0;False;0;False;6;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-1595.343,80.98492;Inherit;False;Property;_FlowStrength;Flow Strength;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-570.632,-990.3637;Inherit;False;Property;_Emission;Emission;8;0;Create;True;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-284.6317,-858.2841;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
WireConnection;28;0;27;0
WireConnection;31;0;30;0
WireConnection;29;0;28;0
WireConnection;38;0;30;1
WireConnection;40;0;38;0
WireConnection;39;0;30;2
WireConnection;42;0;30;2
WireConnection;42;1;39;0
WireConnection;42;2;40;0
WireConnection;42;3;41;0
WireConnection;43;0;42;0
WireConnection;45;0;44;0
WireConnection;44;0;43;0
WireConnection;48;0;47;0
WireConnection;48;1;49;0
WireConnection;48;2;50;0
WireConnection;55;1;54;0
WireConnection;33;1;32;0
WireConnection;63;0;61;0
WireConnection;61;3;60;0
WireConnection;60;0;58;0
WireConnection;59;0;57;0
WireConnection;59;1;56;0
WireConnection;32;0;31;0
WireConnection;32;1;29;0
WireConnection;47;3;46;0
WireConnection;46;0;33;0
WireConnection;51;0;52;0
WireConnection;51;1;48;0
WireConnection;65;0;62;2
WireConnection;65;1;63;0
WireConnection;66;0;58;0
WireConnection;67;0;65;0
WireConnection;67;1;66;0
WireConnection;68;0;67;0
WireConnection;54;0;51;0
WireConnection;54;1;53;0
WireConnection;73;0;69;0
WireConnection;76;0;75;0
WireConnection;76;1;74;0
WireConnection;76;2;73;0
WireConnection;70;0;55;3
WireConnection;70;1;68;0
WireConnection;70;2;71;0
WireConnection;70;3;72;4
WireConnection;81;0;78;0
WireConnection;81;1;70;0
WireConnection;78;0;76;0
WireConnection;78;1;72;0
WireConnection;78;2;77;0
WireConnection;79;1;80;0
WireConnection;79;0;70;0
WireConnection;26;2;81;0
WireConnection;26;9;70;0
WireConnection;62;1;59;0
ASEEND*/
//CHKSM=2A2741B533258724300AF51E2AFEADAB57953C98
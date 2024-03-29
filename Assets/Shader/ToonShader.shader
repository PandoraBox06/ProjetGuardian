// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ToonShader"
{
	Properties
	{
		_Reflexion("Reflexion", Range( 0 , 1)) = 0.97
		_Color0("Color 0", Color) = (1,1,1,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldNormal;
			float3 worldPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _Reflexion;
		uniform float4 _Color0;
		uniform sampler2D _TextureSample0;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			ase_vertexNormal = normalize( ase_vertexNormal );
			float4 ase_vertex4Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 ase_objectlightDir = normalize( ObjSpaceLightDir( ase_vertex4Pos ) );
			float dotResult4 = dot( ase_vertexNormal , ase_objectlightDir );
			float dotResult9 = dot( ase_vertexNormal , ase_objectlightDir );
			float2 temp_cast_0 = (( -0.3 * ( -1.2 - dotResult9 ) )).xx;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV19 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode19 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV19, 5.0 ) );
			c.rgb = ( ( step( _Reflexion , dotResult4 ) + ( _Color0 * tex2D( _TextureSample0, temp_cast_0 ) ) ) * ( 1.0 - step( 0.2697634 , fresnelNode19 ) ) ).rgb;
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
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

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
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
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
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.CommentaryNode;97;-255.8845,185.4464;Inherit;False;1172.245;572.6565;Centralisation;5;55;28;63;89;100;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;53;-1567.381,774.7947;Inherit;False;1142.107;452.0195;Outline;5;19;20;39;44;42;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;23;-1460.345,-418.9056;Inherit;False;921.1976;493.8;Specular;5;4;7;8;46;47;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;22;-1838.818,174.2728;Inherit;False;1536.368;561.2238;Diffuse;9;9;35;45;67;78;77;93;73;76;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;9;-1464.82,344.2728;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ObjSpaceLightDirHlpNode;45;-1810.821,499.4418;Inherit;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalVertexDataNode;35;-1796.791,271.52;Inherit;True;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-845.4412,349.7123;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-1092.444,233.9229;Inherit;False;Constant;_Float4;Float 2;3;0;Create;True;0;0;0;False;0;False;-0.3;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1014.929,263.4368;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;ToonShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.StickyNoteNode;90;-719.6485,-444.9609;Inherit;False;493.2001;264;Explication;;1,1,1,1;Le node Dot créé un produit scalaire entre un vecteur de direction de la lumière normalisé en coordonnées spaciale de l'objet et entre le vecteur de la position des vertices de l'objet en local. Cela me donne alors une réprésentation de la lumiere par rapport a un objet de maniere locale.$$J'ai ensuite créé un step afin de transmer ces valeurs en 0 et en 1 pour avoir un résultat plus tranché.$J'ai utiliser un float afin de mesurer la hauteur de la séparation du Step.$Ces nodes étant pour créée un effet de réflexion de la lumiere en highlight, ma séparation est donc tres haute, laissant uniquement un cercle au dessus.$;0;0
Node;AmplifyShaderEditor.NormalVertexDataNode;46;-1447.469,-376.5765;Inherit;True;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ObjSpaceLightDirHlpNode;47;-1443.101,-144.6552;Inherit;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StepOpNode;7;-940.7414,-252.1051;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1189.629,-369.7056;Inherit;False;Property;_Reflexion;Reflexion;0;0;Create;True;0;0;0;False;0;False;0.97;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;4;-1163.145,-238.5051;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StickyNoteNode;93;-1573.148,578.3558;Inherit;False;1253.463;149.0084;Explication;;1,1,1,1;Le node Dot créé un produit scalaire entre un vecteur de direction de la lumière normalisé en coordonnées spaciale de l'objet et entre le vecteur de la position des vertices de l'objet en local. Cela me donne alors une réprésentation de la lumiere par rapport a un objet de maniere locale.$J'ai créé un Node Subtract connecter a un float qui me permet de soustraire par une valeur négative le resultat du node Dot ce qui me donne un résulat négative que je vais venir multiplier par une autre valeur négative, ce qui va me donner un résultat positif, donc du blanc, afin d'obtenir un dégradé de lumiere diffu, que je vais ensuite connecter à un Texture Sample dans lequel se trouve ma Ramp de dégradé.;0;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-1365.703,231.5786;Inherit;False;Constant;_Float3;Float 2;3;0;Create;True;0;0;0;False;0;False;-1.2;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;76;-1175.196,347.9155;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;67;-586.4461,365.3815;Inherit;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;0;False;0;False;-1;8820d633e8b9c94469ea0103767271c8;51e804cbb26f2774aa0409815a2e98aa;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StickyNoteNode;96;-1551.554,1182.205;Inherit;False;1113.2;129.6;Explication;;1,1,1,1;J'ai utiliser l'effect de Fresnel qui permet de calculer comment est réflechis la lumiere en fonction de l'angle dans lequel on regarde un objet.$J'ai ensuite placer un node Step afin de séparer ces valeurs en 0 et en 1 pour avoir un résultat plus tranché.$J'ai utiliser le node Subtract avec un float positif afin d'inverser les 0 et les 1 du Step pour avoir un Outline noir et un centre blanc, permettant de recevoir le reste de mon shader.;0;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-1499.782,833.5149;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;0.2697634;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;19;-1507.625,944.6756;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;20;-1098.942,927.3328;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;63;-205.8845,235.4464;Inherit;False;Property;_Color0;Color 0;1;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;62.51811,391.7811;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1055.418,833.5767;Inherit;False;Constant;_Float1;Float 0;5;0;Create;True;0;0;0;False;0;False;1;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;42;-680.3748,923.4994;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StickyNoteNode;100;-244.8673,603.2596;Inherit;False;926.0735;147.5215;Explication;;1,1,1,1;J'ai multiplier un node Color à mon node de texture qui finalise la partie Diffuse, afin de choisir la couleur global de mon shader.$J'ai ensuite connecter ce node multiply à un node Add me permettant d'ajouter le node Step du Spectular avec son reflet à mon shader.$J'ai connecter ce node Add à un node Multiply auquel j'ai également connecter le node Subtract de mon Outline afin d'intégrer l'Outline à mon shader.$J'ai enfin connecter ce dernier node à mon parametre de Custom Lighting pour que le shader affecte l'éclairage de l'objet sur lequel il se trouve.;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;55;365.6385,392.8331;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;686.8109,518.9274;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
WireConnection;9;0;35;0
WireConnection;9;1;45;0
WireConnection;77;0;78;0
WireConnection;77;1;76;0
WireConnection;0;13;28;0
WireConnection;7;0;8;0
WireConnection;7;1;4;0
WireConnection;4;0;46;0
WireConnection;4;1;47;0
WireConnection;76;0;73;0
WireConnection;76;1;9;0
WireConnection;67;1;77;0
WireConnection;20;0;39;0
WireConnection;20;1;19;0
WireConnection;89;0;63;0
WireConnection;89;1;67;0
WireConnection;42;0;44;0
WireConnection;42;1;20;0
WireConnection;55;0;7;0
WireConnection;55;1;89;0
WireConnection;28;0;55;0
WireConnection;28;1;42;0
ASEEND*/
//CHKSM=1EF007E1D3113C4DC046AE734F1B1C0C6EFCA0DC
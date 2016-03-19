Shader "Custom/FastLineRendererShader"
{
	Properties
	{
		_MainTex ("Line Texture (RGB) Alpha (A)", 2D) = "white" {}
		_MainTexStartCap ("Line Texture Start Cap (RGB) Alpha (A)", 2D) = "white" {}
		_MainTexEndCap ("Line Texture End Cap (RGB) Alpha (A)", 2D) = "white" {}
		_MainTexRoundJoin ("Line Texture Round Join (RGB) Alpha (A)", 2D) = "white" {}
		_GlowTex ("Glow Texture (RGB) Alpha (A)", 2D) = "white" {}
		_GlowTexStartCap ("Glow Texture Start Cap (RGB) Alpha (A)", 2D) = "white" {}
		_GlowTexEndCap ("Glow Texture End Cap (RGB) Alpha (A)", 2D) = "white" {}
		_GlowTexRoundJoin ("Glow Texture Round Join (RGB) Alpha (A)", 2D) = "transparent" {}

		_TintColor ("Tint Color (RGB)", Color) = (1, 1, 1, 1)
		_GlowTintColor ("Glow Tint Color (RGB)", Color) = (1, 1, 1, 1)
		_GlowIntensityMultiplier ("Glow Intensity (Float)", Float) = 1.0
		_GlowWidthMultiplier("Glow Width Multiplier (Float)", Float) = 1.0
		_GlowLengthMultiplier ("Glow Length Multiplier (Float)", Float) = 0.33
		_InvFade ("Soft Particles Factor", Range(0.01, 3.0)) = 1.0
		_JitterMultiplier ("Jitter Multiplier (Float)", Float) = 0.0
		_Turbulence ("Turbulence (Float)", Float) = 0.0
		_UVXScale("UV X Scale (Float)", Float) = 1.0
		_UVYScale("UV Y Scale (Float)", Float) = 1.0
		_UVXScaleGlow("Glow UV X Scale (Float)", Float) = 1.0
		_UVYScaleGlow("Glow UV Y Scale (Float)", Float) = 1.0

		[HideInInspector]
		_Time2("Internal use only", Vector) = (0, 0, 0, 0)
    }

    SubShader
	{
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent+10" "LightMode"="Always" "PreviewType"="Plane"}
		Cull Off
		Lighting Off
		ZWrite Off
		ColorMask RGBA

		CGINCLUDE
		
		#include "UnityCG.cginc"

		#pragma vertex vert
        #pragma fragment frag
		#pragma multi_compile_particles // for soft particles
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma glsl_no_auto_normalization
		#pragma multi_compile __ DISABLE_CAPS

		float _JitterMultiplier;
		float _Turbulence;
		float4 _Time2;

#if defined(SOFTPARTICLES_ON)

		float _InvFade;
		sampler2D _CameraDepthTexture;

#endif

		struct appdata_t
		{
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			fixed4 dir : TANGENT;
			fixed3 velocity : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 lifeTime : TEXCOORD1;
		};

        struct v2f
        {
            fixed2 texcoord : TEXCOORD0;

#if !defined(DISABLE_CAPS)

			fixed4 texcoord2 : TEXCOORD1;

#endif

            fixed4 color : COLOR0;
            float4 pos : SV_POSITION;

#if defined(SOFTPARTICLES_ON)
            
			float4 projPos : TEXCOORD2;
            
#endif

        };

		inline float rand(float4 pos)
		{
			return frac(sin(dot(_Time2.yzw * pos.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
		}

		inline fixed4 lerpFade(float4 f, float t)
		{
			// the vertex will fade in, stay at full color, then fade out
			// x = creation time seconds
			// y = fade time in seconds
			// z = life time seconds

			// debug
			// return fixed4(1, 1, 1, 1);

			float peakFadeIn = f.x + f.y;
			if (t < peakFadeIn)
			{
				return lerp(fixed4(0, 0, 0, 0), fixed4(1, 1, 1, 1), max(0, ((t - f.x) / (peakFadeIn - f.x))));
			}
			float endLifetime = (f.x + f.z);
			float startFadeOut = endLifetime - f.y;

			// will be negative until fade out time (startFadeOut) is passed which will keep it at full color
			return lerp(fixed4(1, 1, 1, 1), fixed4(0, 0, 0, 0), max(0, ((t - startFadeOut) / (endLifetime - startFadeOut))));
		}

		inline float3 rotate_vertex_position(float3 position, float3 origin, float3 axis, float angle)
		{
			float half_angle = angle * 0.5;
			float4 q = float4(axis.xyz * sin(half_angle), cos(half_angle));
			position -= origin;
			return position + origin + (2.0 * cross(cross(position, q.xyz) + q.w * position, q.xyz));
		}

		inline v2f computeVertexOutput(appdata_t v, float uvxScale, float uvyScale, fixed4 color, fixed4 tintColor, float radiusMultiplier, float lengthMultiplier)
		{
			v2f o;

			float4 worldPos = v.vertex;
			uint lineType = abs(v.texcoord.x);
			float2 texcoord = float2(lineType % 2, v.texcoord.y);
			float dirModifier = (texcoord.x * 2) - 1;
			float jitter = 1.0 + (_JitterMultiplier * rand(worldPos));
			float absRadius = abs(v.dir.w);
			float elapsedSeconds = _Time2.y - v.lifeTime.x;
			float3 dirNorm = normalize(v.dir.xyz);
			float4 velocityOffset = float4((v.velocity + (dirNorm * _Turbulence)) * elapsedSeconds, 0);
			float3 center = worldPos.xyz + (v.dir.xyz * 0.5 * -dirModifier);
			float4 pos;

			if (unity_OrthoParams.w == 1.0)
			{
				float2 tangent = normalize(float2(-v.dir.y, v.dir.x));
				float4 offset = float4((tangent * v.dir.w * radiusMultiplier) + (lengthMultiplier * dirNorm.xy * dirModifier * radiusMultiplier), 0, 0);
				if (v.lifeTime.w != 0.0)
				{
					float modifier = v.lifeTime.w * elapsedSeconds;
					offset.xy = rotate_vertex_position(offset.xyz, float3(0, 0, 0), float3(0, 0, 1), modifier).xy;
					worldPos.xy = rotate_vertex_position(worldPos.xyz, center, float3(0, 0, 1), modifier).xy;
				}
				pos = worldPos + (offset * jitter) + velocityOffset;
			}
			else
			{
				float3 directionToCamera = normalize(_WorldSpaceCameraPos - center);
				float3 tangent = cross(dirNorm.xyz, directionToCamera.xyz);
				float4 offset = float4((tangent * v.dir.w * radiusMultiplier) + (lengthMultiplier * dirNorm * dirModifier * radiusMultiplier), 0);
				if (v.lifeTime.w != 0.0)
				{
					// to rotate around the lines perpendicular, use tangent instead of directionToCamera for the axis
					float modifier = v.lifeTime.w * elapsedSeconds;
					offset.xyz = rotate_vertex_position(offset.xyz, float3(0, 0, 0), directionToCamera, v.lifeTime.w * modifier);
					worldPos.xyz = rotate_vertex_position(worldPos.xyz, center, directionToCamera, v.lifeTime.w * modifier);
				}
				pos = worldPos + (offset * jitter) + velocityOffset;
			}

			o.pos = mul(UNITY_MATRIX_MVP, pos);
			o.color = (lerpFade(v.lifeTime, _Time2.y) * color * tintColor);
			o.texcoord = fixed2(texcoord.x * uvxScale, texcoord.y * uvyScale);

#if !defined(DISABLE_CAPS)

			{
				// check each bit of the 4 possible line type bits

				// full line
				lineType /= 2;
				o.texcoord2.x = lineType % 2;

				// start cap
				lineType /= 2;
				o.texcoord2.y = lineType % 2;

				// end cap
				lineType /= 2;
				o.texcoord2.z = lineType % 2;

				// round join
				lineType /= 2;
				o.texcoord2.w = lineType % 2;
			}

#endif

#if defined(SOFTPARTICLES_ON)

			o.projPos = ComputeScreenPos(o.pos);
			COMPUTE_EYEDEPTH(o.projPos.z);

#endif

			return o;
		}

		inline fixed4 computeFragmentOutput(v2f i, sampler2D texMain

#if !defined(DISABLE_CAPS)
			, sampler2D texStartCap, sampler2D texEndCap, sampler2D texRoundJoin

#endif

		)

		{

#if defined(SOFTPARTICLES_ON)

			float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
			float partZ = i.projPos.z;
			i.color.a *= saturate(_InvFade * (sceneZ - partZ));

#endif

			//i.texcoord.x += (_Time2.y * 2.0);

#if !defined(DISABLE_CAPS)

			// was using an if starement and returning a different sampler, but this had glitches in 3D mode
			// once texture arrays are supported, use those on supported targets
			return
			(
				(i.texcoord2.x * tex2D(texMain, i.texcoord)) +
				(i.texcoord2.y * tex2D(texStartCap, i.texcoord)) +
				(i.texcoord2.z * tex2D(texEndCap, i.texcoord)) +
				(i.texcoord2.w * tex2D(texRoundJoin, i.texcoord))
			) * i.color;

#else

			return tex2D(texMain, i.texcoord) * i.color;

#endif

		}

		ENDCG

		// glow pass
		Pass
		{
			Name "GlowPass"
			LOD 400
			Blend SrcAlpha One

            CGPROGRAM

			float _UVXScaleGlow;
			float _UVYScaleGlow;
			fixed4 _GlowTintColor;
			float _GlowIntensityMultiplier;
			float _GlowWidthMultiplier;
			float _GlowLengthMultiplier;
 			sampler2D _GlowTex;
			float4 _GlowTex_ST;
			float4 _GlowTex_TexelSize;
			sampler2D _GlowTexStartCap;
			float4 _GlowTexStartCap_ST;
			float4 _GlowTexStartCap_TexelSize;
			sampler2D _GlowTexEndCap;
			float4 _GlowTexEndCap_ST;
			float4 _GlowTexEndCap_TexelSize;
			sampler2D _GlowTexRoundJoin;
			float4 _GlowTexRoundJoin_ST;
			float4 _GlowTexRoundJoin_TexelSize;

            v2f vert(appdata_t v)
            {
				v.dir.w *= _GlowWidthMultiplier;
				return computeVertexOutput(v, _UVXScaleGlow, _UVYScaleGlow, fixed4(1, 1, 1, v.texcoord.w * _GlowIntensityMultiplier), _GlowTintColor, v.texcoord.z, _GlowLengthMultiplier);
            }
			
            fixed4 frag(v2f i) : SV_Target
			{

#if !defined(DISABLE_CAPS)

				return computeFragmentOutput(i, _GlowTex, _GlowTexStartCap, _GlowTexEndCap, _GlowTexRoundJoin);

#else

				return computeFragmentOutput(i, _GlowTex);

#endif

            }

            ENDCG
        }

		// line pass
		Pass
		{
			Name "LinePass"
			LOD 100
			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

			fixed4 _TintColor; 
			float _UVXScale;
			float _UVYScale;
 			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			sampler2D _MainTexStartCap;
			float4 _MainTexStartCap_ST;
			float4 _MainTexStartCap_TexelSize;
			sampler2D _MainTexEndCap;
			float4 _MainTexEndCap_ST;
			float4 _MainTexEndCap_TexelSize;
			sampler2D _MainTexRoundJoin;
			float4 _MainTexRoundJoinCap_ST;
			float4 _MainTexRoundJoin_TexelSize;

            v2f vert(appdata_t v)
            {
				return computeVertexOutput(v, _UVXScale, _UVYScale, v.color, _TintColor, 1, 0);
            }
			
            fixed4 frag(v2f i) : SV_Target
			{

#if !defined(DISABLE_CAPS)

				return computeFragmentOutput(i, _MainTex, _MainTexStartCap, _MainTexEndCap, _MainTexRoundJoin);

#else

				return computeFragmentOutput(i, _MainTex);

#endif

            }

            ENDCG
        }
    }
}
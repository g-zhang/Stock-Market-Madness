Shader "Custom/FastLineRendererShaderNoGlow"
{
	Properties
	{
		_MainTex("Line Texture (RGB) Alpha (A)", 2D) = "white" {}
		_MainTexStartCap("Line Texture Start Cap (RGB) Alpha (A)", 2D) = "white" {}
		_MainTexEndCap("Line Texture End Cap (RGB) Alpha (A)", 2D) = "white" {}
		_MainTexRoundJoin("Line Texture Round Join (RGB) Alpha (A)", 2D) = "white" {}

		_TintColor("Tint Color (RGB)", Color) = (1, 1, 1, 1)
		_InvFade("Soft Particles Factor", Range(0.01, 3.0)) = 1.0
		_JitterMultiplier("Jitter Multiplier (Float)", Float) = 0.0
		_Turbulence("Turbulence (Float)", Float) = 0.0
		_UVXScale("UV X Scale (Float)", Float) = 1.0
		_UVYScale("UV Y Scale (Float)", Float) = 1.0

		[HideInInspector]
		_ElapsedSecondsOverride("Internal use only", Float) = 999999.0
    }

    SubShader
	{
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent+10" "LightMode"="Always" "PreviewType"="Plane"}
		UsePass "Custom/FastLineRendererShader/LINEPASS"
    }
}
Shader "Passthrough/PassedthroughShader"
{

Properties
{
	_Stencil("StencilID", Int) = 22
	_StencilComp("StencilComp", Int) = 3
	_StencilOp("StencilOp", Int) = 0
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader
{
	Tags { "RenderType"="Opaque" "Queue"="Geometry" }
	LOD 200
	Cull Back

	Stencil
	{
		Ref [_Stencil]
		Comp [_StencilComp]
		Pass [_StencilOp]
	}

	CGPROGRAM
	#pragma surface surf Lambert

	sampler2D _MainTex;
	fixed4 _Color;

	struct Input
	{
		float2 uv_MainTex;
	};

	void surf (Input IN, inout SurfaceOutput o)
	{
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	
	ENDCG
}

Fallback "VertexLit"
}
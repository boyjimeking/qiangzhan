Shader "Fantasy/ui/weapon" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (1, 1, 1, 0)
	_Shininess ("Shininess", Range (0.01, 1)) = 1
	_Strengthen ("Strengthen", Range (1, 10)) = 4
	_MainTex ("Base (RGB) TransGloss (A)", 2D) = "white" {}
	_HightLightTex ("Hightlightmap", 2D) = "bump" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 400
	
CGPROGRAM
#pragma surface surf BlinnPhong
#pragma exclude_renderers flash

sampler2D _MainTex;
sampler2D _HightLightTex;
fixed4 _Color;
half _Shininess;
half _Strengthen;

struct Input {
	float2 uv_MainTex;
	float2 uv_HightLightTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 s = tex2D(_HightLightTex, IN.uv_HightLightTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = s.r*_Strengthen;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
}
ENDCG
}

FallBack "Transparent/Cutout/VertexLit"
}

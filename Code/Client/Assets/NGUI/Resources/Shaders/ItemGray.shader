Shader "Custom/ItmeCantUseShader"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
		_Color("white Color", Color) = (1,1,1,1)
	}
	
	SubShader
	{
		LOD 100
		Cull off 
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
     	
     	CGPROGRAM
	      #pragma surface surf Lambert finalcolor:mycolor alpha
	      struct Input {
	          float2 uv_MainTex;
	      };
	       void mycolor (Input IN, SurfaceOutput o, inout fixed4 color)
	      {
	            color.rgb =  0.299 * color.r*3 +  0.587 * color.g*3 +  0.114 * color.b*3;
	      }
	      sampler2D _MainTex;
	      void surf (Input IN, inout SurfaceOutput o) {
	           o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
	           
	           o.Alpha = tex2D (_MainTex, IN.uv_MainTex).a;
	      }
	      ENDCG
	}
}
Shader "Unlit/Transparent Colored Mask"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_MaskTex ("MaskTex (RGB) (A)", 2D) = "white" {}
		_MaskOperation ("Offset Scale", Vector) = (0, 0,1,1)
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
				
			#include "UnityCG.cginc"
	
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			sampler2D _MainTex;
			sampler2D _MaskTex;
			float4 _MaskOperation;
			float4 _MainTex_ST;
				
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord.xy = v.texcoord;
				o.texcoord.zw = o.vertex.zw;
				o.color = v.color;
				return o;
			}
				
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col;
				if(i.color.r<0.001 && i.color.g>0.999 && i.color.b>0.999)
				{
					col=tex2D(_MainTex, i.texcoord.xy);
					float grey=dot(col.rgb, float3(0.2, 0.2, 0.2));
					col.rgb = float3(grey, grey, grey);
				}else
				{
					col = tex2D(_MainTex, i.texcoord.xy) * i.color;
				}
				float4 projCoord;
				projCoord.xy = i.texcoord.xy * _MaskOperation.zw+_MaskOperation.xy;
				projCoord.zw = i.texcoord.zw;
				col.a *= 1-tex2Dproj(_MaskTex,projCoord);
				return col;
			}
			ENDCG
		}
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			ColorMask RGB
			AlphaTest Greater .01
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}

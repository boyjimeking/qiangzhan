
Shader "qmqz/ScreenMask" 
{
    Properties 
    {
        _Color ("Color", Color) = (0,0,0,0.6)
    }
    SubShader 
    {
        Tags 
        {
            "Queue"="Overlay"
            "RenderType"="Overlay"
        }
        LOD 200
        Pass 
        {
            Name "ForwardBase"
            Tags
            {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Always
            ZWrite Off
            Cull Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            uniform float4 _LightColor0;
            uniform float4 _Color;
            
            struct VertexInput
            {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float3 normal : NORMAL;
            };
            struct VertexOutput
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v)
            {
            //屏幕空间，不在做坐标变换直接映射到(-1,1)
                VertexOutput o;
                o.pos = v.vertex;
                o.uv.xy = v.texcoord0;
                o.uv.zw = v.vertex.zw;
                o.uv.y = 1-o.uv.y;
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR
            {
                float3 finalColor = 0;
                finalColor = _Color.rgb;
                return fixed4(finalColor,_Color.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

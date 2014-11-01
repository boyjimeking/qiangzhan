
Shader "qmqz/ScreenCoordinate" 
{
    Properties 
    {
        _Color ("Color", Color) = (0,0,0,0.6)
        _MainTex ("MainTex", 2D) = "white"{}
    }
    SubShader 
    {
        Tags 
        {
            "Queue"="Background"
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
            ZWrite off
            Cull Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            uniform float4 _Color;
            sampler2D _MainTex;
            
            struct VertexInput
            {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float3 normal : NORMAL;
            };
            struct VertexOutput
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v)
            {
            //屏幕空间，不在做坐标变换直接映射到(-1,1)
                VertexOutput o;
                o.pos = v.vertex;
                o.uv = v.texcoord0;
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR
            {                
               	fixed4 tex = tex2D(_MainTex,i.uv);

                return fixed4(tex.rgb,tex.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

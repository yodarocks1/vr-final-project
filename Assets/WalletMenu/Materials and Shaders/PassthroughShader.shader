Shader "Passthrough/PassthroughShader"
{
    Properties
    {
        _Stencil("Stencil", Int) = 22
        _Tint("Tint", Color) = ( 0, 0, 0, 0.6 )
        _TintStrength("Tint Strength", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" }
        ColorMask 0
        ZWrite off
        
        Stencil
        {
            Ref [_Stencil]
            Comp always
            Pass replace
        }
        
        Pass
        {
            Cull Back
            ZTest Less
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            struct appdata
            {
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float4 pos : SV_POSITION;
            };
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            half4 frag(v2f i) : COLOR
            {
                return half4(1,1,0,1);
            }
            
            ENDCG
        }
    }
}
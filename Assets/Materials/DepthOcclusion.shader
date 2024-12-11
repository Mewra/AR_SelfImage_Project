Shader "Custom/DepthOcclusion"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _DepthTex ("Depth Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _DepthTex;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float objectDepth = tex2D(_DepthTex, i.uv).r;

                // Se il pixel è più lontano della profondità dell'ambiente, lo scartiamo
                if (objectDepth > 0.5) // Cambia la soglia secondo necessità
                {
                    discard;
                }

                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
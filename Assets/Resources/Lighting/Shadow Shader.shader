Shader "Custom/ShadowSprite" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _ShadowTex("Shadow Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _ShadowColor("Shadow Color", Color) = (0,0,0,0.5)
        _ShadowOffset("Shadow Offset", Range(0, 1)) = 0.1
    }

        SubShader{
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

            Pass {
                Cull Off
                ZWrite Off
                Blend SrcAlpha OneMinusSrcAlpha
                ColorMask RGB
                Offset - 1, -1

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float4 shadowVertex : TEXCOORD1;
                };

                float _ShadowOffset;
                sampler2D _ShadowTex;
                float4 _ShadowColor;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;

                    // Calculate shadow vertex
                    float4 shadowVertex = o.vertex;
                    shadowVertex.x -= _ShadowOffset * (1 - tex2D(_ShadowTex, o.uv).a);
                    shadowVertex.y -= _ShadowOffset * (1 - tex2D(_ShadowTex, o.uv).a);
                    o.shadowVertex = shadowVertex;

                    return o;
                }

                sampler2D _MainTex;
                float4 _Color;

                fixed4 frag(v2f i) : SV_Target {
                    // Sample texture color
                    fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                // Add shadow color
                float shadow = tex2D(_ShadowTex, i.uv).a;
                col.rgb += _ShadowColor.rgb * _ShadowColor.a * shadow;
                col.a = _ShadowColor.a * shadow;

                return col;
            }
            ENDCG
        }
        }

            FallBack "Sprites/Default"
}

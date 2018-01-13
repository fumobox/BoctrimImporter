Shader "Custom/SimpleBoctShader" {

    Properties{
        _Texture("Texture", 2D) = "white" {}
       _DiffuseColor("Diffuse Color", Color) = (1.0, 1.0, 1.0)
    }

    SubShader {
        Pass {
            Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption APB_precision_hint_fastest
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            float4 _LightColor0;
            sampler2D _Texture;
            float4 _Texture_ST;
            struct vertout
            {
                float4 pos: SV_POSITION;
                float2 TexCoord: TEXCOORD0;
                float3 Normal: TEXCOORD2;
                float3 lightDir: TEXCOORD3;
                float3 viewDir: TEXCOORD4;
                fixed4 color : COLOR;
                LIGHTING_COORDS(5, 6)
            };

            vertout vert(appdata_full v) {
                vertout OUT;
                OUT.pos = UnityObjectToClipPos(v.vertex);
                OUT.TexCoord = TRANSFORM_TEX(v.texcoord, _Texture);
                OUT.Normal = normalize(v.normal).xyz;
                OUT.lightDir = normalize(ObjSpaceLightDir(v.vertex));
                OUT.viewDir = normalize(ObjSpaceViewDir(v.vertex));
                OUT.color = v.color;
                TRANSFER_VERTEX_TO_FRAGMENT(OUT);
                return OUT;
            }

            float3 _DiffuseColor;
            fixed4 frag(vertout In) : COLOR {
                float diffuse0 = max(0, dot(In.lightDir, In.Normal));
                float diffuse1 = max(0.2, dot(In.viewDir, In.Normal));
                float4 color = In.color * diffuse0 + (In.color + 0.5) * diffuse1 * 0.4;
                color.a = 1;
                return color;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}

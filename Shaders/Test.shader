Shader "Custom/Test"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Coordinate("Coordinate", Vector) = (0,0,0,0)
        _Color("Draw Color", Color) = (1,0,0,0)
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            LOD 100

            CGINCLUDE
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float4 _Coordinate;
            float4 _Color;

            float3 GetTriplanarBlend(float3 worldPos, float3 normal)
            {
                float3 blending = abs(normal);
                blending = normalize(max(blending, 0.0005)); // Force some blending to avoid artifacts
                blending /= (blending.x + blending.y + blending.z);
                return blending;
            }

            ENDCG

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                    float3 worldPos : TEXCOORD1;
                    float4 vertex : SV_POSITION;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.normal = UnityObjectToWorldNormal(v.normal);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float3 blending = GetTriplanarBlend(i.worldPos, i.normal);
                    float2 uvX = i.worldPos.zy;
                    float2 uvY = i.worldPos.xz;
                    float2 uvZ = i.worldPos.xy;

                    fixed4 col = tex2D(_MainTex, uvX) * blending.x + tex2D(_MainTex, uvY) * blending.y + tex2D(_MainTex, uvZ) * blending.z;
                    float draw = pow(saturate(1 - distance(i.uv, _Coordinate.xy)), 50);
                    fixed4 drawcol = _Color * (draw * 1);
                    return saturate(col + drawcol);
                }
                ENDCG
            }
        }
}

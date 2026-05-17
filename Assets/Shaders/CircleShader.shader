Shader "Unlit/CircleShader"
{
    Properties
    {
        _MainTex ("Render Texture", 2D) = "white" {}
        _Radius ("Radius", Range(0.1, 1)) = 0.8
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float _Radius;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Центрируем координаты (0-1 -> -0.5 до 0.5)
                float2 centered = i.uv - 0.5;
                
                // Расстояние от центра
                float dist = length(centered);
                
                // Берем цвет из Render Texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Если пиксель вне круга - делаем прозрачным
                if (dist > _Radius)
                {
                    col.a = 0;
                }
                
                return col;
            }
            ENDCG
        }
    }
}

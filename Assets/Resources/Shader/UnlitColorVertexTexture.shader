Shader "Neverpants/UnlitColorTextureTransparent"
{
	Properties
	{
		_MainTex("MainTex (Alpha)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Cutoff("Cutoff", Range(0, 1)) = 1
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True"  "ForceNoShadowCasting" = "True"  "LightMode" = "Always" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off ZWrite On

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)

			uniform half4 _Color;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _Cutoff;

			struct appdata_t
			{
				float4 vertex : POSITION;
                float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : POSITION;
                fixed4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				half4 col = tex2D(_MainTex, i.texcoord) * i.color;

				if (i.texcoord.x > _Cutoff)
                    col.a = 0;

				return col;
			}
			ENDCG
		}
	}
}



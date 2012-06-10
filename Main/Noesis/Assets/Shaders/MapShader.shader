Shader "Custom/MapShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ProvinceMap ("Province Map (RGB)", 2D) = "black" {}
		_ProvincePalatte ("Province Palatte (RGB)", 2D) = "black" {}
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		
		Pass 
		{
			Lighting Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _ProvinceMap;
			sampler2D _ProvincePalatte;

			struct v2f 
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};

			float4 _MainTex_ST;

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				return o;
			}

			half4 frag (v2f i) : COLOR
			{
				float4 texcol = tex2D (_MainTex, i.uv);
				float2 palatteuv = tex2D (_ProvinceMap, i.uv).rg;
				float4 palcol = tex2D (_ProvincePalatte, palatteuv);
				return half4 (texcol.rgb * (1 - palcol.a) + palcol.rgb * palcol.a, 1);
			}
			ENDCG

		} 
	}
	FallBack "Unlit/Texture"
}

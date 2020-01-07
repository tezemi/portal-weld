Shader "Portal Weld/Default" 
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Rotation("Rotation", Float) = 0.0
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;

		struct Input 
		{
			float2 uv_MainTex;
		};

		float _Rotation;
		void vert(inout appdata_full v) 
		{
			float sinX = sin(_Rotation);
			float cosX = cos(_Rotation);
			float sinY = sin(_Rotation);
			float2x2 rotationMatrix = float2x2(cosX, -sinX, sinY, cosX);
			v.texcoord.xy = mul(v.texcoord.xy, rotationMatrix);
		}

		void surf(Input IN, inout SurfaceOutput o) 
		{			
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}

	FallBack "Diffuse"
}
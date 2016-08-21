Shader "Unlit/BasicShader"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			// Colour info
			uniform float _maxHeight; 
			uniform float _snowHeight;
			uniform float _dirtHeight;
			uniform float _sandHeight;

			// Lighting info
			uniform float _Ia;	// Ambient intensity
			uniform float _Ka;	// Ambient albedo
			uniform float _Ip;	// Diffuse intensity
			uniform float _Kd;	// Diffuse albedo
			uniform float _C;	// Attenuation factor
			uniform float _n;	// Specularity
			uniform float _Ks;	// Specular albedo

			uniform float4 _camPosition;
			uniform float4 _sunPosition;

			struct vertIn
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float4 colour : COLOR;
				float2 distance : TEXCOORD0;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v)
			{
				vertOut o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				// Colour based on height
				float4 col;
				if (v.vertex.y > _maxHeight * _snowHeight) {
					// white
					col = float4(1, 1, 1, 1);
				}
				else if (v.vertex.y > _maxHeight * _dirtHeight) {
					// brown
					col = float4(0.57254, 0.38431, 0.22353, 1);
				}
				else if (v.vertex.y > _maxHeight * _sandHeight) {
					// green
					col = float4(0.09804, 0.54902, 0.09804, 1);
				}
				else {
					// yellow
					col = float4(0.96863, 0.9451, 0.7451, 1);
				}
				// Lighting
				float3 L = _sunPosition - v.vertex.xyz;
				float3 V = _camPosition - v.vertex.xyz;
				float3 H = L + V;
				float U = length(L);
				o.distance = float2(length(V), 0);

				float4 ambient = _Ia * _Ka * col;
				float4 diffuse = _Kd * col * max(dot(v.normal.xyz, L), 0);
				float4 specular = _Ks * pow(max(dot(v.normal.xyz, H), 0), _n) * col;

				diffuse *= _Ip / (_C + U);
				specular *= _Ip / (_C + U);

				col = ambient + diffuse + specular;
				col.w = 1;

				o.colour = col;
				return o;
			}

			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target
			{
				float4 col = v.colour;
				float4 fogCol = float4(0.2, 0.4, 1, 1);
				float fogDensity = 0.03;
				// water is located at -15
				if (_camPosition.y < -15) {
					//return float4(1 / (v.distance.x * 5), 0, 0, 1);
					//col *= fogCol;
					// calculate fog
					float fogFactor = clamp(1.0 / exp(v.distance.x * fogDensity), 0, 1);
					col = lerp(fogCol, col, fogFactor);
				}
				return col;
			}
			ENDCG
		}
	}
}

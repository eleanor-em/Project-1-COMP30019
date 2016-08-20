Shader "Unlit/BasicShader"
{
	Properties
	{
		_maxHeight ("Max Height", Float) = 1
		_snowHeight("Snow Height", Float) = 1
		_dirtHeight("Dirt Height", Float) = 1
		_sandHeight("Sand Height", Float) = 1
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float _maxHeight;
			uniform float _snowHeight;
			uniform float _dirtHeight;
			uniform float _sandHeight;

			struct vertIn
			{
				float4 vertex : POSITION;
			};
				struct vertOut
			{
				float4 vertex : SV_POSITION;
				float4 colour : COLOR;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v)
			{
				vertOut o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				// Colour based on height
				float4 col;
				if (v.vertex.y > _maxHeight * _snowHeight) {
					col = float4(1, 1, 1, 1);
				}
				else if (v.vertex.y > _maxHeight * _dirtHeight) {
					col = float4(0.57254, 0.38431, 0.22353, 1);
				}
				else if (v.vertex.y > _maxHeight * _sandHeight) {
					col = float4(0.09804, 0.54902, 0.09804, 1);
				}
				else {
					col = float4(0.96863, 0.9451, 0.7451, 1);
				}
				col *= 0.75  + 0.25 * v.vertex.y / _maxHeight;
				o.colour = col;
				return o;
			}

			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target
			{
				return v.colour;
			}
			ENDCG
		}
	}
}

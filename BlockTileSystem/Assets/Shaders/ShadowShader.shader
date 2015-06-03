Shader "Custom/CircleShader" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_Position1("Position1(Vector)",Vector)=(0,0,0,0)
		_Position2("Position2(Vector)",Vector)=(0,0,0,0)
		_Radius("Radius(Float)",Float)=0
		_Color ("Tint", Color) = (1.0, 0.6, 0.6, 1.0)
    }

    SubShader {
	ZTest Always // Always on top!
	Pass {
			Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
			uniform fixed4 _Color;
			uniform float2 _Position1;
			uniform float2 _Position2;
			uniform float _Radius;
			
            float4 frag(v2f_img i) : COLOR
			{
				float4 result = tex2D(_MainTex, i.uv);
				float dist1 = length(i.uv-_Position1);
				float dist2 = length(i.uv-_Position2);
				
				result.a = saturate(dist1/_Radius);
				result.a *= saturate(dist2/_Radius);


                return result*_Color;
			}
            ENDCG
        }
    }
}
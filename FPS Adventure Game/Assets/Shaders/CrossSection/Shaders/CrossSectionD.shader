// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

  Shader "CrossSection/Diffuse" {
    Properties {
      _Color ("Main Color", Color) = (1,1,1,1)
	  _MainTex("Albedo (RGB)", 2D) = "white" {}
	  
      _SectionColor ("Section Color", Color) = (1,0,0,1)
      _SectionPlane ("SectionPlane (x, y, z)", vector) = (0.707,0,-0.2)
      _SectionPoint ("SectionPoint (x, y, z)", vector) = (0,0,0)
      _SectionOffset ("SectionOffset", float) = 0
    }
    
    
    SubShader {
    	Tags { "RenderType"="Transparent" }
		LOD 200

		//  crossection pass (backfaces + fog)
	   Pass {
         Cull front // cull only front faces
         
         CGPROGRAM 
         
         #pragma vertex vert
         #pragma fragment frag
		 #pragma multi_compile_fog
		 #include "UnityCG.cginc"

 		 fixed4 _SectionColor;
 		 float3 _SectionPlane;
	     float3 _SectionPoint;
	     float _SectionOffset;
  		 
         struct vertexInput {
            float4 vertex : POSITION;			
         };

		 struct fragmentInput{
                float4 pos : SV_POSITION;
				float3 wpos : TEXCOORD0;
                UNITY_FOG_COORDS(1)
         };

		 fragmentInput vert(vertexInput i){
                fragmentInput o;
                o.pos = UnityObjectToClipPos (i.vertex);
                o.wpos = mul (unity_ObjectToWorld, i.vertex).xyz;

                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
         }

         fixed4 frag(fragmentInput i) : SV_Target {
				if(_SectionOffset -dot((i.wpos - _SectionPoint),_SectionPlane) < 0) discard;
				if( _SectionColor.a <0.5f) discard;
                fixed4 color = _SectionColor;
                UNITY_APPLY_FOG(i.fogCoord, color); 
                return color;
         }

         ENDCG  
         
      }  
	     
	      
	      CGPROGRAM
	      #pragma surface surf Lambert addshadow
	      #pragma debug
	      fixed4 _Color;
	      float3 _SectionPlane;
	      float3 _SectionPoint;
	      float _SectionOffset;
	      
	      struct Input {
	          float2 uv_MainTex;
	          float3 worldPos;
	      };
	      sampler2D _MainTex;
	      
	      void surf (Input IN, inout SurfaceOutput o) {
	          clip (_SectionOffset -dot((IN.worldPos - _SectionPoint),_SectionPlane));
	          fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	          o.Albedo = tex.rgb * _Color.rgb;

	          o.Alpha = tex.a * _Color.a;
	      }
	      ENDCG

    } 

    Fallback "Diffuse"
  }
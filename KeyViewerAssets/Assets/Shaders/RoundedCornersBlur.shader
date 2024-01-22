Shader "UI/RoundedCorners/RoundedCorners" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        // --- Mask support ---
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask_RC ("Color Mask Rounded Corner", Float) = 15
        [HideInInspector] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        
        [HideInInspector]
        _ColorMask ("Color Mask Blur", Color) = (1, 1, 1, 0.2)
        _TintColor ("Tint Color", Color) = (1, 1, 1, 0.2)
        _Size ("Spacing", Range(0, 40)) = 5.0
        _Vibrancy ("Vibrancy", Range(0, 2)) = 0.2
        
        // Definition in Properties section is required to Mask works properly
        _WidthHeightRadius ("WidthHeightRadius", Vector) = (0,0,0,0)
        // ---
    }
    
    SubShader {
        Tags {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        // --- Mask support ---
        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Cull Off
        Lighting Off
        ZTest [unity_GUIZTestMode]
        ColorMask [_ColorMask_RC]
        // ---
        
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZWrite Off

        GrabPass {
        //"_GrabTexture" // Uncomment to speed-up, see documentation.
        Tags { "LightMode" = "Always" }
        }
      
      // Vertical blur
      Pass {
        Name "VERTICAL"
        Tags { "LightMode" = "Always" }

        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma fragmentoption ARB_precision_hint_fastest
        #include "UnityCG.cginc"
         
        struct appdata_t {
          float4 vertex : POSITION;
          float2 texcoord: TEXCOORD0;
        };
         
        struct v2f {
          float4 vertex : POSITION;
          float4 uvgrab : TEXCOORD0;
          float2 uv : TEXCOORD1;
        };

        float4 _MainTex_ST;
         
        v2f vert (appdata_t v) {
          v2f o;
          o.vertex = UnityObjectToClipPos(v.vertex);
          #if UNITY_UV_STARTS_AT_TOP
          float scale = -1.0;
          #else
          float scale = 1.0;
          #endif
          o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
          o.uvgrab.zw = o.vertex.zw;
          o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
          return o;
        }
         
        sampler2D _GrabTexture;
        float4 _GrabTexture_TexelSize;
        float _Size;
        sampler2D _MainTex;
         
        half4 frag( v2f i ) : COLOR {
          half4 sum = half4(0,0,0,0);
 
          #define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely * _Size * 1.61, i.uvgrab.z, i.uvgrab.w))) * weight
 
          sum += GRABPIXEL(0.05, -4.0);
          sum += GRABPIXEL(0.09, -3.0);
          sum += GRABPIXEL(0.12, -2.0);
          sum += GRABPIXEL(0.15, -1.0);
          sum += GRABPIXEL(0.18,  0.0);
          sum += GRABPIXEL(0.15, +1.0);
          sum += GRABPIXEL(0.12, +2.0);
          sum += GRABPIXEL(0.09, +3.0);
          sum += GRABPIXEL(0.05, +4.0);

          half4 texcol = tex2D(_MainTex, i.uv);
          sum.a = texcol.a;
          return sum;
        }
        
        ENDCG
      }

      GrabPass {
        Tags { "LightMode" = "Always" }
      }

      // Horizontal blur
      Pass {
        Name "HORIZONTAL"
        Tags { "LightMode" = "Always" }

        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma fragmentoption ARB_precision_hint_fastest
        #include "UnityCG.cginc"

         
        struct appdata_t {
          float4 vertex : POSITION;
          float2 texcoord: TEXCOORD0;
        };
         
        struct v2f {
          float4 vertex : POSITION;
          float4 uvgrab : TEXCOORD0;
          float2 uv : TEXCOORD1;
        };

        float4 _MainTex_ST;
         
        v2f vert (appdata_t v) {
          v2f o;
          o.vertex = UnityObjectToClipPos(v.vertex);
          #if UNITY_UV_STARTS_AT_TOP
          float scale = -1.0;
          #else
          float scale = 1.0;
          #endif
          o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
          o.uvgrab.zw = o.vertex.zw;
          o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
          return o;
        }
         
        sampler2D _GrabTexture;
        float4 _GrabTexture_TexelSize;
        float _Size;
        sampler2D _MainTex;
         
        half4 frag( v2f i ) : COLOR {
          half4 sum = half4(0,0,0,0);
 
          #define GRABPIXEL(weight,kernelx) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx * _Size * 1.61, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
 
          sum += GRABPIXEL(0.05, -4.0);
          sum += GRABPIXEL(0.09, -3.0);
          sum += GRABPIXEL(0.12, -2.0);
          sum += GRABPIXEL(0.15, -1.0);
          sum += GRABPIXEL(0.18,  0.0);
          sum += GRABPIXEL(0.15, +1.0);
          sum += GRABPIXEL(0.12, +2.0);
          sum += GRABPIXEL(0.09, +3.0);
          sum += GRABPIXEL(0.05, +4.0);

          half4 texcol = tex2D(_MainTex, i.uv);
          sum.a = texcol.a;
          return sum;
        }
        
        ENDCG
      }
        
      GrabPass {
        Tags { "LightMode" = "Always" }
      }
      
      // Vertical blur
      Pass {
        Name "VERTICAL"
        Tags { "LightMode" = "Always" }

        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma fragmentoption ARB_precision_hint_fastest
        #include "UnityCG.cginc"
         
        struct appdata_t {
          float4 vertex : POSITION;
          float2 texcoord: TEXCOORD0;
        };
         
        struct v2f {
          float4 vertex : POSITION;
          float4 uvgrab : TEXCOORD0;
          float2 uv : TEXCOORD1;
        };

        float4 _MainTex_ST;
         
        v2f vert (appdata_t v) {
          v2f o;
          o.vertex = UnityObjectToClipPos(v.vertex);
          #if UNITY_UV_STARTS_AT_TOP
          float scale = -1.0;
          #else
          float scale = 1.0;
          #endif
          o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
          o.uvgrab.zw = o.vertex.zw;
          o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
          return o;
        }
         
        sampler2D _GrabTexture;
        float4 _GrabTexture_TexelSize;
        float _Size;
        sampler2D _MainTex;
         
        half4 frag( v2f i ) : COLOR {
          half4 sum = half4(0,0,0,0);
 
          #define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely * _Size, i.uvgrab.z, i.uvgrab.w))) * weight
 
          sum += GRABPIXEL(0.05, -4.0);
          sum += GRABPIXEL(0.09, -3.0);
          sum += GRABPIXEL(0.12, -2.0);
          sum += GRABPIXEL(0.15, -1.0);
          sum += GRABPIXEL(0.18,  0.0);
          sum += GRABPIXEL(0.15, +1.0);
          sum += GRABPIXEL(0.12, +2.0);
          sum += GRABPIXEL(0.09, +3.0);
          sum += GRABPIXEL(0.05, +4.0);

          half4 texcol = tex2D(_MainTex, i.uv);
          sum.a = texcol.a;
          return sum;
        }
        
        ENDCG
      }

      GrabPass {             
        Tags { "LightMode" = "Always" }
      }

      // Horizontal blur
      Pass {
        Name "HORIZONTAL"
        Tags { "LightMode" = "Always" }

        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma fragmentoption ARB_precision_hint_fastest
        #include "UnityCG.cginc"
         
        struct appdata_t {
          float4 vertex : POSITION;
          float2 texcoord: TEXCOORD0;
        };
         
        struct v2f {
          float4 vertex : POSITION;
          float4 uvgrab : TEXCOORD0;
          float2 uv : TEXCOORD1;
        };

        float4 _MainTex_ST;
         
        v2f vert (appdata_t v) {
          v2f o;
          o.vertex = UnityObjectToClipPos(v.vertex);
          #if UNITY_UV_STARTS_AT_TOP
          float scale = -1.0;
          #else
          float scale = 1.0;
          #endif
          o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
          o.uvgrab.zw = o.vertex.zw;
          o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
          return o;
        }
         
        sampler2D _GrabTexture;
        float4 _GrabTexture_TexelSize;
        float _Size;
        sampler2D _MainTex;
         
        half4 frag( v2f i ) : COLOR {
          half4 sum = half4(0,0,0,0);
 
          #define GRABPIXEL(weight,kernelx) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx * _Size, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
 
          sum += GRABPIXEL(0.05, -4.0);
          sum += GRABPIXEL(0.09, -3.0);
          sum += GRABPIXEL(0.12, -2.0);
          sum += GRABPIXEL(0.15, -1.0);
          sum += GRABPIXEL(0.18,  0.0);
          sum += GRABPIXEL(0.15, +1.0);
          sum += GRABPIXEL(0.12, +2.0);
          sum += GRABPIXEL(0.09, +3.0);
          sum += GRABPIXEL(0.05, +4.0);

          half4 texcol = tex2D(_MainTex, i.uv);
          sum.a = texcol.a;
          return sum;
        }
        
        ENDCG
      }
      GrabPass {
        Tags { "LightMode" = "Always" }
      }
      
      // Vertical blur
      Pass {
        Name "VERTICAL"
        Tags { "LightMode" = "Always" }

        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma fragmentoption ARB_precision_hint_fastest
        #include "UnityCG.cginc"
         
        struct appdata_t {
          float4 vertex : POSITION;
          float2 texcoord: TEXCOORD0;
        };
         
        struct v2f {
          float4 vertex : POSITION;
          float4 uvgrab : TEXCOORD0;
          float2 uv : TEXCOORD1;
        };

        float4 _MainTex_ST;
         
        v2f vert (appdata_t v) {
          v2f o;
          o.vertex = UnityObjectToClipPos(v.vertex);
          #if UNITY_UV_STARTS_AT_TOP
          float scale = -1.0;
          #else
          float scale = 1.0;
          #endif
          o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
          o.uvgrab.zw = o.vertex.zw;
          o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
          return o;
        }
         
        sampler2D _GrabTexture;
        float4 _GrabTexture_TexelSize;
        float _Size;
        sampler2D _MainTex;
         
        half4 frag( v2f i ) : COLOR {
          half4 sum = half4(0,0,0,0);
 
          #define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely * _Size * 0.2, i.uvgrab.z, i.uvgrab.w))) * weight
 
          sum += GRABPIXEL(0.05, -4.0);
          sum += GRABPIXEL(0.09, -3.0);
          sum += GRABPIXEL(0.12, -2.0);
          sum += GRABPIXEL(0.15, -1.0);
          sum += GRABPIXEL(0.18,  0.0);
          sum += GRABPIXEL(0.15, +1.0);
          sum += GRABPIXEL(0.12, +2.0);
          sum += GRABPIXEL(0.09, +3.0);
          sum += GRABPIXEL(0.05, +4.0);

          half4 texcol = tex2D(_MainTex, i.uv);
          sum.a = texcol.a;
          return sum;
        }
        
        ENDCG
      }

      GrabPass {             
        Tags { "LightMode" = "Always" }
      }

      // Horizontal blur
      Pass {
        Name "HORIZONTAL"
        Tags { "LightMode" = "Always" }

        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma fragmentoption ARB_precision_hint_fastest
        #include "UnityCG.cginc"
         
        struct appdata_t {
          float4 vertex : POSITION;
          float2 texcoord: TEXCOORD0;
        };
         
        struct v2f {
          float4 vertex : POSITION;
          float4 uvgrab : TEXCOORD0;
          float2 uv : TEXCOORD1;
        };

        float4 _MainTex_ST;
         
        v2f vert (appdata_t v) {
          v2f o;
          o.vertex = UnityObjectToClipPos(v.vertex);
          #if UNITY_UV_STARTS_AT_TOP
          float scale = -1.0;
          #else
          float scale = 1.0;
          #endif
          o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
          o.uvgrab.zw = o.vertex.zw;
          o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
          return o;
        }
         
        sampler2D _GrabTexture;
        float4 _GrabTexture_TexelSize;
        float _Size;
        sampler2D _MainTex;
         
        half4 frag( v2f i ) : COLOR {
          half4 sum = half4(0,0,0,0);
 
          #define GRABPIXEL(weight,kernelx) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx * _Size * 0.2, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
 
          sum += GRABPIXEL(0.05, -4.0);
          sum += GRABPIXEL(0.09, -3.0);
          sum += GRABPIXEL(0.12, -2.0);
          sum += GRABPIXEL(0.15, -1.0);
          sum += GRABPIXEL(0.18,  0.0);
          sum += GRABPIXEL(0.15, +1.0);
          sum += GRABPIXEL(0.12, +2.0);
          sum += GRABPIXEL(0.09, +3.0);
          sum += GRABPIXEL(0.05, +4.0);

          half4 texcol = tex2D(_MainTex, i.uv);
          sum.a = texcol.a;
           
          return sum;
        }
        
        ENDCG
      }

      // Distortion
      GrabPass {             
        Tags { "LightMode" = "Always" }
      }
      
      Pass {
        Tags { "LightMode" = "Always" }

        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        // Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members uv)
        // #pragma exclude_renderers d3d11 xbox360
        #pragma vertex vert
        #pragma fragment frag
        #pragma fragmentoption ARB_precision_hint_fastest
        #include "UnityCG.cginc"

        struct appdata_t {
          float4 vertex : POSITION;
          float2 texcoord: TEXCOORD0;
        };
         
        struct v2f {
          float4 vertex : POSITION;
          float4 uvgrab : TEXCOORD0;
          float2 uv : TEXCOORD1;
        };
         
        float4 _MainTex_ST;

        v2f vert (appdata_t v) {
          v2f o;
          o.vertex = UnityObjectToClipPos(v.vertex);
          #if UNITY_UV_STARTS_AT_TOP
          float scale = -1.0;
          #else
          float scale = 1.0;
          #endif
          o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
          o.uvgrab.zw = o.vertex.zw;
          o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
          return o;
        }
         
        half4 _TintColor;
        float _Vibrancy;
        sampler2D _GrabTexture;
        float4 _GrabTexture_TexelSize;
        sampler2D _MainTex;
        
        half4 frag( v2f i ) : COLOR {
          half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
          half4 texcol = tex2D(_MainTex, i.uv);
          col.r *= texcol.r;
          col.g *= texcol.g;
          col.b *= texcol.b;
          col.rgb *= 1 + _Vibrancy;
          col.a = texcol.a;
          col = lerp (col, _TintColor, _TintColor.w);
          return col;
        }
        
        ENDCG
      }

       GrabPass {
           "_MainTex" 
          Tags { "LightMode" = "Always" }

        }
        Pass {
            CGPROGRAM
            
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"          
            #include "SDFUtils.cginc"
            #include "ShaderSetup.cginc"
            
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            float4 _WidthHeightRadius;
            sampler2D _MainTex;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;


            struct appdata {
          float4 vertex : POSITION;
          float2 texcoord: TEXCOORD0;
          float4 color : COLOR;
        };
         
        struct v2f {
          float4 vertex : POSITION;
          float4 uvgrab : TEXCOORD0;
          float2 uv : TEXCOORD1;
          float4 color : COLOR;
        };
         

            v2f vert (appdata v) {
              v2f o;
              UNITY_SETUP_INSTANCE_ID(v);
              UNITY_INITIALIZE_OUTPUT(v2f, o);
              UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
              o.vertex = UnityObjectToClipPos(v.vertex);
              #if UNITY_UV_STARTS_AT_TOP
              float scale = -1.0;
              #else
              float scale = 1.0;
              #endif
              o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
              o.uvgrab.zw = o.vertex.zw;
              o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
              return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half4 color = (tex2D(_MainTex, i.uv) + _TextureSampleAdd) * i.color;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                if (color.a <= 0) {
                    return color;
                }

                float alpha = CalcAlpha(i.uv, _WidthHeightRadius.xy, _WidthHeightRadius.z);

                #ifdef UNITY_UI_ALPHACLIP
                clip(alpha - 0.001);
                #endif
                
                return mixAlpha(tex2D(_MainTex, i.uv), i.color, alpha);
            }
            
            ENDCG
        }
    }
}

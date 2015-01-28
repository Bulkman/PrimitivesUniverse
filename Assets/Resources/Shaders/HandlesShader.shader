Shader "Handels" {
	SubShader { 
		Pass {
 			Blend SrcAlpha OneMinusSrcAlpha
 			//ZTest Always
            ZWrite Off 
            Cull Off 
            Lighting Off
            Fog { Mode Off }
            BindChannels { Bind "vertex", vertex Bind "color", color }
        } 
	} 
}

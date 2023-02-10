// HSV to RBG from https://www.rapidtables.com/convert/color/hsv-to-rgb.html
// Rotation matrix from https://en.wikipedia.org/wiki/Rotation_matrix

shader_type canvas_item;

const float PI = 3.1415926535;

uniform float strength: hint_range(0., 1.) = 0.5;
uniform float speed: hint_range(0., 10.) = .5;
uniform float angle: hint_range(0., 360.) = 0.;

//void fragment() {
//	float time = cos(TIME * PI * speed);
//
//    // Calcula a coordenada UV para o pixel atual
//    vec2 uv = SCREEN_UV;
//
//    // Calcula a posição do centro da tela
//    vec3 from = vec3(1.0, uv.y, 0);
//
//    // Calcula a posição do pixel atual
//    vec3 to = vec3(uv.x, uv.y, 0);
//
//    // Calcula a direção do pixel atual em relação ao centro da tela
//    vec3 direction = normalize(to - from);
//
//    // Calcula a distância do pixel atual para o centro da tela
//    float d = length(to - from);
//
//    // Calcula a cor base do degradê
//    vec3 color = vec3(0.5 + 0.5 * cos(d * 6.0 + time), 0.5 + 0.5 * cos(d * 4.0 + time), 0.5 + 0.5 * cos(d * 8.0 + time));
//
//    // Adiciona as cores do arco-íris à cor base do degradê
//    color = mix(vec3(1.0, 0.0, 0.0), vec3(1.0, 1.0, 0.0), smoothstep(0.0, 0.2, d + time));
//    color = mix(color, vec3(0.0, 1.0, 0.0), smoothstep(0.2, 0.4, d + time));
//    color = mix(color, vec3(0.0, 1.0, 1.0), smoothstep(0.4, 0.6, d + time));
//    color = mix(color, vec3(0.0, 0.0, 1.0), smoothstep(0.6, 0.8, d + time));
//    color = mix(color, vec3(1.0, 0.0, 1.0), smoothstep(0.8, 1.0, d + time));
//
//    // Define a cor final do pixel atual
//	vec4 colorOriginal = texture(TEXTURE, UV);
//    COLOR = mix(colorOriginal, vec4(color, colorOriginal.a), strength);
//}

void fragment() {
	vec2 uv = vec2(SCREEN_UV.x, SCREEN_UV.y);
	float hue = uv.x * cos(radians(angle)) - uv.y * sin(radians(angle));
	hue = fract(hue + fract(TIME  * speed));
	float x = 1. - abs(mod(hue / (1./ 6.), 2.) - 1.);
	vec3 rainbow;
	if(hue < 1./6.){
		rainbow = vec3(1., x, 0.);
	} else if (hue < 1./3.) {
		rainbow = vec3(x, 1., 0);
	} else if (hue < 0.5) {
		rainbow = vec3(0, 1., x);
	} else if (hue < 2./3.) {
		rainbow = vec3(0., x, 1.);
	} else if (hue < 5./6.) {
		rainbow = vec3(x, 0., 1.);
	} else {
		rainbow = vec3(1., 0., x);
	}
	vec4 color = texture(TEXTURE, UV);
	vec4 fColor = mix(color, vec4(rainbow, color.a), strength);
	
	COLOR = vec4(min(COLOR.rgb, fColor.rgb), fColor.a);
}

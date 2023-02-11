shader_type canvas_item;

uniform vec4 shine_color : hint_color = vec4(1.0);
uniform float shine_size : hint_range(0.01, 1.0, 0.01) = 0.1;
uniform float shine_angle : hint_range(0.0, 89.9, 0.1) = 45.0;
uniform float speed : hint_range(0.0, 1) = .1;

void fragment() {
	//float temp = TIME;
	//float _shine_size = abs(cos(temp / speed));
	float _shine_size = shine_size;
    COLOR = texture(TEXTURE, UV);
	float slope = tan(radians(shine_angle));
	//vec3 intensity = vec3(dot(COLOR.rgb, vec3(0.3, 0.59, 0.11)));
    float shine = step(slope * UV.x - UV.y, _shine_size + _shine_size * slope) - step(slope * UV.x - UV.y, 1);
    COLOR.rgb = mix(COLOR.rgb, shine_color.rgb, shine * shine_color.a * UV.x);
}
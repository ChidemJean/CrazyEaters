shader_type canvas_item;

const float PI = 3.1415926535;
uniform vec4 shine_color : hint_color = vec4(1.0);
uniform float shine_progress : hint_range(0.0, 1.0, 0.01) = 0.0;
uniform float shine_size : hint_range(0.01, 1.0, 0.01) = 0.1;
uniform float shine_angle : hint_range(0.0, 89.9, 0.1) = 45.0;
uniform float speed : hint_range(0.0, 1) = .1;

float scale(float value, float inMin, float inMax, float outMin, float outMax) {
    return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
}

void fragment() {
	float _shine_progress = cos(TIME / speed);
	float _shine_size = abs(cos(TIME / speed) * .34);
    COLOR = texture(TEXTURE, UV);
	float slope = tan(radians(shine_angle));
	float progress = scale(_shine_progress, 0.0, 1.0, -1.0 - _shine_size - _shine_size * slope, 1.0 * slope);
    float shine = step(slope * UV.x - UV.y, progress + _shine_size + _shine_size * slope) - step(slope * UV.x - UV.y, progress);
    COLOR.rgb = mix(COLOR.rgb, shine_color.rgb, shine * shine_color.a);
}
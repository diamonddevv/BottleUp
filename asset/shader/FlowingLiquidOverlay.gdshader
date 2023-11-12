shader_type canvas_item;

uniform sampler2D NOISE_TEXTURE: repeat_enable;

uniform float strength: hint_range(0.0, 5, 0.1) = 1.0;
uniform float uv_scaling: hint_range (0.0, 1.0, 0.05) = 1.0;
uniform vec2 movement_direction = vec2(1, 0);
uniform vec2 wave_movement_direction = vec2(1, 0);
uniform float movement_speed: hint_range (0.0, 0.5, 0.01) = 0.1;
uniform float wave_movement_speed: hint_range (0.0, 0.5, 0.01) = 0.1;

uniform vec4 color : source_color;
uniform vec4 foam_color : source_color;

float average(vec3 v) {
	return (v.x + v.y + v.z) / 3.;
}

bool range(float f, float mn, float mx) {
	return (f > mn) && (f < mx);
}

void fragment() {
	vec2 uv = UV;
	
	vec2 movement_factor = movement_direction * movement_speed * TIME;
	vec2 wave_movement_factor = wave_movement_direction * wave_movement_speed * TIME;
	
	float noise_value = texture(NOISE_TEXTURE, uv*uv_scaling + movement_factor).r - 0.5;
	uv += noise_value * SCREEN_PIXEL_SIZE * strength;
	
	vec4 waveNoise = texture(NOISE_TEXTURE, UV + wave_movement_factor/5.);
	
	COLOR = texture(TEXTURE, uv) * color;
}
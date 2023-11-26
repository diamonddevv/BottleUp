shader_type canvas_item;

uniform bool _enabled = true;

uniform sampler2D noiseR : repeat_enable;
uniform sampler2D noiseG : repeat_enable;
uniform sampler2D noiseB : repeat_enable;

uniform float speed : hint_range(0.0, 1.0, 0.01);
uniform vec2 direction;
uniform vec2 zoom;
uniform float contrast : hint_range(0.0, 1.0, 0.1);
uniform float brightness : hint_range(0.0, 1.0, 0.1);
uniform float final_brightness : hint_range(0.0, 1.0, 0.1);

void fragment() {
	vec2 moveFactor = direction * speed * TIME;
	
	vec4 frag = texture(TEXTURE, UV);
	
	float sampleRa = texture(noiseR, UV*zoom + moveFactor).r - .5 * contrast + .5f + brightness;
	float sampleGa = texture(noiseG, UV*zoom + moveFactor).g - .5 * contrast + .5f + brightness;
	float sampleBa = texture(noiseB, UV*zoom + moveFactor).b - .5 * contrast + .5f + brightness;
	
	float sampleRb = texture(noiseR, UV*zoom - moveFactor).r - .5 * contrast + .5f + brightness;
	float sampleGb = texture(noiseG, UV*zoom - moveFactor).g - .5 * contrast + .5f + brightness;
	float sampleBb = texture(noiseB, UV*zoom - moveFactor).b - .5 * contrast + .5f + brightness;
	
	vec4 sample = mix(vec4(sampleRa, sampleGa, sampleBa, 1), vec4(sampleRb, sampleGb, sampleBb, 1), sin(TIME)*speed);

	
	if (_enabled) {
		frag.rgb += final_brightness;
		COLOR = frag * sample * 1.2;
	} else {
		COLOR = frag;
	}
}

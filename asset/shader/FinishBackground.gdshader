shader_type canvas_item;

uniform float speed : hint_range(0.0, 1.0, 0.01);
uniform vec2 direction;
uniform sampler2D noisemap : repeat_enable;

float avg(vec3 vec) {
	return (vec.x + vec.y + vec.z) / 3.;
}

void fragment() {
	vec2 moveFactor = direction * speed * TIME;
	vec4 frag = texture(noisemap, UV + moveFactor);
	COLOR += frag + vec4(UV.x, UV.y, 1, 1) - .8;

}

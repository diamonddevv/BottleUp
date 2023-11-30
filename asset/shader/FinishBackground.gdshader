shader_type canvas_item;

uniform float speed : hint_range(0.0, 1.0, 0.0000001);
uniform float XDirection : hint_range(-10.0, 10.0, 0.01);
uniform float YDirection : hint_range(-10.0, 10.0, 0.01);
uniform sampler2D noisemap : repeat_enable;

void fragment() {
	vec2 moveFactor = vec2(XDirection, YDirection) * speed * TIME;
	vec4 frag = texture(noisemap, UV*vec2(0.1) + moveFactor);
	vec4 col = mix(vec4(UV.x, UV.y, 1, 1), vec4(UV.y, UV.x, 1, 1), 1.8);
	COLOR = frag + col - .8;

}

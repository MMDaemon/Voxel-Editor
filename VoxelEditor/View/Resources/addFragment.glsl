#version 430 core

uniform sampler2D image1;
uniform sampler2D image2;

in vec2 uv;

void main() {
	vec4 texture1 = texture(image1, uv);
    vec4 texture2 = texture(image2, uv);
    float ratio = texture2.a / (texture1.a+texture2.a);
	gl_FragColor = mix(texture1, texture2, ratio);
}
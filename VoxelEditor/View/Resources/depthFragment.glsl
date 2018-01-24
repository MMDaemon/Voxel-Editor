#version 430 core

in float depth;

out vec4 color;

void main() {
	color = vec4(vec3(depth),1);
}
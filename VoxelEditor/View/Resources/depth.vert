#version 430 core

uniform mat4 camera;

in vec3 position;

out float depth;

void main() {
	gl_Position = camera * vec4(position, 1.0);
	depth = gl_Position.z;
}
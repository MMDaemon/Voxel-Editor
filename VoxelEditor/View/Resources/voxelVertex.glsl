#version 430 core

uniform mat4 camera;

in vec3 instancePosition;

in vec3 position;
in vec3 normal;

out vec3 var_color;

void main() {
	var_color = abs(normal);
	gl_Position = camera * vec4(position + instancePosition, 1.0);
}
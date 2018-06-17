#version 430 core

uniform mat4 camera;

in vec3 position;
in vec3 normal;
in vec2 uv;

out vec3 pos;
out vec3 n;
out vec2 uvPos;

void main() {
	pos = position;
	n = normal;
	uvPos = uv;
	gl_Position = camera * vec4(position, 1.0);
}
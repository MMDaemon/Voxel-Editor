#version 430 core

uniform mat4 camera;

in vec3 position;
in vec3 normal;
in vec3 uv3d;

out vec3 pos;
out vec3 n;
out vec2 uvX;
out vec2 uvY;
out vec2 uvZ;

void main() {
	pos = position;
	n = normal;
	uvX = uv3d.yz;
	uvY = uv3d.xz;
	uvZ = uv3d.xy;
	gl_Position = camera * vec4(position, 1.0);
}
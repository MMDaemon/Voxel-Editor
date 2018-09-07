#version 430 core

uniform mat4 camera;

in vec3 position;
in vec3 normal;
in vec3 uv3d;
in int voxelId;

out vec3 pos;
out vec3 n;
out vec3 uv;
flat out int id;

void main() {
	pos = position;
	n = normal;
	uv = uv3d;
	id = voxelId;
	gl_Position = camera * vec4(position, 1.0);
}
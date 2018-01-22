﻿#version 430 core

uniform mat4 camera;

in vec3 position;
in vec3 normal;

out vec3 pos;
out vec3 n;
out vec3 materialColor;

void main() {
	pos = position;
	n = normal;
	materialColor = vec3(1);
	gl_Position = camera * vec4(position, 1.0);
}
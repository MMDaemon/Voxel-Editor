#version 430 core

uniform vec3 cameraPosition;
uniform vec3 ambientLightColor;
uniform vec3 lightDirection;
uniform vec3 lightColor;
uniform sampler2D tex;

in vec3 pos;
in vec3 n;
in vec2 uvPos;

out vec4 color;

#include "lightCalculation.glsl"

void main() {
	vec3 normal = normalize(n);
	vec3 viewDirection = normalize(cameraPosition - pos);

	vec3 materialColor = texture(tex, uvPos).xyz;

	color = calculateLight(materialColor, lightColor, ambientLightColor, lightDirection, viewDirection, normal);
}
#version 430 core

uniform vec3 cameraPosition;
uniform vec3 ambientLightColor;
uniform vec3 lightDirection;
uniform vec3 lightColor;
uniform sampler2DArray texArray;
uniform int materialCount;

layout(std430, binding = 3) buffer materialLayout
{
	float materials[];
};

in vec3 pos;
in vec3 n;
in vec3 uv;
flat in int id;

out vec4 color;

#include "lightCalculation.glsl"

vec3 triplanarTexture(sampler2DArray texArray, float id, vec3 uv, vec3 normal)
{
	return (abs(normal.x) * texture(texArray, vec3(uv.yz, id)).xyz + abs(normal.y) * texture(texArray, vec3(uv.xz, id)).xyz + abs(normal.z) * texture(texArray, vec3(uv.xy, id)).xyz);
}

void main() {
	vec3 normal = normalize(n);
	vec3 viewDirection = normalize(cameraPosition - pos);

	vec3 materialColor = vec3(0);

	for(int i = 0; i < materialCount; i++){
		materialColor += materials[(id * materialCount) + i] * triplanarTexture(texArray, i, uv, normal);
	}

	color = calculateLight(materialColor, lightColor, ambientLightColor, lightDirection, viewDirection, normal);
}
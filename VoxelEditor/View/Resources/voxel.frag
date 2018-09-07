#version 430 core

uniform vec3 cameraPosition;
uniform vec3 ambientLightColor;
uniform vec3 lightDirection;
uniform vec3 lightColor;
uniform sampler2DArray texArray;

layout(std430, binding = 3) buffer amountLayout
{
	float amounts[];
};
layout(std430, binding = 4) buffer idLayout
{
	int ids[];
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

float[8] calculateDistances(vec3 uv){
	float distances[8];

	distances[0] = (1-uv.x)*(1-uv.y)*(1-uv.z);
	distances[1] = uv.x*(1-uv.y)*(1-uv.z);
	distances[2] = uv.x*uv.y*(1-uv.z);
	distances[3] = (1-uv.x)*uv.y*(1-uv.z);
	distances[4] = (1-uv.x)*(1-uv.y)*uv.z;
	distances[5] = uv.x*(1-uv.y)*uv.z;
	distances[6] = uv.x*uv.y*uv.z;
	distances[7] = (1-uv.x)*uv.y*uv.z;

	return distances;
}

void main() {
	vec3 normal = normalize(n);
	vec3 viewDirection = normalize(cameraPosition - pos);

	float distances[8] = calculateDistances(uv);

	float divider = 0;
	vec3 materialColor = vec3(0);

	for(int i = 0; i < 8; i++){
		materialColor += triplanarTexture(texArray, ids[id*8 + i], uv, normal) * amounts[id*8+i] * distances[i];
		divider += amounts[id*8+i]*distances[i];
	}
	materialColor/= divider;

	//materialColor = vec3(distances[7]);

	color = calculateLight(materialColor, lightColor, ambientLightColor, lightDirection, viewDirection, normal);
	//acolor = vec4(materialColor,1);
}
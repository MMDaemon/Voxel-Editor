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
in vec2 uvX;
in vec2 uvY;
in vec2 uvZ;
flat in int id;

out vec4 color;

float lambert(vec3 n, vec3 l)
{
	return max(0, dot(n, l));
}

float specular(vec3 n, vec3 l, vec3 v, float shininess)
{
	//if(0 > dot(n, l)) return 0;
	vec3 r = reflect(-l, n);
	return pow(max(0, dot(r, v)), shininess);
}

void main() {
	vec3 normal = normalize(n);
	vec3 viewDirection = normalize(cameraPosition - pos);

	vec3 materialColor = vec3(0);

	//materialColor += materials[(id * materialCount)+4];

	//for(int i = 0; i < materialCount; i++){
	//	materialColor += materials[(id * materialCount)];
	//}

	for(int i = 0; i < materialCount; i++){
		materialColor += materials[(id * materialCount) + i] * (abs(normal.x) * texture(texArray, vec3(uvX, i)).xyz + abs(normal.y) * texture(texArray, vec3(uvY, i)).xyz + abs(normal.z) * texture(texArray, vec3(uvZ, i)).xyz);
	}

	vec3 ambient = ambientLightColor * materialColor;

	vec3 light = materialColor * lightColor * lambert(normal, -lightDirection) + lightColor * specular(normal, lightDirection, viewDirection, 100);

	color = vec4(ambient + light, 1.0);
}
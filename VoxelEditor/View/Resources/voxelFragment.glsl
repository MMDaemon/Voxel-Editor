#version 430 core

uniform vec3 cameraPosition;
uniform vec3 ambientLightColor;
uniform vec3 lightDirection;
uniform vec3 lightColor;

in vec3 pos;
in vec3 n;
in vec3 materialColor;

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

	vec3 ambient = ambientLightColor * materialColor;

	vec3 light = materialColor * lightColor * lambert(normal, -lightDirection) + lightColor * specular(normal, -lightDirection, viewDirection, 100);

	color = vec4(ambient + light, 1.0);
}
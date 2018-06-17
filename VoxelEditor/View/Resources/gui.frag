#version 430 core
uniform sampler2D texDiffuse;

in vec2 uvPos;
out vec4 color;

void main() 
{
	color = texture(texDiffuse, uvPos);
}
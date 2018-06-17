#version 430 core				
in vec2 position;
in vec2 uv;

out vec2 uvPos;

void main() 
{
	uvPos = uv;
	gl_Position = vec4(position, 0.0, 1.0);
}
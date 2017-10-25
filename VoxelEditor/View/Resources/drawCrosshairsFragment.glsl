#version 430 core

uniform float aspect;

in vec2 uv;

void main() {
	vec2 pos = uv*2 -1;
	pos.x*=aspect;
	vec4 color = vec4(0);
	if(abs(pos.x)+abs(pos.y)<0.02){
		color = vec4(0,0,0,1);
	}
	if(abs(pos.x)+abs(pos.y)<0.019){
		color = vec4(1,1,1,1);
	}
	if(abs(pos.x)+abs(pos.y)<0.015){
		color = vec4(0,0,0,1);
	}
	if(abs(pos.x)+abs(pos.y)<0.014){
		color = vec4(0);
	}
	if(abs(pos.x)+abs(pos.y)<0.003){
		color = vec4(0,0,0,1);
	}
	if(abs(pos.x)+abs(pos.y)<0.002){
		color = vec4(1,1,1,1);
	}
	gl_FragColor = color;
}
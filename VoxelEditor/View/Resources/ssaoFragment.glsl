#version 430 core

#define SAMPLES 30

uniform sampler2D image;
uniform float iGlobalTime;

in vec2 uv;

void main()
{
	float radAttenuation = 5.;//0:2:1
	float radius = 0.01;//0:0.6:0.024
	float spiral = 16524.56;//1:100:50
	float spinSpeed = .10;

	// sample zbuffer (in linear eye space) at the current shading point	
	float zr = 1.0-texture( image, uv ).x;

    // sample neighbor pixels
	float ao = 0.0;
	for( int i=0; i<SAMPLES; i++ )
	{
		float f = float(i)/float(SAMPLES);
        // get a random 2D offset vector
        vec2 off = vec2(radius*pow(f, radAttenuation)*sin(f*spiral+iGlobalTime*spinSpeed), 
						radius*pow(f, radAttenuation)*cos(f*spiral+iGlobalTime*spinSpeed));	
        // sample the zbuffer at a neightbor pixel (in a 16 pixel radious)        		
        float z = 1.0-texture( image, uv + off ).x;
        // accumulate occlusion if difference is less than 0.1 units
		ao += clamp( (zr-z)/0.1, 0.0, 1.0);
	}
    // average down the occlusion	
    ao = clamp( 1.0 - ao/8.0, 0.0, 1.0 );
	
	vec3 col = vec3(ao);
	
	gl_FragColor = vec4(col,1.0);
}
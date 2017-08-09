using DMS.OpenGL;

namespace DMS.Application
{
	public interface IShaderProvider
	{
		//public delegate void ShaderChangedHandler(string name, Shader shader);
		//public event ShaderChangedHandler ShaderChanged;

		Shader GetShader(string name);
		//Texture GetTexture(string name);
	}
}
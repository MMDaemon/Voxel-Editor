using DMS.OpenGL;
using System;

namespace DMS.Application
{
	public class ResourceVertFragShaderString : IResource<Shader>
	{
		public ResourceVertFragShaderString(string sVertex, string sFragment)
		{
			shader = ShaderLoader.FromStrings(sVertex, sFragment);
		}

		public bool IsValueCreated { get { return true; } }

		public Shader Value { get { return shader; } }

		public event EventHandler<Shader> Change { add { } remove { } }

		private Shader shader;
	}
}
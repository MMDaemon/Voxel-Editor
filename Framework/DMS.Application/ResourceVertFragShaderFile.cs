using DMS.OpenGL;
using System;

namespace DMS.Application
{
	public class ResourceVertFragShaderFile : IResource<Shader>
	{
		public ResourceVertFragShaderFile(string sVertexShdFile_, string sFragmentShdFile_)
		{
			shader = ShaderLoader.FromFiles(sVertexShdFile_, sFragmentShdFile_);
		}

		public bool IsValueCreated { get { return true; } }

		public Shader Value { get { return shader; } }

		public event EventHandler<Shader> Change { add { } remove { } }

		private Shader shader;
	}
}

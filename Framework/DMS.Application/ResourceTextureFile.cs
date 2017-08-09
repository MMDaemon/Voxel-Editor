using DMS.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Application
{
	public class ResourceTextureFile : IResource<Texture>
	{
		public ResourceTextureFile(string filePath)
		{
			texture = TextureLoader.FromFile(filePath);
		}

		public bool IsValueCreated { get { return true; } }

		public Texture Value { get { return texture; } }

		public event EventHandler<Texture> Change { add { } remove { } }

		private Texture texture;
	}
}

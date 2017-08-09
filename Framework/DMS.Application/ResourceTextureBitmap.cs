using DMS.OpenGL;
using System;
using System.Drawing;

namespace DMS.Application
{
	public class ResourceTextureBitmap : IResource<Texture>
	{
		public ResourceTextureBitmap(Bitmap bitmap)
		{
			texture = TextureLoader.FromBitmap(bitmap);
		}

		public bool IsValueCreated { get { return true;  } }

		public Texture Value { get { return texture; } }

		public event EventHandler<Texture> Change {  add { } remove { } }

		private Texture texture;
	}
}

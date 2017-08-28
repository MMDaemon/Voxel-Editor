using System.Collections.Generic;
using System.Drawing;
using VoxelEditor.Controller.Initialisation;

namespace VoxelEditor.View.Registry
{
	internal class ViewRegistry
	{
		private Dictionary<int, MaterialInfo> _materials;

		public ViewRegistry()
		{
			_materials = new Dictionary<int, MaterialInfo>();
		}

		internal void RegisterMaterial(int id, Bitmap texture, RenderProperties renderProperties)
		{
			//TODO implement
		}
	}
}

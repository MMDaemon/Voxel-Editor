using System.Collections.Generic;
using System.Drawing;
using VoxelEditor.Initialisation.Material;

namespace VoxelEditor.Registry.View
{
	internal class ViewRegistry
	{
		private Dictionary<int, ViewMaterialInfo> _materials;

		public ViewRegistry()
		{
			_materials = new Dictionary<int, ViewMaterialInfo>();
		}

		internal void RegisterMaterial(int id, Bitmap texture, RenderProperties renderProperties)
		{
			//TODO implement
		}
	}
}

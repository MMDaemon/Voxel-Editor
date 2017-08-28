using System.Collections.Generic;
using VoxelEditor.Controller.Initialisation;

namespace VoxelEditor.Model.Registry
{
	internal class ModelRegistry
	{
		private readonly Dictionary<int, MaterialInfo> _materials;

		public ModelRegistry()
		{
			_materials = new Dictionary<int, MaterialInfo>();
		}

		public void RegisterMaterial(int id, MaterialBehavior materialBehavior)
		{
			//TODO implement
		}

		public bool MaterialExists(int id)
		{
			return _materials.ContainsKey(id);
		}
	}
}

using System.Collections.Generic;
using MVCCore.Interfaces;
using VoxelUtils.Initialization;

namespace VoxelUtils.Registry.Model
{
	public class ModelRegistry : IModelRegistry
	{
		private readonly Dictionary<int, ModelMaterialInfo> _materials;

		public ModelRegistry()
		{
			_materials = new Dictionary<int, ModelMaterialInfo>();
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

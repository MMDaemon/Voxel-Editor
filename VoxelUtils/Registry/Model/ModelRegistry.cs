using System.Collections.Generic;
using MVCCore.Interfaces;

namespace VoxelUtils.Registry.Model
{
    public class ModelRegistry : IModelRegistry
	{
		private readonly Dictionary<int, IModelMaterialInfo> _materials;

	    public int MaterialCount => _materials.Count;

		public ModelRegistry()
		{
			_materials = new Dictionary<int, IModelMaterialInfo>();
		}

		public void RegisterMaterial(int id, IModelMaterialInfo materialInfo)
		{
			_materials.Add(id, materialInfo);
		}

		public bool MaterialExists(int id)
		{
			return _materials.ContainsKey(id);
		}

	    public IModelMaterialInfo GetMaterialInfo(int id)
	    {
	        return _materials[id];
	    }
    }
}

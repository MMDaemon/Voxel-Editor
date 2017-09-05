﻿using System.Collections.Generic;
using VoxelEditor.Initialisation.Material;

namespace VoxelEditor.Registry.Model
{
	internal class ModelRegistry
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
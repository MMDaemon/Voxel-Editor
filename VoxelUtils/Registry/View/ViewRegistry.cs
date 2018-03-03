using System.Collections.Generic;
using System.Linq;
using MVCCore.Interfaces;

namespace VoxelUtils.Registry.View
{
    public class ViewRegistry : IViewRegistry
    {
        public List<int> MaterialIds => _materials.Keys.ToList();
        private Dictionary<int, IViewMaterialInfo> _materials;

        public ViewRegistry()
        {
            _materials = new Dictionary<int, IViewMaterialInfo>();
        }

        public void RegisterMaterial(int id, IViewMaterialInfo materialInfo)
        {
            _materials.Add(id, materialInfo);
        }

        public IViewMaterialInfo GetMaterialInfo(int id)
        {
            return _materials[id];
        }
    }
}

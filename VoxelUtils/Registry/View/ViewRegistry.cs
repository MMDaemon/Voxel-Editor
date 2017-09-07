using System.Collections.Generic;
using System.Drawing;
using MVCCore.Interfaces;
using VoxelUtils.Initialization;

namespace VoxelUtils.Registry.View
{
    public class ViewRegistry : IViewRegistry
    {
        private Dictionary<int, ViewMaterialInfo> _materials;

        public ViewRegistry()
        {
            _materials = new Dictionary<int, ViewMaterialInfo>();
        }

        public void RegisterMaterial(int id, Bitmap texture, RenderProperties renderProperties)
        {
            //TODO implement
        }
    }
}

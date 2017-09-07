using System.Collections.Generic;
using System.Drawing;
using VoxelEditor.Common.Initialization;
using VoxelEditor.MVCInterfaces;

namespace VoxelEditor.Registry.View
{
    internal class ViewRegistry : IViewRegistry
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

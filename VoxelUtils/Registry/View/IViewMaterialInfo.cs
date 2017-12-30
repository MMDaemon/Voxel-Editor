using System.Drawing;
using VoxelUtils.Initialization;

namespace VoxelUtils.Registry.View
{
	public interface IViewMaterialInfo
	{
	    string Name { get; }
	    Bitmap Texture { get; }
	    RenderProperties RenderProperties { get; }
    }
}

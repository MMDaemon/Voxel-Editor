using System.Drawing;
using VoxelUtils.Registry.Model;
using VoxelUtils.Registry.View;

namespace VoxelUtils.Initialization
{
	public class MaterialInfo : IModelMaterialInfo, IViewMaterialInfo
	{
	    public string Name { get; private set; }
		public Bitmap Texture { get; private set; }
		public RenderProperties RenderProperties { get; private set; }

		public MaterialBehavior MaterialBehavior { get; private set; }

		public MaterialInfo(string name, Bitmap texture, RenderProperties renderProperties, MaterialBehavior materialBehavior)
		{
		    Name = name;
            Texture = texture;
			RenderProperties = renderProperties;
			MaterialBehavior = materialBehavior;
		}
	}
}
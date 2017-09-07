using System.Drawing;

namespace VoxelEditor.Common.Initialization
{
	internal class MaterialInfo
	{
		public Bitmap Texture { get; private set; }
		public RenderProperties RenderProperties { get; private set; }

		public MaterialBehavior MaterialBehavior { get; private set; }

		public MaterialInfo(Bitmap texture, RenderProperties renderProperties, MaterialBehavior materialBehavior)
		{
			Texture = texture;
			RenderProperties = renderProperties;
			MaterialBehavior = materialBehavior;
		}
	}
}
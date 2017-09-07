using System.Collections.Generic;
using System.Numerics;

namespace VoxelEditor.MVCInterfaces
{
	public class ModelInput
	{
		public Vector2 MousePosition { get; private set; }

		public List<int> KeyActions { get; private set; }

		public ModelInput(Vector2 mousePosition)
		{
			MousePosition = mousePosition;
			KeyActions = new List<int>();
		}
	}
}

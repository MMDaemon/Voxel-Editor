using System.Collections.Generic;
using System.Numerics;
using VoxelEditor.Common.Enums;

namespace VoxelEditor.Common.Transfer
{
	internal class ModelInput
	{
		public Vector2 MousePosition { get; private set; }

		public List<KeyAction> KeyActions { get; private set; }

		public ModelInput(Vector2 mousePosition)
		{
			MousePosition = mousePosition;
			KeyActions = new List<KeyAction>();
		}
	}
}

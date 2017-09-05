using System.Collections.Generic;
using System.Numerics;
using VoxelEditor.Common.Enums;

namespace VoxelEditor.Common.Transfer
{
	internal class ModelInput
	{
		public Vector2 MousePos { get; private set; }

		public List<KeyAction> KeyActions { get; private set; }

		public ModelInput(Vector2 mousePos)
		{
			MousePos = mousePos;
			KeyActions = new List<KeyAction>();
		}
	}
}

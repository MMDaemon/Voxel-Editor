using System.Numerics;

namespace VoxelEditor.Common.Transfer
{
	internal class ViewModel
	{
		public Vector3 PlayerPosition { get; private set; }
		public ViewModel(Vector3 playerPosition)
		{
			PlayerPosition = playerPosition;
		}
	}
}

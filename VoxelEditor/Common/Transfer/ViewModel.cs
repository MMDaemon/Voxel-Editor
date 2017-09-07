using System.Numerics;

namespace VoxelEditor.Common.Transfer
{
	internal class ViewModel
	{
		public Vector3 PlayerPosition { get; private set; }
        public Vector2 PlayerRotation { get; private set; }
		public ViewModel(Vector3 playerPosition, Vector2 playerRotation)
		{
		    PlayerPosition = playerPosition;
		    PlayerRotation = playerRotation;
		}
	}
}

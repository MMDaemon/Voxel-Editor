using System.Numerics;

namespace VoxelEditor.Editor.Model
{
	internal class Player
	{
		public Vector3 Position { get; private set; }

		public Player()
		{
			Position = Vector3.Zero;
		}

		public void Move(Vector3 velocity, float timeDelta)
		{
			Position += velocity* timeDelta;
		}
	}
}

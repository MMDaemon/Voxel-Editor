namespace VoxelUtils.Shared
{
	public class Chunk
	{
		public bool Full => _usedVoxelAmount == Constant.MaxVoxelsPerChunk;
		public bool Empty => _usedVoxelAmount == 0;

		private readonly Voxel[,,] _voxels;
		private int _usedVoxelAmount;

		public Chunk()
		{
			_voxels = new Voxel[Constant.ChunkSizeX + 1, Constant.ChunkSizeY + 1, Constant.ChunkSizeZ + 1];
			_usedVoxelAmount = 0;
		}

		public Voxel this[int x, int y, int z]
		{
			get { return _voxels[x, y, z]; }
			set
			{
				if (x < Constant.ChunkSizeX && y < Constant.ChunkSizeY && z < Constant.ChunkSizeZ)
				{
					if (value != null && _voxels[x, y, z] == null)
					{
						_usedVoxelAmount++;
					}
					else if (value == null && _voxels[x, y, z] != null)
					{
						_usedVoxelAmount--;
					}
				}

				_voxels[x, y, z] = value;
			}
		}
	}
}

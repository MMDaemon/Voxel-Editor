using System.Numerics;

namespace VoxelUtils.Shared
{
    public class Chunk
    {
        public Vector3I Position { get; private set; }
        public bool Full => _usedVoxelAmount == Constant.MaxVoxelsPerChunk;
        public bool Empty => _usedVoxelAmount == 0;

        private readonly Voxel[,,] _voxels;
        private int _usedVoxelAmount;

        public Chunk(Vector3I position)
        {
            Position = position;
            _voxels = new Voxel[Constant.ChunkSizeX + 1, Constant.ChunkSizeY + 1, Constant.ChunkSizeZ + 1];
            _usedVoxelAmount = 0;
        }

        public Voxel this[Vector3I position]
        {
            get => _voxels[position.X, position.Y, position.Z];
            set
            {
                if (value != null && _voxels[position.X, position.Y, position.Z] == null)
                {
                    _usedVoxelAmount++;
                }
                else if (value == null && _voxels[position.X, position.Y, position.Z] != null)
                {
                    _usedVoxelAmount--;
                }

                _voxels[position.X, position.Y, position.Z] = value;
            }
        }

        public Voxel this[int x, int y, int z]
        {
            get => _voxels[x, y, z];
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

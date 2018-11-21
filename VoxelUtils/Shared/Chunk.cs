using System.Net.Mime;
using System.Xml.Linq;

namespace VoxelUtils.Shared
{
    public class Chunk
    {
        public Vector3I Position { get; private set; }
        public bool Full => UsedVoxelAmount == Constant.MaxVoxelsPerChunk;
        public bool Empty => UsedVoxelAmount == 0;

        public int UsedVoxelAmount { get; set; }

        private readonly Voxel[,,] _voxels;

        public Chunk(Vector3I position)
        {
            Position = position;
            _voxels = new Voxel[Constant.ChunkSizeX + 1, Constant.ChunkSizeY + 1, Constant.ChunkSizeZ + 1];
            UsedVoxelAmount = 0;

            InitializeVoxels();
        }

        public Voxel this[Vector3I position]
        {
            get => this[position.X, position.Y, position.Z];
            set => this[position.X, position.Y, position.Z] = value;
        }

        public Voxel this[int x, int y, int z]
        {
            get => _voxels[x, y, z];
            set
            {
                if (x < Constant.ChunkSizeX && y < Constant.ChunkSizeY && z < Constant.ChunkSizeZ)
                {
                    if (value != null && value.Amount != 0 && (_voxels[x, y, z] == null || _voxels[x, y, z].Amount == 0))
                    {
                        UsedVoxelAmount++;
                    }
                    else if ((value == null || value.Amount == 0) && _voxels[x, y, z] != null)
                    {
                        UsedVoxelAmount--;
                    }
                }

                _voxels[x, y, z] = value;
            }
        }

        private void InitializeVoxels()
        {
            for (int x = 0; x <= Constant.ChunkSizeX; x++)
            {
                for (int y = 0; y <= Constant.ChunkSizeY; y++)
                {
                    for (int z = 0; z <= Constant.ChunkSizeZ; z++)
                    {
                        _voxels[x, y, z] = new Voxel(Constant.MaterialAir, 0);
                    }
                }
            }
        }

        public override string ToString()
        {
            string position = $"{Position.X},{Position.Y},{Position.Z}";
            string types = "";
            string amounts = "";

            if (Empty)
            {
                types += $"{Constant.MaxVoxelsPerChunk}:{Constant.MaterialAir},";
                amounts += $"{Constant.ChunkSizeX * Constant.ChunkSizeY * Constant.ChunkSizeZ}:{0},";
            }

            else
            {
                int typeCount = 0;
                int currentType = Constant.MaterialAir;
                int amountCount = 0;
                int currentAmount = 0;

                for (int z = 0; z < Constant.ChunkSizeX; z++)
                {
                    for (int y = 0; y < Constant.ChunkSizeY; y++)
                    {
                        for (int x = 0; x < Constant.ChunkSizeZ; x++)
                        {
                            if (_voxels[x, y, z].MaterialId == currentType)
                            {
                                typeCount++;
                            }
                            else
                            {
                                if (typeCount > 0)
                                {
                                    types += $"{typeCount}:{currentType},";
                                }
                                currentType = _voxels[x, y, z].MaterialId;
                                typeCount = 1;
                            }

                            if (_voxels[x, y, z].Amount == currentAmount)
                            {
                                amountCount++;
                            }
                            else
                            {
                                if (amountCount > 0)
                                {
                                    amounts += $"{amountCount}:{currentAmount},";
                                }
                                currentAmount = _voxels[x, y, z].Amount;
                                amountCount = 1;
                            }
                        }
                    }
                }

                types += $"{typeCount}:{currentType},";
                amounts += $"{amountCount}:{currentAmount},";
            }

            XElement chunk =
                new XElement("Chunk",
                    new XElement("Position", position),
                    new XElement("Types", types),
                    new XElement("Amounts", amounts));

            return chunk.ToString();
        }
    }
}

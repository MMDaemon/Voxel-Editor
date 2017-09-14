using System.Collections.Generic;
using System.Linq;
using VoxelUtils;
using VoxelUtils.Shared;

namespace VoxelEditor.Model
{
    internal class World
    {
        private readonly Dictionary<Vector3I, Chunk> _chunks;

        public List<Chunk> Chunks => _chunks.Values.ToList();
        public float VoxelSize => 0.5f / Constant.ChunkSizeY;

        public World()
        {
            _chunks = new Dictionary<Vector3I, Chunk>();
            InitializeChunks(new Vector3I(4, 4, 4));

        }

        public void AddMaterial(int materialId, int amount, Vector3I globalPosition)
        {
            Vector3I chunkPosition = CalculateChunkPosition(globalPosition);
            Vector3I voxelPosition = CalculateVoxelPositionInChunk(globalPosition, chunkPosition);

            if (_chunks[chunkPosition][voxelPosition] != null)
            {
                if (_chunks[chunkPosition][voxelPosition].MaterialId == materialId)
                {
                    _chunks[chunkPosition][voxelPosition].AddMaterial(amount);
                }
            }
            else
            {
                _chunks[chunkPosition][voxelPosition] = new Voxel(materialId, amount);
            }
        }

        public void TakeMaterial(int materialId, int amount, Vector3I globalPosition)
        {
            Vector3I chunkPosition = CalculateChunkPosition(globalPosition);
            Vector3I voxelPosition = CalculateVoxelPositionInChunk(globalPosition, chunkPosition);

            if (_chunks[chunkPosition][voxelPosition] != null && _chunks[chunkPosition][voxelPosition].MaterialId == materialId)
            {
                _chunks[chunkPosition][voxelPosition].TakeMaterial(amount);
            }
        }

        private void InitializeChunks(Vector3I size)
        {
            Vector3I position = -(size / 2);
            position.Y = 0; /*minimum height = 0*/
            Vector3I initialPosition = position;
            size += position;
            while (position.X < size.X)
            {
                while (position.Y < size.Y)
                {
                    while (position.Z < size.Z)
                    {
                        _chunks.Add(position, new Chunk(position));
                        position.Z++;
                    }
                    position.Z = initialPosition.Z;
                    position.Y++;
                }
                position.Y = initialPosition.Y;
                position.X++;
            }
        }

        private Vector3I CalculateChunkPosition(Vector3I globalPosition)
        {
            if (globalPosition.X < 0)
            {
                globalPosition.X -= (Constant.ChunkSizeX - 1);
            }
            if (globalPosition.Y < 0)
            {
                globalPosition.Y -= (Constant.ChunkSizeY - 1);
            }
            if (globalPosition.Z < 0)
            {
                globalPosition.Z -= (Constant.ChunkSizeZ - 1);
            }
            return globalPosition / Constant.ChunkSize;
        }

        private Vector3I CalculateVoxelPositionInChunk(Vector3I globalPosition, Vector3I chunkPosition)
        {
            Vector3I voxelInChunkPosition = globalPosition - (chunkPosition * Constant.ChunkSize);

            return voxelInChunkPosition;
        }
    }
}

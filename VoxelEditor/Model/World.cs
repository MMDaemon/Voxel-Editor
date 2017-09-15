using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VoxelUtils;
using VoxelUtils.Shared;

namespace VoxelEditor.Model
{
    internal class World
    {
        private readonly Vector3I _worldSize;
        private readonly Dictionary<Vector3I, Chunk> _chunks;

        public List<Chunk> Chunks => _chunks.Values.ToList();
        public float VoxelSize => 0.5f / Constant.ChunkSizeY;

        public World(Vector3I worldSize)
        {
            _worldSize = worldSize;
            _chunks = new Dictionary<Vector3I, Chunk>();
            InitializeChunks();

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

        public bool RaytraceFilledVoxel(Vector3 rayStartPosition, Vector3 rayDirection, out Vector3 voxelPosition)
        {
            bool success = CalculateRayStartPosition(ref rayStartPosition, rayDirection);
            voxelPosition = new Vector3(-1,-1,-1);

            if (success)
            {
                Console.WriteLine(rayStartPosition);
                Vector3I rayPosition = (Vector3I)rayStartPosition;
                Vector3 rayOffset = rayStartPosition - rayPosition;
            }

            return success;
        }

        private bool CalculateRayStartPosition(ref Vector3 rayStartPosition, Vector3 rayDirection)
        {
            rayStartPosition /= VoxelSize;

            Vector3I negativeWorldSize = ((-_worldSize / 2) * Constant.ChunkSize);
            negativeWorldSize.Y = 0;
            Vector3I positiveWorldSize = ((_worldSize / 2) * Constant.ChunkSize);
            positiveWorldSize.Y = _worldSize.Y * Constant.ChunkSizeY;

            if (IsInsideWorld(rayStartPosition))
            {
                return true;
            }

            if (rayStartPosition.X < negativeWorldSize.X)
            {
                Vector3 planePosition = new Vector3(negativeWorldSize.X, 0, 0);
                Vector3 planeNormal = new Vector3(-1, 0, 0);
                float denominator = Vector3.Dot(rayDirection, planeNormal);
                if (Math.Abs(denominator) > 0.0f)
                {
                    float d = Vector3.Dot((planePosition - rayStartPosition), planeNormal) / denominator;
                    Vector3 collision = rayStartPosition + rayDirection * d;
                    if (IsInsideWorld(collision))
                    {
                        rayStartPosition = collision;
                        return true;
                    }
                }
            }

            if (rayStartPosition.X > positiveWorldSize.X)
            {
                Vector3 planePosition = new Vector3(positiveWorldSize.X, 0, 0);
                Vector3 planeNormal = new Vector3(1, 0, 0);
                float denominator = Vector3.Dot(rayDirection, planeNormal);
                if (Math.Abs(denominator) > 0.0f)
                {
                    float d = Vector3.Dot((planePosition - rayStartPosition), planeNormal) / denominator;
                    Vector3 collision = rayStartPosition + rayDirection * d;
                    if (IsInsideWorld(collision))
                    {
                        rayStartPosition = collision;
                        return true;
                    }
                }
            }

            if (rayStartPosition.Y < negativeWorldSize.Y)
            {
                Vector3 planePosition = new Vector3(0, negativeWorldSize.Y, 0);
                Vector3 planeNormal = new Vector3(0, -1, 0);
                float denominator = Vector3.Dot(rayDirection, planeNormal);
                if (Math.Abs(denominator) > 0.0f)
                {
                    float d = Vector3.Dot((planePosition - rayStartPosition), planeNormal) / denominator;
                    Vector3 collision = rayStartPosition + rayDirection * d;
                    if (IsInsideWorld(collision))
                    {
                        rayStartPosition = collision;
                        return true;
                    }
                }
            }

            if (rayStartPosition.Y > positiveWorldSize.Y)
            {
                Vector3 planePosition = new Vector3(0, positiveWorldSize.Y, 0);
                Vector3 planeNormal = new Vector3(0, 1, 0);
                float denominator = Vector3.Dot(rayDirection, planeNormal);
                if (Math.Abs(denominator) > 0.0f)
                {
                    float d = Vector3.Dot((planePosition - rayStartPosition), planeNormal) / denominator;
                    Vector3 collision = rayStartPosition + rayDirection * d;
                    if (IsInsideWorld(collision))
                    {
                        rayStartPosition = collision;
                        return true;
                    }
                }
            }

            if (rayStartPosition.Z < negativeWorldSize.Z)
            {
                Vector3 planePosition = new Vector3(0, 0, negativeWorldSize.Z);
                Vector3 planeNormal = new Vector3(0, 0, -1);
                float denominator = Vector3.Dot(rayDirection, planeNormal);
                if (Math.Abs(denominator) > 0.0f)
                {
                    float d = Vector3.Dot((planePosition - rayStartPosition), planeNormal) / denominator;
                    Vector3 collision = rayStartPosition + rayDirection * d;
                    if (IsInsideWorld(collision))
                    {
                        rayStartPosition = collision;
                        return true;
                    }
                }
            }

            if (rayStartPosition.Z > positiveWorldSize.Z)
            {
                Vector3 planePosition = new Vector3(0, 0, positiveWorldSize.Z);
                Vector3 planeNormal = new Vector3(0, 0, 1);
                float denominator = Vector3.Dot(rayDirection, planeNormal);
                if (Math.Abs(denominator) > 0.0f)
                {
                    float d = Vector3.Dot((planePosition - rayStartPosition), planeNormal) / denominator;
                    Vector3 collision = rayStartPosition + rayDirection * d;
                    if (IsInsideWorld(collision))
                    {
                        rayStartPosition = collision;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsInsideWorld(Vector3 position)
        {
            Vector3I negativeWorldSize = ((-_worldSize / 2) * Constant.ChunkSize);
            negativeWorldSize.Y = 0;
            Vector3I positiveWorldSize = ((_worldSize / 2) * Constant.ChunkSize);
            positiveWorldSize.Y = _worldSize.Y * Constant.ChunkSizeY;
            return !(position < negativeWorldSize) && !(position > positiveWorldSize);
        }


        private void InitializeChunks()
        {
            Vector3I position = -(_worldSize / 2);
            position.Y = 0; /*minimum height = 0*/
            Vector3I initialPosition = position;
            Vector3I worldSize = _worldSize + position;
            while (position.X < worldSize.X)
            {
                while (position.Y < worldSize.Y)
                {
                    while (position.Z < worldSize.Z)
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

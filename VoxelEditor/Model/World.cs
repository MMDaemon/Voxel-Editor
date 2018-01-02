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
        private readonly Dictionary<Vector3I, Chunk> _chunks;
        private readonly List<Vector3I> _updatedChunkCoordinates;
        private readonly VoxelMarcher _voxelMarcher;

        public Vector3I WorldSize { get; private set; }
        public IEnumerable<Chunk> Chunks => _chunks.Values.ToList();

        public IEnumerable<Chunk> UpdatedChunks
        {
            get
            {
                List<Chunk> updatedChunks = new List<Chunk>();
                foreach (Vector3I coordinate in _updatedChunkCoordinates)
                {
                    updatedChunks.Add(_chunks[coordinate]);
                }
                return updatedChunks;
            }
        }

        public World(Vector3I worldSize)
        {
            WorldSize = worldSize;
            _chunks = new Dictionary<Vector3I, Chunk>();
            _updatedChunkCoordinates = new List<Vector3I>();
            _voxelMarcher = new VoxelMarcher(worldSize);
            InitializeChunks();

        }

        public Voxel GetVoxel(Vector3I globalPosition)
        {
            Vector3I chunkPosition = CalculateChunkPosition(globalPosition);
            Vector3I voxelPosition = CalculateVoxelPositionInChunk(globalPosition, chunkPosition);

            return _chunks[chunkPosition][voxelPosition];
        }

        public bool AddMaterial(int materialId, int amount, Vector3I globalPosition)
        {
            bool success = false;

            if (amount > 0)
            {
                bool emptyBefore = false;

                Vector3I chunkPosition = CalculateChunkPosition(globalPosition);
                Vector3I voxelPosition = CalculateVoxelPositionInChunk(globalPosition, chunkPosition);

                if (amount <= Constant.MaxMaterialAmount)
                {
                    IList<(Vector3I ChunkPosition, Vector3I VoxelPosition)> positions =
                        CalculateVoxelPositions(globalPosition);

                    if (_chunks[chunkPosition][voxelPosition].MaterialId == Constant.MaterialAir)
                    {
                        emptyBefore = true;
                        foreach ((Vector3I ChunkPosition, Vector3I VoxelPosition) position in positions)
                        {
                            _chunks[position.ChunkPosition][position.VoxelPosition].MaterialId = materialId;
                        }
                    }
                    if (_chunks[chunkPosition][voxelPosition].MaterialId == materialId && _chunks[chunkPosition][voxelPosition].Amount + amount <= Constant.MaxMaterialAmount)
                    {
                        foreach ((Vector3I ChunkPosition, Vector3I VoxelPosition) position in positions)
                        {
                            _chunks[position.ChunkPosition][position.VoxelPosition].AddMaterial(amount);
                            if (!_updatedChunkCoordinates.Contains(position.ChunkPosition))
                            {
                                _updatedChunkCoordinates.Add(position.ChunkPosition);
                            }
                        }
                        success = true;
                    }
                }

                if (emptyBefore && success)
                {
                    DecrementNeighborsEmptyNeighborCount(globalPosition);
                }

            }

            return success;
        }

        public bool TakeMaterial(int materialId, int amount, Vector3I globalPosition)
        {
            bool success = false;

            if (amount > 0)
            {
                Vector3I chunkPosition = CalculateChunkPosition(globalPosition);
                Vector3I voxelPosition = CalculateVoxelPositionInChunk(globalPosition, chunkPosition);

                if (_chunks[chunkPosition][voxelPosition].MaterialId == materialId && _chunks[chunkPosition][voxelPosition].Amount >= amount)
                {
                    IList<(Vector3I ChunkPosition, Vector3I VoxelPosition)> positions =
                        CalculateVoxelPositions(globalPosition);

                    foreach ((Vector3I ChunkPosition, Vector3I VoxelPosition) position in positions)
                    {
                        _chunks[position.ChunkPosition][position.VoxelPosition].TakeMaterial(amount);
                        if (!_updatedChunkCoordinates.Contains(position.ChunkPosition))
                        {
                            _updatedChunkCoordinates.Add(position.ChunkPosition);
                        }
                    }

                    success = true;
                    if (_chunks[chunkPosition][voxelPosition].Amount == 0)
                    {
                        IncrementNeighborsEmptyNeighborCount(globalPosition);
                    }
                }
            }


            return success;
        }

        public bool RaytraceFilledVoxel(Vector3 rayPosition, Vector3 rayDirection, out Vector3I voxelPosition, out Vector3 hitPosition)
        {
            bool success = _voxelMarcher.InitializeStartPosition(rayPosition, rayDirection);
            voxelPosition = new Vector3I(-1);
            hitPosition = new Vector3(-1);

            while (success && !PositionIsUnderWorld(_voxelMarcher.VoxelPosition) && GetVoxel(_voxelMarcher.VoxelPosition).IsEmpty)
            {
                success = _voxelMarcher.CalculateNextPosition();
            }
            if (success && !PositionIsUnderWorld(_voxelMarcher.VoxelPosition) && !GetVoxel(_voxelMarcher.VoxelPosition).IsEmpty)
            {
                voxelPosition = _voxelMarcher.VoxelPosition;
                hitPosition = _voxelMarcher.HitPosition;
            }
            else
            {
                success = false;
            }
            return success;
        }

        public bool RaytraceEmptyOnFilledVoxel(Vector3 rayPosition, Vector3 rayDirection, out Vector3I voxelPosition, out Vector3 hitPosition)
        {
            bool success = _voxelMarcher.InitializeStartPosition(rayPosition, rayDirection);
            voxelPosition = new Vector3I(-1);
            hitPosition = new Vector3(-1);

            bool emptySet = false;

            while (success && !PositionIsUnderWorld(_voxelMarcher.VoxelPosition) && GetVoxel(_voxelMarcher.VoxelPosition).IsEmpty)
            {
                emptySet = true;
                voxelPosition = _voxelMarcher.VoxelPosition;

                success = _voxelMarcher.CalculateNextPosition();
            }
            if (success && emptySet && (PositionIsUnderWorld(_voxelMarcher.VoxelPosition) ||
                                        !GetVoxel(_voxelMarcher.VoxelPosition).IsEmpty))
            {
                hitPosition = _voxelMarcher.HitPosition;
            }
            else
            {
                success = false;
            }
            return success;
        }

        public void ResetUpdateList()
        {
            _updatedChunkCoordinates.Clear();
        }

        private IEnumerable<Vector3I> GetNeighbors(Vector3I globalPosition)
        {
            return new List<Vector3I>
            {
                globalPosition + new Vector3I(1, 0, 0),
                globalPosition + new Vector3I(-1, 0, 0),
                globalPosition + new Vector3I(0, 1, 0),
                globalPosition + new Vector3I(0, -1, 0),
                globalPosition + new Vector3I(0, 0, 1),
                globalPosition + new Vector3I(0, 0, -1)
            };
        }

        private void IncrementNeighborsEmptyNeighborCount(Vector3I globalPosition)
        {
            IEnumerable<Vector3I> neighborPositions = GetNeighbors(globalPosition);

            foreach (Vector3I neighborPosition in neighborPositions)
            {
                if (PositionIsInsideWorld(neighborPosition))
                {
                    IEnumerable<(Vector3I ChunkPosition, Vector3I VoxelPosition)> positions =
                        CalculateVoxelPositions(neighborPosition);

                    foreach ((Vector3I ChunkPosition, Vector3I VoxelPosition) position in positions)
                    {
                        _chunks[position.ChunkPosition][position.VoxelPosition].EmptyNeighborCount++;
                    }
                }
            }
        }

        private void DecrementNeighborsEmptyNeighborCount(Vector3I globalPosition)
        {
            IEnumerable<Vector3I> neighborPoitions = GetNeighbors(globalPosition);

            foreach (Vector3I neighborPosition in neighborPoitions)
            {
                if (PositionIsInsideWorld(neighborPosition))
                {
                    IEnumerable<(Vector3I ChunkPosition, Vector3I VoxelPosition)> positions =
                        CalculateVoxelPositions(neighborPosition);

                    foreach ((Vector3I ChunkPosition, Vector3I VoxelPosition) position in positions)
                    {
                        _chunks[position.ChunkPosition][position.VoxelPosition].EmptyNeighborCount--;
                    }
                }
            }
        }

        private IList<(Vector3I ChunkPosition, Vector3I VoxelPosition)> CalculateVoxelPositions(Vector3I globalPosition)
        {
            List<(Vector3I ChunkPosition, Vector3I VoxelPosition)> positions = new List<(Vector3I ChunkPosition, Vector3I VoxelPosition)>();

            Vector3I chunkPosition = CalculateChunkPosition(globalPosition);
            Vector3I voxelPosition = CalculateVoxelPositionInChunk(globalPosition, chunkPosition);
            positions.Add((chunkPosition, voxelPosition));

            Vector3I afterChunkPosition;
            Vector3I afterVoxelPosition;
            if (voxelPosition.X == 0 && ChunkIsInsideWorld((chunkPosition - new Vector3I(1, 0, 0))))
            {
                foreach ((Vector3I ChunkPosition, Vector3I VoxelPosition) position in positions.ToArray())
                {
                    afterChunkPosition = position.ChunkPosition - new Vector3I(1, 0, 0);
                    afterVoxelPosition = position.VoxelPosition;
                    afterVoxelPosition.X = Constant.ChunkSizeX;
                    positions.Add((afterChunkPosition, afterVoxelPosition));
                }
            }
            if (voxelPosition.Y == 0 && ChunkIsInsideWorld((chunkPosition - new Vector3I(0, 1, 0))))
            {
                foreach ((Vector3I ChunkPosition, Vector3I VoxelPosition) position in positions.ToArray())
                {
                    afterChunkPosition = position.ChunkPosition - new Vector3I(0, 1, 0);
                    afterVoxelPosition = position.VoxelPosition;
                    afterVoxelPosition.Y = Constant.ChunkSizeY;
                    positions.Add((afterChunkPosition, afterVoxelPosition));
                }
            }
            if (voxelPosition.Z == 0 && ChunkIsInsideWorld((chunkPosition - new Vector3I(0, 0, 1))))
            {
                foreach ((Vector3I ChunkPosition, Vector3I VoxelPosition) position in positions.ToArray())
                {
                    afterChunkPosition = position.ChunkPosition - new Vector3I(0, 0, 1);
                    afterVoxelPosition = position.VoxelPosition;
                    afterVoxelPosition.Z = Constant.ChunkSizeZ;
                    positions.Add((afterChunkPosition, afterVoxelPosition));
                }
            }

            return positions;
        }

        /// <summary>
        /// Get Positive offset between including 0.0 and excluding 1.0
        /// </summary>
        /// <param name="rayPosition"></param>
        /// <returns></returns>
        private Vector3 CalculateRayOffset(Vector3 rayPosition)
        {
            Vector3 rayOffset = rayPosition - (Vector3I)rayPosition;
            if (rayOffset.X < 0)
            {
                rayOffset.X += 1;
                if (rayOffset.X >= 1.0f)
                {
                    rayOffset.X = 0.0f;
                }
            }
            if (rayOffset.Y < 0)
            {
                rayOffset.Y += 1;
                if (rayOffset.Y >= 1.0f)
                {
                    rayOffset.Y = 0.0f;
                }
            }
            if (rayOffset.Z < 0)
            {
                rayOffset.Z += 1;
                if (rayOffset.Z >= 1.0f)
                {
                    rayOffset.Z = 0.0f;
                }
            }
            return rayOffset;
        }

        private Vector3 RayStep(Vector3 rayPosition, Vector3 rayDirection)
        {
            Vector3 rayOffset = CalculateRayOffset(rayPosition);

            Vector3 stepFactors = new Vector3(1.0f);

            if (rayDirection.X < 0)
            {
                stepFactors.X = (rayOffset.X > 0.0f ? rayOffset.X : 1.0f) / Math.Abs(rayDirection.X);
            }
            if (rayDirection.X > 0)
            {
                stepFactors.X = (1 - rayOffset.X) / Math.Abs(rayDirection.X);
            }

            if (rayDirection.Y < 0)
            {
                stepFactors.Y = (rayOffset.Y > 0.0f ? rayOffset.Y : 1.0f) / Math.Abs(rayDirection.Y);
            }
            if (rayDirection.Y > 0)
            {
                stepFactors.Y = (1 - rayOffset.Y) / Math.Abs(rayDirection.Y);
            }

            if (rayDirection.Z < 0)
            {
                stepFactors.Z = (rayOffset.Z > 0.0f ? rayOffset.Z : 1.0f) / Math.Abs(rayDirection.Z);
            }
            if (rayDirection.Z > 0)
            {
                stepFactors.Z = (1 - rayOffset.Z) / Math.Abs(rayDirection.Z);
            }

            float stepFactor = stepFactors.X < stepFactors.Y ? stepFactors.X : stepFactors.Y;
            stepFactor = stepFactor < stepFactors.Z ? stepFactor : stepFactors.Z;

            return rayPosition + rayDirection * stepFactor;
        }

        private Vector3I CalculateCurrentVoxelPosition(Vector3 rayPosition, Vector3 rayDirection)
        {
            Vector3I position = new Vector3I();
            if (rayDirection.X >= 0)
            {
                position.X = (int)rayPosition.X;
            }
            else
            {
                position.X = (int)Math.Ceiling((float)rayPosition.X) - 1;
            }

            if (rayDirection.Y >= 0)
            {
                position.Y = (int)rayPosition.Y;
            }
            else
            {
                position.Y = (int)Math.Ceiling((float)rayPosition.Y) - 1;
            }

            if (rayDirection.Z >= 0)
            {
                position.Z = (int)rayPosition.Z;
            }
            else
            {
                position.Z = (int)Math.Ceiling((float)rayPosition.Z) - 1;
            }

            return position;
        }

        private bool CalculateRayStartPosition(ref Vector3 rayStartPosition, Vector3 rayDirection)
        {
            Vector3I negativeWorldSize = ((-WorldSize / 2) * Constant.ChunkSize);
            negativeWorldSize.Y = 0;
            Vector3I positiveWorldSize = ((WorldSize / 2) * Constant.ChunkSize);
            positiveWorldSize.Y = WorldSize.Y * Constant.ChunkSizeY;

            if (PositionIsInsideWorld(rayStartPosition))
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
                    if (PositionIsInsideWorld(collision))
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
                    if (PositionIsInsideWorld(collision))
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
                    if (PositionIsInsideWorld(collision))
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
                    if (PositionIsInsideWorld(collision))
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
                    if (PositionIsInsideWorld(collision))
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
                    if (PositionIsInsideWorld(collision))
                    {
                        rayStartPosition = collision;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool PositionIsInsideWorld(Vector3 position)
        {
            Vector3I negativeWorldSize = ((-WorldSize / 2) * Constant.ChunkSize);
            negativeWorldSize.Y = 0;
            Vector3I positiveWorldSize = ((WorldSize / 2) * Constant.ChunkSize);
            positiveWorldSize.Y = WorldSize.Y * Constant.ChunkSizeY;
            return !(position < negativeWorldSize) && !(position > positiveWorldSize);
        }

        private bool PositionIsInsideWorld(Vector3I globalPosition)
        {
            Vector3I negativeWorldSize = ((-WorldSize / 2) * Constant.ChunkSize);
            negativeWorldSize.Y = 0;
            Vector3I positiveWorldSize = ((WorldSize / 2) * Constant.ChunkSize);
            positiveWorldSize.Y = WorldSize.Y * Constant.ChunkSizeY;
            return !(globalPosition < negativeWorldSize) && !(globalPosition >= positiveWorldSize);
        }

        private bool PositionIsUnderWorld(Vector3I globalPosition)
        {
            Vector3I negativeWorldSize = ((-WorldSize / 2) * Constant.ChunkSize);
            negativeWorldSize.Y = 0;
            Vector3I positiveWorldSize = ((WorldSize / 2) * Constant.ChunkSize);
            positiveWorldSize.Y = WorldSize.Y * Constant.ChunkSizeY;

            return !(globalPosition.X < negativeWorldSize.X || globalPosition.X > positiveWorldSize.X ||
                globalPosition.Z < negativeWorldSize.Z || globalPosition.Z > positiveWorldSize.Z ||
                globalPosition.Y >= negativeWorldSize.Y);
        }
        private bool ChunkIsInsideWorld(Vector3I position)
        {
            Vector3I negativeWorldSize = -(WorldSize / 2);
            negativeWorldSize.Y = 0; /*minimum height = 0*/
            Vector3I positiveWorldSize = WorldSize + negativeWorldSize;
            return !(position < negativeWorldSize) && !(position >= positiveWorldSize);
        }


        private void InitializeChunks()
        {
            Vector3I position = -(WorldSize / 2);
            position.Y = 0; /*minimum height = 0*/
            Vector3I initialPosition = position;
            Vector3I worldSize = WorldSize + position;
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

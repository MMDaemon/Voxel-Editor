using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;
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
                Vector3I chunkPosition = CalculateChunkPosition(globalPosition);
                Vector3I voxelPosition = CalculateVoxelPositionInChunk(globalPosition, chunkPosition);

                bool existsBefore = _chunks[chunkPosition][voxelPosition].Exists;

                if (_chunks[chunkPosition][voxelPosition].Amount + amount <= Constant.MaxMaterialAmount)
                {
                    IList<(Vector3I ChunkPosition, Vector3I VoxelPosition)> positions =
                        CalculateVoxelPositions(globalPosition);

                    if (_chunks[chunkPosition][voxelPosition].MaterialId == Constant.MaterialAir)
                    {
                        foreach ((Vector3I ChunkPosition, Vector3I VoxelPosition) position in positions)
                        {
                            _chunks[position.ChunkPosition][position.VoxelPosition].MaterialId = materialId;
                        }
                    }
                    if (_chunks[chunkPosition][voxelPosition].MaterialId == materialId)
                    {
                        foreach ((Vector3I ChunkPosition, Vector3I VoxelPosition) position in positions)
                        {
                            _chunks[position.ChunkPosition][position.VoxelPosition].AddMaterial(amount);
                            if (!_updatedChunkCoordinates.Contains(position.ChunkPosition))
                            {
                                _updatedChunkCoordinates.Add(position.ChunkPosition);
                            }
                        }

                        //Update NeighborChunk if neccessairy
                        foreach (Vector3I neighborPosition in GetNeighbors(globalPosition))
                        {
                            if (PositionIsInsideWorld(neighborPosition))
                            {
                                IList<(Vector3I ChunkPosition, Vector3I VoxelPosition)> neighborPositions =
                                    CalculateVoxelPositions(neighborPosition);
                                foreach ((Vector3I ChunkPosition, Vector3I VoxelPosition) position in neighborPositions)
                                {
                                    if (!_updatedChunkCoordinates.Contains(position.ChunkPosition))
                                    {
                                        _updatedChunkCoordinates.Add(position.ChunkPosition);
                                    }
                                }
                            }
                        }

                        success = true;
                    }
                }

                if (!existsBefore && _chunks[chunkPosition][voxelPosition].Exists)
                {
                    _chunks[chunkPosition].UsedVoxelAmount++;
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

                bool existsBefore = _chunks[chunkPosition][voxelPosition].Exists;

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

                    //Update NeighborChunk if neccessairy
                    foreach (Vector3I neighborPosition in GetNeighbors(globalPosition))
                    {
                        if (PositionIsInsideWorld(neighborPosition))
                        {
                            IList<(Vector3I ChunkPosition, Vector3I VoxelPosition)> neighborPositions =
                            CalculateVoxelPositions(neighborPosition);
                            foreach ((Vector3I ChunkPosition, Vector3I VoxelPosition) position in neighborPositions)
                            {
                                if (!_updatedChunkCoordinates.Contains(position.ChunkPosition))
                                {
                                    _updatedChunkCoordinates.Add(position.ChunkPosition);
                                }
                            }
                        }
                    }

                    success = true;
                    if (existsBefore && !_chunks[chunkPosition][voxelPosition].Exists)
                    {
                        _chunks[chunkPosition].UsedVoxelAmount--;
                        IncrementNeighborsEmptyNeighborCount(globalPosition);
                    }
                }
            }


            return success;
        }

        public bool RaytraceFilledVoxel(Vector3 rayPosition, Vector3 rayDirection, out Vector3I voxelPosition)
        {
            bool success = _voxelMarcher.InitializeStartPosition(rayPosition, rayDirection);
            voxelPosition = new Vector3I(-1);

            while (success && !PositionIsUnderWorld(_voxelMarcher.VoxelPosition) && GetVoxel(_voxelMarcher.VoxelPosition).IsEmpty)
            {
                success = _voxelMarcher.CalculateNextPosition();
            }
            if (success && !PositionIsUnderWorld(_voxelMarcher.VoxelPosition) && !GetVoxel(_voxelMarcher.VoxelPosition).IsEmpty)
            {
                voxelPosition = _voxelMarcher.VoxelPosition;
            }
            else
            {
                success = false;
            }
            return success;
        }

        public bool RaytraceEmptyOnFilledVoxel(Vector3 rayPosition, Vector3 rayDirection, out Vector3I voxelPosition)
        {
            bool success = _voxelMarcher.InitializeStartPosition(rayPosition, rayDirection);
            voxelPosition = new Vector3I(-1);

            bool emptySet = false;

            while (success && !PositionIsUnderWorld(_voxelMarcher.VoxelPosition) && GetVoxel(_voxelMarcher.VoxelPosition).IsEmpty)
            {
                emptySet = true;
                voxelPosition = _voxelMarcher.VoxelPosition;

                success = _voxelMarcher.CalculateNextPosition();
            }
            if (!(success && emptySet && (PositionIsUnderWorld(_voxelMarcher.VoxelPosition) ||
                                        !GetVoxel(_voxelMarcher.VoxelPosition).IsEmpty)))
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
                        bool existsBefore = _chunks[position.ChunkPosition][position.VoxelPosition].Exists;
                        _chunks[position.ChunkPosition][position.VoxelPosition].EmptyNeighborCount++;
                        if (!existsBefore && _chunks[position.ChunkPosition][position.VoxelPosition].Exists)
                        {
                            DecrementNeighborsEmptyNeighborCount(neighborPosition);
                        }
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
                        bool existsBefore = _chunks[position.ChunkPosition][position.VoxelPosition].Exists;
                        _chunks[position.ChunkPosition][position.VoxelPosition].EmptyNeighborCount--;
                        if (existsBefore && !_chunks[position.ChunkPosition][position.VoxelPosition].Exists)
                        {
                            IncrementNeighborsEmptyNeighborCount(neighborPosition);
                        }
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

        public void SaveWorld()
        {
            XElement world = new XElement("World");

            foreach (var chunk in _chunks.Values)
            {
                world.Add(XElement.Parse(chunk.ToString()));
            }

            XDocument doc = new XDocument(world);

            doc.Save(@"save.xml");
        }

        public void LoadWorld()
        {

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

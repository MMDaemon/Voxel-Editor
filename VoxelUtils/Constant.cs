namespace VoxelUtils
{
    public static class Constant
    {
        public const int MaxMaterialAmount = 128;
        public const int ChunkSizeX = 32;
        public const int ChunkSizeY = 32;
        public const int ChunkSizeZ = 32;
        public const int MaxVoxelsPerChunk = ChunkSizeX * ChunkSizeY * ChunkSizeZ;

        public static readonly Vector3I ChunkSize = new Vector3I(ChunkSizeX, ChunkSizeY, ChunkSizeZ);
    }
}

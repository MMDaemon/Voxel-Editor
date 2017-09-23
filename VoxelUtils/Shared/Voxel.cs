namespace VoxelUtils.Shared
{
    public class Voxel
    {
        public int MaterialId { get; set; }
        public int Amount { get; private set; }
        public int EmptyNeighborCount { get; set; }
        public float FillingQuantity => (float)Amount / Constant.MaxMaterialAmount;

        public bool Exists => FillingQuantity > 1.0f / (2 * EmptyNeighborCount);

        public Voxel(int materialId, int amount, int emptyNeighborCount = 6)
        {
            MaterialId = materialId;
            Amount = amount;
            EmptyNeighborCount = emptyNeighborCount;
        }

        public void AddMaterial(int amount)
        {
            Amount += amount;
        }

        public void TakeMaterial(int amount)
        {
            Amount -= amount;
        }
    }
}

using System.Collections.Generic;
using System.Numerics;
using DMS.Geometry;
using VoxelUtils;
using VoxelUtils.Shared;

namespace VoxelEditor.View.MeshGeberation
{
    internal class ChunkMesh : Mesh
    {
        public ChunkMesh(Chunk chunk)
        {
        }

        private MarchingCubesVoxel CreateMarchingCubesVoxel(Chunk chunk, Vector3I position)
        {
            List<Voxel> voxels = new List<Voxel>();

            voxels.Add(chunk[position]);
            voxels.Add(chunk[position+new Vector3I(1,0,0)]);
            voxels.Add(chunk[position + new Vector3I(1, 1, 0)]);
            voxels.Add(chunk[position + new Vector3I(0, 1, 0)]);
            voxels.Add(chunk[position + new Vector3I(0, 0, 1)]);
            voxels.Add(chunk[position + new Vector3I(1, 0, 1)]);
            voxels.Add(chunk[position + new Vector3I(1, 1, 1)]);
            voxels.Add(chunk[position + new Vector3I(0, 1, 1)]);

            return new MarchingCubesVoxel(voxels);
        }
    }
}

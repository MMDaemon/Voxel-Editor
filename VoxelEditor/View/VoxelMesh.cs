using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Zenseless.Geometry;

namespace VoxelEditor.View
{
    internal class VoxelMesh
    {
        public DefaultMesh DefaultMesh { get; private set; }
        public List<Vector3> TexCoord3D { get; }
        public List<uint> VoxelId { get; }
        public List<float> MaterialAmount { get; }
        public List<int> MaterialId { get; }

        public VoxelMesh()
        {
            DefaultMesh = new DefaultMesh();
            TexCoord3D = new List<Vector3>();
            MaterialAmount = new List<float>();
            MaterialId = new List<int>();
            VoxelId = new List<uint>();
        }

        private VoxelMesh(VoxelMesh voxelMesh, DefaultMesh defaultMesh)
        {
            DefaultMesh = defaultMesh;
            TexCoord3D = voxelMesh.TexCoord3D;
            MaterialAmount = voxelMesh.MaterialAmount;
            MaterialId = voxelMesh.MaterialId;
            VoxelId = voxelMesh.VoxelId;
        }

        public void Add(VoxelMesh voxelMesh)
        {
            DefaultMesh.Add(voxelMesh.DefaultMesh);
            TexCoord3D.AddRange(voxelMesh.TexCoord3D);
            MaterialAmount.AddRange(voxelMesh.MaterialAmount);
            MaterialId.AddRange(voxelMesh.MaterialId);
            uint max = 0;
            if (VoxelId.Count > 0)
            {
                max = (uint)(VoxelId.Last() + 1);
            }
            foreach (var voxelId in voxelMesh.VoxelId)
            {
                VoxelId.Add(voxelId + max);
            }
        }

        public VoxelMesh Transform(Matrix4x4 transform)
        {
            return new VoxelMesh(this, DefaultMesh.Transform(transform));
        }
    }
}

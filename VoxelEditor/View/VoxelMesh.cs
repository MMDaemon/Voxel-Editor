using System.Collections.Generic;
using System.Numerics;
using Zenseless.Geometry;

namespace VoxelEditor.View
{
    internal class VoxelMesh
    {
        public DefaultMesh DefaultMesh { get; private set; }
        public List<Vector3> TexCoord3D { get; }
        public List<float> MaterialAmount { get; }

        private readonly int _materialCount;

        public VoxelMesh(int materialCount)
        {
            DefaultMesh = new DefaultMesh();
            TexCoord3D = new List<Vector3>();
            MaterialAmount = new List<float>();

            _materialCount = materialCount;
        }

        private VoxelMesh(VoxelMesh voxelMesh, DefaultMesh defaultMesh)
        {
            DefaultMesh = defaultMesh;
            TexCoord3D = voxelMesh.TexCoord3D;
            MaterialAmount = voxelMesh.MaterialAmount;
        }

        public void Add(DefaultMesh defaultMesh)
        {
            DefaultMesh.Add(defaultMesh);
            if (defaultMesh.TexCoord.Count > 0)
            {
                foreach (var texCoord in defaultMesh.TexCoord)
                {
                    TexCoord3D.Add(new Vector3(texCoord.X, texCoord.Y, texCoord.Y));

                    for (int i = 0; i < _materialCount; i++)
                    {
                        MaterialAmount.Add(i == 0 ? 1 : 0);
                    }
                }
            }
            else
            {
                foreach (var position in defaultMesh.Position)
                {
                    TexCoord3D.Add(new Vector3(position.X, position.Y, position.Z));

                    for (int i = 0; i < _materialCount; i++)
                    {
                        MaterialAmount.Add(i == 0 ? 1 : 0);
                    }
                }
            }

        }

        public void Add(VoxelMesh voxelMesh)
        {
            DefaultMesh.Add(voxelMesh.DefaultMesh);
            TexCoord3D.AddRange(voxelMesh.TexCoord3D);
            MaterialAmount.AddRange(voxelMesh.MaterialAmount);
        }

        public VoxelMesh Transform(Matrix4x4 transform)
        {
            return new VoxelMesh(this, DefaultMesh.Transform(transform));
        }
    }
}

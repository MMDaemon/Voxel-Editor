using System.Collections.Generic;
using System.Numerics;
using MVCCore.Interfaces;
using VoxelUtils.Shared;

namespace VoxelEditor.ViewModel
{
    internal class EditorViewModel : IViewModel
    {
        public Matrix4x4 CameraMatrix { get; private set; }

        public List<Chunk> Chunks { get; private set; }
        public float VoxelSize { get; private set; }

        public Vector3 TestPosition { get; private set; }
        public EditorViewModel(Matrix4x4 cameraMatrix, List<Chunk> chunks, float voxelSize, Vector3 testPosition)
        {
            CameraMatrix = cameraMatrix;
            Chunks = chunks;
            VoxelSize = voxelSize;
            TestPosition = testPosition;
            
        }
    }
}

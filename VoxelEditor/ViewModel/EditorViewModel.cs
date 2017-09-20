using System.Collections.Generic;
using System.Numerics;
using MVCCore.Interfaces;
using VoxelUtils;
using VoxelUtils.Shared;

namespace VoxelEditor.ViewModel
{
    internal class EditorViewModel : IViewModel
    {
        private Vector3I _rayTraceCollisionPosition;

        public Matrix4x4 CameraMatrix { get; private set; }

        public IEnumerable<Chunk> Chunks { get; private set; }
        public float VoxelSize { get; private set; }

        public bool RaytraceCollided { get; private set; }
        public Vector3 RayTraceCollisionPosition => (Vector3)_rayTraceCollisionPosition * VoxelSize;

        public EditorViewModel(Matrix4x4 cameraMatrix, IEnumerable<Chunk> chunks, float voxelSize, Vector3I rayTraceCollisionPosition, bool raytraceCollided)
        {
            CameraMatrix = cameraMatrix;
            Chunks = chunks;
            VoxelSize = voxelSize;
            _rayTraceCollisionPosition = rayTraceCollisionPosition;
            RaytraceCollided = raytraceCollided;
        }
    }
}

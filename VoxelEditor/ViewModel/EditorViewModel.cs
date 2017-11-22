using System.Collections.Generic;
using System.Numerics;
using MVCCore.Interfaces;
using VoxelUtils;
using VoxelUtils.Shared;

namespace VoxelEditor.ViewModel
{
    internal class EditorViewModel : IViewModel
    {
        private readonly Vector3I _raytraceVoxelPosition;

        public Matrix4x4 CameraMatrix { get; private set; }

        public IEnumerable<Chunk> Chunks { get; private set; }
        public float VoxelSize { get; private set; }

        public Vector3I WorldSize { get; private set; }

        public bool RaytraceCollided { get; private set; }
        public Vector3 RaytraceVoxelPosition => (Vector3)_raytraceVoxelPosition * VoxelSize;
        public Vector3 RaytraceHitPosition { get; private set; }

        public EditorViewModel(Matrix4x4 cameraMatrix, IEnumerable<Chunk> chunks, float voxelSize, Vector3I worldSize, Vector3I raytraceVoxelPosition, Vector3 raytraceHitPosition, bool raytraceCollided)
        {
            CameraMatrix = cameraMatrix;
            Chunks = chunks;
            VoxelSize = voxelSize;
            WorldSize = worldSize;
            _raytraceVoxelPosition = raytraceVoxelPosition;
            RaytraceHitPosition = raytraceHitPosition;
            RaytraceCollided = raytraceCollided;
        }
    }
}

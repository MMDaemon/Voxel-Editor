﻿using System.Collections.Generic;
using System.Numerics;
using MVCCore.Interfaces;
using VoxelUtils;
using VoxelUtils.Shared;

namespace VoxelEditor.ViewModel
{
    internal class EditorViewModel : IViewModel
    {
        public float Time { get; private set; }
        public Vector3 CameraPosition { get; private set; }
        public Matrix4x4 CameraMatrix { get; private set; }

        public IEnumerable<Chunk> Chunks { get; private set; }
        public float VoxelSize { get; private set; }

        public Vector3I WorldSize { get; private set; }

        public int MaterialId { get; private set; }
        public int MaterialAmount { get; private set; }

        public Voxel RaytracedVoxel { get; private set; }
        public Vector3 RaytraceVoxelPosition { get; private set; }

        public EditorViewModel(float time, Vector3 cameraPosition, Matrix4x4 cameraMatrix, IEnumerable<Chunk> chunks, float voxelSize, Vector3I worldSize, int materialId, int materialAmount, Vector3I raytraceVoxelPosition, Voxel raytracedVoxel)
        {
            Time = time;

            CameraPosition = cameraPosition;
            CameraMatrix = cameraMatrix;

            Chunks = chunks;
            VoxelSize = voxelSize;

            WorldSize = worldSize;

            MaterialId = materialId;
            MaterialAmount = materialAmount;

            RaytraceVoxelPosition = (Vector3)raytraceVoxelPosition * VoxelSize;
            RaytracedVoxel = raytracedVoxel;
        }
    }
}

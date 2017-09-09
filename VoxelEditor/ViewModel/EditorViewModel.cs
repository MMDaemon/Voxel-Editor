using System.Numerics;
using MVCCore.Interfaces;

namespace VoxelEditor.ViewModel
{
    internal class EditorViewModel : IViewModel
    {
        public Matrix4x4 CameraMatrix { get; private set; }
        public Vector3 TestPosition { get; private set; }
        public EditorViewModel(Matrix4x4 cameraMatrix, Vector3 testPosition)
        {
            CameraMatrix = cameraMatrix;
            TestPosition = testPosition;
        }
    }
}

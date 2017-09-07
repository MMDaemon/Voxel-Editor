using System.Numerics;
using MVCCore.Interfaces;

namespace VoxelEditor.ViewModel
{
    internal class EditorViewModel : IViewModel
    {
        public Vector3 PlayerPosition { get; private set; }
        public Vector2 PlayerRotation { get; private set; }
        public EditorViewModel(Vector3 playerPosition, Vector2 playerRotation)
        {
            PlayerPosition = playerPosition;
            PlayerRotation = playerRotation;
        }
    }
}

using System.Numerics;
using DMS.Geometry;

namespace VoxelEditor.Model
{
    internal class Player
    {
        public Vector3 Position { get; private set; }
        public Vector2 Rotation { get; private set; }

        public Player()
        {
            Position = Vector3.Zero;
            Rotation = Vector2.Zero;
        }

        public void Move(Vector3 velocity, float timeDelta)
        {
            velocity = GetVectorAfterRotation(velocity);
            Position += velocity * timeDelta;
        }

        public void Rotate(Vector2 rotation)
        {
            Rotation += rotation;
        }

        public Vector3 GetVectorAfterRotation(Vector3 vector, float addRotationX = 0.0f, float addRotationY = 0.0f)
        {
            Matrix4x4 rotX = Matrix4x4.Transpose(Matrix4x4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X+addRotationX)));
            Matrix4x4 rotY = Matrix4x4.Transpose(Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y+addRotationY)));
            Matrix4x4 rotation = rotX * rotY;
            return Vector3.Transform(vector, rotation);
        }
    }
}

using System;
using System.Numerics;
using DMS.Geometry;

namespace VoxelUtils.Common.Visual
{
	public class CameraPerspective
	{
		private float _fovY;
		private Vector3 _position;

		public CameraPerspective()
		{
			Aspect = 1;
			FarClip = 2;
			FovY = 90;
			Jaw = 0;
			NearClip = 0.1f;
			Position = Vector3.Zero;
			Pitch = 0;
		}

		public float Aspect { get; set; }
		public float Jaw { get; set; }
		public float Pitch { get; set; }
		public float FarClip { get; set; }
		public float FovY { get { return _fovY; } set { _fovY = MathHelper.Clamp(value, 0f, 179.9f); } }
		public float NearClip { get; set; }
		public Vector3 Position { get { return _position; } set { _position = value; } }
		public float PositionX { get { return Position.X; } set { _position.X = value; } }
		public float PositionY { get { return Position.Y; } set { _position.Y = value; } }
		public float PositionZ { get { return Position.Z; } set { _position.Z = value; } }

		public Matrix4x4 CalcViewMatrix()
		{
			var mtxPitch = Matrix4x4.Transpose(Matrix4x4.CreateRotationX(MathHelper.DegreesToRadians(Pitch)));
			var mtxJaw = Matrix4x4.Transpose(Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(Jaw)));
			var mtxPosition = Matrix4x4.Transpose(Matrix4x4.CreateTranslation(-Position));
			return mtxPitch * mtxJaw * mtxPosition;
		}

		public Matrix4x4 CalcProjectionMatrix()
		{
			return Matrix4x4.Transpose(Matrix4x4.CreatePerspectiveFieldOfView(
				MathHelper.DegreesToRadians(FovY),
				Aspect, NearClip, FarClip));
		}

		public Matrix4x4 CalcMatrix()
		{
			return CalcProjectionMatrix() * CalcViewMatrix();
		}

		public Vector3 CalcPosition()
		{
			var view = CalcViewMatrix();
			Matrix4x4 inverse;
			if (!Matrix4x4.Invert(view, out inverse)) throw new ArithmeticException("Could not invert matrix");
			return new Vector3(inverse.M14, inverse.M24, inverse.M34);
		}
	}
}

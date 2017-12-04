using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VoxelUtils
{
    public class Vector3D
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Z { get; set; }

        public Vector3D(decimal x, decimal y, decimal z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3D(decimal value = 0) : this(value, value, value) { }
        public Vector3D(Vector3D vector) : this(vector.X, vector.Y, vector.Z) { }

        public override string ToString() => $"({X},{Y},{Z})";

        public static Vector3D operator -(Vector3D vec) => new Vector3D(-vec.X, -vec.Y, -vec.Z);
        public static Vector3D operator +(Vector3D left, Vector3D right) => new Vector3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        public static Vector3D operator -(Vector3D left, Vector3D right) => new Vector3D(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        public static Vector3D operator *(Vector3D left, Vector3D right) => new Vector3D(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        public static Vector3D operator /(Vector3D left, Vector3D right) => new Vector3D(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
        public static Vector3D operator %(Vector3D left, Vector3D right) => new Vector3D(left.X % right.X, left.Y % right.Y, left.Z % right.Z);
        public static bool operator <(Vector3D left, Vector3D right) => (left.X < right.X || left.Y < right.Y || left.Z < right.Z);
        public static bool operator <=(Vector3D left, Vector3D right) => (left.X <= right.X || left.Y <= right.Y || left.Z <= right.Z);
        public static bool operator >(Vector3D left, Vector3D right) => (left.X > right.X || left.Y > right.Y || left.Z > right.Z);
        public static bool operator >=(Vector3D left, Vector3D right) => (left.X >= right.X || left.Y >= right.Y || left.Z >= right.Z);

        public static Vector3D operator *(Vector3D vec, decimal value) => new Vector3D(vec.X * value, vec.Y * value, vec.Z * value);
        public static Vector3D operator /(Vector3D vec, decimal value) => new Vector3D(vec.X / value, vec.Y / value, vec.Z / value);
        public static Vector3D operator %(Vector3D vec, decimal value) => new Vector3D(vec.X % value, vec.Y % value, vec.Z % value);

        public static explicit operator Vector3(Vector3D vec) => new Vector3((float)vec.X, (float)vec.Y, (float)vec.Z);
        public static explicit operator Vector3I(Vector3D vec) => new Vector3I((int)vec.X, (int)vec.Y, (int)vec.Z);
        public static explicit operator Vector3D(Vector3 vec) => new Vector3D((decimal)vec.X, (decimal)vec.Y, (decimal)vec.Z);
    }
}

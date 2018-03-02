using System;
using System.Numerics;

namespace VoxelUtils
{
    public class Vector3M
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Z { get; set; }

        public Vector3M(decimal x, decimal y, decimal z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3M(decimal value = 0) : this(value, value, value) { }
        public Vector3M(Vector3M vector) : this(vector.X, vector.Y, vector.Z) { }

        public override string ToString() => $"({X},{Y},{Z})";

        public decimal this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    default:
                        throw new IndexOutOfRangeException();
                }

            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public static Vector3M operator -(Vector3M vec) => new Vector3M(-vec.X, -vec.Y, -vec.Z);
        public static Vector3M operator +(Vector3M left, Vector3M right) => new Vector3M(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        public static Vector3M operator -(Vector3M left, Vector3M right) => new Vector3M(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        public static Vector3M operator *(Vector3M left, Vector3M right) => new Vector3M(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        public static Vector3M operator /(Vector3M left, Vector3M right) => new Vector3M(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
        public static Vector3M operator %(Vector3M left, Vector3M right) => new Vector3M(left.X % right.X, left.Y % right.Y, left.Z % right.Z);
        public static bool operator <(Vector3M left, Vector3M right) => (left.X < right.X || left.Y < right.Y || left.Z < right.Z);
        public static bool operator <=(Vector3M left, Vector3M right) => (left.X <= right.X || left.Y <= right.Y || left.Z <= right.Z);
        public static bool operator >(Vector3M left, Vector3M right) => (left.X > right.X || left.Y > right.Y || left.Z > right.Z);
        public static bool operator >=(Vector3M left, Vector3M right) => (left.X >= right.X || left.Y >= right.Y || left.Z >= right.Z);

        public static Vector3M operator *(Vector3M vec, decimal value) => new Vector3M(vec.X * value, vec.Y * value, vec.Z * value);
        public static Vector3M operator /(Vector3M vec, decimal value) => new Vector3M(vec.X / value, vec.Y / value, vec.Z / value);
        public static Vector3M operator %(Vector3M vec, decimal value) => new Vector3M(vec.X % value, vec.Y % value, vec.Z % value);

        public static explicit operator Vector3(Vector3M vec) => new Vector3((float)vec.X, (float)vec.Y, (float)vec.Z);
        public static explicit operator Vector3I(Vector3M vec) => new Vector3I((int)vec.X, (int)vec.Y, (int)vec.Z);
        public static explicit operator Vector3M(Vector3 vec) => new Vector3M((decimal)vec.X, (decimal)vec.Y, (decimal)vec.Z);

        public static implicit operator Vector3M(Vector3I vec) => new Vector3M(vec.X, vec.Y, vec.Z);

        public Vector3M Floor() => new Vector3M(Math.Floor(X), Math.Floor(Y), Math.Floor(Z));
        public Vector3M Abs() => new Vector3M(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
    }
}

using System;
using System.Numerics;

namespace VoxelUtils
{
    public struct Vector3I
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Vector3I(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3I(int value = 0) : this(value, value, value) { }
        public Vector3I(Vector3I vector) : this(vector.X, vector.Y, vector.Z) { }

        public override string ToString() => $"({X},{Y},{Z})";

        public int this[int index]
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

        public static Vector3I operator -(Vector3I vec) => new Vector3I(-vec.X, -vec.Y, -vec.Z);
        public static Vector3I operator +(Vector3I left, Vector3I right) => new Vector3I(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        public static Vector3I operator -(Vector3I left, Vector3I right) => new Vector3I(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        public static Vector3I operator *(Vector3I left, Vector3I right) => new Vector3I(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        public static Vector3I operator /(Vector3I left, Vector3I right) => new Vector3I(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
        public static Vector3I operator %(Vector3I left, Vector3I right) => new Vector3I(left.X % right.X, left.Y % right.Y, left.Z % right.Z);
        public static bool operator <(Vector3I left, Vector3I right) => (left.X < right.X || left.Y < right.Y || left.Z < right.Z);
        public static bool operator <=(Vector3I left, Vector3I right) => (left.X <= right.X || left.Y <= right.Y || left.Z <= right.Z);
        public static bool operator >(Vector3I left, Vector3I right) => (left.X > right.X || left.Y > right.Y || left.Z > right.Z);
        public static bool operator >=(Vector3I left, Vector3I right) => (left.X >= right.X || left.Y >= right.Y || left.Z >= right.Z);

        public static Vector3I operator *(Vector3I vec, int value) => new Vector3I(vec.X * value, vec.Y * value, vec.Z * value);
        public static Vector3I operator /(Vector3I vec, int value) => new Vector3I(vec.X / value, vec.Y / value, vec.Z / value);

        public static Vector3 operator +(Vector3 left, Vector3I right) => new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        public static Vector3 operator -(Vector3 left, Vector3I right) => new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

        public static bool operator <(Vector3 left, Vector3I right) => (left.X < right.X || left.Y < right.Y || left.Z < right.Z);
        public static bool operator <(Vector3I left, Vector3 right) => (left.X < right.X || left.Y < right.Y || left.Z < right.Z);
        public static bool operator <=(Vector3 left, Vector3I right) => (left.X <= right.X || left.Y <= right.Y || left.Z <= right.Z);
        public static bool operator <=(Vector3I left, Vector3 right) => (left.X <= right.X || left.Y <= right.Y || left.Z <= right.Z);
        public static bool operator >(Vector3 left, Vector3I right) => (left.X > right.X || left.Y > right.Y || left.Z > right.Z);
        public static bool operator >(Vector3I left, Vector3 right) => (left.X > right.X || left.Y > right.Y || left.Z > right.Z);
        public static bool operator >=(Vector3 left, Vector3I right) => (left.X >= right.X || left.Y >= right.Y || left.Z >= right.Z);
        public static bool operator >=(Vector3I left, Vector3 right) => (left.X >= right.X || left.Y >= right.Y || left.Z >= right.Z);

        public static implicit operator Vector3(Vector3I vec) => new Vector3(vec.X, vec.Y, vec.Z);
        public static explicit operator Vector3I(Vector3 vec) => new Vector3I((int)vec.X, (int)vec.Y, (int)vec.Z);
    }
}
using System.Numerics;

namespace VoxelUtils
{
    public struct Vector3I
    {
        public int X;
        public int Y;
        public int Z;

        public Vector3I(int value = 0)
        {
            X = value;
            Y = value;
            Z = value;
        }

        public Vector3I(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3I operator +(Vector3I left, Vector3I right)
        {
            return new Vector3I(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vector3I operator -(Vector3I left, Vector3I right)
        {
            return new Vector3I(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static Vector3I operator -(Vector3I vec)
        {
            return new Vector3I(-vec.X, -vec.Y, -vec.Z);
        }

        public static bool operator <(Vector3I left, Vector3I right)
        {
            return (left.X < right.X || left.Y < right.Y || left.Z < right.Z);
        }

        public static bool operator <=(Vector3I left, Vector3I right)
        {
            return (left.X <= right.X || left.Y <= right.Y || left.Z <= right.Z);
        }

        public static bool operator >(Vector3I left, Vector3I right)
        {
            return (left.X > right.X || left.Y > right.Y || left.Z > right.Z);
        }

        public static bool operator >=(Vector3I left, Vector3I right)
        {
            return (left.X >= right.X || left.Y >= right.Y || left.Z >= right.Z);
        }

        public static Vector3I operator *(Vector3I left, Vector3I right)
        {
            return new Vector3I(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        }

        public static Vector3I operator *(Vector3I vec, int value)
        {
            return new Vector3I(vec.X * value, vec.Y * value, vec.Z * value);
        }

        public static Vector3I operator /(Vector3I left, Vector3I right)
        {
            return new Vector3I(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
        }
        public static Vector3I operator /(Vector3I left, int value)
        {
            return new Vector3I(left.X / value, left.Y / value, left.Z / value);
        }

        public static Vector3I operator %(Vector3I left, Vector3I right)
        {
            return new Vector3I(left.X % right.X, left.Y % right.Y, left.Z % right.Z);
        }

        public static explicit operator Vector3(Vector3I vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }

        public static Vector3 operator -(Vector3 left, Vector3I right)
        {
            return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static bool operator <(Vector3 left, Vector3I right)
        {
            return (left.X < right.X || left.Y < right.Y || left.Z < right.Z);
        }

        public static bool operator <(Vector3I left, Vector3 right)
        {
            return (left.X < right.X || left.Y < right.Y || left.Z < right.Z);
        }

        public static bool operator <=(Vector3 left, Vector3I right)
        {
            return (left.X <= right.X || left.Y <= right.Y || left.Z <= right.Z);
        }

        public static bool operator <=(Vector3I left, Vector3 right)
        {
            return (left.X <= right.X || left.Y <= right.Y || left.Z <= right.Z);
        }

        public static bool operator >(Vector3 left, Vector3I right)
        {
            return (left.X > right.X || left.Y > right.Y || left.Z > right.Z);
        }

        public static bool operator >(Vector3I left, Vector3 right)
        {
            return (left.X > right.X || left.Y > right.Y || left.Z > right.Z);
        }

        public static bool operator >=(Vector3 left, Vector3I right)
        {
            return (left.X >= right.X || left.Y >= right.Y || left.Z >= right.Z);
        }

        public static bool operator >=(Vector3I left, Vector3 right)
        {
            return (left.X >= right.X || left.Y >= right.Y || left.Z >= right.Z);
        }

        public static explicit operator Vector3I(Vector3 vec)
        {
            return new Vector3I((int)vec.X, (int)vec.Y, (int)vec.Z);
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }
    }
}
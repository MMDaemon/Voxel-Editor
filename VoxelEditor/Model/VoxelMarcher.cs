using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using VoxelUtils;

namespace VoxelEditor.Model
{
    internal class VoxelMarcher
    {
        private Vector3I _position;
        private Vector3D _distanceToNext;
        private bool colliding;

        private Vector3I _worldSize;

        public VoxelMarcher(Vector3I worldSize, Vector3 rayStartPosition, Vector3 rayDirection)
        {
            _position = new Vector3I();
            _distanceToNext = new Vector3D();
            _worldSize = worldSize;
            colliding = CalculateStartPosition(rayStartPosition, rayDirection);
        }

        public bool CalculatePathToCollision(out List<Vector3I> positions)
        {
            positions = new List<Vector3I>();
            if (colliding)
            {
                
            }
            return colliding;
        }

        private bool CalculateStartPosition(Vector3 rayStartPosition, Vector3 rayDirection)
        {
            Vector3I negativeWorldSize = ((-_worldSize / 2) * Constant.ChunkSize);
            negativeWorldSize.Y = 0;
            Vector3I positiveWorldSize = ((_worldSize / 2) * Constant.ChunkSize);
            positiveWorldSize.Y = _worldSize.Y * Constant.ChunkSizeY;

            if (PositionIsInsideWorld(rayStartPosition))
            {
                SetStartPosition(rayStartPosition, rayDirection);
                return true;
            }
            if (rayStartPosition.X < negativeWorldSize.X)
            {
                if (GetRayColissionWithPlane(rayStartPosition, rayDirection, new Vector3(negativeWorldSize.X, 0, 0), new Vector3(-1, 0, 0), out Vector3 collision) && PositionIsInsideWorld(collision))
                {
                    SetStartPosition(collision, rayDirection);
                    return true;
                }
            }
            if (rayStartPosition.X > positiveWorldSize.X)
            {
                if (GetRayColissionWithPlane(rayStartPosition, rayDirection, new Vector3(positiveWorldSize.X, 0, 0), new Vector3(1, 0, 0), out Vector3 collision) && PositionIsInsideWorld(collision))
                {
                    SetStartPosition(collision, rayDirection);
                    return true;
                }
            }
            if (rayStartPosition.Y < negativeWorldSize.Y)
            {
                if (GetRayColissionWithPlane(rayStartPosition, rayDirection, new Vector3(0, negativeWorldSize.Y, 0), new Vector3(0, -1, 0), out Vector3 collision) && PositionIsInsideWorld(collision))
                {
                    SetStartPosition(collision, rayDirection);
                    return true;
                }
            }
            if (rayStartPosition.Y > positiveWorldSize.Y)
            {
                if (GetRayColissionWithPlane(rayStartPosition, rayDirection, new Vector3(0, positiveWorldSize.Y, 0), new Vector3(0, 1, 0), out Vector3 collision) && PositionIsInsideWorld(collision))
                {
                    SetStartPosition(collision, rayDirection);
                    return true;
                }
            }
            if (rayStartPosition.Z < negativeWorldSize.Z)
            {
                if (GetRayColissionWithPlane(rayStartPosition, rayDirection, new Vector3(0, 0, negativeWorldSize.Z), new Vector3(0, 0, -1), out Vector3 collision) && PositionIsInsideWorld(collision))
                {
                    SetStartPosition(collision, rayDirection);
                    return true;
                }
            }
            if (rayStartPosition.Z > positiveWorldSize.Z)
            {
                if (GetRayColissionWithPlane(rayStartPosition, rayDirection, new Vector3(0, 0, positiveWorldSize.Z), new Vector3(0, 0, 1), out Vector3 collision) && PositionIsInsideWorld(collision))
                {
                    SetStartPosition(collision, rayDirection);
                    return true;
                }
            }
            return false;
        }

        private void SetStartPosition(Vector3 position, Vector3 rayDirection)
        {
            Vector3D fraction = (Vector3D)position;
            _position = (Vector3I)fraction;
            Vector3D offset = fraction % 1.0m;

            if (position.X < 0.0f)
            {
                _position.X--;
                offset.X++;
            }
            if (position.Y < 0.0f)
            {
                _position.Y--;
                offset.Y++;
            }
            if (position.Z < 0.0f)
            {
                _position.Z--;
                offset.Z++;
            }

            if (rayDirection.X >= 0.0f)
            {
                if (offset.X >= 1.0m)
                {
                    _position.X++;
                    offset.X = 0.0m;
                }
                _distanceToNext.X = 1.0m - offset.X;
            }
            else
            {
                if (offset.X <= 0.0m)
                {
                    _position.X--;
                    offset.X = 1.0m;
                }
                _distanceToNext.X = offset.X;
            }

            if (rayDirection.Y >= 0.0f)
            {
                if (offset.Y >= 1.0m)
                {
                    _position.Y++;
                    offset.Y = 0.0m;
                }
                _distanceToNext.Y = 1.0m - offset.Y;
            }
            else
            {
                if (offset.Y <= 0.0m)
                {
                    _position.Y--;
                    offset.Y = 1.0m;
                }
                _distanceToNext.Y = offset.Y;
            }

            if (rayDirection.Z >= 0.0f)
            {
                if (offset.Z >= 1.0m)
                {
                    _position.Z++;
                    offset.Z = 0.0m;
                }
                _distanceToNext.Z = 1.0m - offset.Z;
            }
            else
            {
                if (offset.Z <= 0.0m)
                {
                    _position.Z--;
                    offset.Z = 1.0m;
                }
                _distanceToNext.Z = offset.Z;
            }
        }

        private bool GetRayColissionWithPlane(Vector3 rayPosition, Vector3 rayDirection, Vector3 planePosition,
            Vector3 planeNormal, out Vector3 collision)
        {
            collision = Vector3.Zero;

            float denominator = Vector3.Dot(rayDirection, planeNormal);
            if (Math.Abs(denominator) > 0.0f)
            {
                float d = Vector3.Dot((planePosition - rayPosition), planeNormal) / denominator;
                collision = rayPosition + rayDirection * d;
                return true;
            }
            return false;
        }

        private bool PositionIsInsideWorld(Vector3 position)
        {
            Vector3I negativeWorldSize = ((-_worldSize / 2) * Constant.ChunkSize);
            negativeWorldSize.Y = 0;
            Vector3I positiveWorldSize = ((_worldSize / 2) * Constant.ChunkSize);
            positiveWorldSize.Y = _worldSize.Y * Constant.ChunkSizeY;
            return !(position < negativeWorldSize) && !(position > positiveWorldSize);
        }

    }
}

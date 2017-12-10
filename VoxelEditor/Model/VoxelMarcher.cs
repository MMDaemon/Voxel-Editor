using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using VoxelUtils;

namespace VoxelEditor.Model
{
    internal class VoxelMarcher
    {
        private Vector3I _worldSize;
        private bool _colliding;
        private Vector3M _marchingDirection;
        private Vector3I _position;
        private Vector3M _distanceToNext;

        public Vector3I Position => _position;

        public Vector3 GlobalPosition
        {
            get
            {
                Vector3M offset = new Vector3M();
                for (int i = 0; i <= 2; i++)
                {
                    if (_distanceToNext[i] == 1.0m)
                    {
                        offset[i] = 0.0m;
                    }
                    else
                    {
                        offset[i] = _distanceToNext[i] * Math.Sign(_marchingDirection[i]);
                    }
                }
                return new Vector3((float)(_position.X + offset.X), (float)(_position.Y + offset.Y), (float)(_position.Z + offset.Z));
            }
        }

        public VoxelMarcher(Vector3I worldSize)
        {
            _worldSize = worldSize;
            _colliding = false;
            _position = new Vector3I();
            _distanceToNext = new Vector3M();

        }

        public bool InitializeStartPosition(Vector3 rayStartPosition, Vector3 rayDirection)
        {
            _marchingDirection = (Vector3M)rayDirection;
            _colliding = CalculateStartPosition(rayStartPosition, rayDirection);
            return _colliding;
        }

        public bool CalculateNextPosition()
        {
            if (_colliding)
            {
                _distanceToNext -= _marchingDirection.Abs() * CalculateCheapestStep();
                for (int i = 0; i <= 2; i++)
                {
                    if (_distanceToNext[i] == 0.0m)
                    {
                        _distanceToNext[i] = 1.0m;
                        _position[i] += Math.Sign(_marchingDirection[i]);
                    }
                }
            }
            return _colliding;
        }

        private decimal CalculateCheapestStep()
        {
            int dir = 0;
            for (int i = 1; i <= 2; i++)
            {
                if (_distanceToNext[i] / Math.Abs(_marchingDirection[i]) < _distanceToNext[dir] / Math.Abs(_marchingDirection[dir]))
                {
                    dir = i;
                }
            }
            return _distanceToNext[dir] / Math.Abs(_marchingDirection[dir]);
        }

        private bool CalculateStartPosition(Vector3 rayStartPosition, Vector3 rayDirection)
        {
            Vector3I negativeWorldSize = ((-_worldSize / 2) * Constant.ChunkSize);
            negativeWorldSize.Y = 0;
            Vector3I positiveWorldSize = ((_worldSize / 2) * Constant.ChunkSize);
            positiveWorldSize.Y = _worldSize.Y * Constant.ChunkSizeY;

            if (PositionIsInsideWorld(rayStartPosition))
            {
                SetStartPosition(rayStartPosition);
                return true;
            }
            if (rayStartPosition.X < negativeWorldSize.X)
            {
                if (GetRayColissionWithPlane(rayStartPosition, rayDirection, new Vector3(negativeWorldSize.X, 0, 0), new Vector3(-1, 0, 0), out Vector3 collision) && PositionIsInsideWorld(collision))
                {
                    SetStartPosition(collision);
                    return true;
                }
            }
            if (rayStartPosition.X > positiveWorldSize.X)
            {
                if (GetRayColissionWithPlane(rayStartPosition, rayDirection, new Vector3(positiveWorldSize.X, 0, 0), new Vector3(1, 0, 0), out Vector3 collision) && PositionIsInsideWorld(collision))
                {
                    SetStartPosition(collision);
                    return true;
                }
            }
            if (rayStartPosition.Y < negativeWorldSize.Y)
            {
                if (GetRayColissionWithPlane(rayStartPosition, rayDirection, new Vector3(0, negativeWorldSize.Y, 0), new Vector3(0, -1, 0), out Vector3 collision) && PositionIsInsideWorld(collision))
                {
                    SetStartPosition(collision);
                    return true;
                }
            }
            if (rayStartPosition.Y > positiveWorldSize.Y)
            {
                if (GetRayColissionWithPlane(rayStartPosition, rayDirection, new Vector3(0, positiveWorldSize.Y, 0), new Vector3(0, 1, 0), out Vector3 collision) && PositionIsInsideWorld(collision))
                {
                    SetStartPosition(collision);
                    return true;
                }
            }
            if (rayStartPosition.Z < negativeWorldSize.Z)
            {
                if (GetRayColissionWithPlane(rayStartPosition, rayDirection, new Vector3(0, 0, negativeWorldSize.Z), new Vector3(0, 0, -1), out Vector3 collision) && PositionIsInsideWorld(collision))
                {
                    SetStartPosition(collision);
                    return true;
                }
            }
            if (rayStartPosition.Z > positiveWorldSize.Z)
            {
                if (GetRayColissionWithPlane(rayStartPosition, rayDirection, new Vector3(0, 0, positiveWorldSize.Z), new Vector3(0, 0, 1), out Vector3 collision) && PositionIsInsideWorld(collision))
                {
                    SetStartPosition(collision);
                    return true;
                }
            }
            return false;
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

        private void SetStartPosition(Vector3 position)
        {
            Vector3M fraction = (Vector3M)position;
            _position = (Vector3I)fraction.Floor();
            _distanceToNext = fraction - _position;

            for (int i = 0; i <= 2; i++)
            {
                if (_marchingDirection[i] < 0)
                {
                    if (_distanceToNext[i] == 0.0m)
                    {
                        _position[i]--;
                    }
                    _distanceToNext[i] = 1 - _distanceToNext[i];
                }
                else if (_distanceToNext[i] == 0)
                {
                    _distanceToNext[i] = 1;
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MVCCore.Interfaces;
using VoxelEditor.ViewModel;
using VoxelUtils;
using VoxelUtils.Enums;
using VoxelUtils.Registry.Model;
using VoxelUtils.Visual;

namespace VoxelEditor.Model
{
    public class EditorModel : ModelRegistryContainer, IModel
    {
        private const float VoxelSize = 1.0f / Constant.ChunkSizeX;

        private ModelRegistry _registry;
        private readonly Random _random;

        private float _lastUpdateTime;
        private float _deltaTime;
        private Vector2 _lastMousePosition;
        private Vector2 _deltaMousePosition;

        private readonly CameraPerspective _camera;

        private readonly Player _player;
        private readonly World _world;

        private bool _raytraceCollided;
        private Vector3I _raytraceVoxelPosition;
        private Vector3 _raytraceHitPosition;

        public IViewModel ViewModel => CreateViewModel();

        public event EventHandler ModelEvent;
        public event StateChangedHandler StateChanged;

        public EditorModel(IModelRegistry registry) : base(registry)
        {
            _registry = (ModelRegistry)registry;
            _random = new Random();

            _lastUpdateTime = 0.0f;
            _lastMousePosition = Vector2.Zero;

            _camera = new CameraPerspective { FarClip = 100 };

            _player = new Player(Vector3.UnitY);
            _world = new World(new Vector3I(4, 4, 4));
            //TestInitVoxels();
        }

        public void Update(float absoluteTime, ModelInput input)
        {
            _deltaTime = absoluteTime - _lastUpdateTime;
            _lastUpdateTime = absoluteTime;

            _deltaMousePosition = input.MousePosition - _lastMousePosition;
            _lastMousePosition = input.MousePosition;

            HandleKeyActions(input.KeyActions.Cast<KeyAction>().ToList());
        }

        public void Resize(int width, int height)
        {
            _camera.Aspect = (float)width / height;
        }

        private void HandleKeyActions(ICollection<KeyAction> keyActions)
        {
            HandleMovement(keyActions);
            HandleRaytraceSelection(keyActions);
            HandleUpdateWorld(keyActions);

        }

        private void HandleMovement(ICollection<KeyAction> keyActions)
        {
            float speed = 2;

            if (keyActions.Contains(KeyAction.MoveUp))
            {
                _player.Move(Vector3.UnitY * speed, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveDown))
            {
                _player.Move(-Vector3.UnitY * speed, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveForwards))
            {
                _player.Move(-Vector3.UnitZ * speed, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveBackwards))
            {
                _player.Move(Vector3.UnitZ * speed, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveLeft))
            {
                _player.Move(-Vector3.UnitX * speed, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveRight))
            {
                _player.Move(Vector3.UnitX * speed, _deltaTime);
            }
            CalculatePlayerRotation();

            _camera.Position = (_player.Position - new Vector3(0.5f)) * VoxelSize;
            _camera.Pitch = _player.Rotation.X;
            _camera.Jaw = _player.Rotation.Y;
        }

        private void HandleRaytraceSelection(ICollection<KeyAction> keyActions)
        {
            if (keyActions.Contains(KeyAction.RayTraceEmpty))
            {
                _raytraceCollided = _world.RaytraceEmptyOnFilledVoxel(_player.Position, _player.GetVectorAfterRotation(-Vector3.UnitZ), out _raytraceVoxelPosition, out _raytraceHitPosition);
            }
            else
            {
                _raytraceCollided = _world.RaytraceFilledVoxel(_player.Position, _player.GetVectorAfterRotation(-Vector3.UnitZ), out _raytraceVoxelPosition, out _raytraceHitPosition);
            }
        }

        private void HandleUpdateWorld(ICollection<KeyAction> keyActions)
        {
            if (keyActions.Contains(KeyAction.PlaceMaterial) && _raytraceCollided)
            {
                _world.AddMaterial(1, Constant.MaxMaterialAmount, _raytraceVoxelPosition);
            }
            if (keyActions.Contains(KeyAction.TakeMaterial) && _raytraceCollided)
            {
                _world.TakeMaterial(1, Constant.MaxMaterialAmount, _raytraceVoxelPosition);
            }
        }

        private void CalculatePlayerRotation()
        {
            _player.Rotate(300 * new Vector2(-_deltaMousePosition.Y, _deltaMousePosition.X));
        }

        private Vector3 CalculateRaytraceDirection(Vector2 mousePosition)
        {
            Matrix4x4.Invert(_camera.CalcProjectionMatrix(), out var inverseProjection);
            Vector4 rayEye = Vector4.Transform(new Vector3(new Vector2(mousePosition.X / _camera.Aspect, mousePosition.Y), -1), inverseProjection);
            rayEye = new Vector4(rayEye.X, rayEye.Y, -1, 0);

            Vector4 rayWorld = Vector4.Transform(rayEye, _camera.CalcViewMatrix());
            rayWorld = Vector4.Normalize(rayWorld);

            return new Vector3(rayWorld.X, rayWorld.Y, rayWorld.Z);
        }

        private EditorViewModel CreateViewModel()
        {
            EditorViewModel viewModel = new EditorViewModel(_camera.CalcMatrix(), _world.UpdatedChunks, VoxelSize, _world.WorldSize, _raytraceVoxelPosition, (_raytraceHitPosition - new Vector3(0.5f)) * VoxelSize, _raytraceCollided);
            _world.ResetUpdateList();
            return viewModel;
        }

        private void TestInitVoxels()
        {
            for (int x = -2 * Constant.ChunkSizeX; x < 2 * Constant.ChunkSizeX; x++)
            {
                for (int z = -2 * Constant.ChunkSizeZ; z < 2 * Constant.ChunkSizeZ; z++)
                {
                    int y = (int)(_random.NextDouble() * 4 * Constant.ChunkSizeY);
                    _world.AddMaterial(1, Constant.MaxMaterialAmount, new Vector3I(x, y, z));
                }
            }
        }

        private void OnModelEvent()
        {
            ModelEvent?.Invoke(null, null);
        }

        private void OnStateChanged(State state, bool temporary)
        {
            StateChanged?.Invoke((int)state, temporary);
        }
    }
}

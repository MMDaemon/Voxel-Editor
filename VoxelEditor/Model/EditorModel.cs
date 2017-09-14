using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DMS.Geometry;
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
        private ModelRegistry _registry;
        private Random _random;

        private float _lastUpdateTime;
        private float _deltaTime;
        private Vector2 _lastMousePosition;
        private Vector2 _deltaMousePosition;

        private readonly CameraPerspective _camera;

        private readonly Player _player;
        private readonly World _world;

        private Vector3 _testPosition;

        public IViewModel ViewModel => CreateViewModel();

        public event EventHandler ModelEvent;
        public event StateChangedHandler StateChanged;

        public EditorModel(IModelRegistry registry) : base(registry)
        {
            _registry = (ModelRegistry)registry;
            _random = new Random();

            _lastUpdateTime = 0.0f;
            _lastMousePosition = Vector2.Zero;

            _camera = new CameraPerspective();

            _player = new Player();
            _world = new World();
            TestInitVoxels();
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
            HandleRaytraceSelection();
        }

        private void HandleMovement(ICollection<KeyAction> keyActions)
        {
            if (keyActions.Contains(KeyAction.MoveUp))
            {
                _player.Move(Vector3.UnitY, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveDown))
            {
                _player.Move(-Vector3.UnitY, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveForwards))
            {
                _player.Move(-Vector3.UnitZ, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveBackwards))
            {
                _player.Move(Vector3.UnitZ, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveLeft))
            {
                _player.Move(-Vector3.UnitX, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.MoveRight))
            {
                _player.Move(Vector3.UnitX, _deltaTime);
            }
            if (keyActions.Contains(KeyAction.EnableCameraRotation))
            {
                CalculatePlayerRotation();
            }

            _camera.Position = _player.Position;
            _camera.Pitch = _player.Rotation.X;
            _camera.Jaw = _player.Rotation.Y;
        }

        private void HandleRaytraceSelection()
        {
            _testPosition = _player.Position + CalculateRaytraceDirection(_lastMousePosition);
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
            EditorViewModel viewModel = new EditorViewModel(_camera.CalcMatrix(), _world.Chunks, _world.VoxelSize, _testPosition);

            return viewModel;
        }

        private void TestInitVoxels()
        {
            for (int x = -2 * Constant.ChunkSizeX; x < 2 * Constant.ChunkSizeX; x++)
            {
                for (int z = -2 * Constant.ChunkSizeZ; z < 2 * Constant.ChunkSizeZ; z++)
                {
                    int y = (int)(_random.NextDouble() * Constant.ChunkSizeY);
                    _world.AddMaterial(1, 1, new Vector3I(x, y, z));
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DMS.Geometry;
using MVCCore.Interfaces;
using VoxelEditor.ViewModel;
using VoxelUtils.Enums;
using VoxelUtils.Registry.Model;
using VoxelUtils.Visual;

namespace VoxelEditor.Model
{
    public class EditorModel : ModelRegistryContainer, IModel
    {
        private float _lastUpdateTime;
        private float _deltaTime;
        private Vector2 _lastMousePosition;
        private Vector2 _deltaMousePosition;
        private readonly CameraPerspective _camera;

        private readonly Player _player;
        private ModelRegistry _registry;

        private Vector3 _testPosition;

        public IViewModel ViewModel => CreateViewModel();

        public event EventHandler ModelEvent;
        public event StateChangedHandler StateChanged;

        public EditorModel(IModelRegistry registry) : base(registry)
        {
            _registry = (ModelRegistry)registry;
            _lastUpdateTime = 0.0f;
            _lastMousePosition = Vector2.Zero;
            _player = new Player();
            _camera = new CameraPerspective();
        }

        public void Update(float absoluteTime, ModelInput input)
        {
            _deltaTime = absoluteTime - _lastUpdateTime;
            _lastUpdateTime = absoluteTime;
            _deltaMousePosition = input.MousePosition - _lastMousePosition;
            _lastMousePosition = input.MousePosition;
            HandleKeyActions(input.KeyActions.Cast<KeyAction>().ToList());
            _camera.Position = _player.Position;
            _camera.Pitch = _player.Rotation.X;
            _camera.Jaw = _player.Rotation.Y;
            _testPosition = _player.Position + CalculateRaytraceDirection(input.MousePosition);
        }

        public void Resize(int width, int height)
        {
            _camera.Aspect = (float)width / height;
        }

        private void HandleKeyActions(ICollection<KeyAction> keyActions)
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
            EditorViewModel viewModel = new EditorViewModel(_camera.CalcMatrix(), _testPosition);

            return viewModel;
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

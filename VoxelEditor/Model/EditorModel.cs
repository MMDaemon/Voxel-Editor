using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using MVCCore.Interfaces;
using VoxelEditor.ViewModel;
using VoxelUtils;
using VoxelUtils.Enums;
using VoxelUtils.Registry.Model;
using VoxelUtils.Visual;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace VoxelEditor.Model
{
    public class EditorModel : ModelRegistryContainer, IModel
    {
        private const float VoxelSize = 1.0f / Constant.ChunkSizeX;

        private readonly ModelRegistry _registry;
        private readonly Random _random;

        private float _lastUpdateTime;
        private float _deltaTime;
        private int _scrollWheelBefore;
        private bool _cursorLocked;
        private Point _screenCenter;
        private Vector2 _cursorDelta;

        private readonly CameraPerspective _camera;

        private readonly Player _player;
        private readonly World _world;
        private int _materialAmount;
        private int _materialID;

        private bool _raytraceCollided;
        private Vector3I _raytraceVoxelPosition;

        public IViewModel ViewModel => CreateViewModel();

        public event EventHandler ModelEvent;
        public event StateChangedHandler StateChanged;

        public EditorModel(IModelRegistry registry) : base(registry)
        {
            _registry = (ModelRegistry)registry;
            _random = new Random();

            _lastUpdateTime = 0.0f;
            _scrollWheelBefore = 0;
            _cursorLocked = true;

            _camera = new CameraPerspective { FarClip = 100 };

            _player = new Player(Vector3.UnitY);
            _world = new World(new Vector3I(2, 2, 2));
            _materialAmount = Constant.MaxMaterialAmount;
            _materialID = 1;
            //TestInitVoxels();
        }

        public void Update(float absoluteTime, ModelInput input)
        {
            _deltaTime = absoluteTime - _lastUpdateTime;
            _lastUpdateTime = absoluteTime;

            _screenCenter = input.ScreenCenter;

            if (_cursorLocked)
            {
                Point mouseDelta = Cursor.Position - new Size(_screenCenter);
                _cursorDelta = new Vector2((float)mouseDelta.X, -(float)mouseDelta.Y);
                Cursor.Position = _screenCenter;
            }
            else
            {
                _cursorDelta = new Vector2(0);
            }
            HandleScrolling(input.MouseState.Wheel);
            HandleKeyActions(input.KeyActions.Cast<KeyAction>().ToList());
        }

        public void Resize(int width, int height)
        {
            _camera.Aspect = (float)width / height;
        }

        private void HandleScrolling(int scrollWheel)
        {
            int difference = scrollWheel - _scrollWheelBefore;
            _scrollWheelBefore = scrollWheel;
            for (int i = 0; i < difference; i++)
            {
                if (_materialAmount < Constant.MaxMaterialAmount)
                {
                    _materialAmount *= 2;
                }
            }
            for (int i = 0; i > difference; i--)
            {
                if (_materialAmount > 1)
                {
                    _materialAmount /= 2;
                }
            }
        }

        private void HandleKeyActions(ICollection<KeyAction> keyActions)
        {
            HandleGuiActions(keyActions);
            HandleMovement(keyActions);
            HandleSelection(keyActions);
            HandleRaytraceSelection(keyActions);
            HandleUpdateWorld(keyActions);

        }

        private void HandleGuiActions(ICollection<KeyAction> keyActions)
        {
            List<KeyAction> guiKeyActions = new List<KeyAction>();
            if (keyActions.Contains(KeyAction.Exit))
            {
                if (!_cursorLocked)
                {
                    guiKeyActions.Add(KeyAction.Exit);
                }
                else
                {
                    _cursorLocked = false;
                    guiKeyActions.Add(KeyAction.ToggleCursorVisible);
                }
            }
            if (!_cursorLocked)
            {
                if (keyActions.Contains(KeyAction.PlaceMaterial))
                {
                    _cursorLocked = true;
                    guiKeyActions.Add(KeyAction.ToggleCursorVisible);
                    keyActions.Remove(KeyAction.PlaceMaterial);
                    Cursor.Position = _screenCenter;
                }
                if (keyActions.Contains(KeyAction.TakeMaterial))
                {
                    keyActions.Remove(KeyAction.TakeMaterial);
                }
            }

            if (keyActions.Contains(KeyAction.ToggleFullscreen))
            {
                guiKeyActions.Add(KeyAction.ToggleFullscreen);
            }
            if (guiKeyActions.Count > 0)
            {
                OnGuiKeyActionsSet(guiKeyActions);
            }
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

        private void HandleSelection(ICollection<KeyAction> keyActions)
        {
            if (keyActions.Contains(KeyAction.SelectMaterial))
            {
                _materialID = _materialID % _registry.MaterialCount + 1;
            }
        }

        private void HandleRaytraceSelection(ICollection<KeyAction> keyActions)
        {
            if (keyActions.Contains(KeyAction.RayTraceEmpty))
            {
                _raytraceCollided = _world.RaytraceEmptyOnFilledVoxel(_player.Position, _player.GetVectorAfterRotation(-Vector3.UnitZ), out _raytraceVoxelPosition);
            }
            else
            {
                _raytraceCollided = _world.RaytraceFilledVoxel(_player.Position, _player.GetVectorAfterRotation(-Vector3.UnitZ), out _raytraceVoxelPosition);
            }
        }

        private void HandleUpdateWorld(ICollection<KeyAction> keyActions)
        {
            if (keyActions.Contains(KeyAction.PlaceMaterial) && _raytraceCollided)
            {
                int addAmount = _materialAmount;
                int overhead = Constant.MaxMaterialAmount - (_world.GetVoxel(_raytraceVoxelPosition).Amount + _materialAmount);
                if (overhead < 0)
                {
                    addAmount += overhead;
                }
                _world.AddMaterial(_materialID, addAmount, _raytraceVoxelPosition);
            }
            if (keyActions.Contains(KeyAction.TakeMaterial) && _raytraceCollided)
            {
                int takeAmount = _materialAmount;
                if (_world.GetVoxel(_raytraceVoxelPosition).Amount < _materialAmount)
                {
                    takeAmount = _world.GetVoxel(_raytraceVoxelPosition).Amount;
                }
                _world.TakeMaterial(_materialID, takeAmount, _raytraceVoxelPosition);
            }
        }

        private void CalculatePlayerRotation()
        {
            _player.Rotate(new Vector2(-_cursorDelta.Y, _cursorDelta.X));
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
            EditorViewModel viewModel = new EditorViewModel(_camera.CalcMatrix(), _world.UpdatedChunks, VoxelSize, _world.WorldSize, _materialID, _materialAmount, _raytraceVoxelPosition, _raytraceCollided);
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

        private void OnGuiKeyActionsSet(ICollection<KeyAction> keyActions)
        {
            ModelEvent?.Invoke(this, new GuiKeyActionsEventArgs(keyActions));
        }

        private void OnStateChanged(State state, bool temporary)
        {
            StateChanged?.Invoke((int)state, temporary);
        }
    }
}

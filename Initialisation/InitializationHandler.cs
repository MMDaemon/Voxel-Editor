using MVCCore;
using MVCCore.Handlers;
using MVCCore.Interfaces;
using OpenTK;
using OpenTK.Input;
using VoxelEditor.Model;
using VoxelEditor.View;
using VoxelMenu.Model;
using VoxelMenu.View;
using VoxelUtils.Enums;
using VoxelUtils.Initialization;
using VoxelUtils.Registry.Model;
using VoxelUtils.Registry.View;

namespace Initialisation
{
    internal class InitializationHandler : IInitializationHandler
    {
        public int InitialState => (int)State.Start;
        private ModelRegistry _modelRegistry;
        private ViewRegistry _viewRegistry;

        public InitializationHandler()
        {
            MainController controller = new MainController(this);
        }

        public InputHandler InitializeInputHandler(GameWindow gameWindow)
        {
            InputHandler inputHandler = new InputHandler(gameWindow);
            inputHandler.AddPressKeyAction(Key.Escape, (int)KeyAction.Exit);
            inputHandler.AddPressKeyAction(Key.F11, (int)KeyAction.ToggleFullscreen);
            inputHandler.AddPressKeyAction(Key.E, (int)KeyAction.SelectMaterial);
            inputHandler.AddPressKeyAction(Key.F1, (int)KeyAction.Save);
            inputHandler.AddPressKeyAction(Key.F2, (int)KeyAction.Load);

            inputHandler.AddHoldKeyAction(Key.W, (int)KeyAction.MoveForwards);
            inputHandler.AddHoldKeyAction(Key.Up, (int)KeyAction.MoveForwards);
            inputHandler.AddHoldKeyAction(Key.S, (int)KeyAction.MoveBackwards);
            inputHandler.AddHoldKeyAction(Key.Down, (int)KeyAction.MoveBackwards);
            inputHandler.AddHoldKeyAction(Key.A, (int)KeyAction.MoveLeft);
            inputHandler.AddHoldKeyAction(Key.Left, (int)KeyAction.MoveLeft);
            inputHandler.AddHoldKeyAction(Key.D, (int)KeyAction.MoveRight);
            inputHandler.AddHoldKeyAction(Key.Right, (int)KeyAction.MoveRight);
            inputHandler.AddHoldKeyAction(Key.Space, (int)KeyAction.MoveUp);
            inputHandler.AddHoldKeyAction(Key.ShiftLeft, (int)KeyAction.MoveDown);
            inputHandler.AddHoldKeyAction(Key.Q, (int)KeyAction.RayTraceEmpty);

            inputHandler.AddPressKeyAction(MouseButton.Left, (int)KeyAction.PlaceMaterial);
            inputHandler.AddPressKeyAction(MouseButton.Right, (int)KeyAction.TakeMaterial);
            return inputHandler;
        }

        public StateHandler InitializeStateHandler()
        {
            StateHandler stateHandler = new StateHandler();
            stateHandler.AddStateInformation((int)State.Start, typeof(EditorModel), typeof(EditorView));
            stateHandler.AddStateInformation((int)State.Menu, typeof(MenuModel), typeof(MenuView));
            stateHandler.AddStateInformation((int)State.Editor, typeof(EditorModel), typeof(EditorView));
            return stateHandler;
        }

        public IModelRegistry InitalizeModelRegistry()
        {
            if (_modelRegistry == null)
            {
                InitializeRegistry();
            }
            return _modelRegistry;
        }

        public IViewRegistry InitializeViewRegistry()
        {
            if (_viewRegistry == null)
            {
                InitializeRegistry();
            }
            return _viewRegistry;
        }

        private void InitializeRegistry()
        {
            _modelRegistry = new ModelRegistry();
            _viewRegistry = new ViewRegistry();

            RegisterMaterial(1, new MaterialInfo("stone", Textures.Stone, new RenderProperties(), new MaterialBehavior()));
            RegisterMaterial(2, new MaterialInfo("dirt", Textures.Dirt, new RenderProperties(), new MaterialBehavior()));
            RegisterMaterial(3, new MaterialInfo("grass", Textures.Grass, new RenderProperties(), new MaterialBehavior()));
            RegisterMaterial(4, new MaterialInfo("sand", Textures.Sand, new RenderProperties(), new MaterialBehavior()));
        }

        public void RegisterMaterial(int id, MaterialInfo materialInfo)
        {
            _modelRegistry.RegisterMaterial(id, materialInfo);
            _viewRegistry.RegisterMaterial(id, materialInfo);
        }
    }
}

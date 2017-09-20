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
            inputHandler.AddHoldKeyAction(Key.ControlLeft, (int)KeyAction.EnableCameraRotation);
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

            //TODO regsiter Materials
        }

        public void RegisterMaterial(int id, MaterialInfo materialInfo)
        {
            _modelRegistry.RegisterMaterial(id, materialInfo.MaterialBehavior);
            _viewRegistry.RegisterMaterial(id, materialInfo.Texture, materialInfo.RenderProperties);
        }
    }
}

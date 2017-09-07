using OpenTK;
using OpenTK.Input;
using VoxelEditor.Common.Enums;
using VoxelEditor.Common.Initialization;
using VoxelEditor.Editor.Model;
using VoxelEditor.Editor.View;
using VoxelEditor.Menu.Model;
using VoxelEditor.Menu.View;
using VoxelEditor.MVCCore;
using VoxelEditor.MVCInterfaces;
using VoxelEditor.Registry.Model;
using VoxelEditor.Registry.View;

namespace VoxelEditor.Initialization
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
            inputHandler.AddHoldKeyAction(MouseButton.Right, (int)KeyAction.EnableCameraRotation);
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

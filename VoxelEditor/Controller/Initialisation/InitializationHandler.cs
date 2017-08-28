using VoxelEditor.Common.Enums;
using VoxelEditor.Model.Editor;
using VoxelEditor.Model.Menu;
using VoxelEditor.Model.Registry;
using VoxelEditor.View.Editor;
using VoxelEditor.View.Menu;
using VoxelEditor.View.Registry;

namespace VoxelEditor.Controller.Initialisation
{
	internal class InitializationHandler
	{
		private ModelRegistry _modelRegistry;
		private ViewRegistry _viewRegistry;

		public StateHandler InitializeStateHandler()
		{
			StateHandler stateHandler = new StateHandler();
			stateHandler.AddStateInformation(State.Start, typeof(EditorModel), typeof(EditorView));
			stateHandler.AddStateInformation(State.Menu, typeof(MenuModel), typeof(MenuView));
			stateHandler.AddStateInformation(State.Editor, typeof(EditorModel), typeof(EditorView));
			return stateHandler;
		}

		public ModelRegistry InitalizeModelRegistry()
		{
			if (_modelRegistry == null)
			{
				InitializeRegistry();
			}
			return _modelRegistry;
		}

		public ViewRegistry InitializeViewRegistry()
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

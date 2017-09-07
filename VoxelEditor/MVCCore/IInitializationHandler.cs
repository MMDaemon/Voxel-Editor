using OpenTK;
using VoxelEditor.MVCInterfaces;

namespace VoxelEditor.MVCCore
{
    public interface IInitializationHandler
    {
        int InitialState { get; }
        InputHandler InitializeInputHandler(GameWindow appGameWindow);
        StateHandler InitializeStateHandler();
        IModelRegistry InitalizeModelRegistry();
        IViewRegistry InitializeViewRegistry();
    }
}

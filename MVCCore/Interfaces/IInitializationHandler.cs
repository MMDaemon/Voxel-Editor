using MVCCore.Handlers;
using OpenTK;

namespace MVCCore.Interfaces
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

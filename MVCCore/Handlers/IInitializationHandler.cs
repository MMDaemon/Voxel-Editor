using MVCCore.Interfaces;
using OpenTK;

namespace MVCCore.Handlers
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

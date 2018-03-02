using System;
using OpenTK;
using Zenseless.Application;
using Zenseless.HLGL;

namespace MVCCore.Interfaces
{
    public delegate void GameWindowEventHandler(Action<GameWindow> gameWindowCall);

    public interface IView
    {
        void ShaderChanged(string name, IShader shader);

        void LoadResources(ResourceManager resourceManager);

        void Render(IViewModel viewModel);

        void Resize(int width, int height);
        void ProcessModelEvent(EventArgs e);

        event GameWindowEventHandler GameWindowEvent;
    }
}

using System;
using System.IO;
using DMS.Application;
using DMS.Base;
using DMS.OpenGL;
using MVCCore.Interfaces;

namespace VoxelMenu.View
{
    public class MenuView : IView
    {
        public event GameWindowEventHandler GameWindowEvent;

        private Shader _menuShader;

        public void ShaderChanged(string name, Shader shader)
        {
            if (nameof(_menuShader) != name) return;
            _menuShader = shader;
            if (ReferenceEquals(shader, null)) return;
            UpdateMesh();
        }

        public void LoadResources(ResourceManager resourceManager)
        {
            if (ReferenceEquals(null, resourceManager.GetShader(nameof(_menuShader))))
            {
                var dir = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + @"\Resources\";
                resourceManager.AddShader(nameof(_menuShader), dir + "vertex.glsl", dir + "fragment.glsl"
                    , Resourcen.vertex, Resourcen.fragment);
            }
        }

        public void Render(IViewModel viewModel)
        {
            //TODO implement
        }

        public void Resize(int width, int height)
        {
            //TODO implement
        }

        public void ProcessModelEvent(EventArgs e)
        {
            //TODO implement
        }

        private void UpdateMesh()
        {
            //TODO implement
        }

        private void ExitGame()
        {
            GameWindowEvent?.Invoke(gameWindow => gameWindow.Exit());
        }
    }
}

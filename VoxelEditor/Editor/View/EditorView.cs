using System;
using System.IO;
using DMS.Application;
using DMS.Base;
using DMS.OpenGL;
using MVCCore.Interfaces;
using VoxelEditor.Editor.ViewModel;
using VoxelUtils.Registry.View;

namespace VoxelEditor.Editor.View
{
    public class EditorView : ViewRegistryContainer, IView
    {
        private readonly ViewRegistry _registry;
        private readonly EditorVisual _visual;
        private readonly EditorSound _sound;

        public EditorView(IViewRegistry registry) : base(registry)
        {
            _registry = (ViewRegistry)registry;
            _visual = new EditorVisual();
            _sound = new EditorSound();
        }

        public void ProcessModelEvent(EventArgs e)
        {
            //TODO implement
        }

        public void LoadResources(ResourceManager resourceManager)
        {
            if (ReferenceEquals(null, resourceManager.GetShader(_visual.ShaderName)))
            {
                var dir = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + @"\Resources\";
                resourceManager.AddShader(_visual.ShaderName, dir + "vertex.glsl", dir + "fragment.glsl"
                    , Resourcen.vertex, Resourcen.fragment);
            }
        }

        public void ShaderChanged(string name, Shader shader)
        {
            _visual.ShaderChanged(name, shader);
        }

        public void Render(IViewModel viewModel)
        {
            _visual.Render((EditorViewModel)viewModel);
        }

        public void Resize(int width, int height)
        {
            _visual.Resize(width, height);
        }
    }
}

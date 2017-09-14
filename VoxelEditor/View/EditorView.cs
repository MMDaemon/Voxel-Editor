using System;
using System.IO;
using DMS.Application;
using DMS.Base;
using DMS.OpenGL;
using MVCCore.Interfaces;
using VoxelEditor.ViewModel;
using VoxelUtils.Registry.View;

namespace VoxelEditor.View
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
            if (ReferenceEquals(null, resourceManager.GetShader(_visual.VoxelShaderName)))
            {
                var dir = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + @"\Resources\";
                resourceManager.AddShader(_visual.VoxelShaderName, dir + "voxelVertex.glsl", dir + "voxelFragment.glsl"
                    , Resourcen.voxelVertex, Resourcen.voxelFragment);
                resourceManager.AddShader(_visual.RaytraceShaderName, dir + "raytraceVertex.glsl", dir + "raytraceFragment.glsl"
                    , Resourcen.raytraceVertex, Resourcen.raytraceFragment);
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

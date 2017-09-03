using System.IO;
using System.Numerics;
using DMS.Application;
using DMS.Base;
using DMS.OpenGL;
using OpenTK.Graphics.OpenGL;
using VoxelEditor.Common.Transfer;
using VoxelEditor.Common.Visual;
using VoxelEditor.Core.View;
using VoxelEditor.Registry.View;
using DMS.Geometry;

namespace VoxelEditor.Editor.View
{
	internal class EditorView : IView
	{
		private ViewRegistry _registry;
		private EditorVisual _visual;
		private EditorSound _sound;

		private Shader _editorShader;
		private readonly CameraPerspective _camera;
		private VAO _geometry;

		public EditorView(ViewRegistry registry)
		{
			_registry = registry;
			_visual = new EditorVisual();
			_sound = new EditorSound();
			_camera = new CameraPerspective
			{
				Position = new Vector3(0.5f, 0.5f, -0.5f),
				Jaw = 210,
				Pitch = 30
			};

			GL.Enable(EnableCap.DepthTest);
		}

		public void ProcessModelEvent()
		{
			//TODO implement
		}

		public void LoadResources(ResourceManager resourceManager)
		{
			if (ReferenceEquals(null, resourceManager.GetShader(nameof(_editorShader))))
			{
				var dir = Path.GetDirectoryName(PathTools.GetSourceFilePath()) + @"\Resources\";
				resourceManager.AddShader(nameof(_editorShader), dir + "vertex.glsl", dir + "fragment.glsl"
					, Resourcen.vertex, Resourcen.fragment);
			}
		}

		public void ShaderChanged(string name, Shader shader)
		{
			if (nameof(_editorShader) != name) return;
			_editorShader = shader;
			if (ReferenceEquals(shader, null)) return;
			UpdateMesh();
		}

		public void Render(ViewModel viewModel)
		{
			if (ReferenceEquals(_editorShader, null)) return;
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			_editorShader.Activate();
			float[] cam = _camera.CalcMatrix().ToArray();
			GL.UniformMatrix4(_editorShader.GetUniformLocation("camera"), 1, false, cam);
			_geometry.Draw();
			_editorShader.Deactivate();
		}
		private void UpdateMesh()
		{
			Mesh mesh = Meshes.CreateCubeWithNormals(0.2f);
			_geometry = VAOLoader.FromMesh(mesh, _editorShader);
		}
	}
}

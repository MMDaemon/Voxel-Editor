using System.Numerics;
using DMS.Geometry;
using DMS.OpenGL;
using OpenTK.Graphics.OpenGL;
using VoxelEditor.Common.Transfer;
using VoxelEditor.Common.Visual;

namespace VoxelEditor.Editor.View
{
	internal class EditorVisual
	{
		private Shader _editorShader;
		private readonly CameraPerspective _camera;
		private VAO _geometry;

		public string ShaderName => nameof(_editorShader);

		public EditorVisual()
		{
			_camera = new CameraPerspective
			{
				Position = new Vector3(0.5f, 0.5f, -0.5f),
				Jaw = 210,
				Pitch = 30
			};

			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.CullFace);
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
			_camera.Position = viewModel.PlayerPosition;

			if (ReferenceEquals(_editorShader, null)) return;
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			_editorShader.Activate();
			float[] cam = _camera.CalcMatrix().ToArray();
			GL.UniformMatrix4(_editorShader.GetUniformLocation("camera"), 1, false, cam);
			_geometry.Draw();
			_editorShader.Deactivate();
		}

		public void Resize(int width, int height)
		{
			_camera.Aspect = (float)width/height;
		}

		private void UpdateMesh()
		{
			Mesh mesh = Meshes.CreateCubeWithNormals(0.2f);
			_geometry = VAOLoader.FromMesh(mesh, _editorShader);
		}
	}
}

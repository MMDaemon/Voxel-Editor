using System.Numerics;
using DMS.Geometry;
using DMS.OpenGL;
using OpenTK.Graphics.OpenGL;
using VoxelEditor.ViewModel;

namespace VoxelEditor.View
{
	internal class EditorVisual
	{
		private Shader _editorShader;
		private VAO _geometry;

	    private Vector3 _testPos;

		public string ShaderName => nameof(_editorShader);

		public EditorVisual()
		{
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.CullFace);

            _testPos = Vector3.Zero;
		}

		public void ShaderChanged(string name, Shader shader)
		{
			if (nameof(_editorShader) != name) return;
			_editorShader = shader;
			if (ReferenceEquals(shader, null)) return;
			UpdateMesh();
		}

		public void Render(EditorViewModel viewModel)
		{
			_testPos = viewModel.TestPosition;
            UpdateMesh();

			if (ReferenceEquals(_editorShader, null)) return;
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			_editorShader.Activate();
			float[] cam = viewModel.CameraMatrix.ToArray();
			GL.UniformMatrix4(_editorShader.GetUniformLocation("camera"), 1, false, cam);
			_geometry.Draw();
			_editorShader.Deactivate();
		}

		public void Resize(int width, int height)
		{
			//do nothing because camera is defined in model
		}

		private void UpdateMesh()
		{
			Mesh mesh = Meshes.CreateCubeWithNormals(0.2f);
            mesh.Add(Meshes.CreateSphere(0.01f).Transform(Matrix4x4.CreateTranslation(_testPos)));
			_geometry = VAOLoader.FromMesh(mesh, _editorShader);
		}
	}
}

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DMS.Geometry;
using DMS.OpenGL;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using VoxelEditor.ViewModel;
using VoxelUtils;
using VoxelUtils.Shared;

namespace VoxelEditor.View
{
    internal class EditorVisual
    {
        private const string FragmentShaderAdd = @"
			#version 430 core
			uniform sampler2D image1;
            uniform sampler2D image2;
			in vec2 uv;
			void main() {
                vec4 texture1 = texture(image1, uv);
                vec4 texture2 = texture(image2, uv);
                float ratio = texture2.a / (texture1.a+texture2.a);
				gl_FragColor = mix(texture1, texture2, ratio);
			}";

        private Shader _voxelShader;
        private Shader _raytraceShader;

        private VAO _voxelGeometry;
        private VAO _raytraceGeometry;

        private int _screenWidth;
        private int _screenHeight;

        private float _voxelSize;
        private List<Vector3> _instancePositions;

        private Vector3 _testPos;

        public string VoxelShaderName => nameof(_voxelShader);
        public string RaytraceShaderName => nameof(_raytraceShader);

        public EditorVisual()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            _instancePositions = new List<Vector3>();
            _testPos = Vector3.Zero;
        }

        public void ShaderChanged(string name, Shader shader)
        {
            switch (name)
            {
                case nameof(_voxelShader):
                    _voxelShader = shader;
                    if (!ReferenceEquals(shader, null))
                    {
                        UpdateVoxelMesh();
                    }
                    break;
                case nameof(_raytraceShader):
                    _raytraceShader = shader;
                    if (!ReferenceEquals(shader, null))
                    {
                        UpdateRaytraceMesh();
                    }
                    break;
            }
        }

        public void Render(EditorViewModel viewModel)
        {
            float[] cam = viewModel.CameraMatrix.ToArray();
            _testPos = viewModel.TestPosition;

            Texture raytraceTexture = RenderRaytraceOnTexture(cam);

            _voxelSize = viewModel.VoxelSize;
            _instancePositions = CalculateVoxelPositions(viewModel.Chunks);

            Texture voxelTexture = RenderVoxelTexture(cam);

            Shader shader = ShaderLoader.FromStrings(TextureToFrameBuffer.VertexShaderScreenQuad,
                FragmentShaderAdd);

            TextureToFrameBuffer ttfb = new TextureToFrameBuffer();

            GL.Disable(EnableCap.DepthTest);

            shader.Activate();
            GL.ActiveTexture(TextureUnit.Texture0);
            voxelTexture.Activate();
            GL.Uniform1(shader.GetUniformLocation("image1"),TextureUnit.Texture0 -TextureUnit.Texture0);

            GL.ActiveTexture(TextureUnit.Texture1);
            raytraceTexture.Activate();
            GL.Uniform1(shader.GetUniformLocation("image2"), TextureUnit.Texture1 - TextureUnit.Texture0);

            GL.DrawArrays(PrimitiveType.Quads, 0, 4);

            raytraceTexture.Deactivate();
            voxelTexture.Deactivate();
            shader.Deactivate();

            GL.Enable(EnableCap.DepthTest);
        }

        public void Resize(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;
        }

        private Texture RenderVoxelTexture(float[] cam)
        {
            UpdateVoxelMesh();

            FBO renderToTexture = new FBO(Texture.Create(_screenWidth, _screenHeight));
            renderToTexture.Activate();

            GL.Color4(Color4.Transparent);
            _voxelShader.Activate();
            GL.UniformMatrix4(_voxelShader.GetUniformLocation("camera"), 1, false, cam);
            _voxelGeometry.Draw(_instancePositions.Count);
            _voxelShader.Deactivate();

            renderToTexture.Deactivate();
            return renderToTexture.Texture;
        }

        private Texture RenderRaytraceOnTexture(float[] cam)
        {
            UpdateRaytraceMesh();

            FBO renderToTexture = new FBO(Texture.Create(_screenWidth, _screenHeight));
            renderToTexture.Activate();

            GL.Color4(Color4.Transparent);
            _raytraceShader.Activate();
            GL.UniformMatrix4(_voxelShader.GetUniformLocation("camera"), 1, false, cam);
            _raytraceGeometry.Draw();
            _raytraceShader.Deactivate();

            renderToTexture.Deactivate();
            return renderToTexture.Texture;
        }

        private void UpdateVoxelMesh()
        {
            Mesh mesh = Meshes.CreateCubeWithNormals(_voxelSize);
            _voxelGeometry = VAOLoader.FromMesh(mesh, _voxelShader);

            _voxelGeometry.SetAttribute(_voxelShader.GetAttributeLocation("instancePosition"), _instancePositions.ToArray(), VertexAttribPointerType.Float, 3, true);
        }

        private void UpdateRaytraceMesh()
        {
            Mesh mesh = Meshes.CreateSphere(0.01f).Transform(Matrix4x4.CreateTranslation(_testPos));
            _raytraceGeometry = VAOLoader.FromMesh(mesh, _raytraceShader);
        }

        private List<Vector3> CalculateVoxelPositions(IEnumerable<Chunk> chunks)
        {
            List<Vector3> voxelPositions = new List<Vector3>();
            foreach (Chunk chunk in chunks)
            {
                for (int x = 0; x < Constant.ChunkSizeX; x++)
                {
                    for (int y = 0; y < Constant.ChunkSizeY; y++)
                    {
                        for (int z = 0; z < Constant.ChunkSizeZ; z++)
                        {
                            if (chunk[x, y, z] != null)
                            {
                                voxelPositions.Add(_voxelSize * (Vector3)(chunk.Position * Constant.ChunkSize + new Vector3I(x, y, z)));
                            }
                        }
                    }
                }
            }
            return voxelPositions;
        }
    }
}

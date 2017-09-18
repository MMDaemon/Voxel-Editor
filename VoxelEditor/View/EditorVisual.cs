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
        private Shader _addShader;

        private VAO _voxelGeometry;
        private VAO _raytraceGeometry;

        private int _screenWidth;
        private int _screenHeight;

        private float _voxelSize;
        private List<Vector3> _instancePositions;
        private bool _voxelsUpdated;

        private bool _raytraceCollided;
        private Vector3 _raytraceCollisionPosition;

        public string VoxelShaderName => nameof(_voxelShader);
        public string RaytraceShaderName => nameof(_raytraceShader);

        public EditorVisual()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            _instancePositions = new List<Vector3>();
            _raytraceCollided = false;
            _raytraceCollisionPosition = Vector3.Zero;
            _addShader = ShaderLoader.FromStrings(TextureToFrameBuffer.VertexShaderScreenQuad,
                FragmentShaderAdd);

            _voxelsUpdated = false;
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
            _raytraceCollided = viewModel.RaytraceCollided;
            _raytraceCollisionPosition = viewModel.RayTraceCollisionPosition;

            Texture raytraceTexture = RenderRaytraceOnTexture(cam);

            _voxelSize = viewModel.VoxelSize;
            _instancePositions = CalculateVoxelPositions(viewModel.Chunks);

            Texture voxelTexture = RenderVoxelTexture(cam);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _addShader.Activate();
            GL.ActiveTexture(TextureUnit.Texture0);
            voxelTexture.Activate();
            GL.Uniform1(_addShader.GetUniformLocation("image1"), TextureUnit.Texture0 - TextureUnit.Texture0);

            GL.ActiveTexture(TextureUnit.Texture1);
            raytraceTexture.Activate();
            GL.Uniform1(_addShader.GetUniformLocation("image2"), TextureUnit.Texture1 - TextureUnit.Texture0);

            GL.DrawArrays(PrimitiveType.Quads, 0, 4);

            raytraceTexture.Deactivate();
            voxelTexture.Deactivate();
            _addShader.Deactivate();
        }

        public void Resize(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;
        }

        private Texture RenderVoxelTexture(float[] cam)
        {
            UpdateVoxelMesh();
            _voxelsUpdated = true;

            FBO renderToTexture = new FBO(Texture.Create(_screenWidth, _screenHeight));
            renderToTexture.Activate();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
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

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (_raytraceCollided)
            {
                GL.Color4(Color4.Transparent);
                _raytraceShader.Activate();
                GL.UniformMatrix4(_voxelShader.GetUniformLocation("camera"), 1, false, cam);
                _raytraceGeometry.Draw();
                _raytraceShader.Deactivate();
            }

            renderToTexture.Deactivate();
            return renderToTexture.Texture;
        }

        private void UpdateVoxelMesh()
        {
            if (!_voxelsUpdated)
            {
                Mesh mesh = Meshes.CreateCubeWithNormals(_voxelSize);
                _voxelGeometry = VAOLoader.FromMesh(mesh, _voxelShader);

                _voxelGeometry.SetAttribute(_voxelShader.GetAttributeLocation("instancePosition"), _instancePositions.ToArray(), VertexAttribPointerType.Float, 3, true);
            }
        }

        private void UpdateRaytraceMesh()
        {
            Mesh mesh = Meshes.CreateSphere(_voxelSize).Transform(Matrix4x4.CreateTranslation(_raytraceCollisionPosition));
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
